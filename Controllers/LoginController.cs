using TheLibrary.Data;
using TheLibrary.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TheLibrary.Controllers
{

    public class LoginController : Controller
    {
        private readonly LibraryDbContext _context;

        public LoginController(LibraryDbContext context)
        {
            _context = context;
        }


        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        // POST: Login

        [HttpPost]
        public ActionResult Index(Librarian obj)
        {
            List<Librarian> liblist = _context.Librarians.ToList();
            foreach (var lib in liblist)
            {
                if(lib.Email == obj.Email && lib.Password == obj.Password)
                {
                    HttpContext.Session.SetString("LibrarianId", lib.LibrarianId.ToString());
                    return RedirectToAction("Index","Home");
                }
            }
            ViewBag.ErrorMessage = "Wrong credentials";
            return View();
        }

    }
}
