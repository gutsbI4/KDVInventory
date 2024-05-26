using api.Data;
using api.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BrandController : ControllerBase
    {
        private KDVDbContext _dbContext;
        public BrandController(KDVDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        [Route("GetBrands")]
        public async Task<ActionResult<IEnumerable<BrandDTO>>> GetBrands()
        {
            try
            {
                var brands = await _dbContext.Brand.ToListAsync();
                return Ok(brands.ConvertAll(m => new BrandDTO(m)));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status501NotImplemented);
            }
        }
        [Authorize]
        [HttpPost]
        [Route("AddBrand")]
        public async Task<ActionResult> AddBrand([FromQuery] string name)
        {
            try
            {
                var type = await _dbContext.Brand.FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower());
                if (type == null)
                {
                    await _dbContext.Brand.AddAsync(new Models.Brand { Name = name });
                    await _dbContext.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    return BadRequest("Такая торг. марка уже есть в базе данных");
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status501NotImplemented, "");
            }
        }
    }
}
