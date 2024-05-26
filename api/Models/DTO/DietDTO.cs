namespace api.Models.DTO
{
    public class DietDTO
    {
        public DietDTO(Diet diet)
        {
            DietId = diet.DietId;
            Name = diet.Name;
        }
        public int DietId { get; set; }

        public string Name { get; set; } = null!;
    }
}
