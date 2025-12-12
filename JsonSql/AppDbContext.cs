using Microsoft.EntityFrameworkCore;

namespace JsonSql;

public class AppDbContext : DbContext
{
	public DbSet<Product> Products => Set<Product>();

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		// Configura la connessione a SQL Server con livello di compatibilità 170 (SQL Server 2025)
		optionsBuilder.UseSqlServer(
			@"Server=(localdb)\mssqllocaldb;Database=JsonSqlDemo;Trusted_Connection=True;TrustServerCertificate=True",
			sqlOptions => sqlOptions.UseCompatibilityLevel(170));
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Product>(entity =>
		{
			entity.HasKey(e => e.Id);

			entity.Property(e => e.Name)
				.IsRequired()
				.HasMaxLength(200);

			entity.Property(e => e.Price)
				.HasPrecision(18, 2);

			// Configurazione usando Complex Property per supportare ExecuteUpdate
			// I Complex Types in EF Core 10 supportano update bulk con ExecuteUpdateAsync
			entity.ComplexProperty(e => e.Details, builder =>
			{
				builder.IsRequired();
				builder.Property(d => d.Category).HasMaxLength(100);
				builder.Property(d => d.Manufacturer).HasMaxLength(100);

				// Dimensioni come complex property annidato
				builder.ComplexProperty(d => d.Dimensions, dimBuilder =>
				{
					dimBuilder.Property(dim => dim.Unit).HasMaxLength(10);
				});
			});

			// Specifications con ToJson per le collezioni
			entity.OwnsOne(e => e.Specifications, ownedNavigationBuilder =>
			{
				ownedNavigationBuilder.ToJson();
				ownedNavigationBuilder.OwnsMany(s => s.Features);
			});
		});
	}
}
