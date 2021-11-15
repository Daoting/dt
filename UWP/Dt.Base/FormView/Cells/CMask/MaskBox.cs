#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-03-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Mask;
using System;
using System.ComponentModel;
using System.Globalization;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 支持掩码的文本框，可独立使用
    /// </summary>
    public partial class MaskBox : ContentControl
    {
        #region 静态内容
        /// <summary>
        /// 文本框的实际值
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value",
            typeof(object),
            typeof(MaskBox),
            new CoercePropertyMetadata(null, ValuePropertyChanged, CoerceValue));

        /// <summary>
        /// 掩码类型
        /// </summary>
        public static readonly DependencyProperty MaskTypeProperty = DependencyProperty.Register(
            "MaskType",
            typeof(MaskType),
            typeof(MaskBox),
            new PropertyMetadata(MaskType.Simple, OnMaskTypeChanged));

        /// <summary>
        /// 掩码表达式
        /// </summary>
        public static readonly DependencyProperty MaskProperty = DependencyProperty.Register(
            "Mask",
            typeof(string),
            typeof(MaskBox),
            new PropertyMetadata(null, MaskPropertyChanged));

        /// <summary>
        /// 是否显示掩码占位符，RegEx有效
        /// </summary>
        public static readonly DependencyProperty ShowPlaceHolderProperty = DependencyProperty.Register(
            "ShowPlaceHolders",
            typeof(bool),
            typeof(MaskBox),
            new PropertyMetadata(false, MaskPropertyChanged));

        /// <summary>
        /// 掩码占位符
        /// </summary>
        public static readonly DependencyProperty PlaceHolderProperty = DependencyProperty.Register(
            "PlaceHolder",
            typeof(char),
            typeof(MaskBox),
            new PropertyMetadata('_', MaskPropertyChanged));

        /// <summary>
        /// 是否保存为转换后的结果
        /// </summary>
        public static readonly DependencyProperty SaveLiteralProperty = DependencyProperty.Register(
            "SaveLiteral",
            typeof(bool),
            typeof(MaskBox),
            new PropertyMetadata(true, MaskPropertyChanged));

        /// <summary>
        /// 自动完成方式
        /// </summary>
        public static readonly DependencyProperty AutoCompleteProperty = DependencyProperty.Register(
            "AutoComplete",
            typeof(AutoCompleteType),
            typeof(MaskBox),
            new PropertyMetadata(AutoCompleteType.Default, MaskPropertyChanged));

        /// <summary>
        /// 是否按掩码格式化显示
        /// </summary>
        public static readonly DependencyProperty UseAsDisplayFormatProperty = DependencyProperty.Register(
            "UseAsDisplayFormat",
            typeof(bool),
            typeof(MaskBox),
            new PropertyMetadata(true, MaskPropertyChanged));

        /// <summary>
        /// 输入是否可为空
        /// </summary>
        public static readonly DependencyProperty AllowNullInputProperty = DependencyProperty.Register(
            "AllowNullInput",
            typeof(bool),
            typeof(MaskBox),
            new PropertyMetadata(false, MaskPropertyChanged));

        /// <summary>
        /// 出错时是否声音提示
        /// </summary>
        public static readonly DependencyProperty BeepOnErrorProperty = DependencyProperty.Register(
            "BeepOnError",
            typeof(bool),
            typeof(MaskBox),
            new PropertyMetadata(false, MaskPropertyChanged));

        /// <summary>
        /// 忽略空格
        /// </summary>
        public static readonly DependencyProperty IgnoreBlankProperty = DependencyProperty.Register(
            "IgnoreBlank",
            typeof(bool),
            typeof(MaskBox),
            new PropertyMetadata(true, MaskPropertyChanged));

        static void ValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MaskBox box = (MaskBox)d;
            box.RaiseValueChanged();
        }

        static object CoerceValue(DependencyObject d, object p_newValue)
        {
            MaskBox box = (MaskBox)d;
            if (box._isLoaded)
                box._maskManager.SetInitialEditValue(p_newValue);
            return p_newValue;
        }

        static void OnMaskTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MaskBox box = (MaskBox)d;
            MaskType mt = (MaskType)e.NewValue;
            if (mt == MaskType.DateTime || mt == MaskType.DateTimeAdvancingCaret || mt == MaskType.Numeric)
                box._tb.InputScope = CreateNumberScope();
            else
                box._tb.InputScope = CreateAlphaScope();
            if (box._isLoaded)
                box._maskManager.Initialize();
        }

        static void MaskPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MaskBox box = (MaskBox)d;
            if (box._isLoaded)
                box._maskManager.Initialize();
        }
        #endregion

        #region 成员变量
        readonly TextBox _tb;
        MaskMediator _maskManager;
        MaskBoxInput _inputManager;

        bool _isLoaded;
        MaskBoxState _savedState;
        bool _isSavedStateValid;
        #endregion

        #region 构造方法
        public MaskBox()
        {
            DefaultStyleKey = typeof(MaskBox);

            _tb = new TextBox { Style = Res.FvTextBox };
            _tb.TextChanged += OnTextChanged;
            _tb.AddHandler(KeyDownEvent, (KeyEventHandler)OnKeyDown, true);
            _tb.PointerWheelChanged += OnPointerWheelChanged;
            _tb.GotFocus += OnGotFocus;
            _tb.LostFocus += OnLostFocus;
            _tb.AddHandler(PointerPressedEvent, (PointerEventHandler)OnPointerPressed, true);
            _tb.AddHandler(PointerReleasedEvent, (PointerEventHandler)OnPointerReleased, true);
            // 默认输入范围为英文模式
            _tb.InputScope = CreateAlphaScope();

            _maskManager = new MaskMediator(this);
            _inputManager = new MaskBoxInput(this);
            Content = _tb;
        }
        #endregion

        #region 事件
        /// <summary>
        /// 文本变化事件
        /// </summary>
        public event TextChangedEventHandler TextChanged;

        /// <summary>
        /// 值变化事件
        /// </summary>
        public event EventHandler ValueChanged;
        #endregion

        #region 属性
        /// <summary>
        /// 获取文本框的显示内容
        /// </summary>
        public string Text
        {
            get { return _tb.Text; }
        }

        /// <summary>
        /// 获取设置文本框的实际值
        /// </summary>
        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// 获取设置文本框实际值的字符串
        /// </summary>
        public string ValueStr
        {
            get
            {
                if (Value == null)
                    return "";
                return Value.ToString();
            }
        }

        /// <summary>
        /// 获取文本框内容是否已全选
        /// </summary>
        public bool IsSelectAll
        {
            get { return ((_tb.Text.Length > 0) && (_tb.SelectionLength == Text.Length)); }
        }

        /// <summary>
        /// 选中文本的起始索引
        /// </summary>
        public int CaretIndex
        {
            get { return _tb.SelectionStart; }
            set { _tb.Select(value, 0); }
        }

        /// <summary>
        /// 输入管理员
        /// </summary>
        internal MaskBoxInput InputManager
        {
            get { return _inputManager; }
        }

        /// <summary>
        /// 掩码员
        /// </summary>
        internal MaskMediator MaskManager
        {
            get { return _maskManager; }
        }

        internal TextBox Box
        {
            get { return _tb; }
        }
        #endregion

        #region 掩码属性
        /// <summary>
        /// 掩码类型
        /// </summary>
        public MaskType MaskType
        {
            get { return (MaskType)GetValue(MaskTypeProperty); }
            set { SetValue(MaskTypeProperty, value); }
        }

        /// <summary>
        /// 掩码表达式
        /// </summary>
        public string Mask
        {
            get { return (string)GetValue(MaskProperty); }
            set { SetValue(MaskProperty, value); }
        }

        /// <summary>
        /// 是否显示掩码占位符，RegEx有效
        /// </summary>
        public bool ShowPlaceHolder
        {
            get { return (bool)GetValue(ShowPlaceHolderProperty); }
            set { SetValue(ShowPlaceHolderProperty, value); }
        }

        /// <summary>
        /// 掩码占位符，RegEx有效
        /// </summary>
        public char PlaceHolder
        {
            get { return (char)GetValue(PlaceHolderProperty); }
            set { SetValue(PlaceHolderProperty, value); }
        }

        /// <summary>
        /// 是否保存为转换后的结果，Simple、Regular有效
        /// </summary>
        public bool SaveLiteral
        {
            get { return (bool)GetValue(SaveLiteralProperty); }
            set { SetValue(SaveLiteralProperty, value); }
        }

        /// <summary>
        /// 自动完成方式，RegEx有效
        /// </summary>
        public AutoCompleteType AutoComplete
        {
            get { return (AutoCompleteType)GetValue(AutoCompleteProperty); }
            set { SetValue(AutoCompleteProperty, value); }
        }

        /// <summary>
        /// 是否按掩码格式化显示
        /// </summary>
        public bool UseAsDisplayFormat
        {
            get { return (bool)GetValue(UseAsDisplayFormatProperty); }
            set { SetValue(UseAsDisplayFormatProperty, value); }
        }

        /// <summary>
        /// 输入是否可为空
        /// </summary>
        public bool AllowNullInput
        {
            get { return (bool)GetValue(AllowNullInputProperty); }
            set { SetValue(AllowNullInputProperty, value); }
        }

        /// <summary>
        /// 出错时是否声音提示
        /// </summary>
        public bool BeepOnError
        {
            get { return (bool)GetValue(BeepOnErrorProperty); }
            set { SetValue(BeepOnErrorProperty, value); }
        }

        /// <summary>
        /// 忽略空格
        /// </summary>
        public bool IgnoreBlank
        {
            get { return (bool)GetValue(IgnoreBlankProperty); }
            set { SetValue(IgnoreBlankProperty, value); }
        }

        /// <summary>
        /// 区域设置
        /// </summary>
        public CultureInfo MaskCulture { get; set; }

        #endregion

        #region 重写方法
        /// <summary>
        /// 应用模板
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _isLoaded = true;
            _maskManager.Initialize();
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            _tb.Focus(FocusState.Programmatic);
        }
        #endregion

        #region TextBox事件
        /// <summary>
        /// 文本框中的内容更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            // 注释原因：DateTime类型在点击TextBox右侧x按钮删除内容时无效！
            //if (!_isSavedStateValid)
            //{
            //    // 未保存状态
            //    RaiseTextChanged(e);
            //    return;
            //}

            _isSavedStateValid = false;
            if (string.IsNullOrEmpty(Text))
            {
                // 清空重新执行掩码
                _maskManager.SetInitialEditValue(null);
                RaiseTextChanged(e);
                return;
            }

            // 上一状态选择区域前、选择区域后字符串
            string strBefore = _savedState.Text.Substring(0, _savedState.SelectionStart);
            string strTail = _savedState.Text.Substring(_savedState.SelectionStart + _savedState.SelectionLength);
            if ((Text.Length <= (strBefore.Length + strTail.Length)) || !Text.StartsWith(strBefore) || !Text.EndsWith(strTail))
            {
                RaiseTextChanged(e);
            }
            else
            {
                // 外部新插入的文本
                string insertTxt = Text.Substring(strBefore.Length, (Text.Length - strBefore.Length) - strTail.Length);
                try
                {
                    // 加载上次状态
                    RestoreState(_savedState);
                    _maskManager.Insert(insertTxt);
                }
                finally
                {
                    if (Text != _savedState.Text)
                    {
                        RaiseTextChanged(e);
                    }
                }
            }
        }

        /// <summary>
        /// 键盘按下事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            _inputManager.OnPreviewKeyDown(e);
            _isSavedStateValid = !e.Handled;
            if (_isSavedStateValid)
            {
                // 保存文本框当前状态
                _savedState = SaveState();
            }
        }

        /// <summary>
        /// 鼠标滚轮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            _inputManager.OnPointerWheelChanged(e);
        }

        /*** 鼠标点击获得焦点的顺序 Pressed GotFocus Released ***/
        /// <summary>
        /// 鼠标按下修改光标位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            CaretIndex = _tb.SelectionStart;
            _maskManager.ChangeCursorPosition();
        }

        /// <summary>
        /// 获得焦点时更新要显示的文本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnGotFocus(object sender, RoutedEventArgs e)
        {
            MaskBoxState state = SaveState();
            _maskManager.UpdateTextAndValue(false);
            // 重置光标位置
            _tb.SelectionLength = 0;
            _tb.SelectionStart = (state.SelectionStart == state.Text.Length) ? Text.Length : state.SelectionStart;
        }

        /// <summary>
        /// 鼠标抬起修改选中内容的光标位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            _maskManager.ChangeCursorSelection();
        }

        /// <summary>
        /// 失去焦点时更新要显示的文本和实际值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnLostFocus(object sender, RoutedEventArgs e)
        {
            _maskManager.FlushPendingEditActions();
            _maskManager.UpdateTextAndValue(true);
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 设置文本框的显示内容（不触发TextChanged事件）
        /// </summary>
        /// <param name="p_text"></param>
        internal void SetTextSilent(string p_text)
        {
            if (p_text != _tb.Text)
            {
                _tb.TextChanged -= OnTextChanged;
                _tb.Text = p_text;
                _tb.TextChanged += OnTextChanged;
                RaiseTextChanged(null);
            }
        }

        /// <summary>
        /// 设置文本框的实际值（不执行Coerce操作）
        /// </summary>
        /// <param name="p_value"></param>
        internal void SetValueSilent(object p_value)
        {
            if (p_value != Value)
            {
                this.SetStopCoerce(true);
                Value = p_value;
                this.SetStopCoerce(false);
            }
        }

        /// <summary>
        /// 保存当前文本框的状态
        /// </summary>
        /// <returns></returns>
        MaskBoxState SaveState()
        {
            MaskBoxState state = new MaskBoxState();
            state.SelectionLength = _tb.SelectionLength;
            state.SelectionStart = _tb.SelectionStart;
            state.Text = Text;
            return state;
        }

        /// <summary>
        /// 加载文本框状态
        /// </summary>
        /// <param name="p_state"></param>
        void RestoreState(MaskBoxState p_state)
        {
            if (p_state.Text == _tb.Text)
                return;

            _tb.TextChanged -= OnTextChanged;
            _tb.Text = p_state.Text;
            _tb.Select(p_state.SelectionStart, p_state.SelectionLength);
            _tb.TextChanged += OnTextChanged;
            RaiseTextChanged(null);
        }

        /// <summary>
        /// 触发文本变化事件
        /// </summary>
        /// <param name="e"></param>
        void RaiseTextChanged(TextChangedEventArgs e)
        {
            TextChanged?.Invoke(this, e);
        }

        /// <summary>
        /// 触发文本框值变化事件
        /// </summary>
        void RaiseValueChanged()
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 输入范围为英文
        /// </summary>
        /// <returns></returns>
        static InputScope CreateAlphaScope()
        {
            InputScope scope = new InputScope();
            InputScopeName isn = new InputScopeName()
            {
#if UWP
                NameValue = InputScopeNameValue.AlphanumericHalfWidth,
#else
                NameValue = InputScopeNameValue.Default,
#endif
            };
            scope.Names.Add(isn);
            return scope;
        }

        /// <summary>
        /// 输入范围为数字
        /// </summary>
        /// <returns></returns>
        static InputScope CreateNumberScope()
        {
            InputScope scope = new InputScope();
            // iOS android若设置 Number 键盘无小数点，全角才有，windows上全角无法输入！
            InputScopeName isn = new InputScopeName()
            {
#if UWP
                NameValue = InputScopeNameValue.Number,
#else
                NameValue = InputScopeNameValue.NumberFullWidth,
#endif
            };
            scope.Names.Add(isn);
            return scope;
        }
        #endregion
    }

    /// <summary>
    /// 记录RegTextBox的状态信息
    /// </summary>
    public struct MaskBoxState
    {
        /// <summary>
        /// 文本框中选中文字的程度
        /// </summary>
        public int SelectionLength;

        /// <summary>
        /// 选中文字的开始位置
        /// </summary>
        public int SelectionStart;

        /// <summary>
        /// 文本内容
        /// </summary>
        public string Text;
    }

    public enum AutoCompleteType
    {
        /// <summary>
        /// 
        /// </summary>
        Default,

        /// <summary>
        /// 
        /// </summary>
        None,

        /// <summary>
        /// 
        /// </summary>
        Strong,

        /// <summary>
        /// 
        /// </summary>
        Optimistic
    }

    /// <summary>
    /// 管理键盘鼠标的输入
    /// </summary>
    internal class MaskBoxInput
    {
        readonly MaskBox _owner;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="p_maskBox">编辑器</param>
        public MaskBoxInput(MaskBox p_maskBox)
        {
            _owner = p_maskBox;
        }

        /// <summary>
        /// 预处理按键操作
        /// </summary>
        /// <param name="e"></param>
        public void OnPreviewKeyDown(KeyRoutedEventArgs e)
        {
            bool? handled = null;
            switch (e.Key)
            {
                case VirtualKey.Up:
                    handled = new bool?(_owner.MaskManager.SpinUp());
                    break;
                case VirtualKey.Down:
                    handled = new bool?(_owner.MaskManager.SpinDown());
                    break;
                case VirtualKey.Home:
                    _owner.MaskManager.CursorHome();
                    handled = true;
                    break;
                case VirtualKey.End:
                    _owner.MaskManager.CursorEnd();
                    handled = true;
                    break;
                case VirtualKey.Right:
                    _owner.MaskManager.CursorRight();
                    handled = true;
                    break;
                case VirtualKey.Left:
                    _owner.MaskManager.CursorLeft();
                    handled = true;
                    break;
                case VirtualKey.Back:
                    _owner.MaskManager.Backspace();
                    handled = true;
                    break;
                case VirtualKey.Delete:
                    if (_owner.AllowNullInput)
                        _owner.MaskManager.Delete();
                    handled = true;
                    break;
                case VirtualKey.Space:
                    _owner.MaskManager.Insert(" ");
                    handled = true;
                    break;
                case VirtualKey.A:
                    if (InputManager.IsCtrlPressed)
                    {
                        _owner.MaskManager.SelectAll();
                        handled = true;
                    }
                    break;
            }

            if (handled.HasValue)
                e.Handled = handled.Value;
        }

        /// <summary>
        /// 预处理鼠标滚轮操作
        /// </summary>
        /// <param name="e"></param>
        public void OnPointerWheelChanged(PointerRoutedEventArgs e)
        {
            int num = e.GetCurrentPoint(_owner).Properties.MouseWheelDelta / 120;
            if (num != 0)
            {
                bool flag = false;
                for (int i = 0; i < Math.Abs(num); i++)
                {
                    if (num > 0)
                    {
                        flag = _owner.MaskManager.SpinUp();
                    }
                    else
                    {
                        flag = _owner.MaskManager.SpinDown();
                    }
                }
                e.Handled = flag;
            }
        }
    }

    /// <summary>
    /// 掩码中介，负责MaskBox与掩码员的沟通
    /// </summary>
    internal class MaskMediator
    {
        #region 成员变量
        readonly MaskBox _owner;
        MaskManager _masker;
        Locker _updateTextLocker;
        bool _isUpdateRequired;
        bool _isInLocalEditAction;

        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="p_maskBox">编辑器</param>
        public MaskMediator(MaskBox p_maskBox)
        {
            _owner = p_maskBox;
            _updateTextLocker = new Locker();
        }
        #endregion

        #region 常用操作
        /// <summary>
        /// 初始化掩码员，更新文本框
        /// </summary>
        public void Initialize()
        {
            //AtDebug.Trace("初始化掩码员");
            // 移除掩码管理者的附加事件
            if (_masker != null)
            {
                _masker.LocalEditAction -= new CancelEventHandler(OnLocalEditAction);
                _masker.EditTextChanged -= new EventHandler(OnEditTextChanged);
            }

            _masker = CreateDefaultMaskManager();
            // 为掩码管理者附加事件
            _masker.LocalEditAction += new CancelEventHandler(OnLocalEditAction);
            _masker.EditTextChanged += new EventHandler(OnEditTextChanged);
            SetInitialEditValue(_owner.Value);
        }

        /// <summary>
        /// 设置初始值
        /// </summary>
        /// <param name="initialEditValue"></param>
        public void SetInitialEditValue(object initialEditValue)
        {
            //AtDebug.Trace("设置掩码员初始值");
            _masker.SetInitialEditValue(initialEditValue);
            UpdateTextAndValue(false);
        }

        /// <summary>
        /// 有文本新输入
        /// </summary>
        /// <param name="p_insert">新文本</param>
        public void Insert(string p_insert)
        {
            if (!AllowEdit)
                return;

            SelectAllMode = false;
            if (_owner.IsSelectAll)
            {
                PrepareForClearChangesAfterSelectAll();
            }

            UpdateTextAndValue(_masker.Insert(p_insert));
            ApplyMaskSelection();
        }

        /// <summary>
        /// 下调
        /// </summary>
        /// <returns>true 表示已处理；false 未处理</returns>
        public bool SpinDown()
        {
            if (!AllowEdit)
                return false;

            PrepareForSelectAll();
            if (_masker.SpinDown())
            {
                UpdateTextAndValue(!_isInLocalEditAction);
                ApplyMaskSelection();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 上调
        /// </summary>
        /// <returns>true 表示已处理；false 未处理</returns>
        public bool SpinUp()
        {
            if (!AllowEdit)
                return false;

            PrepareForSelectAll();
            if (_masker.SpinUp())
            {
                UpdateTextAndValue(!_isInLocalEditAction);
                ApplyMaskSelection();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 删除选中部分
        /// </summary>
        /// <returns>true 表示已处理；false 未处理</returns>
        public void Delete()
        {
            if (!AllowEdit || _owner.Box.SelectionLength < 1)
                return;

            bool updateEditValue = _masker.Delete();
            CursorClearSelectAll();
            UpdateTextAndValue(updateEditValue);
            ApplyMaskSelection();
        }

        /// <summary>
        /// 退格
        /// </summary>
        /// <returns>true 表示已处理；false 未处理</returns>
        public void Backspace()
        {
            if (!AllowEdit)
                return;

            bool updateEditValue = _masker.Backspace();
            CursorClearSelectAll();
            UpdateTextAndValue(updateEditValue);
            ApplyMaskSelection();
        }

        /// <summary>
        /// 全选
        /// </summary>
        public void SelectAll()
        {
            _owner.Box.SelectAll();
            SelectAllMode = true;
            CursorSelectAll();
        }

        /// <summary>
        /// 光标左移（可带选中状态）
        /// </summary>
        public void CursorLeft()
        {
            PrepareForSelectAll();
            _masker.CursorLeft(InputManager.IsShiftPressed, false);
            UpdateTextAndValue(_isUpdateRequired);
            ApplyMaskSelection();
        }

        /// <summary>
        /// 光标右移（可带选中状态）
        /// </summary>
        public void CursorRight()
        {
            PrepareForSelectAll();
            _masker.CursorRight(InputManager.IsShiftPressed, false);
            UpdateTextAndValue(_isUpdateRequired);
            ApplyMaskSelection();
        }

        /// <summary>
        /// 光标置行头
        /// </summary>
        public void CursorHome()
        {
            PrepareForSelectAll();
            _masker.CursorHome(InputManager.IsShiftPressed);
            UpdateTextAndValue(_isUpdateRequired);
            ApplyMaskSelection();
        }

        /// <summary>
        /// 光标置尾
        /// </summary>
        public void CursorEnd()
        {
            PrepareForSelectAll();
            _masker.CursorEnd(InputManager.IsShiftPressed);
            UpdateTextAndValue(_isUpdateRequired);
            ApplyMaskSelection();
        }

        /// <summary>
        /// 鼠标按下时移动光标位置
        /// </summary>
        public void ChangeCursorPosition()
        {
            SelectAllMode = false;
            if (_isInLocalEditAction)
            {
                FlushPendingEditActions();
            }
            _masker.CursorToDisplayPosition(_owner.CaretIndex, true);
        }

        /// <summary>
        /// 鼠标抬起时选中光标区域
        /// </summary>
        public void ChangeCursorSelection()
        {
            _masker.CursorToDisplayPosition(_owner.CaretIndex, false);
            _masker.CursorToDisplayPosition(_owner.CaretIndex + _owner.Box.SelectionLength, true);
            PrepareForSelectAll();
            if (_owner.Box.SelectionLength == 0)
            {
                ApplyMaskSelection();
            }
        }

        /// <summary>
        /// 刷新未提交的操作内容
        /// </summary>
        /// <returns>true 表值被改变，需要更新显示；false 不需更新</returns>
        public bool FlushPendingEditActions()
        {
            return _masker.FlushPendingEditActions();
        }

        /// <summary>
        /// 获取掩码员的当前值
        /// </summary>
        /// <returns></returns>
        public object GetCurrentEditValue()
        {
            return _masker.GetCurrentEditValue();
        }

        /// <summary>
        /// 全选准备插入
        /// </summary>
        public void PrepareForClearChangesAfterSelectAll()
        {
            _masker.PrepareForInsertAfterSelectAll();
        }

        /// <summary>
        /// 通知撤消
        /// </summary>
        /// <returns>true 表值被改变，需要更新显示；false 不需更新</returns>
        public bool Undo()
        {
            return _masker.Undo();
        }

        /// <summary>
        /// 选中所有
        /// </summary>
        public void CursorSelectAll()
        {
            _masker.PrepareForCursorMoveAfterSelectAll();
        }

        /// <summary>
        /// 全选清除
        /// </summary>
        public void CursorClearSelectAll()
        {
            if (_owner.IsSelectAll)
            {
                _masker.ClearAfterSelectAll();
            }
        }

        /// <summary>
        /// 更新文本框的显示和值
        /// </summary>
        /// <param name="p_updateValue"></param>
        public void UpdateTextAndValue(bool p_updateValue)
        {
            if (_updateTextLocker.IsLocked)
                return;

            try
            {
                _updateTextLocker.Lock();
                if (p_updateValue)
                {
                    _owner.SetValueSilent(GetCurrentEditValue());
                }
                _owner.SetTextSilent(DisplayText);
            }
            finally
            {
                _updateTextLocker.Unlock();
            }
        }
        #endregion

        #region 属性
        /// <summary>
        /// 是否可撤消
        /// </summary>
        public bool CanUndo
        {
            get { return _masker.CanUndo; }
        }

        /// <summary>
        /// 当前光标位置
        /// </summary>
        public int DisplayCursorPosition
        {
            get { return _masker.DisplayCursorPosition; }
        }

        /// <summary>
        /// 当前选中文本的长度
        /// </summary>
        public int DisplaySelectionLength
        {
            get { return _masker.DisplaySelectionLength; }
        }

        /// <summary>
        /// 选中文本的起始位置
        /// </summary>
        public int DisplaySelectionStart
        {
            get { return _masker.DisplaySelectionStart; }
        }

        /// <summary>
        /// 显示文本
        /// </summary>
        public string DisplayText
        {
            get
            {
                if (_owner.UseAsDisplayFormat || FocusManager.GetFocusedElement() == _owner)
                    return _masker.DisplayText;

                // 显示掩码后的实际值
                object val = _masker.GetCurrentEditValue();
                if (val != null)
                    return val.ToString();
                return "";
            }
        }

        /// <summary>
        /// 值与文本是否相同
        /// </summary>
        public bool IsEditValueDifferFromEditText
        {
            get { return _masker.IsEditValueDifferFromEditText; }
        }

        /// <summary>
        /// 光标是否在最后
        /// </summary>
        public bool IsFinal
        {
            get { return _masker.IsFinal; }
        }

        /// <summary>
        /// 是否匹配
        /// </summary>
        public bool IsMatch
        {
            get { return _masker.IsMatch; }
        }

        /// <summary>
        /// 获取是否可编辑
        /// </summary>
        public bool AllowEdit
        {
            get { return (!_owner.Box.IsReadOnly && _owner.IsEnabled); }
        }

        /// <summary>
        /// 当前是否已全选
        /// </summary>
        internal bool SelectAllMode { get; set; }

        #endregion

        #region 内部方法
        /// <summary>
        /// 根据掩码属性创建编辑器的掩码员
        /// </summary>
        /// <returns></returns>
        MaskManager CreateDefaultMaskManager()
        {
            // 区域信息
            CultureInfo maskCulture = _owner.MaskCulture;
            if (maskCulture == null)
            {
                maskCulture = CultureInfo.CurrentCulture;
            }

            // 掩码表达式
            string str = _owner.Mask;
            if (str == null)
            {
                str = "";
            }

            // 根据掩码属性创建掩码管理者
            switch (_owner.MaskType)
            {
                case MaskType.DateTime:
                    return new DateTimeMaskManager(str, false, maskCulture, _owner.AllowNullInput);

                case MaskType.DateTimeAdvancingCaret:
                    return new DateTimeMaskManager(str, true, maskCulture, _owner.AllowNullInput);

                case MaskType.Numeric:
                    return new NumericMaskManager(str, maskCulture, _owner.AllowNullInput);

                case MaskType.RegEx:
                    if (_owner.IgnoreBlank && (str.Length > 0))
                    {
                        str = "(" + str + ")?";
                    }
                    return new RegExpMaskManager(str, false, _owner.AutoComplete != AutoCompleteType.None, _owner.AutoComplete == AutoCompleteType.Optimistic, _owner.ShowPlaceHolder, _owner.PlaceHolder, maskCulture);

                case MaskType.Regular:
                    return new LegacyMaskManager(LegacyMaskInfo.GetRegularMaskInfo(str, maskCulture), _owner.PlaceHolder, _owner.SaveLiteral, _owner.IgnoreBlank);

                default:
                    return new LegacyMaskManager(LegacyMaskInfo.GetSimpleMaskInfo(str, maskCulture), _owner.PlaceHolder, _owner.SaveLiteral, _owner.IgnoreBlank);
            }
        }

        /// <summary>
        /// 执行选中
        /// </summary>
        void ApplyMaskSelection()
        {
            if (SelectAllMode)
            {
                _owner.Box.SelectAll();
            }
            else
            {
                _owner.Box.SelectionStart = DisplaySelectionStart;
                _owner.Box.SelectionLength = DisplaySelectionLength;
            }
        }

        /// <summary>
        /// 掩码员通知外部来决定是否可编辑
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnLocalEditAction(object sender, CancelEventArgs e)
        {
            _isInLocalEditAction = true;
        }

        /// <summary>
        /// 掩码员文本变化事件的处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnEditTextChanged(object sender, EventArgs e)
        {
            _isUpdateRequired = true;
            _isInLocalEditAction = false;
        }

        /// <summary>
        /// 移动光标前是否需要全选
        /// </summary>
        void PrepareForSelectAll()
        {
            if (SelectAllMode)
            {
                _masker.PrepareForCursorMoveAfterSelectAll();
            }
        }
        #endregion
    }
}
