using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookManagment.Models.ViewModels
{
    public class BookIndexData
    {
        public IEnumerable<Author> Authors { get; set; }
        public IEnumerable<Book> Books { get; set; }
        public IEnumerable<BookAuthor> BookAuthors { get; set; }
    }
}
