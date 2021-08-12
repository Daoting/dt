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
        static CellUIConverter _uiConverter = new CellUIConverter();
        public static readonly DependencyProperty UITypeProperty = DependencyProperty.Register(
            "UIType",
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
        public CellUIType UIType
        {
            get { return (CellUIType)GetValue(UITypeProperty); }
            set { SetValue(UITypeProperty, value); }
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
                    Foreground = Res.深灰边框;
                    FontSize = Res.小字;
                    break;

                case CellFontStyle.大蓝:
                    Foreground = Res.醒目蓝色;
                    FontSize = Res.标题字;
                    break;

                case CellFontStyle.默认:
                    SetBinding(ForegroundProperty, new Binding { Path = new PropertyPath("Foreground") });
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
                        if (ReadLocalValue(FontSizeProperty) == DependencyProperty.UnsetValue)
                            SetBinding(FontSizeProperty, new Binding { Path = new PropertyPath("FontSize") });
                    }
                    break;

                case CellFontStyle.小灰:
                case CellFontStyle.大蓝:
                    {
                        if (ReadLocalValue(BackgroundProperty) == DependencyProperty.UnsetValue)
                            SetBinding(BackgroundProperty, new Binding { Path = new PropertyPath("Background") });
                        if (ReadLocalValue(FontWeightProperty) == DependencyProperty.UnsetValue)
                            SetBinding(FontWeightProperty, new Binding { Path = new PropertyPath("FontWeight") });
                        if (ReadLocalValue(FontStyleProperty) == DependencyProperty.UnsetValue)
                            SetBinding(FontStyleProperty, new Binding { Path = new PropertyPath("FontStyle") });
                    }
                    break;

            }
        }
    }
}
