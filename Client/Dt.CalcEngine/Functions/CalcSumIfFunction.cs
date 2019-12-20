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
    /// Adds the cells specified by a given criteria.
    /// </summary>
    public class CalcSumIfFunction : CalcBuiltinFunction
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
        /// <param name="i">Index of the argument</param>
        /// <value>
        /// <see langword="true" /> if the Evaluate method can process missing arguments; 
        /// otherwise, <see langword="false" />.
        /// </value>
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
        /// Adds the cells specified by a given criteria.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 - 3 items: range, criteria, [sum_range].
        /// </para>
        /// <para>
        /// Range is the range of cells that you want evaluated by criteria.
        /// </para>
        /// <para>
        /// Criteria is the criteria in the form of a number, expression,
        /// or text that defines which cells will be added.
        /// </para>
        /// <para>
        /// [Sum_range] are the actual cells to add if their corresponding
        /// cells in range match criteria.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            object range = args[0];
            object criteria = args[1];
            if (criteria == null)
            {
                throw new ArgumentException("criteria");
            }
            object sumRange = CalcHelper.ArgumentExists(args, 2) ? args[2] : args[0];
            CalcReference objA = args[1] as CalcReference;
            if (!object.ReferenceEquals(objA, null))
            {
                int num = objA.GetRowCount(0);
                int num2 = objA.GetColumnCount(0);
                if ((num == 1) && (num2 == 1))
                {
                    return EvaluateSingleCriteria(objA.GetValue(0, 0, 0), range, sumRange);
                }
                return new TernaryCompositeConcreteReference(objA.GetSource(), objA.GetRow(0), objA.GetColumn(0), num, num2, new Func<object, object, object, object>(CalcSumIfFunction.EvaluateSingleCriteria), range, sumRange);
            }
            CalcArray array = args[1] as CalcArray;
            if (object.ReferenceEquals(array, null))
            {
                return EvaluateSingleCriteria(criteria, range, sumRange);
            }
            int rowCount = array.RowCount;
            int columnCount = array.ColumnCount;
            object[,] values = new object[rowCount, columnCount];
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    values[i, j] = EvaluateSingleCriteria(array.GetValue(i, j), range, sumRange);
                }
            }
            return new ConcreteArray<object>(values);
        }

        private static object EvaluateSingleCriteria(object criteria, object range, object sumRange)
        {
            double num = 0.0;
            if (criteria == null)
            {
                return (double) num;
            }
            Func<object, bool> func = MathHelper.ParseCriteria(criteria);
            if ((ArrayHelper.GetRowCount(range, 0) != ArrayHelper.GetRowCount(sumRange, 0)) || (ArrayHelper.GetColumnCount(range, 0) != ArrayHelper.GetColumnCount(sumRange, 0)))
            {
                return CalcErrors.Value;
            }
            for (int i = 0; i < ArrayHelper.GetLength(range, 0); i++)
            {
                object arg = ArrayHelper.GetValue(range, i, 0);
                if (arg is CalcError)
                {
                    return arg;
                }
                if ((func != null) && func(arg))
                {
                    object obj3 = ArrayHelper.GetValue(sumRange, i, 0);
                    if (CalcConvert.IsNumber(obj3))
                    {
                        num += CalcConvert.ToDouble(obj3);
                    }
                    else if (obj3 is CalcError)
                    {
                        return obj3;
                    }
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
                return "SUMIF";
            }
        }
    }
}

