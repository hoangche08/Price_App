using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Price_App.Helper;
using Price_App.Model;
using Price_App.Service;

namespace Price_App.ViewModel;

public class PriceViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private readonly IPriceService _priceService;

    public PriceViewModel(IPriceService priceService)
    {
        _priceService = priceService;
    }

    public ICommand SearchWebsiteCommand => CommandHelper.CreateAsyncCommand(OnSearchWebsite);
    public ICommand SelectItemCommand => CommandHelper.CreateAsyncCommand(OnSelectItem);

    public ICommand OpenUrlCommand => CommandHelper.CreateAsyncCommand<PricedItem>(OnURLOpen);

    public ObservableCollection<ScrapedItem> ScrapedItems { get; } = new();

    public ObservableCollection<PricedItem> PricedItems { get; } = new();

    private string _description = "";
    public string Description
    {
        get => _description;
        set
        {
            _description = value;
            OnPropertyChanged();
        }
    }

    public ScrapedItem? SelectedScrapedItem { get; set; }
    public ComboBoxItem SelectedSortingMethod { get; set; }

    private async Task OnSearchWebsite()
    {
        ScrapedItems.Clear();


        var scrapedItems = await _priceService.GetScrapedItems(Description);
        // Show results

        foreach (var x in scrapedItems)
            ScrapedItems.Add(x);
        
    }

    private async Task OnSelectItem()
    {
        if (SelectedScrapedItem == null) return;
        
        PricedItems.Clear();
        var pricedItems = await _priceService.GetPricedItems(SelectedScrapedItem);

        

        switch (SelectedSortingMethod.Content)
        {
            case "Provider":
                pricedItems = pricedItems.OrderBy(x => x.Provider).ToList();
                break;
            case "Price":
                pricedItems = pricedItems.OrderBy(x => x.Price ?? decimal.MaxValue).ToList();
                break;
        }
        
        pricedItems.ForEach(x => 
            {
                if(x.Price != null)
                    PricedItems.Add(x);
            });
    }
    
    private static Task OnURLOpen(PricedItem item)
    {
        if (string.IsNullOrEmpty(item.WebSiteUrl)) return Task.CompletedTask;
        
        var psi = new ProcessStartInfo { FileName = item.WebSiteUrl, UseShellExecute = true };
        
        Process.Start(psi);
        
        return Task.CompletedTask;
    }
}