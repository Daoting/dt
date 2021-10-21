#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2015-07-14 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.ListView;
using System;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 列表中行模板的占位格
    /// </summary>
    public partial class Dot : ContentPresenter, ICellUI
    {
        const double _defaultFontSize = 16;
        static CellUIConverter _uiConverter = new CellUIConverter();
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
            new PropertyMetadata(CellFontStyle.默认, OnFontChanged));

        static void OnFontChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Dot)d).ApplyFontStyle();
        }

        public Dot()
        {
            // 系统默认大小14，uwp初次测量结果偏小
            FontSize = _defaultFontSize;
            SetBinding(ContentProperty, new Binding { Converter = _uiConverter, ConverterParameter = this, Mode = BindingMode.OneTime });
            Loaded += OnLoaded;
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

        void ApplyFontStyle()
        {
            switch (Font)
            {
                case CellFontStyle.小灰:
                    Foreground = Res.深灰2;
                    FontSize = Res.小字;
                    break;

                case CellFontStyle.黑白:
                    Foreground = Res.WhiteBrush;
                    Background = Res.BlackBrush;
                    if (ReadLocalValue(PaddingProperty) == DependencyProperty.UnsetValue)
                        Padding = new Thickness(10, 4, 10, 4);
                    break;

                case CellFontStyle.蓝白:
                    Foreground = Res.WhiteBrush;
                    Background = Res.主蓝;
                    if (ReadLocalValue(PaddingProperty) == DependencyProperty.UnsetValue)
                        Padding = new Thickness(10, 4, 10, 4);
                    break;

                case CellFontStyle.红白:
                    Foreground = Res.WhiteBrush;
                    Background = Res.RedBrush;
                    if (ReadLocalValue(PaddingProperty) == DependencyProperty.UnsetValue)
                        Padding = new Thickness(10, 4, 10, 4);
                    break;

                case CellFontStyle.默认:
                    SetBinding(ForegroundProperty, new Binding { Path = new PropertyPath("Foreground") });
                    SetBinding(FontWeightProperty, new Binding { Path = new PropertyPath("FontWeight") });
                    SetBinding(FontSizeProperty, new Binding { Path = new PropertyPath("FontSize") });
                    break;
            }
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            switch (Font)
            {
                case CellFontStyle.默认:
                    {
                        // 优先级：直接设置 > ViewItem属性，未直接设置的绑定ViewItem中行样式
                        if (ReadLocalValue(ForegroundProperty) == DependencyProperty.UnsetValue)
                            SetBinding(ForegroundProperty, new Binding { Path = new PropertyPath("Foreground") });
                        if (ReadLocalValue(BackgroundProperty) == DependencyProperty.UnsetValue)
                            SetBinding(BackgroundProperty, new Binding { Path = new PropertyPath("Background") });
                        if (ReadLocalValue(FontWeightProperty) == DependencyProperty.UnsetValue)
                            SetBinding(FontWeightProperty, new Binding { Path = new PropertyPath("FontWeight") });
                        if (ReadLocalValue(FontStyleProperty) == DependencyProperty.UnsetValue)
                            SetBinding(FontStyleProperty, new Binding { Path = new PropertyPath("FontStyle") });
                        if (FontSize == _defaultFontSize)
                            SetBinding(FontSizeProperty, new Binding { Path = new PropertyPath("FontSize") });
                    }
                    break;

                case CellFontStyle.小灰:
                    {
                        if (ReadLocalValue(BackgroundProperty) == DependencyProperty.UnsetValue)
                            SetBinding(BackgroundProperty, new Binding { Path = new PropertyPath("Background") });
                        if (ReadLocalValue(FontWeightProperty) == DependencyProperty.UnsetValue)
                            SetBinding(FontWeightProperty, new Binding { Path = new PropertyPath("FontWeight") });
                        if (ReadLocalValue(FontStyleProperty) == DependencyProperty.UnsetValue)
                            SetBinding(FontStyleProperty, new Binding { Path = new PropertyPath("FontStyle") });
                    }
                    break;

                case CellFontStyle.黑白:
                case CellFontStyle.蓝白:
                case CellFontStyle.红白:
                    {
                        if (ReadLocalValue(FontWeightProperty) == DependencyProperty.UnsetValue)
                            SetBinding(FontWeightProperty, new Binding { Path = new PropertyPath("FontWeight") });
                        if (ReadLocalValue(FontStyleProperty) == DependencyProperty.UnsetValue)
                            SetBinding(FontStyleProperty, new Binding { Path = new PropertyPath("FontStyle") });
                        if (FontSize == _defaultFontSize)
                            SetBinding(FontSizeProperty, new Binding { Path = new PropertyPath("FontSize") });
                    }
                    break;

            }
        }
    }
}
