using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace VectorSearch;

/// <summary>
/// DbContext per la gestione della ricerca vettoriale con SQL Server 2025
/// </summary>
public class ProductContext : DbContext
{
	public DbSet<Product> Products { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		// Connection string per SQL Server 2025 con supporto vettoriale
		optionsBuilder
			.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=VectorSearchDB;Trusted_Connection=True;MultipleActiveResultSets=true")
			.LogTo(Console.WriteLine, LogLevel.Information)
			.EnableSensitiveDataLogging();
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Product>(entity =>
		{
			entity.HasKey(e => e.Id);

			entity.Property(e => e.Name)
				.IsRequired()
				.HasMaxLength(200);

			entity.Property(e => e.Description)
				.IsRequired()
				.HasMaxLength(1000);

			entity.Property(e => e.Category)
				.IsRequired()
				.HasMaxLength(100);

			entity.Property(e => e.Price)
				.HasPrecision(18, 2);

			// *** EF Core 10 FEATURE: Primitive Collection ***
			// List<float> viene automaticamente memorizzata come JSON in SQL Server
			// Non serve conversione manuale o colonne aggiuntive!
			entity.PrimitiveCollection(e => e.Embedding)
				.IsRequired();

			// EF Core 10: Indici per ottimizzare le query
			entity.HasIndex(e => e.Category);
			entity.HasIndex(e => e.Price);
			entity.HasIndex(e => e.CreatedAt);
		});
	}
}
