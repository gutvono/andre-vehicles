using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class CarJob
    {
        public int? Id { get; set; }
        public Car Car { get; set; }
        public Job Job { get; set; }
        public bool Status { get; set; }
    }
}
