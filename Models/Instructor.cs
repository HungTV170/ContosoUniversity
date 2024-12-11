using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models {
    public class Instructor{
        public int ID {get; set;}

        [Required]
        [StringLength(40)]
        [DisplayName("Last Name")]
        public string LastName {get; set;} = null!;

        [Required]
        [StringLength(40)]
        [Column("FirstName")]
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
        public ICollection<CourseAssignment> CourseAssignments {get; set;} = [];

        public OfficeAssignment OfficeAssignment {get; set;} = null!;
    }
}