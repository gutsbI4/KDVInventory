using api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ManufacturerController : ControllerBase
    {
        private KDVDbContext _dbContext;
        public ManufacturerController(KDVDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        [Route("GetManufacturers")]
        public async Task<ActionResult<IEnumerable<ManufacturerDTO>>> GetManufacturers()
        {
            try
            {
                var manufacturers = await _dbContext.Manufacturer.ToListAsync();
                return Ok(manufacturers.ConvertAll(m => new ManufacturerDTO(m)));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status501NotImplemented);
            }
        }
        [Authorize]
        [HttpPost]
        [Route("AddManufacturer")]
        public async Task<ActionResult> AddManufacturer([FromQuery] string name)
        {
            try
            {
                var type = await _dbContext.Manufacturer.FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower());
                if (type == null)
                {
                    await _dbContext.Manufacturer.AddAsync(new Models.Manufacturer { Name = name });
                    await _dbContext.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    return BadRequest("Такой производитель уже есть в базе данных");
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status501NotImplemented, "");
            }
        }
    }
}
