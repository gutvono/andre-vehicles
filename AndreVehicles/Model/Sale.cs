using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Sale
    {
        public int Id { get; set; }
        public Car Car { get; set; }
        public DateTime SaleDate {  get; set; }
        public Decimal SaleValue {  get; set; }
        public Customer Customer { get; set; }
        public Employee Employee { get; set; }
        public Payment Payment { get; set; }
    }
}
