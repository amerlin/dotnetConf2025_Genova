using Microsoft.EntityFrameworkCore;
using QueryFilter.Models;

namespace QueryFilter
{
    public class DemoDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public DemoDbContext(DbContextOptions<DemoDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. Filtro Nominato: "ActiveOnly"
            // Applica: P.IsActive == true
            modelBuilder.Entity<Product>()
                .HasQueryFilter("ActiveOnly", p => p.IsActive);

            // 2. Filtro Nominato: "PublicOnly"
            // Applica: P.IsPrivate == false
            modelBuilder.Entity<Product>()
                .HasQueryFilter("PublicOnly", p => !p.IsPrivate);

            // Dati di inizializzazione per il database in-memory
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Laptop Base", IsActive = true, IsPrivate = false, Price = 1200.00m },
                new Product { Id = 2, Name = "Smartphone Deluxe", IsActive = true, IsPrivate = false, Price = 800.00m },
                new Product { Id = 3, Name = "Vecchio Monitor", IsActive = false, IsPrivate = false, Price = 150.00m }, // NON ATTIVO
                new Product { Id = 4, Name = "Prodotto Riservato", IsActive = true, IsPrivate = true, Price = 999.99m }   // PRIVATO
            );
        }

    }
}