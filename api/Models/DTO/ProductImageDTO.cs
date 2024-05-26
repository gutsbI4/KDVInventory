using System.Text.Json.Serialization;

namespace api.Models.DTO
{
    public class ProductImageDTO
    {
        [JsonConstructor]
        public ProductImageDTO()
        {
                
        }
        public ProductImageDTO(ProductImage productImage)
        {
            ImageId = productImage.ImageId;
            ProductId = productImage.ProductId;
            Path = productImage.Path;
        }
        public int ImageId { get; set; }

        public int ProductId { get; set; }

        public string Path { get; set; } = null!;
    }
}
