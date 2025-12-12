using LeftRightJoin;
using LeftRightJoin.Model;

// Creo il database SQL Server e applico le migrazioni
using var context = new MyDbContex();

Console.WriteLine("Creazione database LeftJoin...");
await context.Database.EnsureDeletedAsync(); // Pulisce il database se esiste
await context.Database.EnsureCreatedAsync(); // Crea il database
Console.WriteLine("Database creato!\n");

// Aggiungo clienti
context.Customers.AddRange(
    new Customer { Name = "Mario Rossi" },
    new Customer { Name = "Laura Bianchi" },
    new Customer { Name = "Giovanni Verdi" }
);

await context.SaveChangesAsync();

// Recupero gli Id generati automaticamente
var mario = context.Customers.First(c => c.Name == "Mario Rossi");
var laura = context.Customers.First(c => c.Name == "Laura Bianchi");

// Aggiungo ordini (alcuni clienti non hanno ordini)
context.Orders.AddRange(
    new Order { CustomerId = mario.Id, Amount = 100 },
    new Order { CustomerId = mario.Id, Amount = 200 },
    new Order { CustomerId = laura.Id, Amount = 150 }
);

await context.SaveChangesAsync();

Console.WriteLine("=== LEFT JOIN (Query Syntax - Metodo Tradizionale) ===");
// Sintassi tradizionale con GroupJoin e SelectMany
var leftJoin = from c in context.Customers
               join o in context.Orders on c.Id equals o.CustomerId into orders
               from o in orders.DefaultIfEmpty()
               select new { c.Name, OrderAmount = (o != null ? o.Amount : 0) };

foreach (var item in leftJoin)
{
    Console.WriteLine($"{item.Name}: {item.OrderAmount}");
}

Console.WriteLine("\n=== LEFT JOIN (Method Syntax - Metodo Tradizionale) ===");
// Stessa cosa ma con method syntax
var leftJoinMethod = context.Customers
    .GroupJoin(
        context.Orders,
        c => c.Id,
        o => o.CustomerId,
        (c, orders) => new { Customer = c, Orders = orders })
    .SelectMany(
        x => x.Orders.DefaultIfEmpty(),
        (x, o) => new { x.Customer.Name, OrderAmount = (o != null ? o.Amount : 0) });

foreach (var item in leftJoinMethod)
{
    Console.WriteLine($"{item.Name}: {item.OrderAmount}");
}

Console.WriteLine("\n=== RIGHT JOIN (Query Syntax - Simulato) ===");
// RIGHT JOIN simulato invertendo la query: Orders LEFT JOIN Customers
var rightJoin = from o in context.Orders
                join c in context.Customers on o.CustomerId equals c.Id into customers
                from c in customers.DefaultIfEmpty()
                select new { CustomerName = (c != null ? c.Name : "N/A"), o.Amount };

foreach (var item in rightJoin)
{
    Console.WriteLine($"{item.CustomerName}: {item.Amount}");
}

Console.WriteLine("\n=== RIGHT JOIN (Method Syntax - Simulato) ===");
// RIGHT JOIN con method syntax
var rightJoinMethod = context.Orders
    .GroupJoin(
        context.Customers,
        o => o.CustomerId,
        c => c.Id,
        (o, customers) => new { Order = o, Customers = customers })
    .SelectMany(
        x => x.Customers.DefaultIfEmpty(),
        (x, c) => new { CustomerName = (c != null ? c.Name : "N/A"), x.Order.Amount });

foreach (var item in rightJoinMethod)
{
    Console.WriteLine($"{item.CustomerName}: {item.Amount}");
}

Console.WriteLine("\n=== LEFT JOIN (EF Core 10 - Nuova Sintassi) ===");
// EF Core 10 introduce il metodo LeftJoin diretto per semplificare il codice
var leftJoinNew = context.Customers
    .LeftJoin(
        context.Orders,
        c => c.Id,
        o => o.CustomerId,
        (c, o) => new { c.Name, OrderAmount = (o != null ? o.Amount : 0) });

foreach (var item in leftJoinNew)
{
    Console.WriteLine($"{item.Name}: {item.OrderAmount}");
}

Console.WriteLine("\n=== RIGHT JOIN (EF Core 10 - Nuova Sintassi) ===");
// EF Core 10 introduce anche il metodo RightJoin nativo (funziona con SQL Server)
var rightJoinNew = context.Customers
    .RightJoin(
        context.Orders,
        c => c.Id,
        o => o.CustomerId,
        (c, o) => new { CustomerName = (c != null ? c.Name : "N/A"), o.Amount });

foreach (var item in rightJoinNew)
{
    Console.WriteLine($"{item.CustomerName}: {item.Amount}");
}

