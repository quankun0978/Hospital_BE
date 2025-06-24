using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Security.Claims;
using Hospital_BE.PL.DTOs.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hospital_BE.PL.Controllers.Base
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// Lấy ID của user hiện tại từ token
        /// </summary>
        /// <returns>User ID</returns>
        protected string GetCurrentUserId()
        {
            return User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }

        /// <summary>
        /// Lấy ID của user hiện tại từ token dưới dạng Guid
        /// </summary>
        /// <returns>User ID as Guid</returns>
        protected Guid GetCurrentUserIdAsGuid()
        {
            var userIdString = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdString, out var userId))
            {
                return userId;
            }
            return Guid.Empty;
        }

        /// <summary>
        /// Lấy email của user hiện tại từ token
        /// </summary>
        /// <returns>Email</returns>
        protected string GetCurrentUserEmail()
        {
            return User?.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
        }

        /// <summary>
        /// Lấy tên của user hiện tại từ token
        /// </summary>
        /// <returns>User name</returns>
        protected string GetCurrentUserName()
        {
            return User?.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
        }

        /// <summary>
        /// Lấy role của user hiện tại từ token
        /// </summary>
        /// <returns>Role</returns>
        protected string GetCurrentUserRole()
        {
            return User?.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
        }

        /// <summary>
        /// Trả về kết quả phân trang cho một truy vấn IQueryable
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của entity</typeparam>
        /// <typeparam name="TResult">Kiểu dữ liệu kết quả</typeparam>
        /// <param name="query">Truy vấn IQueryable</param>
        /// <param name="parameters">Tham số phân trang</param>
        /// <param name="selector">Biểu thức chọn dữ liệu</param>
        /// <returns>Kết quả phân trang</returns>
        protected async Task<PaginatedResult<TResult>> GetPagedResultAsync<T, TResult>(
            IQueryable<T> query,
            PaginationParameters parameters,
            Expression<Func<T, TResult>> selector)
            where T : class
        {
            var totalCount = await query.CountAsync();
            
            var items = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .Select(selector)
                .ToListAsync();
                
            return new PaginatedResult<TResult>(items, totalCount, parameters.PageNumber, parameters.PageSize);
        }
        
        /// <summary>
        /// Trả về một phản hồi API thành công với dữ liệu
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của kết quả</typeparam>
        /// <param name="data">Dữ liệu trả về</param>
        /// <param name="message">Thông điệp (tùy chọn)</param>
        /// <returns>Kết quả API thành công</returns>
        protected IActionResult ApiOk<T>(T data, string message = null)
        {
            return ApiResponseHelper.Ok(this, data, message);
        }
        
        /// <summary>
        /// Trả về một phản hồi API thất bại
        /// </summary>
        /// <param name="error">Thông báo lỗi</param>
        /// <returns>Kết quả API thất bại</returns>
        protected IActionResult ApiBadRequest(string error)
        {
            return ApiResponseHelper.BadRequest<object>(this, error);
        }
        
        /// <summary>
        /// Trả về một phản hồi API thất bại
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của kết quả</typeparam>
        /// <param name="error">Thông báo lỗi</param>
        /// <returns>Kết quả API thất bại</returns>
        protected IActionResult ApiBadRequest<T>(string error)
        {
            return ApiResponseHelper.BadRequest<T>(this, error);
        }
        
        /// <summary>
        /// Trả về một phản hồi API thất bại với nhiều lỗi
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của kết quả</typeparam>
        /// <param name="errors">Danh sách lỗi</param>
        /// <returns>Kết quả API thất bại</returns>
        protected IActionResult ApiBadRequest<T>(IEnumerable<string> errors)
        {
            return ApiResponseHelper.BadRequest<T>(this, errors);
        }
        
        /// <summary>
        /// Trả về một phản hồi API 404 Not Found với thông báo
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của kết quả</typeparam>
        /// <param name="message">Thông báo lỗi</param>
        /// <returns>Kết quả API Not Found</returns>
        protected IActionResult ApiNotFound<T>(string message)
        {
            return ApiResponseHelper.NotFound<T>(this, message);
        }
        
        /// <summary>
        /// Trả về một phản hồi API 201 Created với dữ liệu đã tạo và URI
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của kết quả</typeparam>
        /// <param name="data">Dữ liệu trả về</param>
        /// <param name="actionName">Tên action để tạo URI (thường là "Get")</param>
        /// <param name="routeValues">Các tham số của route</param>
        /// <param name="message">Thông điệp (tùy chọn)</param>
        /// <returns>Kết quả API Created</returns>
        protected IActionResult ApiCreated<T>(T data, string actionName, object routeValues, string message = null)
        {
            return ApiResponseHelper.Created(this, data, actionName, routeValues, message);
        }
        
        /// <summary>
        /// Trả về một phản hồi API 204 No Content khi xóa thành công
        /// </summary>
        /// <returns>Kết quả API No Content</returns>
        protected IActionResult ApiNoContent()
        {
            return ApiResponseHelper.NoContent(this);
        }
        
        /// <summary>
        /// Thêm thông tin phân trang vào header của phản hồi
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của kết quả phân trang</typeparam>
        /// <param name="result">Kết quả phân trang</param>
        protected void AddPaginationHeader<T>(PaginatedResult<T> result)
        {
            ApiResponseHelper.AddPaginationHeader(this, result);
        }
        
        /// <summary>
        /// Trả về một phản hồi API 403 Forbidden
        /// </summary>
        /// <param name="message">Thông báo lỗi</param>
        /// <returns>Kết quả API Forbidden</returns>
        protected IActionResult ApiForbidden(string message)
        {
            return ApiResponseHelper.Forbidden<object>(this, message);
        }
        
        /// <summary>
        /// Trả về một phản hồi API 403 Forbidden
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của kết quả</typeparam>
        /// <param name="message">Thông báo lỗi</param>
        /// <returns>Kết quả API Forbidden</returns>
        protected IActionResult ApiForbidden<T>(string message)
        {
            return ApiResponseHelper.Forbidden<T>(this, message);
        }
    }
} 