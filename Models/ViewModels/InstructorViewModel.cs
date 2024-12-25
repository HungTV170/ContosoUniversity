using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models.ViewModels{
    public class InstructorViewModel{

        public int ID {get; set;}

        [StringLength(40)]
        [DisplayName("Last Name")]
        public string LastName {get; set;} = null!;

        [StringLength(40)]
        [DisplayName("Fisrt Name")]
        public string FirstMidName {get; set;} = null!;

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Hire Date")]
        public DateTime HireDate {get; set;}
        [Display(Name = "Full Name")]
        public string FullName
        {
            get { return LastName + ", " + FirstMidName; }
        }


        [StringLength(40)]
        [Display(Name = "Office Location")]
        public string? OfficeAssignmentLocation {get; set;} 

        public ICollection<CourseAssignmentViewModel>? CourseAssignments {get; set;} 
    }
}