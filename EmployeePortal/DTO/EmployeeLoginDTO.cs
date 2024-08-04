using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EmployeePortal.DTO
{
    public class EmployeeLoginDTO
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeEmail { get; set; }
        public string WhatOperation { get; set; }
        public string AuthId { get; set; }
        public string EmployeeName { get; set; }
    }
}
