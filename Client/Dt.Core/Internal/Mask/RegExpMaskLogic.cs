#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2010-09-24 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Globalization;
#endregion

namespace Dt.Core.Mask
{
    /// <summary>
    /// 
    /// </summary>
    public class RegExpMaskLogic
    {
        #region 静态内容
        static string StringWithoutLastChar(string str)
        {
            if (str.Length != 0)
            {
                return str.Substring(0, str.Length - 1);
            }
            return null;
        }
        #endregion

        #region 成员变量
        bool _IsAutoComplete;
        RegExpDfa _regExp;
        #endregion

        #region 外部方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="regExp"></param>
        /// <param name="isAutoComplete"></param>
        public RegExpMaskLogic(RegExpDfa regExp, bool isAutoComplete)
        {
            this._IsAutoComplete = true;
            this._regExp = regExp;
            this._IsAutoComplete = isAutoComplete;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="regExp"></param>
        /// <param name="culture"></param>
        /// <param name="isAutoComplete"></param>
        public RegExpMaskLogic(string regExp, CultureInfo culture, bool isAutoComplete) : this(RegExpDfa.Parse(regExp, culture), isAutoComplete) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="head"></param>
        /// <param name="tail"></param>
        /// <returns></returns>
        public MaskLogicResult GetBackspaceResult(string head, string tail)
        {
            if (!this._IsAutoComplete)
            {
                if (head.Length != 0)
                {
                    return this.GetReplaceResult(StringWithoutLastChar(head), head.Substring(head.Length - 1), tail, string.Empty);
                }
                return null;
            }
            if (head.Length > 0)
            {
                string cursorBase = head.Substring(0, head.Length - 1);
                string text = cursorBase + tail;
                if (this.IsMatch(text))
                {
                    return this.CreateResult(text, cursorBase);
                }
            }
            string stringWithoutExacts = StringWithoutLastChar(this.RemoveExacts(string.Empty, head));
            if (stringWithoutExacts == null)
            {
                return null;
            }
            string str4 = this.RestoreExacts(string.Empty, stringWithoutExacts);
            return this.GetReplaceResult(str4, head.Substring(str4.Length), tail, string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="head"></param>
        /// <param name="tail"></param>
        /// <returns></returns>
        public MaskLogicResult GetDeleteResult(string head, string tail)
        {
            if (!this._IsAutoComplete)
            {
                if (tail.Length != 0)
                {
                    return this.GetReplaceResult(head, tail.Substring(0, 1), tail.Substring(1), string.Empty);
                }
                return null;
            }
            if (tail.Length > 0)
            {
                string text = head + tail.Substring(1);
                if (this.IsMatch(text))
                {
                    return this.CreateResult(text, head);
                }
            }
            string str2 = this.RemoveExacts(head, tail);
            if (str2.Length == 0)
            {
                return null;
            }
            string str3 = this.RestoreExacts(head, str2.Substring(0, 1));
            if (!(head + tail).StartsWith(str3))
            {
                return null;
            }
            int length = str3.Length - head.Length;
            return this.GetReplaceResult(head, tail.Substring(0, length), tail.Substring(length), string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="anySymbolHolder"></param>
        /// <returns></returns>
        public string GetMaskedText(string text, char anySymbolHolder)
        {
            string placeHolders = this._regExp.GetPlaceHolders(text, anySymbolHolder);
            if (placeHolders.Length == 0)
            {
                return text;
            }
            return (text + ' ' + placeHolders);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="head"></param>
        /// <param name="replaced"></param>
        /// <param name="tail"></param>
        /// <param name="inserted"></param>
        /// <returns></returns>
        public MaskLogicResult GetReplaceResult(string head, string replaced, string tail, string inserted)
        {
            if (this.IsValidStart(head + replaced + tail))
            {
                MaskLogicResult result;
                if (this._IsAutoComplete)
                {
                    tail = this.AutoCompleteTailPreprocessing(head + replaced, tail);
                }
                string str = head + replaced + tail;
                if (inserted == replaced)
                {
                    return this.CreateResult(str, head + inserted);
                }
                string text = head + inserted;
                if (!this.IsValidStart(text))
                {
                    text = null;
                }
                string tailNonexacts = this.RemoveExacts(head + replaced, tail);
                string appended = this.RestoreExacts(head, inserted);
                if (this._IsAutoComplete && (inserted.Length > 1))
                {
                    result = this.InsertCaseTailNonExacts(text, tailNonexacts);
                    if (result != null)
                    {
                        return result;
                    }
                }
                if (this._IsAutoComplete)
                {
                    result = this.InsertCaseTailNonExacts(appended, tailNonexacts);
                    if (result != null)
                    {
                        return result;
                    }
                }
                if (!this._IsAutoComplete || (inserted.Length > 1))
                {
                    result = this.InsertCaseTailPlain(text, tail);
                    if (result != null)
                    {
                        return result;
                    }
                }
                if (this._IsAutoComplete || (inserted.Length > 1))
                {
                    result = this.InsertCaseTailPlain(appended, tail);
                    if (result != null)
                    {
                        return result;
                    }
                }
                if (!this._IsAutoComplete)
                {
                    result = this.InsertCaseTailNonExacts(text, tailNonexacts);
                    if (result != null)
                    {
                        return result;
                    }
                }
                if (!this._IsAutoComplete && (inserted.Length > 1))
                {
                    result = this.InsertCaseTailNonExacts(appended, tailNonexacts);
                    if (result != null)
                    {
                        return result;
                    }
                }
                if ((inserted.Length > 0) && (!this._IsAutoComplete || (inserted.Length > 1)))
                {
                    result = this.InsertCaseByOneChar(false, head, replaced, inserted, tail);
                    if (result != null)
                    {
                        return result;
                    }
                }
                if ((inserted.Length > 0) && (this._IsAutoComplete || (inserted.Length > 1)))
                {
                    result = this.InsertCaseByOneChar(true, head, replaced, inserted, tail);
                    if (result != null)
                    {
                        return result;
                    }
                }
                if ((this._IsAutoComplete && (replaced.Length == 0)) && (inserted.Length == 1))
                {
                    string str5 = this.SmartAutoComplete(str);
                    if ((str5 != null) && (str5 != str))
                    {
                        return this.CreateResult(str5, head);
                    }
                }
                if (inserted.Length > 1)
                {
                    result = this.InsertCaseByOneChar(true, head, replaced, inserted, tail, true);
                    if ((result != null) && (str != result.EditText))
                    {
                        return result;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool IsFinal(string text)
        {
            return this._regExp.IsFinal(text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool IsMatch(string text)
        {
            return this._regExp.IsMatch(text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="testedPositionInEditText"></param>
        /// <returns></returns>
        public bool IsValidCursorPosition(string text, int testedPositionInEditText)
        {
            if ((testedPositionInEditText < 0) || (testedPositionInEditText > text.Length))
            {
                return false;
            }
            if (!this._IsAutoComplete)
            {
                return true;
            }
            string before = text.Substring(0, testedPositionInEditText);
            return (before == this.SmartAutoComplete(before));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public bool IsValidStart(string text)
        {
            return this._regExp.IsValidStart(text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseText"></param>
        /// <returns></returns>
        public string OptimisticallyExpand(string baseText)
        {
            string text = baseText + this._regExp.GetOptimisticHint(baseText);
            if (this.IsMatch(text))
            {
                return text;
            }
            return baseText;
        }
        #endregion

        #region 内部方法
        string AutoCompleteResultProcessing(string origin)
        {
            object[] placeHoldersInfo = this._regExp.GetPlaceHoldersInfo(origin);
            string displayText = origin;
            foreach (object obj2 in placeHoldersInfo)
            {
                if (obj2 == null)
                {
                    return displayText;
                }
                string str2 = displayText + ((char) obj2);
                object[] objArray2 = this._regExp.GetPlaceHoldersInfo(displayText);
                object[] objArray3 = this._regExp.GetPlaceHoldersInfo(str2);
                if (objArray3.Length != (objArray2.Length - 1))
                {
                    return displayText;
                }
                bool flag = true;
                for (int i = 0; i < objArray3.Length; i++)
                {
                    object obj3 = objArray3[i];
                    object obj4 = objArray2[i + 1];
                    if ((obj3 != null) || (obj4 != null))
                    {
                        if (obj3 != null)
                        {
                            char ch = (char) obj3;
                            if (ch.Equals(obj4))
                            {
                                goto Label_009D;
                            }
                        }
                        flag = false;
                        break;
                    Label_009D:;
                    }
                }
                if (!flag)
                {
                    return displayText;
                }
                displayText = str2;
            }
            return displayText;
        }

        string AutoCompleteTailPreprocessing(string head, string tail)
        {
            string str = head + tail;
            string str2 = tail;
            while (str2.Length > 0)
            {
                string str3 = StringWithoutLastChar(str2);
                if (!(this.AutoCompleteResultProcessing(head + str3) == str))
                {
                    break;
                }
                str2 = str3;
            }
            if (str2 == tail)
            {
                return tail;
            }
            string str4 = this.SmartAutoComplete(head + str2);
            if (str4.Length < head.Length)
            {
                return tail;
            }
            if ((head.Length + tail.Length) <= str4.Length)
            {
                return tail;
            }
            return str4.Substring(head.Length);
        }

        MaskLogicResult CreateResult(string result, string cursorBase)
        {
            if (this._IsAutoComplete)
            {
                result = this.SmartAutoComplete(result);
                result = this.AutoCompleteResultProcessing(result);
                cursorBase = this.SmartAutoComplete(cursorBase);
            }
            return new MaskLogicResult(result, cursorBase.Length);
        }

        string ExactsAppend(string before)
        {
            string text = before;
            while (true)
            {
                RegExpAutoCompleteInfo autoCompleteInfo = this._regExp.GetAutoCompleteInfo(text);
                if (autoCompleteInfo.DfaAutoCompleteType != DfaAutoCompleteType.ExactChar)
                {
                    return text;
                }
                text = text + autoCompleteInfo.AutoCompleteChar;
            }
        }

        string ExactsTruncate(string before)
        {
            string str = before;
            while (str.Length > 0)
            {
                string text = StringWithoutLastChar(str);
                if (this._regExp.GetAutoCompleteInfo(text).DfaAutoCompleteType != DfaAutoCompleteType.ExactChar)
                {
                    return str;
                }
                str = text;
            }
            return str;
        }

        string GetTail(string oldHead, string oldTail, string head)
        {
            string str2;
            string stringWithoutExacts = this.RemoveExacts(oldHead, oldTail);
            if (this._IsAutoComplete)
            {
                str2 = this.RestoreExacts(head, stringWithoutExacts);
                if (str2 != null)
                {
                    return str2;
                }
            }
            str2 = head + oldTail;
            if (this.IsValidStart(str2))
            {
                return str2;
            }
            if (!this._IsAutoComplete)
            {
                str2 = this.RestoreExacts(head, stringWithoutExacts);
                if (str2 != null)
                {
                    return str2;
                }
            }
            if (oldTail.Length > 0)
            {
                str2 = head + oldTail.Substring(1);
                if (this.IsValidStart(str2))
                {
                    return str2;
                }
            }
            if (stringWithoutExacts.Length > 0)
            {
                str2 = this.RestoreExacts(head, stringWithoutExacts.Substring(1));
                if (str2 != null)
                {
                    return str2;
                }
            }
            return null;
        }

        MaskLogicResult InsertCaseByOneChar(bool isSmart, string head, string replaced, string inserted, string tail)
        {
            return this.InsertCaseByOneChar(isSmart, head, replaced, inserted, tail, false);
        }

        MaskLogicResult InsertCaseByOneChar(bool isSmart, string head, string replaced, string inserted, string tail, bool isLastChance)
        {
            string str = head;
            string str2 = tail;
            string str3 = replaced;
            foreach (char ch in inserted)
            {
                string str4;
                string str5;
                if (!this.InsertCaseByOneCharBody(out str4, out str5, isSmart, str, str3, ch, str2))
                {
                    char input = char.ToUpper(ch);
                    if (input == ch)
                    {
                        input = char.ToLower(ch);
                    }
                    if (input == ch)
                    {
                        if (!isLastChance)
                        {
                            return null;
                        }
                        goto Label_0090;
                    }
                    if (!this.InsertCaseByOneCharBody(out str4, out str5, isSmart, str, str3, input, str2))
                    {
                        if (isLastChance)
                        {
                            goto Label_0090;
                        }
                        return null;
                    }
                }
                str = str4;
                str3 = string.Empty;
                str2 = str5.Substring(Math.Min(str4.Length, str5.Length));
            Label_0090: ;
            }
            if (!this.IsValidStart(str + str2))
            {
                return null;
            }
            return this.CreateResult(str + str2, str);
        }

        bool InsertCaseByOneCharBody(out string nextHead, out string nextFull, bool isSmart, string head, string replaced, char input, string tail)
        {
            nextFull = null;
            if (isSmart)
            {
                nextHead = this.SmartAppend(head, input);
                if (nextHead == null)
                {
                    return false;
                }
            }
            else
            {
                nextHead = head + input;
            }
            nextFull = this.GetTail(head + replaced, tail, nextHead);
            return (nextFull != null);
        }

        MaskLogicResult InsertCaseTailNonExacts(string appended, string tailNonexacts)
        {
            if (appended != null)
            {
                string result = this.RestoreExacts(appended, tailNonexacts);
                if (result != null)
                {
                    return this.CreateResult(result, appended);
                }
            }
            return null;
        }

        MaskLogicResult InsertCaseTailPlain(string appended, string tail)
        {
            if (appended != null)
            {
                string text = appended + tail;
                if (this.IsValidStart(text))
                {
                    return this.CreateResult(text, appended);
                }
            }
            return null;
        }

        string RemoveExacts(string stringBefore, string stringWithExacts)
        {
            char ch;
            RegExpAutoCompleteInfo autoCompleteInfo;
            string text = stringBefore + stringWithExacts;
            string str2 = stringBefore;
            string str3 = string.Empty;
            if (this.IsValidStart(text))
            {
                goto Label_00B3;
            }
            return null;
        Label_0072: ;
            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Internal error: non-covered case '{0}'", new object[] { autoCompleteInfo.DfaAutoCompleteType.ToString() }));
        Label_00A6:
            str2 = str2 + ch;
        Label_00B3:
            if (str2.Length < text.Length)
            {
                ch = text[str2.Length];
                autoCompleteInfo = this._regExp.GetAutoCompleteInfo(str2);
                switch (autoCompleteInfo.DfaAutoCompleteType)
                {
                    case DfaAutoCompleteType.None:
                    case DfaAutoCompleteType.FinalOrExactBeforeFinal:
                    case DfaAutoCompleteType.FinalOrExactBeforeFinalOrNone:
                        str3 = str3 + ch;
                        goto Label_00A6;

                    case DfaAutoCompleteType.ExactChar:
                    case DfaAutoCompleteType.FinalOrExactBeforeNone:
                        goto Label_00A6;

                    case DfaAutoCompleteType.Final:
                        goto Label_0072;
                }
                goto Label_0072;
            }
            return str3;
        }

        string RestoreExacts(string stringBefore, string stringWithoutExacts)
        {
            if (!this.IsValidStart(stringBefore))
            {
                return null;
            }
            string before = this.SmartAutoComplete(stringBefore);
            foreach (char ch in stringWithoutExacts)
            {
                before = this.SmartAppend(before, ch);
                if (before == null)
                {
                    return null;
                }
            }
            return before;
        }

        string SmartAppend(string before, char input)
        {
            if (!this.IsValidStart(before))
            {
                return null;
            }
            string text = this.SmartAutoComplete(before);
            RegExpAutoCompleteInfo autoCompleteInfo = this._regExp.GetAutoCompleteInfo(text);
            switch (autoCompleteInfo.DfaAutoCompleteType)
            {
                case DfaAutoCompleteType.None:
                    text = text + input;
                    break;

                case DfaAutoCompleteType.Final:
                    return null;

                case DfaAutoCompleteType.FinalOrExactBeforeNone:
                    text = this.ExactsAppend(text + autoCompleteInfo.AutoCompleteChar) + input;
                    break;

                case DfaAutoCompleteType.FinalOrExactBeforeFinal:
                    text = text + autoCompleteInfo.AutoCompleteChar;
                    break;

                case DfaAutoCompleteType.FinalOrExactBeforeFinalOrNone:
                    text = text + autoCompleteInfo.AutoCompleteChar;
                    if (input != autoCompleteInfo.AutoCompleteChar)
                    {
                        text = this.ExactsAppend(text) + input;
                    }
                    break;

                default:
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Internal error: non-covered case '{0}'", new object[] { autoCompleteInfo.DfaAutoCompleteType.ToString() }));
            }
            return this.SmartAutoComplete(text);
        }

        string SmartAutoComplete(string before)
        {
            if (!this.IsValidStart(before))
            {
                return null;
            }
            string text = StringWithoutLastChar(this.ExactsTruncate(before));
            if ((text != null) && (this._regExp.GetAutoCompleteInfo(text).DfaAutoCompleteType == DfaAutoCompleteType.FinalOrExactBeforeNone))
            {
                return text;
            }
            return this.ExactsAppend(before);
        }
        #endregion
    }
}

