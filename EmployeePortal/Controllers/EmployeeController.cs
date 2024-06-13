//using EmployeePortal.DTO;
//using Microsoft.AspNetCore.Mvc;

//namespace EmployeePortal.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class EmployeeController : ControllerBase
//    {
//        private readonly EmpPortalContext _context;

//        public EmployeeController(EmpPortalContext context)
//        {
//            _context = context;
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetEmployees()
//        {
//            return Ok(await _context.Employees.ToListAsync());
//        }

//        // POST method to create a new employee
//        [HttpPost]
//        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeDTO employeeDto)
//        {
//            // Assuming Employee has properties named Fname and Lastname to match the body
//            if (employeeDto == null)
//            {
//                return BadRequest("Employee data is null");
//            }

//            Employee employee = new Employee();

//            employee.FirstName = employeeDto.FirstName;
//            employee.Lastname = employeeDto.LastName;

//            //Once i do AuthorizationAppBuilderExtensions this will get the autors
//            // API of user id and password 
//            //HttpContext.User 

//            employee.GuId = Guid.NewGuid(); // Set GUID
//            employee.CreatedTime = DateTime.UtcNow;
//            employee.UpdatedTime = DateTime.UtcNow;
//            employee.WhatOperation = "I";

//            _context.Employees.Add(employee);
//            await _context.SaveChangesAsync();

//            return Ok(new { Message = "Employee created successfully" });
//        }
//    }
//}
