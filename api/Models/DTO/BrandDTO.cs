namespace api.Models.DTO
{
    public class BrandDTO
    {
        public BrandDTO(Brand brand)
        {
            BrandId = brand.BrandId;
            Name = brand.Name;
        }
        public int BrandId { get; set; }

        public string Name { get; set; } = null!;
    }
}
