using System.Threading.Tasks;

namespace Hospital_BE.DAL.Interfaces
{
    public interface IOTPRepository
    {
        Task<bool> SaveOTPAsync(string phone, string otpCode);
        Task<bool> VerifyOTPAsync(string phone, string otpCode);
        Task<bool> SavePasswordTempAsync(string phone, string password);
        Task<string> GetTempPasswordAsync(string phone);
        Task<bool> RemoveTempDataAsync(string phone);
    }
} 