using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;


namespace Dt.Shell
{
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            this.InitializeComponent();
        }

        void OnBlank(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(BlankPage));
        }

        void OnChart(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ChartPage));
        }

        void OnMutiTab(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MutiTabPage));
        }

        void OnSparkline(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SparklinePage));
        }

        void OnLoadFile(object sender, RoutedEventArgs e)
        {
            var file = ((Button)sender).Tag.ToString();
            Frame.Navigate(typeof(FilePage), file);
        }

        void OnPieStacked(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PieStackedChart));
        }
    }
}
