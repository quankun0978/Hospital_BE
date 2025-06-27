using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital_BE.DAL.Models;

namespace Hospital_BE.DAL.Interfaces
{
    public interface IDoctorClinicSpecialtyRepository
    {
        Task<List<DoctorClinicSpecialty>> GetByDoctorIdAsync(Guid doctorId);
        Task<DoctorClinicSpecialty> CreateAsync(DoctorClinicSpecialty entity);
        Task DeleteByDoctorIdAsync(Guid doctorId);
        Task<bool> ExistsAsync(Guid doctorId, Guid clinicId, Guid specialtyId);
    }
} 