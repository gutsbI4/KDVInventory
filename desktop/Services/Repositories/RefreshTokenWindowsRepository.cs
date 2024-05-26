using desktop.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Services.Repositories
{
    public class RefreshTokenWindowsRepository : IRefreshTokenRepository
    {
        private readonly byte[] entropy = { 1, 5, 76, 3, 6, 23, 65, 32, 22, 44, 41 };
        private readonly SqLiteContext _context;
        public RefreshTokenWindowsRepository(SqLiteContext sqLiteContext)
        {
            _context = sqLiteContext;
        }
        public async Task AddRefreshToken(string token)
        {
            string protectToken = await Task.Run(() => ProtectToken(token));
            _context.RefreshTokens.Add(new() { Token = protectToken });
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRefreshToken()
        {
            _context.RefreshTokens.Remove(_context.RefreshTokens.FirstOrDefault());
            await _context.SaveChangesAsync();
        }

        public async Task<string?> GetRefreshToken()
        {
            var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync();
            if (refreshToken == null) return null;
            string token = await Task.Run(() => UnprotectToken(refreshToken.Token));
            return token;
        }

        public async Task UpdateRefreshToken(string token)
        {
            DeleteRefreshToken();
            AddRefreshToken(token);
        }
        private string ProtectToken(string token)
        {
            byte[] tokenBytes = Encoding.Unicode.GetBytes(token);
            byte[] encryptBytes = ProtectedData.Protect(tokenBytes, entropy, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encryptBytes);
        }
        private string UnprotectToken(string token)
        {
            byte[] encryptedBytes = Convert.FromBase64String(token);
            byte[] tokenBytes = ProtectedData.Unprotect(encryptedBytes, entropy, DataProtectionScope.CurrentUser);
            return Encoding.Unicode.GetString(tokenBytes);
        }
    }
}
