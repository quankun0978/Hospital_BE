using Hospital_BE.DAL.Models;
using System;
using System.Threading.Tasks;

namespace Hospital_BE.DAL.Interfaces
{
    public interface IMarkdownRepository
    {
        Task<Markdown> CreateAsync(Markdown markdown);
        Task<Markdown> UpdateAsync(Markdown markdown);
        Task<bool> DeleteAsync(Guid id);
        Task<Markdown> GetByIdAsync(Guid id);
        Task<Markdown> GetByDoctorIdAsync(Guid doctorId);
        Task<Markdown> GetByClinicIdAsync(Guid clinicId);
    }
} 