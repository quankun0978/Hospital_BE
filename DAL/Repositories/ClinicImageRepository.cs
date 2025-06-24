using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Hospital_BE.DAL.Context;
using Hospital_BE.DAL.Interfaces;
using Hospital_BE.DAL.Models;

namespace Hospital_BE.DAL.Repositories
{
    public class ClinicImageRepository : IClinicImageRepository
    {
        private readonly ApplicationDbContext _context;

        public ClinicImageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ClinicImage>> GetAllAsync()
        {
            return await _context.ClinicImages
                .Include(ci => ci.Clinic)
                .OrderBy(ci => ci.Id)
                .ToListAsync();
        }

        public async Task<ClinicImage> GetByIdAsync(int id)
        {
            return await _context.ClinicImages
                .Include(ci => ci.Clinic)
                .FirstOrDefaultAsync(ci => ci.Id == id);
        }

        public async Task<IEnumerable<ClinicImage>> GetByClinicIdAsync(Guid clinicId)
        {
            return await _context.ClinicImages
                .Where(ci => ci.ClinicId == clinicId)
                .OrderBy(ci => ci.IsBackground ? 0 : 1) // Background images first
                .ThenBy(ci => ci.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<ClinicImage>> GetBackgroundImagesByClinicIdAsync(Guid clinicId)
        {
            return await _context.ClinicImages
                .Where(ci => ci.ClinicId == clinicId && ci.IsBackground)
                .OrderBy(ci => ci.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<ClinicImage>> GetFallbackImagesByClinicIdAsync(Guid clinicId)
        {
            return await _context.ClinicImages
                .Where(ci => ci.ClinicId == clinicId && !ci.IsBackground)
                .OrderBy(ci => ci.Id)
                .ToListAsync();
        }

        public async Task<ClinicImage> CreateAsync(ClinicImage clinicImage)
        {
            await _context.ClinicImages.AddAsync(clinicImage);
            await _context.SaveChangesAsync();
            return clinicImage;
        }

        public async Task<bool> UpdateAsync(ClinicImage clinicImage)
        {
            var existingImage = await _context.ClinicImages.FindAsync(clinicImage.Id);
            if (existingImage == null)
                return false;

            existingImage.ImageFallbackUrl = clinicImage.ImageFallbackUrl;
            existingImage.IsBackground = clinicImage.IsBackground;
            existingImage.ClinicId = clinicImage.ClinicId;

            _context.ClinicImages.Update(existingImage);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var clinicImage = await _context.ClinicImages.FindAsync(id);
            if (clinicImage == null)
                return false;

            _context.ClinicImages.Remove(clinicImage);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteByClinicIdAsync(Guid clinicId)
        {
            var clinicImages = await _context.ClinicImages
                .Where(ci => ci.ClinicId == clinicId)
                .ToListAsync();

            if (!clinicImages.Any())
                return true;

            _context.ClinicImages.RemoveRange(clinicImages);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.ClinicImages.AnyAsync(ci => ci.Id == id);
        }
    }
} 