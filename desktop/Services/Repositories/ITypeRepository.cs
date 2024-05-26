using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Services.Repositories
{
    public interface ITypeRepository
    {
        [Get("/Type/GetTypes")]
        public Task<IEnumerable<Models.Type>> GetTypes();
        [Post("/Type/AddType")]
        public Task AddType([Authorize("Bearer")] string accessToken,[Query]string name);
    }
}
