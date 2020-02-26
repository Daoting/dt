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
            _lv.Data = new List<CenterInfo>
            {
                new CenterInfo(Icons.汉堡, "基础视图", typeof(LvViewBase), "三类视图，两种数据源，三种选择模式，支持分组"),
                new CenterInfo(Icons.分组, "表格视图", typeof(LvTable), "传统二维表格"),
                new CenterInfo(Icons.详细, "列表视图", typeof(LvList), "水平填充式列表，只垂直滚动"),
                new CenterInfo(Icons.日历, "磁贴视图", typeof(LvTile), "平铺式磁贴，一行多格，只垂直滚动"),
                new CenterInfo(Icons.修改, "视图扩展", typeof(LvViewEx), "定义行样式、扩展列"),
                new CenterInfo(Icons.划卡, "列表行模板选择", typeof(LvListSelector), "根据数据动态选择模板"),
                new CenterInfo(Icons.排列, "磁贴模板选择", typeof(LvTileSelector), "根据数据动态选择模板"),
                new CenterInfo(Icons.录音, "分组模板", typeof(LvGroupTemplate), "设置分组模板"),
                new CenterInfo(Icons.公告, "上下文菜单", typeof(LvContextMenu), "触发上下文菜单的方式：右键、悬停、点击标识按钮"),
            };
        }
    }
}
