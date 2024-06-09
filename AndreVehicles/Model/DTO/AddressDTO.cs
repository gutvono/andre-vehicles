using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DTO
{
    public class AddressDTO
    {
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string Neighborhood { get; set; }
        public string StreetType { get; set; }
        public int Number { get; set; }
        public string Complement { get; set; }
        public string State { get; set; }
        public string City { get; set; }
    }
}
