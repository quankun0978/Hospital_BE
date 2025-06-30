using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Hospital_BE.BLL.Interfaces
{
    public interface IZipService
    {
        /// <summary>
        /// Tạo file zip từ danh sách file
        /// </summary>
        /// <param name="filePaths">Danh sách đường dẫn file</param>
        /// <param name="zipFileName">Tên file zip</param>
        /// <returns>Stream của file zip</returns>
        Task<MemoryStream> CreateZipFromFilesAsync(List<string> filePaths, string zipFileName = "medical_results.zip");

        /// <summary>
        /// Tạo file zip từ danh sách URL ảnh
        /// </summary>
        /// <param name="imageUrls">Danh sách URL ảnh</param>
        /// <param name="zipFileName">Tên file zip</param>
        /// <returns>Stream của file zip</returns>
        Task<MemoryStream> CreateZipFromImageUrlsAsync(List<string> imageUrls, string zipFileName = "medical_results.zip");

        /// <summary>
        /// Xóa file zip tạm thời
        /// </summary>
        /// <param name="zipFilePath">Đường dẫn file zip</param>
        void CleanupTempFile(string zipFilePath);
    }
} 