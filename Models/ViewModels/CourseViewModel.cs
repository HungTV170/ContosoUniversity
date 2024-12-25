using ContosoUniversity.Authorization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models.ViewModels
{
    public class CourseViewModel : AuthorizationPropertyProvider
    {
        [Display(Name = "Number")]
        public int CourseID { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string? Title { get; set; } 

        [Range(0, 5)]
        public int Credits { get; set; }

        public int DepartmentID { get; set; }
        
        [Display(Name = "Department")]
        public string? DepartmentName { get; set; } 

    }
}