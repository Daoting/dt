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
using Microsoft.UI.Xaml.Data;
#endregion

namespace Dt.Sample
{
    public partial class LvCellValChanging : Win
    {
        public LvCellValChanging()
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
    public static class LvBindUI
    {
        public static void ApplyStyle(Env e)
        {
            Border bd = new Border();
            bd.SetBinding(Border.BackgroundProperty, new LvBind(e, (c) =>
            {
                switch (c.GetVal<HostOS>(c.ID))
                {
                    case HostOS.Linux:
                        return Res.WhiteBrush;

                    case HostOS.Android:
                        return Res.BlueBrush;

                    case HostOS.iOS:
                        return Res.BlackBrush;

                    default:
                        return Res.GreenBrush;
                }
            }));
            TextBlock tb = new TextBlock { Style = Res.LvTextBlock };
            tb.SetBinding(TextBlock.TextProperty, new LvBind(e, (c) => c.Str));
            bd.Child = tb;

            e.UI = bd;
        }

    }
}