using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Hospital_BE.DAL.Context;
using Hospital_BE.DAL.Interfaces;
using Hospital_BE.DAL.Models;

namespace Hospital_BE.DAL.Repositories
{
    public class PatientRecordRepository : IPatientRecordRepository
    {
        private readonly ApplicationDbContext _context;

        public PatientRecordRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PatientRecord> CreateAsync(PatientRecord patientRecord)
        {
            await _context.PatientRecords.AddAsync(patientRecord);
            await _context.SaveChangesAsync();
            return patientRecord;
        }

        public async Task<PatientRecord> GetByIdAsync(Guid patientId)
        {
            return await _context.PatientRecords.FindAsync(patientId);
        }

        public async Task<PatientRecord> GetByUserIdAsync(Guid userId)
        {
            return await _context.PatientRecords
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }
    }
} 