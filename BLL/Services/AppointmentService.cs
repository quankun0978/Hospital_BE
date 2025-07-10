using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital_BE.BLL.Models;
using Hospital_BE.DAL.Context;
using Hospital_BE.DAL.Models;
using Hospital_BE.DAL.Repositories;
using Hospital_BE.PL.DTOs;
using Hospital_BE.PL.DTOs.Common;
using Microsoft.EntityFrameworkCore;

namespace Hospital_BE.BLL.Services
{
    public class AppointmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly AppointmentRepository _appointmentRepository;
        private readonly EmailService _emailService;

        public AppointmentService(ApplicationDbContext context, AppointmentRepository appointmentRepository, EmailService emailService)
        {
            _context = context;
            _appointmentRepository = appointmentRepository;
            _emailService = emailService;
        }

        public async Task<ServiceResult<Guid>> CreateAppointmentAsync(CreateAppointmentDTO model)
        {
            try
            {
                
                // Kiểm tra patient tồn tại
                var patientExists = await _context.PatientRecords.AnyAsync(p => p.PatientId == model.PatientId);
                if (!patientExists)
                {
                    return ServiceResult<Guid>.Error("Bệnh nhân không tồn tại trong hệ thống.");
                }
                
                // Kiểm tra doctor tồn tại
                var doctorExists = await _context.Users.AnyAsync(u => u.UserId == model.DoctorId && u.RoleId == "R2");
                if (!doctorExists)
                {
                    return ServiceResult<Guid>.Error("Bác sĩ không tồn tại trong hệ thống.");
                }

                // Kiểm tra khung giờ có trống không
                bool isAvailable = await _appointmentRepository.IsTimeSlotAvailableAsync(
                    model.DoctorId, 
                    model.AppointmentDate, 
                    model.TimeType
                );

                if (!isAvailable)
                {
                    return ServiceResult<Guid>.Error("Khung giờ này đã được đặt trước.");
                }
                
                Console.WriteLine("✅ All validations passed, creating appointment...");

                // Tạo lịch hẹn mới
                var appointment = new Appointment
                {
                    AppointmentId = Guid.NewGuid(),
                    PatientId = model.PatientId,
                    DoctorId = model.DoctorId,
                    AppointmentDate = model.AppointmentDate,
                    TimeType = model.TimeType,
                    Reason = model.Reason,
                    Status = "S1", // Lịch hẹn mới
                    CreatedAt = DateTime.Now
                };

                var createdAppointment = await _appointmentRepository.CreateAsync(appointment);

                // Gửi email xác nhận (không làm fail transaction nếu gửi email lỗi)
                try
                {
                    // Lấy thông tin chi tiết để gửi email
                    var appointmentWithDetails = await _appointmentRepository.GetByIdAsync(createdAppointment.AppointmentId);
                    if (appointmentWithDetails?.Patient?.User?.Email != null)
                    {
                        await _emailService.SendAppointmentConfirmationAsync(
                            appointmentWithDetails.Patient.User.Email,
                            appointmentWithDetails.Patient.FullName ,
                            appointmentWithDetails.Doctor?.Name ?? "Bác sĩ",
                            appointmentWithDetails.AppointmentDate,
                            appointmentWithDetails.TimeType ?? "",
                            "Phòng khám", // Có thể lấy từ clinic của doctor
                            appointmentWithDetails.AppointmentId,
                            appointmentWithDetails.Reason ?? ""
                        );
                    }
                }
                catch (Exception emailEx)
                {
                    // Log lỗi email nhưng không fail transaction
                    Console.WriteLine($"Lỗi gửi email: {emailEx.Message}");
                }

                return ServiceResult<Guid>.Ok("Đặt lịch khám thành công.", appointment.AppointmentId);
            }
            catch (Exception ex)
            {
                return ServiceResult<Guid>.Error($"Lỗi khi đặt lịch khám: {ex.Message}");
            }
        }

        public async Task<ServiceResult<AppointmentDetailsDTO>> GetAppointmentByIdAsync(Guid appointmentId)
        {
            try
            {
                var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
                if (appointment == null)
                {
                    return ServiceResult<AppointmentDetailsDTO>.Error("Không tìm thấy lịch khám.");
                }

                var result = MapToAppointmentDetailsDTO(appointment);
                return ServiceResult<AppointmentDetailsDTO>.Ok("Lấy thông tin lịch khám thành công.", result);
            }
            catch (Exception ex)
            {
                return ServiceResult<AppointmentDetailsDTO>.Error($"Lỗi khi lấy thông tin lịch khám: {ex.Message}");
            }
        }

        public async Task<PaginatedResult<AppointmentDetailsDTO>> GetAppointmentsAsync(QueryParameters parameters)
        {
            var (items, totalCount) = await _appointmentRepository.GetAllAsync(parameters);
            var mappedItems = items.Select(MapToAppointmentDetailsDTO).ToList();
            return new PaginatedResult<AppointmentDetailsDTO>(mappedItems, totalCount, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<ServiceResult<List<AppointmentDetailsDTO>>> GetAppointmentsByPatientIdAsync(Guid patientId)
        {
            try
            {
                var appointments = await _appointmentRepository.GetByPatientIdAsync(patientId);
                var result = appointments.Select(MapToAppointmentDetailsDTO).ToList();
                return ServiceResult<List<AppointmentDetailsDTO>>.Ok("Lấy danh sách lịch khám thành công.", result);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<AppointmentDetailsDTO>>.Error($"Lỗi khi lấy danh sách lịch khám: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<AppointmentDetailsDTO>>> GetAppointmentsByDoctorIdAsync(Guid doctorId)
        {
            try
            {
                var appointments = await _appointmentRepository.GetByDoctorIdAsync(doctorId);
                var result = appointments.Select(MapToAppointmentDetailsDTO).ToList();
                return ServiceResult<List<AppointmentDetailsDTO>>.Ok("Lấy danh sách lịch khám thành công.", result);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<AppointmentDetailsDTO>>.Error($"Lỗi khi lấy danh sách lịch khám: {ex.Message}");
            }
        }

        public async Task<PaginatedResult<AppointmentDetailsDTO>> GetAppointmentsByDoctorIdAsync(Guid doctorId, QueryParameters parameters)
        {
            try
            {
                var (items, totalCount) = await _appointmentRepository.GetByDoctorIdWithFiltersAsync(doctorId, parameters);
                var mappedItems = items.Select(MapToAppointmentDetailsDTO).ToList();
                return new PaginatedResult<AppointmentDetailsDTO>(mappedItems, totalCount, parameters.PageNumber, parameters.PageSize);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception($"Lỗi khi lấy danh sách lịch khám: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<AppointmentDetailsDTO>>> GetAppointmentsByUserIdAsync(Guid userId)
        {
            try
            {
                var appointments = await _appointmentRepository.GetByUserIdAsync(userId);
                var result = appointments.Select(MapToAppointmentDetailsDTO).ToList();
                return ServiceResult<List<AppointmentDetailsDTO>>.Ok("Lấy danh sách lịch khám thành công.", result);
            }
            catch (Exception ex)
            {
                return ServiceResult<List<AppointmentDetailsDTO>>.Error($"Lỗi khi lấy danh sách lịch khám: {ex.Message}");
            }
        }

        public async Task<ServiceResult> UpdateAppointmentStatusAsync(Guid appointmentId, string newStatus)
        {
            try
            {
                if (!new[] { "S1", "S2", "S3", "S4" }.Contains(newStatus))
                {
                    return ServiceResult.Error("Trạng thái không hợp lệ.");
                }

                var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
                if (appointment == null)
                {
                    return ServiceResult.Error("Không tìm thấy lịch khám.");
                }

                appointment.Status = newStatus;
                appointment.UpdatedAt = DateTime.Now;
                await _appointmentRepository.UpdateAsync(appointment);

                string statusText = newStatus switch
                {
                    "S1" => "Lịch hẹn mới",
                    "S2" => "Đã xác nhận",
                    "S3" => "Đã khám xong",
                    "S4" => "Đã hủy",
                    _ => "Không xác định"
                };

                return ServiceResult.Ok($"Cập nhật trạng thái thành '{statusText}' thành công.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Error($"Lỗi khi cập nhật trạng thái: {ex.Message}");
            }
        }

        public async Task<ServiceResult> CancelAppointmentAsync(Guid appointmentId)
        {
            try
            {
                // Sử dụng execution strategy để handle transaction với retry strategy
                var strategy = _context.Database.CreateExecutionStrategy();
                return await strategy.ExecuteAsync(async () =>
                {
                    using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
                if (appointment == null)
                {
                    return ServiceResult.Error("Không tìm thấy lịch khám.");
                }

                // Kiểm tra trạng thái có thể hủy
                if (!new[] { "S1", "S2" }.Contains(appointment.Status))
                {
                    return ServiceResult.Error("Lịch khám này không thể hủy do trạng thái không phù hợp.");
                }

                // Kiểm tra thời gian (chỉ cho phép hủy trong vòng 10 phút)
                var timeDiff = DateTime.Now - appointment.CreatedAt;
                if (timeDiff.TotalMinutes > 10)
                {
                    return ServiceResult.Error("Không thể hủy lịch khám sau 10 phút kể từ lúc đặt lịch.");
                }

                        // Cập nhật status của appointment thành "S4" (Đã hủy)
                        appointment.Status = "S4";
                        appointment.UpdatedAt = DateTime.Now;
                        _context.Appointments.Update(appointment);

                        // Tìm và cập nhật IsActive của Schedule tương ứng về true (trả lại slot)
                        var schedule = await _context.Schedules
                            .FirstOrDefaultAsync(s => 
                                s.DoctorId == appointment.DoctorId && 
                                s.Date.Date == appointment.AppointmentDate.Date && 
                                s.TimeType == appointment.TimeType);

                        if (schedule != null)
                        {
                            schedule.IsActive = true; // Trả lại slot để có thể đặt lại
                            _context.Schedules.Update(schedule);
                        }

                        // Lưu tất cả thay đổi trong một transaction
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        return ServiceResult.Ok("Hủy lịch khám thành công!");
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw; // Re-throw để execution strategy có thể handle retry
                    }
                });
            }
            catch (Exception ex)
            {
                return ServiceResult.Error($"Lỗi khi hủy lịch khám: {ex.Message}");
            }
        }

        public async Task<ServiceResult> ConfirmAppointmentAsync(Guid appointmentId)
        {
            try
            {
                // Sử dụng execution strategy để handle transaction với retry strategy
                var strategy = _context.Database.CreateExecutionStrategy();
                return await strategy.ExecuteAsync(async () =>
                {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
                if (appointment == null)
                {
                    return ServiceResult.Error("Không tìm thấy lịch khám.");
                }

                if (appointment.Status != "S1")
                {
                    return ServiceResult.Error("Lịch khám này đã được xác nhận trước đó hoặc đã bị hủy.");
                }

                // Cập nhật status của appointment thành "S2" (Đã xác nhận)
                appointment.Status = "S2";
                appointment.UpdatedAt = DateTime.Now;
                _context.Appointments.Update(appointment);

                // Tìm và cập nhật IsActive của Schedule tương ứng về false
                var schedule = await _context.Schedules
                    .FirstOrDefaultAsync(s => 
                        s.DoctorId == appointment.DoctorId && 
                        s.Date.Date == appointment.AppointmentDate.Date && 
                        s.TimeType == appointment.TimeType);

                if (schedule != null)
                {
                    schedule.IsActive = false; // Đánh dấu slot này đã được đặt
                    _context.Schedules.Update(schedule);
                }

                // Lưu tất cả thay đổi trong một transaction
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return ServiceResult.Ok("Xác nhận lịch khám thành công!");
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw; // Re-throw để execution strategy có thể handle retry
                    }
                });
            }
            catch (Exception ex)
            {
                return ServiceResult.Error($"Lỗi khi xác nhận lịch khám: {ex.Message}");
            }
        }

        public async Task<ServiceResult> CompleteAppointmentAsync(Hospital_BE.PL.DTOs.CompleteAppointmentDTO model)
        {
            try
            {
                // Lấy thông tin lịch hẹn với đầy đủ thông tin patient và doctor
                var appointment = await _context.Appointments
                    .Include(a => a.Patient).ThenInclude(p => p.User)
                    .Include(a => a.Doctor)
                    .FirstOrDefaultAsync(a => a.AppointmentId == model.AppointmentId);
                    
                if (appointment == null)
                {
                    return ServiceResult.Error("Không tìm thấy lịch khám.");
                }

                // Kiểm tra trạng thái hiện tại
                if (appointment.Status == "S3")
                {
                    return ServiceResult.Error("Lịch khám này đã hoàn thành trước đó.");
                }

                if (appointment.Status == "S4")
                {
                    return ServiceResult.Error("Không thể hoàn thành lịch khám đã bị hủy.");
                }

                // Cập nhật trạng thái thành hoàn thành
                var previousStatus = appointment.Status;
                appointment.Status = "S3";
                appointment.UpdatedAt = DateTime.Now;
                await _appointmentRepository.UpdateAsync(appointment);

                // Chỉ gửi email nếu appointment chưa hoàn thành trước đó (tránh gửi duplicate)
                if (previousStatus != "S3" && appointment.Patient?.User?.Email != null)
                {
                    try
                    {
                        Console.WriteLine($"Gửi email kết quả khám cho: {appointment.Patient.User.Email}");
                        
                        // Lấy TimeType text từ Allcodes
                        var timeTypeText = appointment.TimeType;
                        if (!string.IsNullOrEmpty(appointment.TimeType))
                        {
                            var allcode = await _context.Allcodes
                                .FirstOrDefaultAsync(a => a.CodeKey == appointment.TimeType && a.CodeType == "TIME");
                            timeTypeText = allcode?.ValueVi ?? appointment.TimeType;
                        }
                        
                        var emailResult = await _emailService.SendMedicalResultsAsync(
                            appointment.Patient.User.Email,
                            appointment.Patient.FullName,
                            appointment.Doctor?.Name ?? "Bác sĩ",
                            appointment.AppointmentDate,
                            timeTypeText ?? "Chưa xác định",
                            model.MedicalNotes,
                            model.MedicalImages ?? new List<string>()
                        );

                        if (emailResult.Success)
                        {
                            Console.WriteLine("Email đã được gửi thành công!");
                        }
                        else
                        {
                            Console.WriteLine($"Lỗi gửi email: {emailResult.Message}");
                        }
                    }
                    catch (Exception emailEx)
                    {
                        // Log lỗi email nhưng không fail transaction
                        Console.WriteLine($"Exception khi gửi email kết quả khám: {emailEx.Message}");
                    }
                }

                return ServiceResult.Ok("Hoàn thành khám bệnh và gửi kết quả thành công.");
            }
            catch (Exception ex)
            {
                return ServiceResult.Error($"Lỗi khi hoàn thành khám bệnh: {ex.Message}");
            }
        }

        private AppointmentDetailsDTO MapToAppointmentDetailsDTO(Appointment appointment)
        {
            // Query TimeType text từ Allcodes
            var timeTypeText = appointment.TimeType;
            if (!string.IsNullOrEmpty(appointment.TimeType))
            {
                var allcode = _context.Allcodes
                    .FirstOrDefault(a => a.CodeKey == appointment.TimeType && a.CodeType == "TIME");
                timeTypeText = allcode?.ValueVi ?? appointment.TimeType;
            }

            return new AppointmentDetailsDTO
            {
                AppointmentId = appointment.AppointmentId,
                PatientId = appointment.PatientId,
                Patient = appointment.Patient != null ? new PatientInfoDTO
                {
                    FullName = appointment.Patient.FullName,
                    Phone = appointment.Patient.Phone,
                    Email = appointment.Patient.Email,
                    DateOfBirth = appointment.Patient.DateOfBirth,
                    Gender = appointment.Patient.Gender,
                    Address = appointment.Patient.Address,
                } : null,
                DoctorId = appointment.DoctorId,
                Doctor = appointment.Doctor != null ? new DoctorInfoDTO
                {
                    Name = appointment.Doctor.Name,
                    Email = appointment.Doctor.Email,
                    Phone = null, // DoctorInfo không có Phone
                    SpecialtyName = null, // Sẽ lấy từ DoctorClinicSpecialty nếu cần
                    PositionName = appointment.Doctor.DoctorInfos?.FirstOrDefault()?.Position?.ValueVi
                } : null,
                AppointmentDate = appointment.AppointmentDate,
                TimeType = appointment.TimeType,
                TimeTypeText = timeTypeText,
                Reason = appointment.Reason,
                Status = appointment.Status,
                StatusText = appointment.Status switch
                {
                    "S1" => "Lịch hẹn mới",
                    "S2" => "Đã xác nhận",
                    "S3" => "Đã khám xong", 
                    "S4" => "Đã hủy",
                    _ => "Không xác định"
                },
                CreatedAt = appointment.CreatedAt,
                UpdatedAt = appointment.UpdatedAt
            };
        }
    }


} 