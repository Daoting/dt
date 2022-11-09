#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2015-07-14 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 列表中行模板的占位格
    /// </summary>
    public partial class Dot : ContentPresenter
    {
        #region 静态内容
        const double _defaultFontSize = 16;
        public static readonly DependencyProperty UIProperty = DependencyProperty.Register(
            "UI",
            typeof(CellUIType),
            typeof(Dot),
            new PropertyMetadata(CellUIType.Default));

        public static readonly DependencyProperty FormatProperty = DependencyProperty.Register(
            "Format",
            typeof(string),
            typeof(Dot),
            new PropertyMetadata(null));

        public static readonly DependencyProperty FontProperty = DependencyProperty.Register(
            "Font",
            typeof(CellFontStyle),
            typeof(Dot),
            new PropertyMetadata(CellFontStyle.默认));

        public static readonly DependencyProperty AutoHideProperty = DependencyProperty.Register(
            "AutoHide",
            typeof(bool),
            typeof(Dot),
            new PropertyMetadata(true));

        bool _isInit;
        #endregion

        public Dot()
        {
            // 系统默认大小14，uwp初次测量结果偏小
            FontSize = _defaultFontSize;
            DataContextChanged += OnDataContextChanged;
        }

        /// <summary>
        /// 获取设置列名(字段名)
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 获取设置单元格UI类型
        /// </summary>
        public CellUIType UI
        {
            get { return (CellUIType)GetValue(UIProperty); }
            set { SetValue(UIProperty, value); }
        }

        /// <summary>
        /// 获取设置格式串，null或空时按默认显示，如：时间格式、小数位格式、枚举类型名称
        /// </summary>
        public string Format
        {
            get { return (string)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        /// <summary>
        /// 获取设置文字样式，默认时绑定到ViewItem属性
        /// </summary>
        public CellFontStyle Font
        {
            get { return (CellFontStyle)GetValue(FontProperty); }
            set { SetValue(FontProperty, value); }
        }

        /// <summary>
        /// 获取设置内容为空时是否自动隐藏Dot，默认true
        /// <para>隐藏时Padding 或 Margin 不再占用位置！</para>
        /// <para>若false，内容为空时仍然占位</para>
        /// </summary>
        public bool AutoHide
        {
            get { return (bool)GetValue(AutoHideProperty); }
            set { SetValue(AutoHideProperty, value); }
        }

        void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs e)
        {
            if (!_isInit)
            {
                // OnApplyTemplate 或 Loaded 中绑定在uno上已晚！！
                // 初次触发 DataContextChanged 在加载 DataTemplate 后，还未在可视树上
                _isInit = true;

                // 优先级：直接设置 > ViewItem属性，未直接设置的绑定ViewItem中行样式
                if (ReadLocalValue(ForegroundProperty) == DependencyProperty.UnsetValue)
                    SetBinding(ForegroundProperty, new Binding { Path = new PropertyPath("Foreground"), Mode = BindingMode.OneTime });
                if (ReadLocalValue(BackgroundProperty) == DependencyProperty.UnsetValue)
                    SetBinding(BackgroundProperty, new Binding { Path = new PropertyPath("Background"), Mode = BindingMode.OneTime });
                if (ReadLocalValue(FontWeightProperty) == DependencyProperty.UnsetValue)
                    SetBinding(FontWeightProperty, new Binding { Path = new PropertyPath("FontWeight"), Mode = BindingMode.OneTime });
                if (ReadLocalValue(FontStyleProperty) == DependencyProperty.UnsetValue)
                    SetBinding(FontStyleProperty, new Binding { Path = new PropertyPath("FontStyle"), Mode = BindingMode.OneTime });
                if (FontSize == _defaultFontSize)
                    SetBinding(FontSizeProperty, new Binding { Path = new PropertyPath("FontSize"), Mode = BindingMode.OneTime });
            }

            ViewItem vi = e.NewValue as ViewItem;
            var result = vi.GetCellUI(this);
            if (AutoHide)
            {
                if (result == null)
                {
                    // 隐藏Dot为了其 Padding 或 Margin 不再占用位置！！！
                    // 未处理Table模式的单元格ContentPresenter，因其负责画右下边线！
                    if (Visibility == Visibility.Visible)
                        Visibility = Visibility.Collapsed;
                }
                else if (Visibility == Visibility.Collapsed)
                {
                    // 数据变化时重新可见
                    Visibility = Visibility.Visible;
                }
            }
            else if (result == null)
            {
                // 为占位用
                result = new TextBlock();
            }
            Content = result;
        }
    }
}
