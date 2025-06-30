using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Hospital_BE.BLL.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Hospital_BE.BLL.Services
{
    public class ZipService : IZipService
    {
        private readonly IConfiguration _configuration;
        private readonly string _staticPath;

        public ZipService(IConfiguration configuration)
        {
            _configuration = configuration;
            _staticPath = Path.Combine(Directory.GetCurrentDirectory(), "PL", "static");
        }

        public async Task<MemoryStream> CreateZipFromFilesAsync(List<string> filePaths, string zipFileName = "medical_results.zip")
        {
            var memoryStream = new MemoryStream();
            
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var filePath in filePaths)
                {
                    if (File.Exists(filePath))
                    {
                        var fileName = Path.GetFileName(filePath);
                        var entry = archive.CreateEntry(fileName, CompressionLevel.Optimal);
                        
                        using (var entryStream = entry.Open())
                        using (var fileStream = File.OpenRead(filePath))
                        {
                            await fileStream.CopyToAsync(entryStream);
                        }
                    }
                }
            }
            
            memoryStream.Position = 0;
            return memoryStream;
        }

        public async Task<MemoryStream> CreateZipFromImageUrlsAsync(List<string> imageUrls, string zipFileName = "medical_results.zip")
        {
            var memoryStream = new MemoryStream();
            
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                for (int i = 0; i < imageUrls.Count; i++)
                {
                    var imageUrl = imageUrls[i];
                    try
                    {
                        // Convert URL to file path
                        var filePath = ConvertUrlToFilePath(imageUrl);
                        
                        if (File.Exists(filePath))
                        {
                            // Tạo tên file có thứ tự và mô tả
                            var fileExtension = Path.GetExtension(filePath);
                            var fileName = $"ket_qua_kham_{i + 1:D2}{fileExtension}";
                            
                            var entry = archive.CreateEntry(fileName, CompressionLevel.Optimal);
                            
                            using (var entryStream = entry.Open())
                            using (var fileStream = File.OpenRead(filePath))
                            {
                                await fileStream.CopyToAsync(entryStream);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log error but continue with other files
                        Console.WriteLine($"Lỗi khi thêm file vào zip: {imageUrl} - {ex.Message}");
                    }
                }
            }
            
            memoryStream.Position = 0;
            return memoryStream;
        }

        public void CleanupTempFile(string zipFilePath)
        {
            try
            {
                if (File.Exists(zipFilePath))
                {
                    File.Delete(zipFilePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi xóa file tạm: {ex.Message}");
            }
        }

        private string ConvertUrlToFilePath(string imageUrl)
        {
            // Convert URL like "/static/image/medical/filename.jpg" to physical path
            if (imageUrl.StartsWith("/static/"))
            {
                var relativePath = imageUrl.Substring(8); // Remove "/static/"
                return Path.Combine(_staticPath, relativePath.Replace('/', Path.DirectorySeparatorChar));
            }
            
            // If it's already a file path
            if (Path.IsPathRooted(imageUrl))
            {
                return imageUrl;
            }
            
            // Default case - assume it's in static folder
            return Path.Combine(_staticPath, "image", "medical", Path.GetFileName(imageUrl));
        }
    }
} 