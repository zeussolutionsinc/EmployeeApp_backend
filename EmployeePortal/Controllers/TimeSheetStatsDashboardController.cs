using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EmployeePortal.DTO;
using Microsoft.Extensions.Logging; // Make sure to include this if using ILogger

namespace EmployeePortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeSheetStatsDashboardController : ControllerBase
    {
        private readonly EmpPortalContext _context;
        private readonly ILogger<TimeSheetStatsDashboardController> _logger; // Correct the logger generic type if needed

        // Constructor to initialize context and logger
        public TimeSheetStatsDashboardController(EmpPortalContext context, ILogger<TimeSheetStatsDashboardController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("authid/{authid}")]  // Ensure this is inside the class
        public IActionResult Get(string authid)
        {
            if (authid == null)
            {
                return Unauthorized();
            }

            var authId = _context.EmployeeLogins.FirstOrDefault(ea => ea.AuthId == authid);

            if (authId == null)
            {
                return NotFound();
            }

            var Employee = _context.EmployeeLogins
                                   .Where(ea => ea.AuthId == authid)
                                   .Select(ea => ea.EmployeeId)
                                   .FirstOrDefault();


            if (Employee == null)
            {
                return NotFound("No employee found.");
            }

            var approved = _context.TimeSheets
                                   .Where(ts => ts.ApprovalStatus == "A" && ts.EmployeeId == Employee)
                                   .Select(ts => ts.RecordNumber)
                                   .Distinct()
                                   .Count();

            var pending = _context.TimeSheets
                                  .Where(ts => ts.ApprovalStatus == "P" && ts.EmployeeId == Employee)
                                  .Select(ts => ts.RecordNumber)
                                  .Distinct()
                                  .Count();

            var rejected = _context.TimeSheets
                                   .Where(ts => ts.ApprovalStatus == "R" && ts.EmployeeId == Employee)
                                   .Select(ts => ts.RecordNumber)
                                   .Distinct()
                                   .Count();

            var submitted = _context.TimeSheets
                                    .Where(ts => ts.ApprovalStatus == "S" && ts.EmployeeId == Employee)
                                    .Select(ts => ts.RecordNumber)
                                    .Distinct()
                                    .Count();

            var total = _context.TimeSheets
                                .Where(ts => ts.EmployeeId == Employee)
                                .Select(ts => ts.RecordNumber)
                                .Distinct()
                                .Count();

            var tsfreq = _context.Employees
                                          .Where(pe => pe.EmployeeId == Employee)
                                          .Select(pe => pe.Tsfreq)
                                          .FirstOrDefault();
                                          

            var results = new TimeSheetStatsDashboardDTO()
            {
                ApprovedRecords = approved,
                PendingRecords = pending,
                RejectedRecords = rejected,
                SubmittedRecords = submitted,
                TotalRecords = total,
                TimeSheetFreq = tsfreq?.Trim()

            };

            return Ok(results);
        }
    }
}

