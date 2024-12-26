using AutoMapper;

namespace ContosoUniversity.Models.ViewModels
{
    public class MapperInitilizer : Profile
    {

        public MapperInitilizer()
        {
            // Course
            CreateMap<Course, CourseViewModel>()
                .ForMember(dest => dest.DepartmentName, act => act.MapFrom(src => src.Department!.Name));

            CreateMap<CourseViewModel, Course>()
                .ForMember(dest => dest.Department, act => act.Ignore())
                .ForMember(dest => dest.CourseAssignments, act => act.Ignore())
                .ForMember(dest => dest.Enrollments, act => act.Ignore());

            // CourseAssignment
            CreateMap<CourseAssignment, CourseAssignmentViewModel>();
            CreateMap<CourseAssignmentViewModel, CourseAssignment>()
                .ForMember(dest => dest.Instructor, act => act.Ignore())
                .ForMember(dest => dest.Course, act => act.Ignore());

            // Department
            CreateMap<Department, DepartmentViewModel>()
                .ForMember(dest => dest.AdministratorFullName, act => act.MapFrom(src => src.Administrator!.FullName));

            CreateMap<DepartmentViewModel, Department>()
                .ForMember(dest => dest.Administrator, act => act.Ignore())
                .ForMember(dest => dest.Courses, act => act.Ignore());
            // Enrollment


            // Instructor
            CreateMap<Instructor, InstructorViewModel>()
                .ForMember(dest => dest.OfficeAssignmentLocation, act => act.MapFrom(src => src.OfficeAssignment!.Location));

            CreateMap<InstructorViewModel, Instructor>()
                .ForMember(dest => dest.OfficeAssignment, act => act.MapFrom(
                    src => src.OfficeAssignmentLocation != null
                        ? new OfficeAssignment { Location = src.OfficeAssignmentLocation }
                        : null));


            // OfficeAssignment

            // Student
            CreateMap<Student, StudentViewModel>()
                .ForMember(dest => dest.Enrollments, act => act.MapFrom(src =>
                        src.Enrollments.Select(e => new EnrollmentViewModel
                        {
                            Grade = e.Grade,
                            CourseTitle = e.Course.Title
                        }).ToList()));
            CreateMap<StudentViewModel, Student>()
                .ForMember(dest => dest.Enrollments, act => act.Ignore());
        }
    }

}