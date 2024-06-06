using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Model;

namespace ApiCarProject.Data
{
    public class ApiCarProjectContext : DbContext
    {
        public ApiCarProjectContext (DbContextOptions<ApiCarProjectContext> options)
            : base(options)
        {
        }

        public DbSet<Model.Car> Car { get; set; } = default!;
    }
}
