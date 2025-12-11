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
	/// SQL Server 2025 supporta VECTOR come tipo nativo
	/// </summary>
	[Column(TypeName = "vector(384)")]
	public float[] Embedding { get; set; } = Array.Empty<float>();

	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
