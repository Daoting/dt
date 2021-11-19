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
    /// Returns the <see cref="T:Dt.CalcEngine.CalcArray" /> predicted exponential growth by using existing data.
    /// </summary>
    public class CalcGrowthFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Indicates whether the Evaluate method can process an array arguments.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the Evaluate method can process an array arguments; 
        /// otherwise, <see langword="false" />.
        /// </value>
        public override bool AcceptsArray(int i)
        {
            return (i != 3);
        }

        /// <summary>
        /// Indicates whether the Evaluate method can process missing arguments.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the Evaluate method can process missing arguments; 
        /// otherwise, <see langword="false" />.
        /// </value>
        public override bool AcceptsMissingArgument(int i)
        {
            if ((i != 1) && (i != 2))
            {
                return (i == 3);
            }
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
            return (i != 3);
        }

        /// <summary>
        /// Returns the <see cref="T:Dt.CalcEngine.CalcArray" /> predicted exponential growth by using existing data.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 - 4 items: known_y's, [known_x's], [new_x's], [const].
        /// </para>
        /// <para>
        /// Known_y's is the set of y-values you already know in the relationship y = b*m^x.
        /// </para>
        /// <para>
        /// Known_x's is an optional set of x-values that you may already know in the relationship y = b*m^x.
        /// </para>
        /// <para>
        /// New_x's are new x-values for which you want GROWTH to return corresponding y-values.
        /// </para>
        /// <para>
        /// Const is a logical value specifying whether to force the constant b to equal 1.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:Dt.CalcEngine.CalcArray" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            int rowCount;
            int columnCount;
            double[] numArray2;
            double[,] numArray3;
            base.CheckArgumentsLength(args);
            CalcArray array = CalcConvert.ToArray(args[0]);
            CalcArray array2 = CalcHelper.ArgumentExists(args, 1) ? CalcConvert.ToArray(args[1]) : new DefaultKnownX(array.RowCount, array.ColumnCount);
            CalcArray array3 = CalcHelper.ArgumentExists(args, 2) ? CalcConvert.ToArray(args[2]) : array2;
            bool flag = CalcHelper.ArgumentExists(args, 3) ? CalcConvert.ToBool(args[3]) : true;
            for (int i = 0; i < array.RowCount; i++)
            {
                for (int num2 = 0; num2 < array.ColumnCount; num2++)
                {
                    object obj2 = array.GetValue(i, num2);
                    if (obj2 is CalcError)
                    {
                        return obj2;
                    }
                    if (!CalcConvert.IsNumber(obj2))
                    {
                        return CalcErrors.Value;
                    }
                    if (CalcConvert.ToDouble(obj2) <= 0.0)
                    {
                        return CalcErrors.Number;
                    }
                }
            }
            for (int j = 0; j < array2.RowCount; j++)
            {
                for (int num4 = 0; num4 < array2.ColumnCount; num4++)
                {
                    object obj3 = array2.GetValue(j, num4);
                    if (obj3 is CalcError)
                    {
                        return obj3;
                    }
                    if (!CalcConvert.IsNumber(obj3))
                    {
                        return CalcErrors.Value;
                    }
                }
            }
            for (int k = 0; k < array3.RowCount; k++)
            {
                for (int num6 = 0; num6 < array3.ColumnCount; num6++)
                {
                    object obj4 = array3.GetValue(k, num6);
                    if (obj4 is CalcError)
                    {
                        return obj4;
                    }
                    if (!CalcConvert.IsNumber(obj4))
                    {
                        return CalcErrors.Value;
                    }
                }
            }
            if ((array.RowCount == array2.RowCount) && (array.ColumnCount == array2.ColumnCount))
            {
                double num12;
                double num13;
                double num7 = array2.RowCount * array2.ColumnCount;
                double num8 = 0.0;
                double num9 = 0.0;
                double num10 = 0.0;
                double num11 = 0.0;
                for (int num14 = 0; num14 < array2.RowCount; num14++)
                {
                    for (int num15 = 0; num15 < array2.ColumnCount; num15++)
                    {
                        double num16;
                        double num17;
                        if (!CalcConvert.TryToDouble(array2.GetValue(num14, num15), out num16, true) || !CalcConvert.TryToDouble(array.GetValue(num14, num15), out num17, true))
                        {
                            return CalcErrors.Value;
                        }
                        num17 = Math.Log(num17);
                        num8 += num16;
                        num9 += num16 * num16;
                        num10 += num17;
                        num11 += num16 * num17;
                    }
                }
                if (flag)
                {
                    num12 = ((num7 * num11) - (num8 * num10)) / ((num7 * num9) - (num8 * num8));
                    num13 = ((num10 * num9) - (num8 * num11)) / ((num7 * num9) - (num8 * num8));
                }
                else
                {
                    num12 = num11 / num9;
                    num13 = 0.0;
                }
                double[,] numArray = new double[array3.RowCount, array3.ColumnCount];
                for (int num18 = 0; num18 < array3.RowCount; num18++)
                {
                    for (int num19 = 0; num19 < array3.ColumnCount; num19++)
                    {
                        double num20;
                        if (!CalcConvert.TryToDouble(array3.GetValue(num18, num19), out num20, true))
                        {
                            return CalcErrors.Value;
                        }
                        numArray[num18, num19] = Math.Exp((num12 * num20) + num13);
                    }
                }
                return new ConcreteArray(numArray);
            }
            if (((array.ColumnCount != 1) || (array.RowCount != array2.RowCount)) && ((array.RowCount != 1) || (array.ColumnCount != array2.ColumnCount)))
            {
                return CalcErrors.NotAvailable;
            }
            if (array.ColumnCount == 1)
            {
                rowCount = array2.RowCount;
                columnCount = array2.ColumnCount;
                numArray3 = new double[rowCount, columnCount];
                numArray2 = new double[rowCount];
                for (int num23 = 0; num23 < rowCount; num23++)
                {
                    double num24;
                    if (!CalcConvert.TryToDouble(array.GetValue(num23, 0), out num24, true))
                    {
                        return CalcErrors.Value;
                    }
                    numArray2[num23] = Math.Log(num24);
                }
                for (int num25 = 0; num25 < rowCount; num25++)
                {
                    for (int num26 = 0; num26 < columnCount; num26++)
                    {
                        double num27;
                        if (!CalcConvert.TryToDouble(array2.GetValue(num25, num26), out num27, true))
                        {
                            return CalcErrors.Value;
                        }
                        numArray3[num25, num26] = num27;
                    }
                }
            }
            else
            {
                rowCount = array2.ColumnCount;
                columnCount = array2.RowCount;
                numArray3 = new double[rowCount, columnCount];
                numArray2 = new double[rowCount];
                for (int num28 = 0; num28 < rowCount; num28++)
                {
                    double num29;
                    if (!CalcConvert.TryToDouble(array.GetValue(0, num28), out num29, true))
                    {
                        return CalcErrors.Value;
                    }
                    numArray2[num28] = num29;
                }
                for (int num30 = 0; num30 < rowCount; num30++)
                {
                    for (int num31 = 0; num31 < columnCount; num31++)
                    {
                        double num32;
                        if (!CalcConvert.TryToDouble(array2.GetValue(num31, num30), out num32, true))
                        {
                            return CalcErrors.Value;
                        }
                        numArray3[num30, num31] = num32;
                    }
                }
            }
            double[,] numArray4 = new double[columnCount + 1, columnCount + 2];
            double[] numArray5 = new double[columnCount + 2];
            for (int m = 0; m < rowCount; m++)
            {
                numArray5[columnCount + 1] += numArray2[m] * numArray2[m];
                numArray4[0, columnCount + 1] += numArray2[m];
                numArray5[0] = numArray4[0, columnCount + 1];
                for (int num34 = 0; num34 < columnCount; num34++)
                {
                    numArray4[0, num34 + 1] += numArray3[m, num34];
                    numArray4[num34 + 1, 0] = numArray4[0, num34 + 1];
                    numArray4[num34 + 1, columnCount + 1] += numArray3[m, num34] * numArray2[m];
                    numArray5[num34 + 1] = numArray4[num34 + 1, columnCount + 1];
                    for (int num35 = num34; num35 < columnCount; num35++)
                    {
                        numArray4[num35 + 1, num34 + 1] += numArray3[m, num34] * numArray3[m, num35];
                        numArray4[num34 + 1, num35 + 1] = numArray4[num35 + 1, num34 + 1];
                    }
                }
            }
            numArray4[0, 0] = rowCount;
            if (flag)
            {
                for (int num36 = 0; num36 < (columnCount + 1); num36++)
                {
                    double num38;
                    int num37 = num36;
                    while ((num37 < (columnCount + 1)) && (numArray4[num37, num36] == 0.0))
                    {
                        num37++;
                    }
                    if (num37 >= (columnCount + 1))
                    {
                        return CalcErrors.NotAvailable;
                    }
                    for (int num39 = 0; num39 < (columnCount + 2); num39++)
                    {
                        num38 = numArray4[num36, num39];
                        numArray4[num36, num39] = numArray4[num37, num39];
                        numArray4[num37, num39] = num38;
                    }
                    num38 = 1.0 / numArray4[num36, num36];
                    for (int num40 = 0; num40 < (columnCount + 2); num40++)
                    {
                        numArray4[num36, num40] *= num38;
                    }
                    for (num37 = 0; num37 < (columnCount + 1); num37++)
                    {
                        if (num37 != num36)
                        {
                            num38 = -numArray4[num37, num36];
                            for (int num41 = 0; num41 < (columnCount + 2); num41++)
                            {
                                numArray4[num37, num41] += num38 * numArray4[num36, num41];
                            }
                        }
                    }
                }
            }
            else
            {
                for (int num42 = 1; num42 < (columnCount + 1); num42++)
                {
                    double num44;
                    int num43 = num42;
                    while ((num43 < (columnCount + 1)) && (numArray4[num43, num42] == 0.0))
                    {
                        num43++;
                    }
                    if (num43 >= (columnCount + 1))
                    {
                        return CalcErrors.NotAvailable;
                    }
                    for (int num45 = 1; num45 < (columnCount + 2); num45++)
                    {
                        num44 = numArray4[num42, num45];
                        numArray4[num42, num45] = numArray4[num43, num45];
                        numArray4[num43, num45] = num44;
                    }
                    num44 = 1.0 / numArray4[num42, num42];
                    for (int num46 = 1; num46 < (columnCount + 2); num46++)
                    {
                        numArray4[num42, num46] *= num44;
                    }
                    for (num43 = 1; num43 < (columnCount + 1); num43++)
                    {
                        if (num43 != num42)
                        {
                            num44 = -numArray4[num43, num42];
                            for (int num47 = 1; num47 < (columnCount + 2); num47++)
                            {
                                numArray4[num43, num47] += num44 * numArray4[num42, num47];
                            }
                        }
                    }
                    numArray4[0, columnCount + 1] = 0.0;
                }
            }
            if (array.ColumnCount == 1)
            {
                double[,] numArray6 = new double[array3.RowCount, 1];
                for (int num48 = 0; num48 < array3.RowCount; num48++)
                {
                    double d = numArray4[0, columnCount + 1];
                    for (int num50 = 0; num50 < columnCount; num50++)
                    {
                        double num51;
                        if (!CalcConvert.TryToDouble(array3.GetValue(num48, num50), out num51, true))
                        {
                            return CalcErrors.Value;
                        }
                        d += numArray4[num50 + 1, columnCount + 1] * num51;
                    }
                    numArray6[num48, 0] = Math.Exp(d);
                }
                return new ConcreteArray(numArray6);
            }
            double[,] values = new double[1, array3.ColumnCount];
            for (int n = 0; n < array3.ColumnCount; n++)
            {
                double num53 = numArray4[0, columnCount + 1];
                for (int num54 = 0; num54 < columnCount; num54++)
                {
                    double num55;
                    if (!CalcConvert.TryToDouble(array3.GetValue(num54, n), out num55, true))
                    {
                        return CalcErrors.Value;
                    }
                    num53 += numArray4[num54 + 1, columnCount + 1] * num55;
                }
                values[0, n] = Math.Exp(num53);
            }
            return new ConcreteArray(values);
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
                return 4;
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
                return "GROWTH";
            }
        }

        /// <summary>
        /// A concrete implementation of the CalcArray class.
        /// </summary>
        private class ConcreteArray : CalcArray
        {
            private double[,] values;

            public ConcreteArray(double[,] values)
            {
                this.values = values;
            }

            public override object GetValue(int row, int column)
            {
                return (double) this.values[row, column];
            }

            public override int ColumnCount
            {
                get
                {
                    return this.values.GetLength(1);
                }
            }

            public override int RowCount
            {
                get
                {
                    return this.values.GetLength(0);
                }
            }
        }

        /// <summary>
        /// A concrete implementation of the CalcArray class.
        /// </summary>
        private class DefaultKnownX : CalcArray
        {
            private int columnCount;
            private int rowCount;

            public DefaultKnownX(int rowCount, int columnCount)
            {
                this.rowCount = rowCount;
                this.columnCount = columnCount;
            }

            public override object GetValue(int row, int column)
            {
                return (double) (((row * this.columnCount) + column) + 1);
            }

            public override int ColumnCount
            {
                get
                {
                    return this.columnCount;
                }
            }

            public override int RowCount
            {
                get
                {
                    return this.rowCount;
                }
            }
        }
    }
}

