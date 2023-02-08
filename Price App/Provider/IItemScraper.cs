using System.Collections.Generic;
using System.Threading.Tasks;
using Price_App.Model;

namespace Price_App.Provider;

public interface IItemScraper { 
    Task<List<ScrapedItem>> GetScrapedItems(string description);
}