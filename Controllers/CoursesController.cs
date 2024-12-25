using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using ContosoUniversity.Authorization;
using AutoMapper;
using ContosoUniversity.Models.ViewModels;

namespace ContosoUniversity.Controllers
{
    public class CoursesController : CTUniversity
    {
        private readonly SchoolContext _context;
        private readonly IMapper _mapper;

        public CoursesController(
            SchoolContext context,
            IAuthorizationService authorizationService,
            UserManager<ContosoUser> userManager,
            IMapper mapper
            ) : base(authorizationService,userManager)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: Courses
        public async Task<IActionResult> Index()
        {
            var isAuthorized = User.IsInRole(ContosoResource.ContosoManagersRole) ||
                    User.IsInRole(ContosoResource.ContosoAdministratorsRole);

            var currentUserId = UserManager.GetUserId(User);   

            var schoolContext = _context.Courses.Include(c => c.Department).AsQueryable();
            if (!isAuthorized)
            {
                schoolContext = schoolContext.Where(c => c.Status == ContactStatus.Approved
                                            || c.OwnerID == currentUserId);
            }     

            List<Course> courses = await schoolContext.AsNoTracking().ToListAsync();
            return View(_mapper.Map<IEnumerable<CourseViewModel>>(courses));
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Department)
                .FirstOrDefaultAsync(m => m.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }

            CourseViewModel courseViewModel = _mapper.Map<CourseViewModel>(course);
            return View(courseViewModel);
        }

        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetailsPost(int id,CourseViewModel courseViewModel)
        {
            if (id != courseViewModel.CourseID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var courseToUpdate = await _context.Courses.FirstOrDefaultAsync(s => s.CourseID == id);
                    if (courseToUpdate == null)
                    {
                        return NotFound();
                    }

                    _mapper.Map(courseViewModel, courseToUpdate);
                    AuthorizationResult isAuthorized;

                    if (courseToUpdate.Status == ContactStatus.Approved)
                    {
                        isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                            User, courseToUpdate,
                                                            ResourceOperation.Approve);
                    }
                    else if (courseToUpdate.Status == ContactStatus.Rejected)
                    {
                        isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                           User, courseToUpdate,
                                                           ResourceOperation.Reject);
                    }
                    else
                    {
                        isAuthorized = AuthorizationResult.Failed();
                    }

                    if (!isAuthorized.Succeeded)
                    {
                        return Forbid();
                    }


                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes.");
                }
            }




            PopulateDepartmentsDropDownList(courseViewModel.DepartmentID);
            return View(courseViewModel);
        }
        // GET: Courses/Create
        public IActionResult Create()
        {
            var isAuthorized = User.IsInRole(ContosoResource.ContosoManagersRole) ||
                    User.IsInRole(ContosoResource.ContosoAdministratorsRole);

            if (!isAuthorized)
            {
                return Forbid();
            }
            PopulateDepartmentsDropDownList();
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseViewModel courseViewModel)
        {
            Course course = _mapper.Map<Course>(courseViewModel);
            AuthorizationResult isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                    User, course,
                                                    ResourceOperation.Create);  
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }
            try{
                //ModelState.Remove(nameof(course.Department));
                if (ModelState.IsValid)
                {
                    course.OwnerID = UserManager.GetUserId(User);
                    _context.Add(course);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }catch(DbUpdateException /** ex **/){
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            PopulateDepartmentsDropDownList(course.DepartmentID);
            return View(courseViewModel);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }
            var isAuthorized = User.IsInRole(ContosoResource.ContosoManagersRole) ||
                    User.IsInRole(ContosoResource.ContosoAdministratorsRole);

            if (!isAuthorized)
            {
                return Forbid();
            }            
            CourseViewModel courseViewModel = _mapper.Map<CourseViewModel>(course);
            PopulateDepartmentsDropDownList(course.DepartmentID);
            return View(courseViewModel);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,CourseViewModel courseViewModel)
        {

            if(id != courseViewModel.CourseID){
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    var courseToUpdate = await _context.Courses.FirstOrDefaultAsync(s => s.CourseID == id);
                    if (courseToUpdate == null)
                    {
                        return NotFound();
                    }

                    _mapper.Map(courseViewModel, courseToUpdate);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes.");
                }
            }

            PopulateDepartmentsDropDownList(courseViewModel.DepartmentID);
            return View(courseViewModel);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id ,bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.CourseID == id);
            if (course == null)
            {
                return NotFound();
            }

            if(saveChangesError.GetValueOrDefault()){
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";                
            }

            var isAuthorized = User.IsInRole(ContosoResource.ContosoManagersRole) ||
                    User.IsInRole(ContosoResource.ContosoAdministratorsRole);

            if (!isAuthorized)
            {
                return Forbid();
            }

            CourseViewModel courseViewModel = _mapper.Map<CourseViewModel>(course);
            return View(courseViewModel);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses
                        .AsNoTracking()
                        .FirstOrDefaultAsync(m => m.CourseID == id);

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                    User, course,
                                                    ResourceOperation.Delete);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            try{
                if (course != null)
                {
                    _context.Courses.Remove(course);
                }

                await _context.SaveChangesAsync();
            }catch(DbUpdateException /** ex **/){
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.CourseID == id);
        }

        private void PopulateDepartmentsDropDownList(int? departmentID = null){
            ViewData["DepartmentID"] = new SelectList(_context.Departments.AsNoTracking(), "DepartmentID", "Name", departmentID);
        }
    }
}
