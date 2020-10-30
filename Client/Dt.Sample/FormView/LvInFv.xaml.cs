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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public partial class LvInFv : Win
    {
        public LvInFv()
        {
            InitializeComponent();

        }

        void OnLoadData(object sender, RoutedEventArgs e)
        {
            var tbl = SampleData.CreatePersonsTbl(int.Parse(((Button)sender).Tag.ToString()));
            _lvList.Data = tbl;
            _lvTbl.Data = tbl;
            _lvTile.Data = tbl;
        }

        void OnLoadNull(object sender, RoutedEventArgs e)
        {
            _lvList.Data = null;
            _lvTbl.Data = null;
            _lvTile.Data = null;
        }
    }
}