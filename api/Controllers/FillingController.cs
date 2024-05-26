using api.Data;
using api.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FillingController : ControllerBase
    {
        private KDVDbContext _dbContext;
        public FillingController(KDVDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        [Route("GetFillings")]
        public async Task<ActionResult<IEnumerable<FillingDTO>>> GetFillings()
        {
            try
            {
                var fillings = await _dbContext.Filling.ToListAsync();
                return Ok(fillings.ConvertAll(m => new FillingDTO(m)));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status501NotImplemented);
            }
        }
        [Authorize]
        [HttpPost]
        [Route("AddFilling")]
        public async Task<ActionResult> AddFilling([FromQuery] string name)
        {
            try
            {
                var type = await _dbContext.Filling.FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower());
                if (type == null)
                {
                    await _dbContext.Filling.AddAsync(new Models.Filling { Name = name });
                    await _dbContext.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    return BadRequest("Такая начинка уже есть в базе данных");
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status501NotImplemented, "");
            }
        }
    }
}
