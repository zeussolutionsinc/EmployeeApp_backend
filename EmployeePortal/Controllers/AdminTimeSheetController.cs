using EmployeePortal.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Collections.Generic;

namespace EmployeePortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminTimeSheetController : ControllerBase
    {
        private readonly EmpPortalContext _context;
        private readonly ILogger<AdminTimeSheetController> _logger;

        public AdminTimeSheetController(EmpPortalContext context, ILogger<AdminTimeSheetController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // New endpoint to fetch all employee IDs and their available timesheet months with approval status
        [HttpGet("all")]
        public IActionResult GetAllEmployeesWithMonths()
        {
            var employeeIds = _context.TimeSheets
                .Where(ts => ts.WorkingDate.HasValue)
                .Select(ts => ts.EmployeeId)
                .Distinct()
                .ToList();

            var employees = _context.Employees
                .Where(emp => employeeIds.Contains(emp.EmployeeId))
                .Select(emp => new
                {
                    emp.EmployeeId,
                    Name = emp.FirstName + " " + emp.LastName
                })
                .ToList();

            var employeesData = _context.TimeSheets
                .Where(ts => ts.WorkingDate.HasValue)
                .ToList()
                .GroupBy(ts => ts.EmployeeId)
                .Select(g => new
                {
                    EmployeeId = g.Key,
                    Months = g
                        .GroupBy(ts => new { ts.WorkingDate.Value.Year, ts.WorkingDate.Value.Month })
                        .Select(mg => new
                        {
                            Year = mg.Key.Year,
                            Month = mg.Key.Month,
                            ApprovalStatus = mg.Any(ts => ts.ApprovalStatus == "P") ? "P" : "A"
                        })
                        .OrderBy(date => date.Year)
                        .ThenBy(date => date.Month)
                        .ToList()
                })
                .ToList();

            var result = employeesData.Join(employees,
                ed => ed.EmployeeId,
                e => e.EmployeeId,
                (ed, e) => new
                {
                    e.EmployeeId,
                    e.Name,
                    ed.Months
                }).ToList();

            if (!result.Any())
            {
                return NotFound("No employees found.");
            }

            return Ok(result);
        }


        [HttpGet("{employeeId}")]
        public IActionResult GetAllTimeSheets(Guid employeeId)
        {
            var timesheets = _context.TimeSheets
                .Where(ts => ts.EmployeeId == employeeId)
                .Select(ts => new
                {
                    ts.ProjectId,
                    ts.WorkingDate,
                    Hours = ts.WorkingHours.HasValue ? (int)ts.WorkingHours.Value : 0,
                    ts.ApprovalStatus,
                    ts.ApprovedBy
                }).ToList();

            if (!timesheets.Any())
            {
                return NotFound("No timesheets found for this employee.");
            }

            var timesheetsByMonth = timesheets
                .GroupBy(ts => new { ts.WorkingDate.Value.Year, ts.WorkingDate.Value.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    ApprovalStatus = g.Any(ts => ts.ApprovalStatus == "P") ? "P" : "A", // Adjust the default status as needed
                    TimeSheets = g.Select(ts => new ProjectDateHoursEntry
                    {
                        ProjectId = ts.ProjectId,
                        WorkingDate = ts.WorkingDate.Value,
                        Hours = ts.Hours
                    }).ToList()
                }).ToList();

            var employeeInfo = new
            {
                EmployeeId = employeeId,
                EmployeeName = _context.Employees
                                        .Where(e => e.EmployeeId == employeeId)
                                        .Select(e => e.FirstName + " " + e.LastName)
                                        .FirstOrDefault(),
                TimesheetsByMonth = timesheetsByMonth
            };

            return Ok(employeeInfo);
        }

        [HttpGet("{employeeId}/dateRange")]
        public IActionResult GetTimeSheetsByDateRange(Guid employeeId, [FromQuery] DateOnly startDate, [FromQuery] DateOnly endDate)
        {
            var timesheets = _context.TimeSheets
                .Where(ts => ts.EmployeeId == employeeId &&
                             ts.WorkingDate.HasValue &&
                             ts.WorkingDate.Value >= startDate &&
                             ts.WorkingDate.Value <= endDate)
                .Select(ts => new ProjectDateHoursEntry
                {
                    ProjectId = ts.ProjectId,
                    WorkingDate = ts.WorkingDate.Value,
                    Hours = ts.WorkingHours.HasValue ? (int)ts.WorkingHours.Value : 0
                }).ToList();

            if (!timesheets.Any())
            {
                return NotFound("No timesheets found for this date range.");
            }

            return Ok(timesheets);
        }


        private static readonly Dictionary<string, string> StatusMapping = new Dictionary<string, string>
            {
                { "P", "Pending" },
                { "A", "Approved" },
                { "R", "Rejected" },
                { "S", "Submitted" }
            };

        // New endpoint to fetch timesheets for a specific employee and status
        // New endpoint to fetch timesheets for a specific employee and status
        [HttpGet("{employeeId}/status")]
        public IActionResult GetTimeSheetsByStatus(Guid employeeId, [FromQuery] string status)
        {
            var timesheets = _context.TimeSheets
                .Where(ts => ts.EmployeeId == employeeId)
                .Select(ts => new
                {
                    ts.ProjectId,
                    ts.WorkingDate,
                    Hours = ts.WorkingHours.HasValue ? (int)ts.WorkingHours.Value : 0,
                    ts.ApprovalStatus,
                    ts.ApprovedBy
                }).ToList();

            if (!timesheets.Any())
            {
                return NotFound($"No timesheets found for employee {employeeId}.");
            }

            var timesheetsGroupedByMonth = timesheets
                .GroupBy(ts => new { ts.WorkingDate.Value.Year, ts.WorkingDate.Value.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Status = g.Any(ts => ts.ApprovalStatus == "P") ? "Pending" :
                             g.All(ts => ts.ApprovalStatus == "A") ? "Approved" :
                             g.All(ts => ts.ApprovalStatus == "R") ? "Rejected" :
                             g.All(ts => ts.ApprovalStatus == "S") ? "Submitted" : "Mixed",
                    Timesheets = g.Select(ts => new ProjectDateHoursEntry
                    {
                        ProjectId = ts.ProjectId,
                        WorkingDate = ts.WorkingDate.Value,
                        Hours = ts.Hours
                    }).ToList()
                }).ToList();

            var filteredGroupedByMonth = timesheetsGroupedByMonth
                .Where(g => g.Status == StatusMapping[status])
                .ToList();

            if (!filteredGroupedByMonth.Any())
            {
                return NotFound($"No timesheets found for employee {employeeId} with status {status}.");
            }

            return Ok(filteredGroupedByMonth);
        }


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
