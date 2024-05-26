namespace api.Models.DTO
{
    public class PriceUnitDTO
    {
        public PriceUnitDTO(PriceUnit priceUnit)
        {
            Id = priceUnit.Id;
            Unit = priceUnit.Unit;
        }
        public int Id { get; set; }

        public string Unit { get; set; } = null!;
    }
}
