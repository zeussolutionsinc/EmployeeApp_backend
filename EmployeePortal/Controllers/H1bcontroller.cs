global using Microsoft.EntityFrameworkCore;
using EmployeePortal.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Text;



namespace EmployeePortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class H1bcontroller : ControllerBase
    {
        private readonly EmpPortalContext _context;
        private readonly ILogger<H1bcontroller> _logger; // Declare the logger

        // Correct the constructor to include ILogger<H1bcontroller> parameter
        public H1bcontroller(EmpPortalContext context, ILogger<H1bcontroller> logger)
        {
            _context = context;
            _logger = logger; // Initialize the logger correctly
        }

        [HttpGet("authid/{authid}")]
        public async Task<IActionResult> GetForm(string authid)
        {
            var result = await _context.H1bentries.FirstOrDefaultAsync(entry => entry.AuthId == authid);
            if (result == null)
            {
                return NoContent();
            }
            var dto = MapToDTO(result);

            return Ok(dto);
        }


        // Helper method for option #1: Query based on registration number
        //private async Task<IActionResult> GetFormByRegistrationNumber(string registrationNumber)
        //{
        //    // Attempt to retrieve the entry from the database based on registration number
        //    var result = await _context.H1bentries.FirstOrDefaultAsync(entry => entry.RegistrationId == registrationNumber);

        //    // Check if the result is null
        //    if (result == null)
        //    {
        //        return NotFound("No matching record found!");
        //    }

        //    // Map the result to a DTO
        //    var dto = MapToDTO(result);

        //    return Ok(dto);
        //}


        //// Helper method for option #2: Query based on email and passport number
        //private async Task<IActionResult> GetFormByEmailAndPassport(string emailId, string passportNumber)
        //{
        //    // Attempt to retrieve the entry from the database based on email and passport number
        //    var result = await _context.H1bentries.FirstOrDefaultAsync(entry => entry.Email == emailId && entry.PassportNumber == passportNumber);

        //    // Check if the result is null
        //    if (result == null)
        //    {
        //        return NotFound("No matching record found!");
        //    }

        //    // Map the result to a DTO
        //    var dto = MapToDTO(result);

        //    return Ok(dto);
        //}





        // Map H1bentry entity to H1bDTO
        private H1bDTO MapToDTO(H1bentry result)
        {
            return new H1bDTO
            {
                passportNumber = result.PassportNumber,
                legalFirstName = result.LegalFirstName,
                legalLastName = result.LegalLastName,
                passportExpiryDate = result.PassportExpiryDate != null ? result.PassportExpiryDate.Value.ToString("yyyy-MM-dd") : null,
                email = result.Email,
                contactNumber = result.ContactNumber,
                highestEducation = result.HighestCollegeDegree,
                institutionCity = result.CollegeCity,
                institutionName = result.CollegeName,
                graduationYear = result.GraduationYear.HasValue ? result.GraduationYear.Value : 0,
                onOPT = result.Opt.HasValue ? result.Opt.Value : false,
                workedInUS = result.WorkedInUsa.HasValue ? result.WorkedInUsa.Value : false,
                yearsOfExperience = result.YearsOfExperience.HasValue ? result.YearsOfExperience.Value : 0,
                currentEmployer = result.CurrentEmployer,
                currentJobTitle = result.CurrentJobTitle,
                primaryTechnicalSkills = result.TechnicalSkills,
                referralSource = result.ReferralSource,
                linkedInProfile = result.LinkedInUrl,
                registrationId = result.RegistrationId,
                degreeMajor = result.DegreeMajor,
                // either i cant reupload it, or i cant update the form  without  re-uploading it 
                resume = result.Resume
            };
        }




        static string GenerateRandomAlphaNumericId()
        {
            Random rand = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"; // Characters to choose from
            StringBuilder sb = new StringBuilder(5);

            // Generate a random character from the 'chars' string and append it to the StringBuilder 'length' times
            for (int i = 0; i < 5; i++)
            {
                sb.Append(chars[rand.Next(chars.Length)]);
            }



            return sb.ToString();

        }


        [HttpPost("authid/{authid}")]
        public async Task<IActionResult> CreateH1b(string authid, [FromBody] H1bDTO h1bDto)
        {
            if (h1bDto == null)
            {
                return BadRequest("H1B Form is null");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the passport number already exists in the database
            var existingEntry = await _context.H1bentries
                                .FirstOrDefaultAsync(entry => entry.PassportNumber == h1bDto.passportNumber);
            if (existingEntry != null)
            {
                return BadRequest("An entry with the same passport number already exists.");
            }

            H1bentry h1bentry = new H1bentry
            {
                PassportNumber = h1bDto.passportNumber,
                LegalFirstName = h1bDto.legalFirstName,
                LegalLastName = h1bDto.legalLastName,
                PassportExpiryDate = DateOnly.TryParse(h1bDto.passportExpiryDate, out DateOnly date)
                                     ? date
                                     : throw new FormatException("Invalid date format for passport expiry date"),
                Email = h1bDto.email,
                ContactNumber = h1bDto.contactNumber,
                HighestCollegeDegree = h1bDto.highestEducation,
                DegreeMajor = h1bDto.degreeMajor,
                CollegeCity = h1bDto.institutionCity,
                CollegeName = h1bDto.institutionName,
                GraduationYear = h1bDto.graduationYear,
                Opt = h1bDto.onOPT,
                WorkedInUsa = h1bDto.workedInUS,
                YearsOfExperience = h1bDto.yearsOfExperience,
                CurrentEmployer = h1bDto.currentEmployer,
                CurrentJobTitle = h1bDto.currentJobTitle,
                TechnicalSkills = h1bDto.primaryTechnicalSkills,
                ReferralSource = h1bDto.referralSource,
                LinkedInUrl = h1bDto.linkedInProfile,
                CreatedTime = DateTime.UtcNow,
                UpdatedTime = DateTime.UtcNow,
                WhatOperation = "I",
                CreatedUser = h1bDto.email,
                Resume = h1bDto.resume,
                AuthId = authid,
                RegistrationId = GenerateRandomAlphaNumericId()
            };

            try
            {
                _context.H1bentries.Add(h1bentry);
                await _context.SaveChangesAsync();
                return Ok(new { Message = $"Submission successful, your registration ID is {h1bentry.RegistrationId}" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving H1B entry: {ex.Message}");
                return StatusCode(500, "Internal server error, unable to save the form");
            }
        }



        [HttpPut("{registrationId}")]
        public async Task<IActionResult> UpdateH1b(string registrationId, [FromBody] H1bDTO h1bDto)
        {
            if (h1bDto == null)
            {
                return BadRequest("Invalid data");
            }

            var h1bentry = await _context.H1bentries.FirstOrDefaultAsync(entry => entry.RegistrationId == registrationId);
            if (h1bentry == null)
            {
                return NotFound($"Entry with passport number :  {registrationId} not found.");
            }

            // Map updated fields from DTO to the existing entity
            h1bentry.PassportNumber = h1bDto.passportNumber;
            h1bentry.LegalFirstName = h1bDto.legalFirstName;
            h1bentry.LegalLastName = h1bDto.legalLastName;
            h1bentry.Email = h1bDto.email;
            h1bentry.ContactNumber = h1bDto.contactNumber;
            h1bentry.HighestCollegeDegree = h1bDto.highestEducation;
            h1bentry.DegreeMajor = h1bDto.degreeMajor;
            h1bentry.CollegeCity = h1bDto.institutionCity;
            h1bentry.CollegeName = h1bDto.institutionName;
            h1bentry.GraduationYear = h1bDto.graduationYear;
            h1bentry.Opt = h1bDto.onOPT;
            h1bentry.WorkedInUsa = h1bDto.workedInUS;
            h1bentry.YearsOfExperience = h1bDto.yearsOfExperience;
            h1bentry.CurrentEmployer = h1bDto.currentEmployer;
            h1bentry.CurrentJobTitle = h1bDto.currentJobTitle;
            h1bentry.TechnicalSkills = h1bDto.primaryTechnicalSkills;
            h1bentry.ReferralSource = h1bDto.referralSource;
            h1bentry.LinkedInUrl = h1bDto.linkedInProfile;
            h1bentry.UpdatedTime = DateTime.UtcNow;  // Update the time of last modification
            h1bentry.UpdatedUser = h1bDto.email;
            h1bentry.WhatOperation = "U";
            // TODO : Add resume here 

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.H1bentries.Any(e => e.RegistrationId == registrationId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { message = "Updated Successfully!" });  // Return an HTTP 204 (No Content) indicating successful update
        }

    }
}

