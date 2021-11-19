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
using System.Text.RegularExpressions;
#endregion

namespace Dt.Core.Mask
{
    /// <summary>
    /// 
    /// </summary>
    public class DateTimeMaskManager : MaskManager
    {
        #region 静态内容
        /// <summary>
        /// 
        /// </summary>
        public static bool AlwaysTodayOnClearSelectAll;

        /// <summary>
        /// 
        /// </summary>
        public static bool DoNotClearValueOnInsertAfterSelectAll;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputCulture"></param>
        /// <returns></returns>
        public static DateTimeFormatInfo GetGoodCalendarDateTimeFormatInfo(CultureInfo inputCulture)
        {
            if (IsGoodCalendar(inputCulture.DateTimeFormat.Calendar))
            {
                return inputCulture.DateTimeFormat;
            }
            DateTimeFormatInfo info = (DateTimeFormatInfo)inputCulture.DateTimeFormat.Clone();
            foreach (Calendar calendar in inputCulture.OptionalCalendars)
            {
                if (IsGoodCalendar(calendar))
                {
                    info.Calendar = calendar;
                    return info;
                }
            }
            return DateTimeFormatInfo.InvariantInfo;
        }

        static bool IsGoodCalendar(Calendar calendar)
        {
            // hdt
            return true;
            //return ((calendar is GregorianCalendar) || ((calendar is KoreanCalendar) || ((calendar is TaiwanCalendar) || (calendar is ThaiBuddhistCalendar))));
        }
        #endregion

        #region 成员变量
        /// <summary>
        /// 
        /// </summary>
        protected readonly bool _AllowNull;

        int _cachedDCP;

        int _cachedDSA;

        string _cachedDT;

        int _cachedIndex;

        DateTime _cachedValue;

        /// <summary>
        /// 
        /// </summary>
        protected DateTimeElementEditor _fCurrentElementEditor;

        /// <summary>
        /// 
        /// </summary>
        protected DateTime? _fCurrentValue;

        /// <summary>
        /// 
        /// </summary>
        protected DateTimeMaskFormatInfo _fFormatInfo;

        /// <summary>
        /// 
        /// </summary>
        protected DateTimeFormatInfo _fInitialDateTimeFormatInfo;

        /// <summary>
        /// 
        /// </summary>
        protected DateTime? _fInitialEditValue;

        /// <summary>
        /// 
        /// </summary>
        protected string _fInitialMask;

        /// <summary>
        /// 
        /// </summary>
        protected int _fSelectedFormatInfoIndex;

        /// <summary>
        /// 
        /// </summary>
        protected DateTime? _fUndoValue;

        /// <summary>
        /// 
        /// </summary>
        protected readonly bool _IsOperatorMask;
        #endregion

        #region 构造方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="isOperatorMask"></param>
        /// <param name="culture"></param>
        /// <param name="allowNull"></param>
        public DateTimeMaskManager(string mask, bool isOperatorMask, CultureInfo culture, bool allowNull)
        {
            _AllowNull = true;
            _cachedValue = new DateTime(0L);
            _cachedIndex = -1;
            _cachedDCP = -1;
            _cachedDSA = -1;
            _AllowNull = allowNull;
            _IsOperatorMask = isOperatorMask;
            _fInitialMask = mask;
            _fInitialDateTimeFormatInfo = GetGoodCalendarDateTimeFormatInfo(culture);
            _fFormatInfo = new DateTimeMaskFormatInfo(mask, _fInitialDateTimeFormatInfo);
            CursorHome(false);
        }
        #endregion

        #region 属性
        /// <summary>
        /// 
        /// </summary>
        protected DateTime? CurrentValue
        {
            get { return _fCurrentValue; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override int DisplayCursorPosition
        {
            get
            {
                if (SelectedElement == null)
                {
                    return 0;
                }
                if (IsElementEdited)
                {
                    return (FormatInfo.Format(NonEmptyCurrentValue, 0, SelectedFormatInfoIndex - 1).Length + GetCurrentElementEditor().DisplayText.Length);
                }
                if (!CurrentValue.HasValue)
                {
                    return 0;
                }
                VerifyCache();
                if (_cachedDCP < 0)
                {
                    _cachedDCP = FormatInfo.Format(NonEmptyCurrentValue, 0, SelectedFormatInfoIndex).Length;
                }
                return _cachedDCP;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override int DisplaySelectionAnchor
        {
            get
            {
                if (SelectedElement == null)
                {
                    return 0;
                }
                if (IsElementEdited)
                {
                    return FormatInfo.Format(NonEmptyCurrentValue, 0, SelectedFormatInfoIndex - 1).Length;
                }
                if (!CurrentValue.HasValue)
                {
                    return 0;
                }
                VerifyCache();
                if (_cachedDSA < 0)
                {
                    _cachedDSA = FormatInfo.Format(NonEmptyCurrentValue, 0, SelectedFormatInfoIndex - 1).Length;
                }
                return _cachedDSA;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override string DisplayText
        {
            get
            {
                if (IsElementEdited)
                {
                    return (FormatInfo.Format(NonEmptyCurrentValue, 0, SelectedFormatInfoIndex - 1) + GetCurrentElementEditor().DisplayText + FormatInfo.Format(NonEmptyCurrentValue, SelectedFormatInfoIndex + 1, FormatInfo.Count - 1));
                }
                if (!CurrentValue.HasValue)
                {
                    return string.Empty;
                }
                VerifyCache();
                if (_cachedDT == null)
                {
                    _cachedDT = FormatInfo.Format(NonEmptyCurrentValue);
                }
                return _cachedDT;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected DateTimeMaskFormatInfo FormatInfo
        {
            get { return _fFormatInfo; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool IsEditValueDifferFromEditText
        {
            get { return true; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool IsElementEdited
        {
            get { return (_fCurrentElementEditor != null); }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool IsPlainTextLike
        {
            get { return false; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected DateTime NonEmptyCurrentValue
        {
            get
            {
                if (CurrentValue.HasValue)
                {
                    return CurrentValue.Value;
                }
                return GetClearValue();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected DateTimeMaskFormatElementEditable SelectedElement
        {
            get
            {
                if (SelectedFormatInfoIndex < 0)
                {
                    return null;
                }
                return (DateTimeMaskFormatElementEditable)FormatInfo[SelectedFormatInfoIndex];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected int SelectedFormatInfoIndex
        {
            get { return _fSelectedFormatInfoIndex; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected DateTime? UndoValue
        {
            get { return _fUndoValue; }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialEditValue"></param>
        public void SetInitialEditValue(DateTime? initialEditValue)
        {
            KillCurrentElementEditor();
            _fCurrentValue = initialEditValue;
            _fUndoValue = initialEditValue;
            _fInitialEditValue = initialEditValue;
            CursorHome(false);
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialEditValue"></param>
        public override void SetInitialEditValue(object initialEditValue)
        {
            if (initialEditValue == null)
            {
                SetInitialEditValue(null);
            }
            else if (initialEditValue is DateTime dt)
            {
                if (dt == default)
                    SetInitialEditValue(null);
                else
                    SetInitialEditValue(dt);
            }
            else if (initialEditValue is TimeSpan span)
            {
                SetInitialEditValue((DateTime?)new DateTime(span.Ticks));
            }
            else
            {
                SetInitialEditText(string.Format(CultureInfo.InvariantCulture, "{0}", new object[] { initialEditValue }));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="initialEditText"></param>
        public override void SetInitialEditText(string initialEditText)
        {
            KillCurrentElementEditor();
            DateTime? initialEditValue = null;
            if (!string.IsNullOrEmpty(initialEditText))
            {
                try
                {
                    initialEditValue = new DateTime?(DateTime.Parse(initialEditText, CultureInfo.InvariantCulture));
                }
                catch
                {
                }
            }
            SetInitialEditValue(initialEditValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Backspace()
        {
            DateTimeElementEditor currentElementEditor = GetCurrentElementEditor();
            if (currentElementEditor == null)
            {
                return false;
            }
            return currentElementEditor.Delete();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void ClearAfterSelectAll()
        {
            KillCurrentElementEditor();
            DateTime? newEditValue = null;
            if (!_AllowNull)
            {
                newEditValue = new DateTime?(GetClearValue());
            }
            if (RaiseEditTextChanging(newEditValue))
            {
                _fUndoValue = CurrentValue;
                _fCurrentValue = newEditValue;
                base.RaiseEditTextChanged();
            }
            CursorHome(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="forceSelection"></param>
        /// <returns></returns>
        public override bool CursorEnd(bool forceSelection)
        {
            ApplyCurrentElementEditor();
            if (!CurrentValue.HasValue)
            {
                return CursorHome(forceSelection);
            }
            _fSelectedFormatInfoIndex = -1;
            for (int i = FormatInfo.Count - 1; i >= 0; i--)
            {
                if (FormatInfo[i].Editable)
                {
                    _fSelectedFormatInfoIndex = i;
                    break;
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="forceSelection"></param>
        /// <returns></returns>
        public override bool CursorHome(bool forceSelection)
        {
            ApplyCurrentElementEditor();
            _fSelectedFormatInfoIndex = -1;
            for (int i = 0; i < FormatInfo.Count; i++)
            {
                if (FormatInfo[i].Editable)
                {
                    _fSelectedFormatInfoIndex = i;
                    break;
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="forceSelection"></param>
        /// <param name="isNeededKeyCheck"></param>
        /// <returns></returns>
        public override bool CursorLeft(bool forceSelection, bool isNeededKeyCheck)
        {
            bool flag = false;
            if (IsElementEdited)
            {
                if (!isNeededKeyCheck)
                {
                    flag = true;
                    ApplyCurrentElementEditor();
                }
            }
            else if (!CurrentValue.HasValue)
            {
                return false;
            }
            for (int i = SelectedFormatInfoIndex - 1; i >= 0; i--)
            {
                if (FormatInfo[i].Editable)
                {
                    if (!isNeededKeyCheck)
                    {
                        _fSelectedFormatInfoIndex = i;
                    }
                    return true;
                }
            }
            return flag;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="forceSelection"></param>
        /// <param name="isNeededKeyCheck"></param>
        /// <returns></returns>
        public override bool CursorRight(bool forceSelection, bool isNeededKeyCheck)
        {
            bool flag = false;
            if (IsElementEdited)
            {
                if (!isNeededKeyCheck)
                {
                    flag = true;
                    ApplyCurrentElementEditor();
                }
            }
            else if (!CurrentValue.HasValue)
            {
                return false;
            }
            for (int i = SelectedFormatInfoIndex + 1; i < FormatInfo.Count; i++)
            {
                if (FormatInfo[i].Editable)
                {
                    if (!isNeededKeyCheck)
                    {
                        _fSelectedFormatInfoIndex = i;
                    }
                    return true;
                }
            }
            return flag;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newPosition"></param>
        /// <param name="forceSelection"></param>
        /// <returns></returns>
        public override bool CursorToDisplayPosition(int newPosition, bool forceSelection)
        {
            ApplyCurrentElementEditor();
            if (!CurrentValue.HasValue)
            {
                return false;
            }
            int formatIndexFromPosition = GetFormatIndexFromPosition(newPosition);
            if (formatIndexFromPosition < 0)
            {
                return false;
            }
            if (forceSelection && (formatIndexFromPosition > SelectedFormatInfoIndex))
            {
                return false;
            }
            _fSelectedFormatInfoIndex = formatIndexFromPosition;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Delete()
        {
            DateTimeElementEditor currentElementEditor = GetCurrentElementEditor();
            if (currentElementEditor == null)
            {
                return false;
            }
            return currentElementEditor.Delete();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool FlushPendingEditActions()
        {
            return ApplyCurrentElementEditor();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string GetCurrentEditText()
        {
            if (CurrentValue.HasValue)
            {
                return CurrentValue.Value.ToString("G", CultureInfo.InvariantCulture);
            }
            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override object GetCurrentEditValue()
        {
            return CurrentValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="insertion"></param>
        /// <returns></returns>
        public override bool Insert(string insertion)
        {
            if ((!IsElementEdited && (insertion.Length > 3)) && !Regex.IsMatch(insertion, @"^(\d+|\p{L}+)$"))
            {
                bool flag = true;
                try
                {
                    DateTime inserted = DateTime.ParseExact(insertion, _fInitialMask, _fInitialDateTimeFormatInfo);
                    DateTime newEditValue = CorrectInsertValue(inserted);
                    if (!RaiseEditTextChanging(newEditValue))
                    {
                        return false;
                    }
                    _fUndoValue = CurrentValue;
                    _fCurrentValue = new DateTime?(newEditValue);
                    base.RaiseEditTextChanged();
                    flag = true;
                }
                catch
                {
                }
                return flag;
            }
            DateTimeElementEditor currentElementEditor = GetCurrentElementEditor();
            if (currentElementEditor == null)
            {
                return false;
            }
            if (currentElementEditor.Insert(insertion))
            {
                if (_IsOperatorMask && currentElementEditor.FinalOperatorInsert)
                {
                    base.CursorRight(false);
                }
                return true;
            }
            if (insertion == " ")
            {
                return base.CursorRight(false);
            }
            return ((((insertion.Length > 0) && ((SelectedFormatInfoIndex + 1) < FormatInfo.Count)) && (!FormatInfo[SelectedFormatInfoIndex + 1].Editable && FormatInfo[SelectedFormatInfoIndex + 1].Format(NonEmptyCurrentValue).StartsWith(insertion))) && base.CursorRight(false));
        }

        /// <summary>
        /// 
        /// </summary>
        public override void PrepareForCursorMoveAfterSelectAll()
        {
            CursorHome(false);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void PrepareForInsertAfterSelectAll()
        {
            if (DoNotClearValueOnInsertAfterSelectAll)
            {
                CursorHome(false);
            }
            else
            {
                ClearAfterSelectAll();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool SpinDown()
        {
            DateTimeElementEditor currentElementEditor = GetCurrentElementEditor();
            if (currentElementEditor == null)
            {
                return false;
            }
            return currentElementEditor.SpinDown();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool SpinUp()
        {
            DateTimeElementEditor currentElementEditor = GetCurrentElementEditor();
            if (currentElementEditor == null)
            {
                return false;
            }
            return currentElementEditor.SpinUp();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Undo()
        {
            if (IsElementEdited)
            {
                KillCurrentElementEditor();
            }
            else
            {
                if (!RaiseEditTextChanging(UndoValue))
                {
                    return false;
                }
                DateTime? currentValue = CurrentValue;
                _fCurrentValue = UndoValue;
                _fUndoValue = currentValue;
                base.RaiseEditTextChanged();
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool CanUndo
        {
            get
            {
                if (CurrentValue == UndoValue)
                {
                    return IsElementEdited;
                }
                return true;
            }
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool ApplyCurrentElementEditor()
        {
            if (!IsElementEdited)
            {
                return false;
            }
            int result = GetCurrentElementEditor().GetResult();
            KillCurrentElementEditor();
            DateTime newEditValue = SelectedElement.ApplyElement(result, NonEmptyCurrentValue);
            if (CurrentValue != newEditValue)
            {
                if (!RaiseEditTextChanging(newEditValue))
                {
                    return false;
                }
                _fUndoValue = CurrentValue;
                _fCurrentValue = new DateTime?(newEditValue);
                base.RaiseEditTextChanged();
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected DateTimeElementEditor GetCurrentElementEditor()
        {
            if (SelectedElement != null)
            {
                bool flag = base.RaiseModifyWithoutEditValueChange();
                if (!IsElementEdited && flag)
                {
                    _fCurrentElementEditor = SelectedElement.CreateElementEditor(NonEmptyCurrentValue);
                }
            }
            return _fCurrentElementEditor;
        }

        int GetFormatIndexFromPosition(int position)
        {
            int num = -1;
            for (int i = 0; i < FormatInfo.Count; i++)
            {
                if (FormatInfo[i].Editable)
                {
                    if (FormatInfo.Format(NonEmptyCurrentValue, 0, i).Length >= position)
                    {
                        return i;
                    }
                    num = i;
                }
            }
            return num;
        }

        /// <summary>
        /// 
        /// </summary>
        protected void KillCurrentElementEditor()
        {
            _fCurrentElementEditor = null;
        }

        void VerifyCache()
        {
            if (!CurrentValue.HasValue)
            {
                throw new InvalidOperationException();
            }
            if ((_cachedValue != CurrentValue.Value) || (SelectedFormatInfoIndex != _cachedIndex))
            {
                _cachedIndex = SelectedFormatInfoIndex;
                _cachedDCP = -1;
                _cachedDSA = -1;
            }
            if (_cachedValue != CurrentValue.Value)
            {
                _cachedValue = CurrentValue.Value;
                _cachedDT = null;
            }
        }
        #endregion

        #region 虚拟方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inserted"></param>
        /// <returns></returns>
        protected virtual DateTime CorrectInsertValue(DateTime inserted)
        {
            switch (FormatInfo.DateTimeParts)
            {
                case DateTimePart.Date:
                    return new DateTime(GetClearValue().TimeOfDay.Ticks + inserted.Date.Ticks);

                case DateTimePart.Time:
                    return GetClearValue().Date.AddTicks(inserted.TimeOfDay.Ticks);
            }
            return inserted;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual DateTime GetClearValue()
        {
            if (!AlwaysTodayOnClearSelectAll && _fInitialEditValue.HasValue)
            {
                switch (FormatInfo.DateTimeParts)
                {
                    case DateTimePart.Date:
                        return DateTime.Today.AddTicks(_fInitialEditValue.Value.TimeOfDay.Ticks);

                    case DateTimePart.Time:
                        return _fInitialEditValue.Value.Date;
                }
            }
            return DateTime.Today;
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class DateTimeElementEditor
    {
        #region 构造方法
        /// <summary>
        /// 
        /// </summary>
        protected DateTimeElementEditor() { }
        #endregion

        #region 属性
        /// <summary>
        /// 
        /// </summary>
        public abstract string DisplayText { get; }

        /// <summary>
        /// 
        /// </summary>
        public abstract bool FinalOperatorInsert { get; }
        #endregion

        #region 抽象方法
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract bool Delete();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract int GetResult();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inserted"></param>
        /// <returns></returns>
        public abstract bool Insert(string inserted);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract bool SpinDown();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract bool SpinUp();
        #endregion
    }
}

