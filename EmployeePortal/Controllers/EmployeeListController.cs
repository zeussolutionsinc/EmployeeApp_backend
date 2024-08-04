using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeePortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeListController : ControllerBase
    {
        private readonly EmpPortalContext _context;

        public EmployeeListController(EmpPortalContext context)
        {
            _context = context;
        }

        [HttpGet("GetApproverXEmployee")]
        public async Task<IActionResult> GetApproverXEmployee()
        {
            try
            {
                var approverXEmployeeData = await _context.Employees
                    .GroupJoin(
                        _context.ApproverXemployees,
                        emp => emp.EmployeeId,
                        rel => rel.EmployeeId,
                        (emp, rel) => new {
                            EmployeeId = emp.EmployeeId,
                            EmployeeName = emp.FirstName + ' ' +  emp.LastName,
                            ApproverId = rel.FirstOrDefault().Approver
                        }
                    )
                    .ToListAsync();

                if (approverXEmployeeData == null || !approverXEmployeeData.Any())
                {
                    return NotFound();
                }

                return Ok(approverXEmployeeData);
            }
            catch (Exception ex)
            {
                 return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetEmployeeAdmin()
        {
            // Fetch all employees
            var employees = await _context.Employees.ToListAsync();

            // Fetch all approvers
            var approvers = await _context.Approvers
      .Join(_context.Employees,
          approver => approver.EmployeeId,
          employee => employee.EmployeeId,
          (approver, employee) => new
          {
              approver.EmployeeId,
              employee.FirstName,
              employee.LastName
          })
      .ToListAsync();

            // Prepare DTO for employees
            var employeeList = employees.Select(e => new
            {
                e.EmployeeId,
                FullName = $"{e.FirstName} {e.LastName}"
            }).ToList();

            // Prepare DTO for approvers
            var approverList = approvers.Select(a => new
            {
                a.EmployeeId,
                ApproverName = $"{a.FirstName} {a.LastName}"
            }).ToList();

            var result = new
            {
                Employees = employeeList,
                Approvers = approverList
            };

            return Ok(result);
        }

[HttpPost("AddApprovers")]
public async Task<IActionResult> AddApprovers([FromBody] List<Guid> employeeIds)
{
    var existingApprovers = await _context.Approvers
        .Where(a => employeeIds.Contains(a.EmployeeId.Value))
        .Select(a => a.EmployeeId.Value)
        .ToListAsync();

    foreach (var employeeId in employeeIds)
    {
        if (!existingApprovers.Contains(employeeId))
        {
            var approver = new Approver
            {
                ApproverId = Guid.NewGuid(),
                EmployeeId = employeeId,
                CreatedTime = DateTime.Now,
                UpdatedTime = DateTime.Now,
                // Set other necessary properties
            };

            _context.Approvers.Add(approver);
        }
    }

    await _context.SaveChangesAsync();
    return Ok(new { Message = "Approvers added successfully" });
}
        //public async Task<IActionResult> AddApprovers([FromBody] List<Guid> employeeIds)
        //{
        //    foreach (var employeeId in employeeIds)
        //    {
        //        var approver = new Approver
        //        {
        //            ApproverId = Guid.NewGuid(),
        //            EmployeeId = employeeId,
        //            CreatedTime = DateTime.Now,
        //            UpdatedTime = DateTime.Now,
        //            // Set other necessary properties
        //        };

        //        _context.Approvers.Add(approver);
        //    }

        //    await _context.SaveChangesAsync();
        //    return Ok(new { Message = "Approvers added successfully" });
        //}

        [HttpPost("SetEmployeeApproverRelation")]
        public async Task<IActionResult> SetEmployeeApproverRelation([FromBody] List<ApproverXemployee> approverXEmployees)
        {
            if (approverXEmployees == null || approverXEmployees.Count == 0)
            {
                return BadRequest("No approver-employee relations provided.");
            }

            foreach (var item in approverXEmployees)
            {
                var existingRelation = await _context.ApproverXemployees
                    .FirstOrDefaultAsync(a => a.EmployeeId == item.EmployeeId);

                if (existingRelation != null)
                {
                    existingRelation.Approver = item.Approver;
                    existingRelation.UpdatedTime = DateTime.Now;
                    // Update other necessary properties if needed
                }
                else
                {
                    var relation = new ApproverXemployee
                    {
                        Axeid = Guid.NewGuid(),
                        Approver = item.Approver,
                        EmployeeId = item.EmployeeId,
                        CreatedTime = DateTime.Now,
                        UpdatedTime = DateTime.Now,
                        // Set other necessary properties
                    };

                    _context.ApproverXemployees.Add(relation);
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { Message = "Employee-Approver relations set successfully" });
        }

    }
}
