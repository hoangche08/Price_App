using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Documents;
using HtmlAgilityPack;
using Price_App.Model;

namespace Price_App.Provider.Impl;

public interface INewEggPriceProvider : IPriceProvider { }
public class NewEggPriceProvider : HtmlWebProvider, INewEggPriceProvider
{
    private const string newEggUrl = "https://www.newegg.com/p/pl?d=";


    public async Task<PricedItem> GetPricedItems(string modelId)
    {
        string url = $"{newEggUrl}{HttpUtility.HtmlEncode(modelId)}";
        var document = await LoadDocument(url);
        
        var priceText = string.Join("", new List<HtmlNode?>
            {
                document.DocumentNode
                    .Descendants()
                    .Where(x => x.HasClass("price"))
                    .SelectMany(x => x.Descendants())
                    .FirstOrDefault(x => x.HasClass("price-current"))
            }.Where(x=> x != null)
            .SelectMany(x => x!.Descendants())
            .Where(x => x.Name is "strong" or "sup").Select(x => x.InnerText ?? ""));

        decimal? price = null;
        if (decimal.TryParse(priceText.Replace("$", ""), out var priceDecimal))
            price = priceDecimal;

        var websiteList = document.DocumentNode
           .Descendants()
           .Where(x => x.HasClass("price-current"))
           .SelectMany(x => x.Descendants())
           .Where(x => x.Attributes["href"] != null)
           .Select(x => x.Attributes["href"].Value)
           .ToList();

        var websiteUrl = websiteList.Count == 0 ? "" : $"{websiteList.First()}";
        return new PricedItem("NewEgg", price, websiteUrl);


    }
}