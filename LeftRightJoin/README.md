# Left Join e Right Join in EF Core 10

Questo progetto dimostra l'utilizzo delle join in Entity Framework Core 10, confrontando la sintassi classica con i nuovi metodi `.LeftJoin()` e `.RightJoin()`.

## 📋 Prerequisiti

- .NET 10
- Entity Framework Core 10.0.1
- Entity Framework Core InMemory 10.0.1

## 🗂️ Struttura del Progetto

```
LeftRightJoin/
├── Model/
│   ├── Customer.cs     # Entità Cliente
│   └── Order.cs        # Entità Ordine
├── MyDbContext.cs      # DbContext con database in-memory
└── Program.cs          # Esempi di join
```

## 🔧 Modelli

### Customer
```csharp
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

### Order
```csharp
public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
}
```

## 🎯 Esempi di Join

### 1. LEFT JOIN - Sintassi Classica (LINQ)

```csharp
var leftJoin = from c in context.Customers
               join o in context.Orders on c.Id equals o.CustomerId into orders
               from o in orders.DefaultIfEmpty()
               select new { c.Name, OrderAmount = (o != null ? o.Amount : 0) };
```

**Risultato:**
- Mostra tutti i clienti
- Include anche i clienti senza ordini (con Amount = 0)

### 2. RIGHT JOIN - Sintassi Classica (Simulato)

```csharp
var rightJoin = from o in context.Orders
                join c in context.Customers on o.CustomerId equals c.Id into customers
                from c in customers.DefaultIfEmpty()
                select new { CustomerName = (c != null ? c.Name : "N/A"), o.Amount };
```

**Risultato:**
- Mostra tutti gli ordini
- Include il nome del cliente per ogni ordine

### 3. LEFT JOIN - Nuova Sintassi EF Core 10 ✨

```csharp
var leftJoinNew = context.Customers
    .LeftJoin(context.Orders, 
              c => c.Id, 
              o => o.CustomerId, 
              (c, o) => new { c.Name, OrderAmount = (o != null ? o.Amount : 0) });
```

**Vantaggi:**
- ✅ Sintassi più pulita e leggibile
- ✅ Metodo fluente
- ✅ Più intuitivo per gli sviluppatori

### 4. RIGHT JOIN - Nuova Sintassi EF Core 10

```csharp
// Nota: RightJoin potrebbe non essere supportato da tutti i provider
// Con InMemory, si può simulare invertendo la logica
var rightJoinNew = context.Customers
    .LeftJoin(context.Orders, c => c.Id, o => o.CustomerId, (c, o) => new { c.Name, OrderAmount = (o != null ? o.Amount : 0) })
    .Where(x => x.OrderAmount > 0);
```

## 🚀 Esecuzione

```powershell
dotnet run
```

## 📊 Output Esempio

```
=== LEFT JOIN ===
Mario Rossi: 100
Mario Rossi: 200
Laura Bianchi: 150
Giovanni Verdi: 0

=== RIGHT JOIN (simulato) ===
Mario Rossi: 100
Mario Rossi: 200
Laura Bianchi: 150

=== LEFT JOIN (EF Core 10 - Nuova Sintassi) ===
Mario Rossi: 100
Mario Rossi: 200
Laura Bianchi: 150
Giovanni Verdi: 0

=== RIGHT JOIN (EF Core 10 - Nuova Sintassi simulato con LeftJoin) ===
Mario Rossi: 100
Mario Rossi: 200
Laura Bianchi: 150
```

## 🎓 Differenze tra LEFT e RIGHT JOIN

| Tipo | Descrizione | Risultato |
|------|-------------|-----------|
| **LEFT JOIN** | Mantiene tutte le righe della tabella sinistra (Customers) | Include clienti senza ordini |
| **RIGHT JOIN** | Mantiene tutte le righe della tabella destra (Orders) | Include tutti gli ordini (esclude clienti senza ordini) |

## 💡 Note Importanti

- Il progetto usa un **database in-memory** per la simulazione
- Non è richiesta alcuna connessione a database reale
- I dati vengono generati all'avvio dell'applicazione
- Il provider InMemory potrebbe avere limitazioni su alcune query avanzate

## 📦 Pacchetti NuGet

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.1" />
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="10.0.1" />
```

## 🔗 Risorse

- [EF Core 10 Documentation](https://learn.microsoft.com/en-us/ef/core/)
- [LINQ Query Syntax](https://learn.microsoft.com/en-us/dotnet/csharp/linq/)
