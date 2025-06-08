using System;
using System.Linq;
using System.Threading.Tasks;
using Hospital_BE.BLL.Interfaces;
using Hospital_BE.DAL.Interfaces;
using Hospital_BE.DAL.Models;
using Hospital_BE.PL.DTOs;
using Hospital_BE.PL.DTOs.Common;

namespace Hospital_BE.BLL.Services
{
    public class SpecialtyService : ISpecialtyService
    {
        private readonly ISpecialtyRepository _specialtyRepository;

        public SpecialtyService(ISpecialtyRepository specialtyRepository)
        {
            _specialtyRepository = specialtyRepository;
        }

        public async Task<PaginatedResult<SpecialtyResponseDTO>> GetAllSpecialtiesAsync(PaginationParameters parameters)
        {
            var (items, totalCount) = await _specialtyRepository.GetAllAsync(parameters);
            
            var responseDTOs = items.Select(s => new SpecialtyResponseDTO
            {
                SpecialtyId = s.SpecialtyId,
                Name = s.Name,
                ImageUrl = s.ImageUrl,
                Description = s.Description,
                Slug = s.Slug
            }).ToList();

            return new PaginatedResult<SpecialtyResponseDTO>(responseDTOs, totalCount, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<SpecialtyResponseDTO> GetSpecialtyByIdAsync(Guid id)
        {
            var specialty = await _specialtyRepository.GetByIdAsync(id);
            if (specialty == null)
                return null;

            return new SpecialtyResponseDTO
            {
                SpecialtyId = specialty.SpecialtyId,
                Name = specialty.Name,
                ImageUrl = specialty.ImageUrl,
                Description = specialty.Description,
                Slug = specialty.Slug
            };
        }

        public async Task<SpecialtyResponseDTO> CreateSpecialtyAsync(CreateSpecialtyDTO createDto)
        {
            // Kiểm tra tên chuyên khoa đã tồn tại chưa
            if (await _specialtyRepository.ExistsByNameAsync(createDto.Name))
                throw new InvalidOperationException("Tên chuyên khoa đã tồn tại");

            var specialty = new Specialty
            {
                SpecialtyId = Guid.NewGuid(),
                Name = createDto.Name,
                ImageUrl = createDto.ImageUrl,
                Description = createDto.Description,
                Slug = createDto.Slug ?? GenerateSlugFromName(createDto.Name)
            };

            var createdSpecialty = await _specialtyRepository.CreateAsync(specialty);

            return new SpecialtyResponseDTO
            {
                SpecialtyId = createdSpecialty.SpecialtyId,
                Name = createdSpecialty.Name,
                ImageUrl = createdSpecialty.ImageUrl,
                Description = createdSpecialty.Description,
                Slug = createdSpecialty.Slug
            };
        }

        public async Task<SpecialtyResponseDTO> UpdateSpecialtyAsync(Guid id, UpdateSpecialtyDTO updateDto)
        {
            var existingSpecialty = await _specialtyRepository.GetByIdAsync(id);
            if (existingSpecialty == null)
                return null;

            // Kiểm tra tên chuyên khoa đã tồn tại chưa (trừ chính nó)
            if (!string.IsNullOrEmpty(updateDto.Name) && 
                await _specialtyRepository.ExistsByNameAsync(updateDto.Name, id))
                throw new InvalidOperationException("Tên chuyên khoa đã tồn tại");

            // Cập nhật các thuộc tính nếu có giá trị
            if (!string.IsNullOrEmpty(updateDto.Name))
                existingSpecialty.Name = updateDto.Name;
            
            if (!string.IsNullOrEmpty(updateDto.ImageUrl))
                existingSpecialty.ImageUrl = updateDto.ImageUrl;
            
            if (updateDto.Description != null)
                existingSpecialty.Description = updateDto.Description;

            if (!string.IsNullOrEmpty(updateDto.Slug))
                existingSpecialty.Slug = updateDto.Slug;

            var updatedSpecialty = await _specialtyRepository.UpdateAsync(existingSpecialty);

            return new SpecialtyResponseDTO
            {
                SpecialtyId = updatedSpecialty.SpecialtyId,
                Name = updatedSpecialty.Name,
                ImageUrl = updatedSpecialty.ImageUrl,
                Description = updatedSpecialty.Description,
                Slug = updatedSpecialty.Slug
            };
        }

        public async Task<bool> DeleteSpecialtyAsync(Guid id)
        {
            return await _specialtyRepository.DeleteAsync(id);
        }

        private string GenerateSlugFromName(string name)
        {
            // Tạo slug đơn giản từ tên chuyên khoa
            return name?.ToLower()
                .Replace(" ", "-")
                .Replace("ă", "a")
                .Replace("â", "a")
                .Replace("đ", "d")
                .Replace("ê", "e")
                .Replace("ô", "o")
                .Replace("ơ", "o")
                .Replace("ư", "u")
                .Replace("à", "a")
                .Replace("á", "a")
                .Replace("ạ", "a")
                .Replace("ả", "a")
                .Replace("ã", "a")
                .Replace("è", "e")
                .Replace("é", "e")
                .Replace("ẹ", "e")
                .Replace("ẻ", "e")
                .Replace("ẽ", "e")
                .Replace("ì", "i")
                .Replace("í", "i")
                .Replace("ị", "i")
                .Replace("ỉ", "i")
                .Replace("ĩ", "i")
                .Replace("ò", "o")
                .Replace("ó", "o")
                .Replace("ọ", "o")
                .Replace("ỏ", "o")
                .Replace("õ", "o")
                .Replace("ù", "u")
                .Replace("ú", "u")
                .Replace("ụ", "u")
                .Replace("ủ", "u")
                .Replace("ũ", "u")
                .Replace("ỳ", "y")
                .Replace("ý", "y")
                .Replace("ỵ", "y")
                .Replace("ỷ", "y")
                .Replace("ỹ", "y");
        }
    }
} 