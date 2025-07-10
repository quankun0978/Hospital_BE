using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital_BE.BLL.Interfaces;
using Hospital_BE.BLL.Models;
using Hospital_BE.DAL.Interfaces;
using Hospital_BE.DAL.Context;
using Hospital_BE.DAL.Models;
using Hospital_BE.PL.DTOs;
using Hospital_BE.PL.DTOs.Common;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace Hospital_BE.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserRepository _userRepository;

        public UserService(ApplicationDbContext context, IUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }

        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            return await _userRepository.ExistsByEmailAsync(email);
        }

        public async Task<PaginatedResult<User>> GetUsersAsync(QueryParameters parameters)
        {
            var (items, totalCount) = await _userRepository.GetAllAsync(parameters);
            return new PaginatedResult<User>(items, totalCount, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<List<UserDTO>> GetAllUsersAsync()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .ToListAsync();

            return users.Select(u => MapToUserDTO(u)).ToList();
        }

        public async Task<UserDTO> GetUserByIdAsync(string id)
        {
            if (!Guid.TryParse(id, out Guid userId))
            {
                return null;
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return null;

            return MapToUserDTO(user);
        }

        public async Task<UserDTO> GetUserDetailAsync(string id)
        {
            if (!Guid.TryParse(id, out Guid userId))
            {
                return null;
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.DoctorInfos)
                    .ThenInclude(d => d.Position)
                .Include(u => u.DoctorInfos)
                    .ThenInclude(d => d.Price)
                .Include(u => u.DoctorInfos)
                    .ThenInclude(d => d.Clinic)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return null;

            return MapToUserDTO(user);
        }

        private UserDTO MapToUserDTO(User user)
        {
            return new UserDTO
            {
                UserId = user.UserId.ToString(),
                Username = user.Username,
                Name = user.Name,
                Email = user.Email,
                RoleId = user.RoleId,
                RoleName = user.Role?.ValueVi
            };
        }

        public async Task<ServiceResult<string>> CreateUserAsync(CreateUserDTO model)
        {
            try
            {
                // Kiểm tra email đã tồn tại chưa
                if (!string.IsNullOrEmpty(model.Email))
                {
                    bool emailExists = await _userRepository.ExistsByEmailAsync(model.Email);
                    if (emailExists)
                    {
                        return ServiceResult<string>.Error("Email đã được sử dụng.");
                    }
                }

                // Tạo mới người dùng theo đúng model User.cs
                var userId = Guid.NewGuid();
                var user = new User
                {
                    UserId = userId,
                    Username = model.Username,
                    Password = model.Password, // Nên mã hóa mật khẩu trước khi lưu
                    Email = model.Email,
                    Name = model.Name,
                    RoleId = model.RoleId
                };

                await _userRepository.CreateAsync(user);

                return ServiceResult<string>.Ok("Tạo người dùng thành công.", userId.ToString());
            }
            catch (Exception ex)
            {
                // Log exception
                return ServiceResult<string>.Error($"Lỗi khi tạo người dùng: {ex.Message}");
            }
        }

        public async Task<ServiceResult> UpdateUserAsync(string id, UpdateUserDTO model)
        {
            try
            {
                if (!Guid.TryParse(id, out Guid userId))
                {
                    return ServiceResult.Error("ID người dùng không hợp lệ.");
                }

                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return ServiceResult.Error("Không tìm thấy người dùng.");
                }

                // Kiểm tra email
                if (!string.IsNullOrEmpty(model.Email) && model.Email != user.Email)
                {
                    bool emailExists = await _userRepository.ExistsByEmailAsync(model.Email);
                    if (emailExists)
                    {
                        return ServiceResult.Error("Email đã được sử dụng bởi người dùng khác.");
                    }
                }

                // Cập nhật thông tin người dùng theo đúng model User.cs
                if (!string.IsNullOrEmpty(model.Name))
                    user.Name = model.Name;

                if (!string.IsNullOrEmpty(model.Email))
                    user.Email = model.Email;

                if (!string.IsNullOrEmpty(model.RoleId))
                    user.RoleId = model.RoleId;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return ServiceResult.Ok("Cập nhật người dùng thành công.");
            }
            catch (Exception ex)
            {
                // Log exception
                return ServiceResult.Error($"Lỗi khi cập nhật người dùng: {ex.Message}");
            }
        }

        public async Task<ServiceResult> DeleteUserAsync(string id)
        {
            if (!Guid.TryParse(id, out Guid userId))
            {
                return ServiceResult.Error("ID người dùng không hợp lệ.");
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return ServiceResult.Error("Không tìm thấy người dùng.");
            }

            try
            {
                // Xóa tất cả dependencies trước khi xóa user
                
                // Xóa PatientRecords của user
                var patientRecords = await _context.PatientRecords
                    .Where(pr => pr.UserId == userId)
                    .ToListAsync();
                
                if (patientRecords.Any())
                {
                    _context.PatientRecords.RemoveRange(patientRecords);
                    Console.WriteLine($"Đã xóa {patientRecords.Count} hồ sơ bệnh nhân");
                }
                
                // Xóa DoctorInfos của user
                var doctorInfos = await _context.DoctorInfos
                    .Where(di => di.DoctorId == userId)
                    .ToListAsync();
                
                if (doctorInfos.Any())
                {
                    _context.DoctorInfos.RemoveRange(doctorInfos);
                    Console.WriteLine($"Đã xóa {doctorInfos.Count} thông tin bác sĩ");
                }
                
                // Xóa Appointments liên quan đến user (là doctor)
                var appointments = await _context.Appointments
                    .Where(a => a.DoctorId == userId)
                    .ToListAsync();
                
                if (appointments.Any())
                {
                    _context.Appointments.RemoveRange(appointments);
                    Console.WriteLine($"Đã xóa {appointments.Count} lịch hẹn");
                }

                // Xóa DoctorSpecialties nếu user là bác sĩ
                var doctorSpecialties = await _context.DoctorClinicSpecialties
                    .Where(ds => ds.DoctorId == userId)
                    .ToListAsync();
                
                if (doctorSpecialties.Any())
                {
                    _context.DoctorClinicSpecialties.RemoveRange(doctorSpecialties);
                    Console.WriteLine($"Đã xóa {doctorSpecialties.Count} chuyên khoa bác sĩ");
                }

                // Xóa Markdowns nếu user là bác sĩ
                var markdowns = await _context.Markdowns
                    .Where(m => m.DoctorId == userId)
                    .ToListAsync();
                
                if (markdowns.Any())
                {
                    _context.Markdowns.RemoveRange(markdowns);
                    Console.WriteLine($"Đã xóa {markdowns.Count} nội dung markdown");
                }

                // Cuối cùng xóa user
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return ServiceResult.Ok("Xóa người dùng và tất cả dữ liệu liên quan thành công.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Error($"Lỗi khi xóa người dùng: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> CheckUserExistsAsync(Guid userId, string roleId)
        {
            try
            {
                var exists = await _context.Users
                    .AnyAsync(u => u.UserId == userId && u.RoleId == roleId);
                return ServiceResult<bool>.Ok("", exists);
            }
            catch (Exception ex)
            {
                return ServiceResult<bool>.Error($"Lỗi khi kiểm tra người dùng: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<User>>> GetUsersByRoleAsync(string roleId)
        {
            try
            {
                var users = await _context.Users
                    .Where(u => u.RoleId == roleId)
                    .OrderBy(u => u.Name)
                    .ToListAsync();
                return ServiceResult<List<User>>.Ok("", users);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<User>>.Error($"Lỗi khi lấy danh sách người dùng theo role: {ex.Message}");
            }
        }

        public async Task<ServiceResult> ResetPasswordAsync(ResetPasswordDTO model)
        {
            try
            {
                // Chúng ta sẽ validate token trực tiếp trong UserService để tránh circular dependency
                // Lấy email từ token (token chứa email được hash)
                // Tạm thời để đơn giản, chúng ta sẽ validate token bằng cách decode nó
                
                // Tìm user by email từ token (giả sử token là email được hash hoặc encode)
                // Trong thực tế, bạn có thể lưu token trong database hoặc cache
                
                // Để đơn giản, chúng ta sẽ tạo một cách validate token khác
                // Bây giờ tôi sẽ tạo method validate riêng
                var email = await ValidateAndGetEmailFromTokenAsync(model.Token);
                if (string.IsNullOrEmpty(email))
                {
                    return ServiceResult.Error("Token không hợp lệ hoặc đã hết hạn");
                }

                // Get user by email
                var user = await _userRepository.GetByEmailAsync(email);
                if (user == null)
                {
                    return ServiceResult.Error("Không tìm thấy người dùng");
                }

                // Hash new password
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                user.Password = hashedPassword;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return ServiceResult.Ok("Đặt lại mật khẩu thành công");
            }
            catch (Exception ex)
            {
                return ServiceResult.Error($"Lỗi khi đặt lại mật khẩu: {ex.Message}");
            }
        }

        private async Task<string> ValidateAndGetEmailFromTokenAsync(string token)
        {
            try
            {
                return await ResetPasswordTokenService.ValidateAndGetEmailAsync(token);
            }
            catch
            {
                return null;
            }
        }

        public async Task<ServiceResult> ChangePasswordAsync(string userId, ChangePasswordDTO model)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
                if (user == null)
                {
                    return ServiceResult.Error("Không tìm thấy người dùng");
                }

                // Verify old password
                if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.Password))
                {
                    return ServiceResult.Error("Mật khẩu cũ không đúng");
                }

                // Hash new password
                user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return ServiceResult.Ok("Đổi mật khẩu thành công");
            }
            catch (Exception ex)
            {
                return ServiceResult.Error($"Lỗi khi đổi mật khẩu: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<User>>> GetUsersByRoleAsync(string roleId, QueryParameters queryParams)
        {
            try
            {
                var users = await _userRepository.GetUsersByRoleAsync(roleId, queryParams);
                return ServiceResult<IEnumerable<User>>.Ok("Thành công", users);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<User>>.Error($"Lỗi khi lấy danh sách users: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IEnumerable<User>>> GetUsersByRoleWithoutDoctorInfoAsync(string roleId, QueryParameters queryParams)
        {
            try
            {
                var users = await _userRepository.GetUsersByRoleWithoutDoctorInfoAsync(roleId, queryParams);
                return ServiceResult<IEnumerable<User>>.Ok("Thành công", users);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<User>>.Error($"Lỗi khi lấy danh sách users: {ex.Message}");
            }
        }
    }
} 