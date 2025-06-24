using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Hospital_BE.BLL.Interfaces
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(IFormFile file, string folder = "general");
        Task<bool> DeleteImageAsync(string imagePath);
        bool IsValidImageFile(IFormFile file);
    }
} 