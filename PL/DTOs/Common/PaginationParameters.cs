namespace Hospital_BE.PL.DTOs.Common
{
    /// <summary>
    /// Lớp cơ sở chứa các tham số phân trang
    /// </summary>
    public class PaginationParameters
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 10;

        /// <summary>
        /// Số trang hiện tại, mặc định là 1
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Số lượng bản ghi trên một trang
        /// Giới hạn tối đa là MaxPageSize (50)
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
    }
} 