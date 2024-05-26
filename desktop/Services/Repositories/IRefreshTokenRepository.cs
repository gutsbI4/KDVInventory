using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Services.Repositories
{
    public interface IRefreshTokenRepository
    {
        public Task<string?> GetRefreshToken();
        public Task AddRefreshToken(string token);
        public Task UpdateRefreshToken(string token);
        public Task DeleteRefreshToken();
    }
}
