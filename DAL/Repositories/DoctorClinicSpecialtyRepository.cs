using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital_BE.DAL.Context;
using Hospital_BE.DAL.Interfaces;
using Hospital_BE.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Hospital_BE.DAL.Repositories
{
    public class DoctorClinicSpecialtyRepository : IDoctorClinicSpecialtyRepository
    {
        private readonly ApplicationDbContext _context;

        public DoctorClinicSpecialtyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<DoctorClinicSpecialty>> GetByDoctorIdAsync(Guid doctorId)
        {
            return await _context.DoctorClinicSpecialties
                .Include(dcs => dcs.Doctor)
                .Include(dcs => dcs.Clinic)
                .Include(dcs => dcs.Specialty)
                .Where(dcs => dcs.DoctorId == doctorId)
                .ToListAsync();
        }

        public async Task<DoctorClinicSpecialty> CreateAsync(DoctorClinicSpecialty entity)
        {
            _context.DoctorClinicSpecialties.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteByDoctorIdAsync(Guid doctorId)
        {
            var entities = await _context.DoctorClinicSpecialties
                .Where(dcs => dcs.DoctorId == doctorId)
                .ToListAsync();

            if (entities.Any())
            {
                _context.DoctorClinicSpecialties.RemoveRange(entities);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid doctorId, Guid clinicId, Guid specialtyId)
        {
            return await _context.DoctorClinicSpecialties
                .AnyAsync(dcs => dcs.DoctorId == doctorId && 
                                dcs.ClinicId == clinicId && 
                                dcs.SpecialtyId == specialtyId);
        }
    }
} 