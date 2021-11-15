#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Windows.Foundation;
using Windows.UI.Xaml;
#endregion

namespace Dt.Charts
{
    internal interface IView2DRenderer : IRenderer
    {
        UIElement[] Generate();
        Rect Measure(Size sz);

        AxisStyle Axis { get; }
    }
}

