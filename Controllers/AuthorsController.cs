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
    public class AuthorsController : Controller
    {
        private readonly BookContext _context;

        public AuthorsController(BookContext context)
        {
            _context = context;
        }

        // GET: Authors
        public IActionResult Index(int? id, int? bookId)
        {
            var viewModel = new AuthorIndexData();
            viewModel.Authors = _context.Authors
                .Include(i => i.BookAuthors)
                .ThenInclude(i => i.Book).ToList();
            
            if (id != null)
            {
                ViewData["AuthorId"] = id.Value;
                Author Author = viewModel.Authors.Where(
                    i => i.Id == id.Value).Single();
                viewModel.Books = Author.BookAuthors.Select(s => s.Book);
            }
            return View(viewModel);
        }

        // GET: Authors/Details/5
        public IActionResult Details(int? id)
        {
            var viewModel = new AuthorIndexData();
            viewModel.Authors = _context.Authors.Where(i=>i.Id==id)
                .Include(i => i.BookAuthors)
                .ThenInclude(i => i.Book).ToList();

            if (id != null)
            {
                ViewData["AuthorId"] = id.Value;
                Author Author = viewModel.Authors.Where(
                    i => i.Id == id.Value).Single();
                viewModel.Books = Author.BookAuthors.Select(s => s.Book);
            }
            return View(viewModel);

        }

        // GET: Authors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Authors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName")] Author author)
        {
            if (ModelState.IsValid)
            {
                _context.Add(author);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }

        // GET: Authors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var author = await _context.Authors.Include(b => b.BookAuthors)
                .ThenInclude(b => b.Book)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (author == null)
            {
                return NotFound();
            }
            PopulateAssignedBookData(author);
            return View(author);
        }


        private void PopulateAssignedBookData(Author author)
        {
            var allbooks = _context.Books;
            var authorBooks = new HashSet<int>(author.BookAuthors.Select(b => b.BookId));
            var viewModel = new List<CheckBoxViewModel>();
            foreach (var book in allbooks)
            {
                viewModel.Add(new CheckBoxViewModel
                {
                    Id = book.Id,
                    Title = book.Title,
                    Checked = authorBooks.Contains(book.Id)
                });
            }
            ViewData["Books"] = viewModel;
        }



        // POST: Authors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, string[] selectedBooks)
        {
            if (id == null)
            {
                return NotFound();
            }

            var authorToUpdate = await _context.Authors
                .Include(b => b.BookAuthors)
                .ThenInclude(b => b.Book)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (await TryUpdateModelAsync<Author>(
                authorToUpdate,
                "",
                i => i.FirstName, i => i.LastName, i => i.BookAuthors))
            {
                UpdateAuthorBooks(selectedBooks, authorToUpdate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View();
        }


        private void UpdateAuthorBooks(string[] selectedBooks, Author authorToUpdate)
        {
            if (selectedBooks == null)
            {
                authorToUpdate.BookAuthors = new List<BookAuthor>();
                return;
            }

            var selectedBooksHS = new HashSet<string>(selectedBooks);
            var authorBooks = new HashSet<int>
                (authorToUpdate.BookAuthors.Select(c => c.Book.Id));
            foreach (var book in _context.Books)
            {
                if (selectedBooksHS.Contains(book.Id.ToString()))
                {
                    if (!authorBooks.Contains(book.Id))
                    {
                        authorToUpdate.BookAuthors.Add(new BookAuthor { AuthorId = authorToUpdate.Id, BookId = book.Id });
                    }
                }
                else
                {
                    if (authorBooks.Contains(book.Id))
                    {
                        BookAuthor bookToRemove = authorToUpdate.BookAuthors.FirstOrDefault(i => i.BookId == book.Id);
                        _context.Remove(bookToRemove);
                    }
                }
            }
        }




            // GET: Authors/Delete/5
            public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Authors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // POST: Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AuthorExists(int id)
        {
            return _context.Authors.Any(e => e.Id == id);
        }
    }
}
