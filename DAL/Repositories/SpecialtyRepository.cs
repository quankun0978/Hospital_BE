using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital_BE.DAL.Context;
using Hospital_BE.DAL.Interfaces;
using Hospital_BE.DAL.Models;
using Hospital_BE.PL.DTOs.Common;
using Microsoft.EntityFrameworkCore;

namespace Hospital_BE.DAL.Repositories
{
    public class SpecialtyRepository : ISpecialtyRepository
    {
        private readonly ApplicationDbContext _context;

        public SpecialtyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Specialty> Items, int TotalCount)> GetAllAsync(PaginationParameters parameters)
        {
            var query = _context.Specialties
                .OrderBy(s => s.Name)
                .AsQueryable();

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Specialty> GetByIdAsync(Guid id)
        {
            return await _context.Specialties.FindAsync(id);
        }

        public async Task<Specialty> CreateAsync(Specialty specialty)
        {
            await _context.Specialties.AddAsync(specialty);
            await _context.SaveChangesAsync();
            return specialty;
        }

        public async Task<Specialty> UpdateAsync(Specialty specialty)
        {
            _context.Specialties.Update(specialty);
            await _context.SaveChangesAsync();
            return specialty;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var specialty = await _context.Specialties.FindAsync(id);
            if (specialty == null)
                return false;

            _context.Specialties.Remove(specialty);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null)
        {
            var query = _context.Specialties.Where(s => s.Name.ToLower() == name.ToLower());
            
            if (excludeId.HasValue)
                query = query.Where(s => s.SpecialtyId != excludeId.Value);

            return await query.AnyAsync();
        }
    }
} 