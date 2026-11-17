using Spectre.Console;

namespace AssetTrackingSystem;
//Lets the user search for assets
public class SearchAssetsDisplay : AbstractMenuDisplay
{
    AssetTracker _tracker;
    private int option = 4;
    public SearchAssetsDisplay(string title, AbstractMenuDisplay? parent, AssetTracker t) : base(title, parent)
    {
        _tracker = t;
    }

    public override void Display()
    {
        Console.Clear();
        Console.WriteLine("-------------------------------------");
        Console.WriteLine("ASSET SEARCH");
        Console.WriteLine("-------------------------------------");
        Console.WriteLine();
        Console.WriteLine();
    }

    public override int Interact()
    {
        string option = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]Search by:[/]:")
                .AddChoices("Asset type", "Brand", "Model", "Purchase date", "Office", "Quit to main menu"));
        if (option == "Quit to main menu")
        {
            return -1;
        }
        
        string query;
                
        do
        {
           query  = AnsiConsole.Ask<string>($"{option}:");
           if (query.ToLower().Trim() == "q")
           {
               return -1;
           }
        }while (query == "");
        

        switch (option)
        {
            case "Asset type":
                _tracker.SearchAssets(query, FilterBy.AssetType);
                break;
            case "Brand":
                _tracker.SearchAssets(query, FilterBy.Brand);
                break;
            case "Model":
                _tracker.SearchAssets(query, FilterBy.ModelName);
                break;
            case "Purchase date":
                _tracker.SearchAssets(query, FilterBy.PurchaseDate);
                break;
            case "Office":
               _tracker.SearchAssets(query, FilterBy.Location);
                break;
        }

        return 0;

    }

    public override AbstractMenuDisplay? Run()
    {
        while (true)
        {
            Display();
            option = Interact();
            if (option == -1)
            {
                return Parent;
            }
            else if (option == 0)
            {
                Console.ReadKey();
                return Parent;
            }
        }

    }
}