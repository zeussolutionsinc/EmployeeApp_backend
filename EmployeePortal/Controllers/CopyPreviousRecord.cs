using EmployeePortal.DTO;
using Microsoft.AspNetCore.Mvc;

namespace EmployeePortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CopyPreviousRecord : ControllerBase
    {
        private readonly EmpPortalContext _context;
        private readonly ILogger<CopyPreviousRecord> _logger; // Declare the logger

        // Correct the constructor to include ILogger<H1bcontroller> parameter
        public CopyPreviousRecord(EmpPortalContext context, ILogger<CopyPreviousRecord> logger)
        {
            _context = context;
            _logger = logger; // Initialize the logger correctly
        }

        //[HttpGet]
        //public IActionResult GetCopyPrevRecord(string EmpId)
        //{
        //    var empGuid = new Guid(EmpId);

        //    var today = DateOnly.FromDateTime(DateTime.Today);
        //    var firstDayOfMonth = new DateOnly(today.Year, today.Month, 1);
        //    var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

        //    var timeSheet = _context.Employees
        //                            .Where(e => e.EmployeeId == empGuid)
        //                            .Select(e => e.Tsfreq)
        //                            .FirstOrDefault();

        //    var latestRecordNo = _context.TimeSheets
        //                          .Where(t => t.EmployeeId == empGuid)
        //                          .GroupBy(t => t.RecordNumber)
        //                          .Select(g => new { RecordNumber = g.Key, SubmissionDate = g.First().SubmissionDate })
        //                          .OrderByDescending(x => x.SubmissionDate)
        //                          .Select(x => x.RecordNumber)
        //                          .FirstOrDefault();

        //    if (latestRecordNo == null)
        //    {
        //        return NotFound();
        //    }

        //    var projectDateHours = _context.TimeSheets
        //        .Where(ts => ts.RecordNumber == latestRecordNo &&
        //                     ts.WorkingDate.HasValue)
        //        .Select(ts => new
        //        {
        //            ProjectId = ts.ProjectId,
        //            Date = ts.WorkingDate.Value,
        //            Hours = (int)ts.WorkingHours
        //        }).ToList();


        //    Transform results into List<ProjectDateHoursEntry>

        //   var ProjectDateHoursList = new List<ProjectDateHoursEntry>();

        //    if (timeSheet == "W")
        //    {
        //        ProjectDateHoursList = projectDateHours.Select(r => new ProjectDateHoursEntry
        //        {
        //            ProjectId = r.ProjectId,
        //            WorkingDate = r.Date.AddDays(7),
        //            Hours = r.Hours
        //        })
        //            .Where(p => p.WorkingDate.Month == today.Month)
        //            .ToList();
        //    }

        //    if (timeSheet == "B")
        //    {
        //        ProjectDateHoursList = projectDateHours.Select(r => new ProjectDateHoursEntry
        //        {
        //            ProjectId = r.ProjectId,
        //            WorkingDate = r.Date.AddDays(14),
        //            Hours = r.Hours
        //        })
        //            .Where(p => p.WorkingDate.Month == today.Month)
        //            .ToList();
        //    }

        //    if (timeSheet == "M")
        //    {
        //        ProjectDateHoursList = projectDateHours.Select(r => new ProjectDateHoursEntry
        //        {
        //            ProjectId = r.ProjectId,
        //            WorkingDate = r.Date.AddMonths(1),
        //            Hours = r.Hours
        //        }).ToList();
        //    }
        //    return Ok(ProjectDateHoursList);


        //}


        [HttpGet]
        public IActionResult GetCopyPrevRecord(string EmpId)
        {
            if (!Guid.TryParse(EmpId, out Guid empGuid))
            {
                return BadRequest("Invalid GUID format for Employee ID.");
            }

            var today = DateOnly.FromDateTime(DateTime.Today);
            var firstDayOfMonth = new DateOnly(today.Year, today.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            var timeSheet = _context.Employees
                                    .Where(e => e.EmployeeId == empGuid)
                                    .Select(e => e.Tsfreq)
                                    .FirstOrDefault()?.Trim(); 

            if (timeSheet == null)
            {
                return NotFound("TimeSheet frequency not found.");
            }

            var latestRecordNo = _context.TimeSheets
                                  .Where(t => t.EmployeeId == empGuid)
                                  .GroupBy(t => t.RecordNumber)
                                  .Select(g => new { RecordNumber = g.Key, SubmissionDate = g.First().SubmissionDate })
                                  .OrderByDescending(x => x.SubmissionDate)
                                  .Select(x => x.RecordNumber)
                                  .FirstOrDefault();

            if (latestRecordNo == null)
            {
                return NotFound("Latest record not found.");
            }

            var projectDateHours = _context.TimeSheets
                .Where(ts => ts.RecordNumber == latestRecordNo && ts.WorkingDate.HasValue)
                .Select(ts => new
                {
                    ProjectId = ts.ProjectId,
                    Date = ts.WorkingDate.Value,
                    Hours = (int)ts.WorkingHours
                }).ToList();

            if (projectDateHours == null || !projectDateHours.Any())
            {
                return NotFound("Project date hours not found.");
            }

            var ProjectDateHoursList = new List<ProjectDateHoursEntry>();

            if (timeSheet == "W")
            {
                ProjectDateHoursList = projectDateHours.Select(r => new ProjectDateHoursEntry
                {
                    ProjectId = r.ProjectId,
                    WorkingDate = r.Date.AddDays(7),
                    Hours = r.Hours
                })
                .Where(p => p.WorkingDate.Month == today.Month)
                .ToList();
            }

            if (timeSheet == "B")
            {
                ProjectDateHoursList = projectDateHours.Select(r => new ProjectDateHoursEntry
                {
                    ProjectId = r.ProjectId,
                    WorkingDate = r.Date.AddDays(14),
                    Hours = r.Hours
                })
                .Where(p => p.WorkingDate.Month == today.Month)
                .ToList();
            }

            if (timeSheet == "M")
            {
                ProjectDateHoursList = projectDateHours.Select(r => new ProjectDateHoursEntry
                {
                    ProjectId = r.ProjectId,
                    WorkingDate = r.Date.AddMonths(1),
                    Hours = r.Hours
                })
                // Added month filtering for "M"
                .ToList();
            }

            return Ok(ProjectDateHoursList);
        }

    }
}
