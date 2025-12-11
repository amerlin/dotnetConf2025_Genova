using LeftRightJoin;
using LeftRightJoin.Model;

// Creo il database in-memory e aggiungo dati di test
using var context = new MyDbContex();
// Aggiungo clienti
context.Customers.AddRange(
    new Customer { Id = 1, Name = "Mario Rossi" },
    new Customer { Id = 2, Name = "Laura Bianchi" },
    new Customer { Id = 3, Name = "Giovanni Verdi" }
);

// Aggiungo ordini (alcuni clienti non hanno ordini)
context.Orders.AddRange(
    new Order { Id = 1, CustomerId = 1, Amount = 100 },
    new Order { Id = 2, CustomerId = 1, Amount = 200 },
    new Order { Id = 3, CustomerId = 2, Amount = 150 }
);

context.SaveChanges();

Console.WriteLine("=== LEFT JOIN ===");
var leftJoin = from c in context.Customers
               join o in context.Orders on c.Id equals o.CustomerId into orders
               from o in orders.DefaultIfEmpty()
               select new { c.Name, OrderAmount = (o != null ? o.Amount : 0) };

foreach (var item in leftJoin)
{
    Console.WriteLine($"{item.Name}: {item.OrderAmount}");
}

Console.WriteLine("\n=== RIGHT JOIN (simulato) ===");
var rightJoin = from o in context.Orders
                join c in context.Customers on o.CustomerId equals c.Id into customers
                from c in customers.DefaultIfEmpty()
                select new { CustomerName = (c != null ? c.Name : "N/A"), o.Amount };

foreach (var item in rightJoin)
{
    Console.WriteLine($"{item.CustomerName}: {item.Amount}");
}

Console.WriteLine("\n=== LEFT JOIN (EF Core 10 - Nuova Sintassi) ===");
var leftJoinNew = context.Customers
    .LeftJoin(context.Orders, c => c.Id, o => o.CustomerId, (c, o) => new { c.Name, OrderAmount = (o != null ? o.Amount : 0) });

foreach (var item in leftJoinNew)
{
    Console.WriteLine($"{item.Name}: {item.OrderAmount}");
}

Console.WriteLine("\n=== RIGHT JOIN (EF Core 10 - Nuova Sintassi simulato con LeftJoin) ===");
// RightJoin non è supportato da InMemory, ma possiamo simularlo invertendo la logica con LeftJoin
var rightJoinNew = context.Customers
    .LeftJoin(context.Orders, c => c.Id, o => o.CustomerId, (c, o) => new { c.Name, OrderAmount = (o != null ? o.Amount : 0) })
    .Where(x => x.OrderAmount > 0); // Solo chi ha ordini

foreach (var item in rightJoinNew)
{
    Console.WriteLine($"{item.Name}: {item.OrderAmount}");
}

