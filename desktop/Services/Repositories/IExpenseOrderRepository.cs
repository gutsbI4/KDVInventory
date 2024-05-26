using desktop.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace desktop.Services.Repositories
{
    public interface IExpenseOrderRepository
    {
        [Get("/ExpenseOrder/GetExpenseOrders")]
        public Task<ExpenseOrderCollection> GetExpenseOrders([Authorize("Bearer")] string accessToken, [Query] OwnersParameters ownersParameters);
        [Post("/ExpenseOrder/SaveExpenseOrder")]
        public Task<int> SaveExpenseOrder([Authorize("Bearer")] string accessToken, [Body] ExpenseOrderEdit expenseOrderEdit);
        [Get("/ExpenseOrder/GetExpenseOrderEdit/{id}")]
        public Task<ExpenseOrderEdit> GetExpenseOrderEdit([Authorize("Bearer")] string accessToken, [AliasAs("id")] int id);
    }
}
