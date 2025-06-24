using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital_BE.BLL.Interfaces;
using Hospital_BE.DAL.Interfaces;
using Hospital_BE.DAL.Models;

namespace Hospital_BE.BLL.Services
{
    public class ClinicImageService : IClinicImageService
    {
        private readonly IClinicImageRepository _clinicImageRepository;

        public ClinicImageService(IClinicImageRepository clinicImageRepository)
        {
            _clinicImageRepository = clinicImageRepository;
        }

        public async Task<IEnumerable<ClinicImage>> GetAllImagesAsync()
        {
            return await _clinicImageRepository.GetAllAsync();
        }

        public async Task<ClinicImage> GetImageByIdAsync(int id)
        {
            return await _clinicImageRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<ClinicImage>> GetImagesByClinicIdAsync(Guid clinicId)
        {
            return await _clinicImageRepository.GetByClinicIdAsync(clinicId);
        }

        public async Task<IEnumerable<ClinicImage>> GetBackgroundImagesByClinicIdAsync(Guid clinicId)
        {
            return await _clinicImageRepository.GetBackgroundImagesByClinicIdAsync(clinicId);
        }

        public async Task<IEnumerable<ClinicImage>> GetFallbackImagesByClinicIdAsync(Guid clinicId)
        {
            return await _clinicImageRepository.GetFallbackImagesByClinicIdAsync(clinicId);
        }

        public async Task<ClinicImage> CreateImageAsync(ClinicImage clinicImage)
        {
            // Validation logic có thể thêm ở đây
            if (clinicImage == null)
                throw new ArgumentNullException(nameof(clinicImage));

            if (string.IsNullOrWhiteSpace(clinicImage.ImageFallbackUrl))
                throw new ArgumentException("Image URL không được để trống", nameof(clinicImage.ImageFallbackUrl));

            return await _clinicImageRepository.CreateAsync(clinicImage);
        }

        public async Task<bool> UpdateImageAsync(int id, ClinicImage clinicImage)
        {
            // Validation logic
            if (clinicImage == null)
                throw new ArgumentNullException(nameof(clinicImage));

            if (string.IsNullOrWhiteSpace(clinicImage.ImageFallbackUrl))
                throw new ArgumentException("Image URL không được để trống", nameof(clinicImage.ImageFallbackUrl));

            // Kiểm tra xem image có tồn tại không
            var existingImage = await _clinicImageRepository.GetByIdAsync(id);
            if (existingImage == null)
                return false;

            // Set ID cho update
            clinicImage.Id = id;
            return await _clinicImageRepository.UpdateAsync(clinicImage);
        }

        public async Task<bool> DeleteImageAsync(int id)
        {
            // Kiểm tra xem image có tồn tại không
            var existingImage = await _clinicImageRepository.GetByIdAsync(id);
            if (existingImage == null)
                return false;

            return await _clinicImageRepository.DeleteAsync(id);
        }

        public async Task<bool> DeleteImagesByClinicIdAsync(Guid clinicId)
        {
            return await _clinicImageRepository.DeleteByClinicIdAsync(clinicId);
        }
    }
} 