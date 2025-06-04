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

        public async Task<bool> CheckPhoneExistsAsync(string phone)
        {
            return await _userRepository.ExistsByPhoneAsync(phone);
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
                Phone = user.Phone,
                RoleId = user.RoleId,
                RoleName = user.Role?.ValueVi
            };
        }

        public async Task<ServiceResult<string>> CreateUserAsync(CreateUserDTO model)
        {
            try
            {
                // Kiểm tra số điện thoại đã tồn tại chưa
                bool phoneExists = await _userRepository.ExistsByPhoneAsync(model.Phone);
                if (phoneExists)
                {
                    return ServiceResult<string>.Error("Số điện thoại đã được sử dụng.");
                }

                // Tạo mới người dùng theo đúng model User.cs
                var userId = Guid.NewGuid();
                var user = new User
                {
                    UserId = userId,
                    Username = model.Username,
                    Password = model.Password, // Nên mã hóa mật khẩu trước khi lưu
                    Phone = model.Phone,
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

                // Kiểm tra số điện thoại
                if (!string.IsNullOrEmpty(model.Phone) && model.Phone != user.Phone)
                {
                    bool phoneExists = await _userRepository.ExistsByPhoneAsync(model.Phone);
                    if (phoneExists)
                    {
                        return ServiceResult.Error("Số điện thoại đã được sử dụng bởi người dùng khác.");
                    }
                }

                // Cập nhật thông tin người dùng theo đúng model User.cs
                if (!string.IsNullOrEmpty(model.Name))
                    user.Name = model.Name;

                if (!string.IsNullOrEmpty(model.Phone))
                    user.Phone = model.Phone;

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

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return ServiceResult.Ok("Xóa người dùng thành công.");
        }
    }
} 