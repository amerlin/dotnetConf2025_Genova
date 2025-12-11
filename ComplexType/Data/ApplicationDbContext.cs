using Microsoft.EntityFrameworkCore;
using ComplexType.Models;

namespace ComplexType.Data;

public class ApplicationDbContext : DbContext
{
	public DbSet<Customer> Customers { get; set; }

	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		: base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<Customer>(entity =>
		{
			entity.HasKey(c => c.Id);
			entity.Property(c => c.Name).IsRequired().HasMaxLength(200);

			// Configurazione Complex Type per BillingAddress
			// In EF Core 10, i complex types vengono mappati come owned types
			entity.ComplexProperty(c => c.BillingAddress, address =>
			{
				address.Property(a => a.Street).IsRequired().HasMaxLength(200);
				address.Property(a => a.City).IsRequired().HasMaxLength(100);
				address.Property(a => a.PostalCode).IsRequired().HasMaxLength(20);
				address.Property(a => a.Country).IsRequired().HasMaxLength(100);
			});

			// Configurazione Complex Type per ShippingAddress
			entity.ComplexProperty(c => c.ShippingAddress, address =>
			{
				address.Property(a => a.Street).IsRequired().HasMaxLength(200);
				address.Property(a => a.City).IsRequired().HasMaxLength(100);
				address.Property(a => a.PostalCode).IsRequired().HasMaxLength(20);
				address.Property(a => a.Country).IsRequired().HasMaxLength(100);
			});

			// Configurazione Complex Type per ContactInfo
			entity.ComplexProperty(c => c.Contact, contact =>
			{
				contact.Property(ci => ci.Email).IsRequired().HasMaxLength(200);
				contact.Property(ci => ci.Phone).IsRequired().HasMaxLength(50);
				contact.Property(ci => ci.Website).HasMaxLength(200);
			});

			entity.Property(c => c.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
		});
	}
}
