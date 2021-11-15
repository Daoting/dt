#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2010-09-24 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Globalization;
#endregion 

namespace Dt.Core.Mask
{
    /// <summary>
    /// 
    /// </summary>
    public class NumericMaskLogic
    {
        #region 静态内容
        static char[] allDigits = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        static string Decrement(string number)
        {
            string str = string.Empty;
            for (int i = number.Length - 1; i >= 0; i--)
            {
                char ch = number[i];
                if (ch == '0')
                {
                    str = '9' + str;
                }
                else
                {
                    if ((ch >= '0') && (ch <= '9'))
                    {
                        char ch2 = (char)(ch - '\x0001');
                        return (number.Substring(0, i) + ch2 + str);
                    }
                    str = ch + str;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Div100(string input)
        {
            if (input.IndexOf('.') < 0)
            {
                input = input + '.';
            }
            string str = string.Empty;
            foreach (char ch in input)
            {
                str = ch + str;
            }
            string str2 = Mul100(str);
            string str3 = string.Empty;
            foreach (char ch2 in str2)
            {
                str3 = ch2 + str3;
            }
            return str3;
        }

        static string Increment(string number)
        {
            string str = string.Empty;
            for (int i = number.Length - 1; i >= 0; i--)
            {
                char ch = number[i];
                if (ch == '9')
                {
                    str = '0' + str;
                }
                else
                {
                    if ((ch >= '0') && (ch <= '9'))
                    {
                        char ch2 = (char)(ch + '\x0001');
                        return (number.Substring(0, i) + ch2 + str);
                    }
                    str = ch + str;
                }
            }
            return ('1' + str);
        }

        static string Mul10(string input)
        {
            int index = input.IndexOf('.');
            if (index < 0)
            {
                index = input.Length;
                input = input + '.';
            }
            int num2 = input.IndexOfAny(allDigits, index);
            if (num2 < 0)
            {
                return (input.Substring(0, index) + '0' + input.Substring(index));
            }
            return string.Concat(new object[] { input.Substring(0, index), input.Substring(index + 1, num2 - index), '.', input.Substring(num2 + 1) });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Mul100(string input)
        {
            return Mul10(Mul10(input));
        }

        static string RefineInput(string dirtyInput, CultureInfo refineCulture)
        {
            if ((dirtyInput.Length == 1) && ((dirtyInput[0] == '.') || (dirtyInput[0] == ',')))
            {
                return ".";
            }
            if (refineCulture.NumberFormat.CurrencySymbol != string.Empty)
            {
                dirtyInput = dirtyInput.Replace(refineCulture.NumberFormat.CurrencySymbol, string.Empty);
            }
            string str = string.Empty;
            bool flag = false;
            foreach (char ch in dirtyInput)
            {
                if ((ch >= '0') && (ch <= '9'))
                {
                    str = str + ch;
                }
                else if ((((ch == refineCulture.NumberFormat.NumberDecimalSeparator[0]) || (ch == refineCulture.NumberFormat.CurrencyDecimalSeparator[0])) || (((ch == '.') && ('.' != refineCulture.NumberFormat.NumberGroupSeparator[0])) && ('.' != refineCulture.NumberFormat.CurrencyGroupSeparator[0]))) && !flag)
                {
                    str = str + '.';
                    flag = true;
                }
            }
            return str;
        }

        static string SubtractWithCarry(string number)
        {
            bool flag = false;
            string str = string.Empty;
            int num = number.Length - 1;
            while (true)
            {
                if (num < 0)
                {
                    if (flag)
                    {
                        return str;
                    }
                    return null;
                }
                char ch = number[num];
                if ((ch == '0') && !flag)
                {
                    str = '0' + str;
                }
                else if (((ch >= '0') && (ch <= '9')) && !flag)
                {
                    flag = true;
                    char ch2 = (char)(('9' - ch) + 0x31);
                    str = ch2 + str;
                }
                else if (((ch >= '0') && (ch <= '9')) && flag)
                {
                    char ch3 = (char)(('9' - ch) + 0x30);
                    str = ch3 + str;
                }
                else
                {
                    str = ch + str;
                }
                num--;
            }
        }
        #endregion

        #region 成员变量
        CultureInfo _culture;
        int _maxDigitsAfterDecimalSeparator;
        int _maxDigitsBeforeDecimalSeparator;
        int _minDigitsAfterDecimalSeparator;
        int _minDigitsBeforeDecimalSeparator;
        #endregion

        #region 构造方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxDigitsBeforeDecimalSeparator"></param>
        /// <param name="minDigitsBeforeDecimalSeparator"></param>
        /// <param name="minDigitsAfterDecimalSeparator"></param>
        /// <param name="maxDigitsAfterDecimalSeparator"></param>
        /// <param name="culture"></param>
        public NumericMaskLogic(int maxDigitsBeforeDecimalSeparator, int minDigitsBeforeDecimalSeparator, int minDigitsAfterDecimalSeparator, int maxDigitsAfterDecimalSeparator, CultureInfo culture)
        {
            this.Init(maxDigitsBeforeDecimalSeparator, minDigitsBeforeDecimalSeparator, minDigitsAfterDecimalSeparator, maxDigitsAfterDecimalSeparator, culture);
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="head"></param>
        /// <param name="replaced"></param>
        /// <param name="tail"></param>
        /// <param name="inserted"></param>
        /// <returns></returns>
        public MaskLogicResult GetEditResult(string head, string replaced, string tail, string inserted)
        {
            string str = RefineInput(inserted, this._culture);
            if ((str.Length == 0) && (inserted.Length != 0))
            {
                return null;
            }
            if ((str == ".") && (replaced.Length == 0))
            {
                string resultCandidate = head + tail;
                int index = resultCandidate.IndexOf('.');
                if (index >= 0)
                {
                    return this.CreateResult(resultCandidate, index + 1);
                }
            }
            tail = this.PatchTailIfEmpty(tail);
            if ((str.IndexOf('.') >= 0) && (((this._maxDigitsAfterDecimalSeparator == 0) || (head.IndexOf('.') >= 0)) || (tail.IndexOf('.') >= 0)))
            {
                return null;
            }
            if (((replaced.IndexOf('.') >= 0) && (head.Length > 0)) && ((tail.Length > 0) && (str.IndexOf('.') < 0)))
            {
                return null;
            }
            return this.CreateResult(head + str + tail, head.Length + str.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="head"></param>
        /// <param name="tail"></param>
        /// <param name="isModuloDecrement"></param>
        /// <param name="canChSign"></param>
        /// <param name="chSign"></param>
        /// <returns></returns>
        public MaskLogicResult GetSpinResult(string head, string tail, bool isModuloDecrement, bool canChSign, out bool chSign)
        {
            chSign = false;
            if (isModuloDecrement)
            {
                return this.GetDiveModuloResult(head, tail, canChSign, out chSign);
            }
            return this.GetClimbModuloResult(head, tail);
        }
        #endregion

        #region 内部方法
        MaskLogicResult CreateResult(string resultCandidate, int cursorBase)
        {
            int index = resultCandidate.IndexOf('.');
            if (index < 0)
            {
                index = resultCandidate.Length;
                if (this._maxDigitsAfterDecimalSeparator > 0)
                {
                    resultCandidate = resultCandidate + '.';
                }
            }
            while (((index > this._minDigitsBeforeDecimalSeparator) && (resultCandidate[0] == '0')) && (cursorBase > 0))
            {
                resultCandidate = resultCandidate.Substring(1);
                cursorBase--;
                index--;
            }
            if (index <= this._maxDigitsBeforeDecimalSeparator)
            {
                while (index < this._minDigitsBeforeDecimalSeparator)
                {
                    index++;
                    cursorBase++;
                    resultCandidate = '0' + resultCandidate;
                }
                if ((resultCandidate.Length - index) > this._maxDigitsAfterDecimalSeparator)
                {
                    resultCandidate = resultCandidate.Substring(0, (index + this._maxDigitsAfterDecimalSeparator) + 1);
                    if (cursorBase > resultCandidate.Length)
                    {
                        cursorBase = resultCandidate.Length;
                    }
                }
                while (((resultCandidate.Length > cursorBase) && (this._minDigitsAfterDecimalSeparator < ((resultCandidate.Length - index) - 1))) && resultCandidate.EndsWith("0"))
                {
                    resultCandidate = resultCandidate.Substring(0, resultCandidate.Length - 1);
                }
                while ((this._minDigitsAfterDecimalSeparator > 0) && ((resultCandidate.Length - index) <= this._minDigitsAfterDecimalSeparator))
                {
                    resultCandidate = resultCandidate + '0';
                }
                return new MaskLogicResult(resultCandidate, cursorBase);
            }
            return null;
        }

        MaskLogicResult GetClimbModuloResult(string head, string tail)
        {
            string str = Increment(head);
            return this.CreateResult(str + tail, str.Length);
        }

        MaskLogicResult GetDiveModuloResult(string head, string tail, bool canChSign, out bool chSign)
        {
            chSign = false;
            string str = Decrement(head);
            if (str == null)
            {
                if (!canChSign)
                {
                    return this.CreateResult(string.Empty, 0);
                }
                chSign = true;
                string str2 = SubtractWithCarry(tail);
                if (str2 != null)
                {
                    return this.CreateResult(head + str2, head.Length);
                }
                int length = head.LastIndexOf('0');
                if (length < 0)
                {
                    str = '1' + head;
                }
                else
                {
                    str = head.Substring(0, length) + '1' + head.Substring(length + 1);
                }
            }
            return this.CreateResult(str + tail, str.Length);
        }

        string PatchTailIfEmpty(string tail)
        {
            if (tail.Length <= 0)
            {
                return tail;
            }
            if (tail[0] != '.')
            {
                return tail;
            }
            for (int i = 1; i < tail.Length; i++)
            {
                if (tail[i] != '0')
                {
                    return tail;
                }
            }
            return string.Empty;
        }
        #endregion

        #region 虚拟方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxDigitsBeforeDecimalSeparator"></param>
        /// <param name="minDigitsBeforeDecimalSeparator"></param>
        /// <param name="minDigitsAfterDecimalSeparator"></param>
        /// <param name="maxDigitsAfterDecimalSeparator"></param>
        /// <param name="culture"></param>
        protected virtual void Init(int maxDigitsBeforeDecimalSeparator, int minDigitsBeforeDecimalSeparator, int minDigitsAfterDecimalSeparator, int maxDigitsAfterDecimalSeparator, CultureInfo culture)
        {
            this._maxDigitsBeforeDecimalSeparator = maxDigitsBeforeDecimalSeparator;
            this._maxDigitsAfterDecimalSeparator = maxDigitsAfterDecimalSeparator;
            this._minDigitsBeforeDecimalSeparator = minDigitsBeforeDecimalSeparator;
            this._minDigitsAfterDecimalSeparator = minDigitsAfterDecimalSeparator;
            this._culture = culture;
        }
        #endregion
    }
}
