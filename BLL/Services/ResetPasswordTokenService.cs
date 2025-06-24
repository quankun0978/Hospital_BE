using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Hospital_BE.BLL.Services
{
    public static class ResetPasswordTokenService
    {
        // In-memory storage cho reset password tokens (trong production nên dùng Redis)
        private static readonly ConcurrentDictionary<string, ResetPasswordTokenInfo> _tokenStore = new();

        public static void StoreToken(string token, string email, DateTime expirationTime)
        {
            var tokenInfo = new ResetPasswordTokenInfo
            {
                Email = email,
                Token = token,
                ExpirationTime = expirationTime
            };
            _tokenStore.AddOrUpdate(token, tokenInfo, (key, oldValue) => tokenInfo);
        }

        public static async Task<string> ValidateAndGetEmailAsync(string token)
        {
            return await Task.Run(() =>
            {
                if (!_tokenStore.TryGetValue(token, out var tokenInfo))
                {
                    return null;
                }

                if (DateTime.Now > tokenInfo.ExpirationTime)
                {
                    _tokenStore.TryRemove(token, out _);
                    return null;
                }

                return tokenInfo.Email;
            });
        }

        public static void RemoveToken(string token)
        {
            _tokenStore.TryRemove(token, out _);
        }

        private class ResetPasswordTokenInfo
        {
            public string Email { get; set; }
            public string Token { get; set; }
            public DateTime ExpirationTime { get; set; }
        }
    }
} 