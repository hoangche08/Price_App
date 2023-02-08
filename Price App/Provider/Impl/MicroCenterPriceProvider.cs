using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using Price_App.Model;

namespace Price_App.Provider.Impl;

public interface IMicroCenterPriceProvider : IPriceProvider {}
public class MicroCenterPriceProvider : HtmlWebProvider, IMicroCenterPriceProvider
{

    private const string MicroCenterUrl = "https://www.microcenter.com/search/search_results.aspx?Ntt=";
    
    public async Task<PricedItem> GetPricedItems(string modelId)
    {
        string url = $"{MicroCenterUrl}{HttpUtility.HtmlEncode(modelId)}";

        var document = await LoadDocument(url);

        var priceText = document.DocumentNode
            .Descendants()
            .FirstOrDefault(x => x.Attributes.Any(a=> a.Name == "itemprop" && a.Value == "price"))
            ?.InnerText ?? "";

        decimal? price = null;
        if (decimal.TryParse(priceText.Replace("$", ""), out var priceDecimal)) 
            price = priceDecimal;
        
        var websiteList = document.DocumentNode
            .Descendants()
            .Where(x => x.HasClass("h2"))
            .SelectMany(x=> x.Descendants())
            .Where(x=> x.Attributes["href"] != null)
            .Select(x=> x.Attributes["href"].Value)
            .ToList();

        var websiteUrl = websiteList.Count == 0 ? "" : $"https://www.microcenter.com{websiteList.First()}";
        return new PricedItem("Micro Center", price, websiteUrl);
    }
}