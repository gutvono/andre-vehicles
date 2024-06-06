using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Car
    {
        [Key] public string Plate {  get; set; }
        public string Name { get; set; }
        public int ModelYear { get; set; }
        public int FabricationYear { get; set; }
        public string Color { get; set; }
        public bool Sold { get; set; }
    }
}
