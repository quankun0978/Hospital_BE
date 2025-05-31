using System;
using System.Linq;
using System.Threading.Tasks;
using Hospital_BE.BLL.Interfaces;
using Hospital_BE.DAL.Interfaces;
using Hospital_BE.DAL.Models;
using Hospital_BE.PL.DTOs.Common;

namespace Hospital_BE.BLL.Services
{
    public class ClinicService : IClinicService
    {
        private readonly IClinicRepository _clinicRepository;

        public ClinicService(IClinicRepository clinicRepository)
        {
            _clinicRepository = clinicRepository;
        }

        public async Task<PaginatedResult<Clinic>> GetClinicsAsync(QueryParameters parameters)
        {
            var (items, totalCount) = await _clinicRepository.GetAllAsync(parameters);
            return new PaginatedResult<Clinic>(items, totalCount, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<Clinic> GetClinicByIdAsync(Guid id)
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
    }
} 