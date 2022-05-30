#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.UI
{
    internal static class AxisValueUtility
    {
        public static double CalculateMaximum(double minimum, double maximum, double logarithmicBase, bool baseOne)
        {
            maximum = Math.Pow(logarithmicBase, Math.Ceiling(Math.Log(maximum, logarithmicBase)));
            if (baseOne)
            {
                maximum++;
            }
            return maximum;
        }

        public static double CalculateMinimum(double minimum, double maximum, double logarithmicBase)
        {
            minimum = Math.Pow(logarithmicBase, Math.Floor(Math.Log(minimum, logarithmicBase)));
            return minimum;
        }
    }
}

