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
#endregion

namespace Dt.Cells.UI
{
    internal static class PointExtensions
    {
        /// <summary>
        /// Computes the offset vector between two points.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <param name="newLocation">The new location.</param>
        /// <returns></returns>
        public static Point Delta(this Point reference, Point newLocation)
        {
            return new Point(newLocation.X - reference.X, newLocation.Y - reference.Y);
        }

        /// <summary>
        /// Computes the offset indicated by the X and Y values of the given <see cref="T:Point" />.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public static double Offset(this Point offset)
        {
            return Math.Sqrt(Math.Pow(offset.X, 2.0) + Math.Pow(offset.Y, 2.0));
        }
    }
}

