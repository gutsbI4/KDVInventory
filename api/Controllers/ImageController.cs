using api.Data;
using api.Models;
using api.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("[controller]")]
public class ImageController: ControllerBase
{
    KDVDbContext _dbContext;
    public ImageController(KDVDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("{*image}", Name = "GetImage")]
    public ActionResult GetImage(string image)
    {
        try
        {
            if (image.Contains("..") || image.Contains(":") || Path.IsPathRooted(image))
            {
                return BadRequest("Invalid image path.");
            }

            var filePath = Path.Combine("Images", image);
            var bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, "image/jpeg");
        }
        catch (FileNotFoundException)
        {
            return NotFound();
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
    [Authorize]
	[HttpPost]
	[Route("UploadImage")]
	public async Task<ActionResult<string>> UploadImage([FromForm] IFormFile file)
	{
		if (file.ContentType != "image/jpeg")
			return BadRequest("Неверный формат данных");
		string fileName = "Products/" + Path.GetRandomFileName() + ".jpg";
		try
		{
			using (var stream = new FileStream($"Images/{fileName}", FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}
			return Ok(fileName);
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			return StatusCode(StatusCodes.Status500InternalServerError);
		}
	}
}
