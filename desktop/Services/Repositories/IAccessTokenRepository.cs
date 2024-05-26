using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Services.Repositories
{
    public interface IAccessTokenRepository
    {
        public string? GetAccessToken();
        public void AddAccessToken(string token);
        public void UpdateAccessToken(string token);
        public void DeleteAccessToken();
    }
}
