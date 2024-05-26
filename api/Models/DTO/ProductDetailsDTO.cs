using System.Text.Json.Serialization;

namespace api.Models.DTO
{
    public class ProductDetailsDTO
    {
        [JsonConstructor]
        public ProductDetailsDTO()
        {
            
        }
        public ProductDetailsDTO(ProductDetails product)
        {
            ProductId = product.ProductId;
            BrandId = product.BrandId;
            ManufacturerId = product.ManufacturerId;
            CountryOfProduction = product.CountryOfProduction;
            ShelfLife = product.ShelfLife;
            Weight = product.Weight;
            PackageId = product.PackageId;
            TypeId = product.TypeId;
            TasteId = product.TasteId;
            FillingId = product.FillingId;
            DietId = product.DietId;
        }
        public int ProductId { get; set; }

        public int? BrandId { get; set; }

        public int? ManufacturerId { get; set; }

        public string? CountryOfProduction { get; set; }

        public int? ShelfLife { get; set; }

        public double? Weight { get; set; }

        public int? PackageId { get; set; }

        public int? TypeId { get; set; }

        public int? TasteId { get; set; }

        public int? FillingId { get; set; }

        public int? DietId { get; set; }
    }
}
