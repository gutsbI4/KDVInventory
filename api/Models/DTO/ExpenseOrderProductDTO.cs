using System.Text.Json.Serialization;

namespace api.Models.DTO
{
    public class ExpenseOrderProductDTO
    {
        [JsonConstructor]
        public ExpenseOrderProductDTO()
        {

        }
        public ExpenseOrderProductDTO(ExpenseOrderProduct expenseOrder)
        {
            Product = new ProductDTO(expenseOrder.Product);
            ExpenseOrderId = expenseOrder.ExpenseOrderId;
            Quantity = expenseOrder.Quantity;
            Price = expenseOrder.Price;
        }
        public ProductDTO Product { get; set; }

        public int ExpenseOrderId { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }
}
