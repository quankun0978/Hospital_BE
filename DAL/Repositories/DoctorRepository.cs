using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Hospital_BE.DAL.Context;
using Hospital_BE.DAL.Interfaces;
using Hospital_BE.DAL.Models;
using Hospital_BE.PL.DTOs.Common;

namespace Hospital_BE.DAL.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly ApplicationDbContext _context;

        public DoctorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(List<DoctorInfo> Items, int TotalCount)> GetAllAsync(QueryParameters parameters)
        {
            var query = _context.DoctorInfos
                .Include(d => d.Doctor)
                .Include(d => d.Position)
                .Include(d => d.Price)
                .Include(d => d.Clinic)
                .AsQueryable();

            // Filter by ClinicId if provided
            if (parameters.ClinicId.HasValue)
            {
                query = query.Where(d => d.ClinicId == parameters.ClinicId.Value);
            }

            // Filter by search term if provided
            if (!string.IsNullOrEmpty(parameters.Search) || !string.IsNullOrEmpty(parameters.SearchTerm))
            {
                var searchTerm = parameters.Search ?? parameters.SearchTerm;
                query = query.Where(d => d.Doctor.Name.Contains(searchTerm) || 
                                        (d.Note != null && d.Note.Contains(searchTerm)));
            }

            query = query.OrderBy(d => d.Doctor.Name);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<DoctorInfo> GetByIdAsync(Guid id)
        {
            return await _context.DoctorInfos
                .Include(d => d.Doctor)
                .Include(d => d.Position)
                .Include(d => d.Price)
                .Include(d => d.Clinic)
                .FirstOrDefaultAsync(d => d.DoctorId == id);
        }

        public async Task<DoctorInfo> CreateAsync(DoctorInfo doctorInfo)
        {
            await _context.DoctorInfos.AddAsync(doctorInfo);
            await _context.SaveChangesAsync();
            return doctorInfo;
        }

        public async Task<bool> UpdateAsync(Guid doctorId, DoctorInfo doctorInfo)
        {
            var existingDoctor = await _context.DoctorInfos
                .FirstOrDefaultAsync(d => d.DoctorId == doctorId);
            if (existingDoctor == null)
                return false;

            existingDoctor.PositionId = doctorInfo.PositionId;
            existingDoctor.PriceId = doctorInfo.PriceId;
            existingDoctor.ClinicId = doctorInfo.ClinicId;
            existingDoctor.Note = doctorInfo.Note;
            existingDoctor.ImageUrl = doctorInfo.ImageUrl;
            existingDoctor.Slug = doctorInfo.Slug;
            existingDoctor.Count = doctorInfo.Count;

            _context.DoctorInfos.Update(existingDoctor);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteAsync(Guid doctorId)
        {
            var doctor = await _context.DoctorInfos
                .FirstOrDefaultAsync(d => d.DoctorId == doctorId);
            if (doctor == null)
                return false;

            _context.DoctorInfos.Remove(doctor);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<DoctorInfo> GetBySlugAsync(string slug)
        {
            return await _context.DoctorInfos
                .Include(d => d.Doctor)
                    .ThenInclude(u => u.Role)
                .Include(d => d.Position)
                .Include(d => d.Price)
                .Include(d => d.Clinic)
                .FirstOrDefaultAsync(d => d.Slug == slug);
        }

        public async Task<List<DoctorInfo>> GetAllDoctorInfosByDoctorIdAsync(Guid doctorId)
        {
            return await _context.DoctorInfos
                .Include(d => d.Doctor)
                .Include(d => d.Position)
                .Include(d => d.Price)
                .Include(d => d.Clinic)
                .Where(d => d.DoctorId == doctorId)
                .ToListAsync();
        }

        public async Task<List<DoctorClinicSpecialty>> GetDoctorClinicSpecialtiesByDoctorIdAsync(Guid doctorId)
        {
            return await _context.DoctorClinicSpecialties
                .Include(dcs => dcs.Specialty)
                .Include(dcs => dcs.Clinic)
                .Include(dcs => dcs.Doctor)
                .Where(dcs => dcs.DoctorId == doctorId)
                .ToListAsync();
        }
    }
} 