using api.Data;
using api.Models.DTO;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Security.Claims;
using System.Text.Json;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuditController : Controller
    {
        private KDVDbContext _dbContext;
        public AuditController(KDVDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        [Authorize]
        [HttpGet]
        [Route("GetAuditByUserId/{id}")]
        public async Task<ActionResult<ObservableCollection<AuditDTO>>> GetAuditByUser(int id)
        {
                var user = await _dbContext.User
                    .Include(u => u.Audit)
                    .FirstOrDefaultAsync(u => u.UserId == id);

                if (user == null)
                {
                    return NotFound();
                }

                var auditData = user.Audit
                    .GroupBy(a => a.DateOfAction.Date)
                    .Select(g => new AuditDTO
                    {
                        Date = g.Key,
                        TimeEntry = g.Min(a => a.DateOfAction).ToString("HH:mm:ss"),
                        QuantityActivity = g.Count()
                    })
                    .OrderByDescending(x=>x.Date)
                    .ToList();

                return Ok(auditData);
        }
        [Authorize]
        [HttpPost]
        [Route("SaveAuditToFile/{id}")]
        public async Task<IActionResult> SaveAuditToFile(int id, [FromBody] AuditTimeDTO audit)
        {
            var user = await _dbContext.User
                .Include(u => u.Audit)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            var auditData = user.Audit
                .Where(a => a.DateOfAction.Date == audit.Date)
                .OrderBy(a => a.DateOfAction)
                .ToList();

            if (!auditData.Any())
            {
                return NotFound($"No audit data found for user (ID: {id}) on {audit.Date.ToShortDateString()}.");
            }

            string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AuditReports");
            Directory.CreateDirectory(directoryPath);
            string fileName = $"AuditReport_User_{id}_Date_{audit.Date:yyyyMMdd}.txt";
            string filePath = Path.Combine(directoryPath, fileName);

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine($"Audit Report for User: {user.Login} (ID: {id}) - {audit.Date:yyyy-MM-dd}");
                writer.WriteLine(new string('-', 50));

                foreach (var audit2 in auditData)
                {
                    writer.WriteLine($"Time: {audit2.DateOfAction:HH:mm:ss}");
                    writer.WriteLine($"Action: {audit2.Action}");
                    writer.WriteLine(new string('-', 50));
                }
            }

            return Ok($"Audit report saved successfully to {filePath}");
        }
    }
}
