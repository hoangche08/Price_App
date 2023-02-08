using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Documents;
using HtmlAgilityPack;
using Price_App.Model;

namespace Price_App.Provider.Impl;

public interface IBestBuyProvider : IItemScraper, IPriceProvider { }
public class BestBuyProvider : HtmlWebProvider, IBestBuyProvider
{
    private const string BestBuySearchUrl = "https://www.bestbuy.com/site/searchpage.jsp?st=";

    private static string GetSearchUrl(string searchTerm) => $"{BestBuySearchUrl}{HttpUtility.HtmlEncode(searchTerm)}"; 
    
    public async Task<List<ScrapedItem>> GetScrapedItems(string description)
    {
        var url = GetSearchUrl(description);
        var documentTask = LoadDocument(url);

        var document = await documentTask;

        var scrapedItems = document.DocumentNode
            .Descendants()
            .Where(x => x.HasClass("information"))
            .Select(TransformNode);

        ScrapedItem TransformNode(HtmlNode node)
        {
            var modelId = GetModelId(node);
            var itemDescription = GetDescription(node);
            var item = new ScrapedItem(modelId, itemDescription);
            return item;
        }

        string GetDescription(HtmlNode node)
        {
            var itemDescription = node.Descendants()
                .Where(x => x.HasClass("sku-title"))
                .SelectMany(x=> x.Descendants())
                .FirstOrDefault()?.InnerHtml ?? "";

            return itemDescription;
        }

        string GetModelId(HtmlNode node)
        {
            return node
                .Descendants()
                .Where(x=> x.HasClass("variation-info"))
                .SelectMany(x=> x.Descendants())
                .Where(x=> x.HasClass("sku-model"))
                .SelectMany(x=> x.Descendants())
                .Where(x=> x.HasClass("sku-attribute-title"))
                .SelectMany(x=> x.Descendants())
                .FirstOrDefault(x => x.HasClass("sku-value"))?.InnerText ?? "";
        }

        return scrapedItems.ToList();
    }

    public async Task<PricedItem> GetPricedItems(string modelId)
    {
        var url = GetSearchUrl(modelId);
        var documentTask = LoadDocument(url);

        var document = await documentTask;

        var priceText = document.DocumentNode
            .Descendants()
            .Where(x => x.HasClass("price-block"))
            .Select(FindDescendentFirst)
            .FirstOrDefault(x => x != null)?.InnerHtml ?? "";

        priceText = priceText.Replace("$", "");

        decimal? price = null;
        if (decimal.TryParse(priceText, out var priceDecimal)) price = priceDecimal;


        HtmlNode? FindDescendentFirst(HtmlNode? node)
        {
            if (node == null) return null;

            return node.Name == "span" ? node : FindDescendentFirst(node.Descendants().FirstOrDefault(x => x.Name != "link"));
        }

        var websites = document.DocumentNode
            .Descendants()
            .Where(x => x.HasClass("information"))
            .SelectMany(x=> x.Descendants())
            .Where(x=> x.HasClass("sku-title"))
            .SelectMany(x=> x.Descendants())
            .Where(x => x.Attributes["href"] != null)
            .Select(x => x.Attributes["href"].Value)
            .ToList();

        var websiteUrl = websites.Count == 0 ? "" : $"https://bestbuy.com{websites.First()}";

        return new PricedItem("Best Buy", price, websiteUrl);
    }
}