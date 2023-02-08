using System.Collections.ObjectModel;

namespace Price_App.Model;

public class ScrapedItem
{
    public ScrapedItem(string modelId, string description)
    {
        ModelId = modelId;
        Description = description;
    }

    public string ModelId { get; set; }
    public string Description { get; set; }
    
    //public ObservableCollection<PricedItem> PricedItems { get; } = new();

    public override string ToString()
    {
        return $"{ModelId} - {Description}";
    }
}