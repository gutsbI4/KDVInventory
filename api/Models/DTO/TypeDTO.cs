namespace api.Models.DTO
{
    public class TypeDTO
    {
        public TypeDTO(Type type)
        {
            TypeId = type.TypeId;
            Name = type.Name;
        }
        public int TypeId { get; set; }

        public string Name { get; set; } = null!;
    }
}
