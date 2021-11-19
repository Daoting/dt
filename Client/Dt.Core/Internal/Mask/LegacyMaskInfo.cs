#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2010-09-24 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
#endregion

namespace Dt.Core.Mask
{
    /// <summary>
    /// 
    /// </summary>
    public class LegacyMaskInfo : IEnumerable
    {
        #region 静态内容
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="maskCulture"></param>
        /// <returns></returns>
        public static LegacyMaskInfo GetRegularMaskInfo(string mask, CultureInfo maskCulture)
        {
            LegacyMaskInfo info = new LegacyMaskInfo();
            string str = mask;
            char caseConversion = 'A';
            while (str.Length > 0)
            {
                int index;
                string str3;
                char literal = str[0];
                string str2 = str.Substring(1);
                switch (literal)
                {
                    case '*':
                        info.PatchQuantifier(0, 0x7fffffff);
                        break;
                    case '+':
                        info.PatchQuantifier(1, 0x7fffffff);
                        break;
                    case '.':
                        info.container.Add(new LegacyMaskChar(".", caseConversion, 1, 1));
                        break;
                    case '$':
                        foreach (char ch5 in maskCulture.NumberFormat.CurrencySymbol)
                        {
                            info.container.Add(new LegacyMaskLiteral(ch5));
                        }
                        break;
                    case '<':
                        if (!str.StartsWith("<>"))
                        {
                            caseConversion = 'L';
                            break;
                        }
                        str2 = str.Substring(2);
                        caseConversion = 'A';
                        break;
                    case '>':
                        caseConversion = 'U';
                        break;
                    case '?':
                        info.PatchQuantifier(0, 1);
                        break;
                    case '[':
                        index = str2.Replace(@"\]", @"\}").IndexOf(']');
                        if (index < 0)
                        {
                            throw new ArgumentException("Incorrect mask: ']' expected after '['");
                        }
                        str3 = str.Substring(0, index + 2);
                        str2 = str.Substring(index + 2);
                        info.container.Add(new LegacyMaskChar(str3, caseConversion, 1, 1));
                        break;
                    case '\\':
                        if (str2.Length == 0)
                        {
                            throw new ArgumentException(@"Incorrect mask: character expected after '\'");
                        }
                        literal = str2[0];
                        str2 = str2.Substring(1);
                        if (literal == 'd' || literal == 'w' || literal == 'D' || literal == 'W')
                        {
                            info.container.Add(new LegacyMaskChar(@"\" + literal, caseConversion, 1, 1));
                        }
                        else
                        {
                            info.container.Add(new LegacyMaskLiteral(literal));
                        }
                        break;
                    case '{':
                        int length = str2.IndexOf('}');
                        if (length == 0)
                        {
                            throw new ArgumentException("Incorrect mask: '}' expected after '{'");
                        }
                        string[] strArray = str2.Substring(0, length).Split(new char[] { ',' });
                        str2 = str.Substring(length + 2);
                        if (strArray.Length != 1)
                        {
                            if (strArray.Length != 2)
                            {
                                throw new ArgumentException("Incorrect mask: invalid quantifier format");
                            }
                            int min = int.Parse(strArray[0], CultureInfo.InvariantCulture);
                            int max = (strArray[1].Length == 0) ? 0x7fffffff : int.Parse(strArray[1], CultureInfo.InvariantCulture);
                            info.PatchQuantifier(min, max);
                        }
                        else
                        {
                            int num3 = int.Parse(strArray[0], CultureInfo.InvariantCulture);
                            info.PatchQuantifier(num3, num3);
                        }
                        break;
                    default:
                        info.container.Add(new LegacyMaskLiteral(literal));
                        break;
                }
                str = str2;
            }
            PatchZeroLengthMaskInfo(info, caseConversion);
            return info;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="maskCulture"></param>
        /// <returns></returns>
        public static LegacyMaskInfo GetSimpleMaskInfo(string mask, CultureInfo maskCulture)
        {
            LegacyMaskInfo info = new LegacyMaskInfo();
            string str = mask;
            char caseConversion = 'A';
            while (str.Length > 0)
            {
                string currencySymbol;
                char c = str[0];
                string str2 = str.Substring(1);
                switch (c)
                {
                    case '#':
                        info.container.Add(new LegacyMaskChar(@"(\d|[+]|[-])", caseConversion, 0, 1));
                        break;
                    case '$':
                        currencySymbol = maskCulture.NumberFormat.CurrencySymbol;
                        int num3 = 0;
                        while (num3 < currencySymbol.Length)
                        {
                            char ch5 = currencySymbol[num3];
                            info.container.Add(new LegacyMaskLiteral(ch5));
                            num3++;
                        }
                        break;
                    case '0':
                    case '9':
                        info.container.Add(new LegacyMaskChar(@"\d", caseConversion, (c == '0') ? 1 : 0, 1));
                        break;
                    case '<':
                        if (!str.StartsWith("<>"))
                        {
                            caseConversion = 'L';
                            break;
                        }
                        str2 = str.Substring(2);
                        caseConversion = 'A';
                        break;
                    case '>':
                        caseConversion = 'U';
                        break;
                    case 'A':
                    case 'a':
                        info.container.Add(new LegacyMaskChar(@"(\p{L}|\d)", caseConversion, char.IsUpper(c) ? 1 : 0, 1));
                        break;
                    case 'C':
                    case 'c':
                        info.container.Add(new LegacyMaskChar(".", caseConversion, char.IsUpper(c) ? 1 : 0, 1));
                        break;
                    case 'l':
                    case 'L':
                        info.container.Add(new LegacyMaskChar(@"\p{L}", caseConversion, char.IsUpper(c) ? 1 : 0, 1));
                        break;
                    case '\\':
                        if (str2.Length == 0)
                        {
                            throw new ArgumentException(@"Incorrect mask: character expected after '\'");
                        }
                        c = str2[0];
                        str2 = str2.Substring(1);
                        info.container.Add(new LegacyMaskLiteral(c));
                        break;
                    default:
                        info.container.Add(new LegacyMaskLiteral(c));
                        break;
                }
                str = str2;
            }
            PatchZeroLengthMaskInfo(info, caseConversion);
            return info;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="caseConversion"></param>
        static void PatchZeroLengthMaskInfo(LegacyMaskInfo info, char caseConversion)
        {
            if (info.Count == 0)
            {
                info.container.Add(new LegacyMaskChar(".", caseConversion, 0, 0x7fffffff));
            }
        }
        #endregion

        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public int Count
        {
            get { return this.container.Count; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="primitiveIndex"></param>
        /// <returns></returns>
        public LegacyMaskPrimitive this[int primitiveIndex]
        {
            get { return this.container[primitiveIndex]; }
        }
        #endregion 

        #region 外部方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="blank"></param>
        /// <returns></returns>
        public string GetDisplayText(string[] elements, char blank)
        {
            string str = string.Empty;
            for (int i = 0; i < this.Count; i++)
            {
                str = str + this[i].GetDisplayText(elements[i], blank);
            }
            return str;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="blank"></param>
        /// <param name="saveLiteral"></param>
        /// <returns></returns>
        public string GetEditText(string[] elements, char blank, bool saveLiteral)
        {
            string str = string.Empty;
            for (int i = 0; i < this.Count; i++)
            {
                str = str + this[i].GetEditText(elements[i], blank, saveLiteral);
            }
            return str;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string[] GetElementsEmpty()
        {
            string[] strArray = new string[this.Count];
            for (int i = 0; i < this.Count; i++)
            {
                strArray[i] = string.Empty;
            }
            return strArray;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="editText"></param>
        /// <param name="blank"></param>
        /// <param name="saveLiteral"></param>
        /// <returns></returns>
        public string[] GetElementsFromEditText(string editText, char blank, bool saveLiteral)
        {
            string pattern = this.BuildExtruderRegExp(blank, saveLiteral);
            Match match = Regex.Match(editText, pattern);
            if (!match.Success || (match.Value != editText))
            {
                return null;
            }
            string[] strArray = new string[this.Count];
            for (int i = 0; i < this.Count; i++)
            {
                string str2;
                LegacyMaskPrimitive primitive = this[i];
                if (primitive.IsLiteral)
                {
                    str2 = string.Empty;
                }
                else
                {
                    str2 = match.Groups[string.Format(CultureInfo.InvariantCulture, "element{0}", new object[] { i })].Value.TrimEnd(new char[] { blank });
                    if (!primitive.IsAcceptableStrong(blank))
                    {
                        str2 = str2.Replace(blank.ToString(), string.Empty);
                    }
                }
                strArray[i] = str2;
            }
            return strArray;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetFirstEditableIndex()
        {
            return this.GetNextEditableElement(-1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool GetIsEditable()
        {
            return (this.GetFirstEditableIndex() >= 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetLastEditableIndex()
        {
            return this.GetPrevEditableElement(this.Count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public int GetNextEditableElement(int current)
        {
            for (int i = current + 1; i < this.Count; i++)
            {
                if (!this[i].IsLiteral)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="element"></param>
        /// <param name="insideElement"></param>
        /// <returns></returns>
        public int GetPosition(string[] elements, int element, int insideElement)
        {
            int num = 0;
            for (int i = 0; i < element; i++)
            {
                num += this[i].GetDisplayText(elements[i], '@').Length;
            }
            return (num + insideElement);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        public int GetPrevEditableElement(int current)
        {
            for (int i = current - 1; i >= 0; i--)
            {
                if (!this[i].IsLiteral)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void PatchQuantifier(int min, int max)
        {
            if (this.Count == 0)
            {
                throw new ArgumentException("Incorrect mask: invalid quantifier format");
            }
            LegacyMaskChar ch = this[this.Count - 1] as LegacyMaskChar;
            if (ch == null)
            {
                throw new ArgumentException("Incorrect mask: invalid quantifier format");
            }
            if ((ch.MinMatches != 1) || (ch.MaxMatches != 1))
            {
                throw new ArgumentException("Incorrect mask: invalid quantifier format");
            }
            if ((min < 0) || (min > max))
            {
                throw new ArgumentException("Incorrect mask: invalid quantifier format");
            }
            ch.PatchMatches(min, max);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.container.GetEnumerator();
        }
        #endregion

        #region 内部方法
        readonly List<LegacyMaskPrimitive> container = new List<LegacyMaskPrimitive>();

        string BuildExtruderRegExp(char blank, bool saveLiteral)
        {
            string str = string.Empty;
            string str2 = string.Format(CultureInfo.InvariantCulture, ((blank == '^') || (blank == '\\')) ? @"[\{0}]" : "[{0}]", new object[] { blank });
            for (int i = 0; i < this.Count; i++)
            {
                LegacyMaskPrimitive primitive = this[i];
                if (primitive.IsLiteral)
                {
                    if (saveLiteral)
                    {
                        str = str + "(" + primitive.CapturingExpression + ")";
                    }
                }
                else
                {
                    string format = "(?<element{4}>(({0})";
                    if (!Regex.IsMatch(blank.ToString(), primitive.CapturingExpression))
                    {
                        format = format + "|({1})";
                    }
                    format = format + ")";
                    if ((primitive.MinMatches != 1) || (primitive.MaxMatches != 1))
                    {
                        format = format + "{{{2},{3}}}";
                    }
                    format = format + ")";
                    str = str + string.Format(CultureInfo.InvariantCulture, format, new object[] { primitive.CapturingExpression, str2, primitive.MinMatches, primitive.MaxMatches, i });
                }
            }
            return ("^" + str + "$");
        }
        #endregion
    }
}

