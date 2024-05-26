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
    public class TasteController : ControllerBase
    {
        private KDVDbContext _dbContext;
        public TasteController(KDVDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        [Route("GetTastes")]
        public async Task<ActionResult<IEnumerable<TasteDTO>>> GetTastes()
        {
            try
            {
                var tastes = await _dbContext.Taste.ToListAsync();
                return Ok(tastes.ConvertAll(m => new TasteDTO(m)));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status501NotImplemented);
            }
        }
        [Authorize]
        [HttpPost]
        [Route("AddTaste")]
        public async Task<ActionResult> AddTaste([FromQuery] string name)
        {
            try
            {
                var type = await _dbContext.Taste.FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower());
                if (type == null)
                {
                    await _dbContext.Taste.AddAsync(new Taste { Name = name });
                    await _dbContext.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    return BadRequest("Такой вкус уже есть в базе данных");
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status501NotImplemented, "");
            }
        }
    }
}
