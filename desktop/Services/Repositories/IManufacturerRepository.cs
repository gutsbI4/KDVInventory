using desktop.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Services.Repositories
{
    public interface IManufacturerRepository
    {
        [Get("/Manufacturer/GetManufacturers")]
        public Task<IEnumerable<Manufacturer>> GetManufacturers();
        [Post("/Manufacturer/AddManufacturer")]
        public Task AddManufacturer([Authorize("Bearer")] string accessToken, [Query] string name);
    }
}
