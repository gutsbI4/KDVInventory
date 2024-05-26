using api.Data;
using api.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DietController : ControllerBase
    {
        private KDVDbContext _dbContext;
        public DietController(KDVDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        [Route("GetDiets")]
        public async Task<ActionResult<IEnumerable<DietDTO>>> GetDiets()
        {
            try
            {
                var diets = await _dbContext.Diet.ToListAsync();
                return Ok(diets.ConvertAll(m => new DietDTO(m)));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status501NotImplemented);
            }
        }
        [Authorize]
        [HttpPost]
        [Route("AddDiet")]
        public async Task<ActionResult> AddDiet([FromQuery] string name)
        {
            try
            {
                var type = await _dbContext.Diet.FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower());
                if (type == null)
                {
                    await _dbContext.Diet.AddAsync(new Models.Diet { Name = name });
                    await _dbContext.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    return BadRequest("Такая диета уже есть в базе данных");
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status501NotImplemented, "");
            }
        }
    }
}
