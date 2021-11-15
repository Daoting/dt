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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public partial class LvViewEx : Win
    {
        public LvViewEx()
        {
            InitializeComponent();
            _lv.CellEx = typeof(ViewEx1);
            _lv.Data = SampleData.CreatePersonsTbl(100);
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

        #region ViewEx
        class ViewEx1
        {
            public static void SetStyle(ViewItem p_item)
            {
                var row = p_item.Row;
                if (row.Date("chushengrq").Month == 9)
                    p_item.Background = Res.浅黄;

                if (row.Double("Shengao") > 1.75)
                    p_item.Foreground = Res.RedBrush;

                if (row.Str("bumen") == "循环门诊")
                    p_item.FontWeight = FontWeights.Bold;
                else if (row.Str("bumen") == "内分泌门诊")
                    p_item.FontStyle = FontStyle.Italic;
            }

            public static TextBlock xb(ViewItem p_item)
            {
                TextBlock tb = new TextBlock { FontFamily = Res.IconFont, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                tb.Text = p_item.Row.Str("xb") == "男" ? "\uE060" : "\uE0D9";
                return tb;
            }

            public static NumericTicker Line(ViewItem p_item)
            {
                return new NumericTicker(p_item.Row.Double("shengao"));
            }
        }
        #endregion
    }
}