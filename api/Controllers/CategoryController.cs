using api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("[controller]")]
public class CategoryController : ControllerBase
{
    private KDVDbContext _dbContext;
    public CategoryController(KDVDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    [HttpGet]
    [Route("GetCategories")]
    public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategories()
    {
        try
        {
            var categories = await _dbContext.Category.ToListAsync();
            categories.Add(new api.Models.Category() { Name = "Все категории" });
            return Ok(categories.ConvertAll(m => new CategoryDTO(m)));
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status501NotImplemented);
        }
    }
}
