using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookManagment.Models.ViewModels
{
    public class BookCheckBoxViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public bool Checked { get; set; }
    }
}
