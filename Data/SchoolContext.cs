using ContosoUniversity.Models;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Data{
    public class SchoolContext : DbContext{
        
        public SchoolContext(DbContextOptions<SchoolContext> options):base(options){

        }

        public DbSet<Course> Courses {get; set;} = null!;
        public DbSet<Enrollment> Enrollments {get; set;} = null!;
        public DbSet<Student> Students {get; set;} = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Course>().ToTable("Course");
            modelBuilder.Entity<Enrollment>().ToTable("Enrollment");
            modelBuilder.Entity<Student>().ToTable("Student");
        }
    }
}