using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TheLibrary.Data;
using TheLibrary.Models;

namespace TheLibrary.Controllers
{
    public class BorrowersController : Controller
    {
        private readonly LibraryDbContext _context;

        public BorrowersController(LibraryDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: Borrowers
        public async Task<IActionResult> Index(string searchString)
        {
            if (_context.Borrowers == null)
            {
                return Problem("Entity set 'LibraryDbContext.Borrowers' is null.");
            }

            var borrowers = _context.Borrowers.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                borrowers = borrowers.Where(b => b.BorrowerFullname.Contains(searchString));
            }

            var borrowerList = await borrowers.ToListAsync();
            return View(borrowerList);
        }

        // GET: Borrowers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (_context.Borrowers == null)
            {
                return Problem("Entity set 'LibraryDbContext.Borrowers' is null.");
            }

            var borrower = await _context.Borrowers
                .FirstOrDefaultAsync(m => m.BorrowerId == id);

            if (borrower == null)
            {
                return NotFound();
            }

            return View(borrower);
        }

        // GET: Borrowers/Create
        public IActionResult Create()
        {
            ViewBag.BorrowerType = GetBorrowerTypes();
            return View();
        }

        // POST: Borrowers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BorrowerFname,BorrowerLname,PhoneNumber,Email,BorrowerType")] Borrower borrower)
        {
            if (_context.Borrowers == null)
            {
                return Problem("Entity set 'LibraryDbContext.Borrowers' is null.");
            }

            if (ModelState.IsValid)
            {
                borrower.BorrowerFullname = $"{borrower.BorrowerFname} {borrower.BorrowerLname}";
                _context.Add(borrower);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.BorrowerType = GetBorrowerTypes();
            return View(borrower);
        }

        // GET: Borrowers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (_context.Borrowers == null)
            {
                return Problem("Entity set 'LibraryDbContext.Borrowers' is null.");
            }

            var borrower = await _context.Borrowers.FindAsync(id);
            if (borrower == null)
            {
                return NotFound();
            }

            ViewBag.BorrowerType = GetBorrowerTypes();
            return View(borrower);
        }

        // POST: Borrowers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BorrowerId,BorrowerFname,BorrowerLname,PhoneNumber,Email,BorrowerType")] Borrower borrower)
        {
            if (_context.Borrowers == null)
            {
                return Problem("Entity set 'LibraryDbContext.Borrowers' is null.");
            }

            if (id != borrower.BorrowerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(borrower);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BorrowerExists(borrower.BorrowerId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.BorrowerType = GetBorrowerTypes();
            return View(borrower);
        }

        // GET: Borrowers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (_context.Borrowers == null)
            {
                return Problem("Entity set 'LibraryDbContext.Borrowers' is null.");
            }

            var borrower = await _context.Borrowers
                .FirstOrDefaultAsync(m => m.BorrowerId == id);

            if (borrower == null)
            {
                return NotFound();
            }

            return View(borrower);
        }

        // POST: Borrowers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Borrowers == null)
            {
                return Problem("Entity set 'LibraryDbContext.Borrowers' is null.");
            }

            var borrower = await _context.Borrowers.FindAsync(id);
            if (borrower != null)
            {
                _context.Borrowers.Remove(borrower);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BorrowerExists(int id)
        {
            return _context.Borrowers?.Any(e => e.BorrowerId == id) ?? false;
        }

        private SelectList GetBorrowerTypes()
        {
            var borrowerTypes = _context.Borrowers
                .Select(b => b.BorrowerType)
                .Distinct()
                .OrderBy(bt => bt)
                .ToList();

            return new SelectList(borrowerTypes, borrowerTypes.FirstOrDefault());
        }
    }
}
