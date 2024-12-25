using ContosoUniversity.Authorization;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models.ViewModels
{
    public class CourseAssignmentViewModel 
    {
        public int InstructorID { get; set; }
        public int CourseID { get; set; }

    }
}