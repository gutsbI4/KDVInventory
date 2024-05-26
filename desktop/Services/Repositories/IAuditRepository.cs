using desktop.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Services.Repositories
{
    public interface IAuditRepository
    {
        [Get("/Audit/GetAuditByUserId/{id}")]
        public Task<ObservableCollection<Audit>> GetAuditByUserId([Authorize("Bearer")] string accessToken, [AliasAs("id")] int id);
        [Post("/Audit/SaveAuditToFile/{id}")]
        public Task SaveAuditToFile([Authorize("Bearer")] string accessToken, [AliasAs("id")] int id, [Body] AuditTime time);
    }
}
