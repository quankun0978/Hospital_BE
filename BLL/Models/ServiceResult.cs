namespace Hospital_BE.BLL.Models
{
    public class ServiceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        public static ServiceResult Ok(string message = "Thành công", object data = null)
        {
            return new ServiceResult
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ServiceResult Error(string message = "Có lỗi xảy ra", object data = null)
        {
            return new ServiceResult
            {
                Success = false,
                Message = message,
                Data = data
            };
        }
    }

    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public static ServiceResult<T> Ok(string message = "Thành công", T data = default)
        {
            return new ServiceResult<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ServiceResult<T> Error(string message = "Có lỗi xảy ra", T data = default)
        {
            return new ServiceResult<T>
            {
                Success = false,
                Message = message,
                Data = data
            };
        }
    }
} 