#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-05-25 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.FormView;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 只读信息格，始终只读
    /// </summary>
    public partial class CTip : FvCell
    {
        #region 静态内容
        public static readonly DependencyProperty FormatProperty = DependencyProperty.Register(
            "Format",
            typeof(string),
            typeof(CTip),
            new PropertyMetadata(null, OnFormatChanged));

        static void OnFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CTip tip = (CTip)d;
            if (tip._isLoaded)
            {
                var tb = (TextBlock)tip.GetTemplateChild("Block");
                if (tb != null)
                {
                    tb.ClearValue(TextBlock.TextProperty);
                    tb.SetBinding(TextBlock.TextProperty, tip.ValBinding);
                }
            }
        }
        #endregion

        public CTip()
        {
            DefaultStyleKey = typeof(CTip);
            ValConverter = new TipValConverter(this);
        }

        #region 事件
        /// <summary>
        /// 点击事件
        /// </summary>
#if ANDROID
        new
#endif
        public event EventHandler Click;
        #endregion

        /// <summary>
        /// 获取设置格式串，时间格式如：yyyy-MM-dd HH:mm:ss
        /// </summary>
        [CellParam("格式串")]
        public string Format
        {
            get { return (string)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        #region 重写方法
        /// <summary>
        /// 切换内容
        /// </summary>
        //protected override void OnLoadTemplate()
        //{

        //}

        protected override void SetValBinding()
        {
            var tb = (TextBlock)GetTemplateChild("Block");
            if (tb != null)
                tb.SetBinding(TextBlock.TextProperty, ValBinding);
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            if (Click == null)
                return;

            e.Handled = true;
            VisualStateManager.GoToState(this, "PointerOver", true);
        }
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            if (Click == null)
                return;

            var props = e.GetCurrentPoint(null).Properties;
            if (props.IsLeftButtonPressed)
            {
                if (CapturePointer(e.Pointer))
                {
                    e.Handled = true;
                    VisualStateManager.GoToState(this, "Pressed", true);
                }
            }
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            if (Click == null)
                return;

            ReleasePointerCapture(e.Pointer);
            e.Handled = true;

            var pt = e.GetCurrentPoint(null).Position;
            if (this.ContainPoint(pt))
            {
                VisualStateManager.GoToState(this, "PointerOver", true);
                Click?.Invoke(this, EventArgs.Empty);
            }
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            if (Click == null)
                return;

            e.Handled = true;
            VisualStateManager.GoToState(this, "Normal", true);
        }

        protected override void OnPointerCaptureLost(PointerRoutedEventArgs e)
        {
            if (Click == null)
                return;

            e.Handled = true;
            VisualStateManager.GoToState(this, "Normal", true);
        }
        #endregion
    }

    /// <summary>
    /// 源CTip.Data，目标TextBlock.Text
    /// </summary>
    class TipValConverter : IValueConverter
    {
        CTip _owner;

        public TipValConverter(CTip p_owner)
        {
            _owner = p_owner;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return "";

            if (value is DateTime dt)
            {
                try
                {
                    if (string.IsNullOrEmpty(_owner.Format))
                        return dt.ToString("yyyy-MM-dd");
                    return dt.ToString(_owner.Format);
                }
                catch { }
            }
            else if (!string.IsNullOrEmpty(_owner.Format) && value is IFormattable f)
            {
                try
                {
                    return f.ToString(_owner.Format, null);
                }
                catch { }
            }
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}