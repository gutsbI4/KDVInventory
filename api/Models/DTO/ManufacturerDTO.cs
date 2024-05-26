using api.Models;

public class ManufacturerDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ManufacturerDTO(Manufacturer manufacturer)
    {
        Id = manufacturer.ManufacturerId;
        Name = manufacturer.Name;
    }
}
