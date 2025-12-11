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
			entity.ToTable("Products");

			entity.HasKey(e => e.Id);

			entity.Property(e => e.Name)
				.IsRequired()
				.HasMaxLength(200);

			entity.Property(e => e.Description)
				.HasMaxLength(1000);

			entity.Property(e => e.Category)
				.HasMaxLength(100);

			entity.Property(e => e.Price)
				.HasColumnType("decimal(18,2)");

			// Configurazione del tipo VECTOR per SQL Server 2025
			// Il vettore ha 384 dimensioni (comune per modelli come all-MiniLM-L6-v2)
			entity.Property(e => e.Embedding)
				.HasColumnType("vector(384)")
				.IsRequired();

			// Indice per ottimizzare le query
			entity.HasIndex(e => e.Category);
			entity.HasIndex(e => e.CreatedAt);
		});

		base.OnModelCreating(modelBuilder);
	}
}
