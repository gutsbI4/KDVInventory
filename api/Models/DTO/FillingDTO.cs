namespace api.Models.DTO
{
    public class FillingDTO
    {
        public FillingDTO(Filling filling)
        {
            FillingId = filling.FillingId;
            Name = filling.Name;
        }
        public int FillingId { get; set; }

        public string Name { get; set; } = null!;
    }
}
