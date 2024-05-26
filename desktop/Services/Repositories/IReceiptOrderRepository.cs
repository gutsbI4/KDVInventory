using desktop.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Services.Repositories
{
    public interface IReceiptOrderRepository
    {
        [Get("/ReceiptOrder/GetReceiptOrders")]
        public Task<ReceiptOrderCollection> GetReceiptOrders([Authorize("Bearer")] string accessToken, [Query] OwnersParameters ownersParameters);
        [Post("/ReceiptOrder/SaveReceiptOrder")]
        public Task<int> SaveReceiptOrder([Authorize("Bearer")] string accessToken, [Body] ReceiptOrderEdit receiptOrderEdit);
        [Get("/ReceiptOrder/GetReceiptOrderEdit/{id}")]
        public Task<ReceiptOrderEdit> GetReceiptOrderEdit([Authorize("Bearer")] string accessToken, [AliasAs("id")] int id);
    }
}
