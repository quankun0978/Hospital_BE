using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Hospital_BE.DAL.Context;
using Hospital_BE.DAL.Interfaces;
using Hospital_BE.DAL.Models;
using Hospital_BE.PL.DTOs.Common;
using Hospital_BE.PL.DTOs;

namespace Hospital_BE.DAL.Repositories
{
    public class ClinicRepository : IClinicRepository
    {
        private readonly ApplicationDbContext _context;

        public ClinicRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Clinic> Items, int TotalCount)> GetAllAsync(QueryParameters parameters)
        {
            var query = _context.Clinics
                .OrderBy(c => c.Name)
                .AsQueryable();

            // Tính tổng số bản ghi
            var totalCount = await query.CountAsync();
            
            // Lấy dữ liệu theo trang
            var items = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();
                
            return (items, totalCount);
        }

        public async Task<Clinic> GetByIdAsync(Guid id)
        {
            return await _context.Clinics
                .Include(c => c.Doctors)
                    .ThenInclude(d => d.Doctor)
                .Include(c => c.Doctors)
                    .ThenInclude(d => d.Position)
                .Include(c => c.Doctors)
                    .ThenInclude(d => d.Price)
                .Include(c => c.ClinicImages)
                .FirstOrDefaultAsync(c => c.ClinicId == id);
        }

        public async Task<ClinicDetailDto> GetDetailByIdAsync(Guid id)
        {
            // Truy vấn clinic cùng với tất cả thông tin liên quan
            var clinic = await _context.Clinics
                .Include(c => c.Doctors)
                    .ThenInclude(d => d.Doctor)
                .Include(c => c.Doctors)
                    .ThenInclude(d => d.Position)
                .Include(c => c.Doctors)
                    .ThenInclude(d => d.Price)
                .Include(c => c.ClinicImages)
                .FirstOrDefaultAsync(c => c.ClinicId == id);

            if (clinic == null) return null;

            // Lấy danh sách specialties và doctor-clinic-specialty relationships
            var doctorClinicSpecialties = await _context.DoctorClinicSpecialties
                .Where(dcs => dcs.ClinicId == id)
                .Include(dcs => dcs.Specialty)
                .Include(dcs => dcs.Doctor)
                .ToListAsync();

            // Tạo group specialties với thông tin chi tiết
            var specialtyGroups = doctorClinicSpecialties
                .Where(dcs => dcs.Specialty != null)
                .GroupBy(dcs => dcs.SpecialtyId)
                .Select(g => new ClinicSpecialtyDto
                {
                    SpecialtyId = g.Key,
                    Name = g.First().Specialty.Name,
                    ImageUrl = g.First().Specialty.ImageUrl,
                    Description = g.First().Specialty.Description,
                    Slug = g.First().Specialty.Slug,
                    DoctorCount = g.Count(),
                    DoctorIds = g.Select(dcs => dcs.DoctorId).Distinct().ToList()
                })
                .ToList();

            // Chuyển đổi entity sang DTO
            return new ClinicDetailDto
            {
                ClinicId = clinic.ClinicId,
                Name = clinic.Name,
                Address = clinic.Address,
                Description = clinic.Description,
                Slug = clinic.Slug,
                ImageUrl = clinic.ImageUrl,
                LogoImg = clinic.LogoImg,
                IsHospital = clinic.IsHospital,
                ClinicImages = clinic.ClinicImages?.Select(ci => new ClinicImageDto
                {
                    Id = ci.Id,
                    ImageFallbackUrl = ci.ImageFallbackUrl,
                    IsBackground = ci.IsBackground,
                    ClinicId = ci.ClinicId
                }).ToList() ?? new List<ClinicImageDto>(),
                Doctors = clinic.Doctors?.Select(d => new ClinicDoctorDto
                {
                    Id = d.Id,
                    DoctorId = d.DoctorId,
                    DoctorName = d.Doctor?.Name,
                    PriceId = d.PriceId,
                    PriceName = d.Price?.ValueVi,
                    PositionId = d.PositionId,
                    PositionName = d.Position?.ValueVi,
                    Slug = d.Slug,
                    Note = d.Note,
                    ImageUrl = d.ImageUrl,
                    Count = d.Count
                }).ToList() ?? new List<ClinicDoctorDto>(),
                Specialties = specialtyGroups
            };
        }

        public async Task<Clinic> CreateAsync(Clinic clinic)
        {
            await _context.Clinics.AddAsync(clinic);
            await _context.SaveChangesAsync();
            return clinic;
        }

        public async Task<bool> UpdateAsync(Guid id, Clinic clinic)
        {
            var existingClinic = await _context.Clinics.FindAsync(id);
            if (existingClinic == null)
                return false;

            // Cập nhật thông tin
            existingClinic.Name = clinic.Name;
            existingClinic.Address = clinic.Address;
            existingClinic.Description = clinic.Description;
            existingClinic.Slug = clinic.Slug;
            existingClinic.ImageUrl = clinic.ImageUrl;
            existingClinic.LogoImg = clinic.LogoImg;

            _context.Clinics.Update(existingClinic);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var clinic = await _context.Clinics.FindAsync(id);
            if (clinic == null)
                return false;

            _context.Clinics.Remove(clinic);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
    }
} 