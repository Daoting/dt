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
    /// Calculates how often values occur within a range of values, and 
    /// then returns a vertical array of numbers.
    /// </summary>
    public class CalcFrequencyFunction : CalcBuiltinFunction
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
        /// Calculates how often values occur within a range of values, and
        /// then returns a vertical array of numbers.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 items: data_array, bins_array.
        /// </para>
        /// <para>
        /// Data_array is an array of or reference to a set of values for
        /// which you want to count frequencies.
        /// </para>
        /// <para>
        /// array is an array of or reference to intervals into which you
        /// want to group the values in data_array.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Object" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            CalcArray array = CalcConvert.ToArray(args[0]);
            CalcArray array2 = CalcConvert.ToArray(args[1]);
            int index = 0;
            for (int i = 0; i < array2.RowCount; i++)
            {
                for (int n = 0; n < array2.ColumnCount; n++)
                {
                    object obj2 = array2.GetValue(i, n);
                    if (CalcConvert.IsError(obj2))
                    {
                        return obj2;
                    }
                    if (CalcConvert.IsNumber(obj2))
                    {
                        index++;
                    }
                }
            }
            for (int j = 0; j < array.RowCount; j++)
            {
                for (int num5 = 0; num5 < array.ColumnCount; num5++)
                {
                    object obj3 = array.GetValue(j, num5);
                    if (CalcConvert.IsError(obj3))
                    {
                        return obj3;
                    }
                }
            }
            double[] numArray = new double[index];
            double[] values = new double[index + 1];
            index = 0;
            for (int k = 0; k < array2.RowCount; k++)
            {
                for (int num7 = 0; num7 < array2.ColumnCount; num7++)
                {
                    object obj4 = array2.GetValue(k, num7);
                    if (CalcConvert.IsNumber(obj4))
                    {
                        numArray[index++] = CalcConvert.ToDouble(obj4);
                    }
                }
            }
            Array.Sort<double>(numArray);
            for (int m = 0; m < array.RowCount; m++)
            {
                for (int num9 = 0; num9 < array.ColumnCount; num9++)
                {
                    object obj5 = array.GetValue(m, num9);
                    if (CalcConvert.IsNumber(obj5))
                    {
                        double num10 = CalcConvert.ToDouble(obj5);
                        bool flag = false;
                        for (int num11 = 0; !flag && (num11 < index); num11++)
                        {
                            if (num10 <= numArray[num11])
                            {
                                values[num11]++;
                                flag = true;
                            }
                        }
                        if (!flag)
                        {
                            values[index]++;
                        }
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
                return "FREQUENCY";
            }
        }

        private class ConcreteArray : CalcArray
        {
            private double[] values;

            public ConcreteArray(double[] values)
            {
                this.values = values;
            }

            public override object GetValue(int row, int column)
            {
                return (double) this.values[row];
            }

            public override int ColumnCount
            {
                get
                {
                    return 1;
                }
            }

            public override int RowCount
            {
                get
                {
                    return this.values.Length;
                }
            }
        }
    }
}

