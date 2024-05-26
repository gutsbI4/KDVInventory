using desktop.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Services.Repositories
{
    public interface ICategoryRepository
    {
        [Get("/Category/GetCategories")]
        public Task<IEnumerable<Category>> GetCategories();
    }
}
