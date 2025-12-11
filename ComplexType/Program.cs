using Microsoft.EntityFrameworkCore;
using ComplexType.Data;
using ComplexType.Models;

Console.WriteLine("=== Demo Complex Types in EF Core 10 con SQL Server 2025 ===\n");

// Configurazione del DbContext
var connectionString = "Server=(localdb)\\mssqllocaldb;Database=ComplexTypeDemo;Trusted_Connection=True;TrustServerCertificate=True;";

var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
optionsBuilder.UseSqlServer(connectionString);

using (var context = new ApplicationDbContext(optionsBuilder.Options))
{
	// Creazione database se non esiste
	Console.WriteLine("Creazione database...");
	await context.Database.EnsureCreatedAsync();
	Console.WriteLine("Database creato!\n");

	// Pulizia dati precedenti
	context.Customers.RemoveRange(context.Customers);
	await context.SaveChangesAsync();

	// Creazione di un nuovo Customer con Complex Types
	Console.WriteLine("Inserimento dati...");
	var customer = new Customer
	{
		Name = "Mario Rossi",
		BillingAddress = new Address
		{
			Street = "Via Roma 123",
			City = "Genova",
			PostalCode = "16100",
			Country = "Italia"
		},
		ShippingAddress = new Address
		{
			Street = "Corso Italia 456",
			City = "Milano",
			PostalCode = "20100",
			Country = "Italia"
		},
		Contact = new ContactInfo
		{
			Email = "mario.rossi@example.com",
			Phone = "+39 010 123456",
			Website = "https://www.mariorossi.it"
		},
		CreatedAt = DateTime.UtcNow
	};

	context.Customers.Add(customer);
	await context.SaveChangesAsync();
	Console.WriteLine($"Cliente inserito con ID: {customer.Id}\n");

	// Lettura e visualizzazione dei dati
	Console.WriteLine("Lettura dati dal database...");
	var customers = await context.Customers.ToListAsync();

	foreach (var c in customers)
	{
		Console.WriteLine($"Cliente: {c.Name} (ID: {c.Id})");
		Console.WriteLine($"  Indirizzo Fatturazione:");
		Console.WriteLine($"    {c.BillingAddress.Street}");
		Console.WriteLine($"    {c.BillingAddress.PostalCode} {c.BillingAddress.City}");
		Console.WriteLine($"    {c.BillingAddress.Country}");
		Console.WriteLine($"  Indirizzo Spedizione:");
		Console.WriteLine($"    {c.ShippingAddress.Street}");
		Console.WriteLine($"    {c.ShippingAddress.PostalCode} {c.ShippingAddress.City}");
		Console.WriteLine($"    {c.ShippingAddress.Country}");
		Console.WriteLine($"  Contatti:");
		Console.WriteLine($"    Email: {c.Contact.Email}");
		Console.WriteLine($"    Phone: {c.Contact.Phone}");
		Console.WriteLine($"    Website: {c.Contact.Website}");
		Console.WriteLine($"  Creato: {c.CreatedAt:dd/MM/yyyy HH:mm:ss}\n");
	}

	// Query con filtro sui Complex Types
	Console.WriteLine("\nQuery: Clienti di Genova...");
	var genovaCustomers = await context.Customers
		.Where(c => c.BillingAddress.City == "Genova")
		.ToListAsync();

	Console.WriteLine($"Trovati {genovaCustomers.Count} clienti");
	foreach (var c in genovaCustomers)
	{
		Console.WriteLine($"  - {c.Name} ({c.BillingAddress.City})");
	}

	// Update di un Complex Type
	Console.WriteLine("\nUpdate indirizzo di spedizione...");
	var customerToUpdate = await context.Customers.FirstAsync();
	customerToUpdate.ShippingAddress = new Address
	{
		Street = "Piazza San Marco 1",
		City = "Venezia",
		PostalCode = "30100",
		Country = "Italia"
	};
	await context.SaveChangesAsync();
	Console.WriteLine("Indirizzo aggiornato!");
}

Console.WriteLine("\n=== Demo completata ===");
