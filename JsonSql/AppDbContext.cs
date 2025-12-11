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

			// Configura la colonna JSON per Details
			entity.OwnsOne(e => e.Details, ownedNavigationBuilder =>
			{
				ownedNavigationBuilder.ToJson();

				ownedNavigationBuilder.OwnsOne(d => d.Dimensions);
			});

			// Configura la colonna JSON per Specifications
			entity.OwnsOne(e => e.Specifications, ownedNavigationBuilder =>
			{
				ownedNavigationBuilder.ToJson();
				ownedNavigationBuilder.OwnsMany(s => s.Features);
			});
		});
	}
}
