using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Services.Repositories
{
    public class AccessTokenRepository : IAccessTokenRepository
    {
        private string _token;

        public void AddAccessToken(string token)
        {
            _token = token;
        }

        public void DeleteAccessToken()
        {
            _token = null;
        }

        public string? GetAccessToken()
        {
            return _token;
        }

        public void UpdateAccessToken(string token)
        {
            _token = token;
        }
    }
}
