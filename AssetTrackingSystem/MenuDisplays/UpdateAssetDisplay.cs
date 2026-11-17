using System.Text.RegularExpressions;
using Spectre.Console;

namespace AssetTrackingSystem;
//Lets the user update an asset
public class UpdateAssetDisplay : AbstractMenuDisplay
{
    AssetTracker _tracker;
    public UpdateAssetDisplay(string title, AbstractMenuDisplay? parent, AssetTracker t) : base(title, parent)
    {
        _tracker = t;
    }

    public override void Display()
    {
        AnsiConsole.Clear(); 
        AnsiConsole.MarkupLine("[yellow]UPDATE ASSET, FOLLOW THE INSTRUCTIONS TO UPDATE AN ASSET, TYPE 'Q' ON AN EMPTY LINE TO QUIT[/]");
        AnsiConsole.WriteLine();
    }

    public override int Interact()
    {
        //Get the asset ID
        bool success = false;
        int id = 0;
        
        
        //Get List of all assets as a list
        List<Asset> assets;
        List<string> assetList;
        using (AssetDB db = new()) 
        {
            assetList = _tracker.GetAssetList(db);
        }
        

        
        //Get list of assets and display as a searchable coice list
        SelectionPrompt<string> p = new SelectionPrompt<string>();
        do
        {
            p.Title($"[yellow]Select Asset[/]:");
            foreach (string s in assetList!)
            {
                p.AddChoice(s);
            }
            p.AddChoice("Quit to main menu");
            p.EnableSearch();
            
            
            
            string selectedAsset = AnsiConsole.Prompt(p);
            if (selectedAsset == "Quit to main menu")
            {
                return -1;
            }
            //REgexp to extract the ID
            string assetNumString = Regex.Match(selectedAsset, @"^[0-9]+").Value;
            

            success = int.TryParse(assetNumString, out id);
            if (!success)
            {
                AnsiConsole.MarkupLine("[red]Invalid input. Please enter a valid integer.[/]");
            
            }
        } while (!success);

        if (!_tracker.HasAsset(id))
        {
            AnsiConsole.MarkupLine("[red]Asset not found.[/]");
            Thread.Sleep(2000);
            return -1;
        }
        
        //Get the asset
        Asset asset;
        using (AssetDB db = new()) 
        {
            asset = _tracker.GetAssetById(id, db);
        }
        
        //Ask for new location
        string currentLocation = asset.Office?.City ?? "";
        //Get locations
        HashSet<Office> locations;
        using (AssetDB db = new())
        {
            locations = _tracker.GetOffices(db);
        }
        
        p = new SelectionPrompt<string>();
        p.Title($"[yellow]Select Office[/]:");
        foreach (Office office in locations)
        {
            p.AddChoice(office.City ?? "");
        }
        p.AddChoice("Quit to main menu");
        
        p.DefaultValue = currentLocation;
        p.EnableSearch();

        string location = AnsiConsole.Prompt(p);
        
        if (location == "Quit to main menu")
        {
            return -1;
        }
        
        
        
        //Get the rest of the input
        string assetType = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]Asset Type[/]:")
                .EnableSearch()
                .AddChoices("Desktop", "Laptop", "Mobile", "Tablet", "Quit to main menu"));
        string modelName = "";
        
        if (assetType == "Quit to main menu")
        {
            return -1;
        }
        
        string brand = "";
        do
        {
            brand = AnsiConsole.Ask<string>($"Brand", asset.Brand ?? "");
            if (brand.Trim().ToLower() == "q")
            {
                return -1;
            }
        } while (brand == "");
        
        do
        {
            var prompt = new TextPrompt<string>("Model name")
                .DefaultValue(asset.ModelName ?? "")
                .ShowDefaultValue();
           modelName = AnsiConsole.Prompt(prompt);
           if (modelName.Trim().ToLower() == "q")
           {
               return -1;
           }
        } while (modelName == "");
        
        
        
        string serialNumber = "";
        do
        {
            serialNumber = AnsiConsole.Ask<string>($"Serial number", asset.SerialNumber ?? "");
            if (serialNumber.Trim().ToLower() == "q")
            {
                return -1;
            }
        } while (serialNumber == "");
        
        string purchaseDateString = "";
        DateTime purchaseDate;
        do
        {
            purchaseDateString = AnsiConsole.Ask<string>($"Purchase Date(yyyy-mm-dd)", asset.PurchaseDate.ToShortDateString() ?? "");
            if (purchaseDateString.Trim().ToLower() == "q")
            {
                return -1;
            }
            
        } while (!DateTime.TryParse(purchaseDateString, out purchaseDate));
        
        string warrantyExpirationDateString = "";
        DateTime warrantyExpirationDate;
        do
        {
            warrantyExpirationDateString = AnsiConsole.Ask<string>($"Warrant expiry Date(yyyy-mm-dd)", asset.WarrantyExpirationDate.ToShortDateString() ?? "");
            if (warrantyExpirationDateString.Trim().ToLower() == "q")
            {
                return -1;
            }
        } while (!DateTime.TryParse(warrantyExpirationDateString, out warrantyExpirationDate));

        string purchasePriceString = "";
        decimal purchasePrice;
        do
        {
            purchasePriceString = AnsiConsole.Ask<string>($"Purchase Price($)", asset.PurchasePrice.ToString());
            if (purchasePriceString.Trim().ToLower() == "q")
            {
                return -1;
            }
        } while (!Decimal.TryParse(purchasePriceString, out purchasePrice));
        
        int ret = _tracker.UpdateAsset(asset.Id, assetType, brand, modelName, purchaseDate, purchasePrice, location, serialNumber,
            0, warrantyExpirationDate);
        AnsiConsole.MarkupLine("[green]ASSET UPDATED![/]");
        Thread.Sleep(2000);
        return ret;
    }

    public override AbstractMenuDisplay? Run()
    {
        while (true)
        {
            Display();
            var i = Interact();
            if (i == -1)
            {
                Context = Parent;
                break;
            }
        }
        return Context;
    }
}