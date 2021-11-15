#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using System;
using System.Globalization;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 数值格
    /// </summary>
    public partial class CNum : FvCell
    {
        #region 静态内容
        /// <summary>
        /// 当前值
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(double),
            typeof(CNum),
            new PropertyMetadata(0.0, OnValuePropertyChanged));

        /// <summary>
        /// 是否为整型数值
        /// </summary>
        public static readonly DependencyProperty IsIntegerProperty = DependencyProperty.Register(
            "IsInteger",
            typeof(bool),
            typeof(CNum),
            new PropertyMetadata(false, OnIsIntegerChanged));

        /// <summary>
        /// 小数位数
        /// </summary>
        public static readonly DependencyProperty DecimalsProperty = DependencyProperty.Register(
            "Decimals",
            typeof(int),
            typeof(CNum),
            new PropertyMetadata(-1, OnUpdateText));

        /// <summary>
        /// 最大值
        /// </summary>
        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
            "Maximum",
            typeof(double),
            typeof(CNum),
            new PropertyMetadata(double.MaxValue, OnMaximumPropertyChanged));

        /// <summary>
        /// 最小值
        /// </summary>
        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
            "Minimum",
            typeof(double),
            typeof(CNum),
            new PropertyMetadata(double.MinValue, OnMinimumPropertyChanged));

        /// <summary>
        /// 格式化显示
        /// </summary>
        public static readonly DependencyProperty ValueFormatProperty = DependencyProperty.Register(
            "ValueFormat",
            typeof(ValueFormat),
            typeof(CNum),
            new PropertyMetadata(ValueFormat.Numeric, OnUpdateText));

        /// <summary>
        /// 是否实时更新Cell值
        /// </summary>
        public static readonly DependencyProperty UpdateTimelyProperty = DependencyProperty.Register(
            "UpdateTimely",
            typeof(bool),
            typeof(CNum),
            new PropertyMetadata(false));

        /// <summary>
        /// 自定义单位
        /// </summary>
        public static readonly DependencyProperty CustomUnitProperty = DependencyProperty.Register(
            "CustomUnit",
            typeof(string),
            typeof(CNum),
            new PropertyMetadata(null, OnUpdateText));

        /// <summary>
        /// 是否可为空
        /// </summary>
        public static readonly DependencyProperty NullValueProperty = DependencyProperty.Register(
            "NullValue",
            typeof(string),
            typeof(CNum),
            new PropertyMetadata("", OnUpdateText));

        /// <summary>
        /// 最大变化量
        /// </summary>
        public static readonly DependencyProperty LargeChangeProperty = DependencyProperty.Register(
            "LargeChange",
            typeof(double),
            typeof(CNum),
            new PropertyMetadata(10.0, OnLargeChangePropertyChanged));

        /// <summary>
        /// 最小变化量
        /// </summary>
        public static readonly DependencyProperty SmallChangeProperty = DependencyProperty.Register(
            "SmallChange",
            typeof(double),
            typeof(CNum),
            new PropertyMetadata(1.0, OnSmallChangePropertyChanged));

        /// <summary>
        /// 自动循环调值
        /// </summary>
        public static readonly DependencyProperty AutoReverseProperty = DependencyProperty.Register(
            "AutoReverse",
            typeof(bool),
            typeof(CNum),
            null);

        /// <summary>
        /// 区域格式
        /// </summary>
        public static readonly DependencyProperty NumberFormatInfoProperty = DependencyProperty.Register(
            "NumberFormatInfo",
            typeof(NumberFormatInfo),
            typeof(CNum),
            new PropertyMetadata(null, OnUpdateText));

        /// <summary>
        /// 当前控件是否已获得焦点
        /// </summary>
        public static readonly DependencyProperty IsFocusedProperty = DependencyProperty.Register(
            "IsFocused",
            typeof(bool),
            typeof(CNum),
            new PropertyMetadata(false));

        static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CNum c = (CNum)d;
            double val = (double)e.NewValue;
            if (double.IsInfinity(val))
                throw new ArgumentException("无效值");

            if (c._levelsFromRootCall == 0)
            {
                c._requestedVal = val;
                c._initialVal = (double)e.OldValue;
            }
            c._levelsFromRootCall++;
            c.CoerceValue();
            c._levelsFromRootCall--;
            if (c._levelsFromRootCall == 0)
            {
                if (!AreClose(c._initialVal, val))
                {
                    c.OnValueChanged(new ValueChangedEventArgs<double>(c._initialVal, val));
                }
            }
        }

        static void OnMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CNum c = (CNum)d;
            if (!IsValidDoubleValue((double)e.NewValue))
            {
                throw new ArgumentException("无效最大值");
            }
            if (c._levelsFromRootCall == 0)
            {
                c._requestedMax = (double)e.NewValue;
                c._initialMax = (double)e.OldValue;
                c._initialVal = c.Value;
            }
            c._levelsFromRootCall++;
            c.CoerceMaximum();
            c.CoerceValue();
            c._levelsFromRootCall--;
            if (c._levelsFromRootCall == 0)
            {
                // UI 自动化
            }
        }

        static void OnMinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CNum c = (CNum)d;
            if (!IsValidDoubleValue((double)e.NewValue))
            {
                throw new ArgumentException("无效最小值");
            }
            if (c._levelsFromRootCall == 0)
            {
                c._initialMax = c.Maximum;
                c._initialVal = c.Value;
            }
            c._levelsFromRootCall++;
            c.CoerceMaximum();
            c.CoerceValue();
            c._levelsFromRootCall--;
            if (c._levelsFromRootCall == 0)
            {
                // UI 自动化
            }
        }

        static void OnLargeChangePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!IsValidChange(e.NewValue))
                throw new ArgumentException("无效的最大变化值");
        }

        static void OnSmallChangePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!IsValidChange(e.NewValue))
                throw new ArgumentException("无效的最小变化值");
        }

        static void OnIsIntegerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CNum c = (CNum)d;
            bool isInteger = (bool)e.NewValue;
            switch (c.ValueFormat)
            {
                case ValueFormat.Numeric:
                    c.NumberFormatInfo.NumberDecimalDigits = isInteger ? 0 : CultureInfo.CurrentCulture.NumberFormat.NumberDecimalDigits;
                    break;

                case ValueFormat.Currency:
                    c.NumberFormatInfo.CurrencyDecimalDigits = isInteger ? 0 : CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalDigits;
                    break;

                case ValueFormat.Percentage:
                    c.NumberFormatInfo.PercentDecimalDigits = isInteger ? 0 : CultureInfo.CurrentCulture.NumberFormat.PercentDecimalDigits;
                    break;
            }
            c.UpdateText();
        }

        static void OnUpdateText(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CNum)d).UpdateText();
        }

        public static bool AreClose(double value1, double value2)
        {
            if (value1 == value2)
            {
                return true;
            }
            double num = ((Math.Abs(value1) + Math.Abs(value2)) + 10.0) * 1.1102230246251568E-16;
            double num2 = value1 - value2;
            return ((-num < num2) && (num > num2));
        }

        static bool IsValidChange(object value)
        {
            double val = (double)value;
            return (IsValidDoubleValue(val) && (val >= 0.0));
        }

        static bool IsValidDoubleValue(double value)
        {
            return (!double.IsNaN(value) && !double.IsInfinity(value));
        }
        #endregion

        #region 成员变量
        double _initialMax;
        double _initialVal;
        int _levelsFromRootCall;
        double _requestedMax = double.MaxValue;
        double _requestedVal;
        TextBox _textBox;
        double _inputValue = 0.0;
        bool _isChanging;
        bool _isSpinning;
        bool _updatingContent;
        string _lastInput = "";
        #endregion

        #region 构造方法
        public CNum()
        {
            DefaultStyleKey = typeof(CNum);
            NumberFormatInfo = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
            ValConverter = new NumValConverter();
        }
        #endregion

        #region 事件
        /// <summary>
        /// 数值变化事件
        /// </summary>
        public event EventHandler<ValueChangedEventArgs<double>> ValueChanged;
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置当前值
        /// </summary>
        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// 获取设置是否为整型数值
        /// </summary>
        [CellParam("是否为整数")]
        public bool IsInteger
        {
            get { return (bool)GetValue(IsIntegerProperty); }
            set { SetValue(IsIntegerProperty, value); }
        }

        /// <summary>
        /// 获取设置小数位数
        /// </summary>
        [CellParam("小数位数")]
        public int Decimals
        {
            get { return (int)GetValue(DecimalsProperty); }
            set { SetValue(DecimalsProperty, value); }
        }

        /// <summary>
        /// 获取设置最大值
        /// </summary>
        [CellParam("最大值")]
        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        /// <summary>
        /// 获取设置最小值
        /// </summary>
        [CellParam("最小值")]
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        /// <summary>
        /// 获取设置格式化显示
        /// </summary>
        public ValueFormat ValueFormat
        {
            get { return (ValueFormat)GetValue(ValueFormatProperty); }
            set { SetValue(ValueFormatProperty, value); }
        }

        /// <summary>
        /// 获取设置是否实时更新Cell值
        /// </summary>
        [CellParam("实时更新值")]
        public bool UpdateTimely
        {
            get { return (bool)GetValue(UpdateTimelyProperty); }
            set { SetValue(UpdateTimelyProperty, value); }
        }

        /// <summary>
        /// 获取设置自定义单位，显示在数字后面
        /// </summary>
        [CellParam("自定义单位")]
        public string CustomUnit
        {
            get { return (string)GetValue(CustomUnitProperty); }
            set { SetValue(CustomUnitProperty, value); }
        }

        /// <summary>
        /// 获取设置为空时显示的字符串
        /// </summary>
        [CellParam("为空时的串")]
        public string NullValue
        {
            get { return (string)GetValue(NullValueProperty); }
            set { SetValue(NullValueProperty, value); }
        }

        /// <summary>
        /// 获取设置最大变化量
        /// </summary>
        [CellParam("最大变化量")]
        public double LargeChange
        {
            get { return (double)GetValue(LargeChangeProperty); }
            set { SetValue(LargeChangeProperty, value); }
        }

        /// <summary>
        /// 获取设置最小变化量
        /// </summary>
        [CellParam("最小变化量")]
        public double SmallChange
        {
            get { return (double)GetValue(SmallChangeProperty); }
            set { SetValue(SmallChangeProperty, value); }
        }

        /// <summary>
        /// 获取设置是否可自动循环调值
        /// </summary>
        [CellParam("自动循环调值")]
        public bool AutoReverse
        {
            get { return (bool)GetValue(AutoReverseProperty); }
            set { SetValue(AutoReverseProperty, value); }
        }

        /// <summary>
        /// 获取设置当前控件是否已获得焦点
        /// </summary>
#if ANDROID
        new
#endif
        public bool IsFocused
        {
            get { return (bool)GetValue(IsFocusedProperty); }
            internal set { SetValue(IsFocusedProperty, value); }
        }

        /// <summary>
        /// 获取设置区域格式
        /// </summary>
        public NumberFormatInfo NumberFormatInfo
        {
            get { return (NumberFormatInfo)GetValue(NumberFormatInfoProperty); }
            set { SetValue(NumberFormatInfoProperty, value); }
        }

        /// <summary>
        /// 获取文本框的文本内容
        /// </summary>
        public string ContentText
        {
            get
            {
                if (_textBox != null)
                    return _textBox.Text;
                return FormatDisplay();
            }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 选择文本内容
        /// </summary>
        /// <param name="start">选择区域的开始位置</param>
        /// <param name="length">选择长度</param>
        public void Select(int start, int length)
        {
            if (_textBox != null)
                _textBox.Select(start, length);
        }

        /// <summary>
        /// 全选
        /// </summary>
        public void SelectAll()
        {
            if (_textBox != null)
                _textBox.SelectAll();
        }
        #endregion

        #region 重写方法
        protected override void OnApplyCellTemplate()
        {
            _textBox = (TextBox)GetTemplateChild("TextBox");

            // iOS android若设置 Number 键盘无小数点，全角才有，windows上全角无法输入！
            _textBox.InputScope = new InputScope
            {
                Names = { new InputScopeName
                { 
#if UWP
                    NameValue = InputScopeNameValue.Number,
#else
                    NameValue = InputScopeNameValue.NumberFullWidth,
#endif
                } }
            };

            _textBox.TextChanged += OnTextBoxTextChanged;
            UpdateText();
        }

        protected override void SetValBinding()
        {
            SetBinding(ValueProperty, ValBinding);
        }

        protected override bool SetFocus()
        {
            if (_textBox.Focus(FocusState.Programmatic))
            {
                _textBox.SelectAll();
                return true;
            }
            return false;
        }

        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            base.OnKeyDown(e);
            if (IsReadOnly)
                return;

            switch (e.Key)
            {
                case VirtualKey.PageUp:
                    ChangeValue(LargeChange);
                    e.Handled = true;
                    break;

                case VirtualKey.PageDown:
                    ChangeValue(-LargeChange);
                    e.Handled = true;
                    break;

                case VirtualKey.Enter:
                    UpdateValue();
                    string value = FormatEdit();
                    if (_textBox != null && _textBox.Text != value)
                    {
                        int start = _textBox.SelectionStart;
                        _lastInput = value;
                        _textBox.Text = value;
                        _textBox.Select(start, 0);
                    }
                    break;

                case VirtualKey.Up:
                    ChangeValue(SmallChange);
                    e.Handled = true;
                    break;

                case VirtualKey.Down:
                    ChangeValue(-SmallChange);
                    e.Handled = true;
                    break;
                default:
                    break;
            }
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            IsFocused = true;
            if (_textBox != null)
                _textBox.Focus(FocusState.Programmatic);
            UpdateText();
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);

            if ((FocusManager.GetFocusedElement() == null)
                || !this.IsAncestorOf((FocusManager.GetFocusedElement() as FrameworkElement)))
            {
                IsFocused = false;
            }
            UpdateValue();
            UpdateText();
        }

        protected override void OnPointerWheelChanged(PointerRoutedEventArgs e)
        {
            base.OnPointerWheelChanged(e);
            HandleMouseWheel(e.GetCurrentPoint(this).Properties.MouseWheelDelta);
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} Minimum:{1} Maximum:{2} Value:{3}", new object[] { ToString(), Minimum, Maximum, Value });
        }
        #endregion

        #region 内部方法
        void OnValueChanged(ValueChangedEventArgs<double> e)
        {
            if (!_isChanging)
            {
                if (_inputValue != e.NewValue)
                    _inputValue = e.NewValue;
                ValueChanged?.Invoke(this, e);

                if (!IsFocused || _isSpinning)
                    UpdateText();

                if (!double.IsNaN(e.NewValue))
                {
                    string[] str = e.NewValue.ToString(CultureInfo.InvariantCulture).Split(new char[] { '.' });
                    int decimals = GetNumberDecimalDigits();
                    if ((str.Length > 1) && (str[1].Length > decimals))
                    {
                        _isChanging = true;
                        Value = Math.Round(Value, decimals);
                        _isChanging = false;
                    }
                }
            }
        }

        void ChangeValue(double delta)
        {
            _isSpinning = true;
            UpdateValue();

            if (double.IsNaN(Value))
            {
                Value = 0.0;
            }
            else
            {
                if (AutoReverse)
                {
                    if (Value == Maximum && delta > 0.0)
                    {
                        Value = Minimum;
                        return;
                    }

                    if (Value == Minimum && delta < 0.0)
                    {
                        Value = Maximum;
                        return;
                    }
                }
                Value = Value + delta;
            }

            UpdateText();
        }

        void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_updatingContent)
            {
                _updatingContent = false;
            }
            else if (IsReadOnly && !_isSpinning)
            {
                _updatingContent = true;
                ReturnPreviousInput();
            }
            else
            {
                if (IsFocusWithin())
                {
                    if (string.IsNullOrEmpty(_textBox.Text))
                    {
                        _inputValue = double.NaN;
                        _lastInput = "";
                    }
                    else
                    {
                        double parsedValue = 0.0;
                        string text = _textBox.Text;
                        bool endsWithDecimalSeparator = text.EndsWith(NumberFormatInfo.NumberDecimalSeparator);
                        if ((((AllowDecimalSeparator && endsWithDecimalSeparator) || !endsWithDecimalSeparator) && double.TryParse(text, NumberStyles.Any, (IFormatProvider)NumberFormatInfo, out parsedValue)) || IsSymbolicException(text, NumberFormatInfo))
                        {
                            _inputValue = parsedValue;
                            _lastInput = _textBox.Text;
                        }
                        else
                        {
                            _updatingContent = true;
                            ReturnPreviousInput();
                        }
                    }
                    if (UpdateTimely)
                        UpdateValue();
                }
                _isSpinning = false;
            }
        }

        void UpdateValue()
        {
            if (_inputValue != Value)
                Value = _inputValue;
        }

        void UpdateText()
        {
            string value;
            if (IsFocused)
                value = FormatEdit();
            else
                value = FormatDisplay();

            if (_textBox != null)
            {
                _lastInput = value;
                _textBox.Text = value;
            }
        }

        /// <summary>
        /// 非焦点时的格式化显示串
        /// </summary>
        /// <returns>格式化显示串</returns>
        string FormatDisplay()
        {
            if (double.IsNaN(Value))
                return NullValue;

            string format = "";
            int decimals = GetNumberDecimalDigits();
            switch (ValueFormat)
            {
                case ValueFormat.Numeric:
                    format = "N";
                    NumberFormatInfo.NumberDecimalDigits = decimals;
                    break;

                case ValueFormat.Currency:
                    format = "C";
                    NumberFormatInfo.CurrencyDecimalDigits = decimals;
                    break;

                case ValueFormat.Percentage:
                    format = "P";
                    NumberFormatInfo.PercentDecimalDigits = decimals - 2;
                    break;
            }
            NumberFormatInfo.NumberGroupSeparator = ",";
            string result = Value.ToString(format, NumberFormatInfo);
            if (!string.IsNullOrEmpty(CustomUnit))
            {
                result = result + string.Format(CultureInfo.CurrentCulture, " {0}", new object[] { CustomUnit });
            }
            return result;
        }

        /// <summary>
        /// 获得焦点正在编辑时的内容串
        /// </summary>
        /// <returns>正在编辑时的内容串</returns>
        string FormatEdit()
        {
            NumberFormatInfo.NumberGroupSeparator = "";
            if (double.IsNaN(Value))
            {
                return "";
            }
            return Value.ToString("N" + GetNumberDecimalDigits(), NumberFormatInfo);
        }

        int GetNumberDecimalDigits()
        {
            int numberDecimalDigits = 0;
            if (IsInteger)
            {
                numberDecimalDigits = 0;
            }
            else if (Decimals > -1)
            {
                numberDecimalDigits = Decimals;
            }
            else
            {
                switch (ValueFormat)
                {
                    case ValueFormat.Numeric:
                        numberDecimalDigits = NumberFormatInfo.NumberDecimalDigits;
                        goto Label_0063;

                    case ValueFormat.Currency:
                        numberDecimalDigits = NumberFormatInfo.CurrencyDecimalDigits;
                        goto Label_0063;

                    case ValueFormat.Percentage:
                        numberDecimalDigits = NumberFormatInfo.PercentDecimalDigits;
                        goto Label_0063;
                }
            }
            Label_0063:
            if (ValueFormat == ValueFormat.Percentage)
            {
                numberDecimalDigits += 2;
            }
            return numberDecimalDigits;
        }

        void HandleMouseWheel(int delta)
        {
            if (IsFocused && !IsReadOnly)
            {
                if (delta > 0)
                {
                    ChangeValue(SmallChange);
                }
                else
                {
                    ChangeValue(-SmallChange);
                }
            }
        }

        bool IsFocusWithin()
        {
            return IsFocused;
        }

        bool IsSymbolicException(string keyInput, System.Globalization.NumberFormatInfo formatInfo)
        {
            bool allowDecimalSeparator = AllowDecimalSeparator;
            if ((((keyInput != formatInfo.NumberDecimalSeparator) || !allowDecimalSeparator) && (((keyInput != formatInfo.PositiveSign) && (keyInput != formatInfo.NegativeSign)) && ((keyInput != (formatInfo.NegativeSign + formatInfo.NumberDecimalSeparator)) || !allowDecimalSeparator))) && (!(keyInput == (formatInfo.PositiveSign + formatInfo.NumberDecimalSeparator)) || !allowDecimalSeparator))
            {
                return false;
            }
            return true;
        }

        void ReturnPreviousInput()
        {
            int selectionLenght = _textBox.SelectionLength;
            int selectionStart = _textBox.SelectionStart;
            _textBox.Text = _lastInput;
            _textBox.SelectionStart = (selectionStart == 0) ? 0 : (selectionStart - 1);
            _textBox.SelectionLength = selectionLenght;
        }

        bool AllowDecimalSeparator
        {
            get { return (!IsInteger && (Decimals != 0)); }
        }

        void CoerceMaximum()
        {
            double minimum = Minimum;
            double maximum = Maximum;
            if (!AreClose(_requestedMax, maximum) && (_requestedMax >= minimum))
            {
                SetValue(MaximumProperty, _requestedMax);
            }
            else if (maximum < minimum)
            {
                SetValue(MaximumProperty, minimum);
            }
        }

        void CoerceValue()
        {
            double val = Value;
            if (double.IsNaN(val))
                return;

            double minimum = Minimum;
            double maximum = Maximum;
            if (!AreClose(_requestedVal, val))
            {
                if (_requestedVal >= minimum && _requestedVal <= maximum)
                {
                    SetValue(ValueProperty, _requestedVal);
                    return;
                }
            }
            if (val < minimum)
            {
                SetValue(ValueProperty, minimum);
            }
            if (val > maximum)
            {
                SetValue(ValueProperty, maximum);
            }
        }
        #endregion
    }
}