#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a db number class.
    /// </summary>
    internal sealed class DBNumber
    {
        /// <summary>
        /// the Chinese number1.
        /// </summary>
        static DBNumber chineseDBNum1 = null;
        /// <summary>
        /// the Chinese number2.
        /// </summary>
        static DBNumber chineseDBNum2 = null;
        /// <summary>
        /// the Chinese number3.
        /// </summary>
        static DBNumber chineseDBNum3 = null;
        /// <summary>
        /// ○一二三四五六七八九
        /// </summary>
        static int[] ChineseNumberLetterValues1 = new int[] { 0x25cb, 0x4e00, 0x4e8c, 0x4e09, 0x56db, 0x4e94, 0x516d, 0x4e03, 0x516b, 0x4e5d };
        /// <summary>
        /// 零壹贰叁肆伍陆柒捌玖
        /// </summary>
        static int[] ChineseNumberLetterValues2 = new int[] { 0x96f6, 0x58f9, 0x8d30, 0x53c1, 0x8086, 0x4f0d, 0x9646, 0x67d2, 0x634c, 0x7396 };
        /// <summary>
        /// ０１２３４５６７８９
        /// </summary>
        static int[] ChineseNumberLetterValues3 = new int[] { 0xff10, 0xff11, 0xff12, 0xff13, 0xff14, 0xff15, 0xff16, 0xff17, 0xff18, 0xff19 };
        /// <summary>
        /// 千百十兆千百十亿千百十万千百十
        /// </summary>
        static int[] ChineseNumberUnitLetter1 = new int[] { 0x5343, 0x767e, 0x5341, 0x5146, 0x5343, 0x767e, 0x5341, 0x4ebf, 0x5343, 0x767e, 0x5341, 0x4e07, 0x5343, 0x767e, 0x5341, 0 };
        /// <summary>
        /// 仟佰拾兆仟佰拾亿仟佰拾万仟佰拾
        /// </summary>
        static int[] ChineseNumberUnitLetter2 = new int[] { 0x4edf, 0x4f70, 0x62fe, 0x5146, 0x4edf, 0x4f70, 0x62fe, 0x4ebf, 0x4edf, 0x4f70, 0x62fe, 0x4e07, 0x4edf, 0x4f70, 0x62fe, 0 };
        /// <summary>
        /// the Japanese number1.
        /// </summary>
        static DBNumber japaneseDBNum1 = null;
        /// <summary>
        /// the Japanese number2.
        /// </summary>
        static DBNumber japaneseDBNum2 = null;
        /// <summary>
        /// the Japanese number3.
        /// </summary>
        static DBNumber japaneseDBNum3 = null;
        /// <summary>
        /// 〇一二三四五六七八九
        /// </summary>
        static int[] JapaneseNumberLetterValues1 = new int[] { 0x3007, 0x4e00, 0x4e8c, 0x4e09, 0x56db, 0x4e94, 0x516d, 0x4e03, 0x516b, 0x4e5d };
        /// <summary>
        /// 〇壱弐参四伍六七八九
        /// </summary>
        static int[] JapaneseNumberLetterValues2 = new int[] { 0x3007, 0x58f1, 0x5f10, 0x53c2, 0x56db, 0x4f0d, 0x516d, 0x4e03, 0x516b, 0x4e5d };
        /// <summary>
        /// ０１２３４５６７８９
        /// </summary>
        static int[] JapaneseNumberLetterValues3 = new int[] { 0xff10, 0xff11, 0xff12, 0xff13, 0xff14, 0xff15, 0xff16, 0xff17, 0xff18, 0xff19 };
        /// <summary>
        /// 千百十兆千百十億千百十万千百十
        /// </summary>
        static int[] JapaneseNumberUnitLetter1 = new int[] { 0x5343, 0x767e, 0x5341, 0x5146, 0x5343, 0x767e, 0x5341, 0x5104, 0x5343, 0x767e, 0x5341, 0x4e07, 0x5343, 0x767e, 0x5341, 0 };
        /// <summary>
        /// 阡百拾兆阡百拾億阡百拾萬阡百拾
        /// </summary>
        static int[] JapaneseNumberUnitLetter2 = new int[] { 0x9621, 0x767e, 0x62fe, 0x5146, 0x9621, 0x767e, 0x62fe, 0x5104, 0x9621, 0x767e, 0x62fe, 0x842c, 0x9621, 0x767e, 0x62fe, 0 };
        /// <summary>
        /// the numbers
        /// </summary>
        IList<string> numbers;
        /// <summary>
        /// the units
        /// </summary>
        IList<string> units;

        /// <summary>
        /// Creates a <see cref="T:Dt.Cells.Data.DBNumber" /> object with the specified numbers and units.
        /// </summary>
        /// <param name="units">The number units.</param>
        /// <param name="numbers">The numbers.</param>
        DBNumber(int[] units, int[] numbers)
        {
            if (units != null)
            {
                this.units = (IList<string>) new List<string>();
                foreach (int num in units)
                {
                    if (num == 0)
                    {
                        this.units.Add(string.Empty);
                    }
                    else
                    {
                        char ch = (char) num;
                        this.units.Add(((char) ch).ToString());
                    }
                }
            }
            if (numbers != null)
            {
                this.numbers = (IList<string>) new List<string>();
                foreach (int num2 in numbers)
                {
                    if (num2 == 0)
                    {
                        this.numbers.Add(string.Empty);
                    }
                    else
                    {
                        char ch2 = (char) num2;
                        this.numbers.Add(((char) ch2).ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Gets the Chinese number letters.
        /// </summary>
        /// <value>The Chinese number letters.</value>
        public static DBNumber ChineseDBNum1
        {
            get
            {
                if (chineseDBNum1 == null)
                {
                    chineseDBNum1 = new DBNumber(ChineseNumberUnitLetter1, ChineseNumberLetterValues1);
                }
                return chineseDBNum1;
            }
        }

        /// <summary>
        /// Gets the Chinese number letters.
        /// </summary>
        /// <value>The Chinese number letters.</value>
        public static DBNumber ChineseDBNum2
        {
            get
            {
                if (chineseDBNum2 == null)
                {
                    chineseDBNum2 = new DBNumber(ChineseNumberUnitLetter2, ChineseNumberLetterValues2);
                }
                return chineseDBNum2;
            }
        }

        /// <summary>
        /// Gets the Chinese number letters.
        /// </summary>
        /// <value>The Chinese number letters.</value>
        public static DBNumber ChineseDBNum3
        {
            get
            {
                if (chineseDBNum3 == null)
                {
                    chineseDBNum3 = new DBNumber(null, ChineseNumberLetterValues3);
                }
                return chineseDBNum3;
            }
        }

        /// <summary>
        /// Gets the Japanese number letters.
        /// </summary>
        /// <value>The Japanese number letters.</value>
        public static DBNumber JapaneseDBNum1
        {
            get
            {
                if (japaneseDBNum1 == null)
                {
                    japaneseDBNum1 = new DBNumber(JapaneseNumberUnitLetter1, JapaneseNumberLetterValues1);
                }
                return japaneseDBNum1;
            }
        }

        /// <summary>
        /// Gets the Japanese number letters.
        /// </summary>
        /// <value>The Japanese number letters.</value>
        public static DBNumber JapaneseDBNum2
        {
            get
            {
                if (japaneseDBNum2 == null)
                {
                    japaneseDBNum2 = new DBNumber(JapaneseNumberUnitLetter2, JapaneseNumberLetterValues2);
                }
                return japaneseDBNum2;
            }
        }

        /// <summary>
        /// Gets the Japanese number letters.
        /// </summary>
        /// <value>The Japanese number letters.</value>
        public static DBNumber JapaneseDBNum3
        {
            get
            {
                if (japaneseDBNum3 == null)
                {
                    japaneseDBNum3 = new DBNumber(null, JapaneseNumberLetterValues3);
                }
                return japaneseDBNum3;
            }
        }

        /// <summary>
        /// Gets the numbers.
        /// </summary>
        /// <value>The numbers.</value>
        internal IList<string> Numbers
        {
            get { return  this.numbers; }
        }

        /// <summary>
        /// Gets the units.
        /// </summary>
        /// <value>The units.</value>
        internal IList<string> Units
        {
            get { return  this.units; }
        }
    }
}

