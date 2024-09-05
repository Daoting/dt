#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-06-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Windows.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Text;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 列定义
    /// </summary>
    public partial class Col : DependencyObject
    {
        #region 静态内容
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(Col),
            new PropertyMetadata(null, OnTitleChanged));

        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register(
            "Width",
            typeof(string),
            typeof(Col),
            new PropertyMetadata(null, OnWidthChanged));

        public static readonly DependencyProperty RowSpanProperty = DependencyProperty.Register(
            "RowSpan",
            typeof(int),
            typeof(Col),
            new PropertyMetadata(1));

        public static readonly DependencyProperty AllowSortingProperty = DependencyProperty.Register(
            "AllowSorting",
            typeof(bool),
            typeof(Col),
            new PropertyMetadata(true));

        public static readonly DependencyProperty CallProperty = DependencyProperty.Register(
            "Call",
            typeof(string),
            typeof(Col),
            new PropertyMetadata(null));

        public static readonly DependencyProperty FormatProperty = DependencyProperty.Register(
            "Format",
            typeof(string),
            typeof(Col),
            new PropertyMetadata(null));

        public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
            "Foreground",
            typeof(SolidColorBrush),
            typeof(Col),
            new PropertyMetadata(Res.默认前景));

        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
            "Background",
            typeof(SolidColorBrush),
            typeof(Col),
            new PropertyMetadata(Res.TransparentBrush));

        public static readonly DependencyProperty FontWeightProperty = DependencyProperty.Register(
            "FontWeight",
            typeof(FontWeight),
            typeof(Col),
            new PropertyMetadata(FontWeights.Normal));

        public static readonly DependencyProperty FontStyleProperty = DependencyProperty.Register(
            "FontStyle",
            typeof(FontStyle),
            typeof(Col),
            new PropertyMetadata(FontStyle.Normal));

        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(
            "FontSize",
            typeof(double),
            typeof(Col),
            new PropertyMetadata(16d));

        public static readonly DependencyProperty VisibilityProperty = DependencyProperty.Register(
            "Visibility",
            typeof(Visibility),
            typeof(Col),
            new PropertyMetadata(Visibility.Visible, OnVisibilityChanged));
        
        static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Col c = (Col)d;
            var val = (string)e.NewValue;
            if (!string.IsNullOrEmpty(val) && val.Contains('@'))
            {
                c.Title = val.Replace('@', '\u000A');
                if (c.Owner != null)
                    c.Owner.OnColWidthChanged();
            }
        }

        static void OnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Col c = (Col)d;
            if (c.Owner != null)
                c.Owner.OnColWidthChanged();
        }

        static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Col c = (Col)d;
            if (c.Owner != null)
                c.Owner.OnReloading();
        }
        
        string _id;
        #endregion

        /// <summary>
        /// 获取设置列名(字段名)
        /// </summary>
        public string ID
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    Owner?.OnReloading();
                }
            }
        }

        /// <summary>
        /// 获取设置列标题
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// 获取设置列宽，默认120，能显示6个中文字，支持 * 和 Auto
        /// <para>1. 未设置或Auto时，120</para>
        /// <para>2. 数值时为固定列宽</para>
        /// <para>3. * 或 x* 时，类似Grid动态计算剩余宽度按系数分配，无剩余或1*小于120时采用1*为120的宽度</para>
        /// </summary>
        public string Width
        {
            get { return (string)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        /// <summary>
        /// 获取设置占用的行数，默认1行，视图模式为List和Tile时有效
        /// </summary>
        public int RowSpan
        {
            get { return (int)GetValue(RowSpanProperty); }
            set { SetValue(RowSpanProperty, value); }
        }

        /// <summary>
        /// 获取设置点击列头是否可以排序
        /// </summary>
        public bool AllowSorting
        {
            get { return (bool)GetValue(AllowSortingProperty); }
            set { SetValue(AllowSortingProperty, value); }
        }

        /// <summary>
        /// 获取设置自定义单元格UI的方法名，多个方法名用逗号隔开，形如：Def.Icon,Def.小灰
        /// </summary>
        public string Call
        {
            get { return (string)GetValue(CallProperty); }
            set { SetValue(CallProperty, value); }
        }

        /// <summary>
        /// 获取设置格式串，null或空时按默认显示，如：时间格式、小数位格式、枚举类型名称
        /// <para>也是自定义单元格UI方法的参数</para>
        /// </summary>
        public string Format
        {
            get { return (string)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        /// <summary>
        /// 获取设置列前景画刷
        /// </summary>
        public SolidColorBrush Foreground
        {
            get { return (SolidColorBrush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        /// <summary>
        /// 获取设置列背景画刷
        /// </summary>
        public SolidColorBrush Background
        {
            get { return (SolidColorBrush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        /// <summary>
        /// 获取设置列字体粗细
        /// </summary>
        public FontWeight FontWeight
        {
            get { return (FontWeight)GetValue(FontWeightProperty); }
            set { SetValue(FontWeightProperty, value); }
        }

        /// <summary>
        /// 获取设置列文本样式
        /// </summary>
        public FontStyle FontStyle
        {
            get { return (FontStyle)GetValue(FontStyleProperty); }
            set { SetValue(FontStyleProperty, value); }
        }

        /// <summary>
        /// 获取设置列文本大小
        /// </summary>
        public double FontSize
        {
            get { return (double)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        /// <summary>
        /// 获取设置列是否可见
        /// </summary>
        public Visibility Visibility
        {
            get { return (Visibility)GetValue(VisibilityProperty); }
            set { SetValue(VisibilityProperty, value); }
        }
        
        /// <summary>
        /// 水平位置
        /// </summary>
        internal double Left { get; set; }

        /// <summary>
        /// 实际宽度
        /// </summary>
        internal double ActualWidth { get; set; }

        internal Cols Owner { get; set; }
        
        internal Col Clone()
        {
            Col col = new Col { ID = ID };
            if (!string.IsNullOrEmpty(Title))
                col.Title = Title;
            if (!string.IsNullOrEmpty(Width))
                col.Width = Width;
            if (RowSpan != 1)
                col.RowSpan = RowSpan;
            if (!AllowSorting)
                col.AllowSorting = false;
            if (!string.IsNullOrEmpty(Call))
                col.Call = Call;
            if (!string.IsNullOrEmpty(Format))
                col.Format = Format;
            
            if (Foreground != Res.默认前景)
                col.Foreground = Foreground;
            if (Background != Res.TransparentBrush)
                col.Background = Background;
            if (FontWeight != FontWeights.Normal)
                col.FontWeight = FontWeight;
            if (FontStyle != FontStyle.Normal)
                col.FontStyle = FontStyle;
            if (FontSize != 16d)
                col.FontSize = FontSize;
            
            return col;
        }
    }
}
