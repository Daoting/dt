using Dt.Charts;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;


namespace Dt.Shell
{
    public sealed partial class PieStackedChart : Page
    {
        public PieStackedChart()
        {
            this.InitializeComponent();

            _chart.Data = new ChartSampleData().GetData(ChartType.PieStacked);
            _chart.ChartType = ChartType.PieStacked;
        }

        void OnBack(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
