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
    public sealed partial class TvHome : Win
    {
        public TvHome()
        {
            InitializeComponent();
            _nav.Data = new Nl<Nav>
            {
                new Nav("树", typeof(TvBase), Icons.向下) { Desc = "树控件基础功能" },                                                         
                new Nav("动态加载子节点", typeof(TvDynamicLoading), Icons.顺时针) { Desc = "常用的加载方式" },
                new Nav("视图扩展", typeof(TvViewEx), Icons.全选) { Desc = "自定义节点样式、节点内容" },
                new Nav("节点模板选择器", typeof(TvViewSelector), Icons.排列) { Desc = "动态设置节点模板，节点非虚拟化" },
                new Nav("上下文菜单", typeof(TvContextMenu), Icons.公告) { Desc = "触发上下文菜单的方式：右键、悬停、点击标识按钮" },
                new Nav("外部ScrollViewer", typeof(TvInScrollViewer), Icons.乐谱) { Desc = "外部嵌套ScrollViewer，和其他元素一起滚动" },
            };
        }
    }
}
