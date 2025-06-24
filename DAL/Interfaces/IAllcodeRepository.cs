using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hospital_BE.DAL.Models;
using Hospital_BE.PL.DTOs.Common;

namespace Hospital_BE.DAL.Interfaces
{
    public interface IAllcodeRepository
    {
        Task<(List<Allcode> Items, int TotalCount)> GetByTypeAsync(string codeType, PaginationParameters parameters);
        Task<List<Allcode>> GetByTypeWithoutPaginationAsync(string codeType);
        Task<List<string>> GetAllCodeTypesAsync();
        Task<(List<Allcode> Items, int TotalCount)> GetAllCodesAsync(QueryParameters parameters);
        Task<Allcode> GetByIdAsync(int id);
        Task<Allcode> CreateAsync(Allcode allcode);
        Task<Allcode> UpdateAsync(Allcode allcode);
        Task<bool> DeleteAsync(int id);
    }
} 