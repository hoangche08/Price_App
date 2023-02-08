using System.Threading.Tasks;
using Price_App.Model;

namespace Price_App.Provider;

public interface IPriceProvider { 
    // Must return a item regardless of whether or not there is a price found.
    Task<PricedItem> GetPricedItems(string modelId);
}