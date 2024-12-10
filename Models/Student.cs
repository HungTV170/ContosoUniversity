using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.Models{
    public class Student{
        public int ID {get; set;}

        public string LastName {get; set;} = null!;

        public string FirstMidName {get; set;} = null!;

        [DataType(DataType.Date)]
        public DateTime EnrollmentDate {get; set;}

        public ICollection<Enrollment> Enrollments {get; set;} = null!;
    }
}