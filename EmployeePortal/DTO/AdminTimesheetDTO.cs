using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EmployeePortal.DTO
{
    [Route("api/[controller]")]
    [ApiController]

    public class AdminTimesheetDTO
    {
        public Guid? EmployeeId { get; set; }
        //public Guid? Approver { get; set; }
        //public string EmployeeName { get; set; }

        public string EmployeeName { get; set; }
        public string ProjectName { get; set; }
        public string ApprovalStatus { get; set; }
       // public string ClientEmail { get; set; }
         public DateOnly SubmissionDate { get; set; }
        public List<ProjectDateHoursEntry> ProjectDateHours { get; set; } = new List<ProjectDateHoursEntry>();

        // Additional properties or methods can be added here as needed.

    }
}

