using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneBookApi.DContext;
using PhoneBookApi.Models;
using PhoneBookApi.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PhoneBookApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {

        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }
        // GET: api/<PeersonsController>
        [HttpGet("ReportsList")]
        public async Task<IActionResult> GetAllReports()
        {
            var reports = await _context.PB_REPORTS.ToListAsync();
            return Ok(reports);
        }

        [HttpPost("RequestReport")]
        public async Task<IActionResult> RequestReport()
        {
            // Yeni rapor oluşturuluyor
            var report = new Reports
            {
                Id = Guid.NewGuid(),
                RequestDate = DateTime.Now,
                Status = ReportStatus.Preparing,
            };

            _context.PB_REPORTS.Add(report);
            await _context.SaveChangesAsync();

            // Asenkron rapor işleme başlatılıyor
            _ = Task.Run(() => new ReportService(_context).GenerateReport(report.Id));

            return Ok(new { report.Id });
        }

       


    }
}
