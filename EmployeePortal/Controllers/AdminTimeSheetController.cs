using EmployeePortal.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

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

        [HttpGet("all/{authId}")]
        public async Task<IActionResult> GetEmployeesForAdmin(string authId)
        {
            // Get EmployeeId from EmployeeLogin table using the provided authId
            var employeeLogin = await _context.EmployeeLogins.FirstOrDefaultAsync(el => el.AuthId == authId);
            if (employeeLogin == null)
            {
                return NotFound("Admin not found.");
            }
            var adminEmployeeId = employeeLogin.EmployeeId;

            // Get the list of EmployeeIds that this admin can approve from ApproverXEmployee table
            var approvableEmployeeIds = _context.ApproverXemployees
                .Where(axe => axe.Approver == adminEmployeeId)
                .Select(axe => axe.EmployeeId)
                .ToList();

            // Get the employees and their timesheet data
            var employees = _context.Employees
                .Where(emp => approvableEmployeeIds.Contains(emp.EmployeeId))
                .Select(emp => new
                {
                    emp.EmployeeId,
                    Name = emp.FirstName + " " + emp.LastName,
                    TSFreq = emp.Tsfreq.Trim(),
                })
                .ToList();

            var employeesData = _context.TimeSheets
                .Where(ts => ts.WorkingDate.HasValue && approvableEmployeeIds.Contains(ts.EmployeeId))
                .ToList()
                .GroupBy(ts => ts.EmployeeId)
                .Select(g => new
                {
                    EmployeeId = g.Key,
                    Records = g
                        .GroupBy(ts => ts.RecordNumber)
                        .Select(rg => new
                        {
                            RecordNumber = rg.Key,
                            ApprovalStatus = rg.FirstOrDefault().ApprovalStatus,
                        })
                        .OrderBy(r => r.RecordNumber)
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
                    TSFreq = e.TSFreq == "M" ? "Monthly" : e.TSFreq == "W" ? "Weekly" : e.TSFreq == "B" ? "Bi-weekly" : e.TSFreq, // Map the TSFreq values
                    ed.Records
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
                    Hours = ts.WorkingHours.HasValue ? (int)ts.WorkingHours.Value : 0,
                    RecordNumber = ts.RecordNumber
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


        [HttpGet("{employeeId}/status")]
        public IActionResult GetEmployeeTimesheetsByStatus(Guid employeeId, [FromQuery] string status)
        {
            // Fetch employee details
            var employee = _context.Employees
                .Where(emp => emp.EmployeeId == employeeId)
                .Select(emp => new
                {
                    emp.EmployeeId,
                    Name = emp.FirstName + " " + emp.LastName
                })
                .FirstOrDefault();

            if (employee == null)
            {
                return NotFound($"Employee with ID {employeeId} not found.");
            }

            // Fetch and filter timesheets by status for the specified employee
            var timesheets = _context.TimeSheets
                .Where(ts => ts.EmployeeId == employeeId && ts.ApprovalStatus == status)
                .ToList()
                .GroupBy(ts => new { ts.RecordNumber, Year = ts.WorkingDate.Value.Year, Month = ts.WorkingDate.Value.Month })
                .Select(rg => new
                {
                    RecordNumber = rg.Key.RecordNumber,
                    Year = rg.Key.Year,
                    Month = rg.Key.Month,
                    ApprovalStatus = rg.FirstOrDefault().ApprovalStatus,
                    Timesheets = rg.Select(ts => new
                    {
                        ts.ProjectId,
                        ts.EmployeeId,
                        ts.WorkingDate,
                        ts.SubmissionDate,
                        ts.ApprovalStatus,
                        ts.ApprovedBy,
                        ts.CreatedUser,
                        ts.UpdatedUser,
                        ts.CreatedTime,
                        ts.UpdatedTime,
                        ts.WhatOperation,
                        ts.WorkingHours,
                        ts.RecordId,
                        ts.RecordNumber,
                        ts.WorkingDate.Value.Year,
                        ts.WorkingDate.Value.Month
                    }).ToList()
                })
                .OrderBy(r => r.RecordNumber)
                .ToList();

            if (!timesheets.Any())
            {
                return NotFound($"No timesheets found for employee {employeeId} with status {status}.");
            }

            // Combine employee details with timesheet data
            var result = new
            {
                employee.EmployeeId,
                employee.Name,
                Records = timesheets
            };

            return Ok(result);
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

        [HttpPost("update")]
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

        [HttpPut("approve")]
        public async Task<IActionResult> ApproveTimeSheet([FromBody] ApproverTimeSheetAppRej approverTimeSheetAppRej)
        {
            return await UpdateApprovalStatus(approverTimeSheetAppRej, "A");
        }

        [HttpPut("reject")]
        public async Task<IActionResult> RejectTimeSheet([FromBody] ApproverTimeSheetAppRej approverTimeSheetAppRej)
        {
            return await UpdateApprovalStatus(approverTimeSheetAppRej, "R");
        }

        private async Task<IActionResult> UpdateApprovalStatus(ApproverTimeSheetAppRej approverTimeSheetAppRej, string status)
        {
            if (approverTimeSheetAppRej == null || string.IsNullOrEmpty(approverTimeSheetAppRej.RecordNumber) || approverTimeSheetAppRej.EmployeeId == Guid.Empty)
            {
                return BadRequest("Invalid data received for updating the approval status.");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Fetch all records with the given RecordNumber and EmployeeId
                    var timesheetEntries = _context.TimeSheets
                        .Where(ts => ts.RecordNumber == approverTimeSheetAppRej.RecordNumber && ts.EmployeeId == approverTimeSheetAppRej.EmployeeId)
                        .ToList();

                    if (timesheetEntries.Count == 0)
                    {
                        return NotFound($"No timesheet entries found with RecordNumber {approverTimeSheetAppRej.RecordNumber} for EmployeeId {approverTimeSheetAppRej.EmployeeId}");
                    }

                    // Update the ApprovalStatus for all fetched records
                    foreach (var entry in timesheetEntries)
                    {
                        entry.ApprovalStatus = status;
                        entry.UpdatedTime = DateTime.Now; // Set updated time to current time
                    }

                    // Save changes to the database
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return Ok(new { Message = $"Timesheet records with RecordNumber {approverTimeSheetAppRej.RecordNumber} for EmployeeId {approverTimeSheetAppRej.EmployeeId} have been {status.ToLower()} successfully." });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError($"Error updating timesheet approval status: {ex}");
                    return StatusCode(500, "Internal Server Error");
                }
            }
        }


    }
}
