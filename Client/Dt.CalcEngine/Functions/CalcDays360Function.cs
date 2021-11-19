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
    /// Returns the number of days between two dates based on a 360-day year (twelve 30-day months), 
    /// which is used in some accounting calculations.
    /// Use this function to help compute payments if your accounting system is based 
    /// on twelve 30-day months.
    /// </summary>
    public class CalcDays360Function : CalcBuiltinFunction
    {
        /// <summary>
        /// Indicates whether the Evaluate method can process missing arguments.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the Evaluate method can process missing arguments; 
        /// otherwise, <see langword="false" />.
        /// </value>
        public override bool AcceptsMissingArgument(int i)
        {
            return (i == 2);
        }

        /// <summary>
        /// Returns the number of days between two dates based on a 360-day year (twelve 30-day months),
        /// which is used in some accounting calculations.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 - 3 items: start_date, end_date, [method].
        /// </para>
        /// <para>
        /// Start_date is a date that represents the start date.
        /// </para>
        /// <para>
        /// End_date is a date that represents the start date.
        /// </para>
        /// <para>
        /// [Method] is a logical value that specifies whether to use the U.S.
        /// or European method in the calculation.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Int32" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            DateTime time = CalcConvert.ToDateTime(args[0]);
            DateTime time2 = CalcConvert.ToDateTime(args[1]);
            bool flag = CalcHelper.ArgumentExists(args, 2) ? CalcConvert.ToBool(args[2]) : false;
            int day = time.Day;
            int num2 = time2.Day;
            int month = time.Month;
            int num4 = time2.Month;
            int year = time.Year;
            int num6 = time2.Year;
            if (flag)
            {
                num2 = (num2 == 0x1f) ? 30 : num2;
                day = (day == 0x1f) ? 30 : day;
            }
            else
            {
                day = (day == 0x1f) ? 30 : day;
                if (num2 == 0x1f)
                {
                    if (day < 30)
                    {
                        num2 = 1;
                        num4++;
                        if (num4 > 12)
                        {
                            num4 = 1;
                            num6++;
                        }
                    }
                    else
                    {
                        num2 = 30;
                    }
                }
            }
            return (int) (((((num6 - year) * 12) + (num4 - month)) * 30) + (num2 - day));
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
                return "DAYS360";
            }
        }
    }
}

