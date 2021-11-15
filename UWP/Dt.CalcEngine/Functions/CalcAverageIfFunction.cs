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
    /// Returns the average (arithmetic mean) of all the cells in a range
    /// that meet a given criteria.
    /// </summary>
    public class CalcAverageIfFunction : CalcBuiltinFunction
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
            if (i != 0)
            {
                return (i == 2);
            }
            return true;
        }

        /// <summary>
        /// Indicates whether the Evaluate method can process missing arguments.
        /// </summary>
        public override bool AcceptsMissingArgument(int i)
        {
            return (i == 2);
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
            if (i != 0)
            {
                return (i == 2);
            }
            return true;
        }

        /// <summary>
        /// Returns the average (arithmetic mean) of all the cells in a range
        /// that meet a given criteria.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 - 3 items: range, criteria, [average_range].
        /// </para>
        /// <para>
        /// Range is one or more cells to average, including numbers or names,
        /// arrays, or references that contain numbers.
        /// </para>
        /// <para>
        /// Criteria is the criteria in the form of a number, expression, cell
        /// reference, or text that defines which cells are averaged.
        /// </para>
        /// <para>
        /// [Average_range] is the actual set of cells to average. If omitted,
        /// range is used.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Object" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            object range = args[0];
            object criteria = args[1];
            object sumRange = CalcHelper.ArgumentExists(args, 2) ? args[2] : args[0];
            CalcReference objA = args[1] as CalcReference;
            if (!object.ReferenceEquals(objA, null))
            {
                int num = objA.GetRowCount(0);
                int num2 = objA.GetColumnCount(0);
                object[,] objArray = new object[num, num2];
                for (int j = 0; j < num; j++)
                {
                    for (int k = 0; k < num2; k++)
                    {
                        objArray[j, k] = this.EvaluateSingleCriteria(range, objA.GetValue(0, j, k), sumRange);
                    }
                }
                return new ConcreteArray<object>(objArray);
            }
            CalcArray array = args[1] as CalcArray;
            if (object.ReferenceEquals(array, null))
            {
                return this.EvaluateSingleCriteria(range, criteria, sumRange);
            }
            int rowCount = array.RowCount;
            int columnCount = array.ColumnCount;
            object[,] values = new object[rowCount, columnCount];
            for (int i = 0; i < rowCount; i++)
            {
                for (int m = 0; m < columnCount; m++)
                {
                    values[i, m] = this.EvaluateSingleCriteria(range, array.GetValue(i, m), sumRange);
                }
            }
            return new ConcreteArray<object>(values);
        }

        private object EvaluateSingleCriteria(object range, object criteria, object sumRange)
        {
            double num = 0.0;
            double num2 = 0.0;
            Func<object, bool> func = MathHelper.ParseCriteria(criteria);
            if ((ArrayHelper.GetRowCount(range, 0) != ArrayHelper.GetRowCount(sumRange, 0)) || (ArrayHelper.GetColumnCount(range, 0) != ArrayHelper.GetColumnCount(sumRange, 0)))
            {
                return CalcErrors.Value;
            }
            for (int i = 0; i < ArrayHelper.GetLength(range, 0); i++)
            {
                object arg = ArrayHelper.GetValue(range, i, 0);
                if ((func != null) && func(arg))
                {
                    object obj3 = ArrayHelper.GetValue(sumRange, i, 0);
                    if (CalcConvert.IsNumber(obj3))
                    {
                        num += CalcConvert.ToDouble(obj3);
                        num2++;
                    }
                    else if (obj3 is CalcError)
                    {
                        return obj3;
                    }
                }
            }
            if (num2 == 0.0)
            {
                return CalcErrors.DivideByZero;
            }
            return CalcConvert.ToResult(num / num2);
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
                return 3;
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
                return "AVERAGEIF";
            }
        }
    }
}

