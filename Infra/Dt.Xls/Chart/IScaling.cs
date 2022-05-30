#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Xls.Chart
{
    /// <summary>
    /// Specifies the additional axis scale settings.
    /// </summary>
    public interface IScaling
    {
        /// <summary>
        /// Specifies the logarithmic base for a logarithmic axis.
        /// </summary>
        /// <remarks>
        /// The value should be between 2 and 1000.
        /// </remarks>
        double LogBase { get; set; }

        /// <summary>
        /// Specifies the maximun value of the axis.
        /// </summary>
        double Max { get; set; }

        /// <summary>
        /// Specifies the minimun value of the axis.
        /// </summary>
        double Min { get; set; }

        /// <summary>
        /// Specifies the possible ways to place a picture on a data point, series, wall of floor.
        /// </summary>
        AxisOrientation Orientation { get; set; }
    }
}

