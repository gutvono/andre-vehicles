﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Employee : Person
    {
        Role Role { get; set; }
        public decimal ComissionValue { get; set; }
        public decimal Comission { get; set; }
    }
}
