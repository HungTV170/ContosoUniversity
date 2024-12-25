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
            CreateMap<CourseAssignment, CourseAssignmentViewModel>()
                .ForMember(dest => dest.Instructor, act => act.MapFrom(src => src.Instructor))
                .ForMember(dest => dest.Course, act => act.MapFrom(src => src.Course))
                .ReverseMap();
            // Department
            CreateMap<Department, DepartmentViewModel>()
                .ForMember(dest => dest.AdministratorFullName, act => act.MapFrom(src => src.Administrator!.FullName));
            CreateMap<DepartmentViewModel, Department>()
                .ForMember(dest => dest.Administrator, act => act.Ignore())
                .ForMember(dest => dest.Courses, act => act.Ignore());
            // Enrollment
            CreateMap<Enrollment, EnrollmentViewModel>()
                .ForMember(dest => dest.Course, act => act.MapFrom(src => src.Course))
                .ForMember(dest => dest.Student, act => act.MapFrom(src => src.Student))
                .ReverseMap();
            // Instructor
            CreateMap<Instructor, InstructorViewModel>()
                .ForMember(dest => dest.CourseAssignments, act => act.MapFrom(src => src.CourseAssignments))
                .ForMember(dest => dest.OfficeAssignment, act => act.MapFrom(src => src.OfficeAssignment))
                .ReverseMap();
            // OfficeAssignment
            CreateMap<OfficeAssignment, OfficeAssignmentViewModel>()
                .ForMember(dest => dest.Instructor, act => act.MapFrom(src => src.Instructor))
                .ReverseMap();
            // Student
            CreateMap<Student, StudentViewModel>()
                 .ForMember(dest => dest.Enrollments, act => act.MapFrom(src => src.Enrollments))
                .ReverseMap();
        }
    }

}