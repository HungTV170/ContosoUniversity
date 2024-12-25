using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models.ViewModels{
    public class StudentViewModel{
        public int ID {get; set;}

        [StringLength(40)]
        [DisplayName("Last Name")]
        public string LastName {get; set;} = null!;

        [StringLength(40)]
        [DisplayName("First Name")]
        public string FirstMidName {get; set;} = null!;

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",ApplyFormatInEditMode = true)]
        [DisplayName("Enrollment Date")]
        public DateTime EnrollmentDate {get; set;}

        [DisplayName("Full Name")]
        public string FullName {
            get{
                return LastName + ", " + FirstMidName;
            }
        }

        public ICollection<EnrollmentViewModel>? Enrollments {get; set;} 
    }
}