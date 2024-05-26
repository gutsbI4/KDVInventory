using desktop.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Services.Repositories
{
    public interface IPriceUnitRepository
    {
        [Get("/PriceUnit/GetPriceUnits")]
        public Task<IEnumerable<PriceUnit>> GetPriceUnits();
        [Post("/PriceUnit/AddPriceUnit")]
        public Task AddPriceUnit([Authorize("Bearer")] string accessToken, [Query] string name);
    }
}
