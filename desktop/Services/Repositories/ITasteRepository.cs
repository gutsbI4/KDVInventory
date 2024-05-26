using desktop.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Services.Repositories
{
    public interface ITasteRepository
    {
        [Get("/Taste/GetTastes")]
        public Task<IEnumerable<Taste>> GetTastes();
        [Post("/Taste/AddTaste")]
        public Task AddTaste([Authorize("Bearer")] string accessToken, [Query] string name);
    }
}
