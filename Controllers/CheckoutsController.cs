using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheLibrary.Data;
using TheLibrary.Models;

namespace TheLibrary.Controllers
{
    public class CheckoutsController : Controller
    {
        private readonly LibraryDbContext _context;
        private const int MaxBooksAllowed = 5;

        public CheckoutsController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: Checkouts
        public async Task<IActionResult> Index(string? searchString)
        {
            var query = _context.Checkouts
                .Include(c => c.Borrower)
                .Include(c => c.Librarian)
                .Include(c => c.Notification)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(c => c.Borrower.BorrowerFullname.Contains(searchString));
            }

            var checkouts = await query.ToListAsync();

            return View(checkouts);
        }

        [HttpGet]
        public IActionResult GetBorrowerType(int borrowerId)
        {
            var borrowerType = _context.Borrowers
                                       .Where(b => b.BorrowerId == borrowerId)
                                       .Select(b => b.BorrowerType)
                                       .FirstOrDefault();

            if (borrowerType != null)
            {
                return Json(new { borrowerType });
            }
            else
            {
                return Json(new { borrowerType = "" });
            }
        }

        // GET: Checkouts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var checkout = await _context.Checkouts.Where(c => c.CheckoutId == id)
                                                   .Include(c => c.Checkoutdetails)
                                                   .Include(c => c.Librarian)
                                                   .Include(c => c.Borrower)
                                                   .FirstAsync();

            checkout.Checkoutdetails = await _context.Checkoutdetails.Where(cd => cd.CheckoutId == id)
                                                                     .Include(cd => cd.Book)
                                                                     .ToListAsync();

            if (id == null)
            {
                return NotFound();
            }

            if (checkout == null)
            {
                return NotFound();
            }

            return View(checkout);
        }

        // GET: Checkouts/Create
        public IActionResult Create()
        {
            ViewData["BorrowerId"] = new SelectList(_context.Borrowers, "BorrowerId", "BorrowerFullname");
            var viewModel = new CheckoutViewModel
            {
                AllBooks = _context.Books.Where(b => b.IsAvailable == true).ToList()
            };
            return View(viewModel);
        }

        // POST: Checkouts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CheckoutViewModel viewModel, List<int> SelectedBooks)
        {
            var borrower = await _context.Borrowers.Include(b => b.Checkouts)
                                                   .FirstOrDefaultAsync(b => b.BorrowerId == viewModel.BorrowerId);
            if (borrower == null)
            {
                return NotFound("Borrower not found.");
            }

            if (borrower.Checkouts.Count >= MaxBooksAllowed)
            {
                ModelState.AddModelError(string.Empty, "Borrower has reached the maximum number of borrowed books this week.");
                ViewData["BorrowerId"] = new SelectList(_context.Borrowers, "BorrowerId", "BorrowerFullname", viewModel.BorrowerId);
                viewModel.AllBooks = _context.Books.Where(b => b.IsAvailable == true).ToList();
                return View(viewModel);
            }

            var selectedBooksCount = SelectedBooks.Count;
            var todayCheckoutsCount = await _context.Checkouts
                                                    .Where(c => c.BorrowerId == borrower.BorrowerId && c.CheckoutDate == DateOnly.FromDateTime(DateTime.Today))
                                                    .CountAsync();

            if (todayCheckoutsCount + selectedBooksCount > MaxBooksAllowed)
            {
                ModelState.AddModelError(string.Empty, $"Borrower cannot borrow more than {MaxBooksAllowed} books per day.");
                ViewData["BorrowerId"] = new SelectList(_context.Borrowers, "BorrowerId", "BorrowerFullname", viewModel.BorrowerId);
                viewModel.AllBooks = _context.Books.Where(b => b.IsAvailable == true).ToList();
                return View(viewModel);
            }

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.NotifDescription == "available for pick up");

            if (notification == null)
            {
                ModelState.AddModelError(string.Empty, "Notification 'available for pick up' not found.");
                ViewData["BorrowerId"] = new SelectList(_context.Borrowers, "BorrowerId", "BorrowerFullname", viewModel.BorrowerId);
                viewModel.AllBooks = _context.Books.Where(b => b.IsAvailable == true).ToList();
                return View(viewModel);
            }

            var checkout = new Checkout
            {
                BorrowerId = viewModel.BorrowerId,
                LibrarianId = int.Parse(HttpContext.Session.GetString("LibrarianId")),
                CheckoutDate = DateOnly.FromDateTime(viewModel.CheckoutDate),
                DueDate = DateOnly.FromDateTime(viewModel.DueDate),
                NotifId = notification.NotifId
            };

            _context.Add(checkout);
            await _context.SaveChangesAsync();

            foreach (var id in SelectedBooks)
            {
                var book = await _context.Books.FindAsync(id);
                if (book != null)
                {
                    var checkoutDetails = new Checkoutdetail
                    {
                        CheckoutId = checkout.CheckoutId,
                        Bookid = id,
                        Quantity = 1
                    };
                    _context.Add(checkoutDetails);

                    book.Quantity -= 1; // Decrement the quantity
                    if (book.Quantity <= 0)
                    {
                        book.IsAvailable = false; // Mark as unavailable if quantity is 0 or less
                    }
                    _context.Update(book);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Checkouts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var checkout = await _context.Checkouts.FindAsync(id);
            if (checkout == null)
            {
                return NotFound();
            }
            ViewData["BorrowerId"] = new SelectList(_context.Borrowers, "BorrowerId", "BorrowerFullname", checkout.BorrowerId);
            ViewData["LibrarianId"] = new SelectList(_context.Librarians, "LibrarianId", "LibrarianFullname", checkout.LibrarianId);
            ViewData["NotifId"] = new SelectList(_context.Notifications, "NotifId", "NotifDescription", checkout.NotifId);
            return View(checkout);
        }

        // POST: Checkouts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CheckoutId,BorrowerId,LibrarianId,CheckoutDate,DueDate,NotifId")] Checkout checkout)
        {
            if (id != checkout.CheckoutId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(checkout);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CheckoutExists(checkout.CheckoutId))
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
            ViewData["BorrowerId"] = new SelectList(_context.Borrowers, "BorrowerId", "BorrowerId", checkout.BorrowerId);
            ViewData["LibrarianId"] = new SelectList(_context.Librarians, "LibrarianId", "LibrarianId", checkout.LibrarianId);
            ViewData["NotifId"] = new SelectList(_context.Notifications, "NotifId", "NotifId", checkout.NotifId);
            return View(checkout);
        }

        // GET: Checkouts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var checkout = await _context.Checkouts
                .Include(c => c.Borrower)
                .Include(c => c.Librarian)
                .Include(c => c.Notification)
                .Include(c => c.Checkoutdetails)
                    .ThenInclude(cd => cd.Book) // Include associated books
                .FirstOrDefaultAsync(c => c.CheckoutId == id);

            if (checkout == null)
            {
                return NotFound();
            }

            return View(checkout);
        }


        // POST: Checkouts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var checkout = await _context.Checkouts
                .Include(c => c.Checkoutdetails)
                .FirstOrDefaultAsync(c => c.CheckoutId == id);

            if (checkout != null)
            {
                foreach (var detail in checkout.Checkoutdetails)
                {
                    var book = await _context.Books.FindAsync(detail.Bookid);
                    if (book != null)
                    {
                        book.Quantity += detail.Quantity; // Increment the quantity
                        if (book.Quantity > 0)
                        {
                            book.IsAvailable = true; // Mark as available if quantity is positive
                        }
                        _context.Update(book);
                    }
                }

                _context.Checkoutdetails.RemoveRange(checkout.Checkoutdetails);
                _context.Checkouts.Remove(checkout);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CheckoutExists(int id)
        {
            return _context.Checkouts.Any(e => e.CheckoutId == id);
        }
    }
}
