using desktop.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Services.Repositories
{
    public interface IAuthorizationRepository
    {
        [Post("/Authorization/Login")]
        public Task<Token> Login([Body] Authorization authorization);
        [Post("/Authorization/UpdateToken")]
        public Task<Token> UpdateToken([Header("RefreshToken")] string refreshToken);
        [Post("/Authorization/Logout")]
        public Task Logout([Authorize("Bearer")] string accessToken);
    }
}
