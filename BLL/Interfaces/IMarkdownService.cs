using System;
using System.Threading.Tasks;
using Hospital_BE.DAL.Models;

namespace Hospital_BE.BLL.Interfaces
{
    public interface IMarkdownService
    {
        Task<Markdown> CreateMarkdownAsync(Markdown markdown);
        Task<Markdown> UpdateMarkdownAsync(Markdown markdown);
        Task<bool> DeleteMarkdownAsync(Guid id);
        Task<Markdown> GetMarkdownByIdAsync(Guid id);
        Task<Markdown> GetMarkdownByDoctorIdAsync(Guid doctorId);
        Task<Markdown> GetMarkdownByClinicIdAsync(Guid clinicId);

        /// <summary>
        /// Lấy Markdown theo DoctorId, nếu không có thì lấy theo ClinicId
        /// </summary>
        Task<Markdown> GetMarkdownByDoctorOrClinicIdAsync(Guid id);
    }
} 