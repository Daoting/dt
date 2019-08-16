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
    public class NumericMaskManager : MaskManagerStated
    {
        #region 静态内容
        static bool IsSomethingExceptDotsAndZeros(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                char ch = input[i];
                if ((ch != '0') && (ch != '.'))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region 成员变量
        /// <summary>
        /// 
        /// </summary>
        protected readonly bool _AllowNull;

        // hdt
        Type _editValueTypeCode;
        bool _hasValueType = false;
        //TypeCode? _editValueTypeCode;
        NumericFormatter[] _formatters;
        NumericMaskLogic _logic;
        readonly string _negativeSignString;
        #endregion

        #region 构造方法
        /// <summary>
        /// 数字型掩码管理者
        /// </summary>
        /// <param name="formatString">掩码表达式</param>
        /// <param name="managerCultureInfo">区域信息</param>
        /// <param name="allowNull">是否允许空</param>
        public NumericMaskManager(string formatString, CultureInfo managerCultureInfo, bool allowNull)
            : base(NumericMaskManagerState.NullInstance)
        {
            _formatters = new NumericFormatter[2];
            _AllowNull = allowNull;
            formatString = NumericFormatter.Expand(formatString, managerCultureInfo);
            _negativeSignString = managerCultureInfo.NumberFormat.NegativeSign;
            int index = formatString.Replace(@"\\", "//").Replace(@"\;", "/:").IndexOf(';');
            if (index < 0)
            {
                _formatters[0] = new NumericFormatter(formatString, managerCultureInfo);
                _formatters[1] = null;
            }
            else
            {
                _formatters[0] = new NumericFormatter(formatString.Substring(0, index), managerCultureInfo);
                _formatters[1] = new NumericFormatter(formatString.Substring(index + 1), managerCultureInfo);
                if (_formatters[0].MaxDigitsBeforeDecimalSeparator != _formatters[1].MaxDigitsBeforeDecimalSeparator)
                {
                    throw new ArgumentException("Incorrect mask: the max number of digits before the decimal separator in the positive and negative patterns must match");
                }
                if (_formatters[0].MaxDigitsAfterDecimalSeparator != _formatters[1].MaxDigitsAfterDecimalSeparator)
                {
                    throw new ArgumentException("Incorrect mask: the max number of digits after the decimal separator in the positive and negative patterns must match");
                }
                if (_formatters[0].MinDigitsBeforeDecimalSeparator != _formatters[1].MinDigitsBeforeDecimalSeparator)
                {
                    throw new ArgumentException("Incorrect mask: the min number of digits before the decimal separator in the positive and negative patterns must match");
                }
                if (_formatters[0].MinDigitsAfterDecimalSeparator != _formatters[1].MinDigitsAfterDecimalSeparator)
                {
                    throw new ArgumentException("Incorrect mask: the min number of digits after the decimal separator in the positive and negative patterns must match");
                }
                if (_formatters[0]._Is100Multiplied != _formatters[1]._Is100Multiplied)
                {
                    throw new ArgumentException("Incorrect mask: the percent type (% or %%) in the positive and negative patterns must match");
                }
            }
            _logic = new NumericMaskLogic(_formatters[0].MaxDigitsBeforeDecimalSeparator, _formatters[0].MinDigitsBeforeDecimalSeparator, _formatters[0].MinDigitsAfterDecimalSeparator, _formatters[0].MaxDigitsAfterDecimalSeparator, managerCultureInfo);
            if ((_formatters[0].MaxDigitsAfterDecimalSeparator > 0) || _formatters[0]._Is100Multiplied)
            {
                _editValueTypeCode = typeof(Decimal);
            }
        }
        #endregion

        #region 属性
        /// <summary>
        /// 
        /// </summary>
        protected new NumericMaskManagerState CurrentState
        {
            get { return (NumericMaskManagerState)base.CurrentState; }
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
        public override bool IsFinal
        {
            get
            {
                if (CurrentState.IsNull)
                {
                    return false;
                }
                if (CurrentState.EditText.Length != CurrentState.CursorPosition)
                {
                    return false;
                }
                int index = CurrentState.EditText.IndexOf('.');
                if (index >= 0)
                {
                    return (_formatters[0].MaxDigitsAfterDecimalSeparator == ((CurrentState.EditText.Length - index) - 1));
                }
                return ((_formatters[0].MaxDigitsAfterDecimalSeparator == 0) && (_formatters[0].MaxDigitsBeforeDecimalSeparator == CurrentState.EditText.Length));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool IsPlainTextLike
        {
            get { return true; }
        }

        bool IsSignedMask
        {
            get { return (_formatters[1] != null); }
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 设置初始值
        /// </summary>
        /// <param name="initialEditValue"></param>
        public override void SetInitialEditValue(object initialEditValue)
        {
            if (initialEditValue == null)
            {
                SetInitialEditText(null);
            }
            else
            {
                // hdt
                ForceEditValueTypeCode(initialEditValue.GetType());
                int maxDigitsAfterDecimalSeparator = _formatters[0].MaxDigitsAfterDecimalSeparator;
                if (_formatters[0]._Is100Multiplied)
                {
                    maxDigitsAfterDecimalSeparator += 2;
                }
                string format = "{0:f" + maxDigitsAfterDecimalSeparator.ToString(CultureInfo.InvariantCulture) + "}";
                string input = string.Format(CultureInfo.InvariantCulture, format, new object[] { initialEditValue });
                if (_formatters[0]._Is100Multiplied)
                {
                    input = NumericMaskLogic.Mul100(input);
                }
                SetInitialEditText(input);
            }
        }

        /// <summary>
        /// 设置初始文本
        /// </summary>
        /// <param name="initialEditText"></param>
        public override void SetInitialEditText(string initialEditText)
        {
            if ((initialEditText == null) || (_AllowNull && (initialEditText.Length == 0)))
            {
                base.SetInitialState(NumericMaskManagerState.NullInstance);
            }
            else
            {
                MaskLogicResult result;
                string testedString = initialEditText.Trim();
                bool isNegative = false;
                if (testedString.StartsWith("-"))
                {
                    testedString = testedString.Substring(1);
                    if (IsSignedMask)
                    {
                        isNegative = true;
                    }
                }
                if (IsValidInvariantCultureDecimal(testedString))
                {
                    result = _logic.GetEditResult(testedString, string.Empty, string.Empty, string.Empty);
                    if (result != null)
                    {
                        result = _logic.GetEditResult(string.Empty, string.Empty, result.EditText, string.Empty);
                    }
                }
                else
                {
                    result = _logic.GetEditResult(string.Empty, string.Empty, string.Empty, initialEditText);
                }
                if (result == null)
                {
                    result = _logic.GetEditResult(string.Empty, string.Empty, string.Empty, string.Empty);
                }
                int index = result.EditText.IndexOf('.');
                if (index < 0)
                {
                    index = result.EditText.Length;
                }
                base.SetInitialState(new NumericMaskManagerState(result.EditText, index, index, isNegative));
            }
        }

        /// <summary>
        /// 回退处理
        /// </summary>
        /// <returns></returns>
        public override bool Backspace()
        {
            MaskLogicResult result;
            if (CurrentState.IsNull)
            {
                return false;
            }
            if (base.IsSelection)
            {
                if (_AllowNull)
                {
                    int num = Math.Min(CurrentState.SelectionAnchor, CurrentState.CursorPosition);
                    int num2 = Math.Max(CurrentState.SelectionAnchor, CurrentState.CursorPosition);
                    if ((num == 0) && (num2 == CurrentState.EditText.Length))
                    {
                        return base.Apply(NumericMaskManagerState.NullInstance, MaskManagerStated.StateChangeType.Delete);
                    }
                }
                return Insert(string.Empty);
            }
            if (CurrentState.CursorPosition <= 0)
            {
                result = _logic.GetEditResult(string.Empty, string.Empty, CurrentState.EditText, string.Empty);
            }
            else
            {
                result = _logic.GetEditResult(CurrentState.EditText.Substring(0, CurrentState.CursorPosition - 1), CurrentState.EditText.Substring(CurrentState.CursorPosition - 1, 1), CurrentState.EditText.Substring(CurrentState.CursorPosition), string.Empty);
            }
            if (result == null)
            {
                return base.CursorLeft(false);
            }
            if (base.Apply(new NumericMaskManagerState(result.EditText, result.CursorPosition, result.CursorPosition, CurrentState.IsNegative), MaskManagerStated.StateChangeType.Delete))
            {
                return true;
            }
            if (CurrentState.IsNegative)
            {
                return base.Apply(new NumericMaskManagerState(result.EditText, result.CursorPosition, result.CursorPosition, false), MaskManagerStated.StateChangeType.Delete);
            }
            return ((_AllowNull && !IsSomethingExceptDotsAndZeros(result.EditText)) && base.Apply(NumericMaskManagerState.NullInstance, MaskManagerStated.StateChangeType.Delete));
        }

        /// <summary>
        /// 
        /// </summary>
        public override void ClearAfterSelectAll()
        {
            if (_AllowNull)
            {
                base.Apply(NumericMaskManagerState.NullInstance, MaskManagerStated.StateChangeType.Delete);
            }
            else
            {
                MaskLogicResult result = _logic.GetEditResult(string.Empty, CurrentState.EditText, string.Empty, string.Empty);
                base.Apply(new NumericMaskManagerState(result.EditText, result.CursorPosition, result.CursorPosition, false), MaskManagerStated.StateChangeType.Delete);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="forceSelection"></param>
        /// <returns></returns>
        public override bool CursorEnd(bool forceSelection)
        {
            return StateCursorPositionTo(CurrentState.EditText.Length, forceSelection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="forceSelection"></param>
        /// <returns></returns>
        public override bool CursorHome(bool forceSelection)
        {
            return StateCursorPositionTo(0, forceSelection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="forceSelection"></param>
        /// <param name="isNeededKeyCheck"></param>
        /// <returns></returns>
        public override bool CursorLeft(bool forceSelection, bool isNeededKeyCheck)
        {
            return StateCursorPositionTo(CurrentState.CursorPosition - 1, forceSelection, isNeededKeyCheck);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="forceSelection"></param>
        /// <param name="isNeededKeyCheck"></param>
        /// <returns></returns>
        public override bool CursorRight(bool forceSelection, bool isNeededKeyCheck)
        {
            return StateCursorPositionTo(CurrentState.CursorPosition + 1, forceSelection, isNeededKeyCheck);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newPosition"></param>
        /// <param name="forceSelection"></param>
        /// <returns></returns>
        public override bool CursorToDisplayPosition(int newPosition, bool forceSelection)
        {
            return StateCursorPositionTo(GetFormatter(CurrentState).GetPositionSource(CurrentState.EditText, newPosition), forceSelection);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Delete()
        {
            MaskLogicResult result;
            if (CurrentState.IsNull)
            {
                return false;
            }
            if (base.IsSelection)
            {
                if (_AllowNull)
                {
                    int num = Math.Min(CurrentState.SelectionAnchor, CurrentState.CursorPosition);
                    int num2 = Math.Max(CurrentState.SelectionAnchor, CurrentState.CursorPosition);
                    if ((num == 0) && (num2 == CurrentState.EditText.Length))
                    {
                        return base.Apply(NumericMaskManagerState.NullInstance, MaskManagerStated.StateChangeType.Delete);
                    }
                }
                return Insert(string.Empty);
            }
            if (CurrentState.CursorPosition >= CurrentState.EditText.Length)
            {
                result = _logic.GetEditResult(CurrentState.EditText, string.Empty, string.Empty, string.Empty);
            }
            else
            {
                result = _logic.GetEditResult(CurrentState.EditText.Substring(0, CurrentState.CursorPosition), CurrentState.EditText.Substring(CurrentState.CursorPosition, 1), CurrentState.EditText.Substring(CurrentState.CursorPosition + 1), string.Empty);
            }
            if (result == null)
            {
                return base.CursorRight(false);
            }
            if (base.Apply(new NumericMaskManagerState(result.EditText, result.CursorPosition, result.CursorPosition, CurrentState.IsNegative), MaskManagerStated.StateChangeType.Delete))
            {
                return true;
            }
            if (CurrentState.IsNegative)
            {
                return base.Apply(new NumericMaskManagerState(result.EditText, result.CursorPosition, result.CursorPosition, false), MaskManagerStated.StateChangeType.Delete);
            }
            return ((_AllowNull && !IsSomethingExceptDotsAndZeros(result.EditText)) && base.Apply(NumericMaskManagerState.NullInstance, MaskManagerStated.StateChangeType.Delete));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="insertion"></param>
        /// <returns></returns>
        public override bool Insert(string insertion)
        {
            if (IsSignedMask && ((insertion == "-") || (insertion == _negativeSignString)))
            {
                if (!CurrentState.IsNull)
                {
                    return base.Apply(new NumericMaskManagerState(CurrentState.EditText, CurrentState.CursorPosition, CurrentState.SelectionAnchor, !CurrentState.IsNegative), MaskManagerStated.StateChangeType.Insert);
                }
                MaskLogicResult result = _logic.GetEditResult(string.Empty, string.Empty, string.Empty, string.Empty);
                if (result == null)
                {
                    return false;
                }
                return base.Apply(new NumericMaskManagerState(result.EditText, 0, result.EditText.Length, true), MaskManagerStated.StateChangeType.Insert);
            }
            int length = (CurrentState.CursorPosition < CurrentState.SelectionAnchor) ? CurrentState.CursorPosition : CurrentState.SelectionAnchor;
            int startIndex = (CurrentState.CursorPosition < CurrentState.SelectionAnchor) ? CurrentState.SelectionAnchor : CurrentState.CursorPosition;
            MaskLogicResult result2 = _logic.GetEditResult(CurrentState.EditText.Substring(0, length), CurrentState.EditText.Substring(length, startIndex - length), CurrentState.EditText.Substring(startIndex), insertion);
            if (result2 == null)
            {
                return false;
            }
            bool isNegative = CurrentState.IsNegative;
            if (IsSignedMask && ((insertion.IndexOf("-") >= 0) || (insertion.IndexOf(_negativeSignString) >= 0)))
            {
                isNegative = !isNegative;
            }
            return base.Apply(new NumericMaskManagerState(result2.EditText, result2.CursorPosition, result2.CursorPosition, isNegative), MaskManagerStated.StateChangeType.Insert);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void PrepareForInsertAfterSelectAll()
        {
            if (_AllowNull || CurrentState.IsNull)
            {
                base.Apply(NumericMaskManagerState.NullInstance, MaskManagerStated.StateChangeType.Insert);
            }
            else
            {
                base.Apply(new NumericMaskManagerState(CurrentState.EditText, CurrentState.EditText.Length, 0, false), MaskManagerStated.StateChangeType.Insert);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool SpinDown()
        {
            return SpinKeys(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool SpinUp()
        {
            return SpinKeys(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected override int GetCursorPosition(MaskManagerState state)
        {
            NumericMaskManagerState state2 = (NumericMaskManagerState)state;
            if (state2.IsNull)
            {
                return 0;
            }
            return GetFormatter(state2).GetPositionFormatted(state2.EditText, state2.CursorPosition);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected override string GetDisplayText(MaskManagerState state)
        {
            NumericMaskManagerState state2 = (NumericMaskManagerState)state;
            if (state2.IsNull)
            {
                return string.Empty;
            }
            return GetFormatter(state2).Format(state2.EditText);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected override string GetEditText(MaskManagerState state)
        {
            NumericMaskManagerState state2 = (NumericMaskManagerState)state;
            if (state2.IsNegative)
            {
                return ('-' + state2.EditText);
            }
            return state2.EditText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected override object GetEditValue(MaskManagerState state)
        {
            if (((NumericMaskManagerState)state).IsNull)
            {
                return null;
            }
            string editText = GetEditText(state);
            if (_formatters[0]._Is100Multiplied)
            {
                editText = NumericMaskLogic.Div100(editText);
            }
            if (editText.IndexOf('.') >= 0)
            {
                while (editText.EndsWith("0"))
                {
                    editText = editText.Substring(0, editText.Length - 1);
                }
            }
            if (editText.EndsWith("."))
            {
                editText = editText.Substring(0, editText.Length - 1);
            }
            if ((editText.Length == 0) || (editText == "-"))
            {
                editText = editText + '0';
            }
            if (_hasValueType)
            {
                try
                {
                    return Convert.ChangeType(editText, _editValueTypeCode);
                }
                catch
                {
                    goto Label_00E6;
                }
            }
            try
            {
                return Convert.ChangeType(editText, typeof(Int32));
            }
            catch
            {
            }
            try
            {
                return Convert.ChangeType(editText, typeof(Decimal));
            }
            catch
            {
            }
        Label_00E6:
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected override int GetSelectionAnchor(MaskManagerState state)
        {
            NumericMaskManagerState state2 = (NumericMaskManagerState)state;
            if (state2.IsNull)
            {
                return 0;
            }
            return GetFormatter(state2).GetPositionFormatted(state2.EditText, state2.SelectionAnchor);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newState"></param>
        /// <returns></returns>
        protected override bool IsValid(MaskManagerState newState)
        {
            NumericMaskManagerState state = newState as NumericMaskManagerState;
            if (state == null)
            {
                return false;
            }
            if (!state.IsNull && (GetEditValue(state) == null))
            {
                return false;
            }
            return true;
        }
        #endregion

        #region 内部方法
        bool ForceEditValueTypeCode(Type p_type)
        {
            // hdt
            if (p_type == typeof(byte)
                || p_type == typeof(sbyte)
                || p_type == typeof(Int16)
                || p_type == typeof(UInt16)
                || p_type == typeof(Int32)
                || p_type == typeof(UInt32)
                || p_type == typeof(Int64)
                || p_type == typeof(UInt64)
                || p_type == typeof(Single)
                || p_type == typeof(Double)
                || p_type == typeof(Decimal))
            {
                _editValueTypeCode = p_type;
            }
            else
            {
                _editValueTypeCode = typeof(string);
            }
            _hasValueType = true;
            return true;

            //switch (forcedCode)
            //{
            //    case TypeCode.SByte:
            //    case TypeCode.Byte:
            //    case TypeCode.Int16:
            //    case TypeCode.UInt16:
            //    case TypeCode.Int32:
            //    case TypeCode.UInt32:
            //    case TypeCode.Int64:
            //    case TypeCode.UInt64:
            //    case TypeCode.Single:
            //    case TypeCode.Double:
            //    case TypeCode.Decimal:
            //        _editValueTypeCode = new TypeCode?(forcedCode);
            //        return true;
            //}
            //_editValueTypeCode = TypeCode.String;
            //return true;
        }

        NumericFormatter GetFormatter(NumericMaskManagerState state)
        {
            return _formatters[state.IsNegative ? 1 : 0];
        }

        bool IsValidInvariantCultureDecimal(string testedString)
        {
            int index = testedString.IndexOf('.');
            for (int i = 0; i < testedString.Length; i++)
            {
                if (i != index)
                {
                    char ch = testedString[i];
                    if ((ch < '0') || (ch > '9'))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        bool SpinKeys(bool isUp)
        {
            bool flag;
            int cursorPosition = CurrentState.CursorPosition;
            if (CurrentState.SelectionAnchor != CurrentState.CursorPosition)
            {
                int startIndex = (CurrentState.SelectionAnchor < CurrentState.CursorPosition) ? CurrentState.SelectionAnchor : CurrentState.CursorPosition;
                int num3 = (CurrentState.SelectionAnchor < CurrentState.CursorPosition) ? CurrentState.CursorPosition : CurrentState.SelectionAnchor;
                if (((startIndex == 0) && (num3 == CurrentState.EditText.Length)) || (CurrentState.EditText.Substring(startIndex, num3 - startIndex).IndexOf('.') >= 0))
                {
                    cursorPosition = CurrentState.EditText.IndexOf('.');
                    if (cursorPosition < 0)
                    {
                        cursorPosition = CurrentState.EditText.Length;
                    }
                }
                else
                {
                    cursorPosition = num3;
                }
            }
            bool isModuloDecrement = isUp ? CurrentState.IsNegative : !CurrentState.IsNegative;
            MaskLogicResult result = _logic.GetSpinResult(CurrentState.EditText.Substring(0, cursorPosition), CurrentState.EditText.Substring(cursorPosition), isModuloDecrement, IsSignedMask, out flag);
            if (result == null)
            {
                return false;
            }
            bool isNegative = flag ? !CurrentState.IsNegative : CurrentState.IsNegative;
            return base.Apply(new NumericMaskManagerState(result.EditText, result.CursorPosition, result.CursorPosition, isNegative), MaskManagerStated.StateChangeType.Insert);
        }

        bool StateCursorPositionTo(int newPosition, bool forceSelection)
        {
            return StateCursorPositionTo(newPosition, forceSelection, false);
        }

        bool StateCursorPositionTo(int newPosition, bool forceSelection, bool isNeededKeyCheck)
        {
            if (CurrentState.IsNull)
            {
                return false;
            }
            if (newPosition < 0)
            {
                newPosition = 0;
            }
            else if (newPosition > CurrentState.EditText.Length)
            {
                newPosition = CurrentState.EditText.Length;
            }
            return base.Apply(new NumericMaskManagerState(CurrentState.EditText, newPosition, forceSelection ? CurrentState.SelectionAnchor : newPosition, CurrentState.IsNegative), MaskManagerStated.StateChangeType.Terminator, isNeededKeyCheck);
        }
        #endregion
    }

    public enum TypeCode
    {
        // 摘要:
        //     空引用。
        Empty = 0,
        //
        // 摘要:
        //     常规类型，表示不会由另一个 TypeCode 显式表示的任何引用或值类型。
        Object = 1,
        //
        // 摘要:
        //     数据库空（列）值。
        DBNull = 2,
        //
        // 摘要:
        //     简单类型，表示 true 或 false 的布尔值。
        Boolean = 3,
        //
        // 摘要:
        //     整型，表示值介于 0 到 65535 之间的无符号 16 位整数。System.TypeCode.Char 类型的可能值集与 Unicode 字符集相对应。
        Char = 4,
        //
        // 摘要:
        //     整型，表示值介于 -128 到 127 之间的有符号 8 位整数。
        SByte = 5,
        //
        // 摘要:
        //     整型，表示值介于 0 到 255 之间的无符号 8 位整数。
        Byte = 6,
        //
        // 摘要:
        //     整型，表示值介于 -32768 到 32767 之间的有符号 16 位整数。
        Int16 = 7,
        //
        // 摘要:
        //     整型，表示值介于 0 到 65535 之间的无符号 16 位整数。
        UInt16 = 8,
        //
        // 摘要:
        //     整型，表示值介于 -2147483648 到 2147483647 之间的有符号 32 位整数。
        Int32 = 9,
        //
        // 摘要:
        //     整型，表示值介于 0 到 4294967295 之间的无符号 32 位整数。
        UInt32 = 10,
        //
        // 摘要:
        //     整型，表示值介于 -9223372036854775808 到 9223372036854775807 之间的有符号 64 位整数。
        Int64 = 11,
        //
        // 摘要:
        //     整型，表示值介于 0 到 18446744073709551615 之间的无符号 64 位整数。
        UInt64 = 12,
        //
        // 摘要:
        //     浮点型，表示从大约 1.5 x 10 -45 到 3.4 x 10 38 且精度为 7 位的值。
        Single = 13,
        //
        // 摘要:
        //     浮点型，表示从大约 5.0 x 10 -324 到 1.7 x 10 308 且精度为 15 到 16 位的值。
        Double = 14,
        //
        // 摘要:
        //     简单类型，表示从 1.0 x 10 -28 到大约 7.9 x 10 28 且有效位数为 28 到 29 位的值。
        Decimal = 15,
        //
        // 摘要:
        //     表示一个日期和时间值的类型。
        DateTime = 16,
        //
        // 摘要:
        //     密封类类型，表示 Unicode 字符串。
        String = 18,
    }
}

