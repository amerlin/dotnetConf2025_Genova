# Complex Types in EF Core 10 - Demo

Progetto di esempio che dimostra l'utilizzo dei **Complex Types** in Entity Framework Core 10 con SQL Server 2025.

## 📋 Descrizione

Questo progetto illustra come utilizzare i Complex Types (introdotti in EF Core 8 e migliorati in EF Core 10) per mappare value objects direttamente nelle entità, senza la necessità di creare tabelle separate.

### Cosa sono i Complex Types?

I Complex Types sono un modo per raggruppare proprietà correlate in oggetti riutilizzabili che:
- Non hanno una chiave primaria propria
- Non esistono indipendentemente dall'entità principale
- Vengono mappati come colonne nella stessa tabella dell'entità
- Rappresentano concetti di dominio come indirizzi, coordinate, range di date, etc.

## 🏗️ Struttura del Progetto

```
ComplexType/
├── Models/
│   ├── Address.cs          # Complex type per indirizzi
│   ├── ContactInfo.cs      # Complex type per informazioni di contatto
│   └── Customer.cs         # Entity principale
├── Data/
│   └── ApplicationDbContext.cs  # DbContext con configurazione
├── Program.cs              # Demo con esempi CRUD
└── ComplexType.csproj      # Configurazione progetto
```

### Modelli

**Address** - Complex type riutilizzabile per indirizzi:
- Street
- City
- PostalCode
- Country

**ContactInfo** - Complex type per le informazioni di contatto:
- Email
- Phone
- Website (opzionale)

**Customer** - Entity principale che utilizza i complex types:
- Id (chiave primaria)
- Name
- BillingAddress (Complex Type)
- ShippingAddress (Complex Type)
- Contact (Complex Type)
- CreatedAt

## ⚙️ Configurazione

### Requisiti

- .NET 10.0
- SQL Server 2025 (o LocalDB)
- Entity Framework Core 10.0

### Packages NuGet

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.0" />
```

### Mapping Complex Types

In EF Core 10, i Complex Types si configurano usando il metodo `ComplexProperty`:

```csharp
modelBuilder.Entity<Customer>(entity =>
{
    // Configurazione Complex Type
    entity.ComplexProperty(c => c.BillingAddress, address =>
    {
        address.Property(a => a.Street).IsRequired().HasMaxLength(200);
        address.Property(a => a.City).IsRequired().HasMaxLength(100);
        address.Property(a => a.PostalCode).IsRequired().HasMaxLength(20);
        address.Property(a => a.Country).IsRequired().HasMaxLength(100);
    });
});
```

## 🚀 Esecuzione

1. **Ripristinare i pacchetti NuGet:**
   ```bash
   dotnet restore
   ```

2. **Eseguire l'applicazione:**
   ```bash
   dotnet run
   ```

### Cosa fa il programma?

Il programma dimostra le operazioni principali con Complex Types:

1. ✅ **Creazione database** - Crea automaticamente il database con lo schema appropriato
2. ➕ **Insert** - Inserisce un nuovo cliente con indirizzi e contatti
3. 🔍 **Query** - Legge i dati e filtra per città
4. ✏️ **Update** - Aggiorna un Complex Type (indirizzo di spedizione)

## 💾 Schema Database

La tabella `Customers` generata avrà la seguente struttura:

| Colonna | Tipo | Descrizione |
|---------|------|-------------|
| Id | int | Chiave primaria (IDENTITY) |
| Name | nvarchar(200) | Nome cliente |
| BillingAddress_Street | nvarchar(200) | Via fatturazione |
| BillingAddress_City | nvarchar(100) | Città fatturazione |
| BillingAddress_PostalCode | nvarchar(20) | CAP fatturazione |
| BillingAddress_Country | nvarchar(100) | Paese fatturazione |
| ShippingAddress_Street | nvarchar(200) | Via spedizione |
| ShippingAddress_City | nvarchar(100) | Città spedizione |
| ShippingAddress_PostalCode | nvarchar(20) | CAP spedizione |
| ShippingAddress_Country | nvarchar(100) | Paese spedizione |
| Contact_Email | nvarchar(200) | Email contatto |
| Contact_Phone | nvarchar(50) | Telefono contatto |
| Contact_Website | nvarchar(200) | Sito web (nullable) |
| CreatedAt | datetime2 | Data creazione (default: GETUTCDATE()) |

### Naming Convention

EF Core usa automaticamente il pattern `{PropertyName}_{ComplexTypePropertyName}` per i nomi delle colonne.

## 🔍 Query sui Complex Types

È possibile filtrare direttamente sulle proprietà dei Complex Types:

```csharp
var genovaCustomers = await context.Customers
    .Where(c => c.BillingAddress.City == "Genova")
    .ToListAsync();
```

## ✏️ Update dei Complex Types

Per aggiornare un Complex Type, si deve sostituire l'intero oggetto:

```csharp
customer.ShippingAddress = new Address
{
    Street = "Piazza San Marco 1",
    City = "Venezia",
    PostalCode = "30100",
    Country = "Italia"
};
await context.SaveChangesAsync();
```

## 🎯 Vantaggi dei Complex Types

✅ **Riutilizzabilità** - Stesso tipo usato per più proprietà (BillingAddress, ShippingAddress)  
✅ **Incapsulamento** - Logica e validazione contenute nel complex type  
✅ **Performance** - Nessun JOIN necessario, tutto in una tabella  
✅ **Semplicità** - Nessuna tabella aggiuntiva da gestire  
✅ **Type Safety** - Fortemente tipizzato in C#

## 📚 Risorse

- [EF Core Documentation - Complex Types](https://learn.microsoft.com/en-us/ef/core/modeling/complex-types)
- [What's New in EF Core 10](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-10.0/whatsnew)

## 📝 Note

- I Complex Types sono ideali per value objects senza identità
- Non supportano navigazioni verso altre entità
- Non possono essere interrogati indipendentemente dall'entità padre
- Per relazioni con identità propria, usare invece Owned Entities o relazioni normali

---

**DotNetConf Genova 2024** - Demo Entity Framework Core 10
