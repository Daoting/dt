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
    /// Counts the number of cells within a range that meet multiple criteria.
    /// </summary>
    public class CalcCountIfsFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Determines whether the function accepts array values
        /// for the specified argument.
        /// </summary>
        /// <param name="i">Index of the argument</param>
        /// <returns>
        /// <see langword="true" /> if the function accepts array values
        /// for the specified argument; <see langword="false" /> otherwise.
        /// </returns>
        public override bool AcceptsArray(int i)
        {
            return ((i % 2) == 0);
        }

        /// <summary>
        /// Determines whether the function accepts CalcReference values
        /// for the specified argument.
        /// </summary>
        /// <param name="i">Index of the argument</param>
        /// <returns>
        /// <see langword="true" /> if the function accepts CalcReference values
        /// for the specified argument; <see langword="false" /> otherwise.
        /// </returns>
        public override bool AcceptsReference(int i)
        {
            return ((i % 2) == 0);
        }

        private bool AssertParams(object[] args)
        {
            if ((args.Length % 2) != 0)
            {
                return false;
            }
            int rowCount = ArrayHelper.GetRowCount(args[0], 0);
            int columnCount = ArrayHelper.GetColumnCount(args[0], 0);
            for (int i = 2; i < args.Length; i += 2)
            {
                if (rowCount != ArrayHelper.GetRowCount(args[i], 0))
                {
                    return false;
                }
                if (columnCount != ArrayHelper.GetColumnCount(args[i], 0))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Counts the number of cells within a range that meet multiple criteria.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 - 255 items: range1, criteria1, [range2], [criteria2], ..
        /// </para>
        /// <para>
        /// Range1, range2, ... are 1 to 127 ranges in which to evaluate the associated criteria.
        /// </para>
        /// <para>
        /// Criteria1, criteria2, ... are 1 to 127 criteria in the form of a number,
        /// expression, cell reference, or text that define which cells will be counted .
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Object" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            if (!this.AssertParams(args))
            {
                return CalcErrors.Value;
            }
            double num = 0.0;
            int length = ArrayHelper.GetLength(args[0], 0);
            for (int i = 0; i < length; i++)
            {
                bool flag = true;
                for (int j = 0; j < args.Length; j += 2)
                {
                    object o = args[j];
                    object criteria = args[j + 1];
                    Func<object, bool> func = MathHelper.ParseCriteria(criteria);
                    object arg = ArrayHelper.GetValue(o, i, 0);
                    flag = (func == null) ? false : func(arg);
                    if (!flag)
                    {
                        break;
                    }
                }
                if (flag)
                {
                    num++;
                }
            }
            return CalcConvert.ToResult(num);
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
                return 0xff;
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
                return "COUNTIFS";
            }
        }
    }
}

