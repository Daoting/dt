#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dt.Base;
using Dt.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public sealed partial class MvHome : Win
    {
        public MvHome()
        {
            InitializeComponent();
            _lv.Data = new Nl<MainInfo>
            {
                new MainInfo(Icons.汉堡, "Tab内导航", typeof(MvNavi), "Mv之间导航时输入输出参数、带遮罩的模式视图"),
                new MainInfo(Icons.分组, "表格视图", typeof(LvTable), "传统二维表格"),
                new MainInfo(Icons.全选, "列表视图", typeof(LvList), "水平填充式列表，只垂直滚动"),
                new MainInfo(Icons.日历, "磁贴视图", typeof(LvTile), "平铺式磁贴，一行多格，只垂直滚动"),
                new MainInfo(Icons.书籍, "内置单元格UI", typeof(LvCellUI), "适用于某列为固定UI类型的情况"),
            };
        }
    }
}
