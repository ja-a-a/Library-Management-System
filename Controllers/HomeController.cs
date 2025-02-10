using TheLibrary.Data;
using TheLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using System.Diagnostics;

namespace TheLibrary.Controllers
{
    public class HomeController : Controller
    {
        private readonly LibraryDbContext _context;

        public HomeController(LibraryDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var numofavailable = _context.Database.SqlQueryRaw<string>("SP_SelectAllAvailableBooks").ToList();
            var numofallbooks = _context.Database.SqlQueryRaw<string>("SP_SelectAllBooks").ToList();
            ViewBag.numofavailable = numofavailable.Count;
            ViewBag.numofallbooks = numofallbooks.Count;
            return View();
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
    }
}
