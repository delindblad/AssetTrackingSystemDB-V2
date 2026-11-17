using Spectre.Console;

namespace AssetTrackingSystem;
//Lets the user List the offices
public class ListOfficesDisplay : AbstractMenuDisplay
{
    AssetTracker _tracker;
    public ListOfficesDisplay(string title, AbstractMenuDisplay? parent, AssetTracker t) : base(title, parent)
    {
        _tracker = t;
    }

    public override void Display()
    {
        AnsiConsole.Clear();
        AnsiConsole.WriteLine("-------------------------------------");
        AnsiConsole.WriteLine("OFFICE LIST, TYPE 'Q' TO RETURN TO MAIN MENU");
        AnsiConsole.WriteLine("-------------------------------------"); 
        AnsiConsole.WriteLine();
        _tracker.ListOffices();
        
    }

    public override int Interact()
    {
        var key = Console.ReadKey().KeyChar;
        while (true)
        {
            if (key == 'q'|| key == 'Q')
            {
                return -1;
            }

        }
    }


    public override AbstractMenuDisplay? Run()
    {
        while (true)
        {
            Display();
            int option = Interact();
            if (option == -1)
            {
                return Context;
            }

        }

    }
}