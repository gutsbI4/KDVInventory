using desktop.Models;
using desktop.Tools;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Services.Repositories
{
    public interface IFilterRepository
    {
        [Post("/Filter/GetFilters")]
        public Task<List<FilterCategory>> GetFilters([Body] List<Product> products);
    }
}
