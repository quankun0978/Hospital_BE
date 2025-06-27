using Hospital_BE.DAL.Context;
using Hospital_BE.DAL.Interfaces;
using Hospital_BE.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Hospital_BE.DAL.Repositories
{
    public class MarkdownRepository : IMarkdownRepository
    {
        private readonly ApplicationDbContext _context;

        public MarkdownRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Markdown> CreateAsync(Markdown markdown)
        {
            _context.Markdowns.Add(markdown);
            await _context.SaveChangesAsync();
            return markdown;
        }

        public async Task<Markdown> UpdateAsync(Markdown markdown)
        {
            _context.Markdowns.Update(markdown);
            await _context.SaveChangesAsync();
            return markdown;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var markdown = await _context.Markdowns.FindAsync(id);
            if (markdown == null)
                return false;

            _context.Markdowns.Remove(markdown);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Markdown> GetByIdAsync(Guid id)
        {
            return await _context.Markdowns
                .Include(m => m.Doctor)
                .Include(m => m.Clinic)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Markdown> GetByDoctorIdAsync(Guid doctorId)
        {
            return await _context.Markdowns
                .Include(m => m.Doctor)
                .Include(m => m.Clinic)
                .FirstOrDefaultAsync(m => m.DoctorId == doctorId);
        }

        public async Task<Markdown> GetByClinicIdAsync(Guid clinicId)
        {
            return await _context.Markdowns
                .Include(m => m.Doctor)
                .Include(m => m.Clinic)
                .FirstOrDefaultAsync(m => m.ClinicId == clinicId);
        }
    }
} 