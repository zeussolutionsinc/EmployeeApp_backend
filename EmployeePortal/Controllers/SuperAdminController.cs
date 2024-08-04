using EmployeePortal.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeePortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    

    public class SuperAdminController : ControllerBase
    {
        private readonly EmpPortalContext _context;
        private readonly ILogger<SuperAdminController> _logger;

        public SuperAdminController(EmpPortalContext context, ILogger<SuperAdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("GetSuperAdmins")]
        public async Task<IActionResult> GetSuperAdmins()
        {
            try
            {
                var superAdmins = await _context.Superadmins
                    .Select(sa => sa.EmployeeId)
                    .ToListAsync();

                if (superAdmins == null || !superAdmins.Any())
                {
                    return NotFound();
                }

                return Ok(superAdmins);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching SuperAdmins");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet("authID/{authID}")]
        public async Task<IActionResult> GetSuperAdminByEmployeeId(string? authID)
        {
            if (authID == null)
            {
                return NotFound();
            }
            try
            {
                var employeeLogin = await _context.EmployeeLogins
                    .FirstOrDefaultAsync(el => el.AuthId == authID);

                if (employeeLogin == null)
                {
                    return Ok(false);
                }

                var employeeId = employeeLogin.EmployeeId;
                var superAdmin = await _context.Superadmins
                                               .FirstOrDefaultAsync(sa => sa.EmployeeId == employeeId);

                if (superAdmin == null)
                {
                    return NotFound(); 
                }
                else
                {
                    return Ok(true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching super admin");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("SetSuperAdmin")]
        public async Task<IActionResult> SetSuperAdmin([FromBody] List<Guid> employeeIds)
        {
            if (employeeIds == null || employeeIds.Count == 0)
            {
                return BadRequest("Invalid Employee IDs");
            }

            try
            {
                foreach (var employeeId in employeeIds)
                {
                    var superAdmin = new Superadmin { EmployeeId = employeeId };
                    _context.Superadmins.Add(superAdmin);
                }

                await _context.SaveChangesAsync();

                return Ok("SuperAdmins set successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting SuperAdmins");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
