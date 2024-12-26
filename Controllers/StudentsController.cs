using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using ContosoUniversity.Views;
using AutoMapper;
using ContosoUniversity.Models.ViewModels;
using ContosoUniversity.Repository;
using System.Linq.Expressions;

namespace ContosoUniversity.Controllers
{
    public class StudentsController : Controller
    {
        private readonly IRepositoryService _context;

        private readonly IMapper _mapper;

        public StudentsController(IRepositoryService context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: Students
        public async Task<IActionResult> Index(string sortOrder, string searchString, int? pageNumber, string currentFilter)
        {
            // var students = _context.Students.Select(s => s);

            Expression<Func<Student, bool>>? filter = null;
            Func<IQueryable<Student>, IOrderedQueryable<Student>>? orderBy;
            List<string> includeProperties = new List<string>();

            ViewData["NameSortParam"] = string.IsNullOrEmpty(sortOrder) ? "nameDesc" : "";
            ViewData["DateSortParam"] =  sortOrder =="date" ? "dateDesc" : "date";
            ViewData["CurrentSort"] = sortOrder;

            if(searchString != null){
                pageNumber = 1;
                currentFilter = searchString;
            }else{
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = currentFilter;

            if(!string.IsNullOrEmpty(searchString)){
                // students = students.Where( s => s.LastName.Contains(searchString) 
                //     || s.FirstMidName.Contains(searchString));
                filter = s => s.LastName.Contains(searchString) || s.FirstMidName.Contains(searchString);
            }



            switch(sortOrder){
                case "nameDesc":
                    // students = students.OrderByDescending(s => s.LastName);
                    orderBy = s => s.OrderByDescending(s => s.LastName);
                    break;
                case "dateDesc":
                    //students = students.OrderByDescending(s => s.EnrollmentDate);
                    orderBy = s => s.OrderByDescending(s => s.EnrollmentDate);
                    break;
                case "date":
                    //students = students.OrderBy(s => s.EnrollmentDate);
                    orderBy = s => s.OrderBy(s => s.EnrollmentDate);
                    break;
                default:
                    //students = students.OrderBy(s => s.LastName);
                    orderBy = s => s.OrderBy(s => s.LastName);
                    break;

            }

            int pageSize = 4;

            var students = await _context.Students.GetAllAsync(filter, orderBy, includeProperties);
            var studentsDTO = _mapper.Map<IEnumerable<StudentViewModel>>(students);
            //return View(await PaginatedList<Student>.CreateAsync(students.AsNoTracking(),pageSize, pageNumber ?? 1));
            return View(PaginatedList<StudentViewModel>.Create(studentsDTO.AsQueryable(),pageSize, pageNumber ?? 1));
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // var student = await _context.Students
            //     .Include(s => s.Enrollments)
            //         .ThenInclude(e => e.Course)
            //     .AsNoTracking()
            //     .FirstOrDefaultAsync(m => m.ID == id);

            var student = await _context.Students.GetTAsync(
                s => s.ID == id,
                new List<string>{ "Enrollments.Course" }
            );

            if (student == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<StudentViewModel>(student));
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentViewModel studentViewModel)
        {
            try{
                if (ModelState.IsValid)
                {

                    //_context.Add(_mapper.Map<Student>(studentViewModel));
                    await _context.Students.AddAsync(_mapper.Map<Student>(studentViewModel));
                    await _context.SaveAsync();
                    return RedirectToAction(nameof(Index));
                }
            }catch(DbUpdateException /** ex **/){
                //Log the error (uncomment ex variable name and write a log.
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            return View(studentViewModel);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var student = await _context.Students.FindAsync(id);
            var student = await _context.Students.GetTAsync(s => s.ID == id);
            if (student == null)
            {
                return NotFound();
            }
            return View(_mapper.Map<StudentViewModel>(student));
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int id, StudentViewModel studentViewModel)
        {
            if (id != studentViewModel.ID)
            {
                return NotFound();
            }
            if(ModelState.IsValid){
                try
                {
                    var studentToUpdate = await _context.Students.GetTAsync(s => s.ID == id);
                    if (studentToUpdate == null)
                    {
                        return NotFound(); 
                    }         

                    _mapper.Map(studentViewModel, studentToUpdate);
                    _context.Students.Update(studentToUpdate);
                    await _context.SaveAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException /** ex **/)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            return View(studentViewModel);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var student = await _context.Students
            //    .AsNoTracking()
            //    .FirstOrDefaultAsync(m => m.ID == id);

            var student = await _context.Students.GetTAsync(s => s.ID == id);
            if (student == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "see your system administrator.";
            }
            return View(_mapper.Map<StudentViewModel>(student));
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //var student = await _context.Students.FindAsync(id);
            var student = await _context.Students.GetTAsync(s => s.ID == id);
            try{
                if (student != null)
                {
                    _context.Students.DeleteEntity(student);
                }

                await _context.SaveAsync();
            }catch(DbUpdateException /** ex **/){
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
