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
    public sealed partial class LvHome : Win
    {
        public LvHome()
        {
            InitializeComponent();
            _nav.Data = new Nl<Nav>
            {
                new Nav("基础视图", typeof(LvViewBase), Icons.汉堡) { Desc = "三类视图，两种数据源，三种选择模式，支持分组" },
                new Nav("表格视图", typeof(LvTable), Icons.分组) { Desc = "传统二维表格" },
                new Nav("列表视图", typeof(LvList), Icons.全选) { Desc = "水平填充式列表，只垂直滚动" },
                new Nav("磁贴视图", typeof(LvTile), Icons.日历) { Desc = "平铺式磁贴，一行多格，只垂直滚动" },
                new Nav("内置单元格UI", typeof(LvCellUI), Icons.书籍) { Desc = "适用于某列为固定UI类型的情况" },
                new Nav("自定义单元格UI", typeof(LvViewEx), Icons.修改) { Desc = "定义行样式、扩展列" },
                new Nav("行模板选择", typeof(LvListSelector), Icons.划卡) { Desc = "根据数据动态选择行模板" },
                new Nav("动态行内容", typeof(LvRowView), Icons.划卡) { Desc = "根据数据动态生成行UI内容" },
                new Nav("分组模板", typeof(LvGroupTemplate), Icons.录音) { Desc = "设置分组模板" },
                new Nav("上下文菜单", typeof(LvContextMenu), Icons.公告) { Desc = "触发上下文菜单的方式：右键、悬停、点击标识按钮" },
                new Nav("外部ScrollViewer", typeof(LvInScrollViewer), Icons.乐谱) { Desc = "外部嵌套ScrollViewer，和其他元素一起滚动" },
            };
        }
    }
}
