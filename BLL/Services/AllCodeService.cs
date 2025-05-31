using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital_BE.BLL.Interfaces;
using Hospital_BE.DAL.Interfaces;
using Hospital_BE.DAL.Models;
using Hospital_BE.PL.DTOs.Common;

namespace Hospital_BE.BLL.Services
{
    public class AllCodeService : IAllCodeService
    {
        private readonly IAllcodeRepository _allcodeRepository;

        public AllCodeService(IAllcodeRepository allcodeRepository)
        {
            _allcodeRepository = allcodeRepository;
        }

        public async Task<PaginatedResult<Allcode>> GetAllCodesByTypeAsync(string codeType, PaginationParameters parameters)
        {
            var (items, totalCount) = await _allcodeRepository.GetByTypeAsync(codeType, parameters);
            return new PaginatedResult<Allcode>(items, totalCount, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<List<string>> GetAllCodeTypesAsync()
        {
            return await _allcodeRepository.GetAllCodeTypesAsync();
        }

        public async Task<PaginatedResult<Allcode>> GetAllCodesAsync(QueryParameters parameters)
        {
            var (items, totalCount) = await _allcodeRepository.GetAllCodesAsync(parameters);
            return new PaginatedResult<Allcode>(items, totalCount, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<Allcode> GetAllCodeByIdAsync(int id)
        {
            return await _allcodeRepository.GetByIdAsync(id);
        }

        public async Task<Allcode> CreateAllcodeAsync(Allcode allcode)
        {
            return await _allcodeRepository.CreateAsync(allcode);
        }

        public async Task<bool> UpdateAllcodeAsync(int id, Allcode allcode)
        {
            var existingAllcode = await _allcodeRepository.GetByIdAsync(id);
            if (existingAllcode == null)
                return false;

            existingAllcode.CodeKey = allcode.CodeKey;
            existingAllcode.CodeType = allcode.CodeType;
            existingAllcode.ValueEn = allcode.ValueEn;
            existingAllcode.ValueVi = allcode.ValueVi;

            await _allcodeRepository.UpdateAsync(existingAllcode);
            return true;
        }

        public async Task<bool> DeleteAllcodeAsync(int id)
        {
            return await _allcodeRepository.DeleteAsync(id);
        }
    }
} 