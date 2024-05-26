using System.Text.Json.Serialization;

namespace api.Models.DTO
{
    public class UserEditDTO
    {
        [JsonConstructor]
        public UserEditDTO()
        {
            
        }
        public UserEditDTO(User user)
        {
            UserId = user.UserId;
            Login = user.Login;
            Password = user.Password;
            RoleId = user.RoleId;
            Image = user.Image;
            IsArchive = user.IsArchive;
            IsActive = user.IsActive;
            if(user.Employee != null) Employee = new EmployeeDTO(user.Employee);
        }
        public int UserId { get; set; }

        public string Login { get; set; } = null!;

        public string Password { get; set; } = null!;

        public int RoleId { get; set; }

        public string? Image { get; set; }

        public bool IsArchive { get; set; }

        public bool IsActive { get; set; }
        public EmployeeDTO? Employee { get; set; }
    }
}
