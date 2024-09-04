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
using System.Runtime.InteropServices;
#endregion

namespace Dt.CalcEngine.Functions
{
    /// <summary>
    /// Returns the probability associated with a Student's t-Test.
    /// </summary>
    /// <remarks>
    /// Use TTEST to determine whether two samples are likely to have come 
    /// from the same two underlying populations that have the same mean.
    /// </remarks>
    public class CalcTTestFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Indicates whether the Evaluate method can process an array arguments.
        /// </summary>
        /// <param name="i">Index of the argument</param>
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
        /// Returns the regularized lower incomplete beta function
        /// I_x(a,b) = 1/Beta(a,b) * int(t^(a-1)*(1-t)^(b-1),t=0..x)
        /// </summary>
        private object BetaRegularized(double a, double b, double x)
        {
            double num = ((x == 0.0) || (x == 1.0)) ? 0.0 : Math.Exp((((this.GammaLn(a + b) - this.GammaLn(a)) - this.GammaLn(b)) + (a * Math.Log(x))) + (b * Math.Log(1.0 - x)));
            bool flag = x < ((a + 1.0) / ((a + b) + 2.0));
            if (flag)
            {
                double num2 = a;
                a = b;
                b = num2;
                x = 1.0 - x;
            }
            double num3 = a + b;
            double num4 = a + 1.0;
            double num5 = a - 1.0;
            double num6 = 1.0;
            double num7 = 1.0 - ((num3 * x) / num4);
            if (Math.Abs(num7) < 4.4501477170144028E-308)
            {
                num7 = 4.4501477170144028E-308;
            }
            num7 = 1.0 / num7;
            double num8 = num7;
            int num9 = 1;
            for (int i = 2; num9 <= 100; i += 2)
            {
                double num11 = ((num9 * (b - num9)) * x) / ((num5 + i) * (a + i));
                num7 = 1.0 + (num11 * num7);
                if (Math.Abs(num7) < 4.4501477170144028E-308)
                {
                    num7 = 4.4501477170144028E-308;
                }
                num6 = 1.0 + (num11 / num6);
                if (Math.Abs(num6) < 4.4501477170144028E-308)
                {
                    num6 = 4.4501477170144028E-308;
                }
                num7 = 1.0 / num7;
                num8 *= num7 * num6;
                num11 = ((-(a + num9) * (num3 + num9)) * x) / ((a + i) * (num4 + i));
                num7 = 1.0 + (num11 * num7);
                if (Math.Abs(num7) < 4.4501477170144028E-308)
                {
                    num7 = 4.4501477170144028E-308;
                }
                num6 = 1.0 + (num11 / num6);
                if (Math.Abs(num6) < 4.4501477170144028E-308)
                {
                    num6 = 4.4501477170144028E-308;
                }
                num7 = 1.0 / num7;
                double num12 = num7 * num6;
                num8 *= num12;
                if (Math.Abs((double) (num12 - 1.0)) < double.Epsilon)
                {
                    double num13 = (num * num8) / a;
                    return (flag ? ((double) (1.0 - num13)) : ((double) num13));
                }
                num9++;
            }
            return CalcErrors.Number;
        }

        /// <summary>
        /// Returns the probability associated with a Student's t-Test.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 4 items: array1, array2, tails, type.
        /// </para>
        /// <para>
        /// Array1 is the first data set.
        /// </para>
        /// <para>
        /// Array2 is the second data set.
        /// </para>
        /// <para>
        /// Tails specifies the number of distribution tails.
        /// </para>
        /// <para>
        /// Type is the kind of t-Test to perform.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Object" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            return this.Evaluate_old(args);
        }

        private object Evaluate_old(object[] args)
        {
            double num3;
            double num5;
            double num9;
            base.CheckArgumentsLength(args);
            object obj2 = args[0];
            object obj3 = args[1];
            object obj4 = args[2];
            object obj5 = args[3];
            if (!CalcConvert.IsNumber(obj4) || !CalcConvert.IsNumber(obj5))
            {
                return CalcErrors.Value;
            }
            int num = CalcConvert.ToInt(obj4);
            int num2 = CalcConvert.ToInt(obj5);
            if (((num != 1) && (num != 2)) || ((num2 < 1) || (3 < num2)))
            {
                return CalcErrors.Number;
            }
            if (num2 == 1)
            {
                double num12;
                double num14;
                double num15;
                double num16;
                double num17;
                obj2 = this.iterate(obj2, obj3);
                if (obj2 is CalcError)
                {
                    return obj2;
                }
                double num13 = num14 = num15 = num16 = num17 = num12 = 0.0;
                int i = 0;
                int length = ArrayHelper.GetLength(obj2, 0);
                while (i < length)
                {
                    double num20;
                    if (!CalcConvert.TryToDouble(ArrayHelper.GetValue(obj2, i, 0), out num20, true))
                    {
                        return CalcErrors.Value;
                    }
                    num13 = num20 - num15;
                    num14 = num13 / (num17 + 1.0);
                    num15 += num14;
                    num16 += (num17 * num13) * num14;
                    num17++;
                    num12 += num20;
                    i++;
                }
                if (((num17 - 1.0) == 0.0) || (num17 == 0.0))
                {
                    return CalcErrors.DivideByZero;
                }
                double d = Math.Sqrt(num16 / (num17 - 1.0));
                if (double.IsNaN(d) || double.IsInfinity(d))
                {
                    return CalcErrors.Number;
                }
                num3 = num12 / num17;
                num5 = num3 / (d / Math.Sqrt(num17));
                num9 = num17 - 1.0;
            }
            else
            {
                stat_closure_t _t;
                _t.N = 0;
                _t.M = 0.0;
                _t.Q = 0.0;
                _t.afun_flag = false;
                _t.sum = 0.0;
                this.stat(ref _t, obj2);
                double num7 = _t.Q / (_t.N - 1.0);
                num3 = _t.sum / ((double) _t.N);
                int n = _t.N;
                _t.N = 0;
                _t.M = 0.0;
                _t.Q = 0.0;
                _t.afun_flag = false;
                _t.sum = 0.0;
                this.stat(ref _t, obj3);
                double num8 = _t.Q / (_t.N - 1.0);
                double num4 = _t.sum / ((double) _t.N);
                int num11 = _t.N;
                if (num2 != 2)
                {
                    double num21 = (num7 / ((double) n)) / ((num7 / ((double) n)) + (num8 / ((double) num11)));
                    num9 = 1.0 / (((num21 * num21) / ((double) (n - 1))) + (((1.0 - num21) * (1.0 - num21)) / ((double) (num11 - 1))));
                }
                else
                {
                    num9 = (n + num11) - 2;
                }
                num5 = (num3 - num4) / Math.Sqrt((num7 / ((double) n)) + (num8 / ((double) num11)));
            }
            num5 = Math.Abs(num5);
            object obj6 = this.BetaRegularized(0.5 * num9, 0.5, num9 / (num9 + (num5 * num5)));
            if (obj6 is CalcError)
            {
                return obj6;
            }
            return (double) ((0.5 * num) * ((double) obj6));
        }

        /// <summary>
        /// Returns the natural logarithm of Gamma for a specified value.
        /// </summary>
        private double GammaLn(double value)
        {
            double[] numArray = new double[] { 76.180091729471457, -86.505320329416776, 24.014098240830911, -1.231739572450155, 0.001208650973866179, -5.395239384953E-06 };
            double num = value;
            double d = value + 5.5;
            d -= (value + 0.5) * Math.Log(d);
            double num3 = 1.0000000001900149;
            for (int i = 0; i <= 5; i++)
            {
                num3 += numArray[i] / ++num;
            }
            return (-d + Math.Log((2.5066282746310007 * num3) / value));
        }

        internal object iterate(object array1, object array2)
        {
            int length = ArrayHelper.GetLength(array1, 0);
            int num2 = ArrayHelper.GetLength(array2, 0);
            if (length != num2)
            {
                return CalcErrors.NotAvailable;
            }
            object[] values = new object[length];
            for (int i = 0; i < length; i++)
            {
                double num4;
                double num5;
                if (!CalcConvert.TryToDouble(ArrayHelper.GetValue(array1, i, 0), out num4, true) || !CalcConvert.TryToDouble(ArrayHelper.GetValue(array2, i, 0), out num5, true))
                {
                    return CalcErrors.Value;
                }
                values[i] = num4 - num5;
            }
            return new ConcreteArray(values);
        }

        internal void stat(ref stat_closure_t closure, object array)
        {
            int length = ArrayHelper.GetLength(array, 0);
            for (int i = 0; i < length; i++)
            {
                double num = CalcConvert.ToDouble(ArrayHelper.GetValue(array, i, 0));
                double num2 = num - closure.M;
                double num3 = num2 / ((double) (closure.N + 1));
                closure.M += num3;
                closure.Q += (closure.N * num2) * num3;
                closure.N++;
                closure.sum += num;
            }
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
                return 4;
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
                return "TTEST";
            }
        }

        private class ConcreteArray : CalcArray
        {
            private object[] values;

            public ConcreteArray(object[] values)
            {
                this.values = values;
            }

            public override object GetValue(int row, int column)
            {
                return this.values[column];
            }

            public override int ColumnCount
            {
                get
                {
                    return this.values.Length;
                }
            }

            public override int RowCount
            {
                get
                {
                    return 1;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct stat_closure_t
        {
            public int N;
            public double M;
            public double Q;
            public double sum;
            public bool afun_flag;
        }
    }
}

