using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace EmployeePortal.DTO
{
    public class AdminVacationDTO
    {
        
        public long Id { get; set; }
        public String? AuthId { get; set; }
        public String? Email { get; set; }
        public String? Name { get; set; }

        // [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        public bool IsComplete { get; set; }
        public String? Body { get; set; }
        public string? Secret { get; set; }
        public DateOnly VacationStartdate { get; set; }
        public DateOnly VacationEnddate { get; set; }

        public bool isManager { get; set; }
        public String? ImageUrl { get; set; }
        public String? fileupload { get; set; }
        public String? Endhours { get; set; }
        public String? Starthours { get; set; }
        public bool agree { get; set; }
        public String? ApprovalStatus { get; set; }       
    }
}


