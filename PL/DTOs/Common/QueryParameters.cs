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
        
        /// <summary>
        /// Lọc theo Role ID
        /// </summary>
        public string? RoleId { get; set; }
        
        /// <summary>
        /// Từ khóa tìm kiếm
        /// </summary>
        public string? Search { get; set; }
        
        /// <summary>
        /// Lọc theo Clinic ID
        /// </summary>
        public Guid? ClinicId { get; set; }
        
        /// <summary>
        /// Lọc theo trạng thái (S1, S2, S3, S4)
        /// </summary>
        public string? Status { get; set; }
        
        /// <summary>
        /// Lọc theo ngày hẹn
        /// </summary>
        public DateTime? AppointmentDate { get; set; }
    }
} 