using api.Data;
using api.Models;
using api.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PriceUnitController : ControllerBase
    {
        private KDVDbContext _dbContext;
        public PriceUnitController(KDVDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        [Route("GetPriceUnits")]
        public async Task<ActionResult<IEnumerable<PriceUnitDTO>>> GetPriceUnits()
        {
            try
            {
                var priceUnits = await _dbContext.PriceUnit.ToListAsync();
                return Ok(priceUnits.ConvertAll(m => new PriceUnitDTO(m)));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status501NotImplemented);
            }
        }
        [Authorize]
        [HttpPost]
        [Route("AddPriceUnit")]
        public async Task<ActionResult> AddPriceUnit([FromQuery] string name)
        {
            try
            {
                var type = await _dbContext.PriceUnit.FirstOrDefaultAsync(p => p.Unit.ToLower() == name.ToLower());
                if (type == null)
                {
                    await _dbContext.PriceUnit.AddAsync(new PriceUnit { Unit = name });
                    await _dbContext.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    return BadRequest("Такая ед. измерения уже есть в базе данных");
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status501NotImplemented, "");
            }
        }
    }
}
