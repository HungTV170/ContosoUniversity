using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using ContosoUniversity.Models.ViewModels;


namespace ContosoUniversity.Controllers
{
    public class InstructorsController : Controller
    {
        private readonly SchoolContext _context;

        public InstructorsController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Instructors
        public async Task<IActionResult> Index(int? id, int? courseId)
        {
            InstructorIndexViewData viewData = new();
            viewData.Instructors = await _context.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.CourseAssignments)
                    .ThenInclude( c => c.Course)
                        .ThenInclude(c => c!.Enrollments )
                            .ThenInclude(c => c.Student)
                .Include(i => i.CourseAssignments)
                    .ThenInclude( c => c.Course)
                        .ThenInclude(c => c!.Department )
                .AsNoTracking()
                .ToListAsync();

            if(id.HasValue){
                ViewData["InstructorID"] = id.Value;
                var instructor = viewData.Instructors.Where(
                    i => i.ID == id.Value
                ).Single();
                viewData.Courses = instructor.CourseAssignments.Select(c => c.Course!);
            }

            if(courseId.HasValue){
                ViewData["CourseID"] = courseId.Value;
                var course = viewData.Courses.Where(c => c.CourseID == courseId.Value)
                    .Single();
                viewData.Enrollments = course.Enrollments;
            }

            return View(viewData);
        }

        // GET: Instructors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .Include(i => i.OfficeAssignment)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // GET: Instructors/Create
        public IActionResult Create()
        {
            var instructor = new Instructor();
            instructor.CourseAssignments = [];
            PopulateAssignedCourseData(instructor);
            return View();
        }

        // POST: Instructors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,LastName,FirstMidName,HireDate,OfficeAssignment")] Instructor instructor, string[] selectedCourses)
        {
            if (selectedCourses != null)
            {
                instructor.CourseAssignments = new List<CourseAssignment>();
                foreach (var course in selectedCourses)
                {
                    var courseToAdd = new CourseAssignment { InstructorID = instructor.ID, CourseID = int.Parse(course) };
                    instructor.CourseAssignments.Add(courseToAdd);
                }
            }
            if (ModelState.IsValid)
            {
                _context.Add(instructor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateAssignedCourseData(instructor);
            return View(instructor);
        }

        // GET: Instructors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                                .Include(s => s.OfficeAssignment)
                                .Include(i => i.CourseAssignments).ThenInclude(i => i.Course)
                                .AsNoTracking()
                                .FirstOrDefaultAsync(s => s.ID == id);
            if (instructor == null)
            {
                return NotFound();
            }
            
            PopulateAssignedCourseData(instructor);
            return View(instructor);
        }

        // POST: Instructors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id,string[] selectedCourses)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var instructorToUpdate = await _context.Instructors
                                        .Include(i => i.OfficeAssignment)
                                        .Include(i => i.CourseAssignments)
                                            .ThenInclude(i => i.Course)
                                        .FirstOrDefaultAsync(i => i.ID == id.Value);
            if (instructorToUpdate == null){
                return NotFound();
            }

            if(await TryUpdateModelAsync<Instructor>(
                instructorToUpdate,
                "",
                i => i.FirstMidName, 
                i => i.LastName, 
                i => i.HireDate, 
                i => i.OfficeAssignment
            )){
                if(string.IsNullOrWhiteSpace(instructorToUpdate.OfficeAssignment?.Location)){
                    instructorToUpdate.OfficeAssignment = null;
                }
                UpdateInstructorCourses(selectedCourses, instructorToUpdate);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
                return RedirectToAction(nameof(Index));
            }

            UpdateInstructorCourses(selectedCourses, instructorToUpdate);
            PopulateAssignedCourseData(instructorToUpdate);
            return View(instructorToUpdate);
        }

        // GET: Instructors/Delete/
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors
                .FirstOrDefaultAsync(m => m.ID == id);
            if (instructor == null)
            {
                return NotFound();
            }

            return View(instructor);
        }

        // POST: Instructors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instructor = await _context.Instructors
                .Include(i => i.CourseAssignments)
                .FirstAsync(i => i.ID == id);
            if (instructor != null)
            {
                _context.Instructors.Remove(instructor);
                var departments = await _context.Departments
                    .Where(d => d.InstructorID == id)
                    .ToListAsync();
                departments.ForEach(d => d.InstructorID = null);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InstructorExists(int id)
        {
            return _context.Instructors.Any(e => e.ID == id);
        }

        private void PopulateAssignedCourseData(Instructor instructor){
            var allCourses = _context.Courses;
            var instructorCourse = new HashSet<int>(instructor.CourseAssignments.Select(c => c.CourseID));
            List<AssignedCourseData> assignedCourseDatas =[];
            
            foreach(var item in allCourses){
                assignedCourseDatas.Add(new(){
                    CourseID = item.CourseID,
                    Title = item.Title,
                    Assigned = instructorCourse.Contains(item.CourseID)
                });
            }

            ViewData["Courses"] = assignedCourseDatas;
        }

        private void UpdateInstructorCourses(string[] selectedCourses,Instructor instructorToUpdate){
            if(!selectedCourses.Any()){
                instructorToUpdate.CourseAssignments = [];
                return;
            }

            var selectedCoursesHS = new HashSet<string>(selectedCourses);
            var instructorCourse = new HashSet<int>(instructorToUpdate.CourseAssignments.Select(c => c.CourseID));


            foreach(var item in  _context.Courses){

                if(selectedCoursesHS.Contains(item.CourseID.ToString())){
                    if (!instructorCourse.Contains(item.CourseID))
                    {
                        instructorToUpdate.CourseAssignments.Add(new CourseAssignment { InstructorID = instructorToUpdate.ID, CourseID = item.CourseID });
                    }
                }
                else {
                    if(instructorCourse.Contains(item.CourseID)){
                        CourseAssignment courseToRemove = instructorToUpdate.CourseAssignments.First(i => i.CourseID == item.CourseID);
                        _context.Remove(courseToRemove);
                    }
                }
            }
        }
    }
}
