#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Cells.UI
{
    public partial class SheetView : Panel, IXmlSerializable
    {
        /// <summary>
        /// 是否显示修饰层
        /// </summary>
        public readonly static DependencyProperty ShowDecorationProperty = DependencyProperty.Register(
            "ShowDecoration",
            typeof(bool),
            typeof(SheetView),
            new PropertyMetadata(false));

        Size _paperSize;
        CellRange _decorationRange;

        /// <summary>
        /// 报表项开始拖放事件
        /// </summary>
        public event EventHandler ItemStartDrag;

        /// <summary>
        /// 报表项拖放结束事件
        /// </summary>
        public event EventHandler<CellEventArgs> ItemDropped;

        /// <summary>
        /// 获取设置是否显示修饰层
        /// </summary>
        public bool ShowDecoration
        {
            get { return (bool)GetValue(ShowDecorationProperty); }
            set { SetValue(ShowDecorationProperty, value); }
        }

        /// <summary>
        /// 获取设置页面大小，修饰层画线用
        /// </summary>
        public Size PaperSize
        {
            get { return _paperSize; }
            set
            {
                if (_paperSize != value)
                {
                    _paperSize = value;
                    InvalidateDecoration();
                }
            }
        }

        /// <summary>
        /// 获取设置修饰区域
        /// </summary>
        public CellRange DecorationRange
        {
            get { return _decorationRange; }
            set
            {
                if (_decorationRange != value)
                {
                    _decorationRange = value;
                    InvalidateDecoration();
                }
            }
        }

        void InvalidateDecoration()
        {
            if (_viewportPresenters != null)
            {
                int rowBound = _viewportPresenters.GetUpperBound(0);
                int colBound = _viewportPresenters.GetUpperBound(1);
                for (int i = _viewportPresenters.GetLowerBound(0); i <= rowBound; i++)
                {
                    for (int j = _viewportPresenters.GetLowerBound(1); j <= colBound; j++)
                    {
                        GcViewport viewport = _viewportPresenters[i, j];
                        if (viewport != null)
                            viewport.InvalidateDecorationPanel();
                    }
                }
            }
        }

        /// <summary>
        /// 触发报表项开始拖放事件
        /// </summary>
        internal void OnItemStartDrag()
        {
            if (ItemStartDrag != null)
                ItemStartDrag(this, EventArgs.Empty);
        }

        /// <summary>
        /// 触发报表项拖放结束事件
        /// </summary>
        /// <param name="p_args"></param>
        internal void OnItemDropped(CellEventArgs p_args)
        {
            if (ItemDropped != null)
                ItemDropped(this, p_args);
        }

        /// <summary>
        /// hdt 标志已处理，屏蔽Pis的左右滑动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        void OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            e.Handled = true;
        }

        void OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// 设置打印时隐藏分页线
        /// </summary>
        internal bool HideDecorationWhenPrinting
        {
            set
            {
                if (_viewportPresenters != null)
                {
                    int rowBound = _viewportPresenters.GetUpperBound(0);
                    int colBound = _viewportPresenters.GetUpperBound(1);
                    for (int i = _viewportPresenters.GetLowerBound(0); i <= rowBound; i++)
                    {
                        for (int j = _viewportPresenters.GetLowerBound(1); j <= colBound; j++)
                        {
                            GcViewport viewport = _viewportPresenters[i, j];
                            if (viewport != null)
                                viewport.HideDecorationWhenPrinting = value;
                        }
                    }
                }
            }
        }
    }
}

