using System.Collections.Generic;

namespace Hospital_BE.PL.DTOs.Common
{
    /// <summary>
    /// Lớp đại diện cho phản hồi API tiêu chuẩn
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu của dữ liệu trả về</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Trạng thái thành công của phản hồi
        /// </summary>
        public bool Succeeded { get; set; }

        /// <summary>
        /// Thông điệp từ server
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Danh sách các lỗi (nếu có)
        /// </summary>
        public string[] Errors { get; set; }

        /// <summary>
        /// Dữ liệu trả về
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Tạo một phản hồi thành công với dữ liệu
        /// </summary>
        /// <param name="data">Dữ liệu trả về</param>
        /// <param name="message">Thông điệp (tùy chọn)</param>
        /// <returns>Phản hồi API thành công</returns>
        public static ApiResponse<T> Success(T data, string message = null)
        {
            return new ApiResponse<T>
            {
                Succeeded = true,
                Message = message,
                Data = data
            };
        }

        /// <summary>
        /// Tạo một phản hồi thất bại với thông báo lỗi
        /// </summary>
        /// <param name="errors">Danh sách các lỗi</param>
        /// <returns>Phản hồi API thất bại</returns>
        public static ApiResponse<T> Failure(IEnumerable<string> errors)
        {
            return new ApiResponse<T>
            {
                Succeeded = false,
                Errors = errors as string[] ?? new List<string>(errors).ToArray()
            };
        }

        /// <summary>
        /// Tạo một phản hồi thất bại với một thông báo lỗi
        /// </summary>
        /// <param name="error">Thông báo lỗi</param>
        /// <returns>Phản hồi API thất bại</returns>
        public static ApiResponse<T> Failure(string error)
        {
            return new ApiResponse<T>
            {
                Succeeded = false,
                Errors = new string[] { error }
            };
        }
    }
} 