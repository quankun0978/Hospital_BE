using System;
using System.Threading.Tasks;
using Hospital_BE.DAL.Models;

namespace Hospital_BE.DAL.Interfaces
{
    public interface IPatientRecordRepository
    {
        Task<PatientRecord> CreateAsync(PatientRecord patientRecord);
        Task<PatientRecord> GetByIdAsync(Guid patientId);
        Task<PatientRecord> GetByUserIdAsync(Guid userId);
    }
} 