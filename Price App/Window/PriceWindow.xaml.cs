using Price_App.ViewModel;

namespace Price_App.Window
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class PriceWindow : System.Windows.Window
    {
        public PriceWindow(PriceViewModel priceViewModel)
        {
            DataContext = priceViewModel;
            InitializeComponent();
        }
    }
}