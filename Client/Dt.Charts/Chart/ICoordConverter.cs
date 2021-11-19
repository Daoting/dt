#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System.ComponentModel;
using Windows.Foundation;
#endregion

namespace Dt.Charts
{
    [EditorBrowsable((EditorBrowsableState) EditorBrowsableState.Never)]
    public interface ICoordConverter
    {
        Point ConvertPoint(Point point);
        double ConvertX(double x);
        double ConvertY(double y);
        double ConvertZ(double z);

        Rect DataBounds { get; }

        Rect DataBounds2D { get; }

        Rect ViewBounds { get; }
    }
}

