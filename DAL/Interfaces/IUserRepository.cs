using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hospital_BE.DAL.Models;
using Hospital_BE.PL.DTOs.Common;

namespace Hospital_BE.DAL.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByUsernameAsync(string username);
        Task<User> CreateAsync(User user);
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByIdAsync(Guid userId);
        Task<(List<User> Items, int TotalCount)> GetAllAsync(QueryParameters parameters);
        Task SaveChangesAsync();
        Task<User> GetByUsernameAsync(string username);
        Task<IEnumerable<User>> GetUsersByRoleAsync(string roleId, QueryParameters queryParams);
        Task<IEnumerable<User>> GetUsersByRoleWithoutDoctorInfoAsync(string roleId, QueryParameters queryParams);
    }
} 