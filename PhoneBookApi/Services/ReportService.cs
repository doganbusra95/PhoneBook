using Microsoft.EntityFrameworkCore;
using PhoneBookApi.DContext;
using PhoneBookApi.Models;
using PhoneBookApi.Models.Dtos;

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
                        PersonCount = group.Select(x=>x.PersonId).Distinct().Count(),
                        PhoneNumberCount = _context.PB_CONTACT_INFO.Where(ci => group.Select(g => g.PersonId).Contains(ci.PersonId) && ci.ContactType == ContactType.Phone_Number).Count()
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

        public async Task<List<Reports>> GetAllReportsAsync()
        {
            List<Reports> reports = new List<Reports>();
             reports = await _context.PB_REPORTS.ToListAsync();
            return reports;

        }

        public async Task<ReportDto> CreateReportAsync()
        {
            ReportDto reportDto = new ReportDto();
            var report = new Reports
            {
                Id = Guid.NewGuid(),
                RequestDate = DateTime.Now,
                Status = ReportStatus.Preparing,
            };

            
            try
            {
                _context.PB_REPORTS.Add(report);
                await _context.SaveChangesAsync();
               
                
            }
            catch (Exception ex)
            {

                reportDto.ErrorMessage = ex.Message;
            }

            if (string.IsNullOrEmpty(reportDto.ErrorMessage)) { reportDto.Id = report.Id; }

            return reportDto;
        }
    }
}
