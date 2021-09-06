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
            //Kit.Msg("已复制到剪切板！");
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
                Kit.Msg($"{(e.IsChanged ? "切换行" : "未切换")} \r\n当前行：{row.Str("name")}，\r\n上次行：{(old != null ? old.Str("name") : "无")}");
            }
            else if (e.Data is MedTreeItem per)
            {
                MedTreeItem old = e.OldData as MedTreeItem;
                Kit.Msg($"{(e.IsChanged ? "切换行" : "未切换")} \r\n当前行：{per.Name}，\r\n上次行：{(old != null ? old.Name : "无")}");
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
            Kit.Msg($"增加选择{e.AddedItems.Count}行，取消选择{e.RemovedItems.Count}行");
        }

        void OnRowDoubleClick(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked.Value)
                _tv.ItemDoubleClick += OnItemDoubleClick;
            else
                _tv.ItemDoubleClick -= OnItemDoubleClick;
        }

        void OnItemDoubleClick(object sender, object e)
        {
            if (e is Row row)
            {
                Kit.Msg($"双击行：{row.Str("name")}");
            }
            else if (e is MedTreeItem per)
            {
                Kit.Msg($"双击行：{per.Name}");
            }
        }

        void OnExpandAll(object sender, RoutedEventArgs e)
        {
            _tv.ExpandAll();
        }

        void OnCollapseAll(object sender, RoutedEventArgs e)
        {
            _tv.CollapseAll();
        }

        void OnNoEnteredBrush(object sender, RoutedEventArgs e)
        {
            _tv.EnteredBrush = null;
        }

        void OnEnteredBrush(object sender, RoutedEventArgs e)
        {
            _tv.EnteredBrush = Res.深黄遮罩;
        }

        void OnDefEnteredBrush(object sender, RoutedEventArgs e)
        {
            _tv.ClearValue(Base.TreeView.EnteredBrushProperty);
        }

        void OnNoPressedBrush(object sender, RoutedEventArgs e)
        {
            _tv.PressedBrush = null;
        }

        void OnPressedBrush(object sender, RoutedEventArgs e)
        {
            _tv.PressedBrush = Res.深暗遮罩;
        }

        void OnDefPressedBrush(object sender, RoutedEventArgs e)
        {
            _tv.ClearValue(Base.TreeView.PressedBrushProperty);
        }

        void OnScroll(object sender, RoutedEventArgs e)
        {
            if (_tv.Data is Table tbl)
            {
                int index = new Random().Next(0, tbl.Count);
                _tv.SelectedItem = tbl[index];
                Kit.Msg($"已选择 {tbl[index].Str("name")}");
            }
        }

        void OnScrollTop(object sender, RoutedEventArgs e)
        {
            _tv.ScrollTop();
        }

        void OnScrollBottom(object sender, RoutedEventArgs e)
        {
            _tv.ScrollBottom();
        }
    }
}