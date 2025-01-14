using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ContosoUniversity.Models;
using ContosoUniversity.Data;
using ContosoUniversity.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ContosoUniversity.Controllers;

[AllowAnonymous]

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly SchoolContext _context;
    public HomeController(ILogger<HomeController> logger,SchoolContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index(){
        return View();
    }
    public async Task<IActionResult> About()
    {
        var data = _context.Students.GroupBy(s => s.EnrollmentDate)
            .Select(group => new EnrollmentDateGroup(){
                EnrollmentDate = group.Key,
                StudentCount = group.Count()
            });
        return View(await data.AsNoTracking().ToListAsync());
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult ChatHub(){
        return View();
    }
}
