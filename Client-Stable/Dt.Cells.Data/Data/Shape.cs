#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.Foundation;
#endregion

namespace Dt.Cells.Data
{
    internal class Shape : ShapeBase
    {
        internal static Windows.Foundation.Rect GetRotatedBounds(double left, double top, double right, double bottom, double rotationAngle, double centerX, double centerY)
        {
            double num = rotationAngle % 360.0;
            Windows.Foundation.Point point = new Windows.Foundation.Point(left, top);
            Windows.Foundation.Point point2 = new Windows.Foundation.Point(left, bottom);
            Windows.Foundation.Point point3 = new Windows.Foundation.Point(right, top);
            Windows.Foundation.Point point4 = new Windows.Foundation.Point(right, bottom);
            Windows.Foundation.Point point5 = Transform(point, centerX, centerY, left, top, num);
            Windows.Foundation.Point point6 = Transform(point2, centerX, centerY, left, top, num);
            Windows.Foundation.Point point7 = Transform(point3, centerX, centerY, left, top, num);
            Windows.Foundation.Point point8 = Transform(point4, centerX, centerY, left, top, num);
            double x = Math.Min(Math.Min(point5.X, point6.X), Math.Min(point7.X, point8.X));
            double num3 = Math.Max(Math.Max(point5.X, point6.X), Math.Max(point7.X, point8.X));
            double y = Math.Min(Math.Min(point5.Y, point6.Y), Math.Min(point7.Y, point8.Y));
            double num5 = Math.Max(Math.Max(point5.Y, point6.Y), Math.Max(point7.Y, point8.Y));
            return new Windows.Foundation.Rect(x, y, num3 - x, num5 - y);
        }

        /// <summary>
        /// Transforms the specified point.
        /// </summary>
        /// <param name="point">The point</param>
        /// <param name="centerX">The center point X</param>
        /// <param name="centerY">The center point Y</param>
        /// <param name="offsetX">The offset point X</param>
        /// <param name="offsetY">The offset point Y</param>
        /// <param name="rotationAngle">The rotation angle</param>
        /// <returns>The transformed point</returns>
        internal static Windows.Foundation.Point Transform(Windows.Foundation.Point point, double centerX, double centerY, double offsetX, double offsetY, double rotationAngle)
        {
            MatrixMock mock = new MatrixMock();
            mock.RotateAt(rotationAngle, offsetX + centerX, offsetY + centerY);
            return mock.Transform(point);
        }
    }
}

