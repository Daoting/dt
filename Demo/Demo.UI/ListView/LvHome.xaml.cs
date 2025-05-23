﻿#region 文件描述
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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.UI
{
    public sealed partial class LvHome : Win
    {
        public LvHome()
        {
            InitializeComponent();
            _nav.Data = Dir;
        }

        public static Nl<Nav> Dir { get; } = new Nl<Nav>
        {
            new Nav("基础视图", typeof(LvViewBase), Icons.汉堡) { Desc = "三类视图，两种数据源，三种选择模式，支持分组" },
            new Nav("表格视图", typeof(LvTable), Icons.分组) { Desc = "传统二维表格" },
            new Nav("列表视图", typeof(LvList), Icons.全选) { Desc = "水平填充式列表，只垂直滚动" },
            new Nav("磁贴视图", typeof(LvTile), Icons.日历) { Desc = "平铺式磁贴，一行多格，只垂直滚动" },
            new Nav("列表视图的行样式", typeof(LvListStyle), Icons.全选) { Desc = "定义像表格一样的行、不规则格布局的行，并且采用水平填充式的列表，只垂直滚动" },
            new Nav("内置单元格UI", typeof(LvCellUI), Icons.书籍) { Desc = "内置单元格UI的所有方法" },
            new Nav("自定义单元格UI", typeof(LvCustomCellUI), Icons.Bug) { Desc = "自定义样式、UI，支持多个方法同用，支持重写内置方法" },
            new Nav("自定义行样式", typeof(LvItemStyle), Icons.修改) { Desc = "定义行样式、单元格样式" },
            new Nav("报表预览导出打印", typeof(LvExpPrint), Icons.Excel) { Desc = "预览及导出Excel Pdf及打印" },
            new Nav("Table与报表", typeof(TableToRpt), Icons.Excel) { Desc = "Table数据预览及导出Excel Pdf及打印" },
            new Nav("自动加载表格列设置", typeof(LvSaveCols), Icons.表格) { Desc = "表格视图时自动保存列设置，包括调整后的列宽、列序、隐藏列，以后显示时自动加载" },
            new Nav("下拉刷新", typeof(LvPullToRefresh), Icons.刷新) { Desc = "只支持触摸模式" },
            new Nav("行模板选择", typeof(LvListSelector), Icons.划卡) { Desc = "根据数据动态选择行模板" },
            new Nav("动态行内容", typeof(LvRowView), Icons.划卡) { Desc = "根据数据动态生成行UI内容" },
            new Nav("分组模板", typeof(LvGroupTemplate), Icons.录音) { Desc = "设置分组模板" },
            new Nav("过滤", typeof(LvFilter), Icons.漏斗) { Desc = "三种方式：过滤回调、支持linq的where过滤串、筛选框过滤" },
            new Nav("上下文菜单", typeof(LvContextMenu), Icons.公告) { Desc = "触发上下文菜单的方式：右键、悬停、点击标识按钮" },
            new Nav("外部ScrollViewer", typeof(LvInScrollViewer), Icons.乐谱) { Desc = "外部嵌套ScrollViewer，和其他元素一起滚动" },
        };
    }
}
