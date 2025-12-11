using Microsoft.EntityFrameworkCore;
using QueryFilter.Models;

namespace QueryFilter
{
    public class NamedQueryFilterDemo
    {
        private readonly DbContextOptions<DemoDbContext> _options;

        public NamedQueryFilterDemo()
        {
            // Configurazione del database In-Memory
            _options = new DbContextOptionsBuilder<DemoDbContext>()
                .UseInMemoryDatabase(databaseName: "ProductCatalogDB")
                .Options;

            // Assicurati che il database e i dati seed siano creati
            using var context = new DemoDbContext(_options);
            context.Database.EnsureCreated();
        }


        private void ExecuteQueryAndPrint(string title, Func<DemoDbContext, IQueryable<Product>> queryBuilder)
        {
            using var context = new DemoDbContext(_options);
            var query = queryBuilder(context);
            var results = query.ToList();

            Console.WriteLine($"\n--- {title} ---");
            Console.WriteLine($"Conteggio risultati: {results.Count}");

            foreach (var p in results)
            {
                Console.WriteLine($"- ID: {p.Id}, Nome: {p.Name} (Attivo: {p.IsActive}, Privato: {p.IsPrivate})");
            }
        }

        public void RunDemos()
        {
            // =================================================================
            // 1. Query Standard: Filtri attivi di default (ActiveOnly e PublicOnly)
            // Risultato atteso: Solo 2 prodotti (Laptop Base, Smartphone Deluxe)
            // =================================================================
            ExecuteQueryAndPrint("1. Query Standard (Tutti i filtri attivi)",
                db => db.Products);

            // =================================================================
            // 2. Disabilita il filtro "PublicOnly"
            // Risultato atteso: 3 prodotti (Include il "Prodotto Riservato")
            // =================================================================
            ExecuteQueryAndPrint("2. Disabilita il filtro 'PublicOnly'",
                db => db.Products.IgnoreQueryFilters(["PublicOnly"]));

            // =================================================================
            // 3. Disabilita il filtro "ActiveOnly"
            // Risultato atteso: 3 prodotti (Include il "Vecchio Monitor")
            // =================================================================
            ExecuteQueryAndPrint("3. Disabilita il filtro 'ActiveOnly'",
                db => db.Products.IgnoreQueryFilters(["ActiveOnly"]));

            // =================================================================
            // 4. Disabilita TUTTI i filtri
            // Risultato atteso: 4 prodotti (Tutti)
            // =================================================================
            ExecuteQueryAndPrint("4. Disabilita TUTTI i filtri",
                db => db.Products.IgnoreQueryFilters());
        }
    }
}