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

        void OnLoadFile(object sender, RoutedEventArgs e)
        {
            var file = ((Button)sender).Tag.ToString();
            Frame.Navigate(typeof(FilePage), file);
        }
    }
}
