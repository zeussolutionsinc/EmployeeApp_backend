using System.ComponentModel.DataAnnotations;

namespace EmployeePortal.DTO
{
    public class ApproverDTO
    {
        public Guid ApproverId { get; set; }
        public Guid EmployeeId { get; set; }
        public string WhatOperation { get; set; }
        public Guid CreatedUser { get; set; }
        public DateTime CreatedTime { get; set; }
        public Guid UpdatedUser { get; set; }
        public DateTime UpdatedTime { get; set; }
    }
}
