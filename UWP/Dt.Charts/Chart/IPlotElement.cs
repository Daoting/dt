#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Charts
{
    [EditorBrowsable((EditorBrowsableState) EditorBrowsableState.Never)]
    public interface IPlotElement
    {
        bool IsCompatible(IRenderer rend);
        bool Render(RenderContext rc);
        void SetShape(ShapeStyle shapeStyle);

        bool IsClustered { get; }

        UIElement Label { get; set; }

        Shape LegendShape { get; }

        Windows.UI.Xaml.Style Style { get; set; }
    }
}

