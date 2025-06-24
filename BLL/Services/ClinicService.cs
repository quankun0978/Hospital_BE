using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Hospital_BE.BLL.Interfaces;
using Hospital_BE.DAL.Interfaces;
using Hospital_BE.DAL.Models;
using Hospital_BE.PL.DTOs.Common;
using Hospital_BE.PL.DTOs;

namespace Hospital_BE.BLL.Services
{
    public class ClinicService : IClinicService
    {
        private readonly IClinicRepository _clinicRepository;
        private readonly IClinicImageService _clinicImageService;

        public ClinicService(IClinicRepository clinicRepository, IClinicImageService clinicImageService)
        {
            _clinicRepository = clinicRepository;
            _clinicImageService = clinicImageService;
        }

        public async Task<PaginatedResult<Clinic>> GetClinicsAsync(QueryParameters parameters)
        {
            var (items, totalCount) = await _clinicRepository.GetAllAsync(parameters);
            return new PaginatedResult<Clinic>(items, totalCount, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<ClinicDetailDto> GetClinicByIdAsync(Guid id)
        {
            return await _clinicRepository.GetDetailByIdAsync(id);
        }

        public async Task<ClinicDetailDto> GetClinicBySlugAsync(string slug)
        {
            return await _clinicRepository.GetDetailBySlugAsync(slug);
        }

        public async Task<Clinic> GetClinicEntityByIdAsync(Guid id)
        {
            return await _clinicRepository.GetByIdAsync(id);
        }

        public async Task<Clinic> CreateClinicAsync(Clinic clinic)
        {
            return await _clinicRepository.CreateAsync(clinic);
        }

        public async Task<bool> UpdateClinicAsync(Guid id, Clinic clinic)
        {
            return await _clinicRepository.UpdateAsync(id, clinic);
        }

        public async Task<bool> DeleteClinicAsync(Guid id)
        {
            return await _clinicRepository.DeleteAsync(id);
        }

        public async Task<ClinicImage> AddClinicImageAsync(Guid clinicId, string imageUrl, bool isBackground = false)
        {
            var clinicImage = new ClinicImage
            {
                ClinicId = clinicId,
                ImageFallbackUrl = imageUrl,
                IsBackground = isBackground
            };

            return await _clinicImageService.CreateImageAsync(clinicImage);
        }
    }
} 