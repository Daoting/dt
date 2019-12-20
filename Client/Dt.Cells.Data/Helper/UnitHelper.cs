#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.Data
{
    internal static class UnitHelper
    {
        private static double? _dpi;
        private static readonly double PointsPerInch = 72.0;

        public static double GetDPI()
        {
            if (!_dpi.HasValue)
            {
                _dpi = new double?(GetSystemDPI());
            }
            return _dpi.Value;
        }

        private static double GetSystemDPI()
        {
            return 96.0;
        }

        public static double PixelToPoint(double pixel)
        {
            return ((pixel * PointsPerInch) / GetDPI());
        }

        public static double PointToPixel(double point)
        {
            return ((point * GetDPI()) / PointsPerInch);
        }
    }
}

