using System.Collections.Generic;

namespace Hospital_BE.PL.DTOs.Common
{
    /// <summary>
    /// Lớp chứa kết quả phân trang generic với kiểu dữ liệu T
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu trả về</typeparam>
    public class PaginatedResult<T>
    {
        /// <summary>
        /// Danh sách dữ liệu
        /// </summary>
        public List<T> Data { get; set; }
        
        /// <summary>
        /// Số trang hiện tại
        /// </summary>
        public int CurrentPage { get; set; }
        
        /// <summary>
        /// Tổng số trang
        /// </summary>
        public int TotalPages { get; set; }
        
        /// <summary>
        /// Số lượng bản ghi trên một trang
        /// </summary>
        public int PageSize { get; set; }
        
        /// <summary>
        /// Tổng số bản ghi
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Kiểm tra xem có trang trước hay không
        /// </summary>
        public bool HasPrevious => CurrentPage > 1;
        
        /// <summary>
        /// Kiểm tra xem có trang sau hay không
        /// </summary>
        public bool HasNext => CurrentPage < TotalPages;

        /// <summary>
        /// Khởi tạo một đối tượng PaginatedResult mới
        /// </summary>
        public PaginatedResult(List<T> data, int totalCount, int pageNumber, int pageSize)
        {
            Data = data;
            TotalCount = totalCount;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)System.Math.Ceiling(totalCount / (double)pageSize);
        }
    }
} 