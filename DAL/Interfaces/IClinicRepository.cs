using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital_BE.DAL.Models;
using Hospital_BE.PL.DTOs.Common;

namespace Hospital_BE.DAL.Interfaces
{
    public interface IClinicRepository
    {
        Task<(List<Clinic> Items, int TotalCount)> GetAllAsync(QueryParameters parameters);
        Task<Clinic> GetByIdAsync(Guid id);
        Task<Clinic> CreateAsync(Clinic clinic);
        Task<bool> UpdateAsync(Guid id, Clinic clinic);
        Task<bool> DeleteAsync(Guid id);
    }
} 