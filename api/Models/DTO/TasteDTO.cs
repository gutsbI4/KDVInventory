namespace api.Models.DTO
{
    public class TasteDTO
    {
        public TasteDTO(Taste taste)
        {
            TasteId = taste.TasteId;
            Name = taste.Name;
        }
        public int TasteId { get; set; }

        public string Name { get; set; } = null!;
    }
}
