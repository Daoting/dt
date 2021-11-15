#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2010-09-24 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
#endregion

namespace Dt.Core.Mask
{
    internal class RegExpParser
    {
        #region 静态内容
        protected static short[] yyCheck = new short[] { 
            0x2c, 0, 40, 0x5d, 0x7d, 0x5d, 0x7d, 0x5d, 0x2e, 0, 0x5e, 0, 0, 0x29, 0, 0x11, 
            0x2a, 0x2b, 0x19, 0x101, 0x102, 0, 14, 0x2d, 0x102, 12, 0x2f, 0x1c, 0x17, 0x24, -1, 0x17, 
            -1, 0x19, -1, -1, -1, 0x3f, 40, -1, -1, 40, 0x29, -1, 0x24, 0x25, -1, 0x2e, 
            -1, -1, 0x29, 40, 0x29, 0x5b, -1, 0x29, -1, 0x2e, -1, -1, -1, 40, -1, -1, 
            -1, -1, -1, 0x2e, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
            -1, 0x7d, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 0x5b, -1, -1, -1, 
            0x7c, 0x7b, -1, -1, -1, -1, 0x5b, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
            0x5b, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 0x7c, -1, -1, 
            -1, -1, -1, -1, -1, 0x7c, -1, 0x7c, 0x7c, 0x102, 0x7c, 0x102, -1, -1, -1, -1, 
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
            -1, -1, -1, -1, -1, -1, -1, 0x101, 0x102, 0x101, 0x102, 0x101, 0x102, 0x101, 0x102, -1, 
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
            -1, -1, -1, -1, -1, -1, 0x102, -1, -1, -1, -1, 0x101, 0x102, 0x103, 260, 0x105, 
            0x106, 0x107, 0x108, 0x109, 0x10a, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
            -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 
            -1, -1, 0x101, 0x102, 0x103, 260, 0x105, 0x106, 0x107, 0x108, 0x109, 0x10a, 0x101, 0x102, 0x103, 260, 
            0x105, 0x106, 0x107, 0x108, 0x109, 0x10a, 0x101, 0x102, 0x103, 260, 0x105, 0x106, 0x107, 0x108, 0x109, 0x10a
         };
        static short[] yyDefRed = new short[] { 
            0, 0x21, 0x22, 13, 14, 15, 0x10, 0x11, 0x12, 0x13, 20, 2, 0, 11, 0, 0, 
            0, 0, 0, 7, 10, 12, 0, 0, 0, 0, 0x1d, 1, 0, 0, 0x15, 0x16, 
            0x17, 0, 9, 8, 0, 0, 0x1b, 30, 0, 0x23, 0, 0x1c, 0x20, 0x24, 0x18, 0, 
            0x19, 0, 0x1a
         };
        protected static short[] yyDgoto = new short[] { 15, 0x10, 0x11, 0x12, 0x13, 0x22, 20, 0x15, 0x2a, 0x19, 0x1a };
        protected static int yyFinal = 15;
        protected static short[] yyGindex = new short[] { 0, 13, -1, -2, 0, 0, 8, 0, -21, 5, -7 };
        static short[] yyLen = new short[] { 
            2, 2, 1, 1, 3, 1, 2, 1, 3, 2, 1, 1, 1, 1, 1, 1, 
            1, 1, 1, 1, 1, 1, 1, 1, 3, 4, 5, 3, 4, 1, 2, 1, 
            3, 1, 1, 1, 2
         };
        static short[] yyLhs = new short[] { 
            -1, 0, 0, 1, 1, 2, 2, 3, 3, 3, 4, 4, 4, 4, 4, 4, 
            4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 5, 7, 7, 9, 9, 10, 
            10, 6, 6, 8, 8
         };

        protected static short[] yyRindex = new short[] { 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            0, 9, 1, 0, 0, 0, 0, 0, -86, 0, 0, 0, 0, 11, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 14, 0, 0, 0, 0, 0, 0, 0, 
            0, 0, 0
         };
        protected static short[] yySindex = new short[] { 
            0x15, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, -38, 0, -84, 0, 
            12, -38, -26, 0, 0, 0, -28, -238, -22, -90, 0, 0, -38, -26, 0, 0, 
            0, -234, 0, 0, -88, -238, 0, 0, -38, 0, -44, 0, 0, 0, 0, -121, 
            0, -119, 0
         };
        protected static short[] yyTable = new short[] { 
            0x2f, 5, 12, 0x26, 0x30, 0x2b, 50, 0x1f, 13, 3, 0x17, 6, 0x1b, 0x23, 4, 0x1d, 
            30, 0x1f, 0x27, 1, 2, 11, 0x18, 0x25, 0x29, 0x16, 0x31, 40, 0x24, 0x27, 0, 0x18, 
            0, 0x18, 0, 0, 0, 0x20, 0x1d, 0, 0, 5, 5, 0, 0x18, 0x2c, 0, 5, 
            0, 0, 3, 6, 6, 14, 0, 4, 0, 6, 0, 0, 0, 12, 0, 0, 
            0, 0, 0, 13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            0, 0x2e, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 0, 0, 0, 
            0x1c, 0x21, 0, 0, 0, 0, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            14, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 0, 0, 
            0, 0, 0, 0, 0, 3, 0, 6, 0x1c, 0x29, 4, 0x2d, 0, 0, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 1, 2, 1, 2, 0x1f, 0x1f, 1, 2, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            0, 0, 0, 0, 0, 0, 0x2d, 0, 0, 0, 0, 1, 2, 3, 4, 5, 
            6, 7, 8, 9, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
            0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6, 6, 
            6, 6, 6, 6, 6, 6, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10
         };

        public static RegExpDfa Parse(string regExp, bool reverseAutomate, CultureInfo parseCulture)
        {
            using (StringReader reader = new StringReader(regExp))
            {
                return new RegExpParser().Parse(reader, reverseAutomate, parseCulture);
            }
        }
        #endregion

        #region 成员变量
        RegExpYylex _lexer;
        RegExpDfa _result;
        bool _reverseAutomate;
        int _yyMax;
        #endregion

        #region 属性
        public RegExpDfa Result
        {
            get { return this._result; }
        }
        #endregion

        #region 内部方法
        RegExpDfa Parse(TextReader reader, bool reverseAutomate, CultureInfo parseCulture)
        {
            this._lexer = new RegExpYylex(reader, parseCulture);
            this._reverseAutomate = reverseAutomate;
            this.yyparse(this._lexer);
            return this.Result;
        }

        void yyerror(string message)
        {
            this.yyerror(message, null);
        }

        void yyerror(string message, string[] expected)
        {
            string str = message;
            if ((expected != null) && (expected.Length > 0))
            {
                str = str + message + ", expecting\n";
                for (int i = 0; i < expected.Length; i++)
                {
                    str = str + " " + expected[i];
                }
                str = str + "\n";
            }
            throw new ArgumentException(str);
        }

        object yyparse(IYyInput yyLex)
        {
            int num5;
            if (this._yyMax <= 0)
            {
                this._yyMax = 0x100;
            }
            int index = 0;
            int[] numArray = new int[this._yyMax];
            object obj2 = null;
            object[] objArray = new object[this._yyMax];
            int num2 = -1;
            int num3 = 0;
            int num4 = 0;
            goto Label_0041;
        Label_003B:
            num4++;
        Label_0041:
            if (num4 >= numArray.Length)
            {
                int[] array = new int[numArray.Length + this._yyMax];
                numArray.CopyTo(array, 0);
                numArray = array;
                object[] objArray2 = new object[objArray.Length + this._yyMax];
                objArray.CopyTo(objArray2, 0);
                objArray = objArray2;
            }
            numArray[num4] = index;
            objArray[num4] = obj2;
        Label_008C:
            if ((num5 = yyDefRed[index]) == 0)
            {
                if (num2 < 0)
                {
                    num2 = yyLex.advance() ? yyLex.token() : 0;
                }
                if ((((num5 = yySindex[index]) != 0) && ((num5 += num2) >= 0)) && ((num5 < yyTable.Length) && (yyCheck[num5] == num2)))
                {
                    index = yyTable[num5];
                    obj2 = yyLex.value();
                    num2 = -1;
                    if (num3 > 0)
                    {
                        num3--;
                    }
                    goto Label_003B;
                }
                if ((((num5 = yyRindex[index]) != 0) && ((num5 += num2) >= 0)) && ((num5 < yyTable.Length) && (yyCheck[num5] == num2)))
                {
                    num5 = yyTable[num5];
                }
                else
                {
                    switch (num3)
                    {
                        case 0:
                            this.yyerror("syntax error");
                            goto Label_016F;

                        case 1:
                        case 2:
                            goto Label_016F;

                        case 3:
                            if (num2 == 0)
                            {
                                this.yyerror("irrecoverable syntax error at end-of-file");
                            }
                            num2 = -1;
                            goto Label_008C;
                    }
                }
            }
            int num6 = (num4 + 1) - yyLen[num5];
            obj2 = (num6 > num4) ? null : objArray[num6];
            switch (num5)
            {
                case 1:
                    this._result = (RegExpDfa)objArray[-1 + num4];
                    goto Label_05F4;

                case 2:
                    this._result = RegExpDfa.Empty;
                    goto Label_05F4;

                case 3:
                    obj2 = objArray[num4];
                    goto Label_05F4;

                case 4:
                    obj2 = ((RegExpDfa)objArray[-2 + num4]) | ((RegExpDfa)objArray[num4]);
                    goto Label_05F4;

                case 5:
                    obj2 = objArray[num4];
                    goto Label_05F4;

                case 6:
                    if (this._reverseAutomate)
                    {
                        obj2 = ((RegExpDfa)objArray[num4]) & ((RegExpDfa)objArray[-1 + num4]);
                    }
                    else
                    {
                        obj2 = ((RegExpDfa)objArray[-1 + num4]) & ((RegExpDfa)objArray[num4]);
                    }
                    goto Label_05F4;

                case 7:
                    obj2 = objArray[num4];
                    goto Label_05F4;

                case 8:
                    obj2 = objArray[-1 + num4];
                    goto Label_05F4;

                case 9:
                    obj2 = RegExpDfa.Power((RegExpDfa)objArray[-1 + num4], ((RegExpDupSymbol)objArray[num4])._MinMatches, ((RegExpDupSymbol)objArray[num4])._MaxMatches);
                    goto Label_05F4;

                case 10:
                    obj2 = new RegExpDfa(new OneSymbolTransition((char)objArray[num4]));
                    goto Label_05F4;

                case 11:
                    obj2 = new RegExpDfa(new AnySymbolTransition());
                    goto Label_05F4;

                case 12:
                    obj2 = new RegExpDfa((Transition)objArray[num4]);
                    goto Label_05F4;

                case 13:
                    obj2 = new RegExpDfa(new DecimalDigitTransition(false));
                    goto Label_05F4;

                case 14:
                    obj2 = new RegExpDfa(new DecimalDigitTransition(true));
                    goto Label_05F4;

                case 15:
                    obj2 = new RegExpDfa(new WhiteSpaceTransition(false));
                    goto Label_05F4;

                case 0x10:
                    obj2 = new RegExpDfa(new WhiteSpaceTransition(true));
                    goto Label_05F4;

                case 0x11:
                    obj2 = new RegExpDfa(new WordTransition(false));
                    goto Label_05F4;

                case 0x12:
                    obj2 = new RegExpDfa(new WordTransition(true));
                    goto Label_05F4;

                case 0x13:
                    obj2 = new RegExpDfa(new UnicodeCategoryTransition((string)objArray[num4], false));
                    goto Label_05F4;

                case 20:
                    obj2 = new RegExpDfa(new UnicodeCategoryTransition((string)objArray[num4], true));
                    goto Label_05F4;

                case 0x15:
                    obj2 = new RegExpDupSymbol(0, -1);
                    goto Label_05F4;

                case 0x16:
                    obj2 = new RegExpDupSymbol(1, -1);
                    goto Label_05F4;

                case 0x17:
                    obj2 = new RegExpDupSymbol(0, 1);
                    goto Label_05F4;

                case 0x18:
                    obj2 = new RegExpDupSymbol(objArray[-1 + num4], objArray[-1 + num4]);
                    goto Label_05F4;

                case 0x19:
                    obj2 = new RegExpDupSymbol(objArray[-2 + num4], -1);
                    goto Label_05F4;

                case 0x1a:
                    obj2 = new RegExpDupSymbol(objArray[-3 + num4], objArray[-1 + num4]);
                    goto Label_05F4;

                case 0x1b:
                    obj2 = new BracketTransition(false, ((List<RegExpBracketTransitionRange>)objArray[-1 + num4]).ToArray());
                    goto Label_05F4;

                case 0x1c:
                    obj2 = new BracketTransition(true, ((List<RegExpBracketTransitionRange>)objArray[-1 + num4]).ToArray());
                    goto Label_05F4;

                case 0x1d:
                    {
                        List<RegExpBracketTransitionRange> list = new List<RegExpBracketTransitionRange>();
                        list.Add((RegExpBracketTransitionRange)objArray[num4]);
                        obj2 = list;
                        goto Label_05F4;
                    }
                case 30:
                    obj2 = objArray[-1 + num4];
                    ((List<RegExpBracketTransitionRange>)obj2).Add((RegExpBracketTransitionRange)objArray[num4]);
                    goto Label_05F4;

                case 0x1f:
                    obj2 = new RegExpBracketTransitionRange((char)objArray[num4], (char)objArray[num4]);
                    goto Label_05F4;

                case 0x20:
                    obj2 = new RegExpBracketTransitionRange((char)objArray[-2 + num4], (char)objArray[num4]);
                    goto Label_05F4;

                case 0x21:
                    obj2 = objArray[num4];
                    goto Label_05F4;

                case 0x22:
                    obj2 = objArray[num4];
                    goto Label_05F4;

                case 0x23:
                    obj2 = ((char)objArray[num4]) - '0';
                    goto Label_05F4;

                case 0x24:
                    obj2 = ((((int)objArray[-1 + num4]) * 10) + ((char)objArray[num4])) - 0x30;
                    goto Label_05F4;

                default:
                    goto Label_05F4;
            }
        Label_016F:
            num3 = 3;
            do
            {
                if ((((num5 = yySindex[numArray[num4]]) != 0) && ((num5 += 0x100) >= 0)) && ((num5 < yyTable.Length) && (yyCheck[num5] == 0x100)))
                {
                    index = yyTable[num5];
                    obj2 = yyLex.value();
                    goto Label_003B;
                }
            }
            while (--num4 >= 0);
            this.yyerror("irrecoverable syntax error");
            goto Label_008C;
        Label_05F4:
            num4 -= yyLen[num5];
            index = numArray[num4];
            int num7 = yyLhs[num5];
            if ((index == 0) && (num7 == 0))
            {
                index = yyFinal;
                if (num2 < 0)
                {
                    num2 = yyLex.advance() ? yyLex.token() : 0;
                }
                if (num2 == 0)
                {
                    return obj2;
                }
            }
            else if ((((num5 = yyGindex[num7]) != 0) && ((num5 += index) >= 0)) && ((num5 < yyTable.Length) && (yyCheck[num5] == index)))
            {
                index = yyTable[num5];
            }
            else
            {
                index = yyDgoto[num7];
            }
            goto Label_003B;
        }
        #endregion
    }
}

