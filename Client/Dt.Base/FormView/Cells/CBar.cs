#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Windows.Foundation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Dt.Base.FormView;
using Microsoft.UI.Xaml.Input;
using System.Xml;
using System.Xml.Linq;
using System.Text;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 单元格分隔行，可以Fv之外单独使用
    /// </summary>
    [ContentProperty(Name = "Content")]
    public partial class CBar : Control, IFvCell
    {
        #region 静态成员
        const string _prefix = "🔶 ";

        public readonly static DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(CBar),
            new PropertyMetadata(null, OnContentPropertyChanged));

        public static readonly DependencyProperty ColSpanProperty = DependencyProperty.Register(
            "ColSpan",
            typeof(double),
            typeof(CBar),
            new PropertyMetadata(0.0d, OnColSpanChanged));

        public static readonly DependencyProperty RowSpanProperty = DependencyProperty.Register(
            "RowSpan",
            typeof(int),
            typeof(CBar),
            new PropertyMetadata(1, OnUpdateLayout));

        public readonly static DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content",
            typeof(object),
            typeof(CBar),
            new PropertyMetadata(null, OnContentPropertyChanged));

        public readonly static DependencyProperty ContentXamlProperty = DependencyProperty.Register(
            "ContentXaml",
            typeof(string),
            typeof(CBar),
            new PropertyMetadata(null));

        static void OnUpdateLayout(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pnl = ((CBar)d).GetParent();
            if (pnl != null)
                pnl.InvalidateMeasure();
        }

        static void OnColSpanChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cell = (CBar)d;
            double span = (double)e.NewValue;
            if (span > 1)
            {
                cell.ColSpan = 1;
            }
            else if (span < 0)
            {
                cell.ColSpan = 0;
            }
            else if (Math.Round(span, 2) != span)
            {
                // xaml中的double有17位小数，精度有误差
                cell.ColSpan = Math.Round(span, 2);
            }
            else
            {
                OnUpdateLayout(d, e);
            }
        }

        static void OnContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CBar)d).OnApplyTemplate();
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
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
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
        /// 获取设置单元格占用列的比例，取值范围 0~1，0表示水平填充，1表示占满整列，默认0水平填充
        /// </summary>
        public double ColSpan
        {
            get { return (double)GetValue(ColSpanProperty); }
            set { SetValue(ColSpanProperty, value); }
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
        /// 设计时用，分隔行内容的xaml
        /// </summary>
        public string ContentXaml
        {
            get { return (string)GetValue(ContentXamlProperty); }
            set { SetValue(ContentXamlProperty, value); }
        }

        /// <summary>
        /// 获取所属的Fv
        /// </summary>
        internal Fv Owner { get; set; }

        /// <summary>
        /// 在面板上的布局区域
        /// </summary>
        Rect IFvCell.Bounds { get; set; }

        /// <summary>
        /// 导出xaml
        /// </summary>
        /// <param name="p_xw"></param>
        public void ExportXaml(XmlWriter p_xw)
        {
            Type tp = GetType();
            p_xw.WriteStartElement("a", tp.Name, null);

            if (Title != null)
                p_xw.WriteAttributeString("Title", Title);
            if (RowSpan != 1)
                p_xw.WriteAttributeString("RowSpan", RowSpan.ToString());
            if (ColSpan != 0)
                p_xw.WriteAttributeString("ColSpan", ColSpan.ToString());

            if (!string.IsNullOrEmpty(ContentXaml))
            {
                FvDesignKit.CopyXml(p_xw, ContentXaml);
            }
            p_xw.WriteEndElement();
        }

        internal void AddCustomDesignCells(FvItems p_items)
        {
            CBar bar = new CBar { Title = "分隔行内容" };
            p_items.Add(bar);

            CText text = new CText
            {
                ID = "ContentXaml",
                ShowTitle = false,
                AcceptsReturn = true,
                RowSpan = 6,
            };
            p_items.Add(text);

            var sp = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };
            var btn = new Button { Content = "+模板" };
            Menu m = new Menu { Placement = MenuPosition.BottomLeft };
            Mi mi = new Mi { ID = "单个按钮" };
            mi.Call += () => FvDesignKit.AddXamlToCText(text, "<Button Content=\"按钮\" HorizontalAlignment=\"Right\"/>");
            m.Add(mi);

            mi = new Mi { ID = "多个按钮" };
            mi.Call += () => FvDesignKit.AddXamlToCText(text, "<StackPanel Orientation=\"Horizontal\" HorizontalAlignment=\"Right\">\n  <Button Content=\"按钮\"/>\n  <Button Content=\"按钮\"/>\n</StackPanel>");
            m.Add(mi);

            mi = new Mi { ID = "工具栏" };
            mi.Call += () => FvDesignKit.AddXamlToCText(text, "<a:Menu HorizontalAlignment=\"Right\">\n  <a:Mi ID=\"保存\" Icon=\"保存\" />\n  <a:Mi ID=\"搜索\" Icon=\"搜索\" />\n</a:Menu>");
            m.Add(mi);
            Ex.SetMenu(btn, m);
            sp.Children.Add(btn);

            btn = new Button { Content = "应用" };
            btn.Click += (s, e) =>
            {
                if (string.IsNullOrEmpty(ContentXaml))
                    Content = null;
                else
                    Content = Kit.LoadXaml<object>(ContentXaml);
            };
            sp.Children.Add(btn);
            bar.Content = sp;
        }

        internal void LoadXamlString(XmlNode p_node)
        {
            if (p_node.ChildNodes.Count == 1)
                ContentXaml = FvDesignKit.GetNodeXml(p_node.ChildNodes[0], false);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Grid root = (Grid)GetTemplateChild("RootGrid");
            if (root == null)
                return;

            // 为uno节省一级ContentPresenter！
            while (root.Children.Count > 1)
            {
                root.Children.RemoveAt(0);
            }

            string title = Title;
            if (Content == null && string.IsNullOrEmpty(title))
            {
                // 不绘制任何内容，做空行使用
            }
            else if (Content == null)
            {
                // 只标题
                title = _prefix + title.Replace("\n", "\n" + _prefix);
                TextBlock tb = new TextBlock
                {
                    Text = title,
                    Margin = new Thickness(10, 6, 10, 6),
                    VerticalAlignment = VerticalAlignment.Center,
                    TextWrapping = TextWrapping.NoWrap,
                };
                Grid.SetColumnSpan(tb, 2);
                root.Children.Insert(0, tb);
            }
            else if (string.IsNullOrEmpty(title))
            {
                // 只内容
                var con = Content as FrameworkElement;
                CFree.ApplyCellStyle(con);
                // 左上空出边线
                var margin = con.Margin;
                con.Margin = new Thickness(margin.Left + 1, margin.Top + 1, margin.Right, margin.Bottom);
                Grid.SetColumnSpan(con, 2);
                root.Children.Insert(0, con);
            }
            else
            {
                // 标题 + 内容
                title = _prefix + title.Replace("\n", "\n" + _prefix);
                TextBlock tb = new TextBlock
                {
                    Text = title,
                    Margin = new Thickness(10, 6, 10, 6),
                    VerticalAlignment = VerticalAlignment.Center,
                    TextWrapping = TextWrapping.NoWrap,
                };
                root.Children.Insert(0, tb);

                var con = Content as FrameworkElement;
                CFree.ApplyCellStyle(con);
                // 左上空出边线
                var margin = con.Margin;
                con.Margin = new Thickness(margin.Left + 1, margin.Top + 1, margin.Right, margin.Bottom);
                Grid.SetColumn(con, 1);
                root.Children.Insert(1, con);
            }

            if (this.GetParent() is not FormPanel)
            {
                // 独立使用时右下边框
                Margin = new Thickness(0, 0, 1, 1);
            }
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            if (Owner != null && e.GetCurrentPoint(null).Properties.IsLeftButtonPressed)
            {
                Owner.OnCellClick(this, e);
            }
        }
    }
}