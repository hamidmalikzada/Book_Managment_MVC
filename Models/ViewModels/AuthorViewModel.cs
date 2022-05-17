using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookManagment.Models.ViewModels
{
    public class AuthorViewModel
    {
        public int AuthorId { get; set; }
        public string FullName { get; set; }
        public List<CheckBoxViewModel> Books { get; set; }
    }
}
