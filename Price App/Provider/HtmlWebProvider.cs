using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Price_App.Provider;

public class HtmlWebProvider
{
    private readonly HtmlWeb _htmlWeb = new HtmlWeb();

    protected Task<HtmlDocument> LoadDocument(string url) => _htmlWeb.LoadFromWebAsync(url);
}