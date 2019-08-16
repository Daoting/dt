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
    public partial class Dot : ContentPresenter
    {
        static DotContentConverter _uiConverter = new DotContentConverter();
        public static readonly DependencyProperty ToProperty = DependencyProperty.Register(
            "To",
            typeof(DotContentType),
            typeof(Dot),
            new PropertyMetadata(DotContentType.Default, OnToPropertyChanged));

        static void OnToPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Dot)d).UpdateBinding();
        }

        string _id;

        public Dot()
        {
            Loaded += OnLoaded;
        }

        /// <summary>
        /// 获取设置列名(字段名)
        /// </summary>
        public string ID
        {
            get { return _id; }
            set
            {
                if (!string.IsNullOrEmpty(value) && _id != value)
                {
                    _id = value;
                    UpdateBinding();
                }
            }
        }

        /// <summary>
        /// 获取设置Dot内容类型
        /// </summary>
        public DotContentType To
        {
            get { return (DotContentType)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        void UpdateBinding()
        {
            // 实时绑定，加载时再绑定会造成初次测量时不准！！！
            if (!string.IsNullOrEmpty(_id))
            {
                if (To == DotContentType.Default)
                    SetBinding(ContentProperty, new Binding { Path = new PropertyPath($"[{_id}]"), Mode = BindingMode.OneTime });
                else
                    SetBinding(ContentProperty, new Binding { Path = new PropertyPath($"Data"), Converter = _uiConverter, ConverterParameter = this, Mode = BindingMode.OneTime });
            }
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            // 优先级：直接设置 > LvItem属性
            if (ReadLocalValue(ForegroundProperty) == DependencyProperty.UnsetValue)
                SetBinding(ForegroundProperty, new Binding { Path = new PropertyPath("Foreground") });
            if (ReadLocalValue(FontWeightProperty) == DependencyProperty.UnsetValue)
                SetBinding(FontWeightProperty, new Binding { Path = new PropertyPath("FontWeight") });
            if (ReadLocalValue(FontStyleProperty) == DependencyProperty.UnsetValue)
                SetBinding(FontStyleProperty, new Binding { Path = new PropertyPath("FontStyle") });
            if (ReadLocalValue(FontSizeProperty) == DependencyProperty.UnsetValue)
                SetBinding(FontSizeProperty, new Binding { Path = new PropertyPath("FontSize") });
        }
    }
}
