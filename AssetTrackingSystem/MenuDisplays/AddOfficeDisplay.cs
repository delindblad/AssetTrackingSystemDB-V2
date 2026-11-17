using System.Text.RegularExpressions;
using Spectre.Console;

namespace AssetTrackingSystem;
//Lets the user add an office
public class AddOfficeDisplay : AbstractMenuDisplay
{
    AssetTracker _tracker;
    public AddOfficeDisplay(string title, AbstractMenuDisplay? parent, AssetTracker t) : base(title, parent)
    {
        _tracker = t;
    }

    public override void Display()
    {
        
        AnsiConsole.Clear(); 
        AnsiConsole.MarkupLine("[yellow]ADD OFFICE, FOLLOW THE INSTRUCTIONS TO UPDATE AN ASSET, TYPE 'Q' ON AN EMPTY LINE TO QUIT[/]");
        AnsiConsole.WriteLine();
    }

    public override int Interact()
    {
        string city;
        do
        {
            city = AnsiConsole.Ask<string>("City:");
            if (city.Trim().ToLower() == "q")
            {
                return -1;
            }
        }
        while (city == "");

        string currencyMarker;
        do
        {
            currencyMarker = AnsiConsole.Ask<string>("Currency Marker:");
            if (currencyMarker.Trim().ToLower() == "q")
            {
                return -1;
            }
        }
        while (currencyMarker == "");
        int ret = _tracker.AddOffice(city, currencyMarker);
        AnsiConsole.MarkupLine("[green]OFFICE ADDED![/]");
        Thread.Sleep(2000);
        return ret;
        
        return 0;

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