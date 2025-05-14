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

namespace Demo.UI
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
                { "style", typeof(AppType) },
                { "shengao", typeof(double) },
                { "date", typeof(DateTime) },
                { "warning", typeof(int) },
            };

            Random rand = new Random();
            DateTime birth = Kit.Now;
            for (int i = 0; i < 25; i++)
            {
                tbl.AddRow(new
                {
                    style = (AppType)rand.Next(0, 6),
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

        void OnUpdate(object sender, RoutedEventArgs e)
        {
            if (_dt == null)
            {
                _dt = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };
                _dt.Tick += (s, e) => Update();
            }

            if (_dt.IsEnabled)
            {
                _btn.Content = "开始动态修改数据";
                _dt.Stop();
            }
            else
            {
                _btn.Content = "停止修改";
                _dt.Start();
            }
        }

        void Update()
        {
            Random rand = new Random();
            DateTime birth = Kit.Now;
            foreach (var row in _lv.Table)
            {
                row["style"] = (AppType)rand.Next(0, 6);
                row["shengao"] = (double)rand.Next(150, 190) / 100;
                row["date"] = birth.AddMonths(rand.Next(100));
                row["warning"] = rand.Next(0, 20);
            }
        }

        DispatcherTimer _dt;
    }

    [LvCall]
    public static class LvCustomUI
    {
        public static void ApplyStyle(Env e)
        {
            e.Set += c =>
            {
                // 动态设置样式时，每种情况都需设置相同属性
                var os = c.GetVal<AppType>(c.ID);
                switch (os)
                {
                    case AppType.Wasm:
                        c.Background = Res.RedBrush;
                        c.FontWeight = FontWeights.Normal;
                        c.FontStyle = FontStyle.Italic;
                        break;
                    case AppType.Android:
                        c.Background = Res.主蓝;
                        c.FontWeight = FontWeights.Bold;
                        c.FontStyle = FontStyle.Normal;
                        break;
                    case AppType.iOS:
                        c.Background = Res.BlackBrush;
                        c.FontWeight = FontWeights.Normal;
                        c.FontStyle = FontStyle.Normal;
                        break;
                    case AppType.Windows:
                        c.Background = Res.GrayBrush;
                        c.FontWeight = FontWeights.Bold;
                        c.FontStyle = FontStyle.Italic;
                        break;
                    case AppType.Linux:
                        c.Background = Res.亮蓝;
                        c.FontWeight = FontWeights.Normal;
                        c.FontStyle = FontStyle.Normal;
                        break;
                    default:
                        c.Background = Res.GreenBrush;
                        c.FontWeight = FontWeights.Black;
                        c.FontStyle = FontStyle.Oblique;
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
                if (c.Data is Row row)
                    ticker.Apply(row.Double("shengao"));
                else if (c.Data is LvExpPrint.CustomItem custom)
                    ticker.Apply(custom.Shengao);
            };
        }

        public static void 背景(Env e)
        {
            e.Set += c =>
            {
                c.Background = c.IsHeigher() ? Res.GreenBrush : Res.WhiteBrush;
            };
        }

        public static void 前景(Env e)
        {
            e.Set += c =>
            {
                c.Foreground = c.IsHeigher() ? Res.WhiteBrush : Res.RedBrush;
            };
        }

        public static void AsyncFunc(Env e)
        {
            var tb = new TextBlock { Style = Res.LvTextBlock, };
            e.UI = tb;

            e.Set += async c =>
            {
                await Task.Delay(1000);
                tb.Text = new Random().Next(1000).ToString();
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
                tb.Text = c.GetVal<double>("shengao").ToString(c.Format);
            };
        }
    }

    public static class MyCallArgsEx
    {
        public static bool IsHeigher(this CallArgs c)
        {
            return c.Double > 1.8;
        }
    }

    ///// <summary>
    ///// 重写的不注释会影响全局
    ///// </summary>
    //[LvCall]
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