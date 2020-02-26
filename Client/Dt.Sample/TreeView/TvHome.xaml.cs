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
            _lv.Data = new List<CenterInfo>
            {
                new CenterInfo(Icons.向下, "树", typeof(TvBase), "树控件基础功能"),
                new CenterInfo(Icons.顺时针, "动态加载子节点", typeof(TvDynamicLoading), "常用的加载方式"),
                new CenterInfo(Icons.详细, "视图扩展", typeof(TvViewEx), "自定义节点样式、节点内容"),
                new CenterInfo(Icons.排列, "节点模板选择器", typeof(TvViewSelector), "动态设置节点模板，节点非虚拟化"),
                new CenterInfo(Icons.公告, "上下文菜单", typeof(TvContextMenu), "触发上下文菜单的方式：右键、悬停、点击标识按钮"),
            };
        }
    }
}
