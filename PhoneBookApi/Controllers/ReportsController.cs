using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneBookApi.DContext;
using PhoneBookApi.Models;
using PhoneBookApi.Models.Dtos;
using PhoneBookApi.Services;
using System.Composition;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PhoneBookApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {

        private readonly AppDbContext _context;
        private readonly ReportService _reportService;

        public ReportsController(AppDbContext context)
        {
            _context = context;
            _reportService = new ReportService(_context);
        }
       
        [HttpGet("ReportsList")]
        public async Task<IActionResult> GetAllReports()
        {
            var reports = await  _reportService.GetAllReportsAsync();
            return Ok(reports);
        }

        [HttpPost("RequestReport")]
        public async Task<IActionResult> RequestReport()
        {

            ReportDto reportDto = await _reportService.CreateReportAsync();
            if (string.IsNullOrEmpty(reportDto.ErrorMessage))
            {
                _ = Task.Run(() => _reportService.GenerateReport(reportDto.Id));

                return Ok(new { reportDto.Id });
            }
            else { return BadRequest(reportDto.ErrorMessage); }


          
        }

       


    }
}
