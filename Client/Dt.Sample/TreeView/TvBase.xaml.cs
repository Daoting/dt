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
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public partial class TvBase : Win
    {
        public TvBase()
        {
            InitializeComponent();
            OnLoadTbl(null, null);
        }

        void OnLoadTbl(object sender, RoutedEventArgs e)
        {
            _tv.Data = TvData.GetTbl();
            
            // xml -> code
            //StringBuilder sb = new StringBuilder();
            //foreach (var row in (Table)_tv.Data)
            //{
            //    sb.AppendLine($"tbl.NewRow(\"{row.Str("id")}\", \"{row.Str("parentid")}\", \"{row.Str("name")}\", \"{row.Str("简码")}\");");
            //}
            //DataPackage data = new DataPackage();
            //data.SetText(sb.ToString());
            //Clipboard.SetContent(data);
            //AtKit.Msg("已复制到剪切板！");
        }

        void OnLoadData(object sender, RoutedEventArgs e)
        {
            _tv.Data = TvData.GetTreeData();
        }

        void OnRowClick(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked.Value)
                _tv.ItemClick += OnItemClick;
            else
                _tv.ItemClick -= OnItemClick;
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            if (e.Data is Row row)
            {
                Row old = e.OldData as Row;
                AtKit.Msg($"{(e.IsChanged ? "切换行" : "未切换")} \r\n当前行：{row.Str("name")}，\r\n上次行：{(old != null ? old.Str("name") : "无")}");
            }
            else if (e.Data is MedTreeItem per)
            {
                MedTreeItem old = e.OldData as MedTreeItem;
                AtKit.Msg($"{(e.IsChanged ? "切换行" : "未切换")} \r\n当前行：{per.Name}，\r\n上次行：{(old != null ? old.Name : "无")}");
            }
        }

        void OnSelectionChangedClick(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked.Value)
                _tv.SelectionChanged += OnSelectionChanged;
            else
                _tv.SelectionChanged -= OnSelectionChanged;
        }

        void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AtKit.Msg($"增加选择{e.AddedItems.Count}行，取消选择{e.RemovedItems.Count}行");
        }

        void OnExpandAll(object sender, RoutedEventArgs e)
        {
            _tv.ExpandAll();
        }

        void OnCollapseAll(object sender, RoutedEventArgs e)
        {
            _tv.CollapseAll();
        }

        void OnScroll(object sender, RoutedEventArgs e)
        {
            if (_tv.Data is Table tbl)
            {
                int index = new Random().Next(0, tbl.Count);
                _tv.SelectedItem = tbl[index];
                AtKit.Msg($"已选择 {tbl[index].Str("name")}");
            }
        }
    }
}