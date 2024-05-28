using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EmployeePortal.DTO
{
    public class ProjectDateHoursEntry
    {
        public DateOnly WorkingDate { get; set; }
        public int Hours { get; set; }
        public string? ProjectId { get; set; } // Consider making this nullable if applicable.
    }

    public class TimeSheetInfoDTO
    {
        public Guid? EmployeeId { get; set; }
        public Guid? Approver { get; set; }
        public string EmployeeName { get; set; }
        public string ProjectName { get; set; }
        public string ClientName { get; set; }
        // public DateOnly SubmissionDate { get; set; }
        public List<ProjectDateHoursEntry> ProjectDateHours { get; set; } = new List<ProjectDateHoursEntry>();
        public string RecordNumber { get; set; }

        // Additional properties or methods can be added here as needed.
    }
}
