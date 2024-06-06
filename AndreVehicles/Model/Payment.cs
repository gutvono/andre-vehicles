using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Payment
    {
        public int Id { get; set; }
        public Card Card { get; set; }
        public Ticket Ticket { get; set; }
        public Pix Pix { get; set; }
        public DateTime PaymentDate {  get; set; }
    }
}
