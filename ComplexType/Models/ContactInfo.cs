namespace ComplexType.Models;

/// <summary>
/// Complex Type per le informazioni di contatto
/// </summary>
public class ContactInfo
{
	public required string Email { get; set; }
	public required string Phone { get; set; }
	public string? Website { get; set; }
}
