# Demo: Entity Framework 10 - EnableSensitiveDataLogging

Progetto di esempio che dimostra l'utilizzo di `EnableSensitiveDataLogging()` in Entity Framework Core 10 con SQL Server 2025.

## 📋 Descrizione

Questo progetto mostra come configurare e utilizzare la funzione `EnableSensitiveDataLogging()` di Entity Framework Core per visualizzare i valori dei parametri SQL nei log durante lo sviluppo e il debug.

## ⚙️ Funzionalità Dimostrate

- ✅ Configurazione di **EnableSensitiveDataLogging()**
- ✅ Setup del **LoggerFactory** con filtri personalizzati
- ✅ **EnableDetailedErrors()** per messaggi di errore dettagliati
- ✅ Operazioni CRUD (Create, Read, Update, Delete)
- ✅ Query con parametri sensibili
- ✅ Relazioni tra entità (One-to-Many)
- ✅ Query con JOIN e Include

## 🎯 Che cos'è EnableSensitiveDataLogging?

`EnableSensitiveDataLogging()` è un metodo di configurazione di Entity Framework Core che abilita il logging dei **valori effettivi** dei parametri nelle query SQL.

### Senza EnableSensitiveDataLogging:
```sql
SELECT * FROM Utenti WHERE Email = @p0
-- Il valore di @p0 NON è visibile nei log
```

### Con EnableSensitiveDataLogging:
```sql
SELECT * FROM Utenti WHERE Email = @p0
-- Log mostra: @p0 = 'mario.rossi@example.com'
```

## 🔧 Requisiti

- **.NET 10.0** SDK
- **SQL Server LocalDB** (incluso con Visual Studio) o SQL Server 2025
- **Visual Studio 2025** o **Visual Studio Code**

## 📦 Pacchetti NuGet

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.0" />
<PackageReference Include="Microsoft.Extensions.Logging.Console" Version="10.0.0" />
```

## 🚀 Installazione e Esecuzione

### 1. Ripristina i pacchetti NuGet

```bash
dotnet restore
```

### 2. Esegui il progetto

```bash
dotnet run
```

### 3. Osserva i log nella console

L'applicazione creerà automaticamente il database e mostrerà nei log tutte le operazioni SQL con i valori dei parametri visibili.

## 📊 Struttura del Database

### Tabella Utenti
- `Id` (int, PK)
- `Nome` (nvarchar(100))
- `Cognome` (nvarchar(100))
- `Email` (nvarchar(256), unique)
- `Password` (nvarchar(256))
- `DataCreazione` (datetime2)

### Tabella Ordini
- `Id` (int, PK)
- `NumeroOrdine` (nvarchar(50))
- `Importo` (decimal(18,2))
- `DataOrdine` (datetime2)
- `UtenteId` (int, FK)

## 📝 Esempi di Output

Il programma esegue i seguenti esempi:

1. **Inserimento** di un nuovo utente con dati sensibili
2. **Query** con parametri sensibili (ricerca per email)
3. **Update** della password
4. **Query con JOIN** per recuperare ordini con utenti correlati

Ogni operazione mostrerà nei log i comandi SQL con i valori effettivi dei parametri.

## ⚠️ AVVISI DI SICUREZZA

### 🚨 NON USARE IN PRODUZIONE

`EnableSensitiveDataLogging()` deve essere usato **SOLO in ambiente di sviluppo**!

### Perché NON usarlo in produzione:

- ❌ Espone **password** e **dati sensibili** nei log
- ❌ Violazione della **privacy** degli utenti
- ❌ Rischio di **data leak** se i log vengono compromessi
- ❌ Non conforme a **GDPR** e normative sulla privacy
- ❌ Potenziale **vulnerabilità di sicurezza**

### Configurazione Consigliata per Produzione

```csharp
var options = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseSqlServer(connectionString)
    .UseLoggerFactory(loggerFactory)
    // .EnableSensitiveDataLogging() // ❌ COMMENTATO/RIMOSSO
    .Options;
```

### Configurazione Condizionale (Best Practice)

```csharp
var options = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseSqlServer(connectionString)
    .UseLoggerFactory(loggerFactory);

#if DEBUG
    options.EnableSensitiveDataLogging(); // ✅ Solo in Debug
    options.EnableDetailedErrors();
#endif

return options.Options;
```

## 🛠️ Configurazione del Logging

### Livelli di Log Consigliati

```csharp
var loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Information)
        .AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Information)
        .AddConsole();
});
```

### Livelli Disponibili

- **Trace**: Informazioni molto dettagliate
- **Debug**: Informazioni di debug
- **Information**: Messaggi informativi (SQL query)
- **Warning**: Avvisi
- **Error**: Errori
- **Critical**: Errori critici

## 💡 Casi d'Uso

### Quando Usare EnableSensitiveDataLogging

✅ Debug locale durante lo sviluppo  
✅ Troubleshooting di query complesse  
✅ Verifica dei valori passati a stored procedures  
✅ Analisi di performance con valori reali  
✅ Test e validazione del mapping entità

### Quando NON Usarlo

❌ Ambiente di produzione  
❌ Staging/Pre-produzione con dati reali  
❌ Log centralizzati condivisi  
❌ Applicazioni con dati sensibili (healthcare, finance)  
❌ Sistemi soggetti a audit di sicurezza

## 📚 Risorse

- [Entity Framework Core Documentation](https://docs.microsoft.com/ef/core/)
- [Logging in EF Core](https://docs.microsoft.com/ef/core/logging-events-diagnostics/)
- [SQL Server 2025 Documentation](https://docs.microsoft.com/sql/)

## 📄 Licenza

Questo è un progetto di esempio per scopi educativi.

## 👨‍💻 Autore

Demo per DotNetConf Genova 2025

---

**Ricorda**: La sicurezza dei dati è fondamentale! Usa `EnableSensitiveDataLogging()` con responsabilità. 🔒
