using desktop.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Services.Repositories
{
    public interface IBrandRepository
    {
        [Get("/Brand/GetBrands")]
        public Task<IEnumerable<Brand>> GetBrands();
        [Post("/Brand/AddBrand")]
        public Task AddBrand([Authorize("Bearer")] string accessToken, [Query] string name);
    }
}
