using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Price_App.Provider;
using Price_App.Provider.Impl;
using Price_App.Service;
using Price_App.ViewModel;
using Price_App.Window;

namespace Price_App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IHost? AppHost { get; private set; }

        public App()
        {
            AppHost = Host
                .CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<PriceWindow>();
                    services.AddSingleton<PriceViewModel>();
                    services.AddSingleton<IPriceService, PriceService>();
                    services.AddSingleton<IBestBuyProvider, BestBuyProvider>();
                    services.AddSingleton<IMicroCenterPriceProvider, MicroCenterPriceProvider>();
                    services.AddSingleton<INewEggPriceProvider, NewEggPriceProvider>();
                })
                .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost!.StartAsync();

            var priceWindow = AppHost.Services.GetRequiredService<PriceWindow>();
            priceWindow.Show();
            
            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost!.StopAsync();
            base.OnExit(e);
        }
    }
}