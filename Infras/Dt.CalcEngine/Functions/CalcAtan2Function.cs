#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine;
using System;
#endregion

namespace Dt.CalcEngine.Functions
{
    /// <summary>
    /// Returns the arctangent, or inverse tangent, of the specified x- and y-coordinates.
    /// </summary>
    /// <remarks>
    /// The arctangent is the angle from the x-axis to a line containing 
    /// the origin (0, 0) and a point with coordinates (x_num, y_num). 
    /// The angle is given in radians between -pi and pi, excluding -pi.
    /// </remarks>
    public class CalcAtan2Function : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the arctangent, or inverse tangent, of the specified x- and y-coordinates.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 items: x_num, y_num.
        /// </para>
        /// <para>
        /// X_num is the x-coordinate of the point.
        /// </para>
        /// <para>
        /// Y_num is the y-coordinate of the point.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num;
            double num2;
            base.CheckArgumentsLength(args);
            if (!CalcConvert.TryToDouble(args[0], out num, true) || !CalcConvert.TryToDouble(args[1], out num2, true))
            {
                return CalcErrors.Value;
            }
            if ((num == 0.0) && (num2 == 0.0))
            {
                return CalcErrors.DivideByZero;
            }
            return CalcConvert.ToResult(Math.Atan2(num2, num));
        }

        /// <summary>
        /// Gets the maximum number of arguments for the function.
        /// </summary>
        /// <value>
        /// The maximum number of arguments for the function.
        /// </value>
        public override int MaxArgs
        {
            get
            {
                return 2;
            }
        }

        /// <summary>
        /// Gets the minimum number of arguments for the function.
        /// </summary>
        /// <value>
        /// The minimum number of arguments for the function.
        /// </value>
        public override int MinArgs
        {
            get
            {
                return 2;
            }
        }

        /// <summary>
        /// Gets The name of the function.
        /// </summary>
        /// <value>
        /// The name of the function.
        /// </value>
        public override string Name
        {
            get
            {
                return "ATAN2";
            }
        }
    }
}

