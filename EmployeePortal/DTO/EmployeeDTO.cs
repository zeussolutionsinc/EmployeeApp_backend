using System.ComponentModel.DataAnnotations;

namespace EmployeePortal.DTO
{
    public class EmployeeDTO
    {
        public Guid EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string HomeAddress { get; set; }
        public string TSFreq { get; set; }
    }
}

