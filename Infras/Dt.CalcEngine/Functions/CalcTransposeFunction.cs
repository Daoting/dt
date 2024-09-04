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
    /// Returns a vertical range of cells as a horizontal range, or vice versa.
    /// </summary>
    /// <remarks>
    /// TRANSPOSE must be entered as an array formula in a range that has the 
    /// same number of rows and columns, respectively, as an array has columns 
    /// and rows. Use TRANSPOSE to shift the vertical and horizontal orientation 
    /// of an array on a worksheet.
    /// </remarks>
    public class CalcTransposeFunction : CalcBuiltinFunction
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
            return true;
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
            return true;
        }

        /// <summary>
        /// Returns a vertical range of cells as a horizontal range, or vice versa.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 item: array.
        /// </para>
        /// <para>
        /// Array is an array or range of cells on a worksheet that you want to transpose.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Object" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            return new TransposedArray(args[0]);
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
                return "TRANSPOSE";
            }
        }

        private class TransposedArray : CalcArray
        {
            private object _array;

            public TransposedArray(object array)
            {
                this._array = array;
            }

            public override object GetValue(int row, int column)
            {
                return ArrayHelper.GetValue(this._array, column, row, 0);
            }

            public override int ColumnCount
            {
                get
                {
                    return ArrayHelper.GetRowCount(this._array, 0);
                }
            }

            public override int RowCount
            {
                get
                {
                    return ArrayHelper.GetColumnCount(this._array, 0);
                }
            }
        }
    }
}

