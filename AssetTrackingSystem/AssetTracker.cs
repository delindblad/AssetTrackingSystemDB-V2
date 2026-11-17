using Microsoft.EntityFrameworkCore; // To use ExecuteUpdate, ExecuteDelete.
using Microsoft.EntityFrameworkCore.ChangeTracking; // To use EntityEntry<T>.

using System.Collections;
using System.Globalization;
using System.Text.Json;
using freecurrencyapi;
using Spectre.Console;

namespace AssetTrackingSystem;

//Class provides an interface for tracking assets
public class AssetTracker
{

    private Hashtable CurrencyData { get; set; } = new();
    


    //Used to check asset age
    readonly TimeSpan _yellowSpan = new TimeSpan(913, 0, 0, 0, 0); //2.5 years
    readonly TimeSpan _redSpan = new TimeSpan(1004, 0, 0, 0, 0); //2.75 years

    
   

    //Updates or adds a new asset
    public int UpdateAsset(int id, string type, string brand, string model, DateTime purchaseDate, decimal priceUsd,
        string officeString, string serialNumber, int employeeId,DateTime warrantyExpirationDate)
    {
        using (AssetDB db = new()){
        
            // Firs lets determine if we are creating a new asset or updating an existing one. All valid id's are >= 1.
            // If id is <= 0, we are creating a new asset
            bool isNewAsset = id <= 0;
            //Get offices
            HashSet<Office> offices = GetOffices(db);
            //We do not allow new offices to be created on the fly
            if (offices.Count  <= 0)
            {
                AnsiConsole.MarkupLineInterpolated($"[red]No offices found.[/]");
                return -1;
            }
            //Get the office if it exists, otherwise return -2
            Office? office = OfficeExists(officeString, offices);
            if (office is null)
            {
                AnsiConsole.MarkupLineInterpolated($"[red]Office {officeString} does not exist.[/]");
                return -2;
            }
        
            //This will be used as prefix when printing local price
            string? prefix = office.Currency;
            //Get the local price
            decimal priceLocal = ConvertCurrency(prefix, priceUsd);
        
            //If the asset is new, we need to create a new asset
            Asset? asset;
            
            if (isNewAsset)
            {
                asset = new()
                {
                    AssetType = type,
                    Brand = brand,
                    ModelName = model,
                    PurchaseDate = purchaseDate,
                    PurchasePrice = priceUsd,
                    PriceLocal = priceLocal,
                    Office = office,
                    SerialNumber = serialNumber,
                    WarrantyExpirationDate = warrantyExpirationDate
                };
                EntityEntry<Asset> entity =
                    db.Assets?.Add(asset) ?? throw new ArgumentNullException("db.Assets?.Add(a)");
                AnsiConsole.MarkupLineInterpolated($"[yellow]State: {entity.State}, Asset ID: {asset.Id}[/]");
                int affected = db.SaveChanges();
                AnsiConsole.MarkupLineInterpolated($"[yellow]State: {entity.State}, Asset ID: {asset.Id}[/]");
                AnsiConsole.MarkupLineInterpolated($"[yellow]Affected rows: {affected} , Asset ID: {asset.Id}[/]");
            }
            //Otherwise we need to get the asset
            else
            {
                asset = GetAssetById(id, db);
                //Update the values unless they are empty strings,
                //if nothing new is added, keep the old value
                asset.AssetType = type != "" ? type : asset.AssetType;
                asset.Brand = brand != "" ? brand : asset.Brand;
                asset.ModelName = model != "" ? model : asset.ModelName;
                asset.PurchaseDate = purchaseDate != asset.PurchaseDate ? purchaseDate : asset.PurchaseDate;
                asset.PurchasePrice = priceUsd != 0 ? priceUsd : asset.PurchasePrice;
                asset.PriceLocal = priceLocal;
                asset.Office = office;
                asset.SerialNumber = serialNumber != "" ? serialNumber : asset.SerialNumber;
                //Employee = 
                asset.WarrantyExpirationDate =
                    warrantyExpirationDate != asset.WarrantyExpirationDate ? warrantyExpirationDate : asset.WarrantyExpirationDate;

                int affected = db.SaveChanges();
                AnsiConsole.MarkupLineInterpolated($"[yellow]Affected rows: {affected} , Asset ID: {asset.Id}[/]");

            }

            return 0;
            
        }

        //Utility function,checks i an office name exists in an array, returns null if not,
        //otherwise returns the obeject
        Office? OfficeExists(string office , HashSet<Office> offices) 
        {
            foreach (Office o in offices)
            {
                if (o.City == office)
                {
                    return o;
                }
            }

            return null;
        }
    }
    
    
    //Removes asset from list
    public void RemoveAsset(int id)
    {
        using (AssetDB db = new())
        {
            IQueryable<Asset>? assets = db.Assets?.Where(
                a => a.Id == id);
            if (assets is null || !assets.Any())
            {
                AnsiConsole.MarkupLineInterpolated($"[yellow]No asset with ID {id}[/]");
            }
            else
            {
                if (db.Assets is null) return;
                db.Assets.RemoveRange(assets);
            }
            int affected = db.SaveChanges();
            AnsiConsole.MarkupLineInterpolated($"[yellow]{affected} rows affected[/]");
        }
    }

    //List offices
    public void ListOffices()
    {
        using (AssetDB db = new())
        {
            var offices = db.Offices;
            var officesById =
                from o in offices
                select o;
            if (offices is null || !offices.Any())
            {
                AnsiConsole.MarkupLineInterpolated($"[red]No offices found.[/]");
                return;
            }
            var table = new Table()
                .RoundedBorder()
                .BorderColor(Color.Grey)
                .Title($"[green bold]Offices sorted by ID[/]");
            table.AddColumn($"[white bold]ID[/]");
            table.AddColumn("City");
            table.AddColumn("Currency");
            foreach (Office o in officesById )
            {
                table.AddRow(o.Id.ToString(), o.City ?? "", o.Currency ?? "");
            }
            AnsiConsole.Write(table);
        }
    }
    
    //Returns the offices in a hashset
    public HashSet<Office> GetOffices(AssetDB db)
    {

        var offices = db.Offices;
        var officesById =
            from o in offices
            select o;
        return officesById.ToHashSet();
    }


    //Add a new office
    public int AddOffice(string city, string currency)
    {
        using (AssetDB db = new())
        {
            Office office = new Office()
            {
                City = city,
                Currency = currency
            };
            
            EntityEntry<Office> entity = db.Offices?.Add(office) ?? 
                                         throw new ArgumentNullException("db.Assets?.Add(office)");
            AnsiConsole.MarkupLineInterpolated($"[yellow]State: {entity.State}, Asset ID: {office.Id}[/]");
            int affected = db.SaveChanges();
            AnsiConsole.MarkupLineInterpolated($"[yellow]State: {entity.State}, Asset ID: {office.Id}[/]");
            AnsiConsole.MarkupLineInterpolated($"[yellow]Affected rows: {affected} , Asset ID: {office.Id}[/]");
            return 0;

        }

    }


    //Check if asset exists
    public bool HasAsset(int id)
    {
        using (AssetDB db = new())
        {
            return db.Assets?.Any(a => a.Id == id) ?? false;
        }
    }
    
    //List assets
    public void ListAssets(OrederBy order)
    {
        using (AssetDB db = new())
        {
            IQueryable<Asset>? assets = db.Assets?.Include(a => a.Office);
            IQueryable<Office>? locations = db.Offices;
            if (assets is null || !assets.Any())
            {
                AnsiConsole.MarkupLineInterpolated($"[red]No assets found.[/]");
                return;
            }
            //LINQ queries
            string selectedOrder = "BRAND";
            IOrderedQueryable<Asset>? sortedData;

            var offices =
                from t in assets
                select t.Office;
            
            
            switch (order)
            {
                case OrederBy.Brand:
                    sortedData =
                        from t in assets
                        orderby t.AssetType
                        select t;
                    break;
                case OrederBy.ModelName:
                    sortedData =
                        from t in assets
                        orderby t.ModelName
                        select t;
                    break;
                case OrederBy.Id:
                    sortedData =
                        from t in assets
                        orderby t.Id
                        select t;
                    selectedOrder = "ID";
                    break;
                case OrederBy.PurchaseDate:
                    sortedData =
                        from t in assets
                        orderby t.PurchaseDate descending 
                        select t;
                    selectedOrder = "PURCHASE DATE";
                    break;
                case OrederBy.PurchasePrice:
                    sortedData =
                        from t in assets
                        orderby t.PurchasePrice
                        select t;
                    selectedOrder = "PURCHASE PRICE";
                    break;
                case OrederBy.WarrantyExpiration:
                    sortedData =
                        from t in assets
                        orderby t.WarrantyExpirationDate descending
                        select t;
                    selectedOrder = "WARRANTY EXPIRY DATE";
                    break;
                    case OrederBy.OfficeLocation:
                    sortedData =
                        from t in assets
                        orderby t.Office!.City
                        select t;
                    selectedOrder = "OFFICE";
                    break;
                default:
                    sortedData =
                        from t in assets
                        orderby t.Brand
                        select t;
                    break;
                
            }
            
            

            var table = new Table()
                .RoundedBorder()
                .BorderColor(Color.Grey)
                .Title($"[yellow bold]ASSETS ORDERED BY {selectedOrder}[/]");
            table.AddColumn("ID");
            table.AddColumn("Brand");
            table.AddColumn("Model");
            table.AddColumn("Purchase Date");
            table.AddColumn("Price (USD)");
            table.AddColumn("Price (Local)");
            table.AddColumn("Warranty Expiry Date");
            table.AddColumn("Office");

            
            //Iterate over the sorted data
            foreach (var asset in sortedData)
            {
                
                //Get currency prefix
                string? localPrefix = asset.Office?.Currency;
                //Get time since purchase
                TimeSpan timeSincePurchase = DateTime.Today.Subtract(asset.PurchaseDate);
                //Print with color depending on time since purchase
                string color = "green";
                if (_redSpan.Subtract(timeSincePurchase).Days < 0)
                {
                    color = "red";
                }
                else if (_yellowSpan.Subtract(timeSincePurchase).Days < 0)
                {
                    color = "yellow";
                }
                table.AddRow($"[{color}]{asset.Id.ToString()} [/]", $"[{color}]{asset.Brand} [/]", $"[{color}]{asset.ModelName} [/]" , $"[{color}]{asset.PurchaseDate.ToShortDateString()} [/]" ,
                    $"[{color}]{asset.PurchasePrice.ToString()} [/]" , $"[{color}]{asset.Office?.Currency}{asset.PriceLocal.ToString()} [/]", $"[{color}]{asset.WarrantyExpirationDate.ToShortDateString()} [/]" ,
                    $"[{color}]{asset.Office} [/]"  ?? "");
                
                
                
                
            }
            AnsiConsole.Write(table);
        }

    }
    //Print result og search string
    public void SearchAssets(string query, FilterBy filter)
    {
        using (AssetDB db = new())
        {
            IQueryable<Asset>? assets = db.Assets?.Include(a => a.Office);
            if (assets is null || !assets.Any())
            {
                AnsiConsole.MarkupLineInterpolated($"[red]No assets found.[/]");
                return;
            }
            //LINQ queries
            IQueryable<Asset>? filteredData;

            switch (filter)
            {
                case FilterBy.AssetType:
                    filteredData = from t in assets
                        where t.AssetType!.ToLower().Contains(query)
                        select t; 
                    break;
                case FilterBy.Brand:
                    filteredData = from t in assets
                        where t.Brand!.ToLower().Contains(query)
                        select t; 
                    break;
                case FilterBy.ModelName:
                    filteredData = from t in assets
                        where t.ModelName!.ToLower().Contains(query)
                        select t; 
                    break;
                case FilterBy.Location:
                    filteredData = from t in assets
                        where t.Office!.City!.ToLower().Contains(query)
                        select t; 
                    break;
                case FilterBy.PurchaseDate:
                    filteredData = from t in assets
                        where t.PurchaseDate.ToString(CultureInfo.CurrentCulture).ToLower().Contains(query)
                        select t; 
                    break;
                default:
                    filteredData = from t in assets
                        where t.ModelName!.ToLower().Contains(query)
                        select t; 
                    break;
                
            }
            var table = new Table()
                .RoundedBorder()
                .BorderColor(Color.Grey)
                .Title($"[green bold]SEARCH RESULTS[/]");
            table.AddColumn("ID");
            table.AddColumn("Brand");
            table.AddColumn("Model");
            table.AddColumn("Purchase Date");
            table.AddColumn("Price (USD)");
            table.AddColumn("Price (Local)");
            table.AddColumn("Warranty Expiry Date");
            table.AddColumn("Office");

            
            //Iterate over the sorted data
            foreach (var asset in filteredData)
            {
                
                //Get time since purchase
                TimeSpan timeSincePurchase = DateTime.Today.Subtract(asset.PurchaseDate);
                //Print with color depending on time since purchase
                string color = "green";
                if (_redSpan.Subtract(timeSincePurchase).Days < 0)
                {
                    color = "red";
                }
                else if (_yellowSpan.Subtract(timeSincePurchase).Days < 0)
                {
                    color = "yellow";
                }
                table.AddRow($"[{color}]{asset.Id.ToString()} [/]", $"[{color}]{asset.Brand} [/]", $"[{color}]{asset.ModelName} [/]" , $"[{color}]{asset.PurchaseDate.ToShortDateString()} [/]" ,
                    $"[{color}]{asset.PurchasePrice.ToString(CultureInfo.CurrentCulture)} [/]" , $"[{color}]{asset.Office?.Currency}{asset.PriceLocal.ToString()} [/]", $"[{color}]{asset.WarrantyExpirationDate.ToShortDateString()} [/]" ,
                    $"[{color}]{asset.Office} [/]"  ?? "");
                
                
                
                
            }
            AnsiConsole.Write(table);
        }

    }
    
    //Get asset list
    public List<string>? GetAssetList(AssetDB db)
    {
        IQueryable<Asset>? assets = db.Assets?.Include(a => a.Office);
        IQueryable<Office>? locations = db.Offices;
        if (assets is null || !assets.Any())
        {

            return null;
        }
        IOrderedQueryable<Asset> sortedById =
            from t in assets
            orderby t.Id
            select t;
        List<string> assetList = new();
        foreach (Asset a in sortedById)
        {
            assetList.Add($"{a.Id} -- {a.Office?.City ?? ""}" +
                          $" , Brand: {a.Brand}, Type: {a.AssetType}, Model:{a.ModelName}" +
                          $" , SN: {a.SerialNumber}, PPrice: {a.PurchasePrice}, PDate: {a.PurchaseDate}" +
                          $" , WEDate: {a.WarrantyExpirationDate}");
        }
        return assetList;
    }
    
    //Gets asset by ID
    public Asset GetAssetById(int id, AssetDB? db)
    {
        //If db is null, create a new instance of AssetDB
        Asset? a;
        //If we are using an existing database instance
  
        IQueryable<Asset>? assets = db.Assets?.Include(a => a.Office);
        if (assets is null || !assets.Any())
        {
            AnsiConsole.MarkupLineInterpolated($"[red]No assets found.[/]");
            return null!;
                
        }

        a = assets.FirstOrDefault(a => a.Id == id);
        if(a is null)
        {
            AnsiConsole.MarkupLineInterpolated($"[red]No such asset.[/]");
            return null!;
        }



        return a;

    }
    
    //Deletes an office
    public int DeleteOffice(int id)
    {
        using (AssetDB db = new())
        {
            IQueryable<Office>? offices = db.Offices?.Where(o => o.Id.Equals(id));
            if (offices is  null || !offices.Any())
            {
                AnsiConsole.MarkupLine($"[red] NO OFFICE WITH THAT ID FOUND[/]");
                Thread.Sleep(2000);
                
                return 0;
            }
            else
            {
                if (db.Offices is null) return 0;
                db.Offices.RemoveRange(offices);
             
            }

            try
            {
                AnsiConsole.Markup($"[green]{db.SaveChanges()}[/] OFFICE WERE DELETED");
            }
            catch (Exception e)
            {
                AnsiConsole.MarkupLine($"[red]CAN'T DELETE OFFICE WITH ASSOCIATED ASSETS[/]");
                Thread.Sleep(2000);
                return -1;
            }
            Thread.Sleep(2000);

            return 0;

        }
    }
    
    //Deletes an asset
    public int DeleteAsset(int id)
    {
        using (AssetDB db = new())
        {
            IQueryable<Asset>? assets = db.Assets?.Where(o => o.Id.Equals(id));
            if (assets is  null || !assets.Any())
            {
                AnsiConsole.MarkupLine($"[red] NO ASSET WITH THAT ID FOUND[/]");
                Thread.Sleep(2000);
                return -1;
            }
            else
            {
                if (db.Offices is null) return 0;
                db.Assets?.RemoveRange(assets);
            }
            AnsiConsole.Markup($"[green]{db.SaveChanges()} ASSET DELETED[/" +
                               $"]");
            Thread.Sleep(2000);
            return 0;

        }
    }

    
    //Updates the currency data
    public void UpdateCurrencyData()
    {   decimal d;
        //Get latest currency data
        var fx = new Freecurrencyapi("fca_live_GyuUHAV7ziIvpoHqYrSx5lGpSgmmme8xkXbOepyN");
        var latest = fx.Latest();
        var root = JsonDocument.Parse(latest).RootElement;
        var data = root.GetProperty("data");
        CurrencyData.Clear();
        CurrencyData.Add("SEK", data.GetProperty("SEK").GetDecimal());
        CurrencyData.Add("DKK", data.GetProperty("DKK").GetDecimal());
        CurrencyData.Add("€", data.GetProperty("EUR").GetDecimal());
        CurrencyData.Add("£", data.GetProperty("GBP").GetDecimal());
        CurrencyData.Add("¥", data.GetProperty("JPY").GetDecimal());
        CurrencyData.Add("$", data.GetProperty("USD").GetDecimal());
    }
 
    //Convert currency from $ to local currency
   private decimal ConvertCurrency(string? currency, decimal amount)
    {
        
        var value =  CurrencyData[currency?.Trim() ?? ""];
        if (value == null)
        {
            return 0;
        }
        return (decimal) value * amount;
    }
}