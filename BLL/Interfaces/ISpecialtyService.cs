using System;
using System.Threading.Tasks;
using Hospital_BE.DAL.Models;
using Hospital_BE.PL.DTOs;
using Hospital_BE.PL.DTOs.Common;

namespace Hospital_BE.BLL.Interfaces
{
    public interface ISpecialtyService
    {
        Task<PaginatedResult<SpecialtyResponseDTO>> GetAllSpecialtiesAsync(PaginationParameters parameters);
        Task<SpecialtyResponseDTO> GetSpecialtyByIdAsync(Guid id);
        Task<SpecialtyResponseDTO> CreateSpecialtyAsync(CreateSpecialtyDTO createDto);
        Task<SpecialtyResponseDTO> UpdateSpecialtyAsync(Guid id, UpdateSpecialtyDTO updateDto);
        Task<bool> DeleteSpecialtyAsync(Guid id);
    }
} 