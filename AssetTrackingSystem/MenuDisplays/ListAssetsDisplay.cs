

namespace AssetTrackingSystem;
//Lets the user view assets sorted by various attributes
public class ListAssetsDisplay : AbstractMenuDisplay
{
    AssetTracker _tracker;
    private int _option = 4;
    public ListAssetsDisplay(string title, AbstractMenuDisplay? parent, AssetTracker t) : base(title, parent)
    {
        _tracker = t;
    }

    public override void Display()
    {
        Console.Clear();
        Console.WriteLine("-------------------------------------");
        Console.WriteLine("ASSET LIST");
        Console.WriteLine("-------------------------------------");
        Console.WriteLine();
        Console.WriteLine("Type 'D' - to sort by purchase date, 'M' - to sort by Model, 'T' - to sort by Type, 'B' - to sort by brand, 'I' - to sort by ID, 'O' - to sort by office, P to sort by price 'Q' - back to main menu");
        Console.WriteLine();
        switch (_option)
        {
            case 1:
                _tracker.ListAssets(OrederBy.PurchaseDate);
                break;
            case 2:
                _tracker.ListAssets(OrederBy.ModelName);
                break;
            case 3:
                _tracker.ListAssets(OrederBy.Type);
                break;
            case 4:
                _tracker.ListAssets(OrederBy.Brand);
                break;
            case 5:
                _tracker.ListAssets(OrederBy.Id);
                break;
            case 6:
                _tracker.ListAssets(OrederBy.OfficeLocation);
                break;
            case 7:
                _tracker.ListAssets(OrederBy.PurchasePrice);
                break;
        }

        

    }

    public override int Interact()
    {
        var key = Console.ReadKey().KeyChar;
        if (key == 'Q' || key == 'q')
        {
            return 0;
        }
        else if (key == 'D' || key == 'd')
        {
            return 1;
        }
        else if (key == 'M' || key == 'm')
        {
            return 2;
        }
        else if (key == 'T' || key == 't')
        {
            return 3;
        }
        else if (key == 'B' || key == 'b')
        {
            return 4;
        }
        else if (key == 'I' || key == 'i')
        {
            return 5;
        }
        else if (key == 'O' || key == 'o')
        {
            return 6;
        }
        else if (key == 'P' || key == 'p')
        {
            return 7;
        }
        else
        {
            return -1;
        }
        
    }

    public override AbstractMenuDisplay? Run()
    {
        while (true)
        {
            Display();
            _option = Interact();
            if (_option == -1)
            {
                continue;
            }
            else if (_option == 0)
            {
                goto exit;
            }
        }
        exit:
        return Context;
    }
}