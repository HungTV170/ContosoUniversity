using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models{
    public class Student{
        public int ID {get; set;}

        [Required]
        [StringLength(40)]
        [DisplayName("Last Name")]
        public string LastName {get; set;} = null!;

        [Required]
        [StringLength(40)]
        [Column("FirstName")]
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

        public ICollection<Enrollment> Enrollments {get; set;} = [];
    }
}