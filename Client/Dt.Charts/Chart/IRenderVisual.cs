#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Windows.UI.Xaml;
#endregion

namespace Dt.Charts
{
    internal interface IRenderVisual
    {
        UIElement[] Render(RenderContext rc);

        bool IsClustered { get; }
    }
}

