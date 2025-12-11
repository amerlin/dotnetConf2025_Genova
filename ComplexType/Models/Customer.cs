namespace ComplexType.Models;

/// <summary>
/// Entity principale che utilizza complex types
/// </summary>
public class Customer
{
	public int Id { get; set; }
	public required string Name { get; set; }

	// Complex Type: indirizzo di fatturazione
	public required Address BillingAddress { get; set; }

	// Complex Type: indirizzo di spedizione
	public required Address ShippingAddress { get; set; }

	// Complex Type: informazioni di contatto
	public required ContactInfo Contact { get; set; }

	public DateTime CreatedAt { get; set; }
}
