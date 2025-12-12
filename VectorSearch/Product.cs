namespace VectorSearch;

public class Product
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public string Category { get; set; } = string.Empty;
	public decimal Price { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

	/// <summary>
	/// EF Core 10 Feature: Primitive Collection
	/// List<float> viene automaticamente serializzato come JSON in SQL Server
	/// Nessuna configurazione manuale necessaria!
	/// </summary>
	public List<float> Embedding { get; set; } = new();
}
