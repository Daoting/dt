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
    /// Returns the matrix determinant of an array.
    /// </summary>
    public class CalcMDetermFunction : CalcBuiltinFunction
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
        /// Returns the matrix determinant of an array.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 item: array.
        /// </para>
        /// <para>
        /// Array is a numeric array with an equal number of rows and columns.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            CalcArray array = CalcConvert.ToArray(args[0]);
            if (array.RowCount != array.ColumnCount)
            {
                return CalcErrors.Value;
            }
            int rowCount = array.RowCount;
            double[,] numArray = new double[rowCount, rowCount];
            for (int i = 0; i < rowCount; i++)
            {
                for (int m = 0; m < rowCount; m++)
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
            double num4 = 1.0;
            for (int j = 0; j < (rowCount - 1); j++)
            {
                if (numArray[j, j] == 0.0)
                {
                    bool flag = false;
                    for (int num6 = j + 1; !flag && (num6 < rowCount); num6++)
                    {
                        if (numArray[num6, j] != 0.0)
                        {
                            for (int num7 = j; num7 < rowCount; num7++)
                            {
                                double num8 = numArray[j, num7];
                                numArray[j, num7] = numArray[num6, num7];
                                numArray[num6, num7] = num8;
                            }
                            num4 *= -1.0;
                            flag = true;
                        }
                    }
                    if (!flag)
                    {
                        return (double) 0.0;
                    }
                }
                for (int n = j + 1; n < rowCount; n++)
                {
                    if (numArray[n, j] != 0.0)
                    {
                        double num10 = numArray[n, j] / numArray[j, j];
                        for (int num11 = j; num11 < rowCount; num11++)
                        {
                            numArray[n, num11] -= num10 * numArray[j, num11];
                        }
                    }
                }
            }
            for (int k = 0; k < rowCount; k++)
            {
                num4 *= numArray[k, k];
            }
            return (double) num4;
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
                return "MDETERM";
            }
        }
    }
}

