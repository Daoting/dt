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
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.UIDemo
{
    public partial class FvDataSource : Win
    {
        int _rowNum;

        public FvDataSource()
        {
            InitializeComponent();
        }

        void OnDataRow(object sender, RoutedEventArgs e)
        {
            var r = new Row
            {
                { "name", $"第{++_rowNum}行" },
                { "fontsize", 22 },
                { "id", _rowNum.ToString() },
            };

            r.SetCellHook("name", e =>
            {
                e.NewVal = e.Str.ToUpper();
                Throw.If(e.GbkLength > 8, "超出最大长度", e.Cell);
            });

            _fv.Data = r;
        }

        void OnTgt1(object sender, RoutedEventArgs e)
        {
            _fv.Data = _tb;
        }

        void OnTgt2(object sender, RoutedEventArgs e)
        {
            _fv.Data = _btn;
        }

        void OnTgt3(object sender, RoutedEventArgs e)
        {
            _fv.Data = _tb2;
        }

        void OnNull(object sender, RoutedEventArgs e)
        {
            _fv.Data = null;
        }
    }
}