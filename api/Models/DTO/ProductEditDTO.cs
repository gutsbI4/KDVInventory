using api.Models;
using api.Models.DTO;
using System.Globalization;
using System.Text.Json.Serialization;

public class ProductEditDTO
{
    [JsonConstructor]
    public ProductEditDTO()
    {

    }
    public ProductEditDTO(Product product)
    {
        ProductId = product.ProductId;
        ProductNumber = product.ProductNumber;
        Barcode = product.Barcode;
        Name = product.Name;
        Description = product.Description;
        PriceOfSale = product.PriceOfSale;
        MinPriceOfSale = product.MinPriceOfSale;
        PurchasePrice = product.PurchasePrice;
        LowStockThreshold = product.LowStockThreshold;
        CategoryId = product.CategoryId;
        PriceUnitId = product.PriceUnitId;
        ProductImage = product.ProductImage.ToList().ConvertAll(p=>new ProductImageDTO(p));
        if(product.ProductDetails != null) Details = new ProductDetailsDTO(product.ProductDetails);
        if(product.EnergyValue != null) Energy = new EnergyValueDTO(product.EnergyValue);
    }
    public int ProductId { get; set; }

    public string? ProductNumber { get; set; }

    public long? Barcode { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal PriceOfSale { get; set; }

    public decimal? MinPriceOfSale { get; set; }

    public decimal? PurchasePrice { get; set; }

    public int? LowStockThreshold { get; set; }

    public int? CategoryId { get; set; }

    public int? PriceUnitId { get; set; }
    public IEnumerable<ProductImageDTO>? ProductImage { get; set; }
    public ProductDetailsDTO? Details { get; set; }
    public EnergyValueDTO? Energy { get; set; }
}
