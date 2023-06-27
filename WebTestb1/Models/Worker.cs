using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebTestb1.Models
{
    public class Worker
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        [DisplayName("First Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "First name can not be empty"), MinLength(length: 3, ErrorMessage = "First name must be at least 3 symbols"), MaxLength(length: 20, ErrorMessage = "First name must be at most 20 symbols")]
        public string FirstName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Last name can not be empty"), MinLength(length: 3, ErrorMessage = "Last name must be at least 3 symbols"), MaxLength(length: 20, ErrorMessage = "Last name must be at most 20 symbols")]
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email can not be empty")]
        [DisplayName("Email")]
        [EmailAddress]
        public string Email { get; set; }

        [DisplayName("Full Name")]
        public string FullName { get => string.Concat(FirstName, " ", LastName); }

        [DataType(DataType.Date, ErrorMessage = "Birth date can not be empty")]
        [DisplayName("Birth Date")]
        public DateTime BirthDate { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Position can not be empty"), MinLength(length: 3, ErrorMessage = "Position must be at least 3 symbols"), MaxLength(length: 20, ErrorMessage = "Position must be at most 20 symbols")]
        [DisplayName("Position")]
        public string Position { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Hire date can not be empty")]
        [DisplayName("Hire Date")]
        [WorkerDateRange(nameof(BirthDate))]
        public DateTime HireDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Fire Date")]
        public DateTime? FireDate { get; set; }

        [Required]
        [DisplayName("Worker Status")]
        public WorkerStatus WorkerStatus { get; set; }

        [Required]
        [DisplayName("Education Type")]
        public EducationType EducationType { get; set; }

        [Required]
        [DisplayName("User Type")]
        public UserType UserType { get; set; }

        [NotMapped]
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DisplayName("Password")]
        [DataType(DataType.Password)]
        public string Pass { get; set; }

        [NotMapped]
        [Required]
        [DisplayName("Confirm Password")]
        [Compare("Pass", ErrorMessage = "Confirm Password does not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPass { get; set; }

        public bool IsDeleted { get; set; }

        public Worker()
        {

        }
    }

}
