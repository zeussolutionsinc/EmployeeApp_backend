using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EmployeePortal.DTO;
using System.Text;

namespace EmployeePortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PreviousRecordsController : ControllerBase
    {
        private readonly EmpPortalContext _context;
        private readonly ILogger<PreviousRecordsController> _logger; // Declare the logger

        // Correct the constructor to include ILogger<H1bcontroller> parameter
        public PreviousRecordsController(EmpPortalContext context, ILogger<PreviousRecordsController> logger)
        {
            _context = context;
            _logger = logger; // Initialize the logger correctly
        }

        [HttpGet("authid/{authid}")]
        public IActionResult GetPrevRecords(string authid)
        {
            if (authid == null)
            {
                return Unauthorized();
            }

            var authId = _context.EmployeeXauthIds.FirstOrDefault(ea => ea.AuthId == authid);

            if (authId == null)
            {
                return NotFound();
            }

            var employee = _context.EmployeeXauthIds
                                   .Where(ea =>  ea.AuthId == authid)
                                   .Select(ea => ea.EmployeeId)
                                   .FirstOrDefault();

            // TODO : relate to a particular employee
            List<PreviousRecordDTO> result = new List<PreviousRecordDTO>();

            var recordNumbers = _context.TimeSheets
                                .Where(ts => ts.EmployeeId == employee)
                                .GroupBy(ts => ts.RecordNumber)
                                .Select(g => new {RecordNumber = g.Key, SubmissionDate = g.First().SubmissionDate})
                                .OrderByDescending(x => x.SubmissionDate)
                                .Take(5)    
                                .Select(x => x.RecordNumber)
                                .ToList();


            foreach (var record in recordNumbers)
            {
                var submissionDate = _context.TimeSheets
                                     .Where(ts => ts.RecordNumber == record)
                                     .Select(ts => ts.SubmissionDate)
                                     .FirstOrDefault();

                var totalHours = _context.TimeSheets
                            .Where(ts => ts.RecordNumber == record)
                            .Sum(ts => ts.WorkingHours);

                var approvalStatus = _context.TimeSheets
                                .Where(ts => ts.RecordNumber == record)
                                .Select(ts => ts.ApprovalStatus)
                                .FirstOrDefault();


                var dateRange = _context.TimeSheets
                                .Where(ts => ts.RecordNumber == record)
                                .GroupBy(ts => ts.RecordNumber)
                                .Select(g => new
                                {
                                    RecordNumber = g.Key,
                                    MinDate = g.Min(ts => ts.WorkingDate),
                                    MaxDate = g.Max(ts => ts.WorkingDate)
                                })
                                .FirstOrDefault();

                result.Add(new PreviousRecordDTO
                {
                    RecordNumber = record,
                    SubmissionDate = submissionDate,
                    WorkingHours = (short)totalHours,
                    ApprovalStatus = approvalStatus,
                    StartDate = dateRange.MinDate,
                    EndDate = dateRange.MaxDate
                });

            } 

            return Ok(result);
        }
    }
}
