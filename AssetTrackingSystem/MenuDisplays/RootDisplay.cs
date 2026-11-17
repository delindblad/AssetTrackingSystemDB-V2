using System.Text.Json;
using Spectre.Console;

namespace AssetTrackingSystem;
//This is the main menu
public class RootDisplay : AbstractMenuDisplay
{
    AssetTracker _tracker;
    public RootDisplay(string title, AssetTracker tracker) : base(title, null)
    {
        
        _tracker = tracker;
    }

    public override void Display()
    {
        
        AnsiConsole.Clear();
        AnsiConsole.Write(new FigletText("AssetTracker"));
        AnsiConsole.MarkupLine($"[yellow]{Title}[/]");
        AnsiConsole.WriteLine("");
        
    }

    public override int Interact()
    {
        SelectionPrompt<AbstractMenuDisplay> p = new SelectionPrompt<AbstractMenuDisplay>();
        p.Title = "Select Option:";
        foreach (var page in ChildPages)
        {
            p.AddChoice(page);
        }

        var option = AnsiConsole.Prompt(p);

        Context = option;
        

        return 0;
    }

    public override AbstractMenuDisplay? Run()
    {
        Display();
        if (Interact() == -1)
        {   
            AnsiConsole.MarkupLine("\n[yellow]Exiting[/]");
            Environment.Exit(0);
            return null;
        }
        return Context;
    }
}