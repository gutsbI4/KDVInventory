using api.Data;
using api.Models;

namespace api.Services
{
    public static class AuditLogger
    {
        public static async Task AddAuditRecord(KDVDbContext dbContext, int userId, string actionPerformed)
        {
            var auditRecord = new Audit
            {
                DateOfAction = DateTime.Now,
                UserId = userId,
                Action = actionPerformed
            };

            await dbContext.Audit.AddAsync(auditRecord);
            await dbContext.SaveChangesAsync();
        }
    }
}
