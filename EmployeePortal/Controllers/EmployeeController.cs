using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using EmployeePortal.DTO;

[Route("api/[controller]")]
[ApiController]
public class EmployeeController : ControllerBase
{
    private readonly EmpPortalContext _context;

    public EmployeeController(EmpPortalContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateEmployee([FromBody] EmployeeDTO employeeDto)
    {
        if (employeeDto == null)
        {
            return BadRequest("Employee data is null.");
        }

        var employee = new Employee
        {
            FirstName = employeeDto.FirstName,
            LastName = employeeDto.LastName,
            Email = employeeDto.Email,
            PhoneNumber = employeeDto.PhoneNumber,
            HomeAddress = employeeDto.HomeAddress,
            Tsfreq = employeeDto.TSFreq, // Ensure this is correctly mapped
            CreatedTime = DateTime.UtcNow,
            UpdatedTime = DateTime.UtcNow,
            WhatOperation = "I" // Assuming this indicates an insert operation
        };

        _context.Employees.Add(employee);

        try
        {
            await _context.SaveChangesAsync();
            return Ok(new { EmployeeId = employee.EmployeeId });
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }

    [HttpGet("{employeeId}/tsfreq")]
    public async Task<IActionResult> GetEmployeeTSFreq(Guid employeeId)
    {
        var employee = await _context.Employees
            .Where(e => e.EmployeeId == employeeId)
            .Select(e => new { TSFreq = e.Tsfreq.Trim() })
            .FirstOrDefaultAsync();

        if (employee == null)
        {
            return NotFound("Employee not found.");
        }

        return Ok(employee);
    }

}
