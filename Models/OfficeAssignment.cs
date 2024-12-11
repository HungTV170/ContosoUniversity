using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.Models {
    public class OfficeAssignment{

        [Key]
        public int InstructorID {get; set;}

        [StringLength(40)]
        [Display(Name = "Office Location")]
        public string Location {get; set;} = null!;

        public Instructor Instructor {get; set;} = null!;
    }
}