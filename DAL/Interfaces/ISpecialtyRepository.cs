using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital_BE.DAL.Models;
using Hospital_BE.PL.DTOs.Common;

namespace Hospital_BE.DAL.Interfaces
{
    public interface ISpecialtyRepository
    {
        Task<(List<Specialty> Items, int TotalCount)> GetAllAsync(PaginationParameters parameters);
        Task<Specialty> GetByIdAsync(Guid id);
        Task<Specialty> CreateAsync(Specialty specialty);
        Task<Specialty> UpdateAsync(Specialty specialty);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null);
    }
} 