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
            HostOS os = (HostOS)e.CellVal;
            switch (os)
            {
                case HostOS.Linux:
                    Dt.Base.Def.红白(e);
                    break;
                case HostOS.Android:
                    Dt.Base.Def.蓝白(e);
                    break;
                case HostOS.iOS:
                    Dt.Base.Def.黑白(e);
                    break;
                default:
                    e.Background = Res.GreenBrush;
                    e.Foreground = Res.WhiteBrush;
                    e.FontSize = 22;
                    e.FontWeight = FontWeights.Bold;
                    e.FontStyle = FontStyle.Italic;
                    break;
            }
        }

        public static void 曲线(Env e)
        {
            e.UI = new NumericTicker(e.Row.Double("shengao"));
        }

        public static void 背景(Env e)
        {
            double d = e.Row.Double(e.ID);
            e.Background = (d > 1.8) ? Res.GreenBrush : Res.WhiteBrush;
        }

        public static void 前景(Env e)
        {
            double d = e.Row.Double(e.ID);
            e.Foreground = (d > 1.8) ? Res.WhiteBrush : Res.RedBrush;
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
            double d = e.Row.Double("shengao");
            e.UI = new TextBlock { Style = Res.LvTextBlock, Text = d.ToString(e.Format) };
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