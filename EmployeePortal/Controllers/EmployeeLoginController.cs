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
    public class EmployeeLoginController : ControllerBase
    {
        private readonly EmpPortalContext _context;
        private readonly ILogger<EmployeeLoginController> _logger;

        public EmployeeLoginController(EmpPortalContext context, ILogger<EmployeeLoginController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployeeLogin(EmployeeLoginDTO employeeLoginDto)
        {
            if (employeeLoginDto == null)
            {
                return BadRequest("Invalid data");
            }

            var employeeLogin = new EmployeeLogin
            {

                EmployeeId = employeeLoginDto.EmployeeId, // Convert EmployeeId to uppercase
                EmployeeEmail = employeeLoginDto.EmployeeEmail,
                WhatOperation = employeeLoginDto.WhatOperation,
                AuthId = employeeLoginDto.AuthId
            };

            _context.EmployeeLogins.Add(employeeLogin);
            await _context.SaveChangesAsync();

            return Ok(employeeLogin);
        }

        [HttpGet("{authId}")]
        public async Task<IActionResult> GetEmployeeLoginByAuthId(string authId)
        {
            try
            {
                var employeeLogin = await _context.EmployeeLogins
                    .Include(el => el.Employee) // Ensure Employee navigation property is included
                    .FirstOrDefaultAsync(el => el.AuthId == authId);

                if (employeeLogin == null)
                {
                    return NotFound();
                }

                var employee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.EmployeeId == employeeLogin.EmployeeId);

                if (employee == null)
                {
                    return NotFound();
                }

                var employeeLoginDTO = new EmployeeLoginDTO
                {
                    EmployeeId = employeeLogin.EmployeeId,
                    EmployeeEmail = employeeLogin.EmployeeEmail,
                    WhatOperation = employeeLogin.WhatOperation,
                    AuthId = employeeLogin.AuthId,
                    EmployeeName = $"{employee.FirstName} {employee.LastName}" // Add EmployeeName to the DTO
                };

                return Ok(employeeLoginDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching EmployeeLogin by AuthId");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}

