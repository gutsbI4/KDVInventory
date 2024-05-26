namespace api.Models.DTO
{
    public class PackageDTO
    {
        public PackageDTO(Package package)
        {
            PackageId = package.PackageId;
            Name = package.Name;
        }
        public int PackageId { get; set; }

        public string Name { get; set; } = null!;
    }
}
