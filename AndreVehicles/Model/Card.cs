using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Card
    {
        [Key]public string CardNumber { get; set; }
        public string SecurityCode { get; set; }
        public string ExpirationDate { get; set; }
        public string CardName { get; set; }
    }
}
