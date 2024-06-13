using EmployeePortal.DTO;
using Microsoft.AspNetCore.Mvc;

namespace EmployeePortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecordRetrieveController : ControllerBase
    {
        private readonly EmpPortalContext _context;
        private readonly ILogger<RecordRetrieveController> _logger; // Declare the logger

        // Correct the constructor to include ILogger<H1bcontroller> parameter
        public RecordRetrieveController(EmpPortalContext context, ILogger<RecordRetrieveController> logger)
        {
            _context = context;
            _logger = logger; // Initialize the logger correctly
        }


        [HttpGet]
        public IActionResult GetRecord(string RecordNumber)
        {
            if (RecordNumber == null)
            {
                return NotFound();
            }

            //var today = DateOnly.FromDateTime(DateTime.Today);
            //var firstDayOfMonth = new DateOnly(today.Year, today.Month, 1);
            //var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            var projectDateHours = _context.TimeSheets
                .Where(ts => ts.RecordNumber == RecordNumber &&
                             ts.WorkingDate.HasValue)
                .Select(ts => new
                {
                    ProjectId = ts.ProjectId,
                    Date = ts.WorkingDate.Value,
                    Hours = (int)ts.WorkingHours
                }).ToList();


            // Transform results into List<ProjectDateHoursEntry>
            var ProjectDateHoursList = projectDateHours.Select(r => new ProjectDateHoursEntry
            {
                ProjectId = r.ProjectId,
                WorkingDate = r.Date,
                Hours = r.Hours
            }).ToList();



            return Ok(ProjectDateHoursList);
        }
    }
}
