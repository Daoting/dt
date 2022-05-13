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
    }
}
