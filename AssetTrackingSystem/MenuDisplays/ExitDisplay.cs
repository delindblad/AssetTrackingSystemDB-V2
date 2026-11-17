
namespace AssetTrackingSystem;

//The only purpose of this view is to exit the program when selected
public class ExitDisplay : AbstractMenuDisplay
{
    public ExitDisplay(string title, AbstractMenuDisplay? parent) : base(title, parent)
    {
        
    }

    public override void Display()
    {
        throw new NotImplementedException();
    }

    public override int Interact()
    {
        throw new NotImplementedException();
    }

    public override AbstractMenuDisplay? Run()
    {
        Environment.Exit(0);
        return null;
    }
}