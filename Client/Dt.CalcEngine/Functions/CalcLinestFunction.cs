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
    /// Returns the <see cref="T:Dt.CalcEngine.CalcArray" /> coordinates for a straight line that best fits your data.
    /// </summary>
    public class CalcLinestFunction : CalcBuiltinFunction
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
            if (i != 0)
            {
                return (i == 1);
            }
            return true;
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
            if (i != 0)
            {
                return (i == 1);
            }
            return true;
        }

        /// <summary>
        /// Returns the <see cref="T:Dt.CalcEngine.CalcArray" /> coordinates for a straight line that best fits your data.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 - 4 items: known_y's, [known_x's], [const], [stats].
        /// </para>
        /// <para>
        /// Known_y's is the set of y-values you already know in the relationship y = mx + b.
        /// </para>
        /// <para>
        /// Known_x's is an optional set of x-values that you may already know in the relationship y = mx + b.
        /// </para>
        /// <para>
        /// Const is a logical value specifying whether to force the constant b to equal 0.
        /// </para>
        /// <para>
        /// Stats   is a logical value specifying whether to return additional regression statistics.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:Dt.CalcEngine.CalcArray" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            int rowCount;
            int columnCount;
            double[] numArray;
            double[,] numArray2;
            base.CheckArgumentsLength(args);
            CalcArray array = CalcConvert.ToArray(args[0]);
            CalcArray array2 = CalcHelper.ArgumentExists(args, 1) ? CalcConvert.ToArray(args[1]) : new DefaultKnownX(array.RowCount, array.ColumnCount);
            bool flag = CalcHelper.ArgumentExists(args, 2) ? CalcConvert.ToBool(args[2]) : true;
            bool flag2 = CalcHelper.ArgumentExists(args, 3) ? CalcConvert.ToBool(args[3]) : false;
            for (int i = 0; i < array.RowCount; i++)
            {
                for (int n = 0; n < array.ColumnCount; n++)
                {
                    if (array.GetValue(i, n) is CalcError)
                    {
                        return array.GetValue(i, n);
                    }
                    if (!CalcConvert.IsNumber(array.GetValue(i, n)))
                    {
                        return CalcErrors.Value;
                    }
                }
            }
            for (int j = 0; j < array2.RowCount; j++)
            {
                for (int num4 = 0; num4 < array2.ColumnCount; num4++)
                {
                    if (array2.GetValue(j, num4) is CalcError)
                    {
                        return array2.GetValue(j, num4);
                    }
                    if (!CalcConvert.IsNumber(array2.GetValue(j, num4)))
                    {
                        return CalcErrors.Value;
                    }
                }
            }
            if (((array.RowCount == array2.RowCount) && (array.ColumnCount == array2.ColumnCount)) && (flag || !flag2))
            {
                double num11;
                double num12;
                double num5 = array2.RowCount * array2.ColumnCount;
                double num6 = 0.0;
                double num7 = 0.0;
                double num8 = 0.0;
                double num9 = 0.0;
                double num10 = 0.0;
                for (int num13 = 0; num13 < array2.RowCount; num13++)
                {
                    for (int num14 = 0; num14 < array2.ColumnCount; num14++)
                    {
                        double num15;
                        double num16;
                        if (!CalcConvert.TryToDouble(array2.GetValue(num13, num14), out num15, true) || !CalcConvert.TryToDouble(array.GetValue(num13, num14), out num16, true))
                        {
                            return CalcErrors.Value;
                        }
                        num6 += num15;
                        num7 += num15 * num15;
                        num8 += num16;
                        num9 += num16 * num16;
                        num10 += num15 * num16;
                    }
                }
                if (flag)
                {
                    num11 = ((num5 * num10) - (num6 * num8)) / ((num5 * num7) - (num6 * num6));
                    num12 = ((num8 * num7) - (num6 * num10)) / ((num5 * num7) - (num6 * num6));
                }
                else
                {
                    num11 = num10 / num7;
                    num12 = 0.0;
                }
                object[,] objArray = new object[flag2 ? 5 : 1, 2];
                objArray[0, 0] = (double) num11;
                objArray[0, 1] = (double) num12;
                if (flag2)
                {
                    double num17 = (num5 * num7) - (num6 * num6);
                    double num18 = (num5 * num9) - (num8 * num8);
                    double num19 = (num5 * num10) - (num6 * num8);
                    double num20 = (num9 - (num12 * num8)) - (num11 * num10);
                    double num21 = (num19 * num19) / (num17 * num18);
                    if (num5 < 3.0)
                    {
                        objArray[1, 0] = CalcErrors.Number;
                        objArray[1, 1] = CalcErrors.Number;
                        objArray[2, 1] = CalcErrors.Number;
                        objArray[3, 0] = CalcErrors.Number;
                    }
                    else
                    {
                        objArray[1, 0] = (double) Math.Sqrt((num20 * num5) / (num17 * (num5 - 2.0)));
                        objArray[1, 1] = (double) Math.Sqrt((num20 * num7) / (num17 * (num5 - 2.0)));
                        objArray[2, 1] = (double) Math.Sqrt((num18 - ((num19 * num19) / num17)) / (num5 * (num5 - 2.0)));
                        if (num21 == 1.0)
                        {
                            objArray[3, 0] = CalcErrors.Number;
                        }
                        else
                        {
                            objArray[3, 0] = (num21 * (num5 - 2.0)) / (1.0 - num21);
                        }
                    }
                    objArray[2, 0] = (double) num21;
                    objArray[3, 1] = num5 - 2.0;
                    objArray[4, 0] = (num18 / num5) - num20;
                    objArray[4, 1] = (double) num20;
                }
                return new ConcreteArray(objArray);
            }
            if (((array.ColumnCount != 1) || (array.RowCount != array2.RowCount)) && ((array.RowCount != 1) || (array.ColumnCount != array2.ColumnCount)))
            {
                return CalcErrors.Number;
            }
            if (array.ColumnCount == 1)
            {
                rowCount = array2.RowCount;
                columnCount = array2.ColumnCount;
                numArray2 = new double[rowCount, columnCount];
                numArray = new double[rowCount];
                for (int num24 = 0; num24 < rowCount; num24++)
                {
                    double num25;
                    if (!CalcConvert.TryToDouble(array.GetValue(num24, 0), out num25, true))
                    {
                        return CalcErrors.Value;
                    }
                    numArray[num24] = num25;
                }
                for (int num26 = 0; num26 < rowCount; num26++)
                {
                    for (int num27 = 0; num27 < columnCount; num27++)
                    {
                        double num28;
                        if (!CalcConvert.TryToDouble(array2.GetValue(num26, num27), out num28, true))
                        {
                            return CalcErrors.Value;
                        }
                        numArray2[num26, num27] = num28;
                    }
                }
            }
            else
            {
                rowCount = array2.ColumnCount;
                columnCount = array2.RowCount;
                numArray2 = new double[rowCount, columnCount];
                numArray = new double[rowCount];
                for (int num29 = 0; num29 < rowCount; num29++)
                {
                    double num30;
                    if (!CalcConvert.TryToDouble(array.GetValue(0, num29), out num30, true))
                    {
                        return CalcErrors.Value;
                    }
                    numArray[num29] = num30;
                }
                for (int num31 = 0; num31 < rowCount; num31++)
                {
                    for (int num32 = 0; num32 < columnCount; num32++)
                    {
                        double num33;
                        if (!CalcConvert.TryToDouble(array2.GetValue(num32, num31), out num33, true))
                        {
                            return CalcErrors.Value;
                        }
                        numArray2[num31, num32] = num33;
                    }
                }
            }
            double[,] numArray3 = new double[columnCount + 1, columnCount + 2];
            double[] numArray4 = new double[columnCount + 2];
            double[,] numArray5 = flag2 ? ((double[,]) new double[columnCount + 1, columnCount + 1]) : null;
            for (int k = 0; k < rowCount; k++)
            {
                numArray4[columnCount + 1] += numArray[k] * numArray[k];
                numArray3[0, columnCount + 1] += numArray[k];
                numArray4[0] = numArray3[0, columnCount + 1];
                for (int num35 = 0; num35 < columnCount; num35++)
                {
                    numArray3[0, num35 + 1] += numArray2[k, num35];
                    numArray3[num35 + 1, 0] = numArray3[0, num35 + 1];
                    numArray3[num35 + 1, columnCount + 1] += numArray2[k, num35] * numArray[k];
                    numArray4[num35 + 1] = numArray3[num35 + 1, columnCount + 1];
                    for (int num36 = num35; num36 < columnCount; num36++)
                    {
                        numArray3[num36 + 1, num35 + 1] += numArray2[k, num35] * numArray2[k, num36];
                        numArray3[num35 + 1, num36 + 1] = numArray3[num36 + 1, num35 + 1];
                    }
                }
            }
            numArray3[0, 0] = rowCount;
            if (flag2)
            {
                for (int num37 = 0; num37 < (columnCount + 1); num37++)
                {
                    numArray5[num37, num37] = 1.0;
                }
            }
            if (flag)
            {
                for (int num38 = 0; num38 < (columnCount + 1); num38++)
                {
                    if (numArray3[num38, num38] == 0.0)
                    {
                        bool flag3 = false;
                        for (int num39 = num38 + 1; !flag3 && (num39 < (columnCount + 1)); num39++)
                        {
                            if (numArray3[num39, num38] != 0.0)
                            {
                                for (int num40 = 0; num40 < (columnCount + 2); num40++)
                                {
                                    double num41 = numArray3[num38, num40];
                                    numArray3[num38, num40] = numArray3[num39, num40];
                                    numArray3[num39, num40] = num41;
                                }
                                if (flag2)
                                {
                                    for (int num42 = 0; num42 < (columnCount + 1); num42++)
                                    {
                                        double num43 = numArray5[num38, num42];
                                        numArray5[num38, num42] = numArray5[num39, num42];
                                        numArray5[num39, num42] = num43;
                                    }
                                }
                                flag3 = true;
                            }
                        }
                        if (!flag3)
                        {
                            return CalcErrors.Number;
                        }
                    }
                    double num44 = 1.0 / numArray3[num38, num38];
                    for (int num45 = 0; num45 < (columnCount + 2); num45++)
                    {
                        numArray3[num38, num45] *= num44;
                    }
                    if (flag2)
                    {
                        for (int num46 = 0; num46 < (columnCount + 1); num46++)
                        {
                            numArray5[num38, num46] *= num44;
                        }
                    }
                    for (int num47 = 0; num47 < (columnCount + 1); num47++)
                    {
                        if (num47 != num38)
                        {
                            num44 = -numArray3[num47, num38];
                            for (int num48 = 0; num48 < (columnCount + 2); num48++)
                            {
                                numArray3[num47, num48] += num44 * numArray3[num38, num48];
                            }
                            if (flag2)
                            {
                                for (int num49 = 0; num49 < (columnCount + 1); num49++)
                                {
                                    numArray5[num47, num49] += num44 * numArray5[num38, num49];
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                for (int num50 = 1; num50 < (columnCount + 1); num50++)
                {
                    if (numArray3[num50, num50] == 0.0)
                    {
                        bool flag4 = false;
                        for (int num51 = num50 + 1; !flag4 && (num51 < (columnCount + 1)); num51++)
                        {
                            if (numArray3[num51, num50] != 0.0)
                            {
                                for (int num52 = 0; num52 < (columnCount + 2); num52++)
                                {
                                    double num53 = numArray3[num50, num52];
                                    numArray3[num50, num52] = numArray3[num51, num52];
                                    numArray3[num51, num52] = num53;
                                }
                                if (flag2)
                                {
                                    for (int num54 = 0; num54 < (columnCount + 1); num54++)
                                    {
                                        double num55 = numArray5[num50, num54];
                                        numArray5[num50, num54] = numArray5[num51, num54];
                                        numArray5[num51, num54] = num55;
                                    }
                                }
                                flag4 = true;
                            }
                        }
                        if (!flag4)
                        {
                            return CalcErrors.Number;
                        }
                    }
                    double num56 = 1.0 / numArray3[num50, num50];
                    for (int num57 = 1; num57 < (columnCount + 2); num57++)
                    {
                        numArray3[num50, num57] *= num56;
                    }
                    if (flag2)
                    {
                        for (int num58 = 1; num58 < (columnCount + 1); num58++)
                        {
                            numArray5[num50, num58] *= num56;
                        }
                    }
                    for (int num59 = 1; num59 < (columnCount + 1); num59++)
                    {
                        if (num59 != num50)
                        {
                            num56 = -numArray3[num59, num50];
                            for (int num60 = 1; num60 < (columnCount + 2); num60++)
                            {
                                numArray3[num59, num60] += num56 * numArray3[num50, num60];
                            }
                            if (flag2)
                            {
                                for (int num61 = 1; num61 < (columnCount + 1); num61++)
                                {
                                    numArray5[num59, num61] += num56 * numArray5[num50, num61];
                                }
                            }
                        }
                    }
                    numArray3[0, columnCount + 1] = 0.0;
                }
            }
            object[,] values = new object[flag2 ? 5 : 1, columnCount + 1];
            for (int m = 0; m < (columnCount + 1); m++)
            {
                values[0, m] = (double) numArray3[columnCount - m, columnCount + 1];
            }
            if (flag2)
            {
                double num64 = numArray4[columnCount + 1] - ((numArray4[0] * numArray4[0]) / ((double) rowCount));
                double num63 = numArray4[columnCount + 1];
                for (int num66 = 0; num66 < (columnCount + 1); num66++)
                {
                    num63 -= numArray3[num66, columnCount + 1] * numArray4[num66];
                }
                double num65 = num64 - num63;
                if (num64 == 0.0)
                {
                    values[2, 0] = CalcErrors.Number;
                }
                else
                {
                    values[2, 0] = num65 / num64;
                }
                values[4, 0] = (double) num65;
                values[4, 1] = (double) num63;
                if (flag)
                {
                    if (((rowCount - columnCount) - 1) == 0)
                    {
                        values[2, 1] = CalcErrors.Number;
                        for (int num67 = 0; num67 < (columnCount + 1); num67++)
                        {
                            values[1, num67] = CalcErrors.Number;
                        }
                    }
                    else
                    {
                        double d = num63 / ((double) ((rowCount - columnCount) - 1));
                        for (int num69 = 0; num69 < (columnCount + 1); num69++)
                        {
                            values[1, columnCount - num69] = (double) Math.Sqrt(d * numArray5[num69, num69]);
                        }
                        values[2, 1] = (double) Math.Sqrt(d);
                    }
                    if (num63 == 0.0)
                    {
                        values[3, 0] = CalcErrors.Number;
                    }
                    else
                    {
                        values[3, 0] = (((rowCount - columnCount) - 1) * num65) / (num63 * columnCount);
                    }
                    values[3, 1] = (rowCount - columnCount) - 1;
                }
                else
                {
                    if ((rowCount - columnCount) == 0)
                    {
                        for (int num70 = 0; num70 < (columnCount + 1); num70++)
                        {
                            values[1, num70] = CalcErrors.Number;
                        }
                        values[2, 1] = CalcErrors.Number;
                    }
                    else
                    {
                        double num71 = num63 / ((double) (rowCount - columnCount));
                        values[1, columnCount] = CalcErrors.NotAvailable;
                        for (int num72 = 1; num72 < (columnCount + 1); num72++)
                        {
                            values[1, columnCount - num72] = (double) Math.Sqrt(num71 * numArray5[num72, num72]);
                        }
                        values[2, 1] = (double) Math.Sqrt(num71);
                    }
                    if (num63 == 0.0)
                    {
                        values[3, 0] = CalcErrors.Number;
                    }
                    else
                    {
                        values[3, 0] = ((rowCount - columnCount) * num65) / (num63 * columnCount);
                    }
                    values[3, 1] = rowCount - columnCount;
                }
                for (int num73 = 2; num73 < 5; num73++)
                {
                    for (int num74 = 2; num74 < (columnCount + 1); num74++)
                    {
                        values[num73, num74] = CalcErrors.NotAvailable;
                    }
                }
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
                return "LINEST";
            }
        }

        /// <summary>
        /// A concrete implementation of the CalcArray class.
        /// </summary>
        private class ConcreteArray : CalcArray
        {
            private object[,] values;

            public ConcreteArray(object[,] values)
            {
                this.values = values;
            }

            public override object GetValue(int row, int column)
            {
                return this.values[row, column];
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

