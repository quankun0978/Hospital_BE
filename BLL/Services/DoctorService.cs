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
            
            // Chuyển đổi danh sách entity sang DTO với specialties và clinics
            var doctorDtos = new List<DoctorDto>();
            
            foreach (var doctorInfo in items)
            {
                // Lấy tất cả DoctorInfo của bác sĩ này
                var allDoctorInfos = await _doctorRepository.GetAllDoctorInfosByDoctorIdAsync(doctorInfo.DoctorId);
                
                // Lấy danh sách specialties và clinics thông qua DoctorClinicSpecialty
                var doctorClinicSpecialties = await _doctorRepository.GetDoctorClinicSpecialtiesByDoctorIdAsync(doctorInfo.DoctorId);

                // Group specialties
                var specialties = doctorClinicSpecialties
                    .Where(dcs => dcs.Specialty != null)
                    .GroupBy(dcs => dcs.SpecialtyId)
                    .Select(g => new DoctorSpecialtyDto
                    {
                        SpecialtyId = g.Key,
                        Name = g.First().Specialty.Name,
                        ImageUrl = g.First().Specialty.ImageUrl,
                        Description = g.First().Specialty.Description,
                        Slug = g.First().Specialty.Slug
                    })
                    .ToList();

                // Group clinics
                var clinics = doctorClinicSpecialties
                    .Where(dcs => dcs.Clinic != null)
                    .GroupBy(dcs => dcs.ClinicId)
                    .Select(g => new DoctorClinicDto
                    {
                        ClinicId = g.Key,
                        Name = g.First().Clinic.Name,
                        Address = g.First().Clinic.Address,
                        Description = g.First().Clinic.Description,
                        Slug = g.First().Clinic.Slug,
                        ImageUrl = g.First().Clinic.ImageUrl,
                        LogoImg = g.First().Clinic.LogoImg,
                        IsHospital = g.First().Clinic.IsHospital
                    })
                    .ToList();
                
                doctorDtos.Add(new DoctorDto
                {
                    DoctorId = doctorInfo.Doctor.UserId,
                    Name = doctorInfo.Doctor.Name,
                    Username = doctorInfo.Doctor.Username,
                    Email = doctorInfo.Doctor.Email,
                    RoleId = doctorInfo.Doctor.RoleId,
                    RoleName = doctorInfo.Doctor.Role?.ValueVi,
                    DoctorInfos = allDoctorInfos.Select(di => new DoctorInfoDto
                    {
                        Id = di.Id,
                        DoctorId = di.DoctorId,
                        PriceId = di.PriceId,
                        PriceName = di.Price?.ValueVi,
                        PositionId = di.PositionId,
                        PositionName = di.Position?.ValueVi,
                        ClinicId = di.ClinicId,
                        ClinicName = di.Clinic?.Name,
                        Slug = di.Slug,
                        Note = di.Note,
                        ImageUrl = di.ImageUrl,
                        Count = di.Count
                    }).ToList(),
                    Specialties = specialties,
                    Clinics = clinics
                });
            }
            
            return new PaginatedResult<DoctorDto>(doctorDtos, totalCount, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<DoctorDto> GetDoctorByIdAsync(Guid id)
        {
            var doctorInfo = await _doctorRepository.GetByIdAsync(id);
            if (doctorInfo == null) return null;
            
            // Lấy tất cả DoctorInfo của bác sĩ này
            var allDoctorInfos = await _doctorRepository.GetAllDoctorInfosByDoctorIdAsync(doctorInfo.DoctorId);
            
            // Lấy danh sách specialties và clinics thông qua DoctorClinicSpecialty
            var doctorClinicSpecialties = await _doctorRepository.GetDoctorClinicSpecialtiesByDoctorIdAsync(doctorInfo.DoctorId);

            // Group specialties
            var specialties = doctorClinicSpecialties
                .Where(dcs => dcs.Specialty != null)
                .GroupBy(dcs => dcs.SpecialtyId)
                .Select(g => new DoctorSpecialtyDto
                {
                    SpecialtyId = g.Key,
                    Name = g.First().Specialty.Name,
                    ImageUrl = g.First().Specialty.ImageUrl,
                    Description = g.First().Specialty.Description,
                    Slug = g.First().Specialty.Slug
                })
                .ToList();

            // Group clinics
            var clinics = doctorClinicSpecialties
                .Where(dcs => dcs.Clinic != null)
                .GroupBy(dcs => dcs.ClinicId)
                .Select(g => new DoctorClinicDto
                {
                    ClinicId = g.Key,
                    Name = g.First().Clinic.Name,
                    Address = g.First().Clinic.Address,
                    Description = g.First().Clinic.Description,
                    Slug = g.First().Clinic.Slug,
                    ImageUrl = g.First().Clinic.ImageUrl,
                    LogoImg = g.First().Clinic.LogoImg,
                    IsHospital = g.First().Clinic.IsHospital
                })
                .ToList();
            
            // Chuyển đổi entity sang DTO
            return new DoctorDto
            {
                DoctorId = doctorInfo.Doctor.UserId,
                Name = doctorInfo.Doctor.Name,
                Username = doctorInfo.Doctor.Username,
                Email = doctorInfo.Doctor.Email,
                RoleId = doctorInfo.Doctor.RoleId,
                RoleName = doctorInfo.Doctor.Role?.ValueVi,
                DoctorInfos = allDoctorInfos.Select(di => new DoctorInfoDto
                {
                    Id = di.Id,
                    DoctorId = di.DoctorId,
                    PriceId = di.PriceId,
                    PriceName = di.Price?.ValueVi,
                    PositionId = di.PositionId,
                    PositionName = di.Position?.ValueVi,
                    ClinicId = di.ClinicId,
                    ClinicName = di.Clinic?.Name,
                    Slug = di.Slug,
                    Note = di.Note,
                    ImageUrl = di.ImageUrl,
                    Count = di.Count
                }).ToList(),
                Specialties = specialties,
                Clinics = clinics
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

            // Lấy tất cả DoctorInfo của bác sĩ này
            var allDoctorInfos = await _doctorRepository.GetAllDoctorInfosByDoctorIdAsync(doctorInfo.DoctorId);
            
            // Lấy danh sách specialties và clinics thông qua DoctorClinicSpecialty
            var doctorClinicSpecialties = await _doctorRepository.GetDoctorClinicSpecialtiesByDoctorIdAsync(doctorInfo.DoctorId);

            // Group specialties
            var specialties = doctorClinicSpecialties
                .Where(dcs => dcs.Specialty != null)
                .GroupBy(dcs => dcs.SpecialtyId)
                .Select(g => new DoctorSpecialtyDto
                {
                    SpecialtyId = g.Key,
                    Name = g.First().Specialty.Name,
                    ImageUrl = g.First().Specialty.ImageUrl,
                    Description = g.First().Specialty.Description,
                    Slug = g.First().Specialty.Slug
                })
                .ToList();

            // Group clinics
            var clinics = doctorClinicSpecialties
                .Where(dcs => dcs.Clinic != null)
                .GroupBy(dcs => dcs.ClinicId)
                .Select(g => new DoctorClinicDto
                {
                    ClinicId = g.Key,
                    Name = g.First().Clinic.Name,
                    Address = g.First().Clinic.Address,
                    Description = g.First().Clinic.Description,
                    Slug = g.First().Clinic.Slug,
                    ImageUrl = g.First().Clinic.ImageUrl,
                    LogoImg = g.First().Clinic.LogoImg,
                    IsHospital = g.First().Clinic.IsHospital
                })
                .ToList();

            return new DoctorDto
            {
                DoctorId = doctorInfo.Doctor.UserId,
                Name = doctorInfo.Doctor.Name,
                Username = doctorInfo.Doctor.Username,
                Email = doctorInfo.Doctor.Email,
                RoleId = doctorInfo.Doctor.RoleId,
                RoleName = doctorInfo.Doctor.Role?.ValueVi,
                DoctorInfos = allDoctorInfos.Select(di => new DoctorInfoDto
                {
                    Id = di.Id,
                    DoctorId = di.DoctorId,
                    PriceId = di.PriceId,
                    PriceName = di.Price?.ValueVi,
                    PositionId = di.PositionId,
                    PositionName = di.Position?.ValueVi,
                    ClinicId = di.ClinicId,
                    ClinicName = di.Clinic?.Name,
                    Slug = di.Slug,
                    Note = di.Note,
                    ImageUrl = di.ImageUrl,
                    Count = di.Count
                }).ToList(),
                Specialties = specialties,
                Clinics = clinics
            };
        }
    }
} 