#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-29 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Dt.Base.FormView;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
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

        static void OnAcceptsReturnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CText c = (CText)d;
            if (c._tb != null)
            {
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
        }

        static void OnUpdateTimelyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CText c = (CText)d;
            if (c._tb != null)
            {
                if ((bool)e.NewValue)
                    c._tb.TextChanged += OnUpdateSource;
                else
                    c._tb.TextChanged -= OnUpdateSource;
            }
        }
        #endregion

        TextBox _tb;

        /// <summary>
        /// 获取设置是否允许多行显示
        /// </summary>
        [CellParam("允许多行")]
        public bool AcceptsReturn
        {
            get { return (bool)GetValue(AcceptsReturnProperty); }
            set { SetValue(AcceptsReturnProperty, value); }
        }

        /// <summary>
        /// 获取设置可在文本框中键入或粘贴的最大字符数
        /// </summary>
        [CellParam("最大字符数")]
        public int MaxLength
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }

        /// <summary>
        /// 获取设置是否实时更新值
        /// </summary>
        [CellParam("实时更新值")]
        public bool UpdateTimely
        {
            get { return (bool)GetValue(UpdateTimelyProperty); }
            set { SetValue(UpdateTimelyProperty, value); }
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _tb = (TextBox)GetTemplateChild("TextBox");
            if (UpdateTimely)
                _tb.TextChanged += OnUpdateSource;
            if (AcceptsReturn)
            {
                _tb.AcceptsReturn = true;
                _tb.TextWrapping = TextWrapping.Wrap;
            }
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

        /// <summary>
        /// 确保TextBox的Text实时更新到数据源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void OnUpdateSource(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            // 确保实时更新到数据源
            BindingExpression expresson = tb.GetBindingExpression(TextBox.TextProperty);
            if (expresson != null)
                expresson.UpdateSource();
        }
    }
}