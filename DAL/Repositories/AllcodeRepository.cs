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
    public class AllcodeRepository : IAllcodeRepository
    {
        private readonly ApplicationDbContext _context;

        public AllcodeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Allcode> Items, int TotalCount)> GetByTypeAsync(string codeType, PaginationParameters parameters)
        {
            var query = _context.Allcodes
                .Where(a => a.CodeType == codeType)
                .OrderBy(a => a.CodeKey)
                .AsQueryable();

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<List<string>> GetAllCodeTypesAsync()
        {
            return await _context.Allcodes
                .Select(a => a.CodeType)
                .Distinct()
                .OrderBy(t => t)
                .ToListAsync();
        }

        public async Task<(List<Allcode> Items, int TotalCount)> GetAllCodesAsync(QueryParameters parameters)
        {
            var query = _context.Allcodes
                .OrderBy(a => a.CodeType)
                .ThenBy(a => a.CodeKey)
                .AsQueryable();

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Allcode> GetByIdAsync(int id)
        {
            return await _context.Allcodes.FindAsync(id);
        }

        public async Task<Allcode> CreateAsync(Allcode allcode)
        {
            await _context.Allcodes.AddAsync(allcode);
            await _context.SaveChangesAsync();
            return allcode;
        }

        public async Task<Allcode> UpdateAsync(Allcode allcode)
        {
            _context.Allcodes.Update(allcode);
            await _context.SaveChangesAsync();
            return allcode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var allcode = await _context.Allcodes.FindAsync(id);
            if (allcode == null)
                return false;

            _context.Allcodes.Remove(allcode);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
    }
} 