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
    /// Counts the number of cells within a range that meet the given criteria.
    /// </summary>
    public class CalcCountIfFunction : CalcBuiltinFunction
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
            return (i == 0);
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
            return (i == 0);
        }

        /// <summary>
        /// Counts the number of cells within a range that meet the given criteria.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 items: range, criteria.
        /// </para>
        /// <para>
        /// Range is one or more cells to count, including numbers or names, arrays,
        /// or references that contain numbers. Blank and text values are ignored.
        /// </para>
        /// <para>
        /// Criteria  is the criteria in the form of a number, expression, cell
        /// reference, or text that defines which cells will be counted.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Object" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            object range = args[0];
            object o = args[1];
            if (!ArrayHelper.IsArrayOrRange(o))
            {
                return this.EvaluateSingleCriteria(range, o);
            }
            int rowCount = ArrayHelper.GetRowCount(o, 0);
            int columnCount = ArrayHelper.GetColumnCount(o, 0);
            double[,] numArray = new double[rowCount, columnCount];
            for (int i = 0; i < rowCount; i++)
            {
                for (int k = 0; k < columnCount; k++)
                {
                    double num5;
                    object obj4 = this.EvaluateSingleCriteria(range, ArrayHelper.GetValue(o, i, k, 0));
                    if (CalcConvert.IsError(obj4))
                    {
                        return obj4;
                    }
                    if (!CalcConvert.TryToDouble(obj4, out num5, true))
                    {
                        return CalcErrors.Value;
                    }
                    numArray[i, k] = num5;
                }
            }
            double num6 = 0.0;
            for (int j = 0; j < rowCount; j++)
            {
                for (int m = 0; m < columnCount; m++)
                {
                    num6 += numArray[j, m];
                }
            }
            return (double) num6;
        }

        private object EvaluateSingleCriteria(object range, object criteria)
        {
            double num = 0.0;
            Func<object, bool> func = MathHelper.ParseCriteria(criteria);
            for (short i = 0; i < ArrayHelper.GetRangeCount(range); i++)
            {
                for (int j = 0; j < ArrayHelper.GetLength(range, i); j++)
                {
                    object arg = ArrayHelper.GetValue(range, j, i);
                    if ((func != null) && func(arg))
                    {
                        num++;
                    }
                }
            }
            return CalcConvert.ToResult(num);
        }

        /// <summary>
        /// Gets the maximum number of arguments for the function.
        /// </summary>
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
        public override int MinArgs
        {
            get
            {
                return 2;
            }
        }

        /// <summary>
        /// Gets the name of the function.
        /// </summary>
        public override string Name
        {
            get
            {
                return "COUNTIF";
            }
        }
    }
}

