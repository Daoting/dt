using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Dt.Cells.Data;
using System;

namespace Dt.Shell
{
    public sealed partial class BlankPage : Page
    {
        public BlankPage()
        {
            this.InitializeComponent();
            //var st = _excel.ActiveSheet;
            //if (st != null)
            //    st.RowHeader.IsVisible = !st.RowHeader.IsVisible;
        }

        void OnBack(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        void OnToggleRowHearder(object sender, RoutedEventArgs e)
        {
            var st = _excel.ActiveSheet;
            if (st != null)
                st.RowHeader.IsVisible = !st.RowHeader.IsVisible;
        }

        void OnToggleColHearder(object sender, RoutedEventArgs e)
        {
            var st = _excel.ActiveSheet;
            if (st != null)
                st.ColumnHeader.IsVisible = !st.ColumnHeader.IsVisible;
        }

        void OnToggleGridLine(object sender, RoutedEventArgs e)
        {
            _excel.ShowGridLine = !_excel.ShowGridLine;
            //_excel.AutoRefresh= true;
            //var st = _excel.ActiveSheet;
            //if (st != null)
            //    st.ShowGridLine = !st.ShowGridLine;
        }
    }
}
