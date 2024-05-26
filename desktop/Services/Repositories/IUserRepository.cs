using desktop.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Services.Repositories
{
    public interface IUserRepository
    {
        [Get("/User/GetInfo")]
        public Task<User> GetUserInfo([Authorize] string accessToken);
        [Get("/User/GetUsers")]
        public Task<IEnumerable<User>> GetUsers([Authorize("Bearer")] string accessToken);
        [Get("/User/GetRoles")]
        public Task<IEnumerable<Role>> GetRoles([Authorize("Bearer")] string accessToken);
        [Put("/User/UpdateUser")]
        public Task UpdateUser([Authorize("Bearer")] string accessToken, [Body] UserEdit userEdit);
        [Post("/User/AddUser")]
        public Task AddUser([Authorize("Bearer")] string accessToken, [Body] UserEdit userEdit);
        [Put("/User/ChangePassword")]
        public Task<ApiResponse<string>> ChangePassword([Authorize("Bearer")] string accessToken, [Body] ChangePassword changePassword);
        [Get("/User/GetUserEdit/{id}")]
        public Task<UserEdit> GetUserEdit([Authorize("Bearer")] string accessToken, [AliasAs("id")] int id);
    }
}
