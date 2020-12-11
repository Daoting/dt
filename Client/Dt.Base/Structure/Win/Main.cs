#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-03-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.Docking;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 窗口主区，中部停靠项列表，在xaml中标志作用，不加载到可视树
    /// </summary>
    [ContentProperty(Name = nameof(Items))]
    public partial class Main : Control, IPaneList
    {
        /// <summary>
        /// 获取内容元素集合
        /// </summary>
        public PaneList Items { get; } = new PaneList();
    }
}