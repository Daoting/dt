#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Microsoft.UI.Xaml;
#endregion

namespace Dt.Sample
{
    public partial class FvCustomVal : Win
    {
        Table _tbl;

        public FvCustomVal()
        {
            InitializeComponent();

            _tbl = new Table
            {
                { "num", typeof(int) },
                { "style", typeof(int) },
                { "prefix" },
                { "merge" },
                { "merge1" },
                { "merge2" },
                { "replace" },
            };
            OnRow(null, null);
            _fv.Changed += (s, e) => CellDemoKit.OnChanged(_fv, e);
        }

        void OnRow(object sender, RoutedEventArgs e)
        {
            var rnd = new Random();
            _fv.Data = _tbl.AddRow(new
            {
                num = rnd.Next(100) * 100,
                style = rnd.Next(100),
                prefix = rnd.Next(1000).ToString(),
                merge = rnd.Next(1000).ToString(),
                merge1 = rnd.Next(1000).ToString(),
                merge2 = rnd.Next(1000).ToString(),
                replace = "a",
            });
        }

        void OnObj(object sender, RoutedEventArgs e)
        {
            var rnd = new Random();
            _fv.Data = new CustomData
            {
                Num = rnd.Next(100) * 100,
                Style = rnd.Next(100),
                Prefix = rnd.Next(1000).ToString(),
                Merge = rnd.Next(1000).ToString(),
                Merge1 = rnd.Next(1000).ToString(),
                Merge2 = rnd.Next(1000).ToString(),
                Replace = "a",
            };
        }

        void OnNull(object sender, RoutedEventArgs e)
        {
            _fv.Data = null;
        }

        class CustomData
        {
            public int Num { get; set; }

            public int Style { get; set; }

            public string Prefix { get; set; }

            public string Merge { get; set; }

            public string Merge1 { get; set; }

            public string Merge2 { get; set; }

            public string Replace { get; set; }
        }
    }
}