using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Model;

namespace AndreVehicles.Data
{
    public class AndreVehiclesContext : DbContext
    {
        public AndreVehiclesContext (DbContextOptions<AndreVehiclesContext> options)
            : base(options)
        {
        }

        public DbSet<Model.Address> Address { get; set; } = default!;
        public DbSet<Model.Car> Car { get; set; } = default!;
        public DbSet<Model.Card> Card { get; set; } = default!;
        public DbSet<Model.CarJob> CarJob { get; set; } = default!;
        public DbSet<Model.Customer> Customer { get; set; } = default!;
        public DbSet<Model.Employee> Employee { get; set; } = default!;
        public DbSet<Model.Job> Job { get; set; } = default!;
        public DbSet<Model.Payment> Payment { get; set; } = default!;
        public DbSet<Model.Pix> Pix { get; set; } = default!;
        public DbSet<Model.PixType> PixType { get; set; } = default!;
        public DbSet<Model.Purchase> Purchase { get; set; } = default!;
        public DbSet<Model.Role> Role { get; set; } = default!;
        public DbSet<Model.Sale> Sale { get; set; } = default!;
        public DbSet<Model.Ticket> Ticket { get; set; } = default!;
    }
}
