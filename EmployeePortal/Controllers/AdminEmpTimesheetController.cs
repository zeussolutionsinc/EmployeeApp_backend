using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using EmployeePortal.DTO;

namespace EmployeePortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminEmpTimesheetController : ControllerBase
    {
        private readonly EmpPortalContext _context;
        private readonly ILogger<AdminEmpTimesheetController> _logger;

        public AdminEmpTimesheetController(EmpPortalContext context, ILogger<AdminEmpTimesheetController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Employee
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees()
        {
            var employeeIds = await _context.TimeSheets
                                            .Select(ts => ts.EmployeeId)
                                            .Distinct()
                                            .ToListAsync();

            if (!employeeIds.Any())
            {
                return NotFound("No employees found.");
            }

            var employees = await _context.Employees
                                          .Where(emp => employeeIds.Contains(emp.EmployeeId))
                                          .Select(emp => new
                                          {
                                              emp.EmployeeId,
                                              Name = emp.FirstName + " " + emp.LastName
                                          })
                                          .ToListAsync();

            var employeeList = employees.Select(employee => new AdminEmpTimesheetDTO
            {
                EmployeeId = employee.EmployeeId,
                EmployeeName = employee.Name
            }).ToList();

            return Ok(employeeList);
        }
    }
}
