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
using System.Diagnostics;
using System.Xml;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using System.Linq;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SLine : Control
    {
        #region 静态内容
        // 线端点图形
        static PathGeometry _arrowData;
        static PathGeometry _diamondData;
        static PathGeometry _circleData;

        static PathGeometry ArrowData
        {
            get
            {
                if (_arrowData == null)
                    _arrowData = Res.ParseGeometry("M0,0 8,4 0,8 Z");
                return _arrowData;
            }
        }

        static PathGeometry DiamondData
        {
            get
            {
                if (_diamondData == null)
                    _diamondData = Res.ParseGeometry("M-5,0 0,-5 5,0 0,5 Z");
                return _diamondData;
            }
        }

        static PathGeometry CircleData
        {
            get
            {
                if (_circleData == null)
                    _circleData = Res.ParseGeometry("M5,3C5,4.10456949966159,4.10456949966159,5,3,5C1.89543050033841,5,1,4.10456949966159,1,3C1,1.89543050033841,1.89543050033841,1,3,1C4.10456949966159,1,5,1.89543050033841,5,3z");
                return _circleData;
            }
        }

        /// <summary>
        /// 起始连接点位置
        /// </summary>
        public readonly static DependencyProperty HeaderPortProperty = DependencyProperty.Register(
            "HeaderPort",
            typeof(LinkPortPosition),
            typeof(SLine),
            new PropertyMetadata(LinkPortPosition.RightCenter, OnLinkPathChanged));

        /// <summary>
        /// 结束连接点位置
        /// </summary>
        public readonly static DependencyProperty TailPortProperty = DependencyProperty.Register(
            "TailPort",
            typeof(LinkPortPosition),
            typeof(SLine),
            new PropertyMetadata(LinkPortPosition.LeftCenter, OnLinkPathChanged));

        /// <summary>
        /// 起始端点类型
        /// </summary>
        public readonly static DependencyProperty HeaderShapeProperty = DependencyProperty.Register(
            "HeaderShape",
            typeof(LinkPortShapes),
            typeof(SLine),
            new PropertyMetadata(LinkPortShapes.None, OnHeadShapeChanged));

        /// <summary>
        /// 结束端点类型
        /// </summary>
        public readonly static DependencyProperty TailShapeProperty = DependencyProperty.Register(
            "TailShape",
            typeof(LinkPortShapes),
            typeof(SLine),
            new PropertyMetadata(LinkPortShapes.Arrow, OnTailShapeChanged));

        /// <summary>
        /// 端点大小
        /// </summary>
        public readonly static DependencyProperty HeadTailSizeProperty = DependencyProperty.Register(
            "HeadTailSize",
            typeof(double),
            typeof(SLine),
            new PropertyMetadata(20.0, OnLinkPathChanged));

        /// <summary>
        /// 连线的粗细
        /// </summary>
        public readonly static DependencyProperty LineThicknessProperty = DependencyProperty.Register(
            "LineThickness",
            typeof(double),
            typeof(SLine),
            new PropertyMetadata(4.0));

        /// <summary>
        /// 是否为直线连接
        /// </summary>
        public readonly static DependencyProperty IsStraightLineProperty = DependencyProperty.Register(
            "IsStraightLine",
            typeof(bool),
            typeof(SLine),
            new PropertyMetadata(false, OnLinkPathChanged));

        /// <summary>
        /// 标题文字
        /// </summary>
        public readonly static DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(SLine),
            null);

        /// <summary>
        /// 连线的几何图形
        /// </summary>
        public readonly static DependencyProperty LinkPathGeometryProperty = DependencyProperty.Register(
            "LinkPathGeometry",
            typeof(PathGeometry),
            typeof(SLine),
            null);

        /// <summary>
        /// 连线头几何图形
        /// </summary>
        public readonly static DependencyProperty HeaderGeometryProperty = DependencyProperty.Register(
            "HeaderGeometry",
            typeof(PathGeometry),
            typeof(SLine),
            null);

        /// <summary>
        /// 连线尾几何图形
        /// </summary>
        public readonly static DependencyProperty TailGeometryProperty = DependencyProperty.Register(
            "TailGeometry",
            typeof(PathGeometry),
            typeof(SLine),
            null);

        /// <summary>
        /// 连线头位置
        /// </summary>
        public readonly static DependencyProperty HeaderLeftProperty = DependencyProperty.Register(
            "HeaderLeft",
            typeof(double),
            typeof(SLine),
            new PropertyMetadata(0.0));

        /// <summary>
        /// 连线头位置
        /// </summary>
        public readonly static DependencyProperty HeaderTopProperty = DependencyProperty.Register(
            "HeaderTop",
            typeof(double),
            typeof(SLine),
            new PropertyMetadata(0.0));

        /// <summary>
        /// 连线尾位置
        /// </summary>
        public readonly static DependencyProperty TailLeftProperty = DependencyProperty.Register(
            "TailLeft",
            typeof(double),
            typeof(SLine),
            new PropertyMetadata(0.0));

        /// <summary>
        /// 连线尾位置
        /// </summary>
        public readonly static DependencyProperty TailTopProperty = DependencyProperty.Register(
            "TailTop",
            typeof(double),
            typeof(SLine),
            new PropertyMetadata(0.0));

        /// <summary>
        /// 标题文字的位置
        /// </summary>
        public readonly static DependencyProperty LabelLeftProperty = DependencyProperty.Register(
            "LabelLeft",
            typeof(double),
            typeof(SLine),
            new PropertyMetadata(0.0));

        /// <summary>
        /// 标题文字的位置
        /// </summary>
        public readonly static DependencyProperty LabelTopProperty = DependencyProperty.Register(
            "LabelTop",
            typeof(double),
            typeof(SLine),
            new PropertyMetadata(0.0));

        /// <summary>
        /// 标题文字的旋转角度
        /// </summary>
        public readonly static DependencyProperty LabelAngleProperty = DependencyProperty.Register(
            "LabelAngle",
            typeof(double),
            typeof(SLine),
            new PropertyMetadata(0.0));

        /// <summary>
        /// 是否显示调节器
        /// </summary>
        public readonly static DependencyProperty ThumbVisibilityProperty = DependencyProperty.Register(
            "ThumbVisibility",
            typeof(Visibility),
            typeof(SLine),
            new PropertyMetadata(Visibility.Collapsed));

        static void OnLinkPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SLine line = (SLine)d;
            line.Refresh();
        }

        static void OnHeadShapeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SLine line = (SLine)d;
            line.UpdateShape(line.HeaderShape, true);
        }

        static void OnTailShapeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SLine line = (SLine)d;
            line.UpdateShape(line.TailShape, false);
        }
        #endregion

        #region 成员变量
        const double _lineLength = 150;

        readonly Sketch _owner;
        long _headerID;
        long _tailID;
        SNode _headerNode;
        SNode _tailNode;
        Thumb _headThumb;
        Thumb _tailThumb;
        TextBlock _label;
        bool _isInit;
        CompositeTransform _headPathTransform = new CompositeTransform();
        CompositeTransform _tailPathTransform = new CompositeTransform();
        Rect _bounds;
        #endregion

        #region 构造方法
        public SLine(Sketch p_owner)
        {
            DefaultStyleKey = typeof(SLine);
            _owner = p_owner;
            Loaded += OnLoaded;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取设置连线起始节点的标识
        /// </summary>
        public long HeaderID
        {
            get { return _headerID; }
            set
            {
                if (_headerID != value)
                {
                    _headerID = value;
                    if (_isInit)
                    {
                        UpdateHeader();
                        Refresh();
                    }
                }
            }
        }

        /// <summary>
        /// 获取设置起始连接点位置
        /// </summary>
        public LinkPortPosition HeaderPort
        {
            get { return (LinkPortPosition)GetValue(HeaderPortProperty); }
            set { SetValue(HeaderPortProperty, value); }
        }

        /// <summary>
        /// 获取设置起始端点类型
        /// </summary>
        public LinkPortShapes HeaderShape
        {
            get { return (LinkPortShapes)GetValue(HeaderShapeProperty); }
            set { SetValue(HeaderShapeProperty, value); }
        }

        /// <summary>
        /// 获取设置连线结束节点的标识
        /// </summary>
        public long TailID
        {
            get { return _tailID; }
            set
            {
                if (_tailID != value)
                {
                    _tailID = value;
                    if (_isInit)
                    {
                        UpdateTail();
                        Refresh();
                    }
                }
            }
        }

        /// <summary>
        /// 获取设置结束连接点位置
        /// </summary>
        public LinkPortPosition TailPort
        {
            get { return (LinkPortPosition)GetValue(TailPortProperty); }
            set { SetValue(TailPortProperty, value); }
        }

        /// <summary>
        /// 获取设置结束端点类型
        /// </summary>
        public LinkPortShapes TailShape
        {
            get { return (LinkPortShapes)GetValue(TailShapeProperty); }
            set { SetValue(TailShapeProperty, value); }
        }

        /// <summary>
        /// 获取设置流程图中的迁移标识
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// 获取设置端点大小，默认20
        /// </summary>
        public double HeadTailSize
        {
            get { return (double)GetValue(HeadTailSizeProperty); }
            set { SetValue(HeadTailSizeProperty, value); }
        }

        /// <summary>
        /// 获取设置连线的粗细，默认4
        /// </summary>
        public double LineThickness
        {
            get { return (double)GetValue(LineThicknessProperty); }
            set { SetValue(LineThicknessProperty, value); }
        }

        /// <summary>
        /// 获取设置是否为直线连接
        /// </summary>
        public bool IsStraightLine
        {
            get { return (bool)GetValue(IsStraightLineProperty); }
            set { SetValue(IsStraightLineProperty, value); }
        }

        /// <summary>
        /// 获取设置标题文字
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// 获取设置连线的几何图形
        /// </summary>
        public PathGeometry LinkPathGeometry
        {
            get { return (PathGeometry)GetValue(LinkPathGeometryProperty); }
            set { SetValue(LinkPathGeometryProperty, value); }
        }

        /// <summary>
        /// 获取设置连线头几何图形
        /// </summary>
        public PathGeometry HeaderGeometry
        {
            get { return (PathGeometry)GetValue(HeaderGeometryProperty); }
            set { SetValue(HeaderGeometryProperty, value); }
        }

        /// <summary>
        /// 获取设置连线尾几何图形
        /// </summary>
        public PathGeometry TailGeometry
        {
            get { return (PathGeometry)GetValue(TailGeometryProperty); }
            set { SetValue(TailGeometryProperty, value); }
        }

        /// <summary>
        /// 获取设置连线头位置
        /// </summary>
        public double HeaderLeft
        {
            get { return (double)GetValue(HeaderLeftProperty); }
            set { SetValue(HeaderLeftProperty, value); }
        }

        /// <summary>
        /// 获取设置连线头位置
        /// </summary>
        public double HeaderTop
        {
            get { return (double)GetValue(HeaderTopProperty); }
            set { SetValue(HeaderTopProperty, value); }
        }

        /// <summary>
        /// 获取设置连线尾位置
        /// </summary>
        public double TailLeft
        {
            get { return (double)GetValue(TailLeftProperty); }
            set { SetValue(TailLeftProperty, value); }
        }

        /// <summary>
        /// 获取设置连线尾位置
        /// </summary>
        public double TailTop
        {
            get { return (double)GetValue(TailTopProperty); }
            set { SetValue(TailTopProperty, value); }
        }

        /// <summary>
        /// 获取设置标题文字的位置
        /// </summary>
        public double LabelLeft
        {
            get { return (double)GetValue(LabelLeftProperty); }
            set { SetValue(LabelLeftProperty, value); }
        }

        /// <summary>
        /// 获取设置标题文字的位置
        /// </summary>
        public double LabelTop
        {
            get { return (double)GetValue(LabelTopProperty); }
            set { SetValue(LabelTopProperty, value); }
        }

        /// <summary>
        /// 获取设置标题文字的旋转角度
        /// </summary>
        public double LabelAngle
        {
            get { return (double)GetValue(LabelAngleProperty); }
            set { SetValue(LabelAngleProperty, value); }
        }

        /// <summary>
        /// 获取设置是否显示调节器
        /// </summary>
        public Visibility ThumbVisibility
        {
            get { return (Visibility)GetValue(ThumbVisibilityProperty); }
            set { SetValue(ThumbVisibilityProperty, value); }
        }

        /// <summary>
        /// 获取设置连线的实际区域
        /// </summary>
#if IOS
        new
#endif
        internal Rect Bounds
        {
            get { return _bounds; }
            set { _bounds = value; }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 更新连线
        /// </summary>
        public void Refresh()
        {
            if (!_isInit || _headerNode == null || _tailNode == null)
                return;
            if (IsStraightLine)
            {
                DrawStraightLine();
                UpdateThumbsPos();
            }
            else
            {
                UpdateLines();
            }
        }

        /// <summary>
        /// 加载xml
        /// </summary>
        /// <param name="p_reader"></param>
        internal void ReadXml(XmlReader p_reader)
        {
            for (int i = 0; i < p_reader.AttributeCount; i++)
            {
                p_reader.MoveToAttribute(i);
                string id = p_reader.Name;
                switch (id)
                {
                    case "id": if (long.TryParse(p_reader.Value, out var l)) ID = l; break;
                    case "title": Title = p_reader.Value; break;
                    case "headerid": if (long.TryParse(p_reader.Value, out var h)) HeaderID = h; break;
                    case "headerport": HeaderPort = (LinkPortPosition)int.Parse(p_reader.Value); break;
                    case "headershape": HeaderShape = (LinkPortShapes)int.Parse(p_reader.Value); break;
                    case "tailid": if (long.TryParse(p_reader.Value, out var t)) TailID = t; break;
                    case "tailport": TailPort = (LinkPortPosition)int.Parse(p_reader.Value); break;
                    case "tailshape": TailShape = (LinkPortShapes)int.Parse(p_reader.Value); break;
                    case "headtailsize": HeadTailSize = double.Parse(p_reader.Value); break;
                    case "linethickness": LineThickness = double.Parse(p_reader.Value); break;
                    case "isstraightline": IsStraightLine = bool.Parse(p_reader.Value); break;
                    case "bounds":
                        {
                            string[] bounds = p_reader.Value.Split(',');
                            _bounds.X = double.Parse(bounds[0]);
                            _bounds.Y = double.Parse(bounds[1]);
                            _bounds.Width = double.Parse(bounds[2]);
                            _bounds.Height = double.Parse(bounds[3]);
                            break;
                        }
                }
            }
            p_reader.MoveToElement();
        }

        /// <summary>
        /// 序列化xml
        /// </summary>
        /// <param name="p_writer"></param>
        internal void WriteXml(XmlWriter p_writer)
        {
            UpdateBounds();
            p_writer.WriteStartElement("Line");
            p_writer.WriteAttributeString("id", ID.ToString());
            p_writer.WriteAttributeString("headerid", HeaderID.ToString());
            p_writer.WriteAttributeString("bounds", _bounds.ToString());
            if (!string.IsNullOrEmpty(Title))
                p_writer.WriteAttributeString("title", Title);
            if (this.ExistLocalValue(HeaderPortProperty))
                p_writer.WriteAttributeString("headerport", ((int)HeaderPort).ToString());
            if (this.ExistLocalValue(HeaderShapeProperty))
                p_writer.WriteAttributeString("headershape", ((int)HeaderShape).ToString());
            p_writer.WriteAttributeString("tailid", TailID.ToString());
            if (this.ExistLocalValue(TailPortProperty))
                p_writer.WriteAttributeString("tailport", ((int)TailPort).ToString());
            if (this.ExistLocalValue(TailShapeProperty))
                p_writer.WriteAttributeString("tailshape", ((int)TailShape).ToString());
            if (this.ExistLocalValue(HeadTailSizeProperty))
                p_writer.WriteAttributeString("headtailsize", HeadTailSize.ToString());
            if (this.ExistLocalValue(LineThicknessProperty))
                p_writer.WriteAttributeString("linethickness", LineThickness.ToString());
            if (this.ExistLocalValue(IsStraightLineProperty))
                p_writer.WriteAttributeString("isstraightline", IsStraightLine.ToString());
            p_writer.WriteEndElement();
        }

        /// <summary>
        /// 更新头尾thumb的位置
        /// </summary>
        internal void UpdateThumbsPos()
        {
            UpdateAnThumbPos(true);
            UpdateAnThumbPos(false);
        }

        /// <summary>
        /// 更新头或尾节点手柄位置
        /// </summary>
        /// <param name="p_isHead">true 头节点，false 尾节点</param>
        internal void UpdateAnThumbPos(bool p_isHead)
        {
            if (p_isHead)
            {
                if (_headerNode == null)
                    return;

                Point head = GetLinkPortPosition(_headerNode, HeaderPort);
                if (_headThumb != null)
                {
                    Canvas.SetLeft(_headThumb, head.X - (_headThumb.ActualWidth == 0 ? _headThumb.MinWidth / 2 : _headThumb.ActualWidth / 2));
                    Canvas.SetTop(_headThumb, head.Y - (_headThumb.ActualHeight == 0 ? _headThumb.MinHeight / 2 : _headThumb.ActualHeight / 2));
                }
            }
            else
            {
                if (_tailNode == null)
                    return;

                Point tail = GetLinkPortPosition(_tailNode, TailPort);
                if (_tailThumb != null)
                {
                    Canvas.SetLeft(_tailThumb, tail.X - (_tailThumb.ActualWidth == 0 ? _tailThumb.MinWidth / 2 : _tailThumb.ActualWidth / 2));
                    Canvas.SetTop(_tailThumb, tail.Y - (_tailThumb.ActualHeight == 0 ? _tailThumb.MinHeight / 2 : _tailThumb.ActualHeight / 2));
                }
            }
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 应用模板
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Path path = GetTemplateChild("HeaderPath") as Path;
            path.RenderTransform = _headPathTransform;
            path = GetTemplateChild("TailPath") as Path;
            path.RenderTransform = _tailPathTransform;
            _label = GetTemplateChild("TitleLabel") as TextBlock;

            _headThumb = GetTemplateChild("HeaderThumb") as Thumb;
            if (_headThumb != null)
            {
                _headThumb.DragDelta -= OnHeaderDragDelta;
                _headThumb.DragCompleted -= OnHeaderDragCompleted;

                _headThumb.DragDelta += OnHeaderDragDelta;
                _headThumb.DragCompleted += OnHeaderDragCompleted;
            }

            _tailThumb = GetTemplateChild("TailThumb") as Thumb;
            if (_tailThumb != null)
            {
                _tailThumb.DragDelta -= OnTailDragDelta;
                _tailThumb.DragCompleted -= OnTailDragCompleted;

                _tailThumb.DragDelta += OnTailDragDelta;
                _tailThumb.DragCompleted += OnTailDragCompleted;
            }
            UpdateShape(HeaderShape, true);
            UpdateShape(TailShape, false);
        }

        /// <summary>
        /// 鼠标点击
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            e.Handled = true;
            if (!_owner.IsReadOnly)
                _owner.SelectionClerk.SelectLine(this);
        }
        #endregion

        #region 调节过程


        void OnHeaderDragDelta(object sender, DragDeltaEventArgs e)
        {
            LineThumbDragDelta(sender as Thumb, true, e);
        }

        void OnHeaderDragCompleted(object sender, DragCompletedEventArgs e)
        {
            LineThumbDragCompleted(sender as Thumb, true, e);
        }

        void OnTailDragDelta(object sender, DragDeltaEventArgs e)
        {
            LineThumbDragDelta(sender as Thumb, false, e);
        }

        void OnTailDragCompleted(object sender, DragCompletedEventArgs e)
        {
            LineThumbDragCompleted(sender as Thumb, false, e);
        }

        /// <summary>
        /// 拖动thumb,改变Thumb的样式 1 拖动不删除，2 拖动删除 3 在有效区域内。
        /// </summary>
        /// <param name="p_thumb"></param>
        /// <param name="p_isHead"></param>
        /// <param name="e"></param>
        void LineThumbDragDelta(Thumb p_thumb, bool p_isHead, DragDeltaEventArgs e)
        {
            double left = Canvas.GetLeft(p_thumb) + e.HorizontalChange;
            double top = Canvas.GetTop(p_thumb) + e.VerticalChange;
            Canvas.SetLeft(p_thumb, left);
            Canvas.SetTop(p_thumb, top);
            Rect rect = new Rect(left, top, p_thumb.ActualWidth, p_thumb.ActualHeight);
            if (!_owner.LinkThumbMove(p_thumb))
            {
                VisualStateManager.GoToState(p_thumb, IsEfectDrag(p_thumb, p_isHead, 40) ? "DragDel" : "DragNoValid", false);
            }
        }

        /// <summary>
        /// 完成thumb的拖动
        /// </summary>
        /// <param name="p_thumb"></param>
        /// <param name="p_isHeadNode"></param>
        /// <param name="e"></param>
        void LineThumbDragCompleted(Thumb p_thumb, bool p_isHeadNode, DragCompletedEventArgs e)
        {
            LineMoveCmdArgs _lineMoveArg = null;
            double left = Canvas.GetLeft(p_thumb);
            double top = Canvas.GetTop(p_thumb);
            Rect rect = new Rect(left, top, p_thumb.ActualWidth, p_thumb.ActualHeight);
            SNode target = _owner.GetFirstIntersect(rect);
            SketchMoveLineCmd cmd = _owner.CmdMoveLine;

            if (target == null)
            {
                if (IsEfectDrag(p_thumb, p_isHeadNode, 40))
                {
                    _lineMoveArg = p_isHeadNode ? new LineMoveCmdArgs(this, true, _headerNode, HeaderPort) : new LineMoveCmdArgs(this, false, _tailNode, TailPort);
                    cmd.Execute(_lineMoveArg);
                }
                else
                {
                    UpdateAnThumbPos(p_isHeadNode);
                }
            }
            else
            {
                _owner.LinkThumbEnd();
                LinkPortPosition newPos = _owner.GetLinkPosition(target, rect);
                if (p_isHeadNode)
                {
                    if (newPos != HeaderPort || target != _headerNode)
                    {
                        _lineMoveArg = new LineMoveCmdArgs(this, true, _headerNode, HeaderPort, target, newPos);
                        cmd.Execute(_lineMoveArg);
                    }
                }
                else
                {
                    if (newPos != TailPort || target != _tailNode)
                    {
                        _lineMoveArg = new LineMoveCmdArgs(this, false, _tailNode, TailPort, target, newPos);
                        cmd.Execute(_lineMoveArg);
                    }
                }
            }
        }
        #endregion

        #region 内部方法

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            AfterLoad();
        }

        internal void AfterLoad()
        {
            Loaded -= OnLoaded;
            _isInit = true;
            ApplyTemplate();
            UpdateHeader();
            UpdateTail();
            Refresh();
        }

        /// <summary>
        /// 更新头端点
        /// </summary>
        void UpdateHeader()
        {
            if (_headerID == 0)
            {
                _headerNode = null;
            }
            else
            {
                _headerNode = (from obj in _owner.Container.Children
                               let node = obj as SNode
                               where node != null && node.ID == _headerID
                               select node).FirstOrDefault();
            }
        }

        /// <summary>
        /// 更新尾端点
        /// </summary>
        void UpdateTail()
        {
            if (_tailID == 0)
            {
                _tailNode = null;
            }
            else
            {
                _tailNode = (from obj in _owner.Container.Children
                             let node = obj as SNode
                             where node != null && node.ID == _tailID
                             select node).FirstOrDefault();
            }
        }

        /// <summary>
        /// 更新端点图形
        /// </summary>
        /// <param name="p_shape"></param>
        /// <param name="p_isHeader"></param>
        void UpdateShape(LinkPortShapes p_shape, bool p_isHeader)
        {
            PathGeometry geo = null;
            switch (p_shape)
            {
                case LinkPortShapes.None:
                    geo = null;
                    break;
                case LinkPortShapes.Arrow:
                    geo = Res.CloneGeometry(ArrowData);
                    break;
                case LinkPortShapes.Diamond:
                    geo = Res.CloneGeometry(DiamondData);
                    break;
                case LinkPortShapes.Circle:
                    geo = Res.CloneGeometry(CircleData);
                    break;
            }

            if (p_isHeader)
                HeaderGeometry = geo;
            else
                TailGeometry = geo;
        }

        /// <summary>
        /// 绘制直线
        /// </summary>
        void DrawStraightLine()
        {
            Point start, end;
            start = GetLinkPortPosition(_headerNode, HeaderPort);
            end = GetLinkPortPosition(_tailNode, TailPort);
            if (start.X != end.X && start.Y != end.Y)
            {
                // 斜线
                DrawSlantLine(start, end);
            }
            else if (start.X == end.X)
            {
                // 垂直线
                DrawVerLine(start, end);
            }
            else if (start.Y == end.Y)
            {
                // 水平线
                DrawHorLine(start, end);
            }
        }

        /// <summary>
        /// 绘制水平线
        /// </summary>
        /// <param name="p_start"></param>
        /// <param name="p_end"></param>
        void DrawHorLine(Point p_start, Point p_end)
        {
            Point end;
            double headX, headY, tailX, tailY;

            if (p_start.X < p_end.X)
            {
                // ->
                end = new Point(p_end.X - HeadTailSize, p_end.Y);

                headX = p_start.X;
                headY = p_start.Y - HeadTailSize / 2;

                tailX = end.X;
                tailY = headY;
                _tailPathTransform.Rotation = 0.0;

                LabelLeft = p_start.X + (p_end.X - p_start.X - HeadTailSize - _label.ActualWidth) / 2;
                LabelTop = p_start.Y - LineThickness / 2 - _label.ActualHeight;
                LabelAngle = 0.0;
            }
            else
            {
                //  <-
                end = new Point(p_end.X + HeadTailSize, p_end.Y);

                headX = p_start.X - HeadTailSize;
                headY = p_start.Y - HeadTailSize / 2;

                tailX = p_end.X;
                tailY = headY;
                _tailPathTransform.Rotation = 180.0;

                LabelLeft = end.X + (p_start.X - end.X - _label.ActualWidth) / 2;
                LabelTop = p_start.Y - LineThickness / 2 - _label.ActualHeight;
                LabelAngle = 0.0;
            }

            UpdateDirectPath(p_start, end);
            UpdateHeadNodePos(headX, headY);
            UpdateTailNodePos(tailX, tailY);
        }

        /// <summary>
        /// 绘制垂直线
        /// </summary>
        /// <param name="p_start"></param>
        /// <param name="p_end"></param>
        void DrawVerLine(Point p_start, Point p_end)
        {
            Point end;
            double headX = 0.0, headY = 0.0, tailX = 0, tailY = 0;

            if (p_start.Y < p_end.Y)
            {
                // ↓
                end = new Point(p_end.X, p_end.Y - HeadTailSize);

                headX = p_start.X - HeadTailSize / 2;
                headY = p_start.Y;

                tailX = headX;
                tailY = end.Y;
                _tailPathTransform.Rotation = 90.0;

                LabelLeft = p_start.X - LineThickness / 2 - _label.ActualHeight;
                LabelTop = end.Y - (p_end.Y - p_start.Y - HeadTailSize - _label.ActualWidth) / 2;
                LabelAngle = -90.0;
            }
            else
            {
                // ↑
                end = new Point(p_end.X, p_end.Y + HeadTailSize);

                headX = p_start.X - HeadTailSize / 2;
                headY = p_start.Y - HeadTailSize;

                tailX = headX;
                tailY = p_end.Y;
                _tailPathTransform.Rotation = -90.0;

                LabelLeft = p_start.X + LineThickness / 2 + _label.ActualHeight;
                LabelTop = end.Y + (p_start.Y - p_end.Y - HeadTailSize - _label.ActualWidth) / 2;
                LabelAngle = 90.0;
            }

            UpdateDirectPath(p_start, end);
            UpdateHeadNodePos(headX, headY);
            UpdateTailNodePos(tailX, tailY);
        }

        /// <summary>
        /// 绘制斜线
        /// </summary>
        /// <param name="p_start"></param>
        /// <param name="p_end"></param>
        void DrawSlantLine(Point p_start, Point p_end)
        {
            // 以p_start作为原点
            if (p_start.X < p_end.X)
            {
                if (p_start.Y > p_end.Y)
                {
                    // 第一象限
                    double deltaX = p_end.X - p_start.X;
                    double deltaY = p_start.Y - p_end.Y;
                    double length = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
                    double padding = (length - _label.ActualWidth) / 2;

                    LabelLeft = p_start.X + padding * deltaX / length;
                    LabelTop = p_start.Y - padding * deltaY / length;
                    LabelAngle = -(Math.Atan(deltaY / deltaX) * 180 / Math.PI);
                }
                else
                {
                    // 第四象限
                    double deltaX = p_end.X - p_start.X;
                    double deltaY = p_end.Y - p_start.Y;
                    double length = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
                    double padding = (length - _label.ActualWidth) / 2;

                    double cos = Math.Cos(LabelAngle);
                    double sin = Math.Sin(LabelAngle);
                    double tan = Math.Tan(LabelAngle);
                }
            }
            else
            {
                if (p_start.Y > p_end.Y)
                {
                    // 第二象限
                    double deltaX = p_start.X - p_end.X;
                    double deltaY = p_start.Y - p_end.Y;
                    double length = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
                    double padding = (length - _label.ActualWidth) / 2;

                    LabelLeft = p_end.X + padding * deltaX / length;
                    LabelTop = p_end.Y + padding * deltaY / length;
                    LabelAngle = Math.Atan(deltaY / deltaX) * 180 / Math.PI;
                }
                else
                {
                    // 第三象限
                    double deltaX = p_start.X - p_end.X;
                    double deltaY = p_end.Y - p_start.Y;
                    double length = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
                    double padding = (length - _label.ActualWidth) / 2;

                    LabelLeft = p_end.X + padding * deltaX / length;
                    LabelTop = p_end.Y - padding * deltaY / length;
                    LabelAngle = -(Math.Atan(deltaY / deltaX) * 180 / Math.PI);
                }
            }

            UpdateDirectPath(p_start, p_end);
            UpdateHeadNodePos(0, -1000);
            UpdateTailNodePos(0, -1000);
        }

        /// <summary>
        /// 绘制无节点的空线
        /// </summary>
        void DrawEmptyLine()
        {
            // 在垂直中心处画横线
            double y = (ActualHeight - LineThickness) / 2;
            Point start = new Point(0, y);
            Point end = new Point(ActualWidth - HeadTailSize, y);

            UpdateDirectPath(start, end);
            UpdateHeadNodePos(start.X, start.Y - LineThickness);
            UpdateTailNodePos(end.X, end.Y - LineThickness);

            LabelLeft = (ActualWidth - _label.ActualWidth) / 2;
            LabelTop = y - LineThickness * 2;
            LabelAngle = 0.0;
        }

        /// <summary>
        /// 获取连接点位置
        /// </summary>
        /// <param name="p_node"></param>
        /// <param name="p_pos"></param>
        /// <returns></returns>
        Point GetLinkPortPosition(FrameworkElement p_node, LinkPortPosition p_pos)
        {
            if (p_node == null)
                return new Point(0.0, 0.0);

            Point point = new Point(0.0, 0.0);
            double left = Canvas.GetLeft(p_node);
            double top = Canvas.GetTop(p_node);
            double width = p_node.Width;
            double height = p_node.Height;
            double right = left + width;
            double bottom = top + height;

            switch (p_pos)
            {
                case LinkPortPosition.LeftTop:
                    point = new Point(left, top);
                    break;
                case LinkPortPosition.LeftCenter:
                    point = new Point(left, top + height / 2);
                    break;
                case LinkPortPosition.LeftBottom:
                    point = new Point(left, bottom);
                    break;
                case LinkPortPosition.TopCenter:
                    point = new Point(left + width / 2, top);
                    break;
                case LinkPortPosition.BottomCenter:
                    point = new Point(left + width / 2, bottom);
                    break;
                case LinkPortPosition.RightTop:
                    point = new Point(right, top);
                    break;
                case LinkPortPosition.RightCenter:
                    point = new Point(right, top + height / 2);
                    break;
                case LinkPortPosition.RightBottom:
                    point = new Point(right, bottom);
                    break;
            }
            return point;
        }

        /// <summary>
        /// 更新直线路径
        /// </summary>
        /// <param name="p_start"></param>
        /// <param name="p_end"></param>
        void UpdateDirectPath(Point p_start, Point p_end)
        {
            PathGeometry geometry = new PathGeometry();
            PathFigure figure = new PathFigure();
            figure.StartPoint = p_start;
            LineSegment segment = new LineSegment();
            segment.Point = p_end;
            figure.Segments.Add(segment);
            geometry.Figures.Add(figure);
            LinkPathGeometry = geometry;
        }

        /// <summary>
        /// 更新连线路径
        /// </summary>
        /// <param name="p_linkPoints"></param>
        void UpdatePathGeometry(List<Point> p_linkPoints)
        {
            if (p_linkPoints == null || p_linkPoints.Count < 2)
                return;

            PathGeometry geometry = new PathGeometry();
            PathFigure figure = new PathFigure();
            figure.StartPoint = p_linkPoints[0];
            for (int i = 1; i < p_linkPoints.Count; i++)
            {
                LineSegment segment = new LineSegment();
                segment.Point = p_linkPoints[i];
                figure.Segments.Add(segment);
            }
            geometry.Figures.Add(figure);
            LinkPathGeometry = geometry;
        }

        /// <summary>
        /// 更新线的区域范围
        /// </summary>
        void UpdateBounds()
        {
            if (LinkPathGeometry == null)
                return;
            _bounds = new Rect();
            PathFigure figure = LinkPathGeometry.Figures[0];
            _bounds.X = figure.StartPoint.X;
            _bounds.Y = figure.StartPoint.Y;

            if (HeaderGeometry != null)
            {
                CalcBound(new Point(HeaderLeft, HeaderTop));
                CalcBound(new Point(HeaderLeft + HeadTailSize, HeaderTop));
                CalcBound(new Point(HeaderLeft, HeaderTop + HeadTailSize));
                CalcBound(new Point(HeaderLeft + HeadTailSize, HeaderTop + HeadTailSize));
            }

            if (TailGeometry != null)
            {
                CalcBound(new Point(TailLeft, TailTop));
                CalcBound(new Point(TailLeft + HeadTailSize, TailTop));
                CalcBound(new Point(TailLeft, TailTop + HeadTailSize));
                CalcBound(new Point(TailLeft + HeadTailSize, TailTop + HeadTailSize));
            }

            foreach (LineSegment segment in figure.Segments)
            {
                CalcBound(segment.Point);
            }
        }

        void CalcBound(Point p_pt)
        {
            if (_bounds.Contains(p_pt))
                return;

            if (p_pt.X < _bounds.X)
                _bounds.X = p_pt.X;
            else if (p_pt.X > _bounds.X + _bounds.Width)
                _bounds.Width = p_pt.X - _bounds.X;
            if (p_pt.Y < _bounds.Y)
                _bounds.Y = p_pt.Y;
            else if (p_pt.Y > _bounds.Y + _bounds.Height)
                _bounds.Height = p_pt.Y - _bounds.Y;
        }

        /// <summary>
        /// 更新头端点位置
        /// </summary>
        /// <param name="p_x"></param>
        /// <param name="p_y"></param>
        void UpdateHeadNodePos(double p_x, double p_y)
        {
            HeaderLeft = p_x;
            HeaderTop = p_y;
        }

        /// <summary>
        /// 更新尾端点位置
        /// </summary>
        /// <param name="p_x"></param>
        /// <param name="p_y"></param>
        void UpdateTailNodePos(double p_x, double p_y)
        {
            TailLeft = p_x;
            TailTop = p_y;
        }

        /// <summary>
        /// 是否有效拖动， 是 true 否 false
        /// </summary>
        /// <param name="p_thumb"></param>
        /// <param name="p_isHead"></param>
        /// <param name="p_notValidLongth"></param>
        /// <returns></returns>
        bool IsEfectDrag(Thumb p_thumb, bool p_isHead, double p_notValidLongth)
        {
            if (_headerNode == null || _tailNode == null)
                return true;

            Point thumbCenter = new Point(Canvas.GetLeft(p_thumb) + p_thumb.ActualWidth / 2, Canvas.GetTop(p_thumb) + p_thumb.ActualHeight / 2);
            Point oldPos = p_isHead ? GetLinkPortPosition(_tailNode, TailPort) : GetLinkPortPosition(_tailNode, TailPort);
            return CalDistence(thumbCenter, oldPos) > p_notValidLongth;
        }

        /// <summary>
        /// 计算两点之间的距离
        /// </summary>
        /// <param name="p_start"></param>
        /// <param name="p_end"></param>
        /// <returns></returns>
        double CalDistence(Point p_start, Point p_end)
        {
            return Math.Sqrt(Math.Pow(p_start.X - p_end.X, 2) + Math.Pow(p_start.Y - p_end.Y, 2));
        }

        /// <summary>
        /// 在两个任务节点之间画线：确定连线的点。
        /// 原则：尽量减少线段数量，使其符合画流程图的一般使用习惯。
        /// 方法：首先确定 plainDis（安全距离），
        /// 即一般情况下节点出门线段的长度。如在头节点上方中点开始连线，
        /// 则默认起始线段为头节点上方长度为 plainDis 的线段；
        /// 其次：根据头节点及尾节点起始线段的起点和中点的方位关系匹配出16中情况；
        /// 再次：在每种情况中，根据起始点和终止点的方位关系进行处理。
        /// 如：头节点起始线段是水平的，则开始按起始点左右划分，如是垂直的则开始按上下划分。
        /// 最后：在划分后进行连线，在连线过程中，能够以直线或直角连线的尽量用直线和直角连线。
        /// 连线中尽量避免线段穿过头节点及尾节点的情况，但头节点和尾节点相交的时候打破此规则。
        /// 发生不能以直线和直角线连接的情况时，要在头尾节点的中点处进行分解。
        /// </summary>
        void UpdateLines()
        {
            if (!_isInit || _headerNode == null || _tailNode == null)
                return;

            List<Point> pointsRelativ = new List<Point>();// 画线的点
            List<LinkPortPosition> linkPos = new List<LinkPortPosition>();
            Rect headRect = GetEleRect(_headerNode);
            Rect tailRect = GetEleRect(_tailNode);
            Point headCenter = GetRectCenter(headRect);
            Point tailCenter = GetRectCenter(tailRect);
            Point headerPoint = GetLinkPortPosition(_headerNode, HeaderPort);
            Point tailPoint = GetLinkPortPosition(_tailNode, TailPort);
            double plainDis = HeadTailSize * 2;
            List<Point> headRels = GetRelativePoint(_headerNode, HeaderPort, plainDis);
            List<Point> tailRels = GetRelativePoint(_tailNode, TailPort, plainDis);
            LinkPortPosition tmp;
            bool IsTailPort = false;

            // 获得 headnode 和 tailnode的相对位置，以headnode为参考点
            PosRelation relation = GetPosRelation(headCenter, tailCenter);
            //两点重合，即两个控件中心对齐或是一个控件。
            if (relation == PosRelation.Supper && _headerNode.GetSize() == _tailNode.GetSize())
            {
                if (HeaderPort == TailPort)
                {
                    if (HeaderPort.ToString().IndexOf("Center") > -1)
                    {
                        pointsRelativ = new List<Point>() { headerPoint, headRels[0], tailPoint };
                    }
                    else
                    {
                        pointsRelativ = new List<Point>() { headerPoint, headRels[0], headRels[1], headRels[2], tailPoint };
                    }
                    DrawLine(pointsRelativ);
                    return;
                }

                linkPos.Clear();
                for (int i = 1; i < 5; i++)
                {
                    tmp = GetLeanPort(HeaderPort, i, true);
                    IsTailPort = tmp == TailPort;
                    if (!IsTailPort)
                    {
                        linkPos.Add(tmp);
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }
                if (!IsTailPort)
                {
                    // 顺时针查找结点4次未找到尾节点,逆向查找,本次一定会找到
                    linkPos.Clear();
                    for (int i = 1; i < 4; i++)
                    {
                        tmp = GetLeanPort(HeaderPort, i, false);
                        if (!(tmp == TailPort))
                        {
                            linkPos.Add(tmp);
                            continue;
                        }
                        break;
                    }
                }
                pointsRelativ.Add(headerPoint);
                if (IsTailPort)
                {
                    // 顺时针方向找到的
                    List<Point> relative = GetRelativePoint(_headerNode, HeaderPort, plainDis);
                    pointsRelativ.Add(relative.Count == 1 ? relative[0] : relative[2]);
                    foreach (LinkPortPosition port in linkPos)
                    {
                        pointsRelativ.AddRange(GetRelativePoint(_tailNode, port, plainDis));
                    }
                    pointsRelativ.Add(GetRelativePoint(_tailNode, TailPort, plainDis)[0]);
                }
                else
                {
                    // 逆时针方向找到的
                    List<Point> relative;
                    pointsRelativ.Add(GetRelativePoint(_headerNode, HeaderPort, plainDis)[0]);
                    foreach (LinkPortPosition port in linkPos)
                    {
                        relative = GetRelativePoint(_tailNode, port, plainDis);
                        relative.Reverse();
                        pointsRelativ.AddRange(relative);
                    }
                    relative = GetRelativePoint(_tailNode, TailPort, plainDis);
                    pointsRelativ.Add(relative.Count == 1 ? relative[0] : relative[2]);
                }
                pointsRelativ.Add(tailPoint);
                DrawLine(pointsRelativ);
                return;
            }

            Point testPoint = new Point(0, 0);
            Point fitPoint;
            Rect headNodeExt = new Rect(Canvas.GetLeft(_headerNode) - plainDis, Canvas.GetTop(_headerNode) - plainDis, _headerNode.Width + 2 * plainDis, _headerNode.Height + 2 * plainDis);
            Rect tailNodeExt = new Rect(Canvas.GetLeft(_tailNode) - plainDis, Canvas.GetTop(_tailNode) - plainDis, _tailNode.Width + 2 * plainDis, _tailNode.Height + 2 * plainDis);
            Point HeadTemp = headRels.Count == 1 ? headRels[0] : headRels[1];
            Point TailTemp = tailRels.Count == 1 ? tailRels[0] : tailRels[1];
            if (headRels.Count == 3)
                HeadTemp = CalDistence(headRels[0], tailPoint) < CalDistence(headRels[2], tailPoint) ? headRels[0] : headRels[2];
            if (tailRels.Count == 3)
                TailTemp = CalDistence(tailRels[0], headerPoint) < CalDistence(tailRels[2], headerPoint) ? tailRels[0] : tailRels[2];

            double tempX, tempY;
            if (Math.Abs(headCenter.X - tailCenter.X) < (headRect.Width + tailRect.Width) / 2)
            {
                tempX = (headCenter.X + tailCenter.X) / 2;
            }
            else
            {
                tempX = (Math.Abs(headCenter.X - tailCenter.X) - (headRect.Width + tailRect.Width) / 2) / 2 + Math.Min(headCenter.X, tailCenter.X) + Math.Min(headRect.Width, tailRect.Width) / 2;
            }

            if (Math.Abs(headCenter.Y - tailCenter.Y) < (headRect.Height + tailRect.Height) / 2)
            {
                tempY = (headCenter.Y + tailCenter.Y) / 2;
            }
            else
            {
                tempY = (Math.Abs(headCenter.Y - tailCenter.Y) - (headRect.Height + tailRect.Height) / 2) / 2 + Math.Min(headCenter.Y, tailCenter.Y) + Math.Min(headRect.Height, tailRect.Height) / 2;
            }

            fitPoint = new Point(tempX, tempY);
            PosRelation headRel = GetPosRelation(headerPoint, HeadTemp);
            PosRelation tailRel = GetPosRelation(tailPoint, TailTemp);

            // 相对直线直连
            if ((headRel == PosRelation.Up && tailRel == PosRelation.Down && headerPoint.Y >= tailPoint.Y && headerPoint.X == tailPoint.X)
               || (headRel == PosRelation.Down && tailRel == PosRelation.Up && headerPoint.Y <= tailPoint.Y && headerPoint.X == tailPoint.X)
               || (headRel == PosRelation.Left && tailRel == PosRelation.Right && headerPoint.X >= tailPoint.X && tailPoint.Y == headerPoint.Y)
               || (headRel == PosRelation.Right && tailRel == PosRelation.Left && headerPoint.X <= tailPoint.X && tailPoint.Y == headerPoint.Y))
            {
                pointsRelativ = new List<Point>() { headerPoint, tailPoint };
                DrawLine(pointsRelativ);
                return;
            }
            // 按起点和终点的方向及相对位置来决定如何画线  
            List<Point> headHelps = new List<Point>();
            List<Point> tailHelps = new List<Point>();

            switch (headRel)
            {
                case PosRelation.Up: // 头上
                    switch (tailRel)
                    {
                        case PosRelation.Up: // 头上 尾上
                            // 尾在头上  不相交 的情况。
                            if (tailNodeExt.Y + tailNodeExt.Height < HeadTemp.Y
                                && HeadTemp.X > tailNodeExt.X
                                && HeadTemp.X < tailNodeExt.X + tailNodeExt.Width)
                            {
                                headHelps.Add(new Point(HeadTemp.X, fitPoint.Y));
                                if (HeadTemp.X >= fitPoint.X) // 中点偏左  右转
                                {
                                    headHelps.Add(new Point(tailNodeExt.X + tailNodeExt.Width, fitPoint.Y));
                                    tailHelps.Add(new Point(tailNodeExt.X + tailNodeExt.Width, TailTemp.Y));
                                }
                                else // 左转
                                {
                                    headHelps.Add(new Point(tailNodeExt.X, fitPoint.Y));
                                    tailHelps.Add(new Point(tailNodeExt.X, TailTemp.Y));
                                }
                                pointsRelativ = new List<Point>() { headerPoint, HeadTemp };
                                pointsRelativ.AddRange(headHelps);
                                pointsRelativ.AddRange(tailHelps);
                                pointsRelativ.Add(TailTemp);
                                pointsRelativ.Add(tailPoint);
                                DrawLine(pointsRelativ);
                                return;
                            }
                            // 头上尾下
                            if (tailNodeExt.Y > headNodeExt.Y + headNodeExt.Height
                                && TailTemp.X > headNodeExt.X
                                && TailTemp.X < headNodeExt.X + headNodeExt.Width)
                            {
                                if (TailTemp.X <= headCenter.X) // 左转
                                {
                                    if (HeaderPort == LinkPortPosition.LeftTop)
                                    {
                                        HeadTemp = headRels[0];
                                    }
                                    else
                                    {
                                        headHelps.Add(new Point(headNodeExt.X, HeadTemp.Y));
                                    }
                                    headHelps.Add(new Point(headNodeExt.X, fitPoint.Y));
                                    tailHelps.Add(new Point(TailTemp.X, fitPoint.Y));
                                }
                                else // 右转
                                {
                                    if (HeaderPort == LinkPortPosition.RightTop)
                                    {
                                        HeadTemp = headRels[2];
                                    }
                                    else
                                    {
                                        headHelps.Add(new Point(headNodeExt.X + headNodeExt.Width, HeadTemp.Y));
                                    }
                                    headHelps.Add(new Point(headNodeExt.X + headNodeExt.Width, fitPoint.Y));
                                    tailHelps.Add(new Point(TailTemp.X, fitPoint.Y));
                                }
                                pointsRelativ = new List<Point>() { headerPoint, HeadTemp };
                                pointsRelativ.AddRange(headHelps);
                                pointsRelativ.AddRange(tailHelps);
                                pointsRelativ.Add(TailTemp);
                                pointsRelativ.Add(tailPoint);
                                DrawLine(pointsRelativ);
                                return;
                            }
                            //其他情况
                            testPoint = TailTemp.Y <= HeadTemp.Y ? new Point(HeadTemp.X, TailTemp.Y) : new Point(TailTemp.X, HeadTemp.Y);
                            pointsRelativ = new List<Point>() { headerPoint, HeadTemp, testPoint, TailTemp, tailPoint };
                            DrawLine(pointsRelativ);
                            return;
                        case PosRelation.Down:  // 头上 尾下  
                            if (headerPoint.Y >= tailPoint.Y)
                            {
                                // 两种特殊情况
                                if (tailPoint.X <= headNodeExt.X && HeaderPort == LinkPortPosition.LeftTop)
                                {
                                    HeadTemp = headRels[0];
                                    testPoint = new Point(TailTemp.X, HeadTemp.Y);
                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, testPoint, TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                                if (tailPoint.X >= headNodeExt.X + headNodeExt.Width && HeaderPort == LinkPortPosition.RightTop)
                                {
                                    HeadTemp = headRels[2];
                                    testPoint = new Point(TailTemp.X, HeadTemp.Y);
                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, testPoint, TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                                if (fitPoint.Y > HeadTemp.Y)
                                    HeadTemp.Y = fitPoint.Y;
                                if (fitPoint.Y > TailTemp.Y)
                                    TailTemp.Y = fitPoint.Y;
                                headHelps.Add(new Point(HeadTemp.X, fitPoint.Y));
                                tailHelps.Add(new Point(TailTemp.X, fitPoint.Y));
                            }
                            else
                            {
                                if (HeadTemp.X == TailTemp.X)
                                {
                                    if (HeadTemp.X < tailCenter.X) // 左转
                                    {
                                        if (HeaderPort == LinkPortPosition.LeftTop)
                                        {
                                            HeadTemp = headRels[0];
                                        }
                                        else
                                        {
                                            headHelps.Add(new Point(headNodeExt.X, HeadTemp.Y));
                                        }
                                        if (tailNodeExt.X < headNodeExt.X)
                                        {
                                            headHelps.Add(new Point(tailNodeExt.X, HeadTemp.Y));
                                            tailHelps.Add(new Point(tailNodeExt.X, TailTemp.Y));
                                        }
                                        else
                                        {
                                            tailHelps.Add(new Point(headNodeExt.X, TailTemp.Y));
                                        }
                                    }
                                    else // 右转
                                    {
                                        if (HeaderPort == LinkPortPosition.RightTop)
                                        {
                                            HeadTemp = headRels[2];
                                        }
                                        else
                                        {
                                            headHelps.Add(new Point(headNodeExt.X + headNodeExt.Width, HeadTemp.Y));
                                        }
                                        if (tailNodeExt.X + tailNodeExt.Width > headNodeExt.X + headNodeExt.Width)
                                        {
                                            headHelps.Add(new Point(tailNodeExt.X + tailNodeExt.Width, HeadTemp.Y));
                                            tailHelps.Add(new Point(tailNodeExt.X + tailNodeExt.Width, TailTemp.Y));
                                        }
                                        else
                                        {
                                            tailHelps.Add(new Point(headNodeExt.X + headNodeExt.Width, TailTemp.Y));
                                        }
                                    }
                                }
                                else
                                {
                                    headHelps.Add(new Point(fitPoint.X, HeadTemp.Y));
                                    tailHelps.Add(new Point(fitPoint.X, TailTemp.Y));
                                }
                            }
                            pointsRelativ = new List<Point>() { headerPoint, HeadTemp };
                            pointsRelativ.AddRange(headHelps);
                            pointsRelativ.AddRange(tailHelps);
                            pointsRelativ.Add(TailTemp);
                            pointsRelativ.Add(tailPoint);
                            DrawLine(pointsRelativ);
                            return;
                        case PosRelation.Right: // 头上 尾右
                            if (headerPoint.Y >= tailPoint.Y) // 尾 连接点 在 头上
                            {
                                if (TailTemp.X < headerPoint.X) // 尾在头左
                                {
                                    if (TailTemp.Y > HeadTemp.Y)
                                        HeadTemp.Y = TailTemp.Y;
                                    testPoint = new Point(HeadTemp.X, TailTemp.Y);
                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, testPoint, TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                                else // 尾在头右
                                {
                                    if (TailTemp.X <= headNodeExt.X && HeaderPort == LinkPortPosition.LeftTop) // 特殊情况
                                    {
                                        testPoint = new Point(TailTemp.X, HeadTemp.Y);
                                        pointsRelativ = new List<Point>() { headerPoint, HeadTemp, testPoint, TailTemp, tailPoint };
                                        DrawLine(pointsRelativ);
                                        return;
                                    }
                                    if (fitPoint.Y > HeadTemp.Y)
                                        HeadTemp.Y = fitPoint.Y;
                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, new Point(HeadTemp.X, fitPoint.Y), new Point(TailTemp.X, fitPoint.Y), TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                            }
                            else  // 尾连接点 y 在 头节点 下面
                            {
                                if (TailTemp.X < HeadTemp.X) // 尾连接点在 头节点左面
                                {
                                    if (HeaderPort == LinkPortPosition.LeftTop)
                                        HeadTemp = headRels[0];
                                    if (tailRect.X + tailRect.Width > headRect.X)
                                        fitPoint.X = (HeadTemp.X + tailPoint.X) / 2;
                                    if (fitPoint.X > HeadTemp.X)
                                        HeadTemp.X = fitPoint.X;
                                    if (TailTemp.X > fitPoint.X)
                                        TailTemp.X = fitPoint.X;
                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, new Point(fitPoint.X, HeadTemp.Y), new Point(fitPoint.X, TailTemp.Y), TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                                else // 尾连接点在头节点右面
                                {
                                    if (tailNodeExt.Y < HeadTemp.Y)
                                        HeadTemp.Y = tailNodeExt.Y;// beadtemp 向上延伸
                                    if ((tailNodeExt.Y > headerPoint.Y || TailTemp.X > headNodeExt.X + headNodeExt.Width)
                                        && HeaderPort == LinkPortPosition.RightTop) // 特殊情况
                                    {
                                        HeadTemp = headRels[2];
                                    }
                                    testPoint = TailTemp.X < HeadTemp.X ? new Point(HeadTemp.X, TailTemp.Y) : new Point(TailTemp.X, HeadTemp.Y);
                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, testPoint, TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                            }
                        case PosRelation.Left: // 头上 尾左
                            if (headerPoint.Y >= tailPoint.Y) // 尾 连接点 在 头上
                            {
                                if (TailTemp.X < headerPoint.X) // 尾在头左
                                {
                                    if (TailTemp.X <= headNodeExt.X && HeaderPort == LinkPortPosition.LeftTop) // 特殊情况
                                    {
                                        testPoint = new Point(TailTemp.X, HeadTemp.Y);
                                        pointsRelativ = new List<Point>() { headerPoint, HeadTemp, testPoint, TailTemp, tailPoint };
                                        DrawLine(pointsRelativ);
                                        return;
                                    }
                                    if (fitPoint.Y > HeadTemp.Y)
                                        HeadTemp.Y = fitPoint.Y;

                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, new Point(HeadTemp.X, fitPoint.Y), new Point(TailTemp.X, fitPoint.Y), TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                                else // 尾在头右
                                {
                                    if (TailTemp.Y > HeadTemp.Y)
                                        HeadTemp.Y = TailTemp.Y;
                                    testPoint = new Point(HeadTemp.X, TailTemp.Y);
                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, testPoint, TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                            }
                            else  // 尾连接点 y 在 头节点 下面
                            {
                                if (TailTemp.X < HeadTemp.X) // 尾连接点在 头节点左面
                                {
                                    if (tailNodeExt.Y < HeadTemp.Y)
                                        HeadTemp.Y = tailNodeExt.Y;// beadtemp 向上延伸

                                    if ((tailNodeExt.Y > headerPoint.Y || TailTemp.X > headNodeExt.X)
                                        && HeaderPort == LinkPortPosition.LeftTop) // 特殊情况
                                    {
                                        HeadTemp = headRels[0];
                                    }
                                    testPoint = TailTemp.X < HeadTemp.X ? new Point(TailTemp.X, HeadTemp.Y) : new Point(HeadTemp.X, TailTemp.Y);
                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, testPoint, TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                                else // 尾连接点在头节点右面
                                {
                                    if (HeaderPort == LinkPortPosition.RightTop)
                                        HeadTemp = headRels[2];
                                    if (tailRect.X < headRect.X + headRect.Width)
                                        fitPoint.X = (HeadTemp.X + tailPoint.X) / 2;
                                    if (fitPoint.X < HeadTemp.X)
                                        HeadTemp.X = fitPoint.X;
                                    if (TailTemp.X < fitPoint.X)
                                        TailTemp.X = fitPoint.X;
                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, new Point(fitPoint.X, HeadTemp.Y), new Point(fitPoint.X, TailTemp.Y), TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                            }
                        default:
                            break;
                    }
                    break;
                case PosRelation.Right: // 头右
                    switch (tailRel)
                    {
                        case PosRelation.Up: // 头右 尾上
                            // 左右划分
                            if (tailPoint.X <= headerPoint.X) // 尾左头右
                            {
                                if (tailPoint.Y >= headerPoint.Y) //头上尾下
                                {
                                    if (HeaderPort == LinkPortPosition.RightTop)
                                        HeadTemp = headRels[2];
                                    if (tailRect.Y < headRect.Y + headRect.Height)
                                        fitPoint.Y = (headerPoint.Y + tailPoint.Y) / 2;
                                    if (TailTemp.Y < fitPoint.Y)
                                        TailTemp.Y = fitPoint.Y;
                                    if (HeadTemp.Y > fitPoint.Y)
                                        HeadTemp.Y = fitPoint.Y;
                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, new Point(HeadTemp.X, fitPoint.Y), new Point(TailTemp.X, fitPoint.Y), TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                                else // 头下尾上
                                {
                                    if ((TailTemp.X < headNodeExt.Y + headNodeExt.Height || tailNodeExt.X + tailNodeExt.Width < headerPoint.X)
                                       && HeaderPort == LinkPortPosition.RightBottom)
                                    {
                                        HeadTemp = headRels[2];
                                        testPoint = HeadTemp.Y > TailTemp.Y ? new Point(TailTemp.X, HeadTemp.Y) : new Point(HeadTemp.X, TailTemp.Y);
                                        pointsRelativ = new List<Point>() { headerPoint, HeadTemp, testPoint, TailTemp, tailPoint };
                                        DrawLine(pointsRelativ);
                                        return;
                                    }
                                    if (tailNodeExt.X + tailNodeExt.Width > HeadTemp.X)
                                    {
                                        HeadTemp.X = tailNodeExt.X + tailNodeExt.Width;
                                    }
                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, new Point(HeadTemp.X, TailTemp.Y), TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                            }
                            else // 头左尾右
                            {
                                if (tailPoint.Y >= headerPoint.Y) //头上尾下
                                {
                                    if (TailTemp.X < HeadTemp.X)
                                        HeadTemp.X = TailTemp.X;
                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, new Point(TailTemp.X, HeadTemp.Y), TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                                else // 头下尾上
                                {
                                    if ((tailNodeExt.X > headRect.X || headNodeExt.Y + headNodeExt.Height > tailRect.Y + tailRect.Height)
                                      && HeaderPort == LinkPortPosition.RightBottom) // 特殊情况
                                    {
                                        HeadTemp = headRels[2];
                                        testPoint = HeadTemp.Y < TailTemp.Y ? new Point(HeadTemp.X, TailTemp.Y) : new Point(TailTemp.X, HeadTemp.Y);
                                        pointsRelativ = new List<Point>() { headerPoint, HeadTemp, testPoint, TailTemp, tailPoint };
                                        DrawLine(pointsRelativ);
                                        return;
                                    }
                                    if (tailRect.X < headRect.X + headRect.Width)
                                        fitPoint.X = (headerPoint.X + tailPoint.X) / 2;
                                    if (fitPoint.X < HeadTemp.X)
                                        HeadTemp.X = fitPoint.X;
                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, new Point(fitPoint.X, HeadTemp.Y), new Point(fitPoint.X, TailTemp.Y), TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                            }
                        case PosRelation.Down: //  头右 尾下
                            // 左右划分
                            if (tailPoint.X <= headerPoint.X) // 尾左头右
                            {
                                if (tailPoint.Y >= headerPoint.Y) //头上尾下
                                {
                                    if ((tailNodeExt.X + tailNodeExt.Width < headerPoint.X || TailTemp.Y < headNodeExt.Y + headNodeExt.Height)
                                        && HeaderPort == LinkPortPosition.RightBottom)
                                    {
                                        HeadTemp = headRels[2];
                                        testPoint = TailTemp.Y > HeadTemp.Y ? new Point(HeadTemp.X, TailTemp.Y) : new Point(TailTemp.X, HeadTemp.Y);
                                        pointsRelativ = new List<Point>() { headerPoint, HeadTemp, testPoint, TailTemp, tailPoint };
                                        DrawLine(pointsRelativ);
                                        return;
                                    }
                                    if (tailNodeExt.X + tailNodeExt.Width > HeadTemp.X)
                                    {
                                        HeadTemp.X = tailNodeExt.X + tailNodeExt.Width;
                                    }
                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, new Point(HeadTemp.X, TailTemp.Y), TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                                else // 头下尾上
                                {
                                    if (HeaderPort == LinkPortPosition.RightTop)
                                        HeadTemp = headRels[0];
                                    if (tailRect.Y + tailRect.Height > headRect.Y)
                                        fitPoint.Y = (headerPoint.Y + tailPoint.Y) / 2;
                                    if (TailTemp.Y > fitPoint.Y)
                                        TailTemp.Y = fitPoint.Y;
                                    if (HeadTemp.Y < fitPoint.Y)
                                        HeadTemp.Y = fitPoint.Y;
                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, new Point(HeadTemp.X, fitPoint.Y), new Point(TailTemp.X, fitPoint.Y), TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                            }
                            else // 头左尾右
                            {
                                if (tailPoint.Y >= headerPoint.Y) //头上尾下
                                {
                                    if ((tailNodeExt.X > headerPoint.X || tailPoint.Y < headNodeExt.Y + headNodeExt.Height)
                                     && HeaderPort == LinkPortPosition.RightBottom)
                                    {
                                        HeadTemp = headRels[2];
                                        testPoint = TailTemp.Y > HeadTemp.Y ? new Point(HeadTemp.X, TailTemp.Y) : new Point(TailTemp.X, HeadTemp.Y);
                                        pointsRelativ = new List<Point>() { headerPoint, HeadTemp, testPoint, TailTemp, tailPoint };
                                        DrawLine(pointsRelativ);
                                        return;
                                    }
                                    if (tailRect.X < headerPoint.X)
                                        fitPoint.X = (headerPoint.X + tailPoint.X) / 2;
                                    if (HeadTemp.X > fitPoint.X)
                                        HeadTemp.X = fitPoint.X;
                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, new Point(fitPoint.X, HeadTemp.Y), new Point(fitPoint.X, TailTemp.Y), TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                                else // 头下尾上
                                {
                                    if (TailTemp.X < HeadTemp.X)
                                        HeadTemp.X = TailTemp.X;
                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, new Point(TailTemp.X, HeadTemp.Y), TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                            }
                        case PosRelation.Left: // 头右 尾左

                            if (headerPoint.X <= tailPoint.X)
                            {
                                // 两种特殊情况
                                if (tailPoint.Y <= headNodeExt.Y && HeaderPort == LinkPortPosition.RightTop)
                                {
                                    HeadTemp = headRels[0];
                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, new Point(HeadTemp.X, TailTemp.Y), TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                                if (tailPoint.Y >= headNodeExt.Y + headNodeExt.Height && HeaderPort == LinkPortPosition.RightBottom)
                                {
                                    HeadTemp = headRels[2];
                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, new Point(HeadTemp.X, TailTemp.Y), TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }

                                if (TailTemp.X < fitPoint.X)
                                    TailTemp.X = fitPoint.X;
                                if (HeadTemp.X > fitPoint.X)
                                    HeadTemp.X = fitPoint.X;
                                headHelps.Add(new Point(fitPoint.X, HeadTemp.Y));
                                tailHelps.Add(new Point(fitPoint.X, TailTemp.Y));
                            }
                            else
                            {
                                if (HeadTemp.Y == TailTemp.Y)
                                {
                                    if (HeadTemp.Y < tailCenter.Y) // 上转
                                    {
                                        if (HeaderPort == LinkPortPosition.RightTop)
                                        {
                                            HeadTemp = headRels[0];
                                        }
                                        else
                                        {
                                            headHelps.Add(new Point(headNodeExt.X + headNodeExt.Width, HeadTemp.Y));
                                        }
                                        if (tailNodeExt.Y < headNodeExt.Y)
                                        {
                                            headHelps.Add(new Point(HeadTemp.X, tailNodeExt.Y));
                                            tailHelps.Add(new Point(TailTemp.X, tailNodeExt.Y));
                                        }
                                        else
                                        {
                                            tailHelps.Add(new Point(TailTemp.X, headNodeExt.Y));
                                        }
                                    }
                                    else // 下转
                                    {
                                        if (HeaderPort == LinkPortPosition.RightBottom)
                                        {
                                            HeadTemp = headRels[2];
                                        }
                                        else
                                        {
                                            headHelps.Add(new Point(headNodeExt.X + headNodeExt.Width, headNodeExt.Y + headNodeExt.Height));
                                        }
                                        if (tailNodeExt.Y + tailNodeExt.Height > headNodeExt.Y + headNodeExt.Height)
                                        {
                                            headHelps.Add(new Point(HeadTemp.X, tailNodeExt.Y + tailNodeExt.Height));
                                            tailHelps.Add(new Point(TailTemp.X, tailNodeExt.Y + tailNodeExt.Height));
                                        }
                                        else
                                        {
                                            tailHelps.Add(new Point(TailTemp.X, headNodeExt.Y + headNodeExt.Height));
                                        }
                                    }
                                }
                                else
                                {
                                    headHelps.Add(new Point(HeadTemp.X, fitPoint.Y));
                                    tailHelps.Add(new Point(TailTemp.X, fitPoint.Y));
                                }
                            }
                            pointsRelativ = new List<Point>() { headerPoint, HeadTemp };
                            pointsRelativ.AddRange(headHelps);
                            pointsRelativ.AddRange(tailHelps);
                            pointsRelativ.Add(TailTemp);
                            pointsRelativ.Add(tailPoint);
                            DrawLine(pointsRelativ);
                            return;
                        case PosRelation.Right: //  头右 尾右
                            // 尾在头左面
                            if (tailNodeExt.X + tailNodeExt.Width <= headNodeExt.X
                               && TailTemp.Y > headNodeExt.Y
                               && TailTemp.Y < headNodeExt.Y + headNodeExt.Height)
                            {
                                if (tailCenter.Y <= headCenter.Y) // 上转
                                {
                                    if (HeaderPort == LinkPortPosition.RightTop)
                                    {
                                        HeadTemp = headRels[0];
                                    }
                                    else
                                    {
                                        headHelps.Add(new Point(HeadTemp.X, headNodeExt.Y));
                                    }
                                    headHelps.Add(new Point(fitPoint.X, headNodeExt.Y));
                                    tailHelps.Add(new Point(fitPoint.X, TailTemp.Y));
                                }
                                else // 下转
                                {
                                    if (HeaderPort == LinkPortPosition.RightBottom)
                                    {
                                        HeadTemp = headRels[2];
                                    }
                                    else
                                    {
                                        headHelps.Add(new Point(HeadTemp.X, headNodeExt.Y + headNodeExt.Height));
                                    }
                                    headHelps.Add(new Point(fitPoint.X, headNodeExt.Y + headNodeExt.Height));
                                    tailHelps.Add(new Point(fitPoint.X, TailTemp.Y));
                                }
                                pointsRelativ = new List<Point>() { headerPoint, HeadTemp };
                                pointsRelativ.AddRange(headHelps);
                                pointsRelativ.AddRange(tailHelps);
                                pointsRelativ.Add(TailTemp);
                                pointsRelativ.Add(tailPoint);
                                DrawLine(pointsRelativ);
                                return;
                            }
                            // 尾在头右面
                            if (tailNodeExt.X >= headNodeExt.X + headNodeExt.Width
                              && HeadTemp.Y > tailNodeExt.Y
                              && HeadTemp.Y < tailNodeExt.Y + tailNodeExt.Height)
                            {
                                headHelps.Add(new Point(fitPoint.X, HeadTemp.Y));
                                if (tailCenter.Y >= headCenter.Y) // 上转
                                {
                                    headHelps.Add(new Point(fitPoint.X, tailNodeExt.Y));
                                    tailHelps.Add(new Point(TailTemp.X, tailNodeExt.Y));
                                }
                                else // 下转
                                {
                                    headHelps.Add(new Point(fitPoint.X, tailNodeExt.Y + tailNodeExt.Height));
                                    tailHelps.Add(new Point(TailTemp.X, tailNodeExt.Y + tailNodeExt.Height));
                                }
                                pointsRelativ = new List<Point>() { headerPoint, HeadTemp };
                                pointsRelativ.AddRange(headHelps);
                                pointsRelativ.AddRange(tailHelps);
                                pointsRelativ.Add(TailTemp);
                                pointsRelativ.Add(tailPoint);
                                DrawLine(pointsRelativ);
                                return;
                            }
                            // 其他情况
                            testPoint = TailTemp.X <= HeadTemp.X ? new Point(HeadTemp.X, TailTemp.Y) : new Point(TailTemp.X, HeadTemp.Y);
                            pointsRelativ = new List<Point>() { headerPoint, HeadTemp, testPoint, TailTemp, tailPoint };
                            DrawLine(pointsRelativ);
                            return;
                        default:
                            break;
                    }
                    break;
                case PosRelation.Down: // 头下
                    switch (tailRel)
                    {
                        case PosRelation.Up: //  头下 尾上
                            if (headerPoint.Y <= tailPoint.Y)
                            {
                                // 两种特殊情况
                                if (tailPoint.X <= headNodeExt.X && HeaderPort == LinkPortPosition.LeftBottom)
                                {
                                    HeadTemp = headRels[2];
                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, new Point(TailTemp.X, HeadTemp.Y), TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                                if (tailPoint.X >= headNodeExt.X + headNodeExt.Width && HeaderPort == LinkPortPosition.RightBottom)
                                {
                                    HeadTemp = headRels[0];
                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, new Point(TailTemp.X, HeadTemp.Y), TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                                if (fitPoint.Y < HeadTemp.Y)
                                    HeadTemp.Y = fitPoint.Y;
                                if (fitPoint.Y > TailTemp.Y)
                                    TailTemp.Y = fitPoint.Y;
                                headHelps.Add(new Point(HeadTemp.X, fitPoint.Y));
                                tailHelps.Add(new Point(TailTemp.X, fitPoint.Y));
                            }
                            else
                            {
                                if (TailTemp.X == HeadTemp.X)
                                {
                                    if (HeadTemp.X < tailCenter.X) // 左转
                                    {
                                        if (HeaderPort == LinkPortPosition.LeftBottom)
                                        {
                                            HeadTemp = headRels[2];
                                        }
                                        else
                                        {
                                            headHelps.Add(new Point(headNodeExt.X, HeadTemp.Y));
                                        }
                                        if (tailNodeExt.X < headNodeExt.X)
                                        {
                                            headHelps.Add(new Point(tailNodeExt.X, HeadTemp.Y));
                                            tailHelps.Add(new Point(tailNodeExt.X, TailTemp.Y));
                                        }
                                        else
                                        {
                                            tailHelps.Add(new Point(headNodeExt.X, TailTemp.Y));
                                        }
                                    }
                                    else // 右转
                                    {
                                        if (HeaderPort == LinkPortPosition.RightBottom)
                                        {
                                            HeadTemp = headRels[0];
                                        }
                                        else
                                        {
                                            headHelps.Add(new Point(headNodeExt.X + headNodeExt.Width, HeadTemp.Y));
                                        }
                                        if (tailNodeExt.X + tailNodeExt.Width > headNodeExt.X + headNodeExt.Width)
                                        {
                                            headHelps.Add(new Point(tailNodeExt.X + tailNodeExt.Width, HeadTemp.Y));
                                            tailHelps.Add(new Point(tailNodeExt.X + tailNodeExt.Width, TailTemp.Y));
                                        }
                                        else
                                        {
                                            tailHelps.Add(new Point(headNodeExt.X + headNodeExt.Width, TailTemp.Y));
                                        }
                                    }
                                }
                                else
                                {
                                    headHelps.Add(new Point(fitPoint.X, HeadTemp.Y));
                                    tailHelps.Add(new Point(fitPoint.X, TailTemp.Y));
                                }
                            }
                            pointsRelativ = new List<Point>() { headerPoint, HeadTemp };
                            pointsRelativ.AddRange(headHelps);
                            pointsRelativ.AddRange(tailHelps);
                            pointsRelativ.Add(TailTemp);
                            pointsRelativ.Add(tailPoint);
                            DrawLine(pointsRelativ);
                            return;
                        case PosRelation.Down:  // 头下 尾下
                            if (tailNodeExt.Y + tailNodeExt.Height < headNodeExt.Y
                               && TailTemp.X > headNodeExt.X
                               && TailTemp.X < headNodeExt.X + headNodeExt.Width)  // 尾上 头下不相交
                            {
                                if (tailCenter.X <= HeadTemp.X) // 尾偏左，左转，与尾在下面情况不同
                                {
                                    if (HeaderPort == LinkPortPosition.LeftBottom)
                                    {
                                        HeadTemp = headRels[2];
                                    }
                                    else
                                    {
                                        headHelps.Add(new Point(headNodeExt.X, HeadTemp.Y));
                                    }
                                    headHelps.Add(new Point(headNodeExt.X, fitPoint.Y));
                                    tailHelps.Add(new Point(TailTemp.X, fitPoint.Y));
                                }
                                else // 尾偏右，右转，与尾在下面情况不同
                                {
                                    if (HeaderPort == LinkPortPosition.RightBottom)
                                    {
                                        HeadTemp = headRels[0];
                                    }
                                    else
                                    {
                                        headHelps.Add(new Point(headNodeExt.X + headNodeExt.Width, HeadTemp.Y));
                                    }
                                    headHelps.Add(new Point(headNodeExt.X + headNodeExt.Width, fitPoint.Y));
                                    tailHelps.Add(new Point(TailTemp.X, fitPoint.Y));
                                }
                                pointsRelativ = new List<Point>() { headerPoint, HeadTemp };
                                pointsRelativ.AddRange(headHelps);
                                pointsRelativ.AddRange(tailHelps);
                                pointsRelativ.Add(TailTemp);
                                pointsRelativ.Add(tailPoint);
                                DrawLine(pointsRelativ);
                                return;
                            }
                            if (tailNodeExt.Y > headNodeExt.Y + headNodeExt.Height // 头上尾下
                               && HeadTemp.X > tailNodeExt.X
                               && HeadTemp.X < tailNodeExt.X + tailNodeExt.Width)
                            {
                                headHelps.Add(new Point(HeadTemp.X, fitPoint.Y));
                                if (tailCenter.X <= HeadTemp.X) // 尾偏左，右转
                                {
                                    headHelps.Add(new Point(tailNodeExt.X + tailNodeExt.Width, fitPoint.Y));
                                    tailHelps.Add(new Point(tailNodeExt.X + tailNodeExt.Width, TailTemp.Y));
                                }
                                else // 尾偏右，左转
                                {
                                    headHelps.Add(new Point(tailNodeExt.X, fitPoint.Y));
                                    tailHelps.Add(new Point(tailNodeExt.X, TailTemp.Y));
                                }
                                pointsRelativ = new List<Point>() { headerPoint, HeadTemp };
                                pointsRelativ.AddRange(headHelps);
                                pointsRelativ.AddRange(tailHelps);
                                pointsRelativ.Add(TailTemp);
                                pointsRelativ.Add(tailPoint);
                                DrawLine(pointsRelativ);
                                return;
                            }
                            //其他情况
                            testPoint = TailTemp.Y <= HeadTemp.Y ? new Point(TailTemp.X, HeadTemp.Y) : new Point(HeadTemp.X, TailTemp.Y);
                            pointsRelativ = new List<Point>() { headerPoint, HeadTemp, testPoint, TailTemp, tailPoint };
                            DrawLine(pointsRelativ);
                            return;
                        case PosRelation.Left: // 头下 尾左
                            if (headerPoint.Y >= tailPoint.Y) // 尾 连接点 在 头上
                            {
                                if (TailTemp.X < HeadTemp.X) // 尾连接点在 头节点左面
                                {
                                    if (tailNodeExt.Y + tailNodeExt.Height > HeadTemp.Y)
                                    {
                                        HeadTemp.Y = tailNodeExt.Y + tailNodeExt.Height;
                                    }
                                    if ((tailNodeExt.Y + tailNodeExt.Height < headerPoint.Y || TailTemp.X > headNodeExt.X)
                                        && HeaderPort == LinkPortPosition.LeftBottom)
                                    {
                                        HeadTemp = headRels[2];
                                    }
                                    testPoint = TailTemp.X < HeadTemp.X ? new Point(TailTemp.X, HeadTemp.Y) : new Point(HeadTemp.X, TailTemp.Y);
                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, testPoint, TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                                else // 尾连接点在头节点右面
                                {
                                    if (tailRect.X < headRect.X + headRect.Width)
                                        fitPoint.X = (headerPoint.X + tailPoint.X) / 2;
                                    if (fitPoint.X < HeadTemp.X)
                                        HeadTemp.X = fitPoint.X;
                                    if (fitPoint.X > TailTemp.X)
                                        TailTemp.X = fitPoint.X;

                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, new Point(fitPoint.X, HeadTemp.Y), new Point(fitPoint.X, TailTemp.Y), TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                            }
                            else  // 尾连接点 y 在 头节点 下面
                            {
                                if (TailTemp.X < headerPoint.X) // 尾在头左
                                {
                                    if (HeaderPort == LinkPortPosition.LeftBottom) // 特殊情况
                                    {
                                        HeadTemp = headRels[2];
                                        testPoint = TailTemp.X < HeadTemp.X ? new Point(TailTemp.X, HeadTemp.Y) : new Point(HeadTemp.X, TailTemp.Y);
                                        pointsRelativ = new List<Point>() { headerPoint, HeadTemp, testPoint, TailTemp, tailPoint };
                                        DrawLine(pointsRelativ);
                                        return;
                                    }
                                    if (tailRect.Y < headRect.Y + headRect.Height)
                                        fitPoint.Y = (headerPoint.Y + tailPoint.Y) / 2;
                                    if (HeadTemp.Y > fitPoint.Y)
                                        HeadTemp.Y = fitPoint.Y;
                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, new Point(HeadTemp.X, fitPoint.Y), new Point(TailTemp.X, fitPoint.Y), TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                                else // 尾在头右
                                {
                                    if (HeadTemp.Y > tailPoint.Y)
                                        HeadTemp.Y = tailPoint.Y;
                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, new Point(HeadTemp.X, TailTemp.Y), TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                            }
                        case PosRelation.Right: //  头下 尾右
                            if (headerPoint.Y >= tailPoint.Y) // 尾 连接点 在 头上
                            {
                                if (TailTemp.X < HeadTemp.X) // 尾连接点在 头节点左面
                                {
                                    if (tailRect.X + tailRect.Width > headRect.X)
                                        fitPoint.X = (headerPoint.X + tailPoint.X) / 2;
                                    if (fitPoint.X > HeadTemp.X)
                                        HeadTemp.X = fitPoint.X;
                                    if (fitPoint.X < TailTemp.X)
                                        TailTemp.X = fitPoint.X;
                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, new Point(fitPoint.X, HeadTemp.Y), new Point(fitPoint.X, TailTemp.Y), TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                                else // 尾连接点在头节点右面
                                {
                                    if (tailNodeExt.Y + tailNodeExt.Height > HeadTemp.Y)
                                    {
                                        HeadTemp.Y = tailNodeExt.Y + tailNodeExt.Height;
                                    }
                                    if ((tailNodeExt.Y + tailNodeExt.Height < headerPoint.Y || TailTemp.X > headNodeExt.X)
                                        && HeaderPort == LinkPortPosition.RightBottom)
                                    {
                                        HeadTemp = headRels[0];
                                    }
                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, new Point(TailTemp.X, HeadTemp.Y), TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                            }
                            else  // 尾连接点 y 在 头节点 下面
                            {
                                if (TailTemp.X < headerPoint.X) // 尾在头左
                                {
                                    if (HeadTemp.Y > tailPoint.Y)
                                        HeadTemp.Y = tailPoint.Y;
                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, new Point(HeadTemp.X, TailTemp.Y), TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                                else // 尾在头右
                                {
                                    if (HeaderPort == LinkPortPosition.RightBottom) // 特殊情况
                                    {
                                        HeadTemp = headRels[0];
                                        testPoint = TailTemp.X < HeadTemp.X ? new Point(TailTemp.X, HeadTemp.Y) : new Point(HeadTemp.X, TailTemp.Y);
                                        pointsRelativ = new List<Point>() { headerPoint, HeadTemp, testPoint, TailTemp, tailPoint };
                                        DrawLine(pointsRelativ);
                                        return;
                                    }
                                    if (tailRect.Y < headRect.Y + headRect.Height)
                                        fitPoint.Y = (headerPoint.Y + tailPoint.Y) / 2;
                                    if (HeadTemp.Y > fitPoint.Y)
                                        HeadTemp.Y = fitPoint.Y;
                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, new Point(HeadTemp.X, fitPoint.Y), new Point(TailTemp.X, fitPoint.Y), TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                            }
                        default:
                            break;
                    }
                    break;
                case PosRelation.Left: // 头左
                    switch (tailRel)
                    {
                        case PosRelation.Up: // 头左 尾上
                            // 左右划分
                            if (tailPoint.X <= headerPoint.X) // 尾左头右
                            {
                                if (tailPoint.Y >= headerPoint.Y) //头上尾下
                                {
                                    if (TailTemp.X > HeadTemp.X)
                                        HeadTemp.X = TailTemp.X;
                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, new Point(TailTemp.X, HeadTemp.Y), TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                                else // 头下尾上
                                {
                                    if (HeaderPort == LinkPortPosition.LeftTop) // 特殊情况
                                    {
                                        HeadTemp = headRels[2];
                                        testPoint = TailTemp.Y > HeadTemp.Y ? new Point(HeadTemp.X, TailTemp.Y) : new Point(TailTemp.X, HeadTemp.Y);
                                        pointsRelativ = new List<Point>() { headerPoint, HeadTemp, testPoint, TailTemp, tailPoint };
                                        DrawLine(pointsRelativ);
                                        return;
                                    }
                                    if (tailRect.X + tailRect.Width > headRect.X)
                                        fitPoint.X = (headerPoint.X + tailPoint.X) / 2;
                                    if (HeadTemp.X < fitPoint.X)
                                        HeadTemp.X = fitPoint.X;
                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, new Point(fitPoint.X, HeadTemp.Y), new Point(fitPoint.X, TailTemp.Y), TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                            }
                            else // 头左尾右
                            {
                                if (tailPoint.Y >= headerPoint.Y) //头上尾下
                                {
                                    if (HeaderPort == LinkPortPosition.LeftBottom)
                                        HeadTemp = headRels[0];
                                    if (tailRect.Y < headRect.Y + headRect.Height)
                                        fitPoint.Y = (headerPoint.Y + tailPoint.Y) / 2;
                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, new Point(HeadTemp.X, fitPoint.Y), new Point(TailTemp.X, fitPoint.Y), TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                                else // 头下尾上
                                {
                                    if ((tailRect.X > headerPoint.X || TailTemp.Y > headNodeExt.Y)
                                       && HeaderPort == LinkPortPosition.LeftTop)
                                    {
                                        HeadTemp = headRels[2];
                                        testPoint = TailTemp.Y < HeadTemp.Y ? new Point(HeadTemp.X, TailTemp.Y) : new Point(TailTemp.X, HeadTemp.Y);
                                        pointsRelativ = new List<Point>() { headerPoint, HeadTemp, testPoint, TailTemp, tailPoint };
                                        DrawLine(pointsRelativ);
                                        return;
                                    }
                                    if (tailNodeExt.X < HeadTemp.X)
                                        HeadTemp.X = tailNodeExt.X;
                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, new Point(HeadTemp.X, TailTemp.Y), TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                            }
                        case PosRelation.Down: //  头左 尾下
                            if (tailPoint.X <= headerPoint.X) // 尾左头右
                            {
                                if (tailPoint.Y >= headerPoint.Y) //头上尾下
                                {
                                    if (HeaderPort == LinkPortPosition.LeftBottom) // 特殊情况
                                    {
                                        HeadTemp = headRels[0];
                                        testPoint = TailTemp.Y > HeadTemp.Y ? new Point(HeadTemp.X, TailTemp.Y) : new Point(TailTemp.X, HeadTemp.Y);
                                        pointsRelativ = new List<Point>() { headerPoint, HeadTemp, testPoint, TailTemp, tailPoint };
                                        DrawLine(pointsRelativ);
                                        return;
                                    }
                                    if (tailRect.X + tailRect.Width > headRect.X)
                                        fitPoint.X = (headerPoint.X + tailPoint.X) / 2;
                                    if (HeadTemp.X < fitPoint.X)
                                        HeadTemp.X = fitPoint.X;
                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, new Point(fitPoint.X, HeadTemp.Y), new Point(fitPoint.X, TailTemp.Y), TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                                else // 头下尾上
                                {
                                    if (TailTemp.X > HeadTemp.X)
                                        HeadTemp.X = TailTemp.X;
                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, new Point(TailTemp.X, HeadTemp.Y), TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                            }
                            else // 头左尾右
                            {
                                if (tailPoint.Y >= headerPoint.Y) //头上尾下
                                {
                                    if ((tailRect.X > headerPoint.X || TailTemp.Y > headNodeExt.Y)
                                       && HeaderPort == LinkPortPosition.LeftBottom)
                                    {
                                        HeadTemp = headRels[0];
                                        if (TailTemp.Y < HeadTemp.Y)
                                            testPoint = TailTemp.Y < HeadTemp.Y ? new Point(HeadTemp.X, TailTemp.Y) : new Point(TailTemp.X, HeadTemp.Y);
                                        pointsRelativ = new List<Point>() { headerPoint, HeadTemp, testPoint, TailTemp, tailPoint };
                                        DrawLine(pointsRelativ);
                                        return;
                                    }
                                    if (tailNodeExt.X < HeadTemp.X)
                                        HeadTemp.X = tailNodeExt.X;
                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, new Point(HeadTemp.X, TailTemp.Y), TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                                else // 头下尾上
                                {
                                    if (HeaderPort == LinkPortPosition.LeftTop)
                                        HeadTemp = headRels[2];
                                    if (tailRect.Y < headRect.Y + headRect.Height)
                                        fitPoint.Y = (headerPoint.Y + tailPoint.Y) / 2;
                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, new Point(HeadTemp.X, fitPoint.Y), new Point(TailTemp.X, fitPoint.Y), TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                            }

                        case PosRelation.Left: //  头左 尾左
                            if (tailNodeExt.X + tailNodeExt.Width <= headNodeExt.X
                              && HeadTemp.Y > tailNodeExt.Y
                              && HeadTemp.Y < tailNodeExt.Y + tailNodeExt.Height)
                            {
                                headHelps.Add(new Point(fitPoint.X, HeadTemp.Y));
                                if (tailCenter.Y < HeadTemp.Y) // 从下面绕
                                {
                                    headHelps.Add(new Point(fitPoint.X, tailNodeExt.Y + tailNodeExt.Height));
                                    tailHelps.Add(new Point(TailTemp.X, tailNodeExt.Y + tailNodeExt.Height));
                                }
                                else // 从上面绕
                                {
                                    headHelps.Add(new Point(fitPoint.X, tailNodeExt.Y));
                                    tailHelps.Add(new Point(TailTemp.X, tailNodeExt.Y));
                                }
                                pointsRelativ = new List<Point>() { headerPoint, HeadTemp };
                                pointsRelativ.AddRange(headHelps);
                                pointsRelativ.AddRange(tailHelps);
                                pointsRelativ.Add(TailTemp);
                                pointsRelativ.Add(tailPoint);
                                DrawLine(pointsRelativ);
                                return;
                            }

                            if (headNodeExt.X + headNodeExt.Width < tailNodeExt.X
                              && TailTemp.Y > headNodeExt.Y
                              && TailTemp.Y < headNodeExt.Y + headNodeExt.Height)
                            {
                                if (tailCenter.Y < headCenter.Y) // 从上面绕
                                {
                                    if (HeaderPort == LinkPortPosition.LeftTop)
                                    {
                                        HeadTemp = headRels[2];
                                    }
                                    else
                                    {
                                        headHelps.Add(new Point(headNodeExt.X, headNodeExt.Y));
                                    }
                                    headHelps.Add(new Point(fitPoint.X, headNodeExt.Y));
                                    tailHelps.Add(new Point(fitPoint.X, TailTemp.Y));
                                }
                                else // 从下面绕
                                {
                                    if (HeaderPort == LinkPortPosition.LeftBottom)
                                    {
                                        HeadTemp = headRels[0];
                                    }
                                    else
                                    {
                                        headHelps.Add(new Point(headNodeExt.X, headNodeExt.Y + headNodeExt.Height));
                                    }
                                    headHelps.Add(new Point(fitPoint.X, headNodeExt.Y + headNodeExt.Height));
                                    tailHelps.Add(new Point(fitPoint.X, TailTemp.Y));
                                }
                                pointsRelativ = new List<Point>() { headerPoint, HeadTemp };
                                pointsRelativ.AddRange(headHelps);
                                pointsRelativ.AddRange(tailHelps);
                                pointsRelativ.Add(TailTemp);
                                pointsRelativ.Add(tailPoint);
                                DrawLine(pointsRelativ);
                                return;
                            }
                            testPoint = TailTemp.X < HeadTemp.X ? new Point(TailTemp.X, HeadTemp.Y) : new Point(HeadTemp.X, TailTemp.Y);
                            pointsRelativ = new List<Point>() { headerPoint, HeadTemp, testPoint, TailTemp, tailPoint };
                            DrawLine(pointsRelativ);
                            return;

                        case PosRelation.Right: //  头左 尾右
                            if (tailPoint.X <= headerPoint.X)
                            {
                                // 两种特殊情况
                                if (tailPoint.Y <= headNodeExt.Y && HeaderPort == LinkPortPosition.LeftTop)
                                {
                                    HeadTemp = headRels[2];
                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, new Point(HeadTemp.X, TailTemp.Y), TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                                if (tailPoint.Y >= headNodeExt.Y + headNodeExt.Height && HeaderPort == LinkPortPosition.LeftBottom)
                                {
                                    HeadTemp = headRels[0];
                                    pointsRelativ = new List<Point>() { headerPoint, HeadTemp, new Point(HeadTemp.X, TailTemp.Y), TailTemp, tailPoint };
                                    DrawLine(pointsRelativ);
                                    return;
                                }
                                if (TailTemp.X > fitPoint.X)
                                    TailTemp.X = fitPoint.X;
                                if (HeadTemp.X < fitPoint.X)
                                    HeadTemp.X = fitPoint.X;
                                headHelps.Add(new Point(fitPoint.X, HeadTemp.Y));
                                tailHelps.Add(new Point(fitPoint.X, TailTemp.Y));
                            }
                            else
                            {
                                if (HeadTemp.Y == TailTemp.Y)
                                {
                                    if (HeadTemp.Y < tailCenter.Y) // 上转
                                    {
                                        if (HeaderPort == LinkPortPosition.LeftTop)
                                        {
                                            HeadTemp = headRels[2];
                                        }
                                        else
                                        {
                                            headHelps.Add(new Point(headNodeExt.X, HeadTemp.Y));
                                        }
                                        if (tailNodeExt.Y < headNodeExt.Y)
                                        {
                                            headHelps.Add(new Point(HeadTemp.X, tailNodeExt.Y));
                                            tailHelps.Add(new Point(TailTemp.X, tailNodeExt.Y));
                                        }
                                        else
                                        {
                                            tailHelps.Add(new Point(TailTemp.X, headNodeExt.Y));
                                        }
                                    }
                                    else // 下转
                                    {
                                        if (HeaderPort == LinkPortPosition.LeftBottom)
                                        {
                                            HeadTemp = headRels[0];
                                        }
                                        else
                                        {
                                            headHelps.Add(new Point(headNodeExt.X, headNodeExt.Y + headNodeExt.Height));
                                        }
                                        if (tailNodeExt.Y > headNodeExt.Y + headNodeExt.Height)
                                        {
                                            headHelps.Add(new Point(HeadTemp.X, tailNodeExt.Y));
                                            tailHelps.Add(new Point(TailTemp.X, tailNodeExt.Y));
                                        }
                                        else
                                        {
                                            tailHelps.Add(new Point(TailTemp.X, headNodeExt.Y));
                                        }
                                    }
                                }
                                else
                                {
                                    headHelps.Add(new Point(HeadTemp.X, fitPoint.Y));
                                    tailHelps.Add(new Point(TailTemp.X, fitPoint.Y));
                                }
                            }
                            pointsRelativ = new List<Point>() { headerPoint, HeadTemp };
                            pointsRelativ.AddRange(headHelps);
                            pointsRelativ.AddRange(tailHelps);
                            pointsRelativ.Add(TailTemp);
                            pointsRelativ.Add(tailPoint);
                            DrawLine(pointsRelativ);
                            return;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 最后画线函数，包括更新thumbs位置，头图形及画线函数。
        /// </summary>
        /// <param name="p_points"></param>
        void DrawLine(List<Point> p_points)
        {
            UpdateThumbsPos();
            UpdateEndLayout(p_points);
            UpdatePathGeometry(p_points);
        }

        /// <summary>
        /// 从  p_rect 的 start 点，引出正交线到 p_middle 为远点的坐标轴上，返回经过
        /// 的点。
        /// </summary>
        /// <param name="p_rect"> node extention 范围的矩形</param>
        /// <param name="p_start"></param>
        /// <param name="p_ext"></param>
        /// <param name="p_middle"></param>
        /// <param name="p_isLandscape"></param>
        /// <returns></returns>
        List<Point> GetLinesPoints(Rect p_rect, Point p_start, Point p_ext, Point p_middle, bool p_isLandscape)
        {
            double half;
            Point select, select2, add;
            Point center = GetRectCenter(p_rect);

            if (p_isLandscape) //横向
            {
                if (p_start.Y == p_ext.Y) //出发点是横向的
                {
                    if (p_ext.X < p_start.X == p_middle.X <= p_ext.X) // 同侧
                    {
                        add = new Point(p_middle.X, p_ext.Y);
                        return new List<Point>() { p_start, p_ext, add };
                    }
                    else // 不同侧
                    {
                        half = p_rect.Height / 2;
                        select = new Point(p_ext.X, center.Y + (p_middle.Y > center.Y ? half : -half));
                        add = new Point(p_middle.X, select.Y);
                        return new List<Point>() { p_start, p_ext, select, add };
                    }
                }
                else // 出发点是纵向的。
                {
                    if (p_ext.Y > p_start.Y == p_middle.Y >= p_ext.Y) // 同侧
                    {
                        add = new Point(p_middle.X, p_ext.Y);
                        return new List<Point>() { p_start, p_ext, add };
                    }
                    else // 不同侧
                    {
                        half = p_rect.Width / 2;
                        if (p_middle.X <= p_rect.X || p_middle.X >= p_rect.X + p_rect.Width)
                        {
                            select = new Point(center.X + (p_middle.X > center.X ? half : -half), p_ext.Y);
                            add = new Point(p_middle.X, select.Y);
                            return new List<Point>() { p_start, p_ext, select, add };
                        }
                        else
                        {
                            select = new Point(center.X + (p_middle.X > center.X ? half : -half), p_ext.Y);
                            select2 = new Point(select.X, select.Y + (p_middle.Y >= center.Y ? p_rect.Height : -p_rect.Height));
                            add = new Point(p_middle.X, select2.Y);
                            return new List<Point>() { p_start, p_ext, select, select2, add };
                        }
                    }
                }
            }
            else // 纵向
            {
                if (p_ext.X == p_start.X) //出发点是纵向的
                {
                    if (p_ext.Y < p_start.Y == p_middle.Y <= p_ext.Y) // 同侧
                    {
                        add = new Point(p_ext.X, p_middle.Y);
                        return new List<Point>() { p_start, p_ext, add };
                    }
                    else // 不同侧
                    {
                        half = p_rect.Width / 2;
                        select = new Point(center.X + (p_middle.X > center.X ? half : -half), p_ext.Y);
                        add = new Point(select.X, p_middle.Y);
                        return new List<Point>() { p_start, p_ext, select, add };
                    }
                }
                else // 出发点是横向的
                {
                    if (p_middle.X <= p_ext.X == p_ext.X < p_start.X)
                    {
                        add = new Point(p_ext.X, p_middle.Y);
                        return new List<Point>() { p_start, p_ext, add };
                    }
                    else
                    {
                        half = p_rect.Height / 2;
                        if (p_middle.Y <= p_rect.Y || p_middle.Y >= p_rect.Y + p_rect.Height)
                        {
                            select = new Point(p_ext.X, center.Y + (p_middle.Y > center.Y ? half : -half));
                            add = new Point(select.X, p_middle.Y);
                            return new List<Point>() { p_start, p_ext, select, add };
                        }
                        else
                        {
                            select = new Point(p_ext.X, center.Y + (p_middle.Y > center.Y ? half : -half));
                            select2 = new Point(select.X + (p_middle.X > center.X ? p_rect.Width : -p_rect.Width), select.Y);
                            add = new Point(select2.X, p_middle.Y);
                            return new List<Point>() { p_start, p_ext, select, select2, add };
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 更新端点图形
        /// </summary>
        /// <param name="p_points"> 用于生成最后连线的点</param>
        void UpdateEndLayout(List<Point> p_points)
        {
            Point end, ext;
            int count = p_points.Count;
            // 更新头
            if (HeaderGeometry != null)
            {
                end = p_points[0];
                ext = p_points[1];
                switch (GetPosRelation(end, ext))
                {
                    case PosRelation.Left:
                        _headPathTransform.Rotation = 180.0;
                        HeaderLeft = end.X - HeadTailSize;
                        HeaderTop = end.Y - HeadTailSize / 2;
                        end.X = HeaderLeft;
                        break;
                    case PosRelation.Down:
                        _headPathTransform.Rotation = 90.0;
                        HeaderLeft = end.X - HeadTailSize / 2;
                        HeaderTop = end.Y;
                        end.Y = HeaderTop + HeadTailSize;
                        break;
                    case PosRelation.Right:
                        _headPathTransform.Rotation = 0.0;
                        HeaderLeft = end.X;
                        HeaderTop = end.Y - HeadTailSize / 2;
                        end.X = HeaderLeft + HeadTailSize;
                        break;
                    case PosRelation.Up:
                        _headPathTransform.Rotation = -90.0;
                        HeaderLeft = end.X - HeadTailSize / 2;
                        HeaderTop = end.Y - HeadTailSize;
                        end.Y = HeaderTop;
                        break;
                }
                p_points[0] = end;
            }

            // 更新尾
            if (TailGeometry != null)
            {
                end = p_points[count - 1];
                ext = p_points[count - 2];
                switch (GetPosRelation(end, ext))
                {
                    case PosRelation.Left:
                        _tailPathTransform.Rotation = 0.0;
                        TailLeft = end.X - HeadTailSize;
                        TailTop = end.Y - HeadTailSize / 2;
                        end.X = TailLeft;
                        break;
                    case PosRelation.Down:
                        _tailPathTransform.Rotation = -90.0;
                        TailLeft = end.X - HeadTailSize / 2;
                        TailTop = end.Y;
                        end.Y = TailTop + HeadTailSize;
                        break;
                    case PosRelation.Right:
                        _tailPathTransform.Rotation = 180.0;
                        TailLeft = end.X;
                        TailTop = end.Y - HeadTailSize / 2;
                        end.X = TailLeft + HeadTailSize;
                        break;
                    case PosRelation.Up:
                        _tailPathTransform.Rotation = 90.0;
                        TailLeft = end.X - HeadTailSize / 2;
                        TailTop = end.Y - HeadTailSize;
                        end.Y = TailTop;
                        break;
                }
                p_points[count - 1] = end;
            }
        }

        /// <summary>
        /// 在snode外面虚拟设置16个点，作为连线与snod链接的必经点（角上的点可能只经过一个。）
        /// 本功能是获得指定连接点的对应点。在角上的对应点按顺时针排列
        /// </summary>
        /// <param name="p_node"></param>
        /// <param name="p_pos"></param>
        /// <param name="p_distance"></param>
        /// <returns></returns>
        List<Point> GetRelativePoint(SNode p_node, LinkPortPosition p_pos, double p_distance)
        {
            List<Point> points = new List<Point>();
            if (p_node == null)
                return points;

            Point point;
            double left = Canvas.GetLeft(p_node);
            double top = Canvas.GetTop(p_node);
            double width = p_node.Width;
            double height = p_node.Height;
            double right = left + width;
            double bottom = top + height;

            switch (p_pos)
            {
                case LinkPortPosition.LeftCenter:
                    point = new Point(left, top + height / 2);
                    points.Add(new Point(point.X - p_distance, point.Y));
                    break;
                case LinkPortPosition.TopCenter:
                    point = new Point(left + width / 2, top);
                    points.Add(new Point(point.X, point.Y - p_distance));
                    break;
                case LinkPortPosition.RightCenter:
                    point = new Point(right, top + height / 2);
                    points.Add(new Point(point.X + p_distance, point.Y));
                    break;
                case LinkPortPosition.BottomCenter:
                    point = new Point(left + width / 2, bottom);
                    points.Add(new Point(point.X, point.Y + p_distance));
                    break;
                case LinkPortPosition.LeftTop:
                    point = new Point(left, top);
                    points.Add(new Point(point.X - p_distance, point.Y));
                    points.Add(new Point(point.X - p_distance, point.Y - p_distance));
                    points.Add(new Point(point.X, point.Y - p_distance));
                    break;
                case LinkPortPosition.RightTop:
                    point = new Point(right, top);
                    points.Add(new Point(point.X, point.Y - p_distance));
                    points.Add(new Point(point.X + p_distance, point.Y - p_distance));
                    points.Add(new Point(point.X + p_distance, point.Y));
                    break;
                case LinkPortPosition.RightBottom:
                    point = new Point(right, bottom);
                    points.Add(new Point(point.X + p_distance, point.Y));
                    points.Add(new Point(point.X + p_distance, point.Y + p_distance));
                    points.Add(new Point(point.X, point.Y + p_distance));
                    break;
                case LinkPortPosition.LeftBottom:
                    point = new Point(left, bottom);
                    points.Add(new Point(point.X, point.Y + p_distance));
                    points.Add(new Point(point.X - p_distance, point.Y + p_distance));
                    points.Add(new Point(point.X - p_distance, point.Y));
                    break;
            }
            return points;

        }

        /// <summary>
        /// 获得对象的矩形
        /// </summary>
        /// <param name="p_item"></param>
        /// <returns></returns>
        Rect GetEleRect(FrameworkElement p_item)
        {
            return new Rect(Canvas.GetLeft(p_item), Canvas.GetTop(p_item), p_item.Width, p_item.Height);
        }

        /// <summary>
        /// 获得矩形中心点坐标
        /// </summary>
        /// <param name="p_rect"></param>
        /// <returns></returns>
        Point GetRectCenter(Rect p_rect)
        {
            return new Point(p_rect.X + p_rect.Width / 2, p_rect.Y + p_rect.Height / 2);
        }

        /// <summary>
        /// 获得顺时针或者逆时针方向相邻的第n个点
        /// </summary>
        /// <param name="p_port"> 基准LinkPortPosition位置 </param>
        /// <param name="p_count"> 与本位置相差的数量 </param>
        /// <param name="p_clockwise"> 是否顺时针查找 </param>
        /// <returns></returns>
        LinkPortPosition GetLeanPort(LinkPortPosition p_port, int p_count, bool p_clockwise)
        {
            return (LinkPortPosition)(p_clockwise ? ((int)p_port + p_count) % 8 : ((int)p_port + 8 - p_count) % 8);
        }

        /// <summary>
        ///  获得两点位置关系，p_relative 相对于 p_reference 的关系，p_reference是参照点。
        /// </summary>
        /// <param name="p_reference"></param>
        /// <param name="p_relative"></param>
        /// <returns></returns>
        PosRelation GetPosRelation(Point p_reference, Point p_relative)
        {
            if (p_reference.X == p_relative.X && p_reference.Y == p_relative.Y)
                return PosRelation.Supper;

            if (p_reference.X == p_relative.X)
            {
                return p_reference.Y > p_relative.Y ? PosRelation.Up : PosRelation.Down;
            }
            if (p_reference.Y == p_relative.Y)
            {
                return p_reference.X > p_relative.X ? PosRelation.Left : PosRelation.Right;
            }
            if (p_reference.X > p_relative.X)
            {
                return p_reference.Y > p_relative.Y ? PosRelation.LeftUp : PosRelation.LeftDown;
            }
            else
            {
                return p_reference.Y > p_relative.Y ? PosRelation.RightUp : PosRelation.RightDown;
            }
        }

        /// <summary>
        /// 判定 Rect p_source 是否包含 Rect p_target
        /// </summary>
        /// <param name="p_source"></param>
        /// <param name="p_target"></param>
        /// <returns></returns>
        public bool RectContain(Rect p_source, Rect p_target)
        {
            return p_source.X <= p_target.X
                   && p_source.Y <= p_target.Y
                   && p_source.X + p_source.Width >= p_target.X + p_target.Width
                   && p_source.Y + p_source.Height >= p_target.Y + p_target.Height;
        }

        /// <summary>
        /// 获得点相对于矩形的关系。
        /// </summary>
        /// <param name="p_point"></param>
        /// <param name="p_rect"></param>
        /// <returns></returns>
        PointRectRelation GetPotRectRel(Point p_point, Rect p_rect)
        {
            if (p_rect == RectHelper.Empty)
                return PointRectRelation.None;

            if (((p_point.X == p_rect.X || p_point.X == p_rect.X + p_rect.Width)
               && (p_point.Y >= p_rect.Y && p_point.Y <= p_rect.Y + p_rect.Height))
               || ((p_point.Y == p_rect.X || p_point.Y == p_rect.Y + p_rect.Height)
               && (p_point.X >= p_rect.X && p_point.X <= p_rect.X + p_rect.Width)))
                return PointRectRelation.On;

            return p_point.X > p_rect.X && p_point.X < p_rect.X + p_rect.Width && p_point.Y > p_rect.Y && p_point.Y < p_rect.Y + p_rect.Height ? PointRectRelation.In : PointRectRelation.Out;
        }

        bool IsPointsCrossRect(Point p_p1, Point p_p2, Point p_p3, Rect p_rect)
        {
            if (GetPotRectRel(p_p1, p_rect) == PointRectRelation.In
                || GetPotRectRel(p_p2, p_rect) == PointRectRelation.In
                || GetPotRectRel(p_p3, p_rect) == PointRectRelation.In)
                return true;
            return IsHorPointsCrossRect(p_p1, p_p2, p_rect)
                || IsVerPointsCrossRect(p_p1, p_p2, p_rect)
                || IsHorPointsCrossRect(p_p2, p_p3, p_rect)
                || IsVerPointsCrossRect(p_p2, p_p3, p_rect);
        }

        /// <summary>
        /// 判断 连线 为水平直线的两点 是否贯穿矩形内部
        /// </summary>
        /// <param name="p_p1"></param>
        /// <param name="p_p2"></param>
        /// <param name="p_rect"></param>
        /// <returns></returns>
        bool IsHorPointsCrossRect(Point p_p1, Point p_p2, Rect p_rect)
        {
            if (p_p1.Y != p_p2.Y)
                return false;

            double right = p_rect.X + p_rect.Width;
            if (((p_p1.X <= p_rect.X && p_p2.X >= right)
                || (p_p2.X <= p_rect.X && p_p1.X >= right))
                && p_p1.Y > p_rect.Y && p_p1.Y < p_rect.Y + p_rect.Height)
                return true;

            return false;
        }

        /// <summary>
        /// 判断 连线 为垂直直线的两点 是否贯穿矩形内部
        /// </summary>
        /// <returns></returns>
        bool IsVerPointsCrossRect(Point p_p1, Point p_p2, Rect p_rect)
        {
            if (p_p1.X != p_p2.X)
                return false;

            double bottom = p_rect.Y + p_rect.Height;
            if (((p_p1.Y <= p_rect.Y && p_p2.Y >= bottom)
                || (p_p2.Y <= p_rect.Y && p_p1.Y >= bottom))
                && p_p1.X > p_rect.X && p_p1.X < p_rect.X + p_rect.Width)
                return true;

            return false;
        }

        /// <summary>
        /// 位置关系枚举
        /// </summary>
        public enum PosRelation
        {
            Up = 0,
            RightUp = 1,
            Right = 2,
            RightDown = 3,
            Down = 4,
            LeftDown = 5,
            Left = 6,
            LeftUp = 7,
            Supper = 8,
            None = 9
        }
        enum PointRectRelation
        {
            /// <summary>
            /// 不确定
            /// </summary>
            None = 0,

            /// <summary>
            /// 点在矩形里面
            /// </summary>
            In = 1,

            /// <summary>
            /// 点在矩形边上
            /// </summary>
            On = 2,

            /// <summary>
            /// 点在矩形外面
            /// </summary>
            Out = 3
        }

        #endregion
    }
}
