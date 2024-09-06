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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.UI
{
    public partial class TdBase : Win
    {
        public TdBase()
        {
            InitializeComponent();
            OnLoadTbl(null, null);
        }

        void OnLoadTbl(object sender, RoutedEventArgs e)
        {
            _td.Data = TvData.GetOneRootTbl();
        }

        void OnLoadData(object sender, RoutedEventArgs e)
        {
            _td.Data = TvData.GetTbl();
        }

        void OnRowClick(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked.Value)
                _td.ItemClick += OnItemClick;
            else
                _td.ItemClick -= OnItemClick;
        }

        void OnItemClick(ItemClickArgs e)
        {
            if (e.Data is Row row)
            {
                Row old = e.OldData as Row;
                Kit.Msg($"{(e.IsChanged ? "切换行" : "未切换")} \r\n当前行：{row.Str("name")}，\r\n上次行：{(old != null ? old.Str("name") : "无")}");
            }
        }

        void OnSelectionChangedClick(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked.Value)
                _td.SelectionChanged += OnSelectionChanged;
            else
                _td.SelectionChanged -= OnSelectionChanged;
        }

        void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Kit.Msg($"增加选择{e.AddedItems.Count}行，取消选择{e.RemovedItems.Count}行");
        }

        void OnRowDoubleClick(object sender, RoutedEventArgs e)
        {
            if ((sender as CheckBox).IsChecked.Value)
                _td.ItemDoubleClick += OnItemDoubleClick;
            else
                _td.ItemDoubleClick -= OnItemDoubleClick;
        }

        void OnItemDoubleClick(object e)
        {
            if (e is Row row)
            {
                Kit.Msg($"双击行：{row.Str("name")}");
            }
        }

        void OnNoEnteredBrush(object sender, RoutedEventArgs e)
        {
            _td.EnteredBrush = null;
        }

        void OnEnteredBrush(object sender, RoutedEventArgs e)
        {
            _td.EnteredBrush = Res.深黄遮罩;
        }

        void OnDefEnteredBrush(object sender, RoutedEventArgs e)
        {
            _td.ClearValue(Tv.EnteredBrushProperty);
        }

        void OnNoPressedBrush(object sender, RoutedEventArgs e)
        {
            _td.PressedBrush = null;
        }

        void OnPressedBrush(object sender, RoutedEventArgs e)
        {
            _td.PressedBrush = Res.深暗遮罩;
        }

        void OnDefPressedBrush(object sender, RoutedEventArgs e)
        {
            _td.ClearValue(Tv.PressedBrushProperty);
        }

        void OnStyle1(object sender, RoutedEventArgs e)
        {
            _td.View = Resources["背景"];
        }

        void OnStyle2(object sender, RoutedEventArgs e)
        {
            _td.View = Resources["文字"];
        }

        void OnStyle3(object sender, RoutedEventArgs e)
        {
            _td.View = Resources["椭圆"];
        }

        void OnStyle4(object sender, RoutedEventArgs e)
        {
            _td.View = new RndNodeSelector
            {
                Style1 = (DataTemplate)Resources["随机1"],
                Style2 = (DataTemplate)Resources["随机2"],
                Style3 = (DataTemplate)Resources["随机3"],
            };
        }

        void OnBtnEvent(object sender, RoutedEventArgs e)
        {
            var menu = Ex.GetMenu(_td);
            if (menu != null)
            {
                menu.TriggerEvent = TriggerEvent.Custom;
            }
        }

        void OnRightHolding(object sender, RoutedEventArgs e)
        {
            var menu = Ex.GetMenu(_td);
            if (menu != null)
            {
                menu.Placement = MenuPosition.Default;
                menu.TriggerEvent = TriggerEvent.RightTapped;
            }
        }

        void OnLeftTap(object sender, RoutedEventArgs e)
        {
            var menu = Ex.GetMenu(_td);
            if (menu != null)
            {
                menu.Placement = MenuPosition.Default;
                menu.TriggerEvent = TriggerEvent.LeftTapped;
            }
        }

        void OnChangeMenu(object sender, RoutedEventArgs e)
        {
            var menu = new Menu();
            string id = new Random().Next(1, 100).ToString();
            menu.Items.Add(new Mi { ID = "修改" + id, Icon = Icons.修改 });
            menu.Items.Add(new Mi { ID = "搜索" + id, Icon = Icons.搜索 });
            Ex.SetMenu(_td, menu);
        }

        void OnNoMenu(object sender, RoutedEventArgs e)
        {
            Ex.SetMenu(_td, null);
        }
    }

    public class RndNodeSelector : DataTemplateSelector
    {
        public DataTemplate Style1 { get; set; }
        public DataTemplate Style2 { get; set; }
        public DataTemplate Style3 { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            int index = ((TdItem)item).Row.Index % 3;
            if (index == 0)
                return Style1;
            if (index == 1)
                return Style2;
            return Style3;
        }
    }
}