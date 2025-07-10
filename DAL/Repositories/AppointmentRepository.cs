using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Hospital_BE.DAL.Context;
using Hospital_BE.DAL.Models;
using Hospital_BE.PL.DTOs.Common;

namespace Hospital_BE.DAL.Repositories
{
    public class AppointmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AppointmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Appointment> CreateAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();
            return appointment;
        }

        public async Task<Appointment> GetByIdAsync(Guid appointmentId)
        {
            return await _context.Appointments
                .Include(a => a.Patient).ThenInclude(a=>a.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.DoctorInfos)
                        .ThenInclude(di => di.Position) // Include thông tin chức vụ
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);
        }

        public async Task<(List<Appointment> Items, int TotalCount)> GetAllAsync(QueryParameters parameters)
        {
            var query = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.DoctorInfos)
                        .ThenInclude(di => di.Position) // Include thông tin chức vụ
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                var searchTerm = parameters.SearchTerm.ToLower();
                query = query.Where(a => 
                    (a.Patient.FullName != null && a.Patient.FullName.ToLower().Contains(searchTerm)) ||
                    (a.Doctor.Name != null && a.Doctor.Name.ToLower().Contains(searchTerm)) ||
                    (a.Reason != null && a.Reason.ToLower().Contains(searchTerm))
                );
            }

            // Apply status filter  
            if (!string.IsNullOrWhiteSpace(parameters.Status))
            {
                query = query.Where(a => a.Status == parameters.Status);
            }

            // Apply appointment date filter
            if (parameters.AppointmentDate.HasValue)
            {
                var filterDate = parameters.AppointmentDate.Value.Date;
                query = query.Where(a => a.AppointmentDate.Date == filterDate);
            }

            // Default ordering
            query = query.OrderByDescending(a => a.CreatedAt);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<List<Appointment>> GetByPatientIdAsync(Guid patientId)
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.DoctorInfos)
                        .ThenInclude(di => di.Position) // Include thông tin chức vụ
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetByDoctorIdAsync(Guid doctorId)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.DoctorInfos)
                        .ThenInclude(di => di.Position) // Include thông tin chức vụ
                .Where(a => a.DoctorId == doctorId && a.AppointmentDate.Date <= DateTime.Today) // Chỉ lấy lịch hẹn <= hôm nay
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<(List<Appointment> Items, int TotalCount)> GetByDoctorIdWithFiltersAsync(Guid doctorId, QueryParameters parameters)
        {
            var query = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.DoctorInfos)
                        .ThenInclude(di => di.Position) // Include thông tin chức vụ
                .Where(a => a.DoctorId == doctorId)
                .AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                var searchTerm = parameters.SearchTerm.ToLower();
                query = query.Where(a => 
                    (a.Patient.FullName != null && a.Patient.FullName.ToLower().Contains(searchTerm)) ||
                    (a.Doctor.Name != null && a.Doctor.Name.ToLower().Contains(searchTerm)) ||
                    (a.Reason != null && a.Reason.ToLower().Contains(searchTerm))
                );
            }

            // Apply status filter  
            if (!string.IsNullOrWhiteSpace(parameters.Status))
            {
                query = query.Where(a => a.Status == parameters.Status);
            }

            // Apply appointment date filter
            if (parameters.AppointmentDate.HasValue)
            {
                var filterDate = parameters.AppointmentDate.Value.Date;
                query = query.Where(a => a.AppointmentDate.Date == filterDate);
            }

            // Default ordering
            query = query.OrderByDescending(a => a.CreatedAt);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<List<Appointment>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Appointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.DoctorInfos)
                        .ThenInclude(di => di.Position) // Include thông tin chức vụ
                .Where(a => a.DoctorId == userId || a.Patient.UserId == userId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<Appointment> UpdateAsync(Appointment appointment)
        {
            appointment.UpdatedAt = DateTime.Now;
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
            return appointment;
        }

        public async Task<bool> DeleteAsync(Guid appointmentId)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
                return false;

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsTimeSlotAvailableAsync(Guid doctorId, DateTime appointmentDate, string timeType)
        {
            return !await _context.Appointments
                .AnyAsync(a => a.DoctorId == doctorId && 
                              a.AppointmentDate.Date == appointmentDate.Date && 
                              a.TimeType == timeType && 
                              a.Status != "S4"); // Không tính những lịch đã hủy
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
} 