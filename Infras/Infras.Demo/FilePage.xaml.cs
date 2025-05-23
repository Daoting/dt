using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Dt.Cells.Data;
using System;
using Microsoft.UI.Xaml.Navigation;

namespace Infras.Demo
{
    public sealed partial class FilePage : Page
    {
        public FilePage()
        {
            this.InitializeComponent();

        }

        void OnBack(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var file = e.Parameter.ToString();
            var assembly = typeof(FilePage).Assembly;
            using (var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Bag.{file}"))
            {
                if (file.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                    await _excel.OpenExcel(stream);
                else
                    await _excel.OpenXml(stream);
            }
        }
    }
}
