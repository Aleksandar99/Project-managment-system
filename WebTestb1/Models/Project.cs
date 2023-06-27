using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebTestb1.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Name must be between 5 and 100 characters.")]
        public string Name { get; set; }

        public virtual List<ProjectTask> ProjectTasks { get; set; }

        [Required(ErrorMessage = "From date is required.")]
        public DateTime From { get; set; }

        [Required(ErrorMessage = "To date is required.")]
        [DateRange(nameof(From))]
        public DateTime To { get; set; }

        public bool IsDeleted { get; set; }

        public Project()
        {

        }
    }

}
