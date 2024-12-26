using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using AutoMapper;
using ContosoUniversity.Models.ViewModels;
using ContosoUniversity.Repository;

namespace ContosoUniversity.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly IRepositoryService _context;

        private readonly IMapper _mapper;

        public DepartmentsController(IRepositoryService context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        // GET: Departments
        public async Task<IActionResult> Index()
        {
            // var schoolContext = _context.Departments.Include(d => d.Administrator);
            var schoolContext = await _context.Departments.GetAllAsync(
                null,null,
                new List<string>{"Administrator"});
            return View(_mapper.Map<IEnumerable<DepartmentViewModel>>(schoolContext));
        }

        // GET: Departments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // var department = await _context.Departments
            //     .Include(d => d.Administrator)
            //     .FirstOrDefaultAsync(m => m.DepartmentID == id);
            var department = await _context.Departments.GetTAsync(
                d => d.DepartmentID == id,
                new List<string>{"Administrator"});
            if (department == null)
            {
                return NotFound();
            }

            return View(_mapper.Map<DepartmentViewModel>(department));
        }

        // GET: Departments/Create
        public async Task<IActionResult> Create()
        {
            var source =await _context.Instructors.GetAllAsync();
            ViewData["InstructorID"] = new SelectList(source, "ID", "FullName");
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DepartmentViewModel departmentViewModel)
        {
            if (ModelState.IsValid)
            {
                await _context.Departments.AddAsync(_mapper.Map<Department>(departmentViewModel));
                await _context.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            var source =await _context.Instructors.GetAllAsync();
            ViewData["InstructorID"] = new SelectList(source, "ID", "FullName", departmentViewModel.InstructorID);
            return View(departmentViewModel);
        }

        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // var department = await _context.Departments
            //     .Include(d => d.Administrator)
            //     .AsNoTracking()
            //     .FirstOrDefaultAsync(d => d.DepartmentID == id);
            var department = await _context.Departments.GetTAsync(
                d => d.DepartmentID == id,
                new List<string>{"Administrator"});

            if (department == null)
            {
                return NotFound();
            }

            var source =await _context.Instructors.GetAllAsync();
            ViewData["InstructorID"] = new SelectList(source, "ID", "FullName", department.InstructorID);
            return View(_mapper.Map<DepartmentViewModel>(department));
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, byte[] rowVersion, DepartmentViewModel departmentViewModel)
        {
            // var departmentToUpdate = await _context.Departments.Include(i => i.Administrator).FirstOrDefaultAsync(m => m.DepartmentID == id);
            var departmentToUpdate = await _context.Departments.GetTAsync(
                d => d.DepartmentID == id,
                new List<string>{"Administrator"});

            if (departmentToUpdate == null)
            {
                DepartmentViewModel deletedDepartmentViewModel = new DepartmentViewModel();
                ModelState.AddModelError(string.Empty,
                    "Unable to save changes. The department was deleted by another user.");

                var source =await _context.Instructors.GetAllAsync();                
                ViewData["InstructorID"] = new SelectList(source, "ID", "FullName", deletedDepartmentViewModel.InstructorID);
                return View(deletedDepartmentViewModel);
            }

            //_context.Entry(departmentToUpdate).Property("RowVersion").OriginalValue = rowVersion;
            _context.Departments.UpdateRowVersion(departmentToUpdate, "RowVersion", rowVersion);
            if(ModelState.IsValid){
                _mapper.Map(departmentViewModel, departmentToUpdate);
                try{
                    await _context.SaveAsync();
                    return RedirectToAction(nameof(Index));
                }catch(DbUpdateConcurrencyException ex){
                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (Department)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty,
                            "Unable to save changes. The department was deleted by another user.");
                    }
                    else
                    {
                        var databaseValues = (Department)databaseEntry.ToObject();

                        if (databaseValues.Name != clientValues.Name)
                        {
                            ModelState.AddModelError("Name", $"Current value: {databaseValues.Name}");
                        }
                        if (databaseValues.Budget != clientValues.Budget)
                        {
                            ModelState.AddModelError("Budget", $"Current value: {databaseValues.Budget:c}");
                        }
                        if (databaseValues.StartDate != clientValues.StartDate)
                        {
                            ModelState.AddModelError("StartDate", $"Current value: {databaseValues.StartDate:d}");
                        }
                        if (databaseValues.InstructorID != clientValues.InstructorID)
                        {
                            //Instructor? databaseInstructor = await _context.Instructors.FirstOrDefaultAsync(i => i.ID == databaseValues.InstructorID);
                            Instructor? databaseInstructor = await _context.Instructors.GetTAsync(
                                i => i.ID == databaseValues.InstructorID);
                            ModelState.AddModelError("InstructorID", $"Current value: {databaseInstructor?.FullName}");
                        }

                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                                + "was modified by another user after you got the original value. The "
                                + "edit operation was canceled and the current values in the database "
                                + "have been displayed. If you still want to edit this record, click "
                                + "the Save button again. Otherwise click the Back to List hyperlink.");
                        departmentToUpdate.RowVersion = (byte[]?)databaseValues.RowVersion;
                        ModelState.Remove("RowVersion");
                    }
                }
            }
            var item =await _context.Instructors.GetAllAsync();  
            ViewData["InstructorID"] = new SelectList(item, "ID", "FullName", departmentToUpdate.InstructorID);
            return View(_mapper.Map<DepartmentViewModel>(departmentToUpdate));
        }

        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(int? id,  bool? concurrencyError)
        {
            if (id == null)
            {
                return NotFound();
            }

            // var department = await _context.Departments
            //     .Include(d => d.Administrator)
            //     .AsNoTracking()
            //     .FirstOrDefaultAsync(m => m.DepartmentID == id);
            var department = await _context.Departments.GetTAsync(
                d => d.DepartmentID == id,
                new List<string>{"Administrator"});
            if (department == null)
            {
                return NotFound();
            }
            if (concurrencyError.GetValueOrDefault())
            {
                ViewData["ConcurrencyErrorMessage"] = "The record you attempted to delete "
                    + "was modified by another user after you got the original values. "
                    + "The delete operation was canceled and the current values in the "
                    + "database have been displayed. If you still want to delete this "
                    + "record, click the Delete button again. Otherwise "
                    + "click the Back to List hyperlink.";
            }
            return View(_mapper.Map<DepartmentViewModel>(department));
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(DepartmentViewModel departmentViewModel)
        {
            //var departmentToDelete = await _context.Departments.FirstAsync(d => d.DepartmentID==departmentViewModel.DepartmentID);

            var departmentToDelete = await _context.Departments.GetTAsync(
                d => d.DepartmentID == departmentViewModel.DepartmentID,
                new List<string>{"Administrator"});
            if (departmentToDelete == null){
                return RedirectToAction(nameof(Index));
            }
            if (!departmentToDelete.RowVersion!.SequenceEqual(departmentViewModel.RowVersion!))
            {
                return RedirectToAction(nameof(Delete), new { concurrencyError = true, id = departmentViewModel.DepartmentID });
            }
            try{
                if (departmentViewModel != null)
                {
                    _context.Departments.DeleteEntity(departmentToDelete);
                }


                await _context.SaveAsync();
                return RedirectToAction(nameof(Index));
            }catch(DbUpdateException /** ex **/){
                //Log the error (uncomment ex variable name and write a log.)
                return RedirectToAction(nameof(Delete), new { concurrencyError = true, id = departmentViewModel.DepartmentID });
            }

        }

    }
}
