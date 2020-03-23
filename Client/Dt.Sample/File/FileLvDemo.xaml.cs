﻿#region 文件描述
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
    public partial class FileLvDemo : Win
    {
        public FileLvDemo()
        {
            InitializeComponent();
            _lv.View = GetResource("TableView");
            _lv.ViewMode = ViewMode.Table;
            _lv.Data = SampleData.CreatePersonsTbl(100);
        }

        void OnGridView(object sender, RoutedEventArgs e)
        {
            _lv.ChangeView(GetResource("TableView"), ViewMode.Table);
        }

        void OnListView(object sender, RoutedEventArgs e)
        {
            _lv.ChangeView(GetResource("ListView"), ViewMode.List);
        }

        void OnFormList(object sender, RoutedEventArgs e)
        {
            _lv.ChangeView(GetResource("TableView"), ViewMode.List);
        }

        void OnTileView(object sender, RoutedEventArgs e)
        {
            _lv.ChangeView(GetResource("TileView"), ViewMode.Tile);
        }

        void OnFormTile(object sender, RoutedEventArgs e)
        {
            _lv.ChangeView(GetResource("TableView"), ViewMode.Tile);
        }

        object GetResource(string p_key)
        {
#if UWP
            return Resources[p_key];
#else
            if (p_key == "ListView")
                return StaticResources.ListView;
            if (p_key == "TileView")
                return StaticResources.TileView;
            return StaticResources.TableView;
#endif
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
                AtKit.Msg($"{(e.IsChanged ? "切换行" : "未切换")} \r\n当前行：{row.Str("xm")}，\r\n上次行：{(old != null ? old.Str("xm") : "无")}");
            }
            else if (e.Data is Person per)
            {
                Person old = e.OldData as Person;
                AtKit.Msg($"{(e.IsChanged ? "切换行" : "未切换")} \r\n当前行：{per.Xm}，\r\n上次行：{(old != null ? old.Xm : "无")}");
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
            AtKit.Msg($"增加选择{e.AddedItems.Count}行，取消选择{e.RemovedItems.Count}行");
        }

        void OnAddRow(object sender, RoutedEventArgs e)
        {
            var tbl = _lv.Table;
            if (tbl != null)
            {
                Row row = tbl.NewRow(new { xm = "数据行" });
                _lv.InsertRow(row);
            }
            else
            {
                _lv.InsertRow(new Person { Xm = "对象行" });
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
                _lv.InsertRows(data, 0);
            }
            else
            {
                var ls = SampleData.CreatePersonsList(10);
                foreach (var per in ls)
                {
                    per.Xm = "新对象行";
                }
                _lv.InsertRows(ls, 0);
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
            int index = new Random().Next(0, ((IList)_lv.Data).Count);
            _lv.ScrollInto(index);
            AtKit.Msg($"滚动到第 {index + 1} 行");
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
            IList ls;
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
    }
}