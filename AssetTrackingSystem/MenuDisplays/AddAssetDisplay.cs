using System.Text.RegularExpressions;
using Spectre.Console;

namespace AssetTrackingSystem;
//Lets the user add an asset
public class AddAssetDisplay : AbstractMenuDisplay
{
    AssetTracker _tracker;

    public AddAssetDisplay(string title, AbstractMenuDisplay? parent, AssetTracker t) : base(title, parent)
    {
        _tracker = t;
    }

    public override void Display()
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine(
            "[yellow]ADD ASSET, FOLLOW THE INSTRUCTIONS TO ADD AN ASSET, TYPE 'Q' ON AN EMPTY LINE TO QUIT[/]");
        AnsiConsole.WriteLine();

    }


    public override int Interact()
    {



        //Get locations
        HashSet<Office> locations;
        using (AssetDB db = new())
        {
            locations = _tracker.GetOffices(db);
        }

        if (locations.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No offices found.[/]");
            Thread.Sleep(2000);
            return -1;
        }

        //Get input

        SelectionPrompt<string> p = new SelectionPrompt<string>();
        p.Title($"[yellow]Select Office[/]:");
        foreach (Office office in locations)
        {
            p.AddChoice(office.City ?? "");
        }
        p.AddChoice("Quit to main menu");
        

        p.EnableSearch();

        string location = AnsiConsole.Prompt(p);
        
        if (location == "Quit to main menu")
        
        {
            return -1;
        }

        string assetType = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]Asset Type[/]:")
                .EnableSearch()
                .AddChoices("Desktop", "Laptop", "Mobile", "Tablet", "Quit to main menu"));
            if (assetType == "Quit to main menu")
            {
                return -1;
            }
        
        string brand = "";
        do
        {
            brand = AnsiConsole.Ask<string>("Brand:");
            if (brand.Trim().ToLower() == "q")
            {
                return -1;
            }
        } while (brand == "");
        

        string modelName = "";
        do
        {
            var prompt = new TextPrompt<string>("Model name:");
            modelName = AnsiConsole.Prompt(prompt);
            if (modelName.Trim().ToLower() == "q")
            {
                return -1;
            }
        } while (modelName == "");

        

        string serialNumber = "";
        do
        {
            serialNumber = AnsiConsole.Ask<string>("Serial number:");
            if (serialNumber.Trim().ToLower() == "q")
            {
                return -1;
            }
        } while (serialNumber == "");

        string purchaseDateString = "";
        DateTime purchaseDate;
        do
        {
            purchaseDateString = AnsiConsole.Ask<string>("Purchase Date(yyyy-mm-dd):");
            if (purchaseDateString.Trim().ToLower() == "q")
            {
                return -1;
            }

        } while (!DateTime.TryParse(purchaseDateString, out purchaseDate));

        string warrantyExpirationDateString = "";
        DateTime warrantyExpirationDate;
        do
        {
            warrantyExpirationDateString = AnsiConsole.Ask<string>($"Warrant expiry Date(yyyy-mm-dd):");
            if (warrantyExpirationDateString.Trim().ToLower() == "q")
            {
                return -1;
            }
        } while (!DateTime.TryParse(warrantyExpirationDateString, out warrantyExpirationDate));

        string purchasePriceString = "";
        decimal purchasePrice;
        do
        {
            purchasePriceString = AnsiConsole.Ask<string>($"Purchase Price($):");
            if (purchasePriceString.Trim().ToLower() == "q")
            {
                return -1;
            }
        } while (!Decimal.TryParse(purchasePriceString, out purchasePrice));

        int ret = _tracker.UpdateAsset(0, assetType, brand, modelName, purchaseDate, purchasePrice, location,
            serialNumber,
            0, warrantyExpirationDate);
        AnsiConsole.MarkupLine("[green]ASSET ADDED![/]");
        Thread.Sleep(2000);
        return ret;
    }

    public override AbstractMenuDisplay? Run()
    {
        Display();
        var i = Interact();
        Context = Parent;
        return Context;
    }
}