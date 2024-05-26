using api.Data;
using api.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PackageController : ControllerBase
    {
        private KDVDbContext _dbContext;
        public PackageController(KDVDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        [Route("GetPackages")]
        public async Task<ActionResult<IEnumerable<PackageDTO>>> GetPackages()
        {
            try
            {
                var packages = await _dbContext.Package.ToListAsync();
                return Ok(packages.ConvertAll(m => new PackageDTO(m)));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status501NotImplemented);
            }
        }
        [Authorize]
        [HttpPost]
        [Route("AddPackage")]
        public async Task<ActionResult> AddPackage([FromQuery] string name)
        {
            try
            {
                var type = await _dbContext.Package.FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower());
                if (type == null)
                {
                    await _dbContext.Package.AddAsync(new Models.Package { Name = name });
                    await _dbContext.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    return BadRequest("Такая упаковка уже есть в базе данных");
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status501NotImplemented, "");
            }
        }
    }
}
