using Microsoft.EntityFrameworkCore;
using VectorSearch;

Console.WriteLine("=== Vector Search con Entity Framework Core 10 e SQL Server 2025 ===\n");

// Inizializza il database
await InitializeDatabaseAsync();

// Esegui esempi di ricerca vettoriale
await PerformVectorSearchAsync();

Console.WriteLine("\n=== Esempio completato ===");


/// <summary>
/// Inizializza il database e inserisce dati di esempio
/// </summary>
async Task InitializeDatabaseAsync()
{
	using var context = new ProductContext();

	Console.WriteLine("Creazione del database...");
	await context.Database.EnsureDeletedAsync();
	await context.Database.EnsureCreatedAsync();

	Console.WriteLine("Inserimento dati di esempio...\n");

	var products = new[]
	{
		new Product
		{
			Name = "Laptop Dell XPS 15",
			Description = "Potente laptop per sviluppatori con processore Intel i9 e 32GB RAM",
			Category = "Elettronica",
			Price = 1899.99m,
			Embedding = GenerateMockEmbedding("laptop computer developer programming high performance")
		},
		new Product
		{
			Name = "iPhone 15 Pro",
			Description = "Smartphone Apple con fotocamera avanzata e chip A17 Pro",
			Category = "Elettronica",
			Price = 1199.99m,
			Embedding = GenerateMockEmbedding("smartphone mobile phone camera apple ios")
		},
		new Product
		{
			Name = "Scrivania Ergonomica",
			Description = "Scrivania regolabile in altezza per lavorare in piedi o seduti",
			Category = "Arredamento",
			Price = 499.99m,
			Embedding = GenerateMockEmbedding("desk furniture office ergonomic adjustable")
		},
		new Product
		{
			Name = "Tastiera Meccanica RGB",
			Description = "Tastiera gaming meccanica con switch Cherry MX e illuminazione RGB",
			Category = "Elettronica",
			Price = 149.99m,
			Embedding = GenerateMockEmbedding("keyboard mechanical gaming rgb switches typing")
		},
		new Product
		{
			Name = "Monitor 4K 27 pollici",
			Description = "Monitor professionale 4K UHD con calibrazione colore accurata",
			Category = "Elettronica",
			Price = 599.99m,
			Embedding = GenerateMockEmbedding("monitor screen display 4k uhd professional color")
		},
		new Product
		{
			Name = "Sedia da Ufficio Herman Miller",
			Description = "Sedia ergonomica di design con supporto lombare regolabile",
			Category = "Arredamento",
			Price = 899.99m,
			Embedding = GenerateMockEmbedding("chair office ergonomic comfort lumbar support")
		},
		new Product
		{
			Name = "AirPods Pro",
			Description = "Auricolari wireless con cancellazione attiva del rumore",
			Category = "Elettronica",
			Price = 279.99m,
			Embedding = GenerateMockEmbedding("headphones earbuds wireless noise cancellation audio")
		},
		new Product
		{
			Name = "Lampada da Scrivania LED",
			Description = "Lampada LED intelligente con controllo temperatura colore",
			Category = "Arredamento",
			Price = 79.99m,
			Embedding = GenerateMockEmbedding("lamp light led desk smart color temperature")
		}
	};

	await context.Products.AddRangeAsync(products);
	await context.SaveChangesAsync();

	Console.WriteLine($"✓ Inseriti {products.Length} prodotti\n");
}

/// <summary>
/// Esegue varie ricerche vettoriali di esempio
/// </summary>
async Task PerformVectorSearchAsync()
{
	using var context = new ProductContext();

	// Esempio 1: Ricerca per "dispositivo per scrivere codice"
	Console.WriteLine("--- Ricerca 1: 'dispositivo per scrivere codice' ---");
	var query1 = GenerateMockEmbedding("device programming coding writing software");
	await SearchAndDisplayAsync(context, query1, 3);

	// Esempio 2: Ricerca per "cuffie senza fili"
	Console.WriteLine("\n--- Ricerca 2: 'cuffie senza fili' ---");
	var query2 = GenerateMockEmbedding("wireless headphones audio music");
	await SearchAndDisplayAsync(context, query2, 3);

	// Esempio 3: Ricerca per "mobili per ufficio"
	Console.WriteLine("\n--- Ricerca 3: 'mobili per ufficio' ---");
	var query3 = GenerateMockEmbedding("office furniture workspace desk chair");
	await SearchAndDisplayAsync(context, query3, 3);

	// Esempio 4: Ricerca per "schermo alta risoluzione"
	Console.WriteLine("\n--- Ricerca 4: 'schermo alta risoluzione' ---");
	var query4 = GenerateMockEmbedding("display high resolution screen monitor quality");
	await SearchAndDisplayAsync(context, query4, 3);
}

/// <summary>
/// Esegue la ricerca vettoriale e visualizza i risultati
/// </summary>
async Task SearchAndDisplayAsync(ProductContext context, float[] queryEmbedding, int topK)
{
	// SQL Server 2025 supporta la funzione VECTOR_DISTANCE per calcolare la similarità
	// Usiamo la distanza coseno (più bassa = più simile)
	var results = await context.Products
		.FromSqlRaw(@"
            SELECT TOP {0} 
                Id, Name, Description, Category, Price, Embedding, CreatedAt,
                VECTOR_DISTANCE('cosine', Embedding, @queryVector) AS Distance
            FROM Products
            ORDER BY Distance ASC",
			topK,
			queryEmbedding)
		.ToListAsync();

	// Nota: in una vera implementazione, dovresti usare EF.Functions o un metodo personalizzato
	// Qui mostriamo il concetto. Per ora, facciamo una query alternativa:

	var allProducts = await context.Products.ToListAsync();
	var scoredResults = allProducts
		.Select(p => new
		{
			Product = p,
			Similarity = CalculateCosineSimilarity(queryEmbedding, p.Embedding)
		})
		.OrderByDescending(x => x.Similarity)
		.Take(topK)
		.ToList();

	foreach (var result in scoredResults)
	{
		Console.WriteLine($"  • {result.Product.Name}");
		Console.WriteLine($"    Categoria: {result.Product.Category}");
		Console.WriteLine($"    Prezzo: €{result.Product.Price:N2}");
		Console.WriteLine($"    Similarità: {result.Similarity:P2}");
		Console.WriteLine($"    {result.Product.Description}");
		Console.WriteLine();
	}
}

/// <summary>
/// Genera un embedding mock per scopi dimostrativi
/// In produzione, useresti un modello di embedding reale (OpenAI, Azure OpenAI, Sentence Transformers, etc.)
/// </summary>
float[] GenerateMockEmbedding(string text)
{
	var random = new Random(text.GetHashCode()); // Seed basato sul testo per consistenza
	var embedding = new float[384];

	for (int i = 0; i < 384; i++)
	{
		embedding[i] = (float)(random.NextDouble() * 2 - 1); // Valori tra -1 e 1
	}

	// Normalizza il vettore
	var magnitude = Math.Sqrt(embedding.Sum(x => x * x));
	for (int i = 0; i < embedding.Length; i++)
	{
		embedding[i] /= (float)magnitude;
	}

	return embedding;
}

/// <summary>
/// Calcola la similarità coseno tra due vettori
/// </summary>
float CalculateCosineSimilarity(float[] vector1, float[] vector2)
{
	if (vector1.Length != vector2.Length)
		throw new ArgumentException("I vettori devono avere la stessa lunghezza");

	float dotProduct = 0;
	float magnitude1 = 0;
	float magnitude2 = 0;

	for (int i = 0; i < vector1.Length; i++)
	{
		dotProduct += vector1[i] * vector2[i];
		magnitude1 += vector1[i] * vector1[i];
		magnitude2 += vector2[i] * vector2[i];
	}

	magnitude1 = MathF.Sqrt(magnitude1);
	magnitude2 = MathF.Sqrt(magnitude2);

	if (magnitude1 == 0 || magnitude2 == 0)
		return 0;

	return dotProduct / (magnitude1 * magnitude2);
}
