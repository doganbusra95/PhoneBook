using Microsoft.EntityFrameworkCore;
using PhoneBookApi.DContext;
using PhoneBookApi.Models;

namespace PhoneBookApi.Services
{
    public class ReportService
    {
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task GenerateReport(Guid reportId)
        {
            var report = await _context.PB_REPORTS.FindAsync(reportId);

            if (report == null) return;

            try
            {
         
                var locationStats = await _context.PB_CONTACT_INFO
                    .Where(x => x.ContactType == ContactType.Location)
                    .GroupBy(x => x.Info)
                    .Select(group => new LocationStat
                    {
                        Location = group.Key,
                        PersonCount = group.Count(),
                        PhoneNumberCount = _context.PB_CONTACT_INFO.Count(ci => ci.Info == group.Key && ci.ContactType == ContactType.Phone_Number)
                    })
                    .ToListAsync();

          
                report.Status = ReportStatus.Completed;
                await _context.SaveChangesAsync();

           
            }
            catch (Exception ex)
            {
            
                report.Status = ReportStatus.Failed;
                await _context.SaveChangesAsync();
            }
        }
    }
}
