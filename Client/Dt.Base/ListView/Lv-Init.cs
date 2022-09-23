#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-09-20 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.ListView;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 初始化相关
    /// </summary>
    public partial class Lv
    {
        /************************************************************************************************************************************/
        // 在MeasureOverride中尽可能不增删Children元素，uno中每增删一个元素会重复一次MeasureOverride，严重时死循环！！！
        // 采用虚拟行模式时，需要根据可视区大小确定生成的虚拟行行数，可视区大小在MeasureOverride时才能确定，故解决方法：
        // 在Lv.MeasureOverride时准确获取可见区大小，若大小变化则重新生成虚拟行，添加虚拟行会造成多次MeasureOverride，未发现其他好方法！！！
        // 若放在SizeChanged中生成虚拟行时uno会警告 requestLayout() improperly called by xxx: posting in next frame！！！
        //
        // 重新生成虚拟行的场景：
        // 1. 可视区大小变化时 LvPanel.SetMaxSize
        // 2. View CellEx ItemHeight等属性变化时的重新加载 LvPanel.Reload
        // 3. 切换数据源时 LvPanel.OnRowsChanged
        //
        // 调用UpdateLayout的不同：
        // WIN：UpdateLayout内部会依次 > MeasureOverride > ArrangeOverride > SizeChanged
        // uno: UpdateLayout调用时未同步调用上述方法，内部异步测量布局，和InvalidateMeasure功能相似
        /************************************************************************************************************************************/


        /**********************************************************************************************************************************************************/
        // 不同平台主事件调用顺序
        // 
        // WIN：
        // ApplyTemplate(父子) -> OnLoadTemplate(父子) -> MeasureOverride(父子) -> ArrangeOverride(父子) -> SizeChanged(父子) -> Loaded(父子) -> OnControlLoaded(父子)
        // 
        // Android：
        // ApplyTemplate(子父) -> Loaded(父子) -> OnLoadTemplate(父子) -> OnControlLoaded(父子) -> MeasureOverride(父子) -> ArrangeOverride(父子) -> SizeChanged(子父)
        // 
        // iOS：
        // ApplyTemplate(子父) -> Loaded(子父) -> OnLoadTemplate(子父) -> OnControlLoaded(子父) -> MeasureOverride(父子) -> SizeChanged(父子) -> ArrangeOverride(父子)
        //
        // wasm:
        // ApplyTemplate(父子) -> Loaded(父子) -> OnLoadTemplate(父子) -> OnControlLoaded(父子) -> MeasureOverride(父子) -> ArrangeOverride(父子) -> SizeChanged(子父)
        //
        // WIN的OnApplyTemplate时控件已在可视树上，可查询父元素；uno此时不在可视树上，只能在Loaded时查询父元素！！！
        /***********************************************************************************************************************************************************/


        /************************************************************************************************************************************/
        // Lv控件模板只有一个Border，内部元素动态构造
        //
        // 调整说明：
        // 1. 原来继承 DtControl，主要原因是 uno 在 OnApplyTemplate 方法中无法查找父元素中是否有 ScrollViewer
        //    所以动态构造内部元素 win 在 OnApplyTemplate 中处理，uno 在 Loaded 事件中处理
        // 2. 后因增加下拉刷新功能，RefreshContainer 在 Loaded 事件动态添加时，在iOS上下拉刷新功能无效
        // 3. 因 uno 无法在 OnApplyTemplate 中查找父元素，先假设无外部滚动栏动态添加元素，在 Loaded 事件中再处理外部滚动栏的情况
        //    外部滚动栏情况非常少，故效率不受影响
        /************************************************************************************************************************************/


        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _root = (Border)GetTemplateChild("Border");

            ScrollViewer sv;

#if WIN
            // win模式可查找父元素，查询范围限制在Tabs内
            sv = this.FindParentInWin<ScrollViewer>();
            if (sv == null)
            {
                // 内部滚动栏
                sv = new ScrollViewer();
                if (PullToRefresh)
                {
                    // 只有滚动栏在内部时支持下拉刷新
                    var rc = new RefreshContainer { Content = sv };
                    _root.Child = rc;
                    rc.RefreshRequested += OnRefreshRequested;
                }
                else
                {
                    _root.Child = sv;
                }
            }
            else if (Kit.IsPhoneUI)
            {
                // 外部滚动栏
                // 参见win.xaml：win模式在Tabs定义，phone模式在Tab定义
                // 因phone模式Lv所属的Tab始终不变
                _sizedPresenter = sv.FindParentInWin<SizedPresenter>();
            }
#else
            // uno无法查找父元素，假设无外部滚动栏，在 Loaded 事件中再处理外部滚动栏的情况
            sv = new ScrollViewer();
            if (PullToRefresh)
            {
                // 只有滚动栏在内部时支持下拉刷新
                var rc = new RefreshContainer { Content = sv };
                _root.Child = rc;
                rc.RefreshRequested += OnRefreshRequested;
            }
            else
            {
                _root.Child = sv;
            }
#endif

            sv.HorizontalScrollMode = ScrollMode.Auto;
            sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            sv.VerticalScrollMode = ScrollMode.Auto;
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            Scroll = sv;

            LoadPanel();
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

#if !WIN
            // 处理外部滚动栏的情况，因 uno 在 OnApplyTemplate 方法中无法查找父元素
            // win模式查询范围限制在Tabs内，phone模式限制在Tab内
            var sv = this.FindParentInWin<ScrollViewer>();
            if (sv != null)
            {
                if (_root.Child is RefreshContainer rc)
                {
                    rc.RefreshRequested -= OnRefreshRequested;
                    rc.Content = null;
                }
                Scroll.Content = null;
                _root.Child = _panel;

                if (Kit.IsPhoneUI)
                {
                    // 参见win.xaml：win模式在Tabs定义，phone模式在Tab定义
                    // 因phone模式Lv所属的Tab始终不变
                    _sizedPresenter = sv.FindParentInWin<SizedPresenter>();
                }

                sv.HorizontalScrollMode = ScrollMode.Auto;
                sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                sv.VerticalScrollMode = ScrollMode.Auto;
                sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                Scroll = sv;
            }
#endif

            // 滚动到顶部或底部时添加分页数据
            if (PageData != null)
                Scroll.ViewChanged += OnScrollViewChanged;

            // 确保初次加载后键盘操作有效
            Focus(FocusState.Programmatic);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            // 准确获取可见区大小！
            if (_panel != null)
            {
                if (!double.IsInfinity(availableSize.Width) && !double.IsInfinity(availableSize.Height))
                {
                    // 外部无ScrollViewer StackPanel的情况
                    _panel.SetMaxSize(availableSize);
                }
                else if (_sizedPresenter != null)
                {
                    // Phone模式Lv外部有ScrollViewer时，取父级SizedPresenter有效大小
                    _panel.SetMaxSize(new Size(
                        Math.Min(_sizedPresenter.AvailableSize.Width, availableSize.Width),
                        Math.Min(_sizedPresenter.AvailableSize.Height, availableSize.Height)));
                }
                else if (!IsInnerScroll)
                {
                    // Win模式外部有ScrollViewer时，动态获取父级，因所属Tabs在Win中恢复布局时变化
                    var pre = Scroll.FindParentInWin<SizedPresenter>();
                    _panel.SetMaxSize(new Size(
                        Math.Min(pre.AvailableSize.Width, availableSize.Width),
                        Math.Min(pre.AvailableSize.Height, availableSize.Height)));
                }
                else
                {
                    // 无有效大小时以窗口大小为准
                    double width = double.IsInfinity(availableSize.Width) ? Kit.ViewWidth : availableSize.Width;
                    double height = double.IsInfinity(availableSize.Height) ? Kit.ViewHeight : availableSize.Height;
                    _panel.SetMaxSize(new Size(width, height));
                }
            }
            return base.MeasureOverride(availableSize);
        }

        /// <summary>
        /// 动态加载面板
        /// </summary>
        void LoadPanel()
        {
            if (_root == null || View == null)
                return;

            if (_panel != null)
            {
                if (IsInnerScroll)
                    Scroll.Content = null;
                else
                    _root.Child = null;
                _panel.Unload();
            }

            LvPanel pnl;
            ViewMode mode = CurrentViewMode;
            if (mode == ViewMode.List)
            {
                pnl = new ListPanel(this);
            }
            else if (mode == ViewMode.Table)
            {
                if (View is Cols)
                    pnl = new TablePanel(this);
                else
                    throw new Exception("未提供表格所需的Cols定义！");
            }
            else
            {
                pnl = new TilePanel(this);
            }
            // 切换面板时保留大小
            if (_panel != null)
                pnl.SetMaxSize(_panel.GetMaxSize());
            _panel = pnl;

            if (mode == ViewMode.Table)
            {
                Scroll.HorizontalScrollMode = ScrollMode.Auto;
                Scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            }
            else
            {
                Scroll.HorizontalScrollMode = ScrollMode.Disabled;
                Scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            }

            // 内部有滚动栏时，面板放在滚动栏内
            if (IsInnerScroll)
                Scroll.Content = _panel;
            else
                _root.Child = _panel;
        }

        /// <summary>
        /// 重新加载面板内容
        /// </summary>
        void Reload()
        {
            if (_panel != null && !_updatingView)
                _panel.Reload();
        }
    }
}