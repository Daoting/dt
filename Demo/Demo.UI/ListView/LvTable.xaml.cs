#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Dt.Base.ListView;
#endregion

namespace Demo.UI
{
    public partial class LvTable : Win
    {
        public LvTable()
        {
            InitializeComponent();

            _lv.View = Resources["普通"];
            _lv.GroupName = "bumen";
            _lv.Data = SampleData.CreatePersonsTbl(100);
        }

        void OnLoadData(object sender, RoutedEventArgs e)
        {
            _lv.Data = SampleData.CreatePersonsTbl(int.Parse(((Button)sender).Tag.ToString()));
        }

        void OnLoadNull(object sender, RoutedEventArgs e)
        {
            _lv.Data = null;
        }

        void OnGroup(object sender, RoutedEventArgs e)
        {
            _lv.GroupName = "bumen";
        }

        void OnDelGroup(object sender, RoutedEventArgs e)
        {
            _lv.GroupName = null;
        }

        void OnAutoHeight(object sender, RoutedEventArgs e)
        {
            _lv.ItemHeight = double.NaN;
        }

        void OnFormView(object sender, RoutedEventArgs e)
        {
            _lv.ViewMode = ViewMode.List;
        }

        void OnSelectNull(object sender, RoutedEventArgs e)
        {
            _lv.SelectedItem = null;
        }

        void OnScroll(object sender, RoutedEventArgs e)
        {
            int index = new Random().Next(0, ((IList)_lv.Data).Count);
            _lv.ScrollInto(index);
            Kit.Msg($"滚动到第 {index + 1} 行");
        }

        void OnToggleView(object sender, RoutedEventArgs e)
        {
            _lv.ItemHeight = 0;
            _lv.View = Resources[((Button)sender).Tag.ToString()];
        }

        void OnHideCol(object sender, RoutedEventArgs e)
        {
            var cols = _lv.View as Cols;
            cols.Hide("bh", "xm");
        }

        void OnHideExcept(object sender, RoutedEventArgs e)
        {
            var cols = _lv.View as Cols;
            cols.HideExcept("bh", "xm");
        }

        void OnShowAllCol(object sender, RoutedEventArgs e)
        {
            var cols = _lv.View as Cols;
            cols.ShowExcept();
        }

        void OnSetColHeader(object sender, RoutedEventArgs e)
        {
            _lv.LoadColHeaderCell -= OnLoadColHeaderCell;
            _lv.LoadColHeaderCell += OnLoadColHeaderCell;
            _lv.ViewMode = ViewMode.List;
            _lv.ViewMode = ViewMode.Table;
        }

        void OnLoadColHeaderCell(ColHeaderCell cell)
        {
            if (cell.Col.ID == "bh")
            {
                cell.Background = Res.TransparentBrush;
                cell.Foreground = Res.RedBrush;
            }
            else if (cell.Col.ID == "xm")
            {
                cell.Background = Res.BlackBrush;
                cell.Foreground = Res.WhiteBrush;
            }
            else
            {
                
            }
        }

        void OnResetColHeader(object sender, RoutedEventArgs e)
        {
            _lv.LoadColHeaderCell -= OnLoadColHeaderCell;
            _lv.ViewMode = ViewMode.List;
            _lv.ViewMode = ViewMode.Table;
        }
    }
}