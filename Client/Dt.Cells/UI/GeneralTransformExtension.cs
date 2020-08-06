#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.UI
{
    internal static class GeneralTransformExtension
    {
        internal static Point Transform(this GeneralTransform transform, Point point)
        {
            return transform.TransformPoint(point);
        }
    }
}

