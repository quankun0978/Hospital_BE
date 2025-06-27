using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Hospital_BE.DAL.Context;
using Hospital_BE.DAL.Interfaces;
using Hospital_BE.DAL.Models;
using Hospital_BE.PL.DTOs.Common;

namespace Hospital_BE.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<User> CreateAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return null;
            }
            return user;
        }

        public async Task<User> GetByIdAsync(Guid userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<(List<User> Items, int TotalCount)> GetAllAsync(QueryParameters parameters)
        {
            var query = _context.Users
                .Include(u => u.Role)
                .OrderBy(u => u.Name)
                .AsQueryable();

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string roleId, QueryParameters queryParams)
        {
            var query = _context.Users
                .Include(u => u.Role)
                .Where(u => u.RoleId == roleId);

            if (!string.IsNullOrEmpty(queryParams.Search))
            {
                query = query.Where(u => u.Name.Contains(queryParams.Search) || 
                                        u.Email.Contains(queryParams.Search) ||
                                        u.Username.Contains(queryParams.Search));
            }

            return await query
                .OrderBy(u => u.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByRoleWithoutDoctorInfoAsync(string roleId, QueryParameters queryParams)
        {
            var query = _context.Users
                .Include(u => u.Role)
                .Where(u => u.RoleId == roleId)
                .Where(u => !_context.DoctorInfos.Any(d => d.DoctorId == u.UserId)); // Chỉ lấy users chưa có DoctorInfo

            if (!string.IsNullOrEmpty(queryParams.Search))
            {
                query = query.Where(u => u.Name.Contains(queryParams.Search) || 
                                        u.Email.Contains(queryParams.Search) ||
                                        u.Username.Contains(queryParams.Search));
            }

            return await query
                .OrderBy(u => u.Name)
                .ToListAsync();
        }
    }
} 