namespace JsonSql;

// Modello principale con colonna JSON
public class Product
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public decimal Price { get; set; }

	// Colonna JSON per i dettagli del prodotto
	public ProductDetails Details { get; set; } = new();

	// Colonna JSON per le specifiche tecniche
	public TechnicalSpecs? Specifications { get; set; }
}

// Oggetto complesso mappato come JSON
public class ProductDetails
{
	public string Category { get; set; } = string.Empty;
	public string Manufacturer { get; set; } = string.Empty;
	public List<string> Tags { get; set; } = new();
	public Dimensions? Dimensions { get; set; }
}

public class Dimensions
{
	public double Width { get; set; }
	public double Height { get; set; }
	public double Depth { get; set; }
	public string Unit { get; set; } = "cm";
}

public class TechnicalSpecs
{
	public string Model { get; set; } = string.Empty;
	public int WarrantyMonths { get; set; }
	public List<Feature> Features { get; set; } = new();
}

public class Feature
{
	public string Key { get; set; } = string.Empty;
	public string Value { get; set; } = string.Empty;
}
