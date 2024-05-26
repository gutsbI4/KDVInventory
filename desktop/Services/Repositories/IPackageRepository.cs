using desktop.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Services.Repositories
{
    public interface IPackageRepository
    {
        [Get("/Package/GetPackages")]
        public Task<IEnumerable<Package>> GetPackages();
        [Post("/Package/AddPackage")]
        public Task AddPackage([Authorize("Bearer")] string accessToken, [Query] string name);
    }
}
