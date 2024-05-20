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

namespace Dt.UIDemo
{
    public partial class LvItemStyle : Win
    {
        public LvItemStyle()
        {
            InitializeComponent();
            _lv.ChangeView(Resources["GridView"], ViewMode.Table);
            _lv.Data = SampleData.CreatePersonsTbl(100);

            _lv.ItemStyle = e =>
            {
                var row = e.Row;
                e.Background = row.Date("chushengrq").Month == 9 ? Res.浅黄 : Res.WhiteBrush;
                e.Foreground = row.Double("Shengao") > 1.75 ? Res.RedBrush : Res.BlackBrush;
                e.FontWeight = row.Str("bumen") == "循环门诊" ? FontWeights.Bold : FontWeights.Normal;
                e.FontStyle = row.Str("bumen") == "内分泌门诊" ? FontStyle.Italic : FontStyle.Normal;
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

        void OnUpdateSg(object sender, RoutedEventArgs e)
        {
            var row = _lv.Table[0];
            if (row.Double("shengao") > 1.75)
                row["shengao"] = 1.69;
            else
                row["shengao"] = 1.89;
        }

        void OnUpdateBirth(object sender, RoutedEventArgs e)
        {
            var row = _lv.Table[0];
            if (row.Date("chushengrq").Month == 9)
                row["chushengrq"] = row.Date("chushengrq").AddMonths(1);
            else
                row["chushengrq"] = new DateTime(1950, 9, 12);
        }

        void OnUpdateDep(object sender, RoutedEventArgs e)
        {
            var row = _lv.Table[0];
            if (row.Str("bumen") == "循环门诊")
                row["bumen"] = "内分泌门诊";
            else
                row["bumen"] = "循环门诊";
        }
    }

    [LvCall]
    public static class LvItemStyleUI
    {
        public static void 性别头像(Env e)
        {
            var tb = new TextBlock { FontFamily = Res.IconFont, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            e.UI = tb;

            e.Set += c =>
            {
                if (c.Data is Row row)
                tb.Text = row.Str("xb") == "男" ? "\uE060" : "\uE0D9";
                else if (c.Data is Person person)
                    tb.Text = person.Xb == "男" ? "\uE060" : "\uE0D9";
            };
        }

        public static void 曲线(Env e)
        {
            var ticker = new NumericTicker();
            e.UI = ticker;

            e.Set += c =>
            {
                if (c.Data is Row row)
                    ticker.Apply(row.Double("shengao"));
                else if (c.Data is Person person)
                    ticker.Apply(person.Shengao);
            };
        }
    }
}