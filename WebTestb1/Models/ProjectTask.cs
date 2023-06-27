using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebTestb1.Models
{
    public class DateRangeAttribute : ValidationAttribute
    {
        private readonly string _fromPropertyName;

        public DateRangeAttribute(string fromPropertyName)
        {
            _fromPropertyName = fromPropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var toProperty = validationContext.ObjectType.GetProperty("To");
            var fromProperty = validationContext.ObjectType.GetProperty(_fromPropertyName);

            if (toProperty == null || fromProperty == null)
            {
                return new ValidationResult($"Unknown property: {"From"} or {"To"}");
            }

            var fromDate = (DateTime)fromProperty.GetValue(validationContext.ObjectInstance);
            var toDate = (DateTime)value;

            if (toDate <= fromDate)
            {
                return new ValidationResult("To date must be greater than From date.");
            }

            return ValidationResult.Success;
        }
    }
    
    public class WorkerDateRangeAttribute : ValidationAttribute
    {
        private readonly string _fromPropertyName;

        public WorkerDateRangeAttribute(string fromPropertyName)
        {
            _fromPropertyName = fromPropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var toProperty = validationContext.ObjectType.GetProperty("HireDate");
            var fromProperty = validationContext.ObjectType.GetProperty(_fromPropertyName);

            if (toProperty == null || fromProperty == null)
            {
                return new ValidationResult($"Unknown property: {"BirthDate"} or {"HireDate"}");
            }

            var fromDate = (DateTime)fromProperty.GetValue(validationContext.ObjectInstance);
            var toDate = (DateTime)value;

            if (toDate <= fromDate)
            {
                return new ValidationResult("Hire date must be greater than Birth date.");
            }

            return ValidationResult.Success;
        }
    }

    public class ProjectTask
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Name must be between 5 and 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "From date is required.")]
        [DataType(DataType.DateTime)]
        public DateTime From { get; set; }

        [Required(ErrorMessage = "To date is required.")]
        [DataType(DataType.DateTime)]
        [DateRange(nameof(From))]
        public DateTime To { get; set; }

        public Project Project { get; set; }

        public string Username { get; set; }

        [Required(ErrorMessage = "Project task state is required.")]
        [Display(Name = "Project Task State")]
        public ProjectTaskState ProjectTaskState { get; set; }

        [NotMapped]
        public SelectList Projects { get; set; }

        [NotMapped]
        [Display(Name = "Project")]
        public int ProjectId { get; set; }

        [NotMapped]
        public SelectList Usernames { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Username is required.")]
        [Display(Name = "User")]
        public string UsernameId { get; set; }

        public bool IsDeleted { get; set; }

        public ProjectTask()
        {

        }
    }
}
