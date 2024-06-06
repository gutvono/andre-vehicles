using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public abstract class Person
    {
        public string Document {  get; set; }
        public string Name { get; set; }
        public string BirthDate { get; set; }
        public Address Address {  get; set; }
        public string Email { get; set; }
    }
}
