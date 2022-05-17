using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookManagment.Models
{
    public class Book
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Publishing Date")]
        public DateTime PublishingDate { get; set; }
        [Required]
        public Category Category { get; set; }
        public int PublisherId { get; set; }
        public Publisher Publisher { get; set; }
        public ICollection<Author> Authors { get; set; }
        public ICollection<BookAuthor> BookAuthors { get; set; }
    }
    public enum Category
    {
        Fantasy,
        Fiction,
        Historical,
        Horror,
        Romance,
        Thriller,
    }
}

