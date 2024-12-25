using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models.ViewModels{
    public class EnrollmentViewModel{
        [DisplayFormat(NullDisplayText = "No grade")]
        public Grade? Grade {get; set;}

        public string? CourseTitle {get; set;} 
    }
}