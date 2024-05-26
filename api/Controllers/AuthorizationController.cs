using api.Data;
using api.Models;
using api.Services;
using api.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("[controller]")]
public class AuthorizationController : ControllerBase
{
    private const int RefreshTokenExpiryTimeDay = 60;
    private KDVDbContext _dbContext;
    public AuthorizationController(KDVDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    [HttpPost]
    [Route("Login")]
    public async Task<ActionResult<TokenDTO>> Login([FromBody] AuthorizationDTO authorizationDTO)
    {
        if (string.IsNullOrEmpty(authorizationDTO.Login) || string.IsNullOrEmpty(authorizationDTO.Password))
            return BadRequest();
        User? user;
        try
        {
            user = await _dbContext.User.Include(p => p.Role).FirstOrDefaultAsync(a =>
            a.Login == authorizationDTO.Login);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        if (user == null)
        {
            return Unauthorized();
        }
        else if(user.IsArchive || !user.IsActive)
        {
            return BadRequest("Вам отключили доступ к системе!");
        }

        PasswordHasher<object> passwordHasher = new PasswordHasher<object>();
        var result = passwordHasher.VerifyHashedPassword(null, user.Password, authorizationDTO.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            return Unauthorized();
        }
        
        var newDateExpiry = DateTime.Now.ToLocalTime();
        newDateExpiry = newDateExpiry.AddDays(RefreshTokenExpiryTimeDay);
        string refreshToken = GenerateRefreshToken(user, newDateExpiry);
        user.RefreshToken = refreshToken;
        user.RefreshTokenExp = newDateExpiry;
        try
        {
            await _dbContext.SaveChangesAsync();
            await AuditLogger.AddAuditRecord(_dbContext, user.UserId, "Авторизация в системе.");
        }

        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        return Ok(new TokenDTO() { AccessToken = GenerateAccessToken(user), RefreshToken = refreshToken });

    }
    [HttpPost]
    [Route("UpdateToken")]
    public async Task<ActionResult<TokenDTO>> UpdateToken([FromHeader(Name = "RefreshToken")] string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            ModelState.AddModelError("ErrorMessage", "Неверный формат данных");
            return BadRequest(ModelState);
        }
        string? idUser = GetIdFromJWTToken(refreshToken);
        if (string.IsNullOrEmpty(idUser))
        {
            ModelState.AddModelError("ErrorMessage", "Неверный RefreshToken");
            return BadRequest(ModelState);
        }
        User? user;
        try
        {
            user = await _dbContext.User.Include(p => p.Role).FirstOrDefaultAsync(a => a.UserId == int.Parse(idUser) && a.RefreshToken.Equals(refreshToken));
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        if (user == null)
        {
            ModelState.AddModelError("ErrorMessage", "Неверный RefreshToken");
            return BadRequest(ModelState);
        }

        if (user.RefreshTokenExp < DateTime.Now.ToLocalTime())
        {
            ModelState.AddModelError("ErrorMessage", "RefreshToken просрочен");
            return BadRequest(ModelState);
        }
        var newDateExpiry = DateTime.Now.ToLocalTime();
        newDateExpiry = newDateExpiry.AddDays(RefreshTokenExpiryTimeDay);
        string newRefreshToken = GenerateRefreshToken(user, newDateExpiry);
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExp = newDateExpiry;
        try
        {
            await _dbContext.SaveChangesAsync();
        }

        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        return Ok(new TokenDTO() { AccessToken = GenerateAccessToken(user), RefreshToken = newRefreshToken });

    }
    [HttpGet]
    [Route("HashPassword")]
    public async Task<ActionResult<string>> GetHashPassword(string password)
    {
        PasswordHasher<object> passwordHasher = new PasswordHasher<object>();
        return Ok(passwordHasher.HashPassword(null, password));
    }
    private string GenerateToken(IEnumerable<Claim> claims, DateTime exp)
    {
        var claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        var dateNow = DateTime.Now;
        var jwtToken = new JwtSecurityToken(
          issuer: JwtSettings.KDVInventoryServer,
          audience: JwtSettings.Audience,
          notBefore: dateNow,
          claims: claimsIdentity.Claims,
          expires: exp,
          signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSettings.Key))
          , SecurityAlgorithms.HmacSha256)
        );
        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }
    [Authorize]
    [HttpPost]
    [Route("Logout")]
    public async Task<ActionResult> Logout()
    {
        string idUser = User.FindFirst(ClaimTypes.Name)?.Value;
        var user = await _dbContext.User.Include(p => p.Role).FirstOrDefaultAsync(p => p.UserId == int.Parse(idUser));
        if (user == null) return NotFound();
        user.RefreshToken = null;
        user.RefreshTokenExp = null;
        try
        {
            _dbContext.SaveChanges();
            await AuditLogger.AddAuditRecord(_dbContext, user.UserId, "Вышел из системы.");
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        return Ok();
    }
    private string GenerateRefreshToken(User user, DateTime exp)
    {
        var claims = new List<Claim>
        {
            new Claim("id",user.UserId.ToString()),
        };
        var dateNow = DateTime.Now;
        return GenerateToken(claims, exp.ToUniversalTime());
    }
    private string GenerateAccessToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType,user.UserId.ToString()),
            new Claim(ClaimsIdentity.DefaultRoleClaimType,user.Role.Name)
        };
        return GenerateToken(claims, DateTime.Now.AddMinutes(JwtSettings.LifeTime));
    }
    private string? GetIdFromJWTToken(string jwtToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var validations = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSettings.Key)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = JwtSettings.KDVInventoryServer,
            ValidAudience = JwtSettings.Audience,
            ValidateLifetime = true,
        };
        try
        {
            var claims = handler.ValidateToken(jwtToken, validations, out var securityToken);
            return claims.FindFirstValue("id");
        }
        catch (Exception)
        {
            return null;
        }
    }
}
