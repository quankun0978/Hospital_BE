using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital_BE.DAL.Models;

namespace Hospital_BE.BLL.Interfaces
{
    public interface IClinicImageService
    {
        Task<IEnumerable<ClinicImage>> GetAllImagesAsync();
        Task<ClinicImage> GetImageByIdAsync(int id);
        Task<IEnumerable<ClinicImage>> GetImagesByClinicIdAsync(Guid clinicId);
        Task<IEnumerable<ClinicImage>> GetBackgroundImagesByClinicIdAsync(Guid clinicId);
        Task<IEnumerable<ClinicImage>> GetFallbackImagesByClinicIdAsync(Guid clinicId);
        Task<ClinicImage> CreateImageAsync(ClinicImage clinicImage);
        Task<bool> UpdateImageAsync(int id, ClinicImage clinicImage);
        Task<bool> DeleteImageAsync(int id);
        Task<bool> DeleteImagesByClinicIdAsync(Guid clinicId);
    }
} 