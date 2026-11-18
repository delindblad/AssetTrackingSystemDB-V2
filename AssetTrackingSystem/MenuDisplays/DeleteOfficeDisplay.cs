using AssetTrackingSystem;
using Spectre.Console;
//Lets the user delete an office
namespace OfficeTrackingSystem;

public class DeleteOfficeDisplay : AbstractMenuDisplay
{
    private AssetTracker _tracker;
    public DeleteOfficeDisplay(string title, AbstractMenuDisplay? parent, AssetTracker t) : base(title, parent)
    {
        _tracker = t;
    }

    public override void Display()
    {
        AnsiConsole.Markup($"[yellow]DELETE OFFICE[/]");
        AnsiConsole.WriteLine("");
    }

    public override int Interact()
    {
        string idString;
        int id;
        do
        {
            idString = AnsiConsole.Ask<string>("Enter Office ID: ");
            if (idString.ToLower().Trim() == "q")
            {
                return -1;
            }
        } while (!int.TryParse(idString, out id));

        
        return _tracker.DeleteOffice(id);
    }

    public override AbstractMenuDisplay? Run()
    {
        Display();
        Interact();
        return Parent;
    }
}