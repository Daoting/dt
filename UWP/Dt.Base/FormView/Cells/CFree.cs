#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-11-12 创建
******************************************************************************/
#endregion

#region 引用命名
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 自定义格
    /// </summary>
    [ContentProperty(Name = "Content")]
    public partial class CFree : FvCell
    {
        #region 静态成员
        /// <summary>
        /// 自定义格内容
        /// </summary>
        public readonly static DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content",
            typeof(FrameworkElement),
            typeof(CFree),
            new PropertyMetadata(null, OnContentPropertyChanged));

        static void OnContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CFree c = (CFree)d;
            if (e.NewValue != null)
            {
                ApplyCellStyle(e.NewValue);
                if (e.NewValue is IFreeCell fc)
                    fc.Owner = c;
            }
        }
        #endregion

        public CFree()
        {
            DefaultStyleKey = typeof(CFree);
        }

        /// <summary>
        /// 获取设置自定义格内容
        /// </summary>
        public FrameworkElement Content
        {
            get { return (FrameworkElement)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// 应用单元格样式
        /// </summary>
        /// <param name="p_obj"></param>
        internal static void ApplyCellStyle(object p_obj)
        {
            if (p_obj as FrameworkElement == null)
                return;

            if (p_obj is TextBox tb)
            {
                tb.Padding = new Thickness(10);
                tb.BorderThickness = new Thickness(0);
            }
            else if (p_obj is CheckBox cb)
            {
                cb.Margin = new Thickness(10, 0, 0, 0);
                cb.VerticalAlignment = VerticalAlignment.Center;
            }
            else if (p_obj is ComboBox combo)
            {
                combo.HorizontalAlignment = HorizontalAlignment.Stretch;
                combo.VerticalAlignment = VerticalAlignment.Stretch;
                combo.BorderThickness = new Thickness(0);
            }
            else if (p_obj is TextBlock block)
            {
                block.VerticalAlignment = VerticalAlignment.Center;
                block.Margin = new Thickness(10, 0, 10, 0);
            }
            else if (p_obj is Control con)
            {
                con.BorderThickness = new Thickness(0);
            }
        }

        protected override void SetValBinding()
        {
            if (Content is IFreeCell fc)
                fc.SetValBinding(ValBinding);
        }

        protected override bool SetFocus()
        {
            if (Content is IFreeCell fc)
                return fc.SetFocus();

            if (Content is Control con)
            {
                if (con.Focus(FocusState.Programmatic))
                {
                    if (con is TextBox tb)
                        tb.Select(tb.Text.Length, 0);
                    return true;
                }
            }
            return false;
        }
    }
}