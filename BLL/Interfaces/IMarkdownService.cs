using System;
using System.Threading.Tasks;
using Hospital_BE.DAL.Models;

namespace Hospital_BE.BLL.Interfaces
{
    public interface IMarkdownService
    {
        Task<Markdown> GetMarkdownByIdAsync(Guid id);

        /// <summary>
        /// Lấy Markdown theo DoctorId, nếu không có thì lấy theo ClinicId
        /// </summary>
        Task<Markdown> GetMarkdownByDoctorOrClinicIdAsync(Guid id);
    }
} 