using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VectorSearch;

/// <summary>
/// Rappresenta un prodotto con supporto per vector search
/// </summary>
public class Product
{
	[Key]
	public int Id { get; set; }

	[Required]
	[MaxLength(200)]
	public string Name { get; set; } = string.Empty;

	[MaxLength(1000)]
	public string Description { get; set; } = string.Empty;

	[Column(TypeName = "decimal(18,2)")]
	public decimal Price { get; set; }

	[MaxLength(100)]
	public string Category { get; set; } = string.Empty;

	/// <summary>
	/// Vettore di embedding per la ricerca semantica
	/// EF Core 10 supporta Primitive Collections, salvato come JSON
	/// </summary>
	public List<float> Embedding { get; set; } = new();

	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
