using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital_BE.DAL.Models;

namespace Hospital_BE.DAL.Interfaces
{
    public interface IClinicImageRepository
    {
        Task<IEnumerable<ClinicImage>> GetAllAsync();
        Task<ClinicImage> GetByIdAsync(int id);
        Task<IEnumerable<ClinicImage>> GetByClinicIdAsync(Guid clinicId);
        Task<IEnumerable<ClinicImage>> GetBackgroundImagesByClinicIdAsync(Guid clinicId);
        Task<IEnumerable<ClinicImage>> GetFallbackImagesByClinicIdAsync(Guid clinicId);
        Task<ClinicImage> CreateAsync(ClinicImage clinicImage);
        Task<bool> UpdateAsync(ClinicImage clinicImage);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteByClinicIdAsync(Guid clinicId);
        Task<bool> ExistsAsync(int id);
    }
} 