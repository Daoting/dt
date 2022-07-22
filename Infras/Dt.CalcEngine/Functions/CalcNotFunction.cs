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
    /// Reverses the value of its argument. Use NOT when you want to make sure 
    /// a value is not equal to one particular value.
    /// </summary>
    public class CalcNotFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Reverses the value of its argument.
        /// </summary>
        /// <param name="args">The args contains one item : logical.
        /// Logical is a value or expression that can be evaluated to <see langword="true" />
        /// or <see langword="false" />.</param>
        /// <returns>
        /// A <see cref="T:System.Boolean" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            CalcReference objA = args[0] as CalcReference;
            CalcArray array = args[0] as CalcArray;
            if (!object.ReferenceEquals(objA, null))
            {
                if (objA.RangeCount > 1)
                {
                    return CalcErrors.Value;
                }
                return new UnaryCompositeConcreteReference(objA.GetSource(), objA.GetRow(0), objA.GetColumn(0), objA.GetRowCount(0), objA.GetColumnCount(0), new Func<object, object>(this.EvaluateSingleValue));
            }
            if (object.ReferenceEquals(array, null))
            {
                return this.EvaluateSingleValue(args[0]);
            }
            object[,] values = new object[array.RowCount, array.ColumnCount];
            for (int i = 0; i < array.RowCount; i++)
            {
                for (int j = 0; j < array.ColumnCount; j++)
                {
                    values[i, j] = this.EvaluateSingleValue(array.GetValue(i, j));
                }
            }
            return new ConcreteArray<object>(values);
        }

        private object EvaluateSingleValue(object value)
        {
            bool flag;
            if (!CalcConvert.TryToBool(value, out flag))
            {
                return CalcErrors.Value;
            }
            return (bool) !flag;
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
                return 1;
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
                return 1;
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
                return "NOT";
            }
        }
    }
}

