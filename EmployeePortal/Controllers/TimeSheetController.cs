using EmployeePortal.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace EmployeePortal.Controllers
{
    // [Authorize] // This attribute applies to all actions within this controller
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

        // [Authorize]
        [HttpGet("authid/{authid}")]
        public IActionResult GetTimeSheet(string authid)
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

            var today = DateOnly.FromDateTime(DateTime.Today);
            var firstDayOfMonth = new DateOnly(today.Year, today.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            // Query to get all dates of the current month and corresponding hours worked
            var projectDateHours = _context.TimeSheets
                .Where(ts => ts.EmployeeId == Employee &&
                             ts.WorkingDate.HasValue &&
                             ts.WorkingDate.Value >= firstDayOfMonth &&
                             ts.WorkingDate.Value <= lastDayOfMonth)
                .Select(ts => new
                {
                    ProjectId = ts.ProjectId,
                    Date = ts.WorkingDate.Value,
                    Hours = ts.WorkingHours.HasValue ? (int)ts.WorkingHours.Value : 0,
                    ApprovalStatus = ts.ApprovalStatus,
                    RecordNumber = ts.RecordNumber
                }).ToList();


            // Transform results into List<ProjectDateHoursEntry>
            var ProjectDateHoursList = projectDateHours.Select(r => new ProjectDateHoursEntry
            {
                ProjectId = r.ProjectId,
                WorkingDate = r.Date,
                Hours = r.Hours,
                ApprovalStatus = r.ApprovalStatus,
                RecordNumber = r.RecordNumber
            }).ToList();


            // finding all unique project ids that are in the timesheet
            var exisitngProjects = ProjectDateHoursList.Select(e => e.ProjectId).Distinct().ToHashSet();


            // Getting all projectids without any timesheet entries
            var projectsfromprojectsXemployee = _context.ProjectXemployees
                                                .Where(e => e.EmployeeId == Employee)
                                                .Select(e => e.ProjectId)
                                                .ToList();

            // converting it into ProjectDateHours format and ignoring those already found
            var projectsfromprojectsXemployeeObject = projectsfromprojectsXemployee
                .Where(projectfromprojectsXemployee => !exisitngProjects.Contains(projectfromprojectsXemployee))
                .Select(projectid => new ProjectDateHoursEntry
                {
                    ProjectId = projectid,
                    WorkingDate = default,
                    Hours = 0,
                    ApprovalStatus = "P"
                }).ToList();

            // Combining all found projects for an emaployee
            ProjectDateHoursList.AddRange(projectsfromprojectsXemployeeObject);

            // Fetch additional information about the employee, project, and client
            var EmployeeFirstName = _context.Employees
                                .Where(e => e.EmployeeId == Employee)
                                .Select(e => e.FirstName)
                                .FirstOrDefault();

            var EmployeeLastName = _context.Employees
                                    .Where(e => e.EmployeeId == Employee &&
                                    e.FirstName == EmployeeFirstName)
                                    .Select(e => e.LastName)
                                    .FirstOrDefault();

           

            var Approver = _context.ApproverXemployees
                .Where(ts => ts.EmployeeId == Employee)
                .Select(ts => ts.Approver)
                .FirstOrDefault();

            // Prepare the DTO for response
            var employeeInfo = new TimeSheetInfoDTO
            {
                EmployeeId = Employee,
                EmployeeName = EmployeeFirstName + " " + EmployeeLastName,
                ProjectDateHours = ProjectDateHoursList,
                Approver = Approver,
            };

            return Ok(employeeInfo);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTimeSheet([FromQuery] string draftOrSave, [FromBody] TimeSheetEntryDTO timeSheetEntryDto)
        {
            if (timeSheetEntryDto == null || timeSheetEntryDto.ProjectDateHours == null)
            {
                return BadRequest("No data is received for the time sheet entry.");
            }

            bool newEntriesCreated = false;

            try
            {
                var timeSheetFreq = _context.Employees
                                            .Where(e => e.EmployeeId == timeSheetEntryDto.EmployeeId)
                                            .Select(e => e.Tsfreq.Trim())
                                            .FirstOrDefault();

                using (var transaction = _context.Database.BeginTransaction())
                {
                    try
                    {
                        var timeSheetEntries = new List<TimeSheet>();
                        var sortedEntries = timeSheetEntryDto.ProjectDateHours.OrderBy(e => e.WorkingDate).ToList();
                        string recordNo = null;
                        DateOnly? previousPeriodStartDate = null;

                        foreach (var entry in sortedEntries)
                        {
                            DateTime currentEntryDate = entry.WorkingDate.ToDateTime(TimeOnly.MinValue);
                            DateOnly periodStartDate = DateOnly.FromDateTime(currentEntryDate);
                            DateOnly periodEndDate = periodStartDate;

                            if (timeSheetFreq == "W")
                            {
                                // Calculate the start of the week (Sunday)
                                int daysToSubtract = (int)currentEntryDate.DayOfWeek;
                                periodStartDate = DateOnly.FromDateTime(currentEntryDate.AddDays(-daysToSubtract));
                                periodEndDate = periodStartDate.AddDays(6);
                            }
                            else if (timeSheetFreq == "B")
                            {
                                // Calculate the start of the bi-weekly period (Sunday)
                                int daysToSubtract = (int)currentEntryDate.DayOfWeek;
                                periodStartDate = DateOnly.FromDateTime(currentEntryDate.AddDays(-daysToSubtract));
                                periodEndDate = periodStartDate.AddDays(13);
                            }
                            else if (timeSheetFreq == "M")
                            {
                                periodStartDate = new DateOnly(currentEntryDate.Year, currentEntryDate.Month, 1);
                                periodEndDate = periodStartDate.AddMonths(1).AddDays(-1);
                            }

                            var existingRecord = _context.TimeSheets
                                .Where(ts => ts.EmployeeId == timeSheetEntryDto.EmployeeId && ts.WorkingDate >= periodStartDate && ts.WorkingDate <= periodEndDate)
                                .OrderByDescending(ts => ts.WorkingDate)
                                .Select(ts => ts.RecordNumber)
                                .FirstOrDefault();

                            if (existingRecord != null)
                            {
                                recordNo = existingRecord;
                            }
                            else if (previousPeriodStartDate == null || periodStartDate != previousPeriodStartDate)
                            {
                                recordNo = RecordGenerator.GenerateRecordNumber(timeSheetEntryDto.EmployeeId);
                                previousPeriodStartDate = periodStartDate;
                            }

                            var existingEntry = _context.TimeSheets.FirstOrDefault(ts =>
                                ts.EmployeeId == timeSheetEntryDto.EmployeeId &&
                                ts.ProjectId == entry.ProjectId &&
                                ts.WorkingDate == entry.WorkingDate);

                            if (existingEntry != null)
                            {
                                if (existingEntry.ApprovalStatus != "A")
                                {
                                    var updateResult = await UpdateTimeSheet(existingEntry, entry, timeSheetEntryDto.SubmissionDate, draftOrSave);
                                    if (updateResult is OkObjectResult)
                                    {
                                        newEntriesCreated = true; // Set this to true to indicate updates were made
                                    }
                                }
                            }
                            else
                            {
                                var appStat = draftOrSave == "Draft" ? "P" : "S";

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
                                    RecordNumber = recordNo,
                                    ApprovalStatus = appStat
                                };
                                timeSheetEntries.Add(timesheetEntry);
                                newEntriesCreated = true;
                            }
                        }

                        if (newEntriesCreated)
                        {
                            _context.TimeSheets.AddRange(timeSheetEntries);
                            await _context.SaveChangesAsync();
                            transaction.Commit();
                            return Ok(new { Message = $"Submission Successful" });
                        }
                        else
                        {
                            return Ok(new { Message = "Updated all the entries." });
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _logger.LogError($"Error saving time sheet entry: {ex}");
                        return StatusCode(500, "Internal Server Error");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"General Error: {ex}");
                return StatusCode(500, "Internal Server Error");
            }
        }






        public static class RecordGenerator
        {
            private static Dictionary<Guid, int> userRecordNumbers = new Dictionary<Guid, int>();
            private static readonly object lockObj = new object();

            public static string GenerateRecordNumber(Guid? userId)
            {
                if (userId == null)
                {
                    throw new ArgumentNullException(nameof(userId), "User ID cannot be null");
                }

                lock (lockObj)
                {
                    if (!userRecordNumbers.ContainsKey(userId.Value))
                    {
                        userRecordNumbers[userId.Value] = 10000;
                    }
                    else
                    {
                        userRecordNumbers[userId.Value]++;
                    }

                    return $"{userRecordNumbers[userId.Value]}";
                }
            }
        }


        private async Task<IActionResult> UpdateTimeSheet(TimeSheet existingEntry, ProjectDateHoursEntry newData, DateOnly submissionDate, string DraftOrSave)
        {
            _logger.LogInformation($"Updating timesheet for EmployeeID {existingEntry.EmployeeId} on date {existingEntry.WorkingDate}");

            var appStat = DraftOrSave == "Save" ? "S" : "P";

            if (existingEntry.WorkingHours != (short)newData.Hours)
            {
                existingEntry.WorkingHours = (short)newData.Hours;
            }
            existingEntry.UpdatedTime = DateTime.UtcNow;
            existingEntry.WhatOperation = "U";
            existingEntry.SubmissionDate = submissionDate;
            existingEntry.ApprovalStatus = appStat; // Always update the status here

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
