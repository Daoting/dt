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
    public partial class LvCustomCellUI : Win
    {
        public LvCustomCellUI()
        {
            InitializeComponent();
            _lv.ChangeView(Resources["GridView"], ViewMode.Table);
            LoadData();
        }

        void LoadData()
        {
            Table tbl = new Table
            {
                { "style", typeof(HostOS) },
                { "shengao", typeof(double) },
                { "date", typeof(DateTime) },
                { "warning", typeof(int) },
            };

            Random rand = new Random();
            DateTime birth = Kit.Now;
            for (int i = 0; i < 50; i++)
            {
                tbl.AddRow(new
                {
                    style = (HostOS)rand.Next(0, 6),
                    shengao = (double)rand.Next(150, 190) / 100,
                    date = birth.AddMonths(rand.Next(100)),
                    warning = rand.Next(0, 20),
                });
            }
            _lv.Data = tbl;
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
    }

    [CellUI]
    public static class LvCustomUI
    {
        public static void ApplyStyle(Env e)
        {
            e.Set += c =>
            {
                HostOS os = c.GetVal<HostOS>(c.ID);
                switch (os)
                {
                    case HostOS.Linux:
                        c.Background = Res.RedBrush;
                        break;
                    case HostOS.Android:
                        c.Background = Res.主蓝;
                        break;
                    case HostOS.iOS:
                        c.Background = Res.BlackBrush;
                        break;
                    default:
                        c.Background = Res.GreenBrush;
                        c.FontSize = 22;
                        c.FontWeight = FontWeights.Bold;
                        c.FontStyle = FontStyle.Italic;
                        break;
                }
            };
            e.Dot.Foreground = Res.WhiteBrush;
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

        public static void 背景(Env e)
        {
            e.Set += c =>
            {
                double d = c.Double;
                c.Background = (d > 1.8) ? Res.GreenBrush : Res.WhiteBrush;
            };
        }

        public static void 前景(Env e)
        {
            e.Set += c =>
            {
                double d = c.Double;
                c.Foreground = (d > 1.8) ? Res.WhiteBrush : Res.RedBrush;
            };
        }

        public static void ModiDef(Env e)
        {
            var elem = e.CreateDefaultUI();
            Border bd = new Border
            {
                Background = Res.浅灰1,
                CornerRadius = new CornerRadius(8),
                Child = elem,
                Padding = new Thickness(10, 0, 0, 0),
            };
            e.UI = bd;
        }

        public static void Format(Env e)
        {
            var tb = new TextBlock { Style = Res.LvTextBlock,  };
            e.UI = tb;

            e.Set += c =>
            {
                tb.Text = c.Double.ToString(c.Format);
            };
        }
    }

    ///// <summary>
    ///// 重写的不注释会影响全局
    ///// </summary>
    //[CellUI]
    //public class Def
    //{
    //    public static void Warning(Env e)
    //    {
    //        var val = e.CellVal;
    //        if (val == null)
    //            return;

    //        e.UI = new TextBlock { Style = Res.LvTextBlock, Text = val.ToString(), Foreground = Res.RedBrush };
    //    }
    //}
}