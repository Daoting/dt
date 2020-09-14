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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base
{
    public partial class Excel
    {
        internal void ResetCursor()
        {
            SetBuiltInCursor(CoreCursorType.Arrow);
        }

        void HideMouseCursor()
        {
            if (_mouseCursor != null)
            {
                _mouseCursor.Opacity = 0.0;
            }
        }

        void ResetMouseCursor()
        {
            if (_mouseCursor != null)
            {
                _mouseCursor.Opacity = 0.0;
            }
            ResetCursor();
        }

        internal void SetBuiltInCursor(CoreCursorType cursorType)
        {
#if UWP
            CoreWindow win = Windows.UI.Xaml.Window.Current.CoreWindow;
            win.PointerCursor = new CoreCursor(cursorType, 0);
#endif
        }

        void SetCursor(HitTestInformation hi)
        {
            if (CanUserDragFill && hi.ViewportInfo.InDragFillIndicator)
            {
                bool flag;
                bool flag2;
                KeyboardHelper.GetMetaKeyState(out flag, out flag2);
                CursorType cursorType = flag2 ? CursorType.DragFill_CtrlDragCursor : CursorType.DragFill_DragCursor;
                SetMouseCursor(cursorType);
            }
            else if (CanUserDragDrop && hi.ViewportInfo.InSelectionDrag)
            {
                bool flag3;
                bool flag4;
                KeyboardHelper.GetMetaKeyState(out flag3, out flag4);
                CursorType type2 = flag4 ? CursorType.DragCell_CtrlDragCursor : CursorType.DragCell_DragCursor;
                SetMouseCursor(type2);
            }
            else
            {
                if (_mouseCursor != null)
                {
                    _mouseCursor.Opacity = 0.0;
                }
                ResetCursor();
            }
        }

        void SetCursorForFloatingObject(ViewportFloatingObjectHitTestInformation chartInfo)
        {
            // hdt 图表锁定时显示默认光标
            if (chartInfo.FloatingObject != null && chartInfo.FloatingObject.Locked)
            {
                ResetCursor();
            }
            else if (chartInfo.InMoving)
            {
                SetMouseCursor(CursorType.DragCell_DragCursor);
            }
            else if (chartInfo.InTopNWSEResize || chartInfo.InBottomNWSEResize)
            {
                SetBuiltInCursor(CoreCursorType.SizeNorthwestSoutheast);
            }
            else if (chartInfo.InLeftWEResize || chartInfo.InRightWEResize)
            {
                SetBuiltInCursor(CoreCursorType.SizeWestEast);
            }
            else if (chartInfo.InTopNSResize || chartInfo.InBottomNSResize)
            {
                SetBuiltInCursor(CoreCursorType.SizeNorthSouth);
            }
            else if (chartInfo.InTopNESWResize || chartInfo.InBottomNESWResize)
            {
                SetBuiltInCursor(CoreCursorType.SizeNortheastSouthwest);
            }
            else
            {
                ResetCursor();
            }
        }

        internal async void SetMouseCursor(CursorType cursorType)
        {
            if (_mouseCursor == null)
            {
                _mouseCursor = new Image();
                _mouseCursor.IsHitTestVisible = false;
                _trackersPanel.Children.Add(_mouseCursor);
            }
            _mouseCursor.Opacity = 1.0;
#if UWP
            Window.Current.CoreWindow.PointerCursor = null;
#endif
            _mouseCursor.Source = await CursorGenerator.GetCursor(cursorType);
            _mouseCursor.SetValue(Canvas.LeftProperty, (double)(MousePosition.X - 32.0));
            _mouseCursor.SetValue(Canvas.TopProperty, (double)(MousePosition.Y - 32.0));
        }

        void UpdateCursorType()
        {
            if ((_mouseCursor != null) && (_mouseCursor.Opacity != 0.0))
            {
                HitTestInformation savedHitTestInformation = GetHitInfo();
                if ((((savedHitTestInformation != null) && (savedHitTestInformation.ViewportInfo != null)) && savedHitTestInformation.ViewportInfo.InDragFillIndicator) || IsDraggingFill)
                {
                    bool flag;
                    bool flag2;
                    KeyboardHelper.GetMetaKeyState(out flag, out flag2);
                    CursorType cursorType = flag2 ? CursorType.DragFill_CtrlDragCursor : CursorType.DragFill_DragCursor;
                    UpdateMouseCursorType(cursorType);
                }
                else if ((((savedHitTestInformation != null) && (savedHitTestInformation.ViewportInfo != null)) && savedHitTestInformation.ViewportInfo.InSelectionDrag) || IsDragDropping)
                {
                    bool flag3;
                    bool flag4;
                    KeyboardHelper.GetMetaKeyState(out flag3, out flag4);
                    CursorType type2 = flag4 ? CursorType.DragCell_CtrlDragCursor : CursorType.DragCell_DragCursor;
                    UpdateMouseCursorType(type2);
                }
            }
        }

        internal void UpdateMouseCursorLocation()
        {
            if (_mouseCursor != null)
            {
                _mouseCursor.SetValue(Canvas.LeftProperty, (double)(MousePosition.X - 32.0));
                _mouseCursor.SetValue(Canvas.TopProperty, (double)(MousePosition.Y - 32.0));
            }
        }

        async void UpdateMouseCursorType(CursorType cursorType)
        {
            _mouseCursor.Source = await CursorGenerator.GetCursor(cursorType);
        }
    }
}

