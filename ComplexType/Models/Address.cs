namespace ComplexType.Models;

/// <summary>
/// Complex Type per rappresentare un indirizzo
/// I Complex Types in EF Core 10 sono value objects che non hanno identità propria
/// </summary>
public class Address
{
	public required string Street { get; set; }
	public required string City { get; set; }
	public required string PostalCode { get; set; }
	public required string Country { get; set; }
}
