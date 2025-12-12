# Vector Search con Entity Framework Core 10

Questa demo mostra come implementare la **ricerca vettoriale semantica** usando **Entity Framework Core 10 Primitive Collections** e **SQL Server LocalDB**.

## Caratteristiche Principali

- **EF Core 10 Primitive Collections**: Storage automatico di `List<float>` come JSON
- **Ricerca Semantica**: Calcolo della similarità coseno per trovare prodotti simili
- **AsNoTracking**: Ottimizzazione delle performance per query read-only
- **LINQ su Collections**: Query avanzate usando Count e altri metodi su collezioni
- **Zero SQL Raw**: Implementazione completamente LINQ-based
- **LocalDB**: Database locale per testing e sviluppo

## Struttura del Progetto

```
VectorSearch/
├── Product.cs          # Modello con embedding vettoriale List<float>
├── ProductContext.cs   # DbContext con configurazione Primitive Collections
├── Program.cs          # Demo completa con esempi di ricerca semantica
└── README.md          # Questa documentazione
```

## Come Funziona

### 1. Modello Product

```csharp
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    
    // EF Core 10 Primitive Collection - memorizzato automaticamente come JSON
    public List<float> Embedding { get; set; } = new();
}
```

Gli **embeddings** sono vettori di 384 dimensioni che rappresentano il significato semantico del prodotto. EF Core 10 li serializza automaticamente in JSON nel database.

### 2. Database Context con Primitive Collections

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Product>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
        entity.Property(e => e.Description).HasMaxLength(1000);
        entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
        
        // Configurazione Primitive Collection per List<float>
        entity.PrimitiveCollection(e => e.Embedding)
              .IsRequired()
              .HasMaxLength(50000);
    });
}
```

La chiamata a `PrimitiveCollection()` è la **nuova feature di EF Core** che permette di memorizzare liste di tipi primitivi (come `List<float>`) direttamente come JSON nel database, senza bisogno di conversioni personalizzate.

### 3. Ricerca Vettoriale con LINQ

La ricerca usa **query LINQ pure** con calcolo in-memory della similarità coseno:

```csharp
// Recupera tutti i prodotti senza tracking (performance)
var products = await context.Products
    .AsNoTracking()
    .ToListAsync();

// Calcola cosine similarity in-memory per ogni prodotto
var results = products
    .Select(p => new
    {
        Product = p,
        Similarity = CalculateCosineSimilarity(queryEmbedding, p.Embedding)
    })
    .OrderByDescending(r => r.Similarity)
    .Take(5)
    .ToList();
```

### 4. Calcolo Cosine Similarity

```csharp
static double CalculateCosineSimilarity(List<float> vector1, List<float> vector2)
{
    if (vector1.Count != vector2.Count)
        throw new ArgumentException("I vettori devono avere la stessa dimensione");

    double dotProduct = 0;
    double magnitude1 = 0;
    double magnitude2 = 0;

    for (int i = 0; i < vector1.Count; i++)
    {
        dotProduct += vector1[i] * vector2[i];
        magnitude1 += vector1[i] * vector1[i];
        magnitude2 += vector2[i] * vector2[i];
    }

    magnitude1 = Math.Sqrt(magnitude1);
    magnitude2 = Math.Sqrt(magnitude2);

    return magnitude1 > 0 && magnitude2 > 0 
        ? dotProduct / (magnitude1 * magnitude2) 
        : 0;
}
```

## Feature EF Core 10 Dimostrate

La demo mostra queste funzionalità di EF Core 10:

1. **Primitive Collections**: Storage automatico di `List<float>` come JSON
2. **AsNoTracking**: Query ottimizzate per scenari read-only
3. **Count su Collections**: LINQ queries che usano `.Count()` su proprietà di tipo collection
4. **Projections**: Select con proiezioni anonime per performance
5. **Query Complesse**: Combinazione di filtri, ordinamenti e limitazioni
6. **Logging Dettagliato**: Informazioni complete sulle query SQL generate

## Prerequisiti

- .NET 10.0 SDK
- SQL Server LocalDB (incluso in Visual Studio)
- Visual Studio 2025 o VS Code

## Esecuzione

1. Apri il progetto in Visual Studio o esegui da terminale:
   ```bash
   dotnet run
   ```

2. Il programma:
   - Crea automaticamente il database LocalDB
   - Inserisce 8 prodotti di esempio con embeddings
   - Esegue 4 ricerche semantiche diverse
   - Mostra i risultati ordinati per similarità

## Output di Esempio

```
=== Inizializzazione Database ===
✓ Database creato e popolato con 8 prodotti

=== Demo Features EF Core 10 ===
✓ Primitive Collections: Tutti i prodotti hanno embeddings (384 dimensioni)
✓ Count su collections: 8 prodotti con embeddings validi
✓ AsNoTracking: Query ottimizzata per read-only

=== Ricerca Vettoriale: "smartphone premium" ===

Risultati (ordinati per similarità):
1. Smartphone Pro Max - €1099.99
   Descrizione: Smartphone di fascia alta
   Similarità: 0.956 ⭐⭐⭐⭐⭐

2. Tablet Ultra - €899.99
   Descrizione: Tablet professionale
   Similarità: 0.823 ⭐⭐⭐⭐
```

## Architettura e Flusso dei Dati

1. **Generazione Embeddings**: I vettori vengono generati con `GenerateMockEmbedding()` (in produzione si userebbe un modello AI)
2. **Storage con Primitive Collections**: EF Core serializza automaticamente `List<float>` in JSON
3. **Query LINQ**: AsNoTracking recupera tutti i prodotti senza tracking
4. **Calcolo In-Memory**: La similarità coseno è calcolata per ogni prodotto
5. **Ranking**: I risultati sono ordinati per similarità decrescente

## Per la Produzione

In uno scenario reale, dovresti:

1. **Usare embeddings reali** da modelli AI:
   - Azure OpenAI (text-embedding-ada-002)
   - Sentence Transformers (all-MiniLM-L6-v2)
   - Modelli Hugging Face

2. **Ottimizzare le performance**:
   - Per dataset grandi, considera database vettoriali dedicati (Qdrant, Pinecone, Weaviate)
   - Implementa caching degli embeddings
   - Usa batch processing per inserimenti

3. **Implementare funzionalità aggiuntive**:
   - Filtri pre-ricerca (categoria, prezzo, ecc.)
   - Ricerca ibrida (keyword + vettoriale)
   - Re-ranking dei risultati

## Perché Primitive Collections?

Le **Primitive Collections** di EF Core 10 offrono:

- ✅ **Semplicità**: Nessuna conversione personalizzata necessaria
- ✅ **Type Safety**: Lavori con `List<float>` nativo in C#
- ✅ **Automatismo**: EF Core gestisce la serializzazione JSON
- ✅ **LINQ Support**: Query native su proprietà collection
- ✅ **Portabilità**: Funziona su qualsiasi database che supporta JSON

## Alternative a Primitive Collections

Se hai bisogno di funzioni vettoriali native del database:

- **SQL Server 2025**: Tipo `vector(384)` con `VECTOR_DISTANCE()`
- **PostgreSQL**: Estensione `pgvector` con operatori `<->`, `<#>`, `<=>`
- **Azure Cosmos DB**: Ricerca vettoriale integrata
- **Dedicated Vector DBs**: Qdrant, Pinecone, Milvus, Weaviate

Tuttavia, per **dataset piccoli-medi** (< 100K record), il calcolo in-memory con Primitive Collections è più che sufficiente e molto più semplice.

## Risorse

- [Entity Framework Core 10 Documentation](https://docs.microsoft.com/ef/core/)
- [Primitive Collections in EF Core](https://learn.microsoft.com/ef/core/what-is-new/ef-core-8.0/whatsnew#primitive-collections)
- [Vector Search Best Practices](https://docs.microsoft.com/azure/search/vector-search-overview)
- [Cosine Similarity Explained](https://en.wikipedia.org/wiki/Cosine_similarity)

## Licenza

Progetto di esempio per scopi educativi - .NET Conf Genova.

   ```bash
   dotnet restore
   ```

2. Esegui il progetto:
   ```bash
   dotnet run
   ```

Il programma:
- Crea il database `VectorSearchDB`
- Inserisce 8 prodotti di esempio con embeddings
- Esegue 4 query di ricerca semantica diverse
- Visualizza i risultati ordinati per similarità

## Esempio di Output

```
=== Vector Search con Entity Framework Core 10 e SQL Server 2025 ===

Creazione del database...
Inserimento dati di esempio...
✓ Inseriti 8 prodotti

--- Ricerca 1: 'dispositivo per scrivere codice' ---
  • Laptop Dell XPS 15
    Categoria: Elettronica
    Prezzo: €1,899.99
    Similarità: 87.50%
    Potente laptop per sviluppatori...
```

## Note sulla Produzione

⚠️ **Importante**: Questo esempio usa embeddings mock generati casualmente per scopi dimostrativi.

In un ambiente di produzione, dovresti:

1. **Usare modelli di embedding reali**:
   - Azure OpenAI (text-embedding-ada-002 o text-embedding-3-small)
   - OpenAI API
   - Sentence Transformers (all-MiniLM-L6-v2)
   - Modelli Hugging Face

2. **Ottimizzare le performance**:
   - Creare indici vettoriali appropriati
   - Usare batch processing per inserimenti
   - Implementare caching degli embeddings

3. **Usare SQL Server 2025 Vector Functions**:
   ```sql
   SELECT TOP 5 
       *, VECTOR_DISTANCE('cosine', Embedding, @query) AS Distance
   FROM Products
   ORDER BY Distance ASC
   ```

## Funzionalità SQL Server 2025

SQL Server 2025 introduce il supporto nativo per:

- **Tipo VECTOR**: Archiviazione ottimizzata di vettori
- **VECTOR_DISTANCE()**: Calcolo della distanza (coseno, euclidea, dot product)
- **Indici vettoriali**: Per query veloci su grandi dataset

## Risorse

- [Entity Framework Core Documentation](https://docs.microsoft.com/ef/core/)
- [SQL Server 2025 Vector Support](https://docs.microsoft.com/sql/relational-databases/vectors/)
- [Vector Search Best Practices](https://docs.microsoft.com/azure/search/vector-search-overview)

## Licenza

Progetto di esempio per scopi educativi.
