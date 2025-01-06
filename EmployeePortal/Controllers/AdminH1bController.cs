global using Microsoft.EntityFrameworkCore;
using EmployeePortal.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using System.Collections.Generic;

namespace EmployeePortal.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AdminH1bcontroller : ControllerBase
    {
        private readonly EmpPortalContext _context;
        private readonly ILogger<AdminH1bcontroller> _logger;
        private readonly IConfiguration _configuration;

        public AdminH1bcontroller(EmpPortalContext context, ILogger<AdminH1bcontroller> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet("resume/{blobName}")]
        public async Task<IActionResult> GetResume(string blobName)
        {
            try
            {
                string connectionString = _configuration["AzureBlobStorage:H1B:ConnectionString"];
                string containerName = _configuration["AzureBlobStorage:H1B:ContainerName"];

                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                BlobClient blobClient = containerClient.GetBlobClient(blobName);

                if (await blobClient.ExistsAsync())
                {
                    var stream = await blobClient.OpenReadAsync();
                    var contentType = "application/octet-stream";
                    if (blobName.EndsWith(".pdf"))
                    {
                        contentType = "application/pdf";
                    }
                    else if (blobName.EndsWith(".doc") || blobName.EndsWith(".docx"))
                    {
                        contentType = "application/msword";
                    }
                    // Add other content types as necessary

                    return File(stream, contentType, blobName, true);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving resume from blob storage");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("authid/{authId}")]
        public async Task<IActionResult> GetForm(string authId)
        {
            try
            {
                // Define the authorized admin's authIds here (hardcoded or from a secure source)
                var authorizedAdminAuthIds = new List<string> { "674f9c45948b864a9a0abdc3", "674a7b4df099fd1441a0645e" };

                // Check if the provided authId is one of the authorized admin's authIds
                if (!authorizedAdminAuthIds.Contains(authId))
                {
                    return NotFound("Access denied or employee with the specified authId not found.");
                }

                // Verify the authId exists in the EmployeeLogins
                var isAuthorizedAdmin = await _context.EmployeeLogins
                                                      .AnyAsync(el => el.AuthId == authId);
                if (!isAuthorizedAdmin)
                {
                    return NotFound("Access denied or employee with the specified authId not found.");
                }

                // Query the database for all H1b entries that are "Pending"
                var entries = await _context.H1bentries
                                            .ToListAsync();

                // If no entries exist, inform the client
                if (!entries.Any())
                {
                    return NotFound("No pending H1b entries found.");
                }

                var result = entries.Select(entry => MapToDTO(entry)).ToList();

                // Return the mapped results
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the H1b entries");
                return StatusCode(500, "Internal server error");
            }
        }






        private AdminH1bDTO MapToDTO(H1bentry result)
        {
            return new AdminH1bDTO
            {
                passportNumber = result.PassportNumber,
                legalFirstName = result.LegalFirstName,
                legalLastName = result.LegalLastName,
                passportExpiryDate = (DateOnly)result.PassportExpiryDate,
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
                resume = result.Resume,
                ApprovalStatus = result.ApprovalStatus // Ensure this field is included in your DTO
    };
        }

        [HttpPut("{registrationId}/updateStatus")]
        public async Task<IActionResult> UpdateApprovalStatus(string registrationId, [FromQuery] string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return BadRequest("Status is required");
            }

            var h1bentry = await _context.H1bentries.FirstOrDefaultAsync(entry => entry.RegistrationId == registrationId);
            if (h1bentry == null)
            {
                return NotFound($"Entry with registration ID: {registrationId} not found.");
            }

            h1bentry.ApprovalStatus = status;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Status updated successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the approval status");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpPut("{registrationId}")]
        public async Task<IActionResult> AdminUpdateH1b(string registrationId, [FromBody] H1bDTO h1bDto)
        {
            if (h1bDto == null)
            {
                return BadRequest("Invalid data");
            }

            var h1bentry = await _context.H1bentries.FirstOrDefaultAsync(entry => entry.RegistrationId == registrationId);
            if (h1bentry == null)
            {
                return NotFound($"Entry with registration ID: {registrationId} not found.");
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
            h1bentry.UpdatedTime = DateTime.UtcNow;
            h1bentry.UpdatedUser = h1bDto.email;
            h1bentry.WhatOperation = "U";
            // TODO: Add resume handling logic here

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

            return Ok(new { message = "Updated Successfully!" });
        }
    }
}
