
//Abstract base class for menu pages
public abstract class AbstractMenuDisplay
{
    public AbstractMenuDisplay? Context { get; set; } = null;
    public AbstractMenuDisplay? Parent { get; set; }
    public List<AbstractMenuDisplay> ChildPages { get; set; }
    public string Title { get; set; }

    public AbstractMenuDisplay(string title, AbstractMenuDisplay? parent)
    {
        Title = title;
        ChildPages = new List<AbstractMenuDisplay>();
        Context = Parent = parent;
    
        
    }

    public override string ToString()
    {
        return Title;
    }
    
    //Adds child pages
    public void AddChildPage(AbstractMenuDisplay display)
    {
        display.Parent = this;
        ChildPages.Add(display);
    }

    //Displays information when loading page
    public abstract void Display();
    //Handle interaction here, returns int signaling result
    public abstract int Interact();
    //Runs page, returns new context
    public abstract AbstractMenuDisplay? Run();
}