using desktop.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Services.Repositories
{
    public interface IOrderRepository
    {
        [Get("/Order/GetOrders")]
        public Task<OrderCollection> GetOrders([Authorize("Bearer")] string accessToken, [Query] OwnersParameters ownersParameters);
        [Post("/Order/SaveOrder")]
        public Task<int> SaveOrder([Authorize("Bearer")] string accessToken, [Body] OrderEdit orderEdit);
        [Get("/Order/GetOrderEdit/{id}")]
        public Task<OrderEdit> GetOrderEdit([Authorize("Bearer")] string accessToken, [AliasAs("id")] int id);
    }
}
