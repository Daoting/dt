#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-06-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 列定义
    /// </summary>
    public partial class Col : DependencyObject, ICellUI
    {
        #region 静态内容
        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register(
            "Width",
            typeof(double),
            typeof(Col),
            new PropertyMetadata(100d));

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

        public static readonly DependencyProperty UIProperty = DependencyProperty.Register(
            "UI",
            typeof(CellUIType),
            typeof(Col),
            new PropertyMetadata(CellUIType.Default));

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
        #endregion

        /// <summary>
        /// 获取设置列名(字段名)
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 获取设置列标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 获取设置列宽，默认100
        /// </summary>
        public double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        /// <summary>
        /// 获取设置占用的行数，默认1行
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
        /// 水平位置
        /// </summary>
        internal double Left { get; set; }
    }
}
