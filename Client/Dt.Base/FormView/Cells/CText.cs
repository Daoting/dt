#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-29 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Text;
using Dt.Base.FormView;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 普通文本格
    /// </summary>
    public partial class CText : FvCell
    {
        #region 静态成员
        /// <summary>
        /// 是否允许多行显示
        /// </summary>
        public static readonly DependencyProperty AcceptsReturnProperty = DependencyProperty.Register(
            "AcceptsReturn",
            typeof(bool),
            typeof(CText),
            new PropertyMetadata(false, OnAcceptsReturnChanged));

        /// <summary>
        /// 可在文本框中键入或粘贴的最大字符数
        /// </summary>
        public static readonly DependencyProperty MaxLengthProperty = DependencyProperty.Register(
            "MaxLength",
            typeof(int),
            typeof(CText),
            new PropertyMetadata(0));

        /// <summary>
        /// 是否实时更新Cell值
        /// </summary>
        public static readonly DependencyProperty UpdateTimelyProperty = DependencyProperty.Register(
            "UpdateTimely",
            typeof(bool),
            typeof(CText),
            new PropertyMetadata(true, OnUpdateTimelyChanged));

        public static readonly DependencyProperty InputScopeProperty = DependencyProperty.Register(
            "InputScope",
            typeof(InputScope),
            typeof(CText),
            new PropertyMetadata(null, OnInputScopeChanged));

        static void OnAcceptsReturnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CText c = (CText)d;
            if ((bool)e.NewValue)
            {
                c._tb.AcceptsReturn = true;
                c._tb.TextWrapping = TextWrapping.Wrap;
            }
            else
            {
                c._tb.AcceptsReturn = false;
                c._tb.TextWrapping = TextWrapping.NoWrap;
            }
        }

        static void OnUpdateTimelyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CText)d).OnUpdateTimelyChanged();
        }

        static void OnInputScopeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CText c = (CText)d;

            // 输入法始终不可为null，禁止绑定方式，不然uno中文本框右侧的清空按钮失效！！！
            if (e.NewValue == null)
                c._tb.InputScope = new InputScope { Names = { new InputScopeName { NameValue = InputScopeNameValue.Default } } };
            else
                c._tb.InputScope = (InputScope)e.NewValue;
        }
        #endregion

        protected TextBox _tb;

        #region 构造方法
        public CText()
        {
            DefaultStyleKey = typeof(CText);
            _tb = new TextBox { Style = Res.FvTextBox };
        }
        #endregion

        /// <summary>
        /// 获取设置是否允许多行显示，默认false
        /// </summary>
        [CellParam("允许多行")]
        public bool AcceptsReturn
        {
            get { return (bool)GetValue(AcceptsReturnProperty); }
            set { SetValue(AcceptsReturnProperty, value); }
        }

        /// <summary>
        /// 获取设置可在文本框中键入或粘贴的最大字符数，默认0无限制
        /// </summary>
        [CellParam("最大字符数，默认0无限制")]
        public int MaxLength
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }

        /// <summary>
        /// 获取设置是否实时更新值，默认 true 实时更新
        /// </summary>
        [CellParam("实时更新值")]
        public bool UpdateTimely
        {
            get { return (bool)GetValue(UpdateTimelyProperty); }
            set { SetValue(UpdateTimelyProperty, value); }
        }

        /// <summary>
        /// 获取设置输入法，输入法始终不可为null，不然uno中文本框右侧的清空按钮失效！！！
        /// </summary>
        public InputScope InputScope
        {
            get { return (InputScope)GetValue(InputScopeProperty); }
            set { SetValue(InputScopeProperty, value); }
        }

        protected override IFvCall DefaultMiddle => new TextValConverter();

        protected override void OnApplyCellTemplate()
        {
            // 原本放在控件模板中，因wasm中长文本对应textarea放入代码构造，跨平台就是一个妥协过程！
            _tb.SetBinding(TextBox.IsReadOnlyProperty, new Binding { Path = new PropertyPath("ReadOnlyBinding"), Source = this });
            _tb.SetBinding(TextBox.MaxLengthProperty, new Binding { Path = new PropertyPath("MaxLength"), Source = this });
            _tb.SetBinding(TextBox.PlaceholderTextProperty, new Binding { Path = new PropertyPath("Placeholder"), Source = this });

            OnUpdateTimelyChanged();
            _panel.Child = _tb;
        }

        protected override void SetValBinding()
        {
            _tb.SetBinding(TextBox.TextProperty, ValBinding);
        }

        protected override bool SetFocus()
        {
            if (_tb.Focus(FocusState.Programmatic))
            {
                _tb.Select(_tb.Text.Length, 0);
                return true;
            }
            return false;
        }

        void OnUpdateTimelyChanged()
        {
            _tb.TextChanged -= OnUpdateSource;
            if (UpdateTimely)
                _tb.TextChanged += OnUpdateSource;
        }

        /// <summary>
        /// 确保TextBox的Text实时更新到数据源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnUpdateSource(object sender, TextChangedEventArgs e)
        {
            BindingExpression expresson = _tb.GetBindingExpression(TextBox.TextProperty);
            if (expresson == null)
                return;

            string src = null;
            if (ValBinding.Source is ICell cell)
                src = cell.GetVal<string>();

            if (src == _tb.Text)
            {
                // 数据源的值和TextBox.Text相同，是数据源钩子修改了新值，再次更新到TextBox触发TextChanged事件，并且将光标重置到开头位置
                // 不需要再更新到数据源，光标置最后
                _tb.SelectionStart = _tb.Text.Length;
            }
            else
            {
                // 确保实时更新到数据源
                expresson.UpdateSource();
            }
        }

        public override void Destroy()
        {
            if (_tb != null)
                _tb.TextChanged -= OnUpdateSource;
        }
    }

    /// <summary>
    /// 源CText.Data，目标TextBox.Text
    /// </summary>
    class TextValConverter : IFvCall
    {
        /// <summary>
        /// 从数据源取值
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public object Get(Mid m)
        {
            return m.Val;
        }

        /// <summary>
        /// 将值写入数据源，返回值为待写入值
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public object Set(Mid m)
        {
            // *****************************
            // 数据源中保存的换行符始终只是 \n
            // *****************************

            // TextBox支持多行时：
            // windows换行符只有\r，每次向TextBox赋值时 \r\n 或 \n 都被强制替换为 \r
            // 其它平台换行符只有 \n，wasm每次向TextBox赋值时 \r\n 被强制替换为 \n，ios android不强制替换

#if WIN || SKIA
            if (!((CText)m.Cell).AcceptsReturn || m.Val == null)
                return m.Val;

            // 此处为统一：将 \r 替换成 \n
            return m.Str.Replace('\r', '\n');
#else
            return m.Val;
#endif
        }
    }
}