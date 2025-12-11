using JsonSql;
using Microsoft.EntityFrameworkCore;

Console.WriteLine("=== Demo Entity Framework Core 10 - Colonne JSON con SQL Server 2025 ===\n");

using var context = new AppDbContext();

// Crea il database e applica le migrazioni
Console.WriteLine("Creazione database...");
await context.Database.EnsureDeletedAsync();
await context.Database.EnsureCreatedAsync();
Console.WriteLine("Database creato!\n");

// ===== INSERIMENTO DATI =====
Console.WriteLine("--- Inserimento prodotti ---");

var product1 = new Product
{
	Name = "Laptop Dell XPS 15",
	Price = 1899.99m,
	Details = new ProductDetails
	{
		Category = "Computer",
		Manufacturer = "Dell",
		Tags = new List<string> { "laptop", "ultrabook", "business" },
		Dimensions = new Dimensions
		{
			Width = 35.7,
			Height = 2.3,
			Depth = 23.5,
			Unit = "cm"
		}
	},
	Specifications = new TechnicalSpecs
	{
		Model = "XPS-15-9520",
		WarrantyMonths = 24,
		Features = new List<Feature>
		{
			new Feature { Key = "CPU", Value = "Intel Core i7-12700H" },
			new Feature { Key = "RAM", Value = "16GB DDR5" },
			new Feature { Key = "Storage", Value = "512GB NVMe SSD" },
			new Feature { Key = "Display", Value = "15.6\" 4K OLED" }
		}
	}
};

var product2 = new Product
{
	Name = "iPhone 15 Pro",
	Price = 1299.00m,
	Details = new ProductDetails
	{
		Category = "Smartphone",
		Manufacturer = "Apple",
		Tags = new List<string> { "smartphone", "5G", "premium" },
		Dimensions = new Dimensions
		{
			Width = 7.09,
			Height = 0.83,
			Depth = 14.67,
			Unit = "cm"
		}
	},
	Specifications = new TechnicalSpecs
	{
		Model = "A2848",
		WarrantyMonths = 12,
		Features = new List<Feature>
		{
			new Feature { Key = "Chip", Value = "A17 Pro" },
			new Feature { Key = "Storage", Value = "256GB" },
			new Feature { Key = "Camera", Value = "48MP Main" },
			new Feature { Key = "Display", Value = "6.1\" Super Retina XDR" }
		}
	}
};

var product3 = new Product
{
	Name = "Scrivania IKEA Bekant",
	Price = 299.00m,
	Details = new ProductDetails
	{
		Category = "Arredamento",
		Manufacturer = "IKEA",
		Tags = new List<string> { "scrivania", "ufficio", "regolabile" },
		Dimensions = new Dimensions
		{
			Width = 160,
			Height = 75,
			Depth = 80,
			Unit = "cm"
		}
	}
	// Specifications è null per questo prodotto
};

context.Products.AddRange(product1, product2, product3);
await context.SaveChangesAsync();
Console.WriteLine($"✓ Inseriti {await context.Products.CountAsync()} prodotti\n");

// ===== QUERY SU COLONNE JSON =====
Console.WriteLine("--- Query su colonne JSON ---\n");

// 1. Filtra per proprietà JSON
Console.WriteLine("1. Prodotti della categoria 'Computer':");
var computers = await context.Products
	.Where(p => p.Details.Category == "Computer")
	.ToListAsync();
foreach (var p in computers)
	Console.WriteLine($"   - {p.Name} ({p.Details.Manufacturer})");

// 2. Filtra per tag nell'array JSON
Console.WriteLine("\n2. Prodotti con tag 'premium':");
var premiumProducts = await context.Products
	.Where(p => p.Details.Tags.Contains("premium"))
	.ToListAsync();
foreach (var p in premiumProducts)
	Console.WriteLine($"   - {p.Name} - €{p.Price}");

// 3. Filtra per oggetto nested
Console.WriteLine("\n3. Prodotti con larghezza > 100 cm:");
var largeProducts = await context.Products
	.Where(p => p.Details.Dimensions != null && p.Details.Dimensions.Width > 100)
	.ToListAsync();
foreach (var p in largeProducts)
	Console.WriteLine($"   - {p.Name} ({p.Details.Dimensions!.Width} x {p.Details.Dimensions.Depth} cm)");

// 4. Proiezione di dati JSON
Console.WriteLine("\n4. Lista produttori:");
var manufacturers = await context.Products
	.Select(p => new { p.Name, p.Details.Manufacturer })
	.ToListAsync();
foreach (var m in manufacturers)
	Console.WriteLine($"   - {m.Name}: {m.Manufacturer}");

// 5. Ordinamento per proprietà JSON
Console.WriteLine("\n5. Prodotti ordinati per categoria:");
var orderedByCategory = await context.Products
	.OrderBy(p => p.Details.Category)
	.ThenBy(p => p.Name)
	.ToListAsync();
foreach (var p in orderedByCategory)
	Console.WriteLine($"   - [{p.Details.Category}] {p.Name}");

// 6. Filtra per proprietà in Specifications (nullable)
Console.WriteLine("\n6. Prodotti con garanzia >= 24 mesi:");
var longWarranty = await context.Products
	.Where(p => p.Specifications != null && p.Specifications.WarrantyMonths >= 24)
	.ToListAsync();
foreach (var p in longWarranty)
	Console.WriteLine($"   - {p.Name} ({p.Specifications!.WarrantyMonths} mesi)");

// ===== AGGIORNAMENTO COLONNE JSON =====
Console.WriteLine("\n--- Aggiornamento colonne JSON (SaveChanges) ---");

var laptop = await context.Products.FirstAsync(p => p.Name.Contains("Laptop"));
laptop.Details.Tags.Add("in-offerta");
laptop.Price = 1699.99m;
if (laptop.Specifications != null)
{
	laptop.Specifications.Features.Add(new Feature { Key = "Promo", Value = "Sconto Black Friday" });
}
await context.SaveChangesAsync();
Console.WriteLine($"✓ Aggiornato {laptop.Name} con nuovo tag e prezzo\n");

// ===== EXECUTEUPDATE - AGGIORNAMENTO BULK =====
Console.WriteLine("--- ExecuteUpdate - Aggiornamento bulk ---");

// 1. Applica uno sconto del 10% a tutti i prodotti Apple
var appleRowsAffected = await context.Products
	.Where(p => p.Details.Manufacturer == "Apple")
	.ExecuteUpdateAsync(setters => setters
		.SetProperty(p => p.Price, p => p.Price * 0.9m));
Console.WriteLine($"✓ Applicato sconto 10% a {appleRowsAffected} prodotti Apple");

// 2. Aggiorna la categoria per tutti i prodotti di arredamento
var furnitureRowsAffected = await context.Products
	.Where(p => p.Details.Category == "Arredamento")
	.ExecuteUpdateAsync(setters => setters
		.SetProperty(p => p.Details.Category, "Mobili & Arredamento"));
Console.WriteLine($"✓ Aggiornata categoria per {furnitureRowsAffected} prodotti di arredamento");

// 3. Aumenta la garanzia di 12 mesi per tutti i prodotti che hanno specifiche
var warrantyRowsAffected = await context.Products
	.Where(p => p.Specifications != null)
	.ExecuteUpdateAsync(setters => setters
		.SetProperty(p => p.Specifications!.WarrantyMonths, p => p.Specifications!.WarrantyMonths + 12));
Console.WriteLine($"✓ Estesa garanzia di 12 mesi per {warrantyRowsAffected} prodotti");

// 4. Aggiorna proprietà nested nelle dimensioni - converti unità da cm a mm
var dimensionsRowsAffected = await context.Products
	.Where(p => p.Details.Dimensions != null && p.Details.Dimensions.Unit == "cm")
	.ExecuteUpdateAsync(setters => setters
		.SetProperty(p => p.Details.Dimensions!.Width, p => p.Details.Dimensions!.Width * 10)
		.SetProperty(p => p.Details.Dimensions!.Height, p => p.Details.Dimensions!.Height * 10)
		.SetProperty(p => p.Details.Dimensions!.Depth, p => p.Details.Dimensions!.Depth * 10)
		.SetProperty(p => p.Details.Dimensions!.Unit, "mm"));
Console.WriteLine($"✓ Convertite dimensioni da cm a mm per {dimensionsRowsAffected} prodotti");

// 5. Aggiorna il manufacturer per tutti i prodotti di una categoria specifica
var manufacturerRowsAffected = await context.Products
	.Where(p => p.Details.Category == "Mobili & Arredamento")
	.ExecuteUpdateAsync(setters => setters
		.SetProperty(p => p.Details.Manufacturer, "IKEA Sweden"));
Console.WriteLine($"✓ Aggiornato manufacturer per {manufacturerRowsAffected} prodotti di arredamento\n");

// ===== VISUALIZZAZIONE FINALE =====
Console.WriteLine("--- Tutti i prodotti (dettaglio completo) ---");
var allProducts = await context.Products.ToListAsync();
foreach (var p in allProducts)
{
	Console.WriteLine($"\n{p.Name} - €{p.Price}");
	Console.WriteLine($"  Categoria: {p.Details.Category}");
	Console.WriteLine($"  Produttore: {p.Details.Manufacturer}");
	Console.WriteLine($"  Tags: {string.Join(", ", p.Details.Tags)}");
	if (p.Details.Dimensions != null)
	{
		Console.WriteLine($"  Dimensioni: {p.Details.Dimensions.Width}x{p.Details.Dimensions.Height}x{p.Details.Dimensions.Depth} {p.Details.Dimensions.Unit}");
	}
	if (p.Specifications != null)
	{
		Console.WriteLine($"  Modello: {p.Specifications.Model}");
		Console.WriteLine($"  Garanzia: {p.Specifications.WarrantyMonths} mesi");
		Console.WriteLine($"  Caratteristiche:");
		foreach (var feature in p.Specifications.Features)
		{
			Console.WriteLine($"    - {feature.Key}: {feature.Value}");
		}
	}
}

Console.WriteLine("\n=== Demo completata ===");

