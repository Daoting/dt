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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public partial class LvViewBase : Win
    {
        public LvViewBase()
        {
            InitializeComponent();
            _lv.View = Resources["TableView"];
            _lv.ViewMode = ViewMode.Table;
            _lv.GroupName = "bumen";
            _lv.Data = SampleData.CreatePersonsTbl(100);
        }

        void OnGridView(object sender, RoutedEventArgs e)
        {
            _lv.ChangeView(Resources["TableView"], ViewMode.Table);
        }

        void OnListView(object sender, RoutedEventArgs e)
        {
            _lv.ChangeView(Resources["ListView"], ViewMode.List);
        }

        void OnFormList(object sender, RoutedEventArgs e)
        {
            _lv.ChangeView(Resources["TableView"], ViewMode.List);
        }

        void OnTileView(object sender, RoutedEventArgs e)
        {
            _lv.ChangeView(Resources["TileView"], ViewMode.Tile);
        }

        void OnFormTile(object sender, RoutedEventArgs e)
        {
            _lv.ChangeView(Resources["TableView"], ViewMode.Tile);
        }

        void OnRowClick(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked.Value)
                _lv.ItemClick += OnRowChanged;
            else
                _lv.ItemClick -= OnRowChanged;
        }

        void OnRowChanged(object sender, ItemClickArgs e)
        {
            if (e.Data is Row row)
            {
                Row old = e.OldData as Row;
                Kit.Msg($"{(e.IsChanged ? "切换行" : "未切换")} \r\n当前行：{row.Str("xm")}，\r\n上次行：{(old != null ? old.Str("xm") : "无")}");
            }
            else if (e.Data is Person per)
            {
                Person old = e.OldData as Person;
                Kit.Msg($"{(e.IsChanged ? "切换行" : "未切换")} \r\n当前行：{per.Xm}，\r\n上次行：{(old != null ? old.Xm : "无")}");
            }
        }

        void OnSelectionChangedClick(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked.Value)
                _lv.SelectionChanged += OnSelectionChanged;
            else
                _lv.SelectionChanged -= OnSelectionChanged;
        }

        void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Kit.Msg($"增加选择{e.AddedItems.Count}行，取消选择{e.RemovedItems.Count}行");
        }

        void OnRowDoubleClick(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked.Value)
                _lv.ItemDoubleClick += OnItemDoubleClick;
            else
                _lv.ItemDoubleClick -= OnItemDoubleClick;
        }

        void OnItemDoubleClick(object sender, object e)
        {
            if (e is Row row)
            {
                Kit.Msg($"双击行：{row.Str("xm")}");
            }
            else if (e is Person per)
            {
                Kit.Msg($"双击行：{per.Xm}");
            }
        }

        void OnAddRow(object sender, RoutedEventArgs e)
        {
            var tbl = _lv.Table;
            if (tbl != null)
            {
                Row row = tbl.NewRow(new { xm = "数据行" });
                _lv.Data.Add(row);
            }
            else
            {
                _lv.Data.Add(new Person { Xm = "对象行" });
            }
        }

        void OnInsertRows(object sender, RoutedEventArgs e)
        {
            if (_lv.Data is Table)
            {
                var data = SampleData.CreatePersonsTbl(10);
                foreach (var row in data)
                {
                    row.InitVal("xm", "新行");
                }
                _lv.Data.InsertRange(0, data);
            }
            else
            {
                var ls = SampleData.CreatePersonsList(10);
                foreach (var per in ls)
                {
                    per.Xm = "新对象行";
                }
                _lv.Data.InsertRange(0, ls);
            }
        }

        void OnDeleteRow(object sender, RoutedEventArgs e)
        {
            _lv.DeleteSelection();
        }

        void OnSelectNull(object sender, RoutedEventArgs e)
        {
            _lv.SelectedItem = null;
        }

        void OnChangedVal(object sender, RoutedEventArgs e)
        {
            if (_lv.Data is Table tbl)
            {
                var row = tbl[0];
                row["xm"] = row.Str("xm") + "+";
            }
            else if (_lv.Data is List<Person> pers)
            {
                pers[0].Xm = pers[0].Xm + "-";
            }
        }

        void OnScroll(object sender, RoutedEventArgs e)
        {
            int index = new Random().Next(0, _lv.Data.Count);
            _lv.ScrollInto(index);
            Kit.Msg($"滚动到第 {index + 1} 行");
        }

        void OnScrollTop(object sender, RoutedEventArgs e)
        {
            _lv.ScrollTop();
        }

        void OnScrollBottom(object sender, RoutedEventArgs e)
        {
            _lv.ScrollBottom();
        }

        void OnGroup(object sender, RoutedEventArgs e)
        {
            _lv.GroupName = "bumen";
        }

        void OnDelGroup(object sender, RoutedEventArgs e)
        {
            _lv.GroupName = null;
        }

        void OnLoadData(object sender, RoutedEventArgs e)
        {
            _lv.Data = SampleData.CreatePersonsTbl(int.Parse(((Button)sender).Tag.ToString()));
        }

        void OnLoadObjs(object sender, RoutedEventArgs e)
        {
            _lv.Data = SampleData.CreatePersonsList(int.Parse(((Button)sender).Tag.ToString()));
        }

        void OnLoadNull(object sender, RoutedEventArgs e)
        {
            _lv.Data = null;
        }

        bool _isTbl;
        void OnPageData(object sender, RoutedEventArgs e)
        {
            _isTbl = true;
            _lv.PageData = new PageData { NextPage = OnNextPage };
        }

        void OnTopPageData(object sender, RoutedEventArgs e)
        {
            _isTbl = true;
            _lv.PageData = new PageData { NextPage = OnNextPage, InsertTop = true };
        }

        void OnPageObjs(object sender, RoutedEventArgs e)
        {
            _isTbl = false;
            _lv.PageData = new PageData { NextPage = OnNextPage };
        }

        void OnNextPage(PageData p_pd)
        {
            INotifyList ls;
            if (_isTbl)
                ls = SampleData.CreatePersonsTbl(p_pd.PageSize);
            else
                ls = SampleData.CreatePersonsList(p_pd.PageSize);
            p_pd.LoadPageData(ls);
        }

        void OnAutoHeight(object sender, RoutedEventArgs e)
        {
            _lv.ItemHeight = double.NaN;
        }

        void OnToolbar(object sender, RoutedEventArgs e)
        {
            var temp = (DataTemplate)Resources["Toolbar"];
            _lv.Toolbar = temp.LoadContent() as Menu;
        }

        void OnDelToolbar(object sender, RoutedEventArgs e)
        {
            _lv.Toolbar = null;
        }

        void OnNoEnteredBrush(object sender, RoutedEventArgs e)
        {
            _lv.EnteredBrush = null;
        }

        void OnEnteredBrush(object sender, RoutedEventArgs e)
        {
            _lv.EnteredBrush = Res.深黄遮罩;
        }

        void OnDefEnteredBrush(object sender, RoutedEventArgs e)
        {
            _lv.ClearValue(Lv.EnteredBrushProperty);
        }

        void OnNoPressedBrush(object sender, RoutedEventArgs e)
        {
            _lv.PressedBrush = null;
        }

        void OnPressedBrush(object sender, RoutedEventArgs e)
        {
            _lv.PressedBrush = Res.深暗遮罩;
        }

        void OnDefPressedBrush(object sender, RoutedEventArgs e)
        {
            _lv.ClearValue(Lv.PressedBrushProperty);
        }

        void OnToggleViewMode(object sender, Mi e)
        {
            if (_lv.ViewMode == ViewMode.Tile)
            {
                _lv.ChangeView(Resources["ListView"], ViewMode.List);
                e.Icon = Icons.排列;
            }
            else
            {
                _lv.ChangeView(Resources["TileView"], ViewMode.Tile);
                e.Icon = Icons.汉堡;
            }
        }
    }
}