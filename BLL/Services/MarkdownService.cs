using System;
using System.Threading.Tasks;
using Hospital_BE.BLL.Interfaces;
using Hospital_BE.DAL.Interfaces;
using Hospital_BE.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Hospital_BE.BLL.Services
{
    public class MarkdownService : IMarkdownService
    {
        private readonly IMarkdownRepository _markdownRepository;

        public MarkdownService(IMarkdownRepository markdownRepository)
        {
            _markdownRepository = markdownRepository;
        }

        public async Task<Markdown> CreateMarkdownAsync(Markdown markdown)
        {
            if (markdown == null)
                throw new ArgumentNullException(nameof(markdown));

            // Validate required fields
            if (string.IsNullOrEmpty(markdown.ContentHTML) && string.IsNullOrEmpty(markdown.ContentMarkdown))
                throw new ArgumentException("Content HTML hoặc Content Markdown là bắt buộc");

            // Ensure only one of DoctorId or ClinicId is set
            if (markdown.DoctorId.HasValue && markdown.ClinicId.HasValue)
                throw new ArgumentException("Không thể gán cả DoctorId và ClinicId cho một markdown");

            if (!markdown.DoctorId.HasValue && !markdown.ClinicId.HasValue)
                throw new ArgumentException("Phải có DoctorId hoặc ClinicId");

            return await _markdownRepository.CreateAsync(markdown);
        }

        public async Task<Markdown> UpdateMarkdownAsync(Markdown markdown)
        {
            if (markdown == null)
                throw new ArgumentNullException(nameof(markdown));

            var existingMarkdown = await _markdownRepository.GetByIdAsync(markdown.Id);
            if (existingMarkdown == null)
                throw new ArgumentException("Không tìm thấy markdown");

            // Update fields
            existingMarkdown.ContentHTML = markdown.ContentHTML;
            existingMarkdown.ContentMarkdown = markdown.ContentMarkdown;
            existingMarkdown.Description = markdown.Description;

            return await _markdownRepository.UpdateAsync(existingMarkdown);
        }

        public async Task<bool> DeleteMarkdownAsync(Guid id)
        {
            return await _markdownRepository.DeleteAsync(id);
        }

        public async Task<Markdown> GetMarkdownByIdAsync(Guid id)
        {
            return await _markdownRepository.GetByIdAsync(id);
        }

        public async Task<Markdown> GetMarkdownByDoctorIdAsync(Guid doctorId)
        {
            return await _markdownRepository.GetByDoctorIdAsync(doctorId);
        }

        public async Task<Markdown> GetMarkdownByClinicIdAsync(Guid clinicId)
        {
            return await _markdownRepository.GetByClinicIdAsync(clinicId);
        }

        public async Task<Markdown> GetMarkdownByDoctorOrClinicIdAsync(Guid id)
        {
            // Ưu tiên lấy theo DoctorId
            var markdown = await _markdownRepository.GetByDoctorIdAsync(id);
            if (markdown != null)
                return markdown;
            // Nếu không có, lấy theo ClinicId
            return await _markdownRepository.GetByClinicIdAsync(id);
        }
    }
} 