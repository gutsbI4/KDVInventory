using System.Text.Json.Serialization;

namespace api.Models.DTO
{
    public class OrderProductDTO
    {
        [JsonConstructor]
        public OrderProductDTO()
        {
            
        }
        public OrderProductDTO(OrderProduct orderProduct)
        {
            Product = new ProductDTO(orderProduct.Product);
            OrderId = orderProduct.OrderId;
            Quantity = orderProduct.Quantity;
            Price = orderProduct.Price;
        }
        public int OrderId { get; set; }

        public ProductDTO Product { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }
}
