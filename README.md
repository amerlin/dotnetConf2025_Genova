# .NET CONF 2025 - 12/12/2025 - Genova  (Dot Net Liguria)

Questa soluzione contiene diverse demo delle nuove funzionalità di Entity Framework Core 10.

## 🚀 Avvio Rapido

Per eseguire le demo, avvia il progetto `DemoLauncher`:

```bash
cd DemoLauncher
dotnet run
```

Oppure apri la soluzione `DotNetConfGenova.sln` in Visual Studio e imposta `DemoLauncher` come progetto di avvio.

## 📂 Demo Disponibili

1. **[Complex Type](ComplexType/README.md)** - Tipi complessi in EF Core 10
2. **[JSON SQL](JsonSql/README.md)** - Supporto JSON avanzato
3. **[Left/Right Join](LeftRightJoin/README.md)** - Nuove funzionalità di join
4. **[Query Filter](QueryFilter/README.md)** - Named query filters
5. **[Sensitive Data Logging](SensitiveDataLogging/README.md)** - Logging dati sensibili
6. **[Vector Search](VectorSearch/README.md)** - Ricerca vettoriale

## 📋 Requisiti

- .NET 10.0 SDK
- SQL Server (per alcune demo) o in-memory database

## 🎯 Utilizzo

Dal menu interattivo del launcher, seleziona il numero corrispondente alla demo che vuoi eseguire. Ogni demo viene eseguita indipendentemente e al termine puoi tornare al menu principale per eseguire un'altra demo.

## 🛠️ Struttura della Soluzione

```
DotNetConfGenova.sln          # Soluzione master
├── DemoLauncher/              # Menu interattivo per lanciare le demo
├── ComplexType/               # Demo tipi complessi
├── JsonSql/                   # Demo JSON
├── LeftRightJoin/             # Demo join
├── QueryFilter/               # Demo query filters
├── SensitiveDataLogging/      # Demo logging
└── VectorSearch/              # Demo ricerca vettoriale
```
