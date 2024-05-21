using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EmployeePortal.DTO
{
    public class H1bDTO
    {
   

        public string passportNumber { get; set; }
        
 
        [StringLength(50)]
        public string legalFirstName { get; set; }


        [StringLength(50)]
        public string legalLastName { get; set; }

        public string passportExpiryDate { get; set; }

        //[Required]
        [JsonIgnore]
        public DateOnly PassportExpiryDateOnly
        {
            get
            {
                return DateOnly.Parse(passportExpiryDate);
            }
        }

       
   
        public string email { get; set; }

        public string contactNumber { get; set; }

   
        public string highestEducation { get; set; }

    
        public string institutionCity { get; set; }
  
        public string institutionName { get; set; }

        [Range(1900, 2100)]
        public int graduationYear { get; set; }

        public bool onOPT { get; set; }

        public bool workedInUS { get; set; }

        [Range(0, 100)]
        public int yearsOfExperience { get; set; }

        public string currentEmployer { get; set; }

        public string currentJobTitle { get; set; }

        public string primaryTechnicalSkills { get; set; }

        //public string OtherSkills { get; set; }

        public string referralSource { get; set; }

        public string? registrationId { get; set; }

        public string degreeMajor { get; set; }

        [Url]
        public string linkedInProfile { get; set; }

        public string resume {  get; set; }
    }
}
