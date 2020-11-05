#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-09-12 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Windows.System;
#endregion

namespace Dt.Base.Sketches
{
    /// <summary>
    /// 连线处理
    /// </summary>
    internal class SketchLinkManager
    {
        #region 成员变量
        Sketch _owner;
        Thumb _thumb;
        Line _line;

        double _thumbX;
        double _thumbY;
        double _thumbWidth;
        double _thumbHeight;
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="p_owner"></param>
        public SketchLinkManager(Sketch p_owner)
        {
            _owner = p_owner;
        }

        #endregion

        #region 外部方法
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="p_thumb"></param>
        /// <param name="p_line"></param>
        public void Init(Thumb p_thumb, Line p_line)
        {
            _thumb = p_thumb;
            _line = p_line;

            //初始大小
            _thumbWidth = 30;
            _thumbHeight = 30;
            _thumb.DragStarted += OnLinkDragStarted;
            _thumb.DragDelta += OnLinkDragDelta;
            _thumb.DragCompleted += OnLinkDragCompleted;
        }

        /// <summary>
        /// 在指定SNode位置显示连线点(方法内部已判断是否需要显示)
        /// </summary>
        /// <param name="p_node"></param>
        /// <param name="p_x">RightBottom.X</param>
        /// <param name="p_y">RightBottom.Y</param>
        public void ShowLinkPt(SNode p_node, double p_x, double p_y)
        {
            if (p_node != null)
            {
                if (p_x + 40 + p_node.Width > _owner.ActualWidth)
                {
                    //左侧显示
                    _thumbX = p_x - 60;
                    _thumbY = p_y - _thumbHeight / 2;
                }
                else
                {
                    //右侧显示
                    _thumbX = p_x + 40 + p_node.Width;
                    _thumbY = p_y - _thumbHeight / 2;
                }
                Canvas.SetLeft(_thumb, _thumbX);
                Canvas.SetTop(_thumb, _thumbY);
                _thumb.Visibility = Visibility.Visible;
            }
            else
            {
                HideLinkPt();
            }
        }

        /// <summary>
        /// 隐藏连线点
        /// </summary>
        public void HideLinkPt()
        {
            _thumb.Visibility = Visibility.Collapsed;
        }
        #endregion

        #region 事件
        void OnLinkDragStarted(object sender, DragStartedEventArgs e)
        {
            _line.Visibility = Visibility.Visible;
            FrameworkElement curNode = _owner.SelectionClerk.Selection[0];
            if (_thumbX - 40 - _owner.SelectionClerk.Selection[0].Width == Canvas.GetLeft(curNode))
            {
                //起点为 rightbottom
                _line.X1 = Canvas.GetLeft(curNode) + curNode.Width;
                _line.Y1 = Canvas.GetTop(curNode) + curNode.Height;
            }
            else
            {
                //起点为 leftbottom
                _line.X1 = Canvas.GetLeft(curNode);
                _line.Y1 = Canvas.GetTop(curNode) + curNode.Height;
            }
            _line.X2 = e.HorizontalOffset;
            _line.Y2 = e.VerticalOffset;
            Canvas.SetLeft(_thumb, _line.X2 - _thumbWidth / 2);
            Canvas.SetTop(_thumb, _line.Y2 - _thumbHeight / 2);
        }

        void OnLinkDragDelta(object sender, DragDeltaEventArgs e)
        {
            _line.X2 += e.HorizontalChange;
            _line.Y2 += e.VerticalChange;
            Canvas.SetLeft(_thumb, _line.X2 - _thumb.ActualWidth / 2);
            Canvas.SetTop(_thumb, _line.Y2 - _thumb.ActualHeight / 2);
            CaclStart(_owner.SelectionClerk.Selection[0]);
            if (!_owner.LinkThumbMove(_thumb))
            {
                VisualStateManager.GoToState(_thumb, "DragNoValid", true);
            }
        }

        void OnLinkDragCompleted(object sender, DragCompletedEventArgs e)
        {
            SNode curNode = _owner.SelectionClerk.Selection[0] as SNode;
            LinkPortPosition startPos = CaclStart(curNode);
            Rect thumbRect = new Rect(Canvas.GetLeft(_thumb), Canvas.GetTop(_thumb), _thumb.ActualWidth, _thumb.ActualHeight);
            if (_owner.IsValidRegion(thumbRect))
            {
                SNode tagNode = _owner.GetFirstIntersect(thumbRect);
                if (tagNode != null)
                {
                    //是否存在连接
                    bool isHas = (from obj in _owner.Container.Children
                                  let line = obj as SLine
                                  where line != null && line.HeaderID == curNode.ID && line.TailID == tagNode.ID
                                  select line).Any();
                    if (!isHas)
                    {
                        SLine line = new SLine(_owner);
                        line.HeaderID = curNode.ID;
                        line.HeaderPort = startPos;
                        line.TailID = tagNode.ID;
                        line.TailPort = _owner.GetLinkPosition(tagNode, thumbRect);
                        _owner.Insert(line);
                        _owner.SelectionClerk.SelectLine(line);
                        _owner.RefreshLinkLines(tagNode);
                    }
                }
            }
            //还原默认位置
            Canvas.SetLeft(_thumb, _thumbX);
            Canvas.SetTop(_thumb, _thumbY);
            _line.Visibility = Visibility.Collapsed;
            _owner.LinkThumbEnd();
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 计算线的起点
        /// </summary>
        /// <param name="p_node"></param>
        /// <returns></returns>
        LinkPortPosition CaclStart(FrameworkElement p_node)
        {
            double left = Canvas.GetLeft(p_node);
            double top = Canvas.GetTop(p_node);
            double right = left + p_node.Width;
            double bottom = top + p_node.Height;
            Dictionary<LinkPortPosition, Point> dictPt = new Dictionary<LinkPortPosition, Point>();
            dictPt.Add(LinkPortPosition.LeftTop, new Point(left, top));
            dictPt.Add(LinkPortPosition.TopCenter, new Point(left + p_node.Width / 2, top));
            dictPt.Add(LinkPortPosition.RightTop, new Point(right, top));
            dictPt.Add(LinkPortPosition.RightCenter, new Point(right, top + p_node.Height / 2));
            dictPt.Add(LinkPortPosition.RightBottom, new Point(right, bottom));
            dictPt.Add(LinkPortPosition.BottomCenter, new Point(left + p_node.Width / 2, bottom));
            dictPt.Add(LinkPortPosition.LeftBottom, new Point(left, bottom));
            dictPt.Add(LinkPortPosition.LeftCenter, new Point(left, top + p_node.Height / 2));
            Point ptCur = new Point(_line.X2, _line.Y2);
            LinkPortPosition startPos = LinkPortPosition.RightBottom;
            double distance = double.MaxValue;
            foreach (LinkPortPosition pos in dictPt.Keys)
            {
                double dis = GetDistance(ptCur, dictPt[pos]);
                if (dis < distance)
                {
                    distance = dis;
                    startPos = pos;
                }
            }
            //更改开始位置
            _line.X1 = dictPt[startPos].X;
            _line.Y1 = dictPt[startPos].Y;
            return startPos;
        }

        /// <summary>
        /// 获取距离给定源最近的点
        /// </summary>
        /// <param name="p_pt1"></param>
        /// <param name="p_pt2"></param>
        /// <returns></returns>
        double GetDistance(Point p_pt1, Point p_pt2)
        {
            return Math.Sqrt(Math.Pow(Math.Abs(p_pt1.X - p_pt2.X), 2) + Math.Pow(Math.Abs(p_pt1.Y - p_pt2.Y), 2));
        }
        #endregion
    }
}
