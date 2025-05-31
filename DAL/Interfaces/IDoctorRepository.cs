using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital_BE.DAL.Models;
using Hospital_BE.PL.DTOs.Common;

namespace Hospital_BE.DAL.Interfaces
{
    public interface IDoctorRepository
    {
        Task<(List<DoctorInfo> Items, int TotalCount)> GetAllAsync(QueryParameters parameters);
        Task<DoctorInfo> GetByIdAsync(Guid id);
        Task<DoctorInfo> GetBySlugAsync(string slug);
        Task<DoctorInfo> CreateAsync(DoctorInfo doctorInfo);
        Task<bool> UpdateAsync(Guid id, DoctorInfo doctorInfo);
        Task<bool> DeleteAsync(Guid id);
    }
} 