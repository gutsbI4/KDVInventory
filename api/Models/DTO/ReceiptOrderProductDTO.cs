using System.Text.Json.Serialization;

namespace api.Models.DTO
{
    public class ReceiptOrderProductDTO
    {
        [JsonConstructor]
        public ReceiptOrderProductDTO()
        {
            
        }
        public ReceiptOrderProductDTO(ReceiptOrderProduct receiptOrder)
        {
            Product = new ProductDTO(receiptOrder.Product);
            ReceiptOrderId = receiptOrder.ReceiptOrderId;
            Quantity = receiptOrder.Quantity;
            PurchasePrice = receiptOrder.PurchasePrice;
        }
        public ProductDTO Product { get; set; }

        public int ReceiptOrderId { get; set; }

        public int Quantity { get; set; }

        public decimal PurchasePrice { get; set; }

    }
}
