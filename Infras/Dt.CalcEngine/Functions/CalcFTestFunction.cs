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
    /// Returns the <see cref="T:System.Double" /> result of an F-test.
    /// </summary>
    public class CalcFTestFunction : CalcBuiltinFunction
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
        /// Returns the <see cref="T:System.Double" /> result of an F-test.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 items: array1, array2.
        /// </para>
        /// <para>
        /// Array1 is the first array or range of data.
        /// </para>
        /// <para>
        /// Array2 is the second array or range of data.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            stat_closure_t _t;
            base.CheckArgumentsLength(args);
            object o = args[0];
            object obj3 = args[1];
            for (int i = 0; i < ArrayHelper.GetLength(o, 0); i++)
            {
                object obj4 = ArrayHelper.GetValue(o, i, 0);
                if (obj4 is CalcError)
                {
                    return obj4;
                }
            }
            for (int j = 0; j < ArrayHelper.GetLength(obj3, 0); j++)
            {
                object obj5 = ArrayHelper.GetValue(obj3, j, 0);
                if (obj5 is CalcError)
                {
                    return obj5;
                }
            }
            _t.N = 0;
            _t.M = 0.0;
            _t.Q = 0.0;
            _t.afun_flag = false;
            _t.sum = 0.0;
            this.stat(ref _t, o);
            int num6 = _t.N - 1;
            if (_t.N == 1.0)
            {
                return CalcErrors.DivideByZero;
            }
            double num3 = _t.Q / (_t.N - 1.0);
            if (num3 == 0.0)
            {
                return CalcErrors.DivideByZero;
            }
            _t.N = 0;
            _t.M = 0.0;
            _t.Q = 0.0;
            _t.afun_flag = false;
            _t.sum = 0.0;
            this.stat(ref _t, obj3);
            int num7 = _t.N - 1;
            double num4 = _t.Q / (_t.N - 1.0);
            if (num4 == 0.0)
            {
                return CalcErrors.DivideByZero;
            }
            object obj6 = new CalcFDistFunction().Evaluate(new object[] { (double) (num3 / num4), (int) num6, (int) num7 });
            if (obj6 is CalcError)
            {
                return obj6;
            }
            double num5 = (1.0 - ((double) obj6)) * 2.0;
            if (num5 > 1.0)
            {
                num5 = 2.0 - num5;
            }
            return (double) num5;
        }

        internal void stat(ref stat_closure_t closure, object array)
        {
            for (int i = 0; i < ArrayHelper.GetLength(array, 0); i++)
            {
                if (CalcConvert.IsNumber(ArrayHelper.GetValue(array, i, 0)))
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
                return "FTEST";
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

