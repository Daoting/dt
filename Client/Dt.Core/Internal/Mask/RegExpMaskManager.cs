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
    public class RegExpMaskManager : MaskManagerStated
    {
        #region 成员变量
        char _anySymbolPlaceHolder;
        bool _isOptimistic;
        RegExpMaskLogic _logic;
        bool _reverseDfa;
        bool _showPlaceHolders;
        #endregion
        
        #region 属性
        /// <summary>
        /// 
        /// </summary>
        protected new RegExpMaskManagerState CurrentState
        {
            get { return (RegExpMaskManagerState)base.CurrentState; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool IsFinal
        {
            get { return ((DisplayCursorPosition == DisplayText.Length) && _logic.IsFinal(GetCurrentEditText())); }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool IsMatch
        {
            get { return _logic.IsMatch(GetCurrentEditText()); }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool IsPlainTextLike
        {
            get { return true; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool IsEditValueDifferFromEditText
        {
            get { return false; }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="regExp"></param>
        /// <param name="reverseDfa"></param>
        /// <param name="isAutoComplete"></param>
        /// <param name="isOptimistic"></param>
        /// <param name="showPlaceHolders"></param>
        /// <param name="anySymbolPlaceHolder"></param>
        /// <param name="managerCultureInfo"></param>
        public RegExpMaskManager(string regExp, bool reverseDfa, bool isAutoComplete, bool isOptimistic, bool showPlaceHolders, char anySymbolPlaceHolder, CultureInfo managerCultureInfo)
            : base(RegExpMaskManagerState.Empty)
        {
            _logic = new RegExpMaskLogic(RegExpDfa.Parse(regExp, reverseDfa, managerCultureInfo), isAutoComplete);
            _isOptimistic = isOptimistic;
            _showPlaceHolders = showPlaceHolders;
            _anySymbolPlaceHolder = anySymbolPlaceHolder;
            _reverseDfa = reverseDfa;
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 设置初始值
        /// </summary>
        /// <param name="initialEditValue"></param>
        public override void SetInitialEditValue(object initialEditValue)
        {
            SetInitialEditText(string.Format(CultureInfo.InvariantCulture, "{0}", new object[] { initialEditValue }));
        }

        /// <summary>
        /// 设置初始文本
        /// </summary>
        /// <param name="initialEditText"></param>
        public override void SetInitialEditText(string initialEditText)
        {
            MaskLogicResult result = _logic.GetReplaceResult(initialEditText ?? string.Empty, string.Empty, string.Empty, string.Empty);
            if (result != null)
            {
                Apply(result.EditText, result.CursorPosition, result.CursorPosition, MaskManagerStated.StateChangeType.Insert);
            }
            else
            {
                base.SetInitialState(RegExpMaskManagerState.Empty);
                if (!Insert(initialEditText))
                {
                    bool flag = false;
                    foreach (char ch in initialEditText)
                    {
                        if (Insert(ch.ToString()))
                        {
                            flag = true;
                        }
                    }
                    if (!flag)
                    {
                        Insert(string.Empty);
                    }
                }
                base.SetInitialState(new RegExpMaskManagerState(CurrentState.EditText, CurrentState.CursorPosition, CurrentState.SelectionAnchor));
            }
        }

        /// <summary>
        /// 新输入
        /// </summary>
        /// <param name="insertion"></param>
        /// <returns></returns>
        public override bool Insert(string insertion)
        {
            RegExpMaskManagerState state;
            MaskManagerStated.StateChangeType changeType = ((insertion.Length == 0) && base.IsSelection) ? MaskManagerStated.StateChangeType.Delete : MaskManagerStated.StateChangeType.Insert;
            string head = GetCurrentEditText().Substring(0, base.DisplaySelectionStart);
            string replaced = GetCurrentEditText().Substring(base.DisplaySelectionStart, base.DisplaySelectionLength);
            string tail = GetCurrentEditText().Substring(base.DisplaySelectionEnd);
            MaskLogicResult result = _logic.GetReplaceResult(head, replaced, tail, insertion);
            if (result == null)
            {
                return false;
            }
            if (_reverseDfa)
            {
                state = new RegExpMaskManagerState(result.EditText, base.DisplaySelectionStart, base.DisplaySelectionStart);
            }
            else if ((_isOptimistic && (changeType == MaskManagerStated.StateChangeType.Insert)) && (tail.Length == 0))
            {
                string editText = _logic.OptimisticallyExpand(result.EditText);
                state = new RegExpMaskManagerState(editText, result.CursorPosition, editText.Length);
            }
            else
            {
                state = new RegExpMaskManagerState(result.EditText, result.CursorPosition, result.CursorPosition);
            }
            return base.Apply(state, changeType);
        }

        /// <summary>
        /// 退格操作
        /// </summary>
        /// <returns></returns>
        public override bool Backspace()
        {
            if (base.IsSelection)
            {
                return Insert(string.Empty);
            }
            MaskLogicResult backspaceResult = _logic.GetBackspaceResult(GetCurrentEditText().Substring(0, DisplayCursorPosition), GetCurrentEditText().Substring(DisplayCursorPosition));
            return Apply(backspaceResult, MaskManagerStated.StateChangeType.Delete);
        }

        /// <summary>
        /// 删除操作
        /// </summary>
        /// <returns></returns>
        public override bool Delete()
        {
            if (base.IsSelection)
            {
                return Insert(string.Empty);
            }
            MaskLogicResult deleteResult = _logic.GetDeleteResult(GetCurrentEditText().Substring(0, DisplayCursorPosition), GetCurrentEditText().Substring(DisplayCursorPosition));
            return Apply(deleteResult, MaskManagerStated.StateChangeType.Delete);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="forceSelection"></param>
        /// <returns></returns>
        public override bool CursorEnd(bool forceSelection)
        {
            return CursorToDisplayPosition(DisplayText.Length, forceSelection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="forceSelection"></param>
        /// <returns></returns>
        public override bool CursorHome(bool forceSelection)
        {
            return CursorToDisplayPosition(0, forceSelection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="forceSelection"></param>
        /// <param name="isNeededKeyCheck"></param>
        /// <returns></returns>
        public override bool CursorLeft(bool forceSelection, bool isNeededKeyCheck)
        {
            for (int i = CurrentState.CursorPosition - 1; i >= 0; i--)
            {
                if (IsValidCursorPosition(i))
                {
                    return MoveCursorTo(i, forceSelection, isNeededKeyCheck);
                }
            }
            return ((base.IsSelection && !forceSelection) && MoveCursorTo(CurrentState.CursorPosition, false, isNeededKeyCheck));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="forceSelection"></param>
        /// <param name="isNeededKeyCheck"></param>
        /// <returns></returns>
        public override bool CursorRight(bool forceSelection, bool isNeededKeyCheck)
        {
            for (int i = CurrentState.CursorPosition + 1; i <= CurrentState.EditText.Length; i++)
            {
                if (IsValidCursorPosition(i))
                {
                    return MoveCursorTo(i, forceSelection, isNeededKeyCheck);
                }
            }
            return ((base.IsSelection && !forceSelection) && MoveCursorTo(CurrentState.CursorPosition, false, isNeededKeyCheck));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newPosition"></param>
        /// <param name="forceSelection"></param>
        /// <returns></returns>
        public override bool CursorToDisplayPosition(int newPosition, bool forceSelection)
        {
            for (int i = 0; i <= GetCurrentEditText().Length; i++)
            {
                if (IsValidCursorPosition(newPosition + i))
                {
                    return MoveCursorTo(newPosition + i, forceSelection);
                }
                if (IsValidCursorPosition(newPosition - i))
                {
                    return MoveCursorTo(newPosition - i, forceSelection);
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected override int GetCursorPosition(MaskManagerState state)
        {
            return ((RegExpMaskManagerState)state).CursorPosition;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected override string GetDisplayText(MaskManagerState state)
        {
            if (!_showPlaceHolders)
            {
                return GetCurrentEditText();
            }
            return _logic.GetMaskedText(GetCurrentEditText(), _anySymbolPlaceHolder);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected override string GetEditText(MaskManagerState state)
        {
            return ((RegExpMaskManagerState)state).EditText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected override object GetEditValue(MaskManagerState state)
        {
            return GetEditText(state);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected override int GetSelectionAnchor(MaskManagerState state)
        {
            return ((RegExpMaskManagerState)state).SelectionAnchor;
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="editText"></param>
        /// <param name="cursorPosition"></param>
        /// <param name="selectionAnchor"></param>
        /// <param name="changeType"></param>
        /// <returns></returns>
        protected bool Apply(string editText, int cursorPosition, int selectionAnchor, MaskManagerStated.StateChangeType changeType)
        {
            return base.Apply(new RegExpMaskManagerState(editText, cursorPosition, selectionAnchor), changeType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="changeType"></param>
        /// <returns></returns>
        protected bool Apply(MaskLogicResult result, MaskManagerStated.StateChangeType changeType)
        {
            if (result == null)
            {
                return false;
            }
            return base.Apply(new RegExpMaskManagerState(result.EditText, result.CursorPosition, result.CursorPosition), changeType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="testedPosition"></param>
        /// <returns></returns>
        protected bool IsValidCursorPosition(int testedPosition)
        {
            return _logic.IsValidCursorPosition(GetCurrentEditText(), testedPosition);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newPosition"></param>
        /// <param name="forceSelection"></param>
        /// <returns></returns>
        protected bool MoveCursorTo(int newPosition, bool forceSelection)
        {
            return MoveCursorTo(newPosition, forceSelection, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newPosition"></param>
        /// <param name="forceSelection"></param>
        /// <param name="isNeededKeyCheck"></param>
        /// <returns></returns>
        protected bool MoveCursorTo(int newPosition, bool forceSelection, bool isNeededKeyCheck)
        {
            return base.Apply(new RegExpMaskManagerState(GetCurrentEditText(), newPosition, forceSelection ? CurrentState.SelectionAnchor : newPosition), MaskManagerStated.StateChangeType.Terminator, isNeededKeyCheck);
        }
        #endregion
    }
}

