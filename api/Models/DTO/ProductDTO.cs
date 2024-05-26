using api.Models;
using System.Text.Json.Serialization;

public class ProductDTO
{
    [JsonConstructor]
    public ProductDTO()
    {

    }
    public ProductDTO(Product product)
    {
        ProductId = product.ProductId;
        Name = product.Name;
        PriceOfSale = product.PriceOfSale;
        MinPriceOfSale = product.MinPriceOfSale;
        PurchasePrice = product.PurchasePrice;
        var receipts = product.ReceiptOrderProduct.Where(x => x.ProductId == product.ProductId && x.ReceiptOrder.IsReceipt).ToList();
        var expenses = product.ExpenseOrderProduct.Where(x => x.ProductId == product.ProductId && x.ExpenseOrder.IsExpense).ToList();
        var orders = product.OrderProduct.Where(x=>x.ProductId == product.ProductId && x.Order.IsShipment).ToList();
        Count = receipts.Sum(x=>x.Quantity) - expenses.Sum(x=>x.Quantity) - orders.Sum(x=>x.Quantity);
        Barcode = product.Barcode;
        ProductNumber = product.ProductNumber;
        Image = product.ProductImage.FirstOrDefault()?.Path;
    }
    public int ProductId { get; set; }
    public string? ProductNumber { get; set; }
    public long? Barcode { get; set; }
    public int? Count { get; set; }
    public string Name { get; set; } = null!;

    public decimal PriceOfSale { get; set; }

    public decimal? MinPriceOfSale { get; set; }

    public decimal? PurchasePrice { get; set; }

    public string? Image { get; set; }
}

