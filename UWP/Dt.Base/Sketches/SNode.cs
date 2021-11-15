#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-09-09 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Xml;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 流程图节点
    /// </summary>
    [ContentProperty(Name = "Content")]
    public partial class SNode : Control
    {
        #region 静态内容
        /// <summary>
        /// 标题文字
        /// </summary>
        public readonly static DependencyProperty TitleProperty = DependencyProperty.Register(
            "Title",
            typeof(string),
            typeof(SNode),
            new PropertyMetadata(null, OnTitleChanged));

        /// <summary>
        /// 背景图形名称
        /// </summary>
        public readonly static DependencyProperty ShapeProperty = DependencyProperty.Register(
            "Shape",
            typeof(string),
            typeof(SNode),
            new PropertyMetadata(null, OnShapeChanged));

        /// <summary>
        /// 状态图形名称
        /// </summary>
        public readonly static DependencyProperty FlagProperty = DependencyProperty.Register(
            "Flag",
            typeof(Icons),
            typeof(SNode),
            new PropertyMetadata(Icons.None, OnFlagChanged));

        /// <summary>
        /// 内容
        /// </summary>
        public readonly static DependencyProperty ContentProperty = DependencyProperty.Register(
            "Content",
            typeof(object),
            typeof(SNode),
            new PropertyMetadata(null, OnContentChanged));

        /// <summary>
        /// 图形定义
        /// </summary>
        public readonly static DependencyProperty NodeShapeProperty = DependencyProperty.Register(
            "NodeShape",
            typeof(Path),
            typeof(SNode),
            null);

        /// <summary>
        /// 图标的图形定义
        /// </summary>
        public readonly static DependencyProperty IconShapeProperty = DependencyProperty.Register(
            "IconShape",
            typeof(object),
            typeof(SNode),
            null);

        /// <summary>
        /// 状态的图形定义
        /// </summary>
        public readonly static DependencyProperty FlagShapeProperty = DependencyProperty.Register(
            "FlagShape",
            typeof(object),
            typeof(SNode),
            null);

        static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SNode)d).SavePropertyChange(e);
        }

        static void OnFlagChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SNode node = (SNode)d;
            Icons flag = (Icons)e.NewValue;
            if (flag == Icons.None)
            {
                node.FlagShape = null;
            }
            else
            {
                var shape = Res.GetIcon(flag, 20, Res.WhiteBrush);
                shape.IsHitTestVisible = false;
                node.FlagShape = shape;
            }
            if (flag != Icons.None || (Icons)e.OldValue != Icons.None)
                node.SavePropertyChange(e);
        }

        static void OnShapeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SNode node = (SNode)d;
            node.UpdateShape();
            if (!string.IsNullOrEmpty((string)e.NewValue) || !string.IsNullOrEmpty((string)e.OldValue))
                node.SavePropertyChange(e);
        }

        static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SNode)d).UpdateContentState();
        }
        #endregion

        #region 成员变量
        const double _scaleDepth = 0.9;
        const double _rotationDepth = 20;
        static KeyTime _tilePressDuration = TimeSpan.FromSeconds(0.1);

        readonly Sketch _owner;
        uint? _pointerID;
        PointerDirection _ptDirection;
        Storyboard _storyboard;
        Point _currentPoint;
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        public SNode(Sketch p_owner)
        {
            DefaultStyleKey = typeof(SNode);
            _owner = p_owner;
        }
        #endregion

        #region 事件
        /// <summary>
        /// 图元点击事件
        /// </summary>
#if ANDROID
        new
#endif
        public event EventHandler Click;

        #endregion

        #region 属性
        /// <summary>
        /// 获取设置流程图中的图元标识
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// 获取设置标题文字
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// 获取设置背景图形名称
        /// </summary>
        public string Shape
        {
            get { return (string)GetValue(ShapeProperty); }
            set { SetValue(ShapeProperty, value); }
        }

        /// <summary>
        /// 获取设置状态图形名称
        /// </summary>
        public Icons Flag
        {
            get { return (Icons)GetValue(FlagProperty); }
            set { SetValue(FlagProperty, value); }
        }

        /// <summary>
        /// 获取设置内容
        /// </summary>
        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// 获取设置图形定义(绑定用)
        /// </summary>
        public Path NodeShape
        {
            get { return (Path)GetValue(NodeShapeProperty); }
            internal set { SetValue(NodeShapeProperty, value); }
        }

        /// <summary>
        /// 获取设置图标的图形定义(绑定用)
        /// </summary>
        public object IconShape
        {
            get { return GetValue(IconShapeProperty); }
            internal set { SetValue(IconShapeProperty, value); }
        }

        /// <summary>
        /// 获取设置状态的图形定义(绑定用)
        /// </summary>
        public object FlagShape
        {
            get { return GetValue(FlagShapeProperty); }
            internal set { SetValue(FlagShapeProperty, value); }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 加载xml
        /// </summary>
        /// <param name="p_reader"></param>
        internal void ReadXml(XmlReader p_reader)
        {
            for (int i = 0; i < p_reader.AttributeCount; i++)
            {
                p_reader.MoveToAttribute(i);
                switch (p_reader.Name)
                {
                    case "id": if (long.TryParse(p_reader.Value, out var l)) ID = l; break;
                    case "title": Title = p_reader.Value; break;
                    case "shape": Shape = p_reader.Value; break;
                    case "flag": Flag = (Icons)Enum.Parse(typeof(Icons), p_reader.Value); break;
                    case "content": Content = p_reader.Value; break;
                    case "background": Background = new SolidColorBrush(Res.HexStringToColor(p_reader.Value)); break;
                    case "borderbrush": BorderBrush = new SolidColorBrush(Res.HexStringToColor(p_reader.Value)); break;
                    case "left": Canvas.SetLeft(this, double.Parse(p_reader.Value)); break;
                    case "top": Canvas.SetTop(this, double.Parse(p_reader.Value)); break;
                    case "width": Width = double.Parse(p_reader.Value); break;
                    case "height": Height = double.Parse(p_reader.Value); break;
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
            p_writer.WriteStartElement("Node");
            p_writer.WriteAttributeString("id", ID.ToString());
            p_writer.WriteAttributeString("title", Title);
            if (!string.IsNullOrEmpty(Shape))
            {
                p_writer.WriteAttributeString("shape", Shape);
            }
            if (Flag != Icons.None)
            {
                p_writer.WriteAttributeString("flag", Flag.ToString());
            }
            if (Content != null && !string.IsNullOrEmpty(Content.ToString()))
            {
                p_writer.WriteAttributeString("content", Content.ToString());
            }
            if ((Background as SolidColorBrush).Color != Res.主蓝.Color)
            {
                p_writer.WriteAttributeString("background", (Background as SolidColorBrush).Color.ToString());
            }
            if ((BorderBrush as SolidColorBrush).Color != Res.主蓝.Color)
            {
                p_writer.WriteAttributeString("borderbrush", (BorderBrush as SolidColorBrush).Color.ToString());
            }
            p_writer.WriteAttributeString("left", Canvas.GetLeft(this).ToString());
            p_writer.WriteAttributeString("top", Canvas.GetTop(this).ToString());
            p_writer.WriteAttributeString("width", Width.ToString());
            p_writer.WriteAttributeString("height", Height.ToString());
            p_writer.WriteEndElement();
        }
        #endregion

        #region 重写方法
        /// <summary>
        /// 应用模板
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            UpdateShape();
            UpdateContentState();
        }

        /// <summary>
        /// 鼠标进入
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            base.OnPointerEntered(e);
            if (Click != null)
            {
                e.Handled = true;
                VisualStateManager.GoToState(this, "PointerOver", true);
            }
        }

        /// <summary>
        /// 鼠标点击
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);
            if (Click == null || !CapturePointer(e.Pointer))
                return;

            e.Handled = true;
            _pointerID = e.Pointer.PointerId;

            Point ptCurrent = e.GetCurrentPoint(this).Position;
            double width = ActualWidth / 3.0;
            double height = ActualHeight / 3.0;
            if (new Rect(0.0, 0.0, width, height).Contains(ptCurrent))
            {
                _ptDirection = PointerDirection.TopLeft;
            }
            else if (new Rect(width, 0.0, width, height).Contains(ptCurrent))
            {
                _ptDirection = PointerDirection.Top;
            }
            else if (new Rect(width * 2.0, 0.0, width, height).Contains(ptCurrent))
            {
                _ptDirection = PointerDirection.TopRight;
            }
            else if (new Rect(0.0, height, width, height).Contains(ptCurrent))
            {
                _ptDirection = PointerDirection.Left;
            }
            else if (new Rect(width * 2.0, 0.0, width, height).Contains(ptCurrent))
            {
                _ptDirection = PointerDirection.Center;
            }
            else if (new Rect(width * 2.0, height, width, height).Contains(ptCurrent))
            {
                _ptDirection = PointerDirection.Right;
            }
            else if (new Rect(0.0, height * 2.0, width, height).Contains(ptCurrent))
            {
                _ptDirection = PointerDirection.BottomLeft;
            }
            else if (new Rect(width, height * 2.0, width, height).Contains(ptCurrent))
            {
                _ptDirection = PointerDirection.Bottom;
            }
            else if (new Rect(width * 2.0, height * 2.0, width, height).Contains(ptCurrent))
            {
                _ptDirection = PointerDirection.BottomRight;
            }
            else
            {
                _ptDirection = PointerDirection.Center;
            }

            _storyboard = new Storyboard();
            if (_ptDirection <= PointerDirection.Center)
            {
                if (_ptDirection == PointerDirection.Center)
                {
                    Timeline timeline = BuildScaleXAnimation();
                    Timeline timeline2 = BuildScaleYAnimation();
                    _storyboard.Children.Add(timeline);
                    _storyboard.Children.Add(timeline2);
                    _storyboard.Begin();
                }
                else
                {
                    Timeline timeline3 = BuildAnimation(_ptDirection);
                    _storyboard.Children.Add(timeline3);
                    _storyboard.Begin();
                }
            }
            else if (_ptDirection == PointerDirection.TopLeft)
            {
                if (ptCurrent.X > ptCurrent.Y)
                {
                    Timeline timeline4 = BuildAnimation(PointerDirection.Top);
                    _storyboard.Children.Add(timeline4);
                    _storyboard.Begin();
                }
                else
                {
                    Timeline timeline5 = BuildAnimation(PointerDirection.Left);
                    _storyboard.Children.Add(timeline5);
                    _storyboard.Begin();
                }
            }
            else if (_ptDirection == PointerDirection.TopRight)
            {
                if (ptCurrent.Y > (height - (ptCurrent.X - (width * 2.0))))
                {
                    Timeline timeline6 = BuildAnimation(PointerDirection.Right);
                    _storyboard.Children.Add(timeline6);
                    _storyboard.Begin();
                }
                else
                {
                    Timeline timeline7 = BuildAnimation(PointerDirection.Top);
                    _storyboard.Children.Add(timeline7);
                    _storyboard.Begin();
                }
            }
            else if (_ptDirection == PointerDirection.BottomLeft)
            {
                if ((ptCurrent.Y - (height * 2.0)) > (height - ptCurrent.X))
                {
                    Timeline timeline8 = BuildAnimation(PointerDirection.Bottom);
                    _storyboard.Children.Add(timeline8);
                    _storyboard.Begin();
                }
                else
                {
                    Timeline timeline9 = BuildAnimation(PointerDirection.Left);
                    _storyboard.Children.Add(timeline9);
                    _storyboard.Begin();
                }
            }
            else if (ptCurrent.X > ptCurrent.Y)
            {
                Timeline timeline10 = BuildAnimation(PointerDirection.Right);
                _storyboard.Children.Add(timeline10);
                _storyboard.Begin();
            }
            else
            {
                Timeline timeline11 = BuildAnimation(PointerDirection.Bottom);
                _storyboard.Children.Add(timeline11);
                _storyboard.Begin();
            }
        }

        /// <summary>
        /// 抬起
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);
            if (Click == null || _pointerID != e.Pointer.PointerId)
                return;

            ReleasePointerCapture(e.Pointer);
            e.Handled = true;
            _pointerID = null;
            _currentPoint = e.GetCurrentPoint(null).Position;

            if (_ptDirection == PointerDirection.Center)
            {
                DoubleAnimationUsingKeyFrames frames2 = _storyboard.Children[0] as DoubleAnimationUsingKeyFrames;
                DoubleAnimationUsingKeyFrames frames3 = _storyboard.Children[1] as DoubleAnimationUsingKeyFrames;
                if (frames2 != null)
                {
                    EasingDoubleKeyFrame frame3 = frames2.KeyFrames[0] as EasingDoubleKeyFrame;
                    EasingDoubleKeyFrame frame4 = frames2.KeyFrames[1] as EasingDoubleKeyFrame;
                    frame3.Value = frame4.Value;
                    frame4.Value = 1.0;
                }
                if (frames3 != null)
                {
                    EasingDoubleKeyFrame frame5 = frames3.KeyFrames[0] as EasingDoubleKeyFrame;
                    EasingDoubleKeyFrame frame6 = frames3.KeyFrames[1] as EasingDoubleKeyFrame;
                    frame5.Value = frame6.Value;
                    frame6.Value = 1.0;
                }
            }
            else
            {
                DoubleAnimationUsingKeyFrames frames = _storyboard.Children[0] as DoubleAnimationUsingKeyFrames;
                if (frames != null)
                {
                    EasingDoubleKeyFrame frame = frames.KeyFrames[0] as EasingDoubleKeyFrame;
                    EasingDoubleKeyFrame frame2 = frames.KeyFrames[1] as EasingDoubleKeyFrame;
                    frame.Value = frame2.Value;
                    frame2.Value = 0.0;
                }
            }

            if (Click != null)
            {
                _storyboard.Completed += OnAnimationCompleted;
            }
            _storyboard.Begin();
        }

        /// <summary>
        /// 鼠标移出
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            base.OnPointerExited(e);
            if (Click != null)
            {
                e.Handled = true;
                VisualStateManager.GoToState(this, "Normal", true);
            }
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 记录属性变化历史以备撤消
        /// </summary>
        /// <param name="e"></param>
        void SavePropertyChange(DependencyPropertyChangedEventArgs e)
        {
            if (!_owner.IsReadOnly
                && !_owner.CmdPropHis.IsSetting
                && _owner.Container.Children.Contains(this))
            {
                var args = new HisPropertyCmdArgs();
                args.Redo = () => SetValue(e.Property, e.NewValue);
                args.Undo = () => SetValue(e.Property, e.OldValue);
                _owner.His.RecordAction(new CmdAction(_owner.CmdPropHis, args));
            }
        }

        /// <summary>
        /// 根据任务类型返回不同的path
        /// </summary>
        /// <param name="p_name"></param>
        /// <returns></returns>
        Path GetTaskIcon(string p_name)
        {
            switch (p_name)
            {
                case "开始":
                    return Res.ParsePath("M30,15 C30,23.2843 23.2843,30 15,30 C6.71573,30 0,23.2843 0,15 C0,6.71573 6.71573,0 15,0 C23.2843,0 30,6.71573 30,15 z");
                case "任务":
                    return Res.ParsePath("M0,0 L30,0 L30,30 L0,30 z");
                case "同步":
                    return Res.ParsePath("M0,29.875 L0,0 L19.5,0 L19.5,0.011993 L21.5,0.011993 C26.6302,3.78024 30.0353,8.76554 30.0353,15.131 C30.0353,21.4964 26.7552,26.2318 21.625,30 z");
                case "结束":
                    return Res.ParsePath("M0,11.25 C0,4.69162 4.69162,0 11.25,0 L18.75,0 C25.3084,0 30,4.69162 30,11.25 L30,18.625 C30,25.1834 25.3084,30 18.75,30 L11.25,30 C4.69162,30 0,25.1834 0,18.625 z");
                default:
                    return null;
            }
        }
        #endregion

        /// <summary>
        /// 更新背景图形
        /// </summary>
        void UpdateShape()
        {
            string name = "任务";
            if (!string.IsNullOrEmpty(Shape))
                name = Shape;

            Path path = GetTaskIcon(name);
            if (path == null)
            {
                NodeShape = null;
                return;
            }

            path.StrokeThickness = 4;
            path.Stretch = Stretch.Fill;
            path.IsHitTestVisible = false;

            Binding bind = new Binding();
            bind.Path = new PropertyPath("Background");
            bind.Source = this;
            path.SetBinding(Path.FillProperty, bind);

            bind = new Binding();
            bind.Path = new PropertyPath("BorderBrush");
            bind.Source = this;
            path.SetBinding(Path.StrokeProperty, bind);

            NodeShape = path;
        }

        /// <summary>
        /// 内容切换时更改状态
        /// </summary>
        void UpdateContentState()
        {
            if (Content != null && !string.IsNullOrEmpty(Content.ToString()))
            {
                VisualStateManager.GoToState(this, "ExistContent", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "NoContent", true);
            }
        }

        /// <summary>
        /// 动画结束后触发点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnAnimationCompleted(object sender, object e)
        {
            _storyboard.Completed -= OnAnimationCompleted;
            if (this.ContainPoint(_currentPoint))
            {
                Click(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// x轴缩放动画
        /// </summary>
        /// <returns></returns>
        Timeline BuildScaleXAnimation()
        {
            DoubleAnimationUsingKeyFrames timeline = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(timeline, this);
            Storyboard.SetTargetProperty(timeline, "(UIElement.RenderTransform).(CompositeTransform.ScaleX)");
            EasingDoubleKeyFrame frame3 = new EasingDoubleKeyFrame();
            frame3.KeyTime = (KeyTime)TimeSpan.FromSeconds(0.0);
            frame3.Value = 1.0;
            EasingDoubleKeyFrame frame = frame3;
            EasingDoubleKeyFrame frame4 = new EasingDoubleKeyFrame();
            frame4.KeyTime = _tilePressDuration;
            frame4.Value = _scaleDepth;
            EasingDoubleKeyFrame frame2 = frame4;
            timeline.KeyFrames.Add(frame);
            timeline.KeyFrames.Add(frame2);
            CompositeTransform transform = new CompositeTransform();
            RenderTransformOrigin = new Point(0.5, 0.5);
            RenderTransform = transform;
            return timeline;
        }

        /// <summary>
        /// y轴缩放动画
        /// </summary>
        /// <returns></returns>
        Timeline BuildScaleYAnimation()
        {
            DoubleAnimationUsingKeyFrames timeline = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(timeline, this);
            Storyboard.SetTargetProperty(timeline, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
            EasingDoubleKeyFrame frame3 = new EasingDoubleKeyFrame();
            frame3.KeyTime = (KeyTime)TimeSpan.FromSeconds(0.0);
            frame3.Value = 1.0;
            EasingDoubleKeyFrame frame = frame3;
            EasingDoubleKeyFrame frame4 = new EasingDoubleKeyFrame();
            frame4.KeyTime = _tilePressDuration;
            frame4.Value = _scaleDepth;
            EasingDoubleKeyFrame frame2 = frame4;
            timeline.KeyFrames.Add(frame);
            timeline.KeyFrames.Add(frame2);
            CompositeTransform transform = new CompositeTransform();
            RenderTransform = transform;
            RenderTransformOrigin = new Point(0.5, 0.5);
            return timeline;
        }

        /// <summary>
        /// 综合动画
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        Timeline BuildAnimation(PointerDirection direction)
        {
            DoubleAnimationUsingKeyFrames timeline = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(timeline, this);
            if (direction != PointerDirection.Center)
            {
                PlaneProjection projection2 = new PlaneProjection();
                projection2.CenterOfRotationZ = 0.0;
                PlaneProjection projection = projection2;
                EasingDoubleKeyFrame frame3 = new EasingDoubleKeyFrame();
                frame3.KeyTime = (KeyTime)TimeSpan.FromSeconds(0.0);
                frame3.Value = 0.0;
                EasingDoubleKeyFrame frame = frame3;
                EasingDoubleKeyFrame frame4 = new EasingDoubleKeyFrame();
                frame4.KeyTime = _tilePressDuration;
                EasingDoubleKeyFrame frame2 = frame4;
                timeline.KeyFrames.Add(frame);
                timeline.KeyFrames.Add(frame2);
                if ((direction == PointerDirection.Left) || (direction == PointerDirection.Bottom))
                {
                    frame2.Value = _rotationDepth;
                }
                else if ((direction == PointerDirection.Top) || (direction == PointerDirection.Right))
                {
                    frame2.Value = -_rotationDepth;
                }
                if ((direction == PointerDirection.Top) || (direction == PointerDirection.Bottom))
                {
                    Storyboard.SetTargetProperty(timeline, "(UIElement.Projection).(PlaneProjection.RotationX)");
                }
                else if ((direction == PointerDirection.Left) || (direction == PointerDirection.Right))
                {
                    Storyboard.SetTargetProperty(timeline, "(UIElement.Projection).(PlaneProjection.RotationY)");
                }
                if (direction == PointerDirection.Bottom)
                {
                    projection.CenterOfRotationX = 0.5;
                    projection.CenterOfRotationY = 0.0;
                }
                else if (direction == PointerDirection.Top)
                {
                    projection.CenterOfRotationX = 0.5;
                    projection.CenterOfRotationY = 1.0;
                }
                else if (direction == PointerDirection.Left)
                {
                    projection.CenterOfRotationX = 1.0;
                    projection.CenterOfRotationY = 0.5;
                }
                else if (direction == PointerDirection.Right)
                {
                    projection.CenterOfRotationX = 0.0;
                    projection.CenterOfRotationY = 0.5;
                }
                Projection = projection;
            }
            return timeline;
        }

        /// <summary>
        /// 点击位置
        /// </summary>
        enum PointerDirection
        {
            Left,
            Top,
            Right,
            Bottom,
            Center,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }
    }
}
