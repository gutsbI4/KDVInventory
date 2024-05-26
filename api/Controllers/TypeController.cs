using api.Data;
using api.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TypeController : ControllerBase
    {
        private KDVDbContext _dbContext;
        public TypeController(KDVDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [HttpGet]
        [Route("GetTypes")]
        public async Task<ActionResult<IEnumerable<TypeDTO>>> GetTypes()
        {
            try
            {
                var types = await _dbContext.Type.ToListAsync();
                return Ok(types.ConvertAll(m => new TypeDTO(m)));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status501NotImplemented);
            }
        }
        [Authorize]
        [HttpPost]
        [Route("AddType")]
        public async Task<ActionResult> AddType([FromQuery] string name)
        {
            try
            {
                var type = await _dbContext.Type.FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower());
                if(type == null)
                {
                    await _dbContext.Type.AddAsync(new Models.Type { Name = name });
                    await _dbContext.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    return BadRequest("Такой вид уже есть в базе данных");
                }
            }
            catch (Exception)
            {
              return StatusCode(StatusCodes.Status501NotImplemented, "");
            }
        }
    }
}
