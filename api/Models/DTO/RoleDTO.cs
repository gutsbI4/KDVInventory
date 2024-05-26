namespace api.Models.DTO
{
    public class RoleDTO
    {
        public RoleDTO(Role role)
        {
            RoleId = role.RoleId;
            Name = role.Name;
        }
        public int RoleId { get; set; }

        public string Name { get; set; } = null!;
    }
}
