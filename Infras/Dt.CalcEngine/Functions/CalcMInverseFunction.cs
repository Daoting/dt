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
    /// Returns the inverse matrix for the matrix stored in an array.
    /// </summary>
    public class CalcMInverseFunction : CalcBuiltinFunction
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
        /// Returns the inverse matrix for the matrix stored in an array.
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
            double[,] values = new double[rowCount, rowCount];
            for (int i = 0; i < rowCount; i++)
            {
                for (int n = 0; n < rowCount; n++)
                {
                    object obj2 = array.GetValue(i, n);
                    if (CalcConvert.IsError(obj2))
                    {
                        return obj2;
                    }
                    if (!CalcConvert.IsNumber(obj2))
                    {
                        return CalcErrors.Value;
                    }
                    numArray[i, n] = CalcConvert.ToDouble(obj2);
                }
            }
            for (int j = 0; j < rowCount; j++)
            {
                for (int num5 = 0; num5 < rowCount; num5++)
                {
                    values[j, num5] = (j == num5) ? 1.0 : 0.0;
                }
            }
            for (int k = 0; k < rowCount; k++)
            {
                if (numArray[k, k] == 0.0)
                {
                    bool flag = false;
                    for (int num7 = k + 1; !flag && (num7 < rowCount); num7++)
                    {
                        if (numArray[num7, k] != 0.0)
                        {
                            for (int num8 = k; num8 < rowCount; num8++)
                            {
                                double num9 = numArray[k, num8];
                                numArray[k, num8] = numArray[num7, num8];
                                numArray[num7, num8] = num9;
                            }
                            for (int num10 = 1; num10 < rowCount; num10++)
                            {
                                double num11 = values[k, num10];
                                values[k, num10] = values[num7, num10];
                                values[num7, num10] = num11;
                            }
                            flag = true;
                        }
                    }
                    if (!flag)
                    {
                        return CalcErrors.Number;
                    }
                }
                for (int num12 = 0; num12 < rowCount; num12++)
                {
                    if ((num12 != k) && (numArray[num12, k] != 0.0))
                    {
                        double num13 = numArray[num12, k] / numArray[k, k];
                        for (int num14 = k; num14 < rowCount; num14++)
                        {
                            numArray[num12, num14] -= num13 * numArray[k, num14];
                        }
                        for (int num15 = 0; num15 < rowCount; num15++)
                        {
                            values[num12, num15] -= num13 * values[k, num15];
                        }
                    }
                }
            }
            for (int m = 0; m < rowCount; m++)
            {
                double num17 = numArray[m, m];
                for (int num18 = 0; num18 < rowCount; num18++)
                {
                    values[m, num18] /= num17;
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
                return "MINVERSE";
            }
        }
    }
}

