#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-02-26 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

#endregion

namespace Dt.Base.Docking
{
    /// <summary>
    /// 拖动过程的停靠导航，内部用
    /// </summary>
    public partial class Compass : Control
    {
        #region 静态内容
        /// <summary>
        /// 停靠位置
        /// </summary>
        public static readonly DependencyProperty DockPositionProperty = DependencyProperty.Register(
            "DockPosition",
            typeof(DockPosition),
            typeof(Compass),
            new PropertyMetadata(DockPosition.None, OnStateChanged));

        /// <summary>
        /// 底部指示器是否可见
        /// </summary>
        public static readonly DependencyProperty ShowBottomIndicatorProperty = DependencyProperty.Register(
            "ShowBottomIndicator",
            typeof(bool), 
            typeof(Compass), 
            new PropertyMetadata(true, OnStateChanged));
        
        /// <summary>
        /// 中部指示器是否可见
        /// </summary>
        public static readonly DependencyProperty ShowCenterIndicatorProperty = DependencyProperty.Register(
            "ShowCenterIndicator", 
            typeof(bool), 
            typeof(Compass), 
            new PropertyMetadata(true, OnStateChanged));
        
        /// <summary>
        /// 左部指示器是否可见
        /// </summary>
        public static readonly DependencyProperty ShowLeftIndicatorProperty = DependencyProperty.Register(
            "ShowLeftIndicator", 
            typeof(bool), 
            typeof(Compass), 
            new PropertyMetadata(true, OnStateChanged));
        
        /// <summary>
        /// 右部指示器是否可见
        /// </summary>
        public static readonly DependencyProperty ShowRightIndicatorProperty = DependencyProperty.Register(
            "ShowRightIndicator", 
            typeof(bool), 
            typeof(Compass), 
            new PropertyMetadata(true, OnStateChanged));
        
        /// <summary>
        /// 顶部指示器是否可见
        /// </summary>
        public static readonly DependencyProperty ShowTopIndicatorProperty = DependencyProperty.Register(
            "ShowTopIndicator", 
            typeof(bool), 
            typeof(Compass), 
            new PropertyMetadata(true, OnStateChanged));

        static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Compass).ChangeVisualState();
        }
        #endregion

        #region 成员变量
        FrameworkElement _rootPart;
        FrameworkElement _leftIndicator;
        FrameworkElement _rightIndicator;
        FrameworkElement _topIndicator;
        FrameworkElement _bottomIndicator;
        FrameworkElement _centerIndicator;
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        public Compass()
        {
            DefaultStyleKey = typeof(Compass);
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取导航中高亮显示的指示器
        /// </summary>
        public DockPosition DockPosition
        {
            get { return (DockPosition)GetValue(DockPositionProperty); }
            internal set { SetValue(DockPositionProperty, value); }
        }

        /// <summary>
        /// 获取设置底部指示器是否可见
        /// </summary>
        public bool ShowBottomIndicator
        {
            get { return (bool)GetValue(ShowBottomIndicatorProperty); }
            set { SetValue(ShowBottomIndicatorProperty, value); }
        }

        /// <summary>
        /// 获取设置中部指示器是否可见
        /// </summary>
        public bool ShowCenterIndicator
        {
            get { return (bool)GetValue(ShowCenterIndicatorProperty); }
            set { SetValue(ShowCenterIndicatorProperty, value); }
        }

        /// <summary>
        /// 获取设置左部指示器是否可见
        /// </summary>
        public bool ShowLeftIndicator
        {
            get { return (bool)GetValue(ShowLeftIndicatorProperty); }
            set { SetValue(ShowLeftIndicatorProperty, value); }
        }

        /// <summary>
        /// 获取设置右部指示器是否可见
        /// </summary>
        public bool ShowRightIndicator
        {
            get { return (bool)GetValue(ShowRightIndicatorProperty); }
            set { SetValue(ShowRightIndicatorProperty, value); }
        }

        /// <summary>
        /// 获取设置顶部指示器是否可见
        /// </summary>
        public bool ShowTopIndicator
        {
            get { return (bool)GetValue(ShowTopIndicatorProperty); }
            set { SetValue(ShowTopIndicatorProperty, value); }
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// Win停靠位置
        /// </summary>
        /// <param name="position"></param>
        public void ChangeDockPosition(Point position)
        {
            DockPosition = (from dpElem in
                                (from frElem in GetElementsInHostCoordinates((Parent as FrameworkElement), position)
                                 select GetCompassPosition(frElem))
                            where dpElem != DockPosition.None
                            select dpElem).FirstOrDefault();
        }

        /// <summary>
        /// 恢复原始状态
        /// </summary>
        internal void ClearIndicators()
        {
            ClearValue(ShowLeftIndicatorProperty);
            ClearValue(ShowTopIndicatorProperty);
            ClearValue(ShowRightIndicatorProperty);
            ClearValue(ShowBottomIndicatorProperty);
            ClearValue(ShowCenterIndicatorProperty);
        }
        #endregion

        #region 重写方法
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _leftIndicator = GetTemplateChild("PART_LeftIndicator") as FrameworkElement;
            _topIndicator = GetTemplateChild("PART_TopIndicator") as FrameworkElement;
            _rightIndicator = GetTemplateChild("PART_RightIndicator") as FrameworkElement;
            _bottomIndicator = GetTemplateChild("PART_BottomIndicator") as FrameworkElement;
            _centerIndicator = GetTemplateChild("PART_CenterIndicator") as FrameworkElement;
            _rootPart = GetTemplateChild("PART_Center") as FrameworkElement;
            ChangeVisualState();
        }
        #endregion

        #region 内部方法
        void ChangeVisualState()
        {
            VisualStateManager.GoToState(this, ShowLeftIndicator ? "LeftIndicatorVisibile" : "LeftIndicatorHidden", true);
            VisualStateManager.GoToState(this, ShowTopIndicator ? "TopIndicatorVisibile" : "TopIndicatorHidden", true);
            VisualStateManager.GoToState(this, ShowRightIndicator ? "RightIndicatorVisibile" : "RightIndicatorHidden", true);
            VisualStateManager.GoToState(this, ShowBottomIndicator ? "BottomIndicatorVisibile" : "BottomIndicatorHidden", true);
            VisualStateManager.GoToState(this, ShowCenterIndicator ? "CenterIndicatorVisibile" : "CenterIndicatorHidden", true);

            switch (DockPosition)
            {
                case Docking.DockPosition.Top:
                    VisualStateManager.GoToState(this, "HighlightTopIndicator", true);
                    return;

                case Docking.DockPosition.Bottom:
                    VisualStateManager.GoToState(this, "HighlightBottomIndicator", true);
                    return;

                case Docking.DockPosition.Center:
                    VisualStateManager.GoToState(this, "HighlightCenterIndicator", true);
                    return;

                case Docking.DockPosition.Left:
                    VisualStateManager.GoToState(this, "HighlightLeftIndicator", true);
                    return;

                case Docking.DockPosition.Right:
                    VisualStateManager.GoToState(this, "HighlightRightIndicator", true);
                    return;

                default:
                    VisualStateManager.GoToState(this, "Normal", true);
                    return;
            }
        }

        DockPosition GetCompassPosition(FrameworkElement element)
        {
            if (element == _leftIndicator)
            {
                return Docking.DockPosition.Left;
            }
            if (element == _topIndicator)
            {
                return Docking.DockPosition.Top;
            }
            if (element == _rightIndicator)
            {
                return Docking.DockPosition.Right;
            }
            if (element == _bottomIndicator)
            {
                return Docking.DockPosition.Bottom;
            }
            if (element == _centerIndicator) 
            {
                return Docking.DockPosition.Center;
            }
            return Docking.DockPosition.None;
        }

        IEnumerable<FrameworkElement> GetElementsInHostCoordinates(FrameworkElement subtree, Point localPosition)
        {
            subtree = GetSubtree(subtree);
            GeneralTransform generalTransform = subtree.TransformToVisual(null);
            Transform transform = generalTransform as Transform;
            if (transform == null) 
            {
                Point p = generalTransform.TransformPoint(new Point(0.0, 0.0));
                transform = new TranslateTransform() 
                {
                    X = p.X ,
                    Y = p.Y 
                };
            }
            return VisualTreeHelper.FindElementsInHostCoordinates(transform.TransformPoint(localPosition), subtree).OfType<FrameworkElement>();
        }

        FrameworkElement GetSubtree(FrameworkElement element)
        {
            Windows.UI.Xaml.Controls.Primitives.Popup popup = element as Windows.UI.Xaml.Controls.Primitives.Popup;
            if (element == null || popup == null) return element;
            FrameworkElement testElement = (FrameworkElement)((VisualTreeHelper.GetParent(popup) == null) ? popup.Child : GetRootVisual(popup));
            Windows.UI.Xaml.Controls.Primitives.Popup rootPopup = testElement as Windows.UI.Xaml.Controls.Primitives.Popup;
            if (rootPopup != null)
            {
                testElement = (FrameworkElement)rootPopup.Child;
            }
            return testElement;
        }
        
        FrameworkElement GetRootVisual(DependencyObject element)
        {
            DependencyObject parent = null;
            while (element != null)
            {
                parent = VisualTreeHelper.GetParent(element);
                if (parent == null)
                {
                    FrameworkElement childElement = element as FrameworkElement;
                    if (childElement != null)
                    {
                        parent = childElement.Parent;
                        if (parent == null)
                        {
                            parent = element;
                            break;
                        }
                    }
                }
                element = parent;
            }
            return (parent as FrameworkElement);
        }
        #endregion
    }
}

