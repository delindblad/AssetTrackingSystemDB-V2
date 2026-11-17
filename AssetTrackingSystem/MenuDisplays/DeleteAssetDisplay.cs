using Spectre.Console;

namespace AssetTrackingSystem;
//Lets the user delete an asset
public class DeleteAssetDisplay : AbstractMenuDisplay
{
    private AssetTracker _tracker;
    public DeleteAssetDisplay(string title, AbstractMenuDisplay? parent, AssetTracker t) : base(title, parent)
    {
        _tracker = t;
    }

    public override void Display()
    {
        AnsiConsole.Markup($"[yellow]DELETE ASSET[/]");
        AnsiConsole.WriteLine("");
    }

    public override int Interact()
    {
        string idString;
        int id;
        do
        { 
            idString = AnsiConsole.Ask<string>("Enter asset ID: ");
        } while(!int.TryParse(idString, out id));
        
        if (idString.ToLower().Trim() == "q")
        {
            return -1;
        }
        
        return _tracker.DeleteAsset(id);
    }

    public override AbstractMenuDisplay? Run()
    {
        Display();
        Interact();
        return Parent;
    }
}