using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Ticket
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public DateTime ExpirationDate {  get; set; }
    }
}
