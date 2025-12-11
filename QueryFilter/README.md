# Query Filter Demo - Named Query Filters in EF Core

Questo progetto dimostra l'utilizzo dei **Named Query Filters** in Entity Framework Core, una funzionalità che permette di applicare e disabilitare selettivamente i filtri delle query.

## 📋 Descrizione

Il progetto mostra come:
- Definire filtri nominati su entità EF Core
- Applicare filtri automaticamente a tutte le query
- Disabilitare selettivamente filtri specifici
- Disabilitare tutti i filtri contemporaneamente

## 🏗️ Struttura del Progetto

### Models/Product.cs
Entità di base con le seguenti proprietà:
- `Id`: Identificatore del prodotto
- `Name`: Nome del prodotto
- `IsActive`: Stato di attivazione
- `IsPrivate`: Flag di visibilità (pubblico/privato)
- `Price`: Prezzo del prodotto

### DemoDbContext.cs
Contesto del database con due filtri nominati:

1. **ActiveOnly**: Filtra solo i prodotti attivi (`p => p.IsActive`)
2. **PublicOnly**: Filtra solo i prodotti pubblici (`p => !p.IsPrivate`)

#### Dati di Test
```csharp
- Laptop Base (Attivo: ✓, Privato: ✗, Prezzo: €1200)
- Smartphone Deluxe (Attivo: ✓, Privato: ✗, Prezzo: €800)
- Vecchio Monitor (Attivo: ✗, Privato: ✗, Prezzo: €150)
- Prodotto Riservato (Attivo: ✓, Privato: ✓, Prezzo: €999.99)
```

### NamedQueryFilterDemo.cs
Classe che esegue 4 scenari di test:

#### 1️⃣ Query Standard (Tutti i filtri attivi)
```csharp
db.Products
```
**Risultato**: 2 prodotti (Laptop Base, Smartphone Deluxe)

#### 2️⃣ Disabilita filtro "PublicOnly"
```csharp
db.Products.IgnoreQueryFilters(["PublicOnly"])
```
**Risultato**: 3 prodotti (include "Prodotto Riservato")

#### 3️⃣ Disabilita filtro "ActiveOnly"
```csharp
db.Products.IgnoreQueryFilters(["ActiveOnly"])
```
**Risultato**: 3 prodotti (include "Vecchio Monitor")

#### 4️⃣ Disabilita TUTTI i filtri
```csharp
db.Products.IgnoreQueryFilters()
```
**Risultato**: 4 prodotti (tutti)

## 🚀 Esecuzione

```bash
dotnet run
```

## 📊 Output Atteso

```
--- 1. Query Standard (Tutti i filtri attivi) ---
Conteggio risultati: 2
- ID: 1, Nome: Laptop Base (Attivo: True, Privato: False)
- ID: 2, Nome: Smartphone Deluxe (Attivo: True, Privato: False)

--- 2. Disabilita il filtro 'PublicOnly' ---
Conteggio risultati: 3
- ID: 1, Nome: Laptop Base (Attivo: True, Privato: False)
- ID: 2, Nome: Smartphone Deluxe (Attivo: True, Privato: False)
- ID: 4, Nome: Prodotto Riservato (Attivo: True, Privato: True)

--- 3. Disabilita il filtro 'ActiveOnly' ---
Conteggio risultati: 3
- ID: 1, Nome: Laptop Base (Attivo: True, Privato: False)
- ID: 2, Nome: Smartphone Deluxe (Attivo: True, Privato: False)
- ID: 3, Nome: Vecchio Monitor (Attivo: False, Privato: False)

--- 4. Disabilita TUTTI i filtri ---
Conteggio risultati: 4
- ID: 1, Nome: Laptop Base (Attivo: True, Privato: False)
- ID: 2, Nome: Smartphone Deluxe (Attivo: True, Privato: False)
- ID: 3, Nome: Vecchio Monitor (Attivo: False, Privato: False)
- ID: 4, Nome: Prodotto Riservato (Attivo: True, Privato: True)
```

## 🔑 Concetti Chiave

### Named Query Filters
I filtri nominati permettono di:
- ✅ Dare un nome identificativo ai filtri
- ✅ Disabilitare selettivamente filtri specifici
- ✅ Mantenere altri filtri attivi
- ✅ Maggiore controllo e flessibilità

### Sintassi
```csharp
// Definizione del filtro
modelBuilder.Entity<Product>()
    .HasQueryFilter("NomeFiltro", p => p.Condizione);

// Disabilita filtri specifici
query.IgnoreQueryFilters(["NomeFiltro1", "NomeFiltro2"])

// Disabilita tutti i filtri
query.IgnoreQueryFilters()
```

## 🛠️ Tecnologie

- .NET 10.0
- Entity Framework Core (con Named Query Filters)
- Database In-Memory

## 📝 Note

I Named Query Filters sono particolarmente utili per:
- Multi-tenancy (filtrare dati per tenant)
- Soft delete (filtrare record cancellati logicamente)
- Permessi e visibilità (filtrare dati in base a ruoli)
- Stato dei record (filtrare per stato attivo/inattivo)
