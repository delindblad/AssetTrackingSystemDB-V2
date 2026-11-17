using System.Text.Json;
using freecurrencyapi;
using OfficeTrackingSystem;
using Spectre.Console;

namespace AssetTrackingSystem;

class Program
{
    static void Main(string[] args)
    {   
        
        AnsiConsole.MarkupLine("[yellow]Welcome to the Asset Tracker![/]");
        if (args.Length > 1)
            
        {
            if (args[1] == "init")
            {
                AnsiConsole.WriteLine("Adding sample data...");
                AddStuff();
                Environment.Exit(0);
            }
            else
            {
                AnsiConsole.WriteLine("[red]Invalid parameter![/]");
                Environment.Exit(0);
            }
        }
        string? connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        if (connectionString == null)
        {
            AnsiConsole.WriteLine("Connection string is null, please check your environment variables.");
            Environment.Exit(-1);
        }

        
        Run();

    }

    static void AddStuff()
    {
        AssetTracker assetTracker = new();
        assetTracker.UpdateCurrencyData();
        
        
        assetTracker.AddOffice("New York", "$");
        assetTracker.AddOffice("London", "£");
        assetTracker.AddOffice("Frankfurt", "€");
        assetTracker.AddOffice("Tokyo", "¥");
        assetTracker.AddOffice("Copenhagen", "DKK ");
        assetTracker.UpdateAsset(0,"Mobile", "Apple", "iPhone 14", DateTime.Parse("2024-04-04"),600,
            "London", "4354364366656", 0, DateTime.Parse("2023-04-10"));
        assetTracker.UpdateAsset(0, "Laptop", "Lenovo", "Think Pad X1", DateTime.Parse("2024-06-06"), 400,
            "Frankfurt", "435-4657-666-56", 0, DateTime.Parse("2025-02-06"));
        assetTracker.UpdateAsset(0,"Tablet", "Apple", "iPad 12", DateTime.Parse("2025-08-06"),1100,
            "Tokyo", "435-4657-666-56", 0, DateTime.Parse("2027-02-06"));
        assetTracker.UpdateAsset(0,"Tablet", "Apple", "iPad 14", DateTime.Parse("2025-08-06"),1200,
            "Copenhagen", "435-4657-666-56", 0, DateTime.Parse("2027-02-06"));
        
        
    }
    
    static void Run()
    {
        //Create asset tracker

        AssetTracker assetTracker = new AssetTracker();
            

        //Update currency data
        try
        {
            assetTracker.UpdateCurrencyData();
        }
        catch (Exception e)
        {
            AnsiConsole.Markup($"[red]FAILED TO UPDATE CURRENCY DATA[/]");
        }
        //Create root page
        var root = new RootDisplay("", assetTracker);
        
        root.AddChildPage(new ListAssetsDisplay("LIST ASSETS", root, assetTracker));
        root.AddChildPage(new SearchAssetsDisplay("SEARCH ASSETS", root, assetTracker));
        root.AddChildPage(new AddAssetDisplay("ADD ASSET", root, assetTracker));
        root.AddChildPage(new UpdateAssetDisplay("UPDATE ASSET", root, assetTracker));
        root.AddChildPage(new ListOfficesDisplay("LIST OFFICES", root, assetTracker));
        root.AddChildPage(new AddOfficeDisplay("ADD OFFICE", root, assetTracker));
        root.AddChildPage(new DeleteAssetDisplay("DELETE ASSET", root, assetTracker));
        root.AddChildPage(new DeleteOfficeDisplay("DELETE OFFICE", root, assetTracker));
        root.AddChildPage(new ExitDisplay("EXIT", root));

        
    
       
       AbstractMenuDisplay? context = root;
       while (true)
       {
           context = context.Run();
           if (context == null!)
           {
               return;
           }
       }
    }
}