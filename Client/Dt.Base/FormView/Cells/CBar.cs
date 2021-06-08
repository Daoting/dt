#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 单元格分隔行，可以Fv之外单独使用
    /// </summary>
    [ContentProperty(Name = "Content")]
    public partial class CBar : DtControl, IFvCell
    {
        #region 静态成员
        /// <summary>
        /// 分隔行是否水平填充
        /// </summary>
        public static readonly DependencyProperty IsHorStretchProperty = DependencyProperty.Register(
            "IsHorStretch",
            typeof(bool),
            typeof(CBar),
            new PropertyMetadata(true, OnUpdateLayout));

        /// <summary>
        /// 占用的行数
        /// </summary>
        public static readonly DependencyProperty RowSpanProperty = DependencyProperty.Register(
            "RowSpan",
            typeof(int),
            typeof(CBar),
            new PropertyMetadata(1, OnUpdateLayout));

        /// <summary>
        /// 分隔行内容
        /// </summary>
        public readonly static DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content",
            typeof(object),
            typeof(CBar),
            new PropertyMetadata(null, OnContentPropertyChanged));

        static void OnUpdateLayout(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pnl = ((CBar)d).GetParent();
            if (pnl != null)
                pnl.InvalidateMeasure();
        }

        static void OnContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CBar)d).OnLoadTemplate();
        }
        #endregion

        public CBar()
        {
            DefaultStyleKey = typeof(CBar);
        }

        /// <summary>
        /// 获取设置分隔行标题
        /// </summary>
        public string Title
        {
            get { return GetValue(ContentProperty) as string; }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// 获取设置是否水平填充，默认true
        /// </summary>
        public bool IsHorStretch
        {
            get { return (bool)GetValue(IsHorStretchProperty); }
            set { SetValue(IsHorStretchProperty, value); }
        }

        /// <summary>
        /// 获取设置占用的行数，默认1行，-1时自动行高
        /// </summary>
        public int RowSpan
        {
            get { return (int)GetValue(RowSpanProperty); }
            set { SetValue(RowSpanProperty, value); }
        }

        /// <summary>
        /// 获取设置分隔行内容
        /// </summary>
        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// 在面板上的布局区域
        /// </summary>
        Rect IFvCell.Bounds { get; set; }

        protected override void OnLoadTemplate()
        {
            Grid root = (Grid)GetTemplateChild("RootGrid");
            if (root == null)
                return;

            // 为uno节省一级ContentPresenter！
            if (root.Children.Count > 1)
                root.Children.RemoveAt(0);

            if (Content is FrameworkElement con)
            {
                CFree.ApplyCellStyle(con);
                // 左上空出边线
                var margin = con.Margin;
                con.Margin = new Thickness(margin.Left + 1, margin.Top + 1, margin.Right, margin.Bottom);
                root.Children.Insert(0, con);
            }
            else
            {
                StackPanel sp = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(10) };
                TextBlock tb = new TextBlock { FontFamily = Res.IconFont, Text = "\uE02D", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 4, 0) };
                sp.Children.Add(tb);
                tb = new TextBlock { Text = (Content != null ? Content.ToString() : ""), TextWrapping = TextWrapping.NoWrap, VerticalAlignment = VerticalAlignment.Center };
                sp.Children.Add(tb);
                root.Children.Insert(0, sp);
            }
        }
    }
}