using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TheLibrary.Data;
using TheLibrary.Models;

namespace TheLibrary.Controllers
{
    public class BooksController : Controller
    {
        private readonly LibraryDbContext _context;

        public BooksController(LibraryDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: Books
        public async Task<IActionResult> Index(string searchString)
        {
            if (_context.Books == null)
            {
                return Problem("Entity set 'LibraryDbContext.Books' is null.");
            }

            var books = from b in _context.Books
                        .Include(b => b.Author)
                        .Include(b => b.Catalog)
                        .Include(b => b.Genre)
                        .Include(b => b.Publisher)
                        .Include(b => b.Shelf)
                        .Include(b => b.Checkoutdetails) // Ensure related data is included
                        select b;

            if (!string.IsNullOrEmpty(searchString))
            {
                books = books.Where(b => b.BookTitle.Contains(searchString));
            }

            // Fetch the books list and include borrowed count calculation
            var bookList = await books.ToListAsync();
            return View(bookList);
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (_context.Books == null)
            {
                return Problem("Entity set 'LibraryDbContext.Books' is null.");
            }

            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Catalog)
                .Include(b => b.Genre)
                .Include(b => b.Publisher)
                .Include(b => b.Shelf)
                .FirstOrDefaultAsync(m => m.BookId == id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            if (_context.Authors == null || _context.Catalogs == null || _context.Genres == null || _context.Publishers == null || _context.Shelves == null)
            {
                return Problem("Some entity sets in 'LibraryDbContext' are null.");
            }

            ViewData["AuthorId"] = new SelectList(_context.Authors, "AuthorId", "AuthorName");
            ViewData["CatalogId"] = new SelectList(_context.Catalogs, "CatalogId", "CatalogName");
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "Genre1");
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "PublisherId", "PublisherName");
            ViewData["ShelfId"] = new SelectList(_context.Shelves, "ShelfId", "ShelfNumber");
            return View();
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookId,BookTitle,PublicationDate,AuthorId,GenreId,PublisherId,CatalogId,ShelfId,Quantity,IsAvailable")] Book book)
        {
            if (_context.Books == null)
            {
                return Problem("Entity set 'LibraryDbContext.Books' is null.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorId"] = new SelectList(_context.Authors, "AuthorId", "AuthorName", book.AuthorId);
            ViewData["CatalogId"] = new SelectList(_context.Catalogs, "CatalogId", "CatalogName", book.CatalogId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "Genre1", book.GenreId);
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "PublisherId", "PublisherName", book.PublisherId);
            ViewData["ShelfId"] = new SelectList(_context.Shelves, "ShelfId", "ShelfNumber", book.ShelfId);
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (_context.Books == null)
            {
                return Problem("Entity set 'LibraryDbContext.Books' is null.");
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["AuthorId"] = new SelectList(_context.Authors, "AuthorId", "AuthorName", book.AuthorId);
            ViewData["CatalogId"] = new SelectList(_context.Catalogs, "CatalogId", "CatalogName", book.CatalogId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "Genre1", book.GenreId);
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "PublisherId", "PublisherName", book.PublisherId);
            ViewData["ShelfId"] = new SelectList(_context.Shelves, "ShelfId", "ShelfNumber", book.ShelfId);
            return View(book);
        }

        // POST: Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookId,BookTitle,PublicationDate,AuthorId,GenreId,PublisherId,CatalogId,ShelfId,Quantity,IsAvailable")] Book book)
        {
            if (_context.Books == null)
            {
                return Problem("Entity set 'LibraryDbContext.Books' is null.");
            }

            if (id != book.BookId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.BookId))
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
            ViewData["AuthorId"] = new SelectList(_context.Authors, "AuthorId", "AuthorName", book.AuthorId);
            ViewData["CatalogId"] = new SelectList(_context.Catalogs, "CatalogId", "CatalogName", book.CatalogId);
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "Genre1", book.GenreId);
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "PublisherId", "PublisherName", book.PublisherId);
            ViewData["ShelfId"] = new SelectList(_context.Shelves, "ShelfId", "ShelfNumber", book.ShelfId);
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (_context.Books == null)
            {
                return Problem("Entity set 'LibraryDbContext.Books' is null.");
            }

            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Catalog)
                .Include(b => b.Genre)
                .Include(b => b.Publisher)
                .Include(b => b.Shelf)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Books == null)
            {
                return Problem("Entity set 'LibraryDbContext.Books' is null.");
            }

            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books?.Any(e => e.BookId == id) ?? false;
        }
    }
}
