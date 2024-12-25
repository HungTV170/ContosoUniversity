using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.Models.ViewModels {
    public class EnrollmentViewModel{
        public int EnrollmentID {get; set;}

        public int CourseID {get; set;}

        public int StudentID {get; set;}

        [DisplayFormat(NullDisplayText = "No grade")]
        public Grade? Grade {get; set;}

        public CourseAssignmentViewModel? Course {get; set;} 

        public StudentViewModel? Student {get; set;} 
    }
}