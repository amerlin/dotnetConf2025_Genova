using System.Diagnostics;

Console.Clear();
Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
Console.WriteLine("║     DotNetConf Genova - Entity Framework 10 Demo Launcher     ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
Console.WriteLine();

while (true)
{
	Console.WriteLine("Seleziona una demo da eseguire:");
	Console.WriteLine();
	Console.WriteLine("  1. Complex Type - Tipi complessi in EF Core 10");
	Console.WriteLine("  2. JSON SQL - Supporto JSON avanzato");
	Console.WriteLine("  3. Left/Right Join - Nuove funzionalità di join");
	Console.WriteLine("  4. Query Filter - Named query filters");
	Console.WriteLine("  5. Sensitive Data Logging - Logging dati sensibili");
	Console.WriteLine("  6. Vector Search - Ricerca vettoriale");
	Console.WriteLine();
	Console.WriteLine("  0. Esci");
	Console.WriteLine();
	Console.Write("Scelta: ");

	var choice = Console.ReadLine();
	Console.WriteLine();

	if (choice == "0")
	{
		Console.WriteLine("Arrivederci!");
		break;
	}

	var demoPath = choice switch
	{
		"1" => "ComplexType",
		"2" => "JsonSql",
		"3" => "LeftRightJoin",
		"4" => "QueryFilter",
		"5" => "SensitiveDataLogging",
		"6" => "VectorSearch",
		_ => null
	};

	if (demoPath == null)
	{
		Console.WriteLine("❌ Scelta non valida. Riprova.");
		Console.WriteLine();
		continue;
	}

	Console.WriteLine($"▶ Esecuzione demo: {demoPath}");
	Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
	Console.WriteLine();

	try
	{
		var projectPath = Path.Combine("..", demoPath);
		var startInfo = new ProcessStartInfo
		{
			FileName = "dotnet",
			Arguments = $"run --project {projectPath}",
			UseShellExecute = false,
			RedirectStandardOutput = true,
			RedirectStandardError = true
		};

		using var process = Process.Start(startInfo);
		if (process != null)
		{
			var output = process.StandardOutput.ReadToEnd();
			var error = process.StandardError.ReadToEnd();

			process.WaitForExit();

			Console.WriteLine(output);
			if (!string.IsNullOrEmpty(error))
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(error);
				Console.ResetColor();
			}
		}
	}
	catch (Exception ex)
	{
		Console.ForegroundColor = ConsoleColor.Red;
		Console.WriteLine($"❌ Errore durante l'esecuzione della demo: {ex.Message}");
		Console.ResetColor();
	}

	Console.WriteLine();
	Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
	Console.WriteLine();
	Console.WriteLine("Premi un tasto per tornare al menu...");
	Console.ReadKey();
	Console.Clear();
	Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
	Console.WriteLine("║     DotNetConf Genova - Entity Framework 10 Demo Launcher     ║");
	Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
	Console.WriteLine();
}
