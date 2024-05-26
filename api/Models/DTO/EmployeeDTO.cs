using System.Text.Json.Serialization;

namespace api.Models.DTO
{
    public class EmployeeDTO
    {
        [JsonConstructor]
        public EmployeeDTO()
        {
            
        }
        public EmployeeDTO(Employee employee)
        {
            EmployeeId = employee.EmployeeId;
            Surname = employee.Surname;
            Name = employee.Name;
            MiddleName = employee.MiddleName;
            PhoneNumber = employee.PhoneNumber;
        }
        public int EmployeeId { get; set; }

        public string? Surname { get; set; }

        public string Name { get; set; } = null!;

        public string? MiddleName { get; set; }

        public string? PhoneNumber { get; set; }
    }
}
