using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.Models.ViewModels {
    public class OfficeAssignmentViewModel{
        public int InstructorID {get; set;}

        [StringLength(40)]
        [Display(Name = "Office Location")]
        public string? Location {get; set;} 

        public InstructorViewModel? Instructor {get; set;} 
    }
}