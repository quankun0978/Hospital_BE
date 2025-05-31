using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital_BE.BLL.Interfaces;
using Hospital_BE.DAL.Interfaces;
using Hospital_BE.DAL.Models;
using Hospital_BE.PL.DTOs;
using Hospital_BE.PL.DTOs.Common;

namespace Hospital_BE.BLL.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;

        public DoctorService(IDoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        public async Task<PaginatedResult<DoctorDto>> GetDoctorsAsync(QueryParameters parameters)
        {
            var (items, totalCount) = await _doctorRepository.GetAllAsync(parameters);
            
            // Chuyển đổi danh sách entity sang DTO
            var doctorDtos = items.Select(doctorInfo => new DoctorDto
            {
                DoctorId = doctorInfo.Doctor.UserId,
                Name = doctorInfo.Doctor.Name,
                Username = doctorInfo.Doctor.Username,
                Phone = doctorInfo.Doctor.Phone,
                RoleId = doctorInfo.Doctor.RoleId,
                RoleName = doctorInfo.Doctor.Role?.ValueVi,
                DoctorInfos = new List<DoctorInfoDto>
                {
                    new DoctorInfoDto
                    {
                        Id = doctorInfo.Id,
                        DoctorId = doctorInfo.DoctorId,
                        PriceId = doctorInfo.PriceId,
                        PriceName = doctorInfo.Price?.ValueVi,
                        PositionId = doctorInfo.PositionId,
                        PositionName = doctorInfo.Position?.ValueVi,
                        ClinicId = doctorInfo.ClinicId,
                        ClinicName = doctorInfo.Clinic?.Name,
                        Slug = doctorInfo.Slug,
                        Note = doctorInfo.Note,
                        ImageUrl = doctorInfo.ImageUrl,
                        Count = doctorInfo.Count
                    }
                }
            }).ToList();
            
            return new PaginatedResult<DoctorDto>(doctorDtos, totalCount, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<DoctorDto> GetDoctorByIdAsync(Guid id)
        {
            var doctorInfo = await _doctorRepository.GetByIdAsync(id);
            if (doctorInfo == null) return null;
            
            // Chuyển đổi entity sang DTO
            return new DoctorDto
            {
                DoctorId = doctorInfo.Doctor.UserId,
                Name = doctorInfo.Doctor.Name,
                Username = doctorInfo.Doctor.Username,
                Phone = doctorInfo.Doctor.Phone,
                RoleId = doctorInfo.Doctor.RoleId,
                RoleName = doctorInfo.Doctor.Role?.ValueVi,
                DoctorInfos = new List<DoctorInfoDto>
                {
                    new DoctorInfoDto
                    {
                        Id = doctorInfo.Id,
                        DoctorId = doctorInfo.DoctorId,
                        PriceId = doctorInfo.PriceId,
                        PriceName = doctorInfo.Price?.ValueVi,
                        PositionId = doctorInfo.PositionId,
                        PositionName = doctorInfo.Position?.ValueVi,
                        ClinicId = doctorInfo.ClinicId,
                        ClinicName = doctorInfo.Clinic?.Name,
                        Slug = doctorInfo.Slug,
                        Note = doctorInfo.Note,
                        ImageUrl = doctorInfo.ImageUrl,
                        Count = doctorInfo.Count
                    }
                }
            };
        }

        public async Task<DoctorInfo> CreateDoctorInfoAsync(DoctorInfo doctorInfo)
        {
            return await _doctorRepository.CreateAsync(doctorInfo);
        }

        public async Task<bool> UpdateDoctorInfoAsync(Guid id, DoctorInfo doctorInfo)
        {
            return await _doctorRepository.UpdateAsync(id, doctorInfo);
        }

        public async Task<bool> DeleteDoctorInfoAsync(Guid id)
        {
            return await _doctorRepository.DeleteAsync(id);
        }

        public async Task<DoctorDto> GetDoctorBySlugAsync(string slug)
        {
            var doctorInfo = await _doctorRepository.GetBySlugAsync(slug);
            if (doctorInfo == null) return null;

            return new DoctorDto
            {
                DoctorId = doctorInfo.Doctor.UserId,
                Name = doctorInfo.Doctor.Name,
                Username = doctorInfo.Doctor.Username,
                Phone = doctorInfo.Doctor.Phone,
                RoleId = doctorInfo.Doctor.RoleId,
                RoleName = doctorInfo.Doctor.Role?.ValueVi,
                DoctorInfos = new List<DoctorInfoDto>
                {
                    new DoctorInfoDto
                    {
                        Id = doctorInfo.Id,
                        DoctorId = doctorInfo.DoctorId,
                        PriceId = doctorInfo.PriceId,
                        PriceName = doctorInfo.Price?.ValueVi,
                        PositionId = doctorInfo.PositionId,
                        PositionName = doctorInfo.Position?.ValueVi,
                        ClinicId = doctorInfo.ClinicId,
                        ClinicName = doctorInfo.Clinic?.Name,
                        Slug = doctorInfo.Slug,
                        Note = doctorInfo.Note,
                        ImageUrl = doctorInfo.ImageUrl,
                        Count = doctorInfo.Count
                    }
                }
            };
        }
    }
} 