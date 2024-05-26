using desktop.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Services.Repositories
{
    public interface IDietRepository
    {
        [Get("/Diet/GetDiets")]
        public Task<IEnumerable<Diet>> GetDiets();
        [Post("/Diet/AddDiet")]
        public Task AddDiet([Authorize("Bearer")] string accessToken, [Query] string name);
    }
}
