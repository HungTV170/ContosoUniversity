using System;
using System.Linq.Expressions;
using ContosoUniversity.Data;
using ContosoUniversity.Models;

namespace ContosoUniversity.Repository{
    public class SqlRepositoryService : IRepositoryService
    {
        private readonly SchoolContext _context = null!;
        private IGenericRepository<Course> _courses = null!;
        private IGenericRepository<Enrollment> _enrollments = null!;
        private IGenericRepository<Student> _students = null!;
        private IGenericRepository<Department> _departments = null!;
        private IGenericRepository<Instructor> _instructors = null!;
        private IGenericRepository<OfficeAssignment> _officeAssignments = null!;
        private IGenericRepository<CourseAssignment> _courseAssignments = null!;

        public SqlRepositoryService(SchoolContext context)
        {
            _context = context;
        }

        public IGenericRepository<Course> Courses => _courses ??= new SqlGenericRepository<Course>(_context);

        public IGenericRepository<Enrollment> Enrollments => _enrollments ??= new SqlGenericRepository<Enrollment>(_context);

        public IGenericRepository<Student> Students => _students ??= new SqlGenericRepository<Student>(_context);

        public IGenericRepository<Department> Departments => _departments ??= new SqlGenericRepository<Department>(_context);

        public IGenericRepository<Instructor> Instructors => _instructors ??= new SqlGenericRepository<Instructor>(_context);

        public IGenericRepository<OfficeAssignment> OfficeAssignments => _officeAssignments ??= new SqlGenericRepository<OfficeAssignment>(_context);

        public IGenericRepository<CourseAssignment> CourseAssignments => _courseAssignments ??= new SqlGenericRepository<CourseAssignment>(_context);

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }

}