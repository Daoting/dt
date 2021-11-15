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
    public class LegacyMaskManager : MaskManagerStated
    {
        #region 成员变量
        readonly char _blank;
        readonly bool _ignoreMaskBlank;
        readonly LegacyMaskInfo _info;
        readonly bool _saveLiteral;
        #endregion

        #region 构造方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="blank"></param>
        /// <param name="saveLiteral"></param>
        /// <param name="ignoreMaskBlank"></param>
        public LegacyMaskManager(LegacyMaskInfo info, char blank, bool saveLiteral, bool ignoreMaskBlank) : base(new LegacyMaskManagerState(info))
        {
            this._info = info;
            this._saveLiteral = saveLiteral;
            this._blank = blank;
            this._ignoreMaskBlank = ignoreMaskBlank;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 
        /// </summary>
        protected new LegacyMaskManagerState CurrentState
        {
            get { return (LegacyMaskManagerState)base.CurrentState; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool IsEditValueDifferFromEditText
        {
            get { return false; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool IsFinal
        {
            get { return this.CurrentState.IsFinal(this._blank); }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool IsMatch
        {
            get { return ((this._ignoreMaskBlank && this.CurrentState.IsEmpty()) || this.CurrentState.IsMatch(this._blank)); }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool IsPlainTextLike
        {
            get { return true; }
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialEditValue"></param>
        public override void SetInitialEditValue(object initialEditValue)
        {
            this.SetInitialEditText(string.Format(CultureInfo.InvariantCulture, "{0}", new object[] { initialEditValue }));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialEditText"></param>
        public override void SetInitialEditText(string initialEditText)
        {
            string[] elements = this._info.GetElementsFromEditText(initialEditText, this._blank, this._saveLiteral);
            if (elements != null)
            {
                base.SetInitialState(new LegacyMaskManagerState(this._info, elements, 0, 0, 0, 0));
            }
            else
            {
                LegacyMaskManagerState state = new LegacyMaskManagerState(this._info);
                state.Insert(initialEditText);
                base.SetInitialState(new LegacyMaskManagerState(this._info, state.Elements, 0, 0, 0, 0));
            }
            this.CursorHome(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Backspace()
        {
            LegacyMaskManagerState newState = this.CurrentState.Clone();
            if (!newState.Backspace())
            {
                return false;
            }
            return base.Apply(newState, MaskManagerStated.StateChangeType.Delete);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="forceSelection"></param>
        /// <returns></returns>
        public override bool CursorEnd(bool forceSelection)
        {
            LegacyMaskManagerState newState = this.CurrentState.Clone();
            if (!newState.CursorEnd(forceSelection))
            {
                return false;
            }
            return base.Apply(newState, MaskManagerStated.StateChangeType.Terminator);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="forceSelection"></param>
        /// <returns></returns>
        public override bool CursorHome(bool forceSelection)
        {
            LegacyMaskManagerState newState = this.CurrentState.Clone();
            if (!newState.CursorHome(forceSelection))
            {
                return false;
            }
            return base.Apply(newState, MaskManagerStated.StateChangeType.Terminator);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="forceSelection"></param>
        /// <param name="isNeededKeyCheck"></param>
        /// <returns></returns>
        public override bool CursorLeft(bool forceSelection, bool isNeededKeyCheck)
        {
            if (forceSelection)
            {
                return (isNeededKeyCheck || this.CursorToDisplayPosition(this.DisplayCursorPosition - 1, forceSelection));
            }
            LegacyMaskManagerState newState = this.CurrentState.Clone();
            if (!newState.CursorLeft())
            {
                return false;
            }
            return base.Apply(newState, MaskManagerStated.StateChangeType.Terminator, isNeededKeyCheck);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="forceSelection"></param>
        /// <param name="isNeededKeyCheck"></param>
        /// <returns></returns>
        public override bool CursorRight(bool forceSelection, bool isNeededKeyCheck)
        {
            if (forceSelection)
            {
                return (isNeededKeyCheck || this.CursorToDisplayPosition(this.DisplayCursorPosition + 1, forceSelection));
            }
            LegacyMaskManagerState newState = this.CurrentState.Clone();
            if (!newState.CursorRight())
            {
                return false;
            }
            return base.Apply(newState, MaskManagerStated.StateChangeType.Terminator, isNeededKeyCheck);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newPosition"></param>
        /// <param name="forceSelection"></param>
        /// <returns></returns>
        public override bool CursorToDisplayPosition(int newPosition, bool forceSelection)
        {
            LegacyMaskManagerState newState = this.CurrentState.Clone();
            if (!newState.CursorTo(newPosition, forceSelection))
            {
                return false;
            }
            return base.Apply(newState, MaskManagerStated.StateChangeType.Terminator);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Delete()
        {
            LegacyMaskManagerState newState = this.CurrentState.Clone();
            if (!newState.Delete())
            {
                return false;
            }
            return base.Apply(newState, MaskManagerStated.StateChangeType.Delete);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected override int GetCursorPosition(MaskManagerState state)
        {
            return ((LegacyMaskManagerState) state).DisplayCursorPosition;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected override string GetDisplayText(MaskManagerState state)
        {
            return ((LegacyMaskManagerState) state).GetDisplayText(this._blank);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected override string GetEditText(MaskManagerState state)
        {
            if (this._ignoreMaskBlank && ((LegacyMaskManagerState) state).IsEmpty())
            {
                return string.Empty;
            }
            return ((LegacyMaskManagerState) state).GetEditText(this._blank, this._saveLiteral);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected override object GetEditValue(MaskManagerState state)
        {
            return this.GetEditText(state);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected override int GetSelectionAnchor(MaskManagerState state)
        {
            return ((LegacyMaskManagerState) state).DisplaySelectionAnchor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="insertion"></param>
        /// <returns></returns>
        public override bool Insert(string insertion)
        {
            LegacyMaskManagerState newState = this.CurrentState.Clone();
            if (!newState.Insert(insertion))
            {
                return false;
            }
            return base.Apply(newState, MaskManagerStated.StateChangeType.Insert);
        }
        #endregion
    }
}

