using EmployeePortal.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace EmployeePortal.Controllers
{
    [Authorize] // This attribute applies to all actions within this controller
    [Route("api/[controller]")]
    [ApiController]
    public class TimeSheetController : ControllerBase
    {
        private readonly EmpPortalContext _context;
        private readonly ILogger<TimeSheetController> _logger; // Declare the logger

        // Correct the constructor to include ILogger<H1bcontroller> parameter
        public TimeSheetController(EmpPortalContext context, ILogger<TimeSheetController> logger)
        {
            _context = context;
            _logger = logger; // Initialize the logger correctly
        }

        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            // Attempt to fetch the first employeeId found, this could be adjusted based on specific requirements
            var EmployeeId = _context.TimeSheets.Select(e => e.EmployeeId).FirstOrDefault();

            if (EmployeeId == null)
            {
                return NotFound("No employee found.");
            }

            var today = DateOnly.FromDateTime(DateTime.Today);
            var firstDayOfMonth = new DateOnly(today.Year, today.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            // Query to get all dates of the current month and corresponding hours worked
            var projectDateHours = _context.TimeSheets
                .Where(ts => ts.EmployeeId == EmployeeId &&
                             ts.WorkingDate.HasValue &&
                             ts.WorkingDate.Value >= firstDayOfMonth &&
                             ts.WorkingDate.Value <= lastDayOfMonth)
                .Select(ts => new
                {
                    ProjectId = ts.ProjectId,
                    Date = ts.WorkingDate.Value,
                    Hours = ts.WorkingHours.HasValue ? (int)ts.WorkingHours.Value : 0
                }).ToList();


            // Transform results into List<ProjectDateHoursEntry>
            var ProjectDateHoursList = projectDateHours.Select(r => new ProjectDateHoursEntry
            {
                ProjectId = r.ProjectId,
                WorkingDate = r.Date,
                Hours = r.Hours
            }).ToList();


            // finding all unique project ids that are in the timesheet
            var exisitngProjects = ProjectDateHoursList.Select(e => e.ProjectId).Distinct().ToHashSet();


            // Getting all projectids without any timesheet entries
            var projectsfromprojectsXemployee = _context.ProjectXemployees
                                                .Where(e => e.EmployeeId == EmployeeId)
                                                .Select(e => e.ProjectId)
                                                .ToList();

            // converting it into ProjectDateHours format and ignoring those already found
            var projectsfromprojectsXemployeeObject = projectsfromprojectsXemployee
                .Where(projectfromprojectsXemployee => !exisitngProjects.Contains(projectfromprojectsXemployee))
                .Select(projectid => new ProjectDateHoursEntry
                {
                    ProjectId = projectid,
                    WorkingDate = default,
                    Hours = 0
                }).ToList();

            // Combining all found projects for an emaployee
            ProjectDateHoursList.AddRange(projectsfromprojectsXemployeeObject);

            // Fetch additional information about the employee, project, and client
            var EmployeeFirstName = _context.Employees
                                .Where(e => e.EmployeeId == EmployeeId)
                                .Select(e => e.FirstName)
                                .FirstOrDefault();

            var EmployeeLastName = _context.Employees
                                    .Where(e => e.EmployeeId == EmployeeId &&
                                    e.FirstName == EmployeeFirstName)
                                    .Select(e => e.LastName)
                                    .FirstOrDefault();

            var projectId = _context.TimeSheets
                .Where(ts => ts.EmployeeId == EmployeeId)
                .Select(ts => ts.ProjectId)
                .FirstOrDefault();

            var Approver = _context.TimeSheets
                .Where(ts => ts.EmployeeId == EmployeeId)
                .Select(ts => ts.ApprovedBy)
                .FirstOrDefault();

            var ProjectName = _context.Projects
                              .Where(p => p.ProjectId == projectId)
                              .Select(p => p.ProjectName)
                              .FirstOrDefault();

            var ClientName = _context.Projects
                             .Where(p => p.ProjectId == projectId)
                             .Select(p => p.ClientName)
                             .FirstOrDefault();

            // Prepare the DTO for response
            var employeeInfo = new TimeSheetInfoDTO
            {
                EmployeeId = EmployeeId,
                EmployeeName = EmployeeFirstName + " " + EmployeeLastName,
                ProjectName = ProjectName,
                ClientName = ClientName,
                ProjectDateHours = ProjectDateHoursList,
                Approver = Approver,
            };

            return Ok(employeeInfo);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateTimeSheet([FromBody] TimeSheetEntryDTO timeSheetEntryDto)
        {
            if (timeSheetEntryDto == null || timeSheetEntryDto.ProjectDateHours == null)
            {
                return BadRequest("No data is received for the time sheet entry.");
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var timeSheetEntries = new List<TimeSheet>();
                    foreach (var entry in timeSheetEntryDto.ProjectDateHours)
                    {
                        var existingEntry = _context.TimeSheets.FirstOrDefault(ts =>
                            ts.EmployeeId == timeSheetEntryDto.EmployeeId &&
                            ts.ProjectId == entry.ProjectId &&
                            ts.WorkingDate == entry.WorkingDate);

                        if (existingEntry != null)
                        {
                            if (existingEntry.WorkingHours != entry.Hours)
                            {
                                await UpdateTimeSheet(existingEntry, entry, timeSheetEntryDto.SubmissionDate);
                            }
                            continue;
                        }

                        var timesheetEntry = new TimeSheet
                        {
                            EmployeeId = timeSheetEntryDto.EmployeeId,
                            ProjectId = entry.ProjectId,
                            WorkingDate = entry.WorkingDate,
                            WorkingHours = (short)entry.Hours,
                            CreatedTime = DateTime.UtcNow,
                            UpdatedTime = DateTime.UtcNow,
                            WhatOperation = "I",
                            SubmissionDate = timeSheetEntryDto.SubmissionDate,
                            ApprovedBy = timeSheetEntryDto.Approver,
                        };
                        timeSheetEntries.Add(timesheetEntry);
                    }

                    _context.TimeSheets.AddRange(timeSheetEntries);
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                    return Ok(new { Message = "Submission Successful" });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError($"Error saving time sheet entry: {ex}");
                    return StatusCode(500, "Internal Server Error");
                }
            }
        }

        private async Task<IActionResult> UpdateTimeSheet(TimeSheet existingEntry, ProjectDateHoursEntry newData, DateOnly submissionDate)
        {
            _logger.LogInformation($"Updating timesheet for EmployeeID {existingEntry.EmployeeId} on date {existingEntry.WorkingDate}");

            existingEntry.WorkingHours = (short)newData.Hours;
            existingEntry.UpdatedTime = DateTime.UtcNow;
            existingEntry.WhatOperation = "U";
            existingEntry.SubmissionDate = submissionDate;

            try
            {
                _context.TimeSheets.Update(existingEntry);
                int changes = await _context.SaveChangesAsync();
                _logger.LogInformation($"Changes saved to database. Number of records updated: {changes}");

                return Ok(new { message = "Timesheet has been updated successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating the time sheet: {ex}");
                return StatusCode(500, "Internal Server Error");
            }
        }


    }
}
