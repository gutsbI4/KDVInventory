using api.Data;
using api.Models;
using api.Models.DTO;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly KDVDbContext _dbContext;
    public UserController(KDVDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    [Authorize]
    [HttpGet("GetInfo")]
    public async Task<ActionResult<UserDTO>> GetInfo()
    {
        string idUser = User.FindFirst(ClaimTypes.Name)?.Value;
        var user = await _dbContext.User.Include(p => p.Role)
            .Include(p=>p.Employee).FirstOrDefaultAsync(p => p.UserId == int.Parse(idUser));
        if (user == null) return NotFound();
        return Ok(new UserDTO(user));
    }
    [Authorize]
    [HttpGet]
    [Route("GetUsers")]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
    {
        int idUser = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
        try
        {
           var users = await _dbContext.User.Include(p => p.Employee)
                .Include(p => p.Audit)
                .Include(p => p.Role).ToListAsync();

            await AuditLogger.AddAuditRecord(_dbContext, idUser, "Получил список пользователей.");
            return Ok(users.ConvertAll(m => new UserDTO(m)));
        }
        catch (JsonException)
        {
            return BadRequest();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    [Authorize]
    [HttpGet]
    [Route("GetRoles")]
    public async Task<ActionResult<IEnumerable<RoleDTO>>> GetRoles()
    {
        try
        {
            var roles = await _dbContext.Role.ToListAsync();
            return Ok(roles.ConvertAll(m=> new RoleDTO(m)));
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,"Ошибка при получении ролей");
        }
    }
    [Authorize]
    [HttpGet]
    [Route("GetUserEdit/{id}")]
    public async Task<ActionResult<UserEditDTO>> GetUserEdit(int id)
    {
        try
        {
            var user = await _dbContext.User
                                .Include(p => p.Role)
                                .Include(p=>p.Employee)
                                .FirstOrDefaultAsync(b => b.UserId == id);
            if (user == null) return NotFound("Пользователь не найден");
            return Ok(new UserEditDTO(user));
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,"Ошибка при получении пользователя на сервере");
        }
    }
    [Authorize]
    [HttpPut]
    [Route("UpdateUser")]
    public async Task<ActionResult> UpdateUser([FromBody] UserEditDTO userEditDTO)
    {
        int idUser = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
        try
        {
            var user = await _dbContext.User
                                .Include(p => p.Role)
                                .FirstOrDefaultAsync(b => b.UserId == userEditDTO.UserId);
            if (user == null) return NotFound("Пользователь не найден");
            user.IsArchive = userEditDTO.IsArchive;
            user.IsActive = userEditDTO.IsActive;
            if(!user.IsActive || user.IsArchive)
            {
                user.RefreshToken = null;
                user.RefreshTokenExp = null;
            }
            user.Image = userEditDTO.Image;
            user.RoleId = userEditDTO.RoleId;
            user.Login = userEditDTO.Login == "" ? null : userEditDTO.Login;
            Employee? employee = await _dbContext.Employee.FirstOrDefaultAsync(p => p.EmployeeId == userEditDTO.UserId);
            if (employee == null && userEditDTO.Employee != null)
            {
                employee = new Employee();
                employee.EmployeeId = userEditDTO.UserId;
                employee.Surname = userEditDTO.Employee.Surname;
                employee.PhoneNumber = userEditDTO.Employee.PhoneNumber;
                employee.Name = userEditDTO.Employee.Name == "" ? null : userEditDTO.Employee.Name;
                employee.MiddleName = userEditDTO.Employee.MiddleName;
                await _dbContext.Employee.AddAsync(employee);
            }
            else if (employee != null && userEditDTO.Employee != null)
            {
                employee.EmployeeId = userEditDTO.UserId;
                employee.Surname = userEditDTO.Employee.Surname;
                employee.PhoneNumber = userEditDTO.Employee.PhoneNumber;
                employee.Name = userEditDTO.Employee.Name == "" ? null : userEditDTO.Employee.Name;
                employee.MiddleName = userEditDTO.Employee.MiddleName;
            }

            await _dbContext.SaveChangesAsync();
            await AuditLogger.AddAuditRecord(_dbContext, idUser, $"Обновил информацию о пользователе. ID:{user.UserId}");
            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status501NotImplemented, "Не удалось изменить пользователя. Проверьте все ли поля заполнены!");
        }
    }
    [Authorize]
    [HttpPost]
    [Route("AddUser")]
    public async Task<ActionResult> AddUser([FromBody] UserEditDTO userEditDTO)
    {
        int idUser = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
        try
        {
            var user = new User();
            user.IsArchive = userEditDTO.IsArchive;
            user.IsActive = userEditDTO.IsActive;
            user.Image = userEditDTO.Image;
            user.RoleId = userEditDTO.RoleId;
            user.Login = userEditDTO.Login == "" ? null : userEditDTO.Login;
            PasswordHasher<object> passwordHasher = new PasswordHasher<object>();
            user.Password = passwordHasher.HashPassword(null, userEditDTO.Password);
            if(userEditDTO.Employee != null)
            {
                var employee = new Employee();
                employee.Surname = userEditDTO.Employee.Surname;
                employee.PhoneNumber = userEditDTO.Employee.PhoneNumber;
                employee.Name = userEditDTO.Employee.Name;
                employee.MiddleName = userEditDTO.Employee.MiddleName;
                user.Employee = employee;
            }
            
            await _dbContext.User.AddAsync(user);

            await _dbContext.SaveChangesAsync();
            await AuditLogger.AddAuditRecord(_dbContext, idUser, $"Добавил нового пользователя.");
            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status501NotImplemented, "Не удалось добавить пользователя. Проверьте все ли поля заполнены!");
        }
    }
    [Authorize]
    [HttpPut]
    [Route("ChangePassword")]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
    {
        int idUser = int.Parse(User.FindFirst(ClaimTypes.Name)?.Value);
        try
        {
            var user = await _dbContext.User.FirstOrDefaultAsync(x => x.UserId == changePasswordDTO.UserId);
            if (user == null) return NotFound("Пользователь не найден!");
            PasswordHasher<object> passwordHasher = new PasswordHasher<object>();
            var result = passwordHasher.VerifyHashedPassword(null, user.Password, changePasswordDTO.OldPassword);
            if (result == PasswordVerificationResult.Failed) return BadRequest("Введен неверный старый пароль!");
            else
            {
                user.Password = passwordHasher.HashPassword(null, changePasswordDTO.NewPassword);
            }
            await _dbContext.SaveChangesAsync();
            await AuditLogger.AddAuditRecord(_dbContext, idUser,$"Сменил пароль пользователю. ID: {user.UserId}");
            return Ok();

        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status501NotImplemented, "Произошла ошибка на стороне сервера!");
        }
    }
}
