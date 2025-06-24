namespace Hospital_BE.PL.DTOs.Common
{
    /// <summary>
    /// Lớp chứa các tham số truy vấn chung, mở rộng từ PaginationParameters
    /// </summary>
    public class QueryParameters : PaginationParameters
    {
        /// <summary>
        /// Từ khóa tìm kiếm
        /// </summary>
        public string? SearchTerm { get; set; }
        
        /// <summary>
        /// Trường sắp xếp
        /// </summary>
        public string? SortBy { get; set; } = "UpdatedAt";
        
        /// <summary>
        /// Hướng sắp xếp (asc, desc)
        /// </summary>
        public string? SortOrder { get; set; } = "desc";
    }
} 