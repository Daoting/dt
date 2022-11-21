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
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Text;
#endregion

namespace Dt.Sample
{
    public partial class LvItemStyle : Win
    {
        public LvItemStyle()
        {
            InitializeComponent();
            _lv.ChangeView(Resources["GridView"], ViewMode.Table);
            _lv.Data = SampleData.CreatePersonsTbl(100);

            _lv.ItemStyle = (e) =>
            {
                var row = e.Row;
                if (row.Date("chushengrq").Month == 9)
                    e.Background = Res.浅黄;

                if (row.Double("Shengao") > 1.75)
                    e.Foreground = Res.RedBrush;

                if (row.Str("bumen") == "循环门诊")
                    e.FontWeight = FontWeights.Bold;
                else if (row.Str("bumen") == "内分泌门诊")
                    e.FontStyle = FontStyle.Italic;
            };
        }

        void OnGridView(object sender, RoutedEventArgs e)
        {
            _lv.ChangeView(Resources["GridView"], ViewMode.Table);
        }

        void OnListView(object sender, RoutedEventArgs e)
        {
            _lv.ChangeView(Resources["ListView"], ViewMode.List);
        }

        void OnFormView(object sender, RoutedEventArgs e)
        {
            _lv.ChangeView(Resources["ListView"], ViewMode.Tile);
        }

        void OnGroup(object sender, RoutedEventArgs e)
        {
            _lv.GroupName = "bumen";
        }

        void OnDelGroup(object sender, RoutedEventArgs e)
        {
            _lv.GroupName = null;
        }
    }

    [CellUI]
    public static class LvItemStyleUI
    {
        public static void 性别头像(Env e)
        {
            var tb = new TextBlock { FontFamily = Res.IconFont, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            e.UI = tb;

            e.Set += c =>
            {
                tb.Text = c.Row.Str("xb") == "男" ? "\uE060" : "\uE0D9";
            };
        }

        public static void 曲线(Env e)
        {
            var ticker = new NumericTicker();
            e.UI = ticker;

            e.Set += c =>
            {
                ticker.Apply(c.Row.Double("shengao"));
            };
        }
    }
}