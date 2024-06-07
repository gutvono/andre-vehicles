using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Customer : Person
    {
        public string PdfDocument { get; set; }
        public decimal Income { get; set; }
    }
}
