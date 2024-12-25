namespace ContosoUniversity.Models.ViewModels
{
    public class InstructorIndexViewData{
        public IEnumerable<InstructorViewModel> Instructors {get; set;} = [];
        
        public IEnumerable<CourseViewModel> Courses {get; set;} = [];

        public IEnumerable<EnrollmentViewModel> Enrollments {get; set;} = [];
    }
};
