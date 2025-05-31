using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.Json;

namespace Hospital_BE.PL.DTOs.Common
{
    /// <summary>
    /// Lớp helper cung cấp các phương thức xử lý phản hồi API
    /// </summary>
    public static class ApiResponseHelper
    {
        /// <summary>
        /// Tạo phản hồi thành công với dữ liệu
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của kết quả</typeparam>
        /// <param name="controller">Controller gốc</param>
        /// <param name="data">Dữ liệu trả về</param>
        /// <param name="message">Thông điệp (tùy chọn)</param>
        /// <returns>Kết quả API thành công</returns>
        public static IActionResult Ok<T>(ControllerBase controller, T data, string message = null)
        {
            return controller.Ok(ApiResponse<T>.Success(data, message));
        }
        
        /// <summary>
        /// Tạo phản hồi lỗi 400 Bad Request
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của kết quả</typeparam>
        /// <param name="controller">Controller gốc</param>
        /// <param name="error">Thông báo lỗi</param>
        /// <returns>Kết quả API lỗi</returns>
        public static IActionResult BadRequest<T>(ControllerBase controller, string error)
        {
            return controller.BadRequest(ApiResponse<T>.Failure(error));
        }
        
        /// <summary>
        /// Tạo phản hồi lỗi 400 Bad Request với nhiều lỗi
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của kết quả</typeparam>
        /// <param name="controller">Controller gốc</param>
        /// <param name="errors">Danh sách lỗi</param>
        /// <returns>Kết quả API lỗi</returns>
        public static IActionResult BadRequest<T>(ControllerBase controller, IEnumerable<string> errors)
        {
            return controller.BadRequest(ApiResponse<T>.Failure(errors));
        }
        
        /// <summary>
        /// Tạo phản hồi lỗi 404 Not Found
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của kết quả</typeparam>
        /// <param name="controller">Controller gốc</param>
        /// <param name="message">Thông báo lỗi</param>
        /// <returns>Kết quả API lỗi Not Found</returns>
        public static IActionResult NotFound<T>(ControllerBase controller, string message)
        {
            return controller.NotFound(ApiResponse<T>.Failure(message));
        }
        
        /// <summary>
        /// Tạo phản hồi thành công 201 Created
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của kết quả</typeparam>
        /// <param name="controller">Controller gốc</param>
        /// <param name="data">Dữ liệu trả về</param>
        /// <param name="actionName">Tên action để tạo URI</param>
        /// <param name="routeValues">Các tham số của route</param>
        /// <param name="message">Thông điệp (tùy chọn)</param>
        /// <returns>Kết quả API Created</returns>
        public static IActionResult Created<T>(ControllerBase controller, T data, string actionName, object routeValues, string message = null)
        {
            var response = ApiResponse<T>.Success(data, message ?? "Tạo mới thành công");
            return controller.CreatedAtAction(actionName, routeValues, response);
        }
        
        /// <summary>
        /// Tạo phản hồi thành công 204 No Content
        /// </summary>
        /// <param name="controller">Controller gốc</param>
        /// <returns>Kết quả API No Content</returns>
        public static IActionResult NoContent(ControllerBase controller)
        {
            return controller.NoContent();
        }
        
        /// <summary>
        /// Thêm thông tin phân trang vào header của phản hồi
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của kết quả phân trang</typeparam>
        /// <param name="controller">Controller gốc</param>
        /// <param name="result">Kết quả phân trang</param>
        public static void AddPaginationHeader<T>(ControllerBase controller, PaginatedResult<T> result)
        {
            var paginationMetadata = new
            {
                result.TotalCount,
                result.PageSize,
                result.CurrentPage,
                result.TotalPages,
                result.HasNext,
                result.HasPrevious
            };
            
            controller.Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
            controller.Response.Headers.Add("Access-Control-Expose-Headers", "X-Pagination");
        }
    }
} 