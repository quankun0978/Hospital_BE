using System;
using System.IO;
using System.Threading.Tasks;
using Hospital_BE.BLL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Hospital_BE.BLL.Services
{
    public class ImageService : IImageService
    {
        private readonly IConfiguration _configuration;
        private readonly string _uploadPath;

        public ImageService(IConfiguration configuration)
        {
            _configuration = configuration;
            _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "PL", "static", "image");
            
            // Ensure upload directory exists
            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folder = "general")
        {
            try
            {
                if (file == null || file.Length == 0)
                    return null;

                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                
                if (!Array.Exists(allowedExtensions, ext => ext == fileExtension))
                    throw new ArgumentException("Định dạng file không được hỗ trợ");

                // Validate file size (5MB max)
                if (file.Length > 5 * 1024 * 1024)
                    throw new ArgumentException("Kích thước file không được vượt quá 5MB");

                // Create folder path
                var folderPath = Path.Combine(_uploadPath, folder);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Generate unique filename
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(folderPath, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return relative URL that matches static files configuration
                return $"/static/image/{folder}/{fileName}";
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tải ảnh lên: {ex.Message}");
            }
        }

        public async Task<bool> DeleteImageAsync(string imagePath)
        {
            try
            {
                if (string.IsNullOrEmpty(imagePath))
                    return true;

                // Convert URL to physical path
                var relativePath = imagePath.Replace("/static/image/", "").Replace("/", Path.DirectorySeparatorChar.ToString());
                var fullPath = Path.Combine(_uploadPath, relativePath);

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool IsValidImageFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            return Array.Exists(allowedExtensions, ext => ext == fileExtension);
        }
    }
} 