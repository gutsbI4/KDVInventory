using desktop.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Services.Repositories
{
    public interface IFillingRepository
    {
        [Get("/Filling/GetFillings")]
        public Task<IEnumerable<Filling>> GetFillings();
        [Post("/Filling/AddFilling")]
        public Task AddFilling([Authorize("Bearer")] string accessToken, [Query] string name);
    }
}
