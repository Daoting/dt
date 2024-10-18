#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-08-17 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.UI;
using Windows.UI.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Dt.Cells;
using Microsoft.UI.Input;
using System;
using Microsoft.UI.Xaml.Input;
#endregion

namespace Dt.Base
{
    public partial class Excel : IDestroy
    {
        /// <summary>
        /// 销毁对象，释放资源
        /// </summary>
        public void Destroy()
        {
            Workbook.Sheets.CollectionChanged -= OnSheetsCollectionChanged;
            Workbook.PropertyChanged -= OnWorkbookPropertyChanged;

            while (Workbook.Sheets.Count > 0)
            {
                var sheet = Workbook.Sheets[0];
                DetachSheet(sheet);
                Workbook.Sheets.RemoveAt(0);
            }
            Children.Clear();

            DetachPointerEvent();
            DetachScrollBar();
            HideProgressRing();
            CloseDragFillPopup();

            for (int i = 0; i < _cellsPanels.GetLength(0); i++)
            {
                for (int j = 0; j < _cellsPanels.GetLength(1); j++)
                {
                    var cp = _cellsPanels[i, j];
                    if (cp != null)
                    {
                        cp.Destroy();
                        _cellsPanels[i, j] = null;
                    }
                }
            }

            if (_tabStrip != null)
            {
                _tabStrip.ActiveTabChanging -= OnTabStripActiveTabChanging;
                _tabStrip.ActiveTabChanged -= OnTabStripActiveTabChanged;
                _tabStrip.NewTabNeeded -= OnTabStripNewTabNeeded;
                _tabStrip = null;
            }

            if (_filterPopup != null)
            {
                _filterPopup.Opened -= FilterPopup_Opened;
                _filterPopup.Closed -= FilterPopup_Closed;
                _filterPopup = null;
            }
        }

        void DetachScrollBar()
        {
            if (_horizontalScrollBar != null)
            {
                for (int j = 0; j < _horizontalScrollBar.Length; j++)
                {
                    var scrollBar = _horizontalScrollBar[j];
                    scrollBar.Scroll -= HorizontalScrollbar_Scroll;
                    scrollBar.PointerPressed -= OnHorizontalScrollBarPointerPressed;
                    scrollBar.PointerReleased -= OnHorizontalScrollBarPointerReleased;
                    scrollBar.PointerExited -= OnHorizontalScrollBarPointerExited;
                }
                _horizontalScrollBar = null;
            }

            if (_verticalScrollBar != null)
            {
                for (int j = 0; j < _verticalScrollBar.Length; j++)
                {
                    var scrollBar = _verticalScrollBar[j];
                    scrollBar.Scroll -= VerticalScrollbar_Scroll;
                    scrollBar.PointerPressed -= OnVerticalScrollbarPointerPressed;
                    scrollBar.PointerReleased -= OnVerticalScrollbarPointerReleased;
                    scrollBar.PointerExited -= OnVerticalScrollbarPointerExited;
                }
                _verticalScrollBar = null;
            }
        }

        void DetachPointerEvent()
        {
            PointerPressed -= OnPointerPressed;
            PointerMoved -= OnPointerMoved;
            PointerReleased -= OnPointerReleased;
            PointerExited -= OnPointerExited;
            PointerWheelChanged -= OnPointerWheelChanged;
            PointerCaptureLost -= OnPointerCaptureLost;
            RemoveHandler(DoubleTappedEvent, new DoubleTappedEventHandler(OnDoubleTapped));

            _gestrueRecognizer.Tapped -= OnGestureRecognizerTapped;
            _gestrueRecognizer.ManipulationStarted -= OnGestrueRecognizerManipulationStarted;
            _gestrueRecognizer.ManipulationUpdated -= OnGestrueRecognizerManipulationUpdated;
            _gestrueRecognizer.ManipulationCompleted -= OnGestrueRecognizerManipulationCompleted;

            // 标志已处理，屏蔽外部的左右滑动
            ManipulationMode = ManipulationModes.None | ManipulationModes.TranslateX;
            ManipulationStarted -= OnExcelManipulationStarted;
        }
    }

    /// <summary>
    /// 销毁对象接口，因uno中FrameworkElement已实现 IDisposable 
    /// </summary>
    public interface IDestroy
    {
        /// <summary>
        /// 销毁对象，释放资源
        /// </summary>
        void Destroy();
    }
}