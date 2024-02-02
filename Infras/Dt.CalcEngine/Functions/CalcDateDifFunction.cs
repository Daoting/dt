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
using System.Collections.Generic;
using System.Globalization;
#endregion

namespace Dt.CalcEngine.Functions
{
    /// <summary>
    /// Returns the number of days, months, or years between two dates.
    /// </summary>
    public class CalcDateDifFunction : CalcBuiltinFunction
    {
        private static readonly Dictionary<string, Func<DateTime, DateTime, int>> builtinProcedure = new Dictionary<string, Func<DateTime, DateTime, int>>(new KeywordComparer());

        static CalcDateDifFunction()
        {
            builtinProcedure["Y"] = delegate (DateTime startDate, DateTime endDate) {
                return (endDate.Year - startDate.Year) + (((endDate.Month < startDate.Month) || ((endDate.Month == startDate.Month) && (endDate.Day < startDate.Day))) ? -1 : 0);
            };
            builtinProcedure["M"] = delegate (DateTime startDate, DateTime endDate) {
                return ((12 * (endDate.Year - startDate.Year)) + (endDate.Month - startDate.Month)) + ((endDate.Day < startDate.Day) ? -1 : 0);
            };
            builtinProcedure["D"] = delegate (DateTime startDate, DateTime endDate) {
                TimeSpan span = (TimeSpan) (endDate - startDate);
                return span.Days;
            };
            builtinProcedure["MD"] = delegate (DateTime startDate, DateTime endDate) {
                DateTime time = new DateTime(endDate.Year, endDate.Month, 1);
                TimeSpan span = (TimeSpan) (endDate - time.AddMonths((endDate.Day < startDate.Day) ? -1 : 0).AddDays((double) (startDate.Day - 1)));
                return span.Days;
            };
            builtinProcedure["YM"] = delegate (DateTime startDate, DateTime endDate) {
                return ((endDate.Month - startDate.Month) + (((endDate.Month < startDate.Month) || ((endDate.Month == startDate.Month) && (endDate.Day < startDate.Day))) ? 12 : 0)) + ((endDate.Day < startDate.Day) ? -1 : 0);
            };
            builtinProcedure["YD"] = delegate (DateTime startDate, DateTime endDate) {
                DateTime time = new DateTime(endDate.Year, 1, 1);
                TimeSpan span = (TimeSpan) (endDate - time.AddYears(((endDate.Month < startDate.Month) || ((endDate.Month == startDate.Month) && (endDate.Day < startDate.Day))) ? -1 : 0).AddMonths(startDate.Month - 1).AddDays((double) (startDate.Day - 1)));
                return span.Days;
            };
        }

        /// <summary>
        /// Evaluates the function with the given arguments.
        /// </summary>
        /// <param name="args">Arguments for the function evaluation</param>
        /// <returns>
        /// Result of the function applied to the arguments
        /// </returns>
        public override object Evaluate(object[] args)
        {
            Func<DateTime, DateTime, int> func;
            base.CheckArgumentsLength(args);
            DateTime time = CalcConvert.ToDateTime(args[0]);
            DateTime time2 = CalcConvert.ToDateTime(args[1]);
            string key = CalcConvert.ToString(args[2]).ToUpper();
            if ((time2 >= time) && (builtinProcedure.TryGetValue(key, out func) && (func != null)))
            {
                try
                {
                    return (int) func(time, time2);
                }
                catch
                {
                    return CalcErrors.Value;
                }
            }
            return CalcErrors.Number;
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
                return 3;
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
                return 3;
            }
        }

        /// <summary>
        /// Gets the name of the function.
        /// </summary>
        /// <value>
        /// The name of the function.
        /// </value>
        public override string Name
        {
            get
            {
                return "DATEDIF";
            }
        }

        private class KeywordComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                return (CultureInfo.InvariantCulture.CompareInfo.Compare(x, y, CompareOptions.IgnoreCase) == 0);
            }

            public int GetHashCode(string obj)
            {
                if (obj == null)
                {
                    return 0;
                }
                return obj.GetHashCode();
            }
        }
    }
}

