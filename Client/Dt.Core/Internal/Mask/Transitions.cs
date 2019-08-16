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
using System.Reflection;

#endregion

namespace Dt.Core.Mask
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class AnySymbolTransition : Transition
    {
        #region 构造方法
        /// <summary>
        /// 
        /// </summary>
        public AnySymbolTransition() { }

        AnySymbolTransition(RegExpState target)
            : base(target)
        { }
        #endregion

        #region 重写方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public override Transition Copy(RegExpState target)
        {
            return new AnySymbolTransition(target);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override char GetSampleChar()
        {
            return '.';
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool IsMatch(char input)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "any";
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class BracketTransition : Transition
    {
        #region 成员变量
        readonly bool _notMatch;
        readonly RegExpBracketTransitionRange[] _ranges;
        #endregion

        #region 构造方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="notMatch"></param>
        /// <param name="ranges"></param>
        public BracketTransition(bool notMatch, RegExpBracketTransitionRange[] ranges)
        {
            this._notMatch = notMatch;
            this._ranges = ranges;
        }

        BracketTransition(RegExpState target, bool notMatch, RegExpBracketTransitionRange[] ranges) : base(target)
        {
            this._notMatch = notMatch;
            this._ranges = ranges;
        }
        #endregion
        
        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public override bool IsExact
        {
            get
            {
                if (this._notMatch)
                {
                    return false;
                }
                bool flag = false;
                char from = '\0';
                foreach (RegExpBracketTransitionRange range in this._ranges)
                {
                    if (!flag)
                    {
                        flag = true;
                        from = range._From;
                    }
                    if (from != range._From)
                    {
                        return false;
                    }
                    if (from != range._To)
                    {
                        return false;
                    }
                }
                if (!flag)
                {
                    return false;
                }
                return true;
            }
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public override Transition Copy(RegExpState target)
        {
            return new BracketTransition(target, this._notMatch, (RegExpBracketTransitionRange[]) this._ranges.Clone());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override char GetSampleChar()
        {
            foreach (RegExpBracketTransitionRange range in this._ranges)
            {
                char from = range._From;
                if (this.IsMatch(from))
                {
                    return from;
                }
                from = (char) (range._From - '\x0001');
                if (this.IsMatch(from))
                {
                    return from;
                }
                from = (char) (range._To + '\x0001');
                if (this.IsMatch(from))
                {
                    return from;
                }
            }
            return '?';
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool IsMatch(char input)
        {
            foreach (RegExpBracketTransitionRange range in this._ranges)
            {
                if (((input >= range._From) && (input <= range._To)) || ((input == range._From) || (input == range._To)))
                {
                    return !this._notMatch;
                }
            }
            return this._notMatch;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string str = string.Empty;
            foreach (RegExpBracketTransitionRange range in this._ranges)
            {
                object obj2 = str;
                str = string.Concat(new object[] { obj2, range._From, "-", range._To });
            }
            return string.Format("[{0}{1}]", this._notMatch ? "^" : string.Empty, str);
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class DecimalDigitTransition : Transition
    {
        #region 成员变量
        readonly bool _notMatch;
        #endregion

        #region 构造方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="notMatch"></param>
        public DecimalDigitTransition(bool notMatch)
        {
            this._notMatch = notMatch;
        }

        DecimalDigitTransition(RegExpState target, bool notMatch)
            : base(target)
        {
            this._notMatch = notMatch;
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public override Transition Copy(RegExpState target)
        {
            return new DecimalDigitTransition(target, this._notMatch);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override char GetSampleChar()
        {
            if (!this._notMatch)
            {
                return '0';
            }
            return '.';
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool IsMatch(char input)
        {
            bool flag = char.IsDigit(input);
            if (!this._notMatch)
            {
                return flag;
            }
            return !flag;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (!this._notMatch)
            {
                return @"\d";
            }
            return @"\D";
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class EmptyTransition : Transition
    {
        #region 构造方法
        /// <summary>
        /// 
        /// </summary>
        public EmptyTransition() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        public EmptyTransition(RegExpState target)
            : base(target)
        { }
        #endregion

        #region 重写方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public override Transition Copy(RegExpState target)
        {
            return new EmptyTransition(target);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override char GetSampleChar()
        {
            throw new InvalidOperationException("Internal error: GetSampleChar() called for empty transition");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool IsMatch(char input)
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "empty";
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool IsEmpty
        {
            get { return true; }
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class OneSymbolTransition : Transition
    {
        #region 成员变量
        readonly char _input;
        #endregion

        #region 构造方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        public OneSymbolTransition(char input)
        {
            this._input = input;
        }

        OneSymbolTransition(RegExpState target, char input)
            : base(target)
        {
            this._input = input;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public override bool IsExact
        {
            get { return true; }
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public override Transition Copy(RegExpState target)
        {
            return new OneSymbolTransition(target, this._input);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override char GetSampleChar()
        {
            return this._input;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool IsMatch(char input)
        {
            return (input == this._input);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ("'" + this._input + "'");
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class UnicodeCategoryTransition : Transition
    {
        #region 静态内容
        static readonly IDictionary<string, UnicodeCategory[]> fUnicodeCategoryNames = new Dictionary<string, UnicodeCategory[]>(StringComparer.CurrentCultureIgnoreCase);

        const string sampleChars = "Aaǅʰƻ̀ः҈0Ⅰ\x00b2 \u2028\u2029\t܏\ud800\ue000_-()\x00ab\x00bb!+$^\x00a6Ƞ";

        static UnicodeCategoryTransition()
        {
            foreach (UnicodeCategory category in GetValues(typeof(UnicodeCategory)))
            {
                fUnicodeCategoryNames.Add(category.ToString(), new UnicodeCategory[] { category });
            }
            fUnicodeCategoryNames.Add("Lu", new UnicodeCategory[1]);
            fUnicodeCategoryNames.Add("Ll", new UnicodeCategory[] { UnicodeCategory.LowercaseLetter });
            fUnicodeCategoryNames.Add("Lt", new UnicodeCategory[] { UnicodeCategory.TitlecaseLetter });
            fUnicodeCategoryNames.Add("Lm", new UnicodeCategory[] { UnicodeCategory.ModifierLetter });
            fUnicodeCategoryNames.Add("Lo", new UnicodeCategory[] { UnicodeCategory.OtherLetter });
            UnicodeCategory[] categoryArray7 = new UnicodeCategory[5];
            categoryArray7[1] = UnicodeCategory.LowercaseLetter;
            categoryArray7[2] = UnicodeCategory.TitlecaseLetter;
            categoryArray7[3] = UnicodeCategory.ModifierLetter;
            categoryArray7[4] = UnicodeCategory.OtherLetter;
            fUnicodeCategoryNames.Add("L", categoryArray7);
            fUnicodeCategoryNames.Add("Letter", fUnicodeCategoryNames["L"]);
            fUnicodeCategoryNames.Add("Mn", new UnicodeCategory[] { UnicodeCategory.NonSpacingMark });
            fUnicodeCategoryNames.Add("Mc", new UnicodeCategory[] { UnicodeCategory.SpacingCombiningMark });
            fUnicodeCategoryNames.Add("Me", new UnicodeCategory[] { UnicodeCategory.EnclosingMark });
            fUnicodeCategoryNames.Add("M", new UnicodeCategory[] { UnicodeCategory.NonSpacingMark, UnicodeCategory.SpacingCombiningMark, UnicodeCategory.EnclosingMark });
            fUnicodeCategoryNames.Add("Mark", fUnicodeCategoryNames["M"]);
            fUnicodeCategoryNames.Add("Nd", new UnicodeCategory[] { UnicodeCategory.DecimalDigitNumber });
            fUnicodeCategoryNames.Add("Nl", new UnicodeCategory[] { UnicodeCategory.LetterNumber });
            fUnicodeCategoryNames.Add("No", new UnicodeCategory[] { UnicodeCategory.OtherNumber });
            fUnicodeCategoryNames.Add("N", new UnicodeCategory[] { UnicodeCategory.DecimalDigitNumber, UnicodeCategory.LetterNumber, UnicodeCategory.OtherNumber });
            fUnicodeCategoryNames.Add("Number", fUnicodeCategoryNames["N"]);
            fUnicodeCategoryNames.Add("Sm", new UnicodeCategory[] { UnicodeCategory.MathSymbol });
            fUnicodeCategoryNames.Add("Sc", new UnicodeCategory[] { UnicodeCategory.CurrencySymbol });
            fUnicodeCategoryNames.Add("Sk", new UnicodeCategory[] { UnicodeCategory.ModifierLetter });
            fUnicodeCategoryNames.Add("So", new UnicodeCategory[] { UnicodeCategory.OtherSymbol });
            fUnicodeCategoryNames.Add("S", new UnicodeCategory[] { UnicodeCategory.MathSymbol, UnicodeCategory.CurrencySymbol, UnicodeCategory.ModifierLetter, UnicodeCategory.OtherSymbol });
            fUnicodeCategoryNames.Add("Symbol", fUnicodeCategoryNames["S"]);
            fUnicodeCategoryNames.Add("Pc", new UnicodeCategory[] { UnicodeCategory.ConnectorPunctuation });
            fUnicodeCategoryNames.Add("Pd", new UnicodeCategory[] { UnicodeCategory.DashPunctuation });
            fUnicodeCategoryNames.Add("Ps", new UnicodeCategory[] { UnicodeCategory.OpenPunctuation });
            fUnicodeCategoryNames.Add("Pe", new UnicodeCategory[] { UnicodeCategory.ClosePunctuation });
            fUnicodeCategoryNames.Add("Pi", new UnicodeCategory[] { UnicodeCategory.InitialQuotePunctuation });
            fUnicodeCategoryNames.Add("Pf", new UnicodeCategory[] { UnicodeCategory.FinalQuotePunctuation });
            fUnicodeCategoryNames.Add("Po", new UnicodeCategory[] { UnicodeCategory.OtherPunctuation });
            fUnicodeCategoryNames.Add("P", new UnicodeCategory[] { UnicodeCategory.ConnectorPunctuation, UnicodeCategory.DashPunctuation, UnicodeCategory.OpenPunctuation, UnicodeCategory.ClosePunctuation, UnicodeCategory.InitialQuotePunctuation, UnicodeCategory.FinalQuotePunctuation, UnicodeCategory.OtherPunctuation });
            fUnicodeCategoryNames.Add("Punctuation", fUnicodeCategoryNames["P"]);
            fUnicodeCategoryNames.Add("Zs", new UnicodeCategory[] { UnicodeCategory.SpaceSeparator });
            fUnicodeCategoryNames.Add("Zl", new UnicodeCategory[] { UnicodeCategory.LineSeparator });
            fUnicodeCategoryNames.Add("Zp", new UnicodeCategory[] { UnicodeCategory.ParagraphSeparator });
            fUnicodeCategoryNames.Add("Z", new UnicodeCategory[] { UnicodeCategory.SpaceSeparator, UnicodeCategory.LineSeparator, UnicodeCategory.ParagraphSeparator });
            fUnicodeCategoryNames.Add("Separator", fUnicodeCategoryNames["Z"]);
            fUnicodeCategoryNames.Add("Cc", new UnicodeCategory[] { UnicodeCategory.Control });
            fUnicodeCategoryNames.Add("Cf", new UnicodeCategory[] { UnicodeCategory.Format });
            fUnicodeCategoryNames.Add("Cs", new UnicodeCategory[] { UnicodeCategory.Surrogate });
            fUnicodeCategoryNames.Add("Co", new UnicodeCategory[] { UnicodeCategory.PrivateUse });
            fUnicodeCategoryNames.Add("Cn", new UnicodeCategory[] { UnicodeCategory.OtherNotAssigned });
            fUnicodeCategoryNames.Add("C", new UnicodeCategory[] { UnicodeCategory.Control, UnicodeCategory.Format, UnicodeCategory.Surrogate, UnicodeCategory.PrivateUse, UnicodeCategory.OtherNotAssigned });
            fUnicodeCategoryNames.Add("Other", fUnicodeCategoryNames["C"]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static Array GetValues(Type enumType)
        {
            // hdt
            List<FieldInfo> fields = new List<FieldInfo>();
            foreach (FieldInfo field in enumType.GetRuntimeFields())
            {
                if (field.IsPublic && field.IsStatic)
                {
                    fields.Add(field);
                }
            }

            object[] objArray = new object[fields.Count];
            for (int i = 0; i < fields.Count; i++)
            {
                objArray[i] = fields[i].GetValue(null);
            }
            return objArray;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public static UnicodeCategory[] GetUnicodeCategoryListFromCharacterClassName(string className)
        {
            UnicodeCategory[] categoryArray;
            className = className.Replace(" ", string.Empty);
            className = className.Replace("_", string.Empty);
            if (fUnicodeCategoryNames.TryGetValue(className, out categoryArray))
            {
                return categoryArray;
            }
            return null;
        }
        #endregion
        
        #region 成员变量
        readonly UnicodeCategory[] _fCategoriesCodes;
        readonly string _fCategory;
        readonly bool _notMatch;
        #endregion

        #region 构造方法
        UnicodeCategoryTransition(RegExpState target, UnicodeCategoryTransition source)
            : base(target)
        {
            this._notMatch = source._notMatch;
            this._fCategory = source._fCategory;
            this._fCategoriesCodes = source._fCategoriesCodes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <param name="notMatch"></param>
        public UnicodeCategoryTransition(string category, bool notMatch)
        {
            this._notMatch = notMatch;
            this._fCategory = category;
            this._fCategoriesCodes = GetUnicodeCategoryListFromCharacterClassName(category);
            if (this._fCategoriesCodes == null)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Incorrect mask: unsupported unicode category '{0}'", new object[] { category }));
            }
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public override Transition Copy(RegExpState target)
        {
            return new UnicodeCategoryTransition(target, this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override char GetSampleChar()
        {
            foreach (char ch in "Aaǅʰƻ̀ः҈0Ⅰ\x00b2 \u2028\u2029\t܏\ud800\ue000_-()\x00ab\x00bb!+$^\x00a6Ƞ")
            {
                if (this.IsMatch(ch))
                {
                    return ch;
                }
            }
            return '\0';
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool IsMatch(char input)
        {
            UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(input);
            bool flag = false;
            foreach (UnicodeCategory category2 in this._fCategoriesCodes)
            {
                if (category2 == unicodeCategory)
                {
                    flag = true;
                    break;
                }
            }
            if (!this._notMatch)
            {
                return flag;
            }
            return !flag;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Concat(new object[] { @"\", this._notMatch ? 'P' : 'p', '{', this._fCategory, '}' });
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class WhiteSpaceTransition : Transition
    {
        #region 成员变量
        readonly bool notMatch;
        #endregion

        #region 构造方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="notMatch"></param>
        public WhiteSpaceTransition(bool notMatch)
        {
            this.notMatch = notMatch;
        }

        WhiteSpaceTransition(RegExpState target, bool notMatch)
            : base(target)
        {
            this.notMatch = notMatch;
        }
        #endregion 

        #region 重写方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public override Transition Copy(RegExpState target)
        {
            return new WhiteSpaceTransition(target, this.notMatch);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override char GetSampleChar()
        {
            if (!this.notMatch)
            {
                return ' ';
            }
            return '.';
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool IsMatch(char input)
        {
            bool flag;
            // hdt
            if (char.IsWhiteSpace(input) || char.IsSeparator(input))
            {
                flag = true;
            }
            else
            {
                switch (input)
                {
                    case '\t':
                    case '\n':
                    case '\v':
                    case '\f':
                    case '\r':
                    case '\x0085':
                        flag = true;
                        break;
                }
                flag = false;
            }

            //switch (char.GetUnicodeCategory(input))
            //{
            //    case UnicodeCategory.SpaceSeparator:
            //    case UnicodeCategory.LineSeparator:
            //    case UnicodeCategory.ParagraphSeparator:
            //        flag = true;
            //        break;

            //    default:
            //        switch (input)
            //        {
            //            case '\t':
            //            case '\n':
            //            case '\v':
            //            case '\f':
            //            case '\r':
            //            case '\x0085':
            //                flag = true;
            //                break;
            //        }
            //        flag = false;
            //        break;
            //}

            if (!this.notMatch)
            {
                return flag;
            }
            return !flag;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (!this.notMatch)
            {
                return @"\s";
            }
            return @"\S";
        }
        #endregion 
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class WordTransition : Transition
    {
        #region 成员变量
        readonly bool _notMatch;
        #endregion

        #region 构造方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="notMatch"></param>
        public WordTransition(bool notMatch)
        {
            this._notMatch = notMatch;
        }

        WordTransition(RegExpState target, bool notMatch)
            : base(target)
        {
            this._notMatch = notMatch;
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public override Transition Copy(RegExpState target)
        {
            return new WordTransition(target, this._notMatch);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override char GetSampleChar()
        {
            if (!this._notMatch)
            {
                return 'a';
            }
            return '.';
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override bool IsMatch(char input)
        {
            bool flag;
            switch (CharUnicodeInfo.GetUnicodeCategory(input))
            {
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.TitlecaseLetter:
                case UnicodeCategory.OtherLetter:
                case UnicodeCategory.DecimalDigitNumber:
                case UnicodeCategory.ConnectorPunctuation:
                    flag = true;
                    break;

                default:
                    flag = false;
                    break;
            }
            if (!this._notMatch)
            {
                return flag;
            }
            return !flag;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (!this._notMatch)
            {
                return @"\w";
            }
            return @"\W";
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class Transition
    {
        #region 成员变量
        readonly RegExpState _target;
        #endregion

        #region 构造方法
        /// <summary>
        /// 
        /// </summary>
        protected Transition()
            : this(new RegExpState())
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        protected Transition(RegExpState target)
        {
            this._target = target;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public virtual bool IsEmpty
        {
            get { return false; }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual bool IsExact
        {
            get { return false; }
        }

        /// <summary>
        /// 
        /// </summary>
        public RegExpState Target
        {
            get { return this._target; }
        }
        #endregion 

        #region 抽象方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public abstract Transition Copy(RegExpState target);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract char GetSampleChar();
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public abstract bool IsMatch(char input);
        #endregion
    }
}

