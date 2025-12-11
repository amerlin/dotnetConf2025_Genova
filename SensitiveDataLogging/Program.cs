using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

// Configurazione del DbContext con logging e EnableSensitiveDataLogging
var loggerFactory = LoggerFactory.Create(builder =>
{
	builder
		.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Information)
		.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Information)
		.AddConsole();
});

var options = new DbContextOptionsBuilder<ApplicationDbContext>()
	.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=SensitiveDataLoggingDemo;Trusted_Connection=True;TrustServerCertificate=True;")
	.UseLoggerFactory(loggerFactory)
	.EnableSensitiveDataLogging() // Abilita il logging dei dati sensibili (solo per sviluppo!)
	.EnableDetailedErrors()
	.Options;

using var context = new ApplicationDbContext(options);

// Crea il database se non esiste
await context.Database.EnsureDeletedAsync();
await context.Database.EnsureCreatedAsync();

Console.WriteLine("=== DEMO: EnableSensitiveDataLogging ===\n");

// Esempio 1: Inserimento con dati sensibili visibili nei log
Console.WriteLine("1. Inserimento di un nuovo utente...");
var nuovoUtente = new Utente
{
	Nome = "Mario",
	Cognome = "Rossi",
	Email = "mario.rossi@example.com",
	Password = "Password123!" // Dato sensibile!
};

context.Utenti.Add(nuovoUtente);
await context.SaveChangesAsync();
Console.WriteLine("✓ Utente salvato\n");

// Esempio 2: Query con parametri sensibili visibili
Console.WriteLine("2. Query con parametri sensibili...");
var email = "mario.rossi@example.com";
var utenteTrovato = await context.Utenti
	.Where(u => u.Email == email)
	.FirstOrDefaultAsync();
Console.WriteLine($"✓ Utente trovato: {utenteTrovato?.Nome} {utenteTrovato?.Cognome}\n");

// Esempio 3: Update con dati sensibili
Console.WriteLine("3. Aggiornamento password...");
if (utenteTrovato != null)
{
	utenteTrovato.Password = "NuovaPassword456!";
	await context.SaveChangesAsync();
}
Console.WriteLine("✓ Password aggiornata\n");

// Esempio 4: Query complessa con JOIN
Console.WriteLine("4. Query con JOIN...");
var ordine = new Ordine
{
	UtenteId = nuovoUtente.Id,
	NumeroOrdine = "ORD-2025-001",
	Importo = 99.99m,
	DataOrdine = DateTime.Now
};

context.Ordini.Add(ordine);
await context.SaveChangesAsync();

var ordiniUtente = await context.Ordini
	.Include(o => o.Utente)
	.Where(o => o.UtenteId == nuovoUtente.Id)
	.ToListAsync();

Console.WriteLine($"✓ Trovati {ordiniUtente.Count} ordini\n");

// Esempio 5: Dimostrazione del problema con EnableSensitiveDataLogging disabilitato
Console.WriteLine("5. Confronto con/senza EnableSensitiveDataLogging:");
Console.WriteLine("   CON EnableSensitiveDataLogging: I parametri SQL sono visibili nei log");
Console.WriteLine("   SENZA EnableSensitiveDataLogging: I parametri SQL sono nascosti (mostrati come @p0, @p1, ecc.)\n");

Console.WriteLine("=== ATTENZIONE ===");
Console.WriteLine("EnableSensitiveDataLogging dovrebbe essere usato SOLO in ambiente di sviluppo!");
Console.WriteLine("Non abilitare mai questa funzione in produzione per motivi di sicurezza.\n");

// DbContext personalizzato
public class ApplicationDbContext : DbContext
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		: base(options)
	{
	}

	public DbSet<Utente> Utenti { get; set; }
	public DbSet<Ordine> Ordini { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		// Configurazione aggiuntiva se necessaria
		base.OnConfiguring(optionsBuilder);
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		// Configurazione delle entità
		modelBuilder.Entity<Utente>(entity =>
		{
			entity.HasKey(e => e.Id);
			entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
			entity.Property(e => e.Cognome).IsRequired().HasMaxLength(100);
			entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
			entity.HasIndex(e => e.Email).IsUnique();
			entity.Property(e => e.Password).IsRequired().HasMaxLength(256);
		});

		modelBuilder.Entity<Ordine>(entity =>
		{
			entity.HasKey(e => e.Id);
			entity.Property(e => e.NumeroOrdine).IsRequired().HasMaxLength(50);
			entity.Property(e => e.Importo).HasPrecision(18, 2);

			entity.HasOne(e => e.Utente)
				.WithMany(u => u.Ordini)
				.HasForeignKey(e => e.UtenteId)
				.OnDelete(DeleteBehavior.Cascade);
		});

		base.OnModelCreating(modelBuilder);
	}
}

// Modelli di esempio
public class Utente
{
	public int Id { get; set; }
	public string Nome { get; set; } = string.Empty;
	public string Cognome { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty; // Dato sensibile!
	public DateTime DataCreazione { get; set; } = DateTime.Now;

	public ICollection<Ordine> Ordini { get; set; } = new List<Ordine>();
}

public class Ordine
{
	public int Id { get; set; }
	public string NumeroOrdine { get; set; } = string.Empty;
	public decimal Importo { get; set; }
	public DateTime DataOrdine { get; set; }

	public int UtenteId { get; set; }
	public Utente? Utente { get; set; }
}
