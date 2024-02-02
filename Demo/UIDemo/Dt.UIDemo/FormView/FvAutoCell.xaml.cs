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
    public partial class FvAutoCell : Win
    {
        Table _tbl;
        int _rowNum;

        public FvAutoCell()
        {
            InitializeComponent();
            _tbl = new Table { { "name" }, { "fontsize", typeof(double) }, { "id" }, };
        }

        void OnDataRow(object sender, RoutedEventArgs e)
        {
            _fv.Data = _tbl.AddRow(new
            {
                name = $"第{++_rowNum}行",
                fontsize = 22,
                id = _rowNum.ToString()
            });
        }

        void OnTgt1(object sender, RoutedEventArgs e)
        {
            _fv.Data = _tb;
        }

        void OnTgt2(object sender, RoutedEventArgs e)
        {
            _fv.Data = _btn;
        }

        void OnNull(object sender, RoutedEventArgs e)
        {
            _fv.Data = null;
        }
    }
}