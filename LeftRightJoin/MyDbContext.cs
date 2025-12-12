using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LeftRightJoin
{
    public class MyDbContex : DbContext
    {
        public DbSet<Model.Customer> Customers { get; set; }
        public DbSet<Model.Order> Orders { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Usa SQL Server LocalDB invece di InMemory
            optionsBuilder.UseSqlServer(
                @"Server=(localdb)\mssqllocaldb;Database=LeftJoin;Trusted_Connection=True;TrustServerCertificate=True");
        }
    }
}