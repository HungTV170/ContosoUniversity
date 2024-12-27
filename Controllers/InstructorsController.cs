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
using AutoMapper;
using ContosoUniversity.Repository;


namespace ContosoUniversity.Controllers
{
    public class InstructorsController : Controller
    {
        private readonly IRepositoryService _context;

        private readonly IMapper _mapper;


        public InstructorsController(IRepositoryService context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: Instructors
        public async Task<IActionResult> Index(int? id, int? courseId)
        {
            InstructorIndexViewData viewData = new();

            viewData.Instructors = await _context.Instructors.GetAllAsync(
                null,null,
                new List<string>{ "OfficeAssignment", "CourseAssignments.Course.Enrollments.Student", "CourseAssignments.Course.Department" }
            );

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
            var instructor = await _context.Instructors.GetTAsync(
                m => m.ID == id,
                new List<string>{ "OfficeAssignment"}
            );

            if (instructor == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<InstructorViewModel>(instructor));
        }

        // GET: Instructors/Create
        public async Task< IActionResult> Create()
        {
            var instructor = new Instructor();
            instructor.CourseAssignments = [];
            await PopulateAssignedCourseDataAsync(_mapper.Map<InstructorViewModel>(instructor));
            return View();
        }

        // POST: Instructors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InstructorViewModel instructorViewModel, string[] selectedCourses)
        {
            if (selectedCourses != null)
            {
                instructorViewModel.CourseAssignments = new List<CourseAssignmentViewModel>();
                foreach (var course in selectedCourses)
                {
                    var courseToAdd = new CourseAssignmentViewModel { InstructorID = instructorViewModel.ID, CourseID = int.Parse(course) };
                    instructorViewModel.CourseAssignments.Add(courseToAdd);
                }
            }
            if (ModelState.IsValid)
            {
                await _context.Instructors.AddAsync(_mapper.Map<Instructor>(instructorViewModel));
                await _context.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            await PopulateAssignedCourseDataAsync(instructorViewModel);
            return View(instructorViewModel);
        }

        // GET: Instructors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors.GetTAsync(
                    s => s.ID == id,
                    new List<string> { "OfficeAssignment", "CourseAssignments.Course" }
                );
            if (instructor == null)
            {
                return NotFound();
            }

            await PopulateAssignedCourseDataAsync(_mapper.Map<InstructorViewModel>(instructor));
            return View(_mapper.Map<InstructorViewModel>(instructor));
        }

        // POST: Instructors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int id, string[] selectedCourses, InstructorViewModel instructorViewModel)
        {

            var instructorToUpdate = await _context.Instructors.GetTAsync(
                    i => i.ID == id,
                    new List<string> { "OfficeAssignment", "CourseAssignments.Course" }
                );

            if (instructorToUpdate == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var instructorDetached = instructorToUpdate.ID;
                var OfficeAssignmentId = instructorToUpdate.OfficeAssignment?.InstructorID ?? null;



                if (string.IsNullOrWhiteSpace(instructorViewModel.OfficeAssignmentLocation))
                {
                    if (instructorToUpdate.OfficeAssignment != null)
                    {
                        _context.OfficeAssignments.DeleteEntity(instructorToUpdate.OfficeAssignment);
                    }

                }

                await UpdateInstructorCoursesAsync(selectedCourses, instructorToUpdate);
                _mapper.Map(instructorViewModel, instructorToUpdate);

                instructorToUpdate.CourseAssignments = [];
                try
                {
                    _context.Instructors.UpdateEntity(instructorToUpdate);
                    await _context.SaveAsync();
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
            //UpdateInstructorCourses(selectedCourses, instructorToUpdate);
            await PopulateAssignedCourseDataAsync(_mapper.Map<InstructorViewModel>(instructorToUpdate));
            return View(instructorToUpdate);
        }

        // GET: Instructors/Delete/
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var instructor = await _context.Instructors.GetTAsync(
                m => m.ID == id,
                new List<string>{ "OfficeAssignment"}
            );
            if (instructor == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<InstructorViewModel>(instructor));
        }

        // POST: Instructors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var instructor = await _context.Instructors.GetTAsync(
                m => m.ID == id,
                new List<string>{ "CourseAssignments"}
            );
            if (instructor != null)
            {
                _context.Instructors.DeleteEntity(instructor);
                var departments = await _context.Departments.GetAllAsync(d => d.InstructorID == id);
                foreach (var department in departments)
                {
                    department.InstructorID = null;
                    _context.Departments.Update(department);
                }
            }

            await _context.SaveAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateAssignedCourseDataAsync(InstructorViewModel instructor){
            var allCourses = await _context.Courses.GetAllAsync();
            var instructorCourse = new HashSet<int>(instructor.CourseAssignments!.Select(c => c.CourseID));
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

        private async Task UpdateInstructorCoursesAsync(string[] selectedCourses, Instructor instructorToUpdate)
        {
            //if (!selectedCourses.Any())
            //{
            //    instructorToUpdate.CourseAssignments = [];
            //    return;
            //}

            var selectedCoursesHS = new HashSet<string>(selectedCourses);
            var instructorCourse = new HashSet<int>(instructorToUpdate.CourseAssignments.Select(c => c.CourseID));

            var allCourses =await _context.Courses.GetAllAsync();
            foreach (var item in allCourses)
            {

                if (selectedCoursesHS.Contains(item.CourseID.ToString()))
                {
                    if (!instructorCourse.Contains(item.CourseID))
                    {
                        instructorToUpdate.CourseAssignments.Add(new CourseAssignment { InstructorID = instructorToUpdate.ID, CourseID = item.CourseID });
                        await _context.CourseAssignments.AddAsync(new CourseAssignment { InstructorID = instructorToUpdate.ID, CourseID = item.CourseID });
                        
                    }
                }
                else
                {
                    if (instructorCourse.Contains(item.CourseID))
                    {
                        CourseAssignment courseToRemove = new CourseAssignment { InstructorID = instructorToUpdate.ID, CourseID = item.CourseID };
                        _context.CourseAssignments.DeleteEntity(courseToRemove);
         
                    }
                }
            }
        }
    }
}
