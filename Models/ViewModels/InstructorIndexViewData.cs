namespace ContosoUniversity.Models.ViewModels
{
    public class InstructorIndexViewData{
        public IEnumerable<Instructor> Instructors {get; set;} = [];
        
        public IEnumerable<Course> Courses {get; set;} = [];

        public IEnumerable<Enrollment> Enrollments {get; set;} = [];
    }
};
