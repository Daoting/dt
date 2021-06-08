#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-09-09 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.Sketches;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Sketch : Control
    {
        #region 静态内容
        /// <summary>
        /// 是否显示水平尺子
        /// </summary>
        public readonly static DependencyProperty ShowHorRulerProperty = DependencyProperty.Register(
            "ShowHorRuler",
            typeof(bool),
            typeof(Sketch),
            new PropertyMetadata(false));

        /// <summary>
        /// 是否显示垂直尺子
        /// </summary>
        public readonly static DependencyProperty ShowVerRulerProperty = DependencyProperty.Register(
            "ShowVerRuler",
            typeof(bool),
            typeof(Sketch),
            new PropertyMetadata(false));

        /// <summary>
        /// 是否显示网格，默认false
        /// </summary>
        public readonly static DependencyProperty ShowGridLineProperty = DependencyProperty.Register(
            "ShowGridLine",
            typeof(bool),
            typeof(Sketch),
            new PropertyMetadata(false));

        /// <summary>
        /// 是否只读
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
            "IsReadOnly",
            typeof(bool),
            typeof(Sketch),
            new PropertyMetadata(true, OnIsReadOnlyChanged));

        /// <summary>
        /// 页面宽度
        /// </summary>
        public readonly static DependencyProperty PageWidthProperty = DependencyProperty.Register(
            "PageWidth",
            typeof(double),
            typeof(Sketch),
            new PropertyMetadata(0.0d));

        /// <summary>
        /// 页面高度
        /// </summary>
        public readonly static DependencyProperty PageHeightProperty = DependencyProperty.Register(
            "PageHeight",
            typeof(double),
            typeof(Sketch),
            new PropertyMetadata(0.0d));

        /// <summary>
        /// 编辑时图元是否与网格对齐
        /// </summary>
        public static readonly DependencyProperty AlignGridProperty = DependencyProperty.Register(
            "AlignGrid",
            typeof(bool),
            typeof(Sketch),
            new PropertyMetadata(true));

        static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Sketch sk = (Sketch)d;
            if ((bool)e.NewValue)
                sk._inputClerk.DetachEvent();
            else
                sk._inputClerk.AttachEvent();
        }
        #endregion

        #region 合并rect
        static Rect Union(Rect p_src, Rect rect)
        {
            if (p_src.IsEmpty)
            {
                p_src = rect;
            }
            else if (!rect.IsEmpty)
            {
                double left = Math.Min(p_src.Left, rect.Left);
                double top = Math.Min(p_src.Top, rect.Top);


                // We need this check so that the math does not result in NaN
                if ((rect.Width == Double.PositiveInfinity) || (p_src.Width == Double.PositiveInfinity))
                {
                    p_src.Width = Double.PositiveInfinity;
                }
                else
                {
                    //  Max with 0 to prevent double weirdness from causing us to be (-epsilon..0)
                    double maxRight = Math.Max(p_src.Right, rect.Right);
                    p_src.Width = Math.Max(maxRight - left, 0);
                }

                // We need this check so that the math does not result in NaN
                if ((rect.Height == Double.PositiveInfinity) || (p_src.Height == Double.PositiveInfinity))
                {
                    p_src.Height = Double.PositiveInfinity;
                }
                else
                {
                    //  Max with 0 to prevent double weirdness from causing us to be (-epsilon..0)
                    double maxBottom = Math.Max(p_src.Bottom, rect.Bottom);
                    p_src.Height = Math.Max(maxBottom - top, 0);
                }

                p_src.X = left;
                p_src.Y = top;
            }
            return p_src;
        }
        #endregion

        #region 成员变量
        readonly Canvas _container;
        Line _topLine;
        Line _leftLine;
        Line _rightLine;
        Line _bottomLine;
        LinkPrompt _linkPrompt;
        ScrollViewer _scroll;

        SketchInputManager _inputClerk;
        SelectionManager _selectionClerk;
        SketchLinkManager _linkClerk;

        CmdHistory _his;
        UndoCmd _cmdUndo;
        RedoCmd _cmdRedo;
        SketchInsertCmd _cmdInsert;
        SketchDeleteCmd _cmdDelete;
        HisPropertyCmd _cmdPropHis;
        SketchResizeCmd _cmdResize;
        SketchMoveCmd _cmdMove;
        BaseCommand _cmdDeleteSelection;
        SketchMoveLineCmd _cmdMoveLine;

        const double _pageBorderBlank = 20;
        const double _pageRuleSize = 25.1;
        #endregion

        #region 构造方法
        public Sketch()
        {
            DefaultStyleKey = typeof(Sketch);

            _container = new Canvas();
            _inputClerk = new SketchInputManager(this);
            _selectionClerk = new SelectionManager(this);
            _linkClerk = new SketchLinkManager(this);

            // 在andriod中，点击节点事件会无限触发SizeChanged事件。
            SizeChanged += OnSizeChanged;
        }
        #endregion

        #region 事件
        /// <summary>
        /// 添加项事件
        /// </summary>
        public event EventHandler<List<FrameworkElement>> Added;

        /// <summary>
        /// 删除后事件
        /// </summary>
        public event EventHandler<List<FrameworkElement>> Deleted;

        /// <summary>
        /// Tap事件
        /// </summary>
        new public event TappedEventHandler Tapped;
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置是否显示水平尺子，默认false
        /// </summary>
        public bool ShowHorRuler
        {
            get { return (bool)GetValue(ShowHorRulerProperty); }
            set { SetValue(ShowHorRulerProperty, value); }
        }

        /// <summary>
        /// 获取设置是否显示垂直尺子，默认false
        /// </summary>
        public bool ShowVerRuler
        {
            get { return (bool)GetValue(ShowVerRulerProperty); }
            set { SetValue(ShowVerRulerProperty, value); }
        }

        /// <summary>
        /// 获取设置是否显示网格，默认false
        /// </summary>
        public bool ShowGridLine
        {
            get { return (bool)GetValue(ShowGridLineProperty); }
            set { SetValue(ShowGridLineProperty, value); }
        }

        /// <summary>
        /// 获取设置是否只读，默认true，只读时节点可点击无选择状态，非只读主要提供给流程图绘制用
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// 获取设置页面宽度
        /// </summary>
        public double PageWidth
        {
            get { return (double)GetValue(PageWidthProperty); }
            set { SetValue(PageWidthProperty, value); }
        }

        /// <summary>
        /// 获取设置页面高度
        /// </summary>
        public double PageHeight
        {
            get { return (double)GetValue(PageHeightProperty); }
            set { SetValue(PageHeightProperty, value); }
        }

        /// <summary>
        /// 获取设置编辑时图元是否与网格对齐
        /// </summary>
        public bool AlignGrid
        {
            get { return (bool)GetValue(AlignGridProperty); }
            set { SetValue(AlignGridProperty, value); }
        }

        /// <summary>
        /// 获取当前选择元素列表，不包含选中的连线
        /// </summary>
        public List<FrameworkElement> SelectedNodes
        {
            get { return _selectionClerk.Selection; }
        }

        /// <summary>
        /// 获取当前选中的连线
        /// </summary>
        public SLine SelectedLine
        {
            get { return _selectionClerk.SelectedLine; }
        }

        public Canvas Container
        {
            get { return _container; }
        }
        #endregion

        #region 命令对象
        /// <summary>
        /// 历史命令
        /// </summary>
        public CmdHistory His
        {
            get
            {
                if (_his == null)
                    _his = new CmdHistory();
                return _his;
            }
        }

        /// <summary>
        /// 获取撤消命令
        /// </summary>
        public UndoCmd CmdUndo
        {
            get
            {
                if (_cmdUndo == null)
                    _cmdUndo = new UndoCmd(His);
                return _cmdUndo;
            }
        }

        /// <summary>
        /// 获取重做命令
        /// </summary>
        public RedoCmd CmdRedo
        {
            get
            {
                if (_cmdRedo == null)
                    _cmdRedo = new RedoCmd(His);
                return _cmdRedo;
            }
        }

        /// <summary>
        /// 获取删除所有选择项命令，提供给界面绑定
        /// </summary>
        public BaseCommand CmdDeleteSelection
        {
            get
            {
                if (_cmdDeleteSelection == null)
                {
                    _cmdDeleteSelection = new BaseCommand((p_params) => { DeleteSelection(); });
                    OnSelectionChanged();
                }
                return _cmdDeleteSelection;
            }
        }

        /// <summary>
        /// 获取依赖属性历史命令
        /// </summary>
        public HisPropertyCmd CmdPropHis
        {
            get
            {
                if (_cmdPropHis == null)
                    _cmdPropHis = new HisPropertyCmd(His);
                return _cmdPropHis;
            }
        }

        /// <summary>
        /// 获取Resize命令
        /// </summary>
        public SketchResizeCmd CmdResize
        {
            get
            {
                if (_cmdResize == null)
                    _cmdResize = new SketchResizeCmd(this, His);
                return _cmdResize;
            }
        }

        /// <summary>
        /// 获取移动命令
        /// </summary>
        public SketchMoveCmd CmdMove
        {
            get
            {
                if (_cmdMove == null)
                    _cmdMove = new SketchMoveCmd(this, His);
                return _cmdMove;
            }
        }

        /// <summary>
        /// 获取移动连线命令
        /// </summary>
        public SketchMoveLineCmd CmdMoveLine
        {
            get
            {
                if (_cmdMoveLine == null)
                    _cmdMoveLine = new SketchMoveLineCmd(this, His);
                return _cmdMoveLine;
            }
        }
        #endregion

        #region 内部属性

        internal SelectionManager SelectionClerk
        {
            get { return _selectionClerk; }
        }

        internal SketchLinkManager LinkClerk
        {
            get { return _linkClerk; }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 插入新项
        /// </summary>
        /// <param name="p_elem"></param>
        public void Insert(FrameworkElement p_elem)
        {
            if (p_elem != null)
            {
                if (_cmdInsert == null)
                    _cmdInsert = new SketchInsertCmd(this, His);
                _cmdInsert.Execute(p_elem);
            }
        }

        /// <summary>
        /// 删除所有选择项
        /// </summary>
        public void DeleteSelection()
        {
            if (_selectionClerk.SelectedLine == null && _selectionClerk.Selection.Count == 0)
                return;

            if (_cmdDelete == null)
                _cmdDelete = new SketchDeleteCmd(this, His);

            // 整理删除项
            List<FrameworkElement> items = new List<FrameworkElement>();
            if (_selectionClerk.SelectedLine != null)
                items.Add(_selectionClerk.SelectedLine);
            if (_selectionClerk.Selection.Count > 0)
            {
                foreach (var item in _selectionClerk.Selection)
                {
                    items.Add(item);
                    if (item is SNode node)
                    {
                        var lines = from obj in _container.Children
                                    let line = obj as SLine
                                    where line != null && (line.HeaderID == node.ID || line.TailID == node.ID)
                                    select line;
                        foreach (var line in lines)
                        {
                            if (!items.Contains(line))
                                items.Add(line);
                        }
                    }
                }
            }
            _cmdDelete.Execute(items);
        }

        /// <summary>
        /// 设置节点的位置，此位置未考虑尺子及对齐到网格
        /// </summary>
        /// <param name="p_elem"></param>
        /// <param name="p_pos"></param>
        public void SetNodePos(FrameworkElement p_elem, Point p_pos)
        {
            double left = p_pos.X;
            double top = p_pos.Y;
            if (ShowHorRuler)
                top -= 25;
            if (ShowVerRuler)
                left -= 25;

            if (AlignGrid)
            {
                left = (left % 20 > 10) ? Math.Ceiling(left / 20) * 20 : Math.Floor(left / 20) * 20;
                top = (top % 20 > 10) ? Math.Ceiling(top / 20) * 20 : Math.Floor(top / 20) * 20;
            }
            Canvas.SetLeft(p_elem, left);
            Canvas.SetTop(p_elem, top);
        }

        /// <summary>
        /// 查询指定点所在的图元
        /// </summary>
        /// <param name="p_pt"></param>
        /// <returns></returns>
        internal FrameworkElement GetItemByPosition(Point p_pt)
        {
            foreach (object obj in _container.Children)
            {
                var item = obj as FrameworkElement;
                if (item != null && !(item is SLine))
                {
                    double left = Canvas.GetLeft(item);
                    double top = Canvas.GetTop(item);
                    if (p_pt.X >= left
                        && p_pt.X <= left + item.ActualWidth
                        && p_pt.Y >= top
                        && p_pt.Y <= top + item.ActualHeight)
                        return item;
                }
            }
            return null;
        }

        /// <summary>
        /// 指定区域包含的所有图元
        /// </summary>
        /// <param name="p_left"></param>
        /// <param name="p_top"></param>
        /// <param name="p_right"></param>
        /// <param name="p_bottom"></param>
        /// <returns></returns>
        internal List<FrameworkElement> GetItemsByRegion(double p_left, double p_top, double p_right, double p_bottom)
        {
            List<FrameworkElement> ls = new List<FrameworkElement>();
            foreach (object obj in _container.Children)
            {
                var item = obj as FrameworkElement;
                if (item != null && !(item is SLine))
                {
                    double left = Canvas.GetLeft(item);
                    double top = Canvas.GetTop(item);
                    double right = left + item.ActualWidth;
                    double bottom = top + item.ActualHeight;
                    if (left >= p_left
                        && left <= p_right
                        && right >= p_left
                        && right <= p_right
                        && top >= p_top
                        && top <= p_bottom
                        && bottom >= p_top
                        && bottom <= p_bottom)
                        ls.Add(item);
                }
            }
            return ls;
        }

        /// <summary>
        /// 判定传入的区域是否在sketch的对象的有效区域
        /// </summary>
        /// <param name="p_rect"></param>
        /// <returns></returns>
        internal bool IsValidRegion(Rect p_rect)
        {
            foreach (object obj in _container.Children)
            {
                var item = obj as FrameworkElement;
                if (item != null && !(item is SLine))
                {
                    Rect itemRect = new Rect(Canvas.GetLeft(item), Canvas.GetTop(item), item.ActualWidth, item.ActualHeight);
                    if (RectHelper.Intersect(itemRect, p_rect) != RectHelper.Empty)
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获得最上面的相交对象
        /// </summary>
        /// <param name="p_rect"></param>
        /// <returns></returns>
        internal SNode GetFirstIntersect(Rect p_rect)
        {
            return (from obj in _container.Children
                    let item = obj as SNode
                    where item != null
                    && RectHelper.Intersect(new Rect(Canvas.GetLeft(item), Canvas.GetTop(item), item.ActualWidth, item.ActualHeight), p_rect) != RectHelper.Empty
                    orderby Canvas.GetZIndex(item) descending
                    select item).FirstOrDefault<SNode>();
        }

        /// <summary>
        /// 获得指定对象的链接位置
        /// 注意：使用条件为：p_target 不为空，并且 P_target 的矩形与 p_rect 相交。
        /// </summary>
        /// <returns></returns>
        internal LinkPortPosition GetLinkPosition(FrameworkElement p_item, Rect p_rect)
        {
            Point rectCenter = new Point((p_rect.X + p_rect.Width / 2), (p_rect.Y + p_rect.Height / 2));
            Rect itemRect = new Rect(Canvas.GetLeft(p_item), Canvas.GetTop(p_item), p_item.ActualWidth, p_item.ActualHeight);
            Point itemCenter = new Point((itemRect.X + itemRect.Width / 2), (itemRect.Y + itemRect.Height / 2));

            if (rectCenter.X <= itemCenter.X)
            {
                if (rectCenter.Y <= itemCenter.Y)
                {
                    // 第四象限
                    if (rectCenter.X > itemRect.X + itemRect.Width / 4)
                        return LinkPortPosition.TopCenter;
                    else
                        if (rectCenter.Y > itemRect.Y + itemRect.Height / 4)
                        return LinkPortPosition.LeftCenter;
                    else
                        return LinkPortPosition.LeftTop;
                }
                else
                {
                    // 第三象限
                    if (rectCenter.X > itemRect.X + itemRect.Width / 4)
                        return LinkPortPosition.BottomCenter;
                    else
                        if (rectCenter.Y > itemRect.Y + itemRect.Height * 0.75)
                        return LinkPortPosition.LeftBottom;
                    else
                        return LinkPortPosition.LeftCenter;
                }
            }
            else
            {
                if (rectCenter.Y > itemCenter.Y)
                {
                    // 第二象限
                    if (rectCenter.X > itemRect.X + itemRect.Width * 0.75)
                        if (rectCenter.Y > itemRect.Y + itemRect.Height * 0.75)
                            return LinkPortPosition.RightBottom;
                        else
                            return LinkPortPosition.RightCenter;
                    else
                        return LinkPortPosition.BottomCenter;
                }
                else
                {
                    // 第一象限
                    if (rectCenter.X > itemRect.X + itemRect.Width * 0.75)
                        if (rectCenter.Y > itemRect.Y + itemRect.Height / 4)
                            return LinkPortPosition.RightCenter;
                        else
                            return LinkPortPosition.RightTop;
                    else
                        return LinkPortPosition.TopCenter;
                }
            }
        }

        /// <summary>
        /// 刷新与指定元素连接的连线
        /// </summary>
        /// <param name="p_elem"></param>
        /// <returns></returns>
        internal void RefreshLinkLines(FrameworkElement p_elem)
        {
            SNode node = p_elem as SNode;
            if (node != null)
            {
                var lines = from obj in _container.Children
                            let item = obj as SLine
                            where item != null && (item.HeaderID == node.ID || item.TailID == node.ID)
                            select item;
                foreach (var line in lines)
                {
                    line.Refresh();
                }
            }
        }

        /// <summary>
        /// 刷新所有连线
        /// </summary>
        /// <returns></returns>
        internal void RefreshAllLines()
        {
            var lines = from obj in _container.Children
                        let item = obj as SLine
                        where item != null
                        select item;
            foreach (var line in lines)
            {
                line.Refresh();
            }
        }

        /// <summary>
        /// 选择变化
        /// </summary>
        internal void OnSelectionChanged()
        {
            if (_cmdDeleteSelection != null)
                _cmdDeleteSelection.AllowExecute = _selectionClerk.SelectedLine != null || _selectionClerk.Selection.Count > 0;
        }

        /// <summary>
        /// 计算非只读下页面大小；
        /// 如果元素都在当前页面则页面大小为可视区域，否则为最右侧、最下方的值
        /// </summary>
        internal void CalcPageSize()
        {
            ResizeEditPage(CalcRect());
        }
        #endregion

        #region Xml
        /// <summary>
        /// 加载xml字符串
        /// </summary>
        /// <param name="p_xml"></param>
        public void ReadXml(string p_xml)
        {
            _his?.Clear();
            _container.Children.Clear();
            _selectionClerk.Clear();

            if (string.IsNullOrEmpty(p_xml))
                return;

            using (StringReader stream = new StringReader(p_xml))
            using (XmlReader reader = XmlReader.Create(stream, new XmlReaderSettings() { IgnoreWhitespace = true, IgnoreComments = true, IgnoreProcessingInstructions = true }))
            {
                reader.Read();

                Rect totalRect = new Rect();
                bool isFirst = true;
                reader.Read();
                while (reader.NodeType != XmlNodeType.None)
                {
                    if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Sketch")
                        break;

                    Rect rc = new Rect();
                    string name = reader.Name;
                    if (name == "Node")
                    {
                        SNode node = new SNode(this);
                        _container.Children.Add(node);
                        node.ReadXml(reader);
                        rc = new Rect(Canvas.GetLeft(node), Canvas.GetTop(node), node.Width, node.Height);
                    }
                    else if (name == "Line")
                    {
                        SLine line = new SLine(this);
                        _container.Children.Add(line);
                        line.ReadXml(reader);
                        rc = line.Bounds;
                    }
                    else if (name == "Txt")
                    {
                        TextBlock tb = new TextBlock();
                        _container.Children.Add(tb);
                        for (int i = 0; i < reader.AttributeCount; i++)
                        {
                            reader.MoveToAttribute(i);
                            switch (reader.Name)
                            {
                                case "text": tb.Text = reader.Value; break;
                                case "fontsize": tb.FontSize = double.Parse(reader.Value); break;
                                // uwp和手机系统weight的操作方式不同
#if UWP
                                case "fontweight": tb.FontWeight = new FontWeight() { Weight = ushort.Parse(reader.Value) }; break;
#else
                             case "fontweight": tb.FontWeight = new FontWeight(ushort.Parse(reader.Value));break;
#endif
                                case "foreground": tb.Foreground = new SolidColorBrush(Res.HexStringToColor(reader.Value)); break;
                                case "fontstyle": tb.FontStyle = (FontStyle)int.Parse(reader.Value); break;
                                case "fontfamily": tb.FontFamily = new FontFamily(reader.Value); break;
                                case "left": Canvas.SetLeft(tb, double.Parse(reader.Value)); break;
                                case "top": Canvas.SetTop(tb, double.Parse(reader.Value)); break;
                                case "width": tb.Width = double.Parse(reader.Value); break;
                                case "height": tb.Height = double.Parse(reader.Value); break;
                            }
                        }
                        rc = new Rect(Canvas.GetLeft(tb), Canvas.GetTop(tb), tb.Width, tb.Height);
                    }

                    if (isFirst)
                    {
                        totalRect = rc;
                        isFirst = false;
                    }
                    else
                    {
                        totalRect = Union(totalRect, rc);
                    }

                    reader.Read();
                }

                if (IsReadOnly)
                    ResizeReadOnlyPage(totalRect);
                else
                    ResizeEditPage(totalRect);
            }
            RefreshAllLines();
        }

        /// <summary>
        /// 序列化xml
        /// </summary>
        /// <returns></returns>
        public string WriteXml()
        {
            StringBuilder sb = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(sb, new XmlWriterSettings() { OmitXmlDeclaration = true }))
            {
                writer.WriteStartElement("Sketch");
                SNode node;
                SLine line;
                TextBlock tb;
                foreach (object obj in _container.Children)
                {
                    if ((node = obj as SNode) != null)
                    {
                        node.WriteXml(writer);
                    }
                    else if ((line = obj as SLine) != null)
                    {
                        line.WriteXml(writer);
                    }
                    else if ((tb = obj as TextBlock) != null)
                    {
                        writer.WriteStartElement("Txt");
                        writer.WriteAttributeString("text", tb.Text);
                        if (tb.FontSize.ToString("0.000") != "18.667")
                        {
                            writer.WriteAttributeString("fontsize", tb.FontSize.ToString());
                        }
                        if (tb.FontWeight.Weight != 400)
                        {
                            writer.WriteAttributeString("fontweight", tb.FontWeight.Weight.ToString());
                        }
                        if ((tb.Foreground as SolidColorBrush).Color != Res.默认前景.Color)
                        {
                            writer.WriteAttributeString("foreground", (tb.Foreground as SolidColorBrush).Color.ToString());
                        }
                        if (tb.FontStyle != 0)
                        {
                            writer.WriteAttributeString("fontstyle", ((int)tb.FontStyle).ToString());
                        }
                        if (tb.FontFamily.Source != "Microsoft YaHei")
                        {
                            writer.WriteAttributeString("fontfamily", tb.FontFamily.Source);
                        }
                        writer.WriteAttributeString("left", Canvas.GetLeft(tb).ToString());
                        writer.WriteAttributeString("top", Canvas.GetTop(tb).ToString());
                        if (!double.IsNaN(tb.Width))
                        {
                            writer.WriteAttributeString("width", tb.Width.ToString());
                        }
                        if (!double.IsNaN(tb.Height))
                        {
                            writer.WriteAttributeString("height", tb.Height.ToString());
                        }
                        writer.WriteEndElement();
                    }
                }
                writer.Flush();
            }
            return sb.ToString();
        }
        #endregion

        #region 提示线
        /// <summary>
        /// 显示顶部提示线
        /// </summary>
        /// <param name="p_y"></param>
        internal void ShowTopLine(double p_y)
        {
            _topLine.Y1 = _topLine.Y2 = p_y;
            _topLine.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 显示底部提示线
        /// </summary>
        /// <param name="p_y"></param>
        internal void ShowBottomLine(double p_y)
        {
            _bottomLine.Y1 = _bottomLine.Y2 = p_y;
            _bottomLine.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 显示左侧提示线
        /// </summary>
        /// <param name="p_x"></param>
        internal void ShowLeftLine(double p_x)
        {
            _leftLine.X1 = _leftLine.X2 = p_x;
            _leftLine.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 显示右侧提示线
        /// </summary>
        /// <param name="p_x"></param>
        internal void ShowRightLine(double p_x)
        {
            _rightLine.X1 = _rightLine.X2 = p_x;
            _rightLine.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 显示所有提示线
        /// </summary>
        internal void ShowTipLines()
        {
            _leftLine.Visibility = Visibility.Visible;
            _topLine.Visibility = Visibility.Visible;
            _rightLine.Visibility = Visibility.Visible;
            _bottomLine.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 定位提示线
        /// </summary>
        /// <param name="p_left"></param>
        /// <param name="p_top"></param>
        /// <param name="p_right"></param>
        /// <param name="p_bottom"></param>
        internal void MoveTipLines(double p_left, double p_top, double p_right, double p_bottom)
        {
            _leftLine.X1 = _leftLine.X2 = p_left;
            _topLine.Y1 = _topLine.Y2 = p_top;
            _rightLine.X1 = _rightLine.X2 = p_right;
            _bottomLine.Y1 = _bottomLine.Y2 = p_bottom;
        }

        /// <summary>
        /// 隐藏所有提示线
        /// </summary>
        internal void HideTipLines()
        {
            _leftLine.Visibility = Visibility.Collapsed;
            _topLine.Visibility = Visibility.Collapsed;
            _rightLine.Visibility = Visibility.Collapsed;
            _bottomLine.Visibility = Visibility.Collapsed;
        }
        #endregion

        #region 连线相关
        /// <summary>
        /// 拖拽（连接点公共部分处理）
        /// </summary>
        /// <param name="p_thumb">点实例</param>
        internal bool LinkThumbMove(Thumb p_thumb)
        {
            Rect rect = new Rect(Canvas.GetLeft(p_thumb), Canvas.GetTop(p_thumb), p_thumb.ActualWidth, p_thumb.ActualHeight);
            SNode taget = GetFirstIntersect(rect);
            if (taget != null)
            {
                VisualStateManager.GoToState(p_thumb, "DragValid", true);
                //设置目标
                _linkPrompt.Width = taget.Width;
                _linkPrompt.Height = taget.Height;
                Canvas.SetLeft(_linkPrompt, Canvas.GetLeft(taget));
                Canvas.SetTop(_linkPrompt, Canvas.GetTop(taget));
                _linkPrompt.Visibility = Visibility.Visible;
                _linkPrompt.SetValidPos(GetLinkPosition(taget, rect));
                return true;
            }
            else
            {
                _linkPrompt.Visibility = Visibility.Collapsed;
                return false;
            }
        }

        /// <summary>
        /// 拖拽结束
        /// </summary>
        internal void LinkThumbEnd()
        {
            _linkPrompt.Visibility = Visibility.Collapsed;
        }
        #endregion

        #region 重写方法
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _leftLine = GetTemplateChild("Part_LeftLine") as Line;
            _topLine = GetTemplateChild("Part_TopLine") as Line;
            _rightLine = GetTemplateChild("Part_RightLine") as Line;
            _bottomLine = GetTemplateChild("Part_BottomLine") as Line;
            _linkPrompt = GetTemplateChild("Part_LinkPrompt") as LinkPrompt;
            _scroll = GetTemplateChild("Part_ScrollViewer") as ScrollViewer;

            Grid grid = GetTemplateChild("Part_Grid") as Grid;
            grid.Children.Insert(grid.Children.Count - 1, _container);
            Rectangle tmpRec = GetTemplateChild("Part_MouseRect") as Rectangle;
            _inputClerk.Init(
                grid,
                tmpRec);
            if (!IsReadOnly)
                _inputClerk.AttachEvent();
            grid.Tapped -= OnTappedEvent;
            grid.Tapped += OnTappedEvent;

            _linkClerk.Init(
                GetTemplateChild("PART_LinkThumb") as Thumb,
                GetTemplateChild("Part_LinkLine") as Line);

            var nodeSel = GetTemplateChild("Part_NodeSelector") as NodeSelector;
            nodeSel.Owner = this;
            _selectionClerk.Init(nodeSel, GetTemplateChild("Part_SelRect") as Rectangle);
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 计算整个图形大小
        /// </summary>
        /// <returns></returns>
        Rect CalcRect()
        {
            if (_container.Children.Count == 0)
                return new Rect();

            Rect totalRect = new Rect();
            bool isFirst = true;
            SLine line;
            FrameworkElement elem;
            foreach (object item in _container.Children)
            {
                Rect rc = new Rect();
                if ((line = item as SLine) != null)
                    rc = line.Bounds;
                else if ((elem = item as FrameworkElement) != null)
                    rc = new Rect(Canvas.GetLeft(elem), Canvas.GetTop(elem), elem.Width, elem.Height);

                if (isFirst)
                {
                    totalRect = rc;
                    isFirst = false;
                }
                else
                {
                    totalRect = Union(totalRect, rc);
                }
            }
            return totalRect;
        }

        /// <summary>
        /// 重置编辑页面大小
        /// </summary>
        /// <param name="p_totalRect"></param>
        void ResizeEditPage(Rect p_totalRect)
        {
            PageWidth = p_totalRect.Right > ActualWidth - _pageRuleSize ? p_totalRect.Right + _pageBorderBlank * 3 : ActualWidth - _pageRuleSize;
            PageHeight = p_totalRect.Bottom > ActualHeight - _pageRuleSize ? p_totalRect.Bottom + _pageBorderBlank * 3 : ActualHeight - _pageRuleSize;
        }

        /// <summary>
        /// 重置浏览页面大小，将图形区域居中
        /// </summary>
        /// <param name="p_totalRect"></param>
        void ResizeReadOnlyPage(Rect p_totalRect)
        {
            // 页宽
            double deltaX;
            double needWidth = p_totalRect.Width + 2 * _pageBorderBlank;
            if (needWidth >= ActualWidth)
            {
                PageWidth = needWidth;
                deltaX = p_totalRect.Left - _pageBorderBlank;
            }
            else
            {
                PageWidth = ActualWidth;
                deltaX = p_totalRect.Left - _pageBorderBlank - (ActualWidth - needWidth) / 2;
            }

            // 页高
            double needHeight = p_totalRect.Height + 2 * _pageBorderBlank;
            double deltaY = p_totalRect.Top - _pageBorderBlank;
            PageHeight = needHeight >= ActualHeight ? needHeight : ActualHeight;

            // 自动将图形区域居中
            SLine line;
            FrameworkElement elem;
            foreach (var obj in _container.Children)
            {
                if ((line = obj as SLine) != null)
                {
                    Rect rect = line.Bounds;
                    rect.X -= deltaX;
                    rect.Y -= deltaY;
                    line.Bounds = rect;
                    line.Refresh();
                }
                else if ((elem = obj as FrameworkElement) != null)
                {
                    Canvas.SetLeft(elem, (Canvas.GetLeft(elem) - deltaX));
                    Canvas.SetTop(elem, (Canvas.GetTop(elem) - deltaY));
                }
            }
        }

        /// <summary>
        /// 滚动至水平居中
        /// </summary>
        void ScrollIntoHorCenter()
        {
            if (_scroll != null)
            {
                if (_scroll.ComputedHorizontalScrollBarVisibility == Visibility.Visible)
                    _scroll.ChangeView(_scroll.ScrollableWidth / 2, _scroll.VerticalOffset, _scroll.ZoomFactor);
            }
        }
        #endregion

        #region 触发事件
        /// <summary>
        /// 触发增加后事件
        /// </summary>
        /// <param name="e"></param>
        internal void OnAdded(List<FrameworkElement> e)
        {
            Added?.Invoke(this, e);
        }

        /// <summary>
        /// 触发删除后事件
        /// </summary>
        /// <param name="e"></param>
        internal void OnDeleted(List<FrameworkElement> e)
        {
            Deleted?.Invoke(this, e);
        }

        void OnTappedEvent(object sender, TappedRoutedEventArgs e)
        {
            if (e.Handled == false)
                Tapped?.Invoke(this, e);
        }

        void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Rect rc = CalcRect();
            if (IsReadOnly)
                ResizeReadOnlyPage(rc);
            else
                ResizeEditPage(rc);
        }
        #endregion

    }

    /// <summary>
    /// ContentPresenter中ContentTemplateSelector
    /// </summary>
    public class ContentPresenterSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item != null && item.GetType() == typeof(string))
            {
                return (DataTemplate)Application.Current.Resources["Presenter文字"];
            }

            return base.SelectTemplateCore(item, container);
        }
    }
}
