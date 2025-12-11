# Demo: Colonne JSON con Entity Framework Core 10 e SQL Server 2025

Questo progetto dimostra l'utilizzo delle colonne JSON in Entity Framework Core 10 con SQL Server 2025, utilizzando il livello di compatibilità 170.

## Requisiti

- .NET 10.0
- SQL Server 2025 (o LocalDB)
- Entity Framework Core 10.0

## Caratteristiche Implementate

### Modelli con Colonne JSON

Il progetto include il modello `Product` con due colonne JSON:

- **Details** - Contiene informazioni dettagliate sul prodotto:
  - Categoria
  - Produttore
  - Tag (array)
  - Dimensioni (oggetto nested)

- **Specifications** - Contiene specifiche tecniche (nullable):
  - Modello
  - Mesi di garanzia
  - Caratteristiche (collezione di oggetti)

### Configurazione EF Core

- Utilizzo di `.ToJson()` per mappare oggetti come colonne JSON
- `.OwnsOne()` per oggetti singoli
- `.OwnsMany()` per collezioni all'interno del JSON
- Configurazione del livello di compatibilità SQL Server 2025 (170)

### Funzionalità Dimostrate

1. **Query su proprietà JSON** - Filtraggio diretto su campi JSON
2. **Query su array JSON** - Ricerca con `.Contains()` negli array
3. **Query su oggetti nested** - Accesso a proprietà annidate
4. **Proiezioni** - Selezione di campi specifici dal JSON
5. **Ordinamento** - Ordinamento per proprietà JSON
6. **Gestione nullable** - Colonne JSON opzionali
7. **Aggiornamenti** - Modifica di dati nelle colonne JSON

## Struttura del Progetto

```
JsonSql/
├── Program.cs          # Applicazione principale con esempi
├── Models.cs           # Modelli di dominio
├── AppDbContext.cs     # Configurazione DbContext
├── JsonSql.csproj      # File di progetto
└── README.md           # Questa documentazione
```

## Come Eseguire

1. Clona il repository
2. Naviga nella cartella del progetto:
   ```bash
   cd JsonSql
   ```
3. Ripristina i pacchetti NuGet:
   ```bash
   dotnet restore
   ```
4. Esegui l'applicazione:
   ```bash
   dotnet run
   ```

## Visualizzare il Database in SSMS

Per connetterti al database creato usando SQL Server Management Studio (SSMS):

**Server name:**
```
(localdb)\mssqllocaldb
```

**Database:**
```
JsonSqlDemo
```

**Autenticazione:** Windows Authentication

Dopo la connessione, puoi eseguire query SQL per visualizzare le colonne JSON:

```sql
-- Visualizza tutti i prodotti
SELECT * FROM Products

-- Visualizza i dati JSON
SELECT 
    Id, 
    Name, 
    Price,
    Details,
    Specifications
FROM Products

-- Query su colonne JSON con JSON_VALUE
SELECT 
    Name,
    JSON_VALUE(Details, '$.Category') AS Category,
    JSON_VALUE(Details, '$.Manufacturer') AS Manufacturer
FROM Products
```

## Esempi di Query

### Filtro per categoria
```csharp
var computers = await context.Products
    .Where(p => p.Details.Category == "Computer")
    .ToListAsync();
```

### Ricerca per tag
```csharp
var premiumProducts = await context.Products
    .Where(p => p.Details.Tags.Contains("premium"))
    .ToListAsync();
```

### Filtro su proprietà nested
```csharp
var largeProducts = await context.Products
    .Where(p => p.Details.Dimensions != null && 
                p.Details.Dimensions.Width > 100)
    .ToListAsync();
```

### Ordinamento
```csharp
var ordered = await context.Products
    .OrderBy(p => p.Details.Category)
    .ThenBy(p => p.Name)
    .ToListAsync();
```

## Vantaggi delle Colonne JSON

- **Flessibilità** - Schema semi-strutturato per dati variabili
- **Performance** - Query efficienti su dati JSON con indici appropriati
- **Semplicità** - Un'unica colonna per dati complessi correlati
- **Type-safe** - Mapping forte tra C# e JSON
- **Query LINQ** - Utilizzo di LINQ per interrogare dati JSON

## Note

- Le colonne JSON sono supportate da SQL Server 2016+, ma SQL Server 2025 offre funzionalità ottimizzate
- Il livello di compatibilità 170 abilita le funzionalità più recenti
- Le collezioni primitive (List<string>) sono supportate direttamente
- Per dizionari complessi, utilizzare collezioni di oggetti con Key/Value

## Riferimenti

- [Entity Framework Core - JSON Columns](https://learn.microsoft.com/ef/core/what-is-new/ef-core-7.0/whatsnew#json-columns)
- [SQL Server 2025 - JSON Support](https://learn.microsoft.com/sql/relational-databases/json/json-data-sql-server)
