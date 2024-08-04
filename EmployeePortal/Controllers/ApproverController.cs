using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using EmployeePortal.DTO;
using EmployeePortal.Model;

namespace EmployeePortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApproverController : ControllerBase
    {
        private readonly EmpPortalContext _context;
        private readonly ILogger<ApproverController> _logger;

        public ApproverController(EmpPortalContext context, ILogger<ApproverController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetApproverByEmployeeId(Guid employeeId)
        {
            try
            {
                var approver = await _context.Approvers
                                             .FirstOrDefaultAsync(a => a.EmployeeId == employeeId);
                if (approver == null)
                {
                    return NotFound();
                }

                var approverDTO = new ApproverDTO
                {
                    ApproverId = approver.ApproverId,
                    EmployeeId = (Guid)approver.EmployeeId,

                };

                return Ok(approverDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching approver");
                return StatusCode(500, "Internal server error");
            }
        }



    }
}
