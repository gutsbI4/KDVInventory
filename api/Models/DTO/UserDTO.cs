using api.Models;

public class UserDTO
{
    public int IdUser { get; set; }
    public string? Image { get; set; }
    public string Role { get; set; }
    public string UserName { get; set; }
    public string IsActive { get; set; }
    public bool IsArchive { get; set; }
    public DateTime? FirstEntry { get; set; }
    public DateTime? LastAction { get; set; }
    public UserDTO(User user)
    {
        IdUser = user.UserId;
        Image = user.Image;
        Role = user.Role.Name;
        string? surname = user.Employee?.Surname + " ";
        string name = user.Employee?.Name + " ";
        string? middleName = user.Employee?.MiddleName + " ";
        if (!string.IsNullOrWhiteSpace(surname))
        {
            name = name[0] + ".";
            if(!string.IsNullOrWhiteSpace(middleName)) middleName = middleName[0] + ". ";
        }
        UserName = surname + name + middleName;
        FirstEntry = user.Audit.FirstOrDefault(x=>x.DateOfAction.Date == DateTime.Now.Date)?.DateOfAction;
        LastAction = user.Audit.LastOrDefault()?.DateOfAction;
        IsActive = user.IsActive ? "Активен" : "Не активен";
        IsArchive = user.IsArchive;
    }

}
