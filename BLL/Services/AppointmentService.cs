using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital_BE.BLL.Models;
using Hospital_BE.DAL.Context;
using Hospital_BE.DAL.Models;
using Hospital_BE.DAL.Repositories;
using Hospital_BE.PL.DTOs.Common;

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

                // Tạo lịch hẹn mới
                var appointment = new Appointment
                {
                    AppointmentId = Guid.NewGuid(),
                    PatientId = model.PatientId,
                    DoctorId = model.DoctorId,
                    AppointmentDate = model.AppointmentDate,
                    TimeType = model.TimeType,
                    Reason = model.Reason,
                    Status = "S", // Scheduled
                    CreatedAt = DateTime.Now
                };

                var createdAppointment = await _appointmentRepository.CreateAsync(appointment);

                // Gửi email xác nhận (không làm fail transaction nếu gửi email lỗi)
                try
                {
                    // Lấy thông tin chi tiết để gửi email
                    var appointmentWithDetails = await _appointmentRepository.GetByIdAsync(createdAppointment.AppointmentId);
                    if (appointmentWithDetails?.Patient?.Email != null)
                    {
                        await _emailService.SendAppointmentConfirmationAsync(
                            appointmentWithDetails.Patient.Email,
                            appointmentWithDetails.Patient.FullName ,
                            appointmentWithDetails.Doctor?.Name ?? "Bác sĩ",
                            appointmentWithDetails.AppointmentDate,
                            appointmentWithDetails.TimeType ?? "",
                            "Phòng khám", // Có thể lấy từ clinic của doctor
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

        public async Task<ServiceResult> UpdateAppointmentStatusAsync(Guid appointmentId, string newStatus)
        {
            try
            {
                if (!new[] { "S", "C", "N" }.Contains(newStatus))
                {
                    return ServiceResult.Error("Trạng thái không hợp lệ.");
                }

                var appointment = await _appointmentRepository.GetByIdAsync(appointmentId);
                if (appointment == null)
                {
                    return ServiceResult.Error("Không tìm thấy lịch khám.");
                }

                appointment.Status = newStatus;
                await _appointmentRepository.UpdateAsync(appointment);

                string statusText = newStatus switch
                {
                    "S" => "Đã lên lịch",
                    "C" => "Hoàn thành",
                    "N" => "Hủy",
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
            return await UpdateAppointmentStatusAsync(appointmentId, "N");
        }

        private AppointmentDetailsDTO MapToAppointmentDetailsDTO(Appointment appointment)
        {
            return new AppointmentDetailsDTO
            {
                AppointmentId = appointment.AppointmentId,
                PatientId = appointment.PatientId,
                PatientName = appointment.Patient?.FullName,
                DoctorId = appointment.DoctorId,
                DoctorName = appointment.Doctor?.Name,
                AppointmentDate = appointment.AppointmentDate,
                TimeType = appointment.TimeType,
                Reason = appointment.Reason,
                Status = appointment.Status,
                StatusText = appointment.Status switch
                {
                    "S" => "Đã lên lịch",
                    "C" => "Hoàn thành", 
                    "N" => "Hủy",
                    _ => "Không xác định"
                },
                CreatedAt = appointment.CreatedAt,
                UpdatedAt = appointment.UpdatedAt
            };
        }
    }

    // DTOs cho Appointment
    public class CreateAppointmentDTO
    {
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string? TimeType { get; set; }
        public string? Reason { get; set; }
    }

    public class AppointmentDetailsDTO
    {
        public Guid AppointmentId { get; set; }
        public Guid PatientId { get; set; }
        public string? PatientName { get; set; }
        public Guid DoctorId { get; set; }
        public string? DoctorName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string? TimeType { get; set; }
        public string? Reason { get; set; }
        public string Status { get; set; }
        public string StatusText { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 