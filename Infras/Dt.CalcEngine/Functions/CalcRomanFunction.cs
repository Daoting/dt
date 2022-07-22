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
#endregion

namespace Dt.CalcEngine.Functions
{
    /// <summary>
    /// Converts an Arabic numeral to roman, as text.
    /// </summary>
    public class CalcRomanFunction : CalcBuiltinFunction
    {
        private static readonly List<KeyValuePair<char, int>> info = new List<KeyValuePair<char, int>>(7);

        static CalcRomanFunction()
        {
            info.Add(new KeyValuePair<char, int>('M', 0x3e8));
            info.Add(new KeyValuePair<char, int>('D', 500));
            info.Add(new KeyValuePair<char, int>('C', 100));
            info.Add(new KeyValuePair<char, int>('L', 50));
            info.Add(new KeyValuePair<char, int>('X', 10));
            info.Add(new KeyValuePair<char, int>('V', 5));
            info.Add(new KeyValuePair<char, int>('I', 1));
        }

        /// <summary>
        /// Indicates whether the Evaluate method can process missing arguments.
        /// </summary>
        /// <param name="i">Index of the argument</param>
        /// <value>
        /// <see langword="true" /> if the Evaluate method can process missing arguments; 
        /// otherwise, <see langword="false" />.
        /// </value>
        public override bool AcceptsMissingArgument(int i)
        {
            return (i == 1);
        }

        /// <summary>
        /// Converts an Arabic numeral to roman, as text.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 - 2 items: number, [form].
        /// </para>
        /// <para>
        /// Number is the Arabic numeral you want converted.
        /// </para>
        /// <para>
        /// [Form] is a number specifying the type of roman numeral you want.
        /// The roman numeral style ranges from Classic to Simplified, becoming
        /// more concise as the value of form increases.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            if (args[0] is CalcError)
            {
                return args[0];
            }
            int num = CalcConvert.ToInt(args[0]);
            int num2 = 0;
            if (CalcHelper.ArgumentExists(args, 1))
            {
                if (args[1] is CalcError)
                {
                    return args[1];
                }
                num2 = (args[1] is bool) ? (((bool) args[1]) ? 0 : 4) : CalcConvert.ToInt(args[1]);
            }
            List<char> list = new List<char>();
            if (((num < 0) || (0xf9f < num)) || ((num2 < 0) || (4 < num2)))
            {
                return CalcErrors.Value;
            }
            for (int i = 0; i < info.Count; i += 2)
            {
                KeyValuePair<char, int> pair20;
                KeyValuePair<char, int> pair22;
                if (2 > i)
                {
                    goto Label_017D;
                }
                KeyValuePair<char, int> pair = info[i - 2];
                KeyValuePair<char, int> pair2 = info[i];
                if ((pair.Value - pair2.Value) > num)
                {
                    goto Label_017D;
                }
                int num4 = i;
                int num5 = i - 2;
                int num6 = 0;
                goto Label_00D2;
            Label_00C6:
                num4++;
                num6++;
            Label_00D2:
                if ((num6 < num2) && ((num4 + 1) < info.Count))
                {
                    KeyValuePair<char, int> pair3 = info[num5];
                    KeyValuePair<char, int> pair4 = info[num4 + 1];
                    if ((pair3.Value - pair4.Value) <= num)
                    {
                        goto Label_00C6;
                    }
                }
                KeyValuePair<char, int> pair5 = info[num4];
                list.Add(pair5.Key);
                KeyValuePair<char, int> pair6 = info[num5];
                list.Add(pair6.Key);
                KeyValuePair<char, int> pair7 = info[num4];
                num += pair7.Value;
                KeyValuePair<char, int> pair8 = info[num5];
                num -= pair8.Value;
            Label_017D:
                if (1 <= i)
                {
                    KeyValuePair<char, int> pair9 = info[i - 1];
                    if (pair9.Value <= num)
                    {
                        KeyValuePair<char, int> pair10 = info[i - 1];
                        list.Add(pair10.Key);
                        KeyValuePair<char, int> pair11 = info[i - 1];
                        num -= pair11.Value;
                    }
                }
                if (1 > i)
                {
                    goto Label_02FE;
                }
                KeyValuePair<char, int> pair12 = info[i - 1];
                KeyValuePair<char, int> pair13 = info[i];
                if ((pair12.Value - pair13.Value) > num)
                {
                    goto Label_02FE;
                }
                int num7 = i;
                int num8 = i - 1;
                int num9 = 0;
                goto Label_0220;
            Label_0214:
                num7++;
                num9++;
            Label_0220:
                if ((num9 < num2) && ((num7 + 1) < info.Count))
                {
                    KeyValuePair<char, int> pair14 = info[num8];
                    KeyValuePair<char, int> pair15 = info[num7 + 1];
                    if ((pair14.Value - pair15.Value) <= num)
                    {
                        goto Label_0214;
                    }
                }
                KeyValuePair<char, int> pair16 = info[num7];
                list.Add(pair16.Key);
                KeyValuePair<char, int> pair17 = info[num8];
                list.Add(pair17.Key);
                KeyValuePair<char, int> pair18 = info[num7];
                num += pair18.Value;
                KeyValuePair<char, int> pair19 = info[num8];
                num -= pair19.Value;
                goto Label_02FE;
            Label_02CD:
                pair20 = info[i];
                list.Add(pair20.Key);
                KeyValuePair<char, int> pair21 = info[i];
                num -= pair21.Value;
            Label_02FE:
                pair22 = info[i];
                if (pair22.Value <= num)
                {
                    goto Label_02CD;
                }
            }
            return new string(list.ToArray());
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
                return "ROMAN";
            }
        }
    }
}

