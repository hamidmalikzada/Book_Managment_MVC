using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookManagment.Data;
using BookManagment.Models;
using BookManagment.Models.ViewModels;

namespace BookManagment.Controllers
{
    public class BooksController : Controller
    {
        private readonly BookContext _context;

        public BooksController(BookContext context)
        {
            _context = context;
        }

        // GET: Books
        public IActionResult Index(int ? id)
        {
            var viewModel = new BookIndexData();
            viewModel.Books = _context.Books
                .Include(a => a.BookAuthors)
                .ThenInclude(a => a.Author)
                .Include(p => p.Publisher).ToList();
            if (id != null)
            {
                ViewData["AuthorId"] = id.Value;
                Book Book = viewModel.Books.Where(
                    i => i.Id == id.Value).Single();
                viewModel.Authors = Book.BookAuthors.Select(s => s.Author);
            }
            return View(viewModel);
        }

        // GET: Books/Details/5
        public IActionResult Details(int? id)
        {
            var viewModel = new AuthorIndexData();
            viewModel.Books = _context.Books.Where(i => i.Id == id)
                .Include(i => i.BookAuthors)
                .ThenInclude(i => i.Author)
                .ThenInclude(b=>b.Books)
                .ThenInclude(p=>p.Publisher)
                .ToList();

            if (id != null)
            {
                ViewData["AuthorId"] = id.Value;
                Book Book = viewModel.Books.Where(
                    i => i.Id == id.Value).Single();
                viewModel.Authors = Book.BookAuthors.Select(s => s.Author);
                viewModel.Publishers = Book.Publisher.Books.Select(s => s.Publisher);
            }
            return View(viewModel);

        }

        // GET: Books/Create
        public IActionResult Create()
        {
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "PublisherName");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,PublishingDate,Category,PublisherId")] Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "PublisherName", book.PublisherId);
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.Include(b => b.BookAuthors)
                .ThenInclude(a => a.Author)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (book == null)
            {
                return NotFound();
            }
            ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "PublisherName", book.PublisherId);

            PopulateAssignedAuthorData(book);

            return View(book);
        }

        private void PopulateAssignedAuthorData(Book book)
        {
            var allauthors = _context.Authors;
            var authorBooks = new HashSet<int>(book.BookAuthors.Select(b => b.AuthorId));
            var viewModel = new List<BookCheckBoxViewModel>();
            foreach (var author in allauthors)
            {
                viewModel.Add(new BookCheckBoxViewModel
                {
                    Id = author.Id,
                    FullName = author.FullName,
                    Checked = authorBooks.Contains(author.Id)
                });
            }
            ViewData["Authors"] = viewModel;
        }
 



        // POST: Authors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        public async Task<IActionResult> Edit(int? id, string[] selectedAuthors)
        {
            if (id == null)
            {
                return NotFound();
            }


            var bookToUpdate = await _context.Books
                .Include(b => b.BookAuthors)
                .ThenInclude(a => a.Author)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (await TryUpdateModelAsync<Book>(
                bookToUpdate,
                "",
                i => i.Title, i => i.Publisher, i => i.PublishingDate, i => i.PublisherId, i => i.Category, i => i.BookAuthors))
            {
                UpdateAuthorBooks(selectedAuthors, bookToUpdate);
                ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "PublisherName", bookToUpdate.PublisherId);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        private void UpdateAuthorBooks(string[] selectedAuthors, Book bookToUpdate)
        {
            if (bookToUpdate == null)
            {
                bookToUpdate.BookAuthors = new List<BookAuthor>();
                return;
            }

            var selectedAuthorsHS = new HashSet<string>(selectedAuthors);
            var authorBooks = new HashSet<int>
                (bookToUpdate.BookAuthors.Select(c => c.Author.Id));
            foreach (var author in _context.Authors)
            {
                if (selectedAuthorsHS.Contains(author.Id.ToString()))
                {
                    if (!authorBooks.Contains(author.Id))
                    {
                        bookToUpdate.BookAuthors.Add(new BookAuthor { BookId = bookToUpdate.Id, AuthorId = author.Id });
                    }
                }
                else
                {
                    if (authorBooks.Contains(author.Id))
                    {
                        BookAuthor bookToRemove = bookToUpdate.BookAuthors.FirstOrDefault(i => i.AuthorId == author.Id);
                        _context.Remove(bookToRemove);
                    }
                }
            }




        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Publisher)
                .FirstOrDefaultAsync(m => m.Id == id);
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
            var book = await _context.Books.FindAsync(id);
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}
