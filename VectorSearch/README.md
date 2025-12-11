# Vector Search con Entity Framework Core 10 e SQL Server 2025

Questo progetto dimostra come implementare la ricerca vettoriale (vector search) utilizzando Entity Framework Core 10 e SQL Server 2025.

## Caratteristiche

- **Entity Framework Core 10**: Ultima versione del popolare ORM per .NET
- **SQL Server 2025**: Supporto nativo per il tipo `VECTOR` e funzioni di similarità
- **Vector Search**: Ricerca semantica basata su embeddings vettoriali
- **Similarità Coseno**: Calcolo della similarità tra vettori per ranking dei risultati

## Struttura del Progetto

```
VectorSearch/
├── Program.cs           # Entry point con esempi di ricerca
├── Product.cs           # Modello con supporto vettoriale
├── ProductContext.cs    # DbContext per SQL Server 2025
└── VectorSearch.csproj  # File di progetto con dipendenze
```

## Come Funziona

### 1. Modello Product

Il modello `Product` include una proprietà `Embedding` di tipo `float[]` mappata al tipo nativo `vector(384)` di SQL Server 2025:

```csharp
[Column(TypeName = "vector(384)")]
public float[] Embedding { get; set; }
```

### 2. DbContext

Il `ProductContext` configura la connessione a SQL Server 2025 e definisce il mapping del tipo vettoriale:

```csharp
entity.Property(e => e.Embedding)
    .HasColumnType("vector(384)")
    .IsRequired();
```

### 3. Ricerca Vettoriale

La ricerca vettoriale utilizza la similarità coseno per trovare i prodotti più rilevanti:

```csharp
var similarity = CalculateCosineSimilarity(queryEmbedding, productEmbedding);
```

## Prerequisiti

- .NET 10.0 SDK
- SQL Server 2025 (o LocalDB)
- Visual Studio 2025 o VS Code

## Esecuzione

1. Ripristina i pacchetti NuGet:
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
