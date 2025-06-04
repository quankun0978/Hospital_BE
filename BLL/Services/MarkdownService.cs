using System;
using System.Threading.Tasks;
using Hospital_BE.BLL.Interfaces;
using Hospital_BE.DAL.Context;
using Hospital_BE.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Hospital_BE.BLL.Services
{
    public class MarkdownService : IMarkdownService
    {
        private readonly ApplicationDbContext _context;
        public MarkdownService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Markdown> GetMarkdownByIdAsync(Guid id)
        {
            return await _context.Markdowns
                .Include(m => m.Doctor)
                .FirstOrDefaultAsync(m => m.DoctorId == id);
        }

        public async Task<Markdown> GetMarkdownByDoctorOrClinicIdAsync(Guid id)
        {
            // Ưu tiên lấy theo DoctorId
            var markdown = await _context.Markdowns
                .Include(m => m.Doctor)
                .Include(m => m.Clinic)
                .FirstOrDefaultAsync(m => m.DoctorId == id);
            if (markdown != null)
                return markdown;
            // Nếu không có, lấy theo ClinicId
            return await _context.Markdowns
                .Include(m => m.Doctor)
                .Include(m => m.Clinic)
                .FirstOrDefaultAsync(m => m.ClinicId == id);
        }
    }
} 