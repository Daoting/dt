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
    /// Returns the matrix product of two arrays.
    /// </summary>
    /// <remarks>
    /// The result is an array with the same number of rows as array1 
    /// and the same number of columns as array2.
    /// </remarks>
    public class CalcMMultFunction : CalcBuiltinFunction
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
        /// Returns the matrix product of two arrays.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 items: array1, array2.
        /// </para>
        /// <para>
        /// Array1, array2 are the arrays you want to multiply.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            CalcArray array = CalcConvert.ToArray(args[0]);
            CalcArray array2 = CalcConvert.ToArray(args[1]);
            int rowCount = array.RowCount;
            int columnCount = array.ColumnCount;
            int num3 = array2.RowCount;
            int num4 = array2.ColumnCount;
            if (columnCount != num3)
            {
                return CalcErrors.Value;
            }
            double[,] numArray = new double[rowCount, columnCount];
            double[,] numArray2 = new double[num3, num4];
            double[,] values = new double[rowCount, num4];
            for (int i = 0; i < rowCount; i++)
            {
                for (int m = 0; m < columnCount; m++)
                {
                    object obj2 = array.GetValue(i, m);
                    if (CalcConvert.IsError(obj2))
                    {
                        return obj2;
                    }
                    if (!CalcConvert.IsNumber(obj2))
                    {
                        return CalcErrors.Value;
                    }
                    numArray[i, m] = CalcConvert.ToDouble(obj2);
                }
            }
            for (int j = 0; j < num3; j++)
            {
                for (int n = 0; n < num4; n++)
                {
                    object obj3 = array2.GetValue(j, n);
                    if (CalcConvert.IsError(obj3))
                    {
                        return obj3;
                    }
                    if (!CalcConvert.IsNumber(obj3))
                    {
                        return CalcErrors.Value;
                    }
                    numArray2[j, n] = CalcConvert.ToDouble(obj3);
                }
            }
            for (int k = 0; k < rowCount; k++)
            {
                for (int num10 = 0; num10 < num4; num10++)
                {
                    double num11 = 0.0;
                    for (int num12 = 0; num12 < num3; num12++)
                    {
                        num11 += numArray[k, num12] * numArray2[num12, num10];
                    }
                    values[k, num10] = num11;
                }
            }
            return new ConcreteArray<double>(values);
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
                return "MMULT";
            }
        }
    }
}

