using System;
using System.Linq.Expressions;
using ContosoUniversity.Models;

namespace ContosoUniversity.Repository{
    public interface IRepositoryService : IDisposable
    {
        IGenericRepository<Course> Courses {get;}
        IGenericRepository<Enrollment> Enrollments {get;}
        IGenericRepository<Student> Students {get;}
        IGenericRepository<Department> Departments {get;}
        IGenericRepository<Instructor> Instructors {get;}
        IGenericRepository<OfficeAssignment> OfficeAssignments {get;}
        IGenericRepository<CourseAssignment> CourseAssignments {get;}      

        Task SaveAsync();  
    }

}