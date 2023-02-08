namespace Price_App.Model;

public class PricedItem
{
    public PricedItem(string provider, decimal? price, string webSiteUrl)
    {
        Provider = provider;
        Price = price;
        WebSiteUrl = webSiteUrl;
    }

    public string Provider { get; set; }
    public decimal? Price { get; set; }
    public string WebSiteUrl { get; }
}