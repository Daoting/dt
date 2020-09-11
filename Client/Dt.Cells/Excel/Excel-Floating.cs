#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-08-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using Dt.Cells.UI;
using Dt.Cells.UndoRedo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Foundation;
#endregion

namespace Dt.Base
{
    public partial class Excel
    {
        bool InitFloatingObjectsMovingResizing()
        {
            HitTestInformation savedHitTestInformation = GetHitInfo();
            if (IsTouching)
            {
                savedHitTestInformation = _touchStartHitTestInfo;
            }
            if (((savedHitTestInformation.ViewportInfo == null) || (savedHitTestInformation.RowViewportIndex == -2)) || (savedHitTestInformation.ColumnViewportIndex == 2))
            {
                return false;
            }
            _floatingObjectsMovingResizingStartRow = savedHitTestInformation.ViewportInfo.Row;
            _floatingObjectsMovingResizingStartColumn = savedHitTestInformation.ViewportInfo.Column;
            _dragStartRowViewport = savedHitTestInformation.RowViewportIndex;
            _dragStartColumnViewport = savedHitTestInformation.ColumnViewportIndex;
            _dragToRowViewport = savedHitTestInformation.RowViewportIndex;
            _dragToColumnViewport = savedHitTestInformation.ColumnViewportIndex;
            _floatingObjectsMovingResizingStartPoint = savedHitTestInformation.HitPoint;
            SetActiveColumnViewportIndex(savedHitTestInformation.ColumnViewportIndex);
            SetActiveRowViewportIndex(savedHitTestInformation.RowViewportIndex);
            CachFloatingObjectsMovingResizingLayoutModels();
            RowLayout viewportRowLayoutNearY = GetViewportRowLayoutNearY(_dragStartRowViewport, _floatingObjectsMovingResizingStartPoint.Y);
            ColumnLayout viewportColumnLayoutNearX = GetViewportColumnLayoutNearX(_dragToColumnViewport, _floatingObjectsMovingResizingStartPoint.X);
            _floatingObjectsMovingResizingStartPointCellBounds = new Rect(viewportColumnLayoutNearX.X, viewportRowLayoutNearY.Y, viewportColumnLayoutNearX.Width, viewportRowLayoutNearY.Height);
            _floatingObjectsMovingStartLocations = new Dictionary<string, Point>();
            for (int i = 0; i < _movingResizingFloatingObjects.Count; i++)
            {
                IFloatingObject obj2 = _movingResizingFloatingObjects[i];
                _floatingObjectsMovingStartLocations.Add(obj2.Name, obj2.Location);
            }
            return true;
        }

        void StartFloatingObjectsMoving()
        {
            _movingResizingFloatingObjects = GetAllSelectedFloatingObjects();
            if (((_movingResizingFloatingObjects != null) && (_movingResizingFloatingObjects.Count != 0)) && InitFloatingObjectsMovingResizing())
            {
                if ((_touchToolbarPopup != null) && _touchToolbarPopup.IsOpen)
                {
                    _touchToolbarPopup.IsOpen = false;
                }
                IsWorking = true;
                if (IsTouching)
                {
                    IsTouchingMovingFloatingObjects = true;
                }
                else
                {
                    IsMovingFloatingOjects = true;
                }
                StartScrollTimer();
            }
        }

        void StartFloatingObjectsResizing()
        {
            _movingResizingFloatingObjects = GetAllSelectedFloatingObjects();
            if (((_movingResizingFloatingObjects != null) && (_movingResizingFloatingObjects.Count != 0)) && InitFloatingObjectsMovingResizing())
            {
                if ((_touchToolbarPopup != null) && _touchToolbarPopup.IsOpen)
                {
                    _touchToolbarPopup.IsOpen = false;
                }
                IsWorking = true;
                if (IsTouching)
                {
                    IsTouchingResizingFloatingObjects = true;
                }
                else
                {
                    IsResizingFloatingObjects = true;
                }
                StartScrollTimer();
            }
        }

        internal bool HasSelectedFloatingObject()
        {
            foreach (IFloatingObject obj2 in GetAllFloatingObjects())
            {
                if (obj2.IsSelected)
                {
                    return true;
                }
            }
            return false;
        }

        void CachFloatingObjectsMovingResizingLayoutModels()
        {
            ViewportInfo viewportInfo = GetViewportInfo();
            int columnViewportCount = viewportInfo.ColumnViewportCount;
            int rowViewportCount = viewportInfo.RowViewportCount;
            _cachedFloatingObjectMovingResizingLayoutModel = new FloatingObjectLayoutModel[rowViewportCount + 2, columnViewportCount + 2];
            for (int i = -1; i <= rowViewportCount; i++)
            {
                for (int j = -1; j <= columnViewportCount; j++)
                {
                    _cachedFloatingObjectMovingResizingLayoutModel[i + 1, j + 1] = new FloatingObjectLayoutModel(GetViewportFloatingObjectLayoutModel(i, j));
                }
            }
        }

        internal void UnSelectedAllFloatingObjects()
        {
            var ls = GetAllFloatingObjects();
            if (ls.Count > 0)
            {
                foreach (var obj in ls)
                {
                    if (obj.IsSelected)
                        obj.IsSelected = false;
                }
            }
        }

        void UnSelectFloatingObject(FloatingObject floatingObject)
        {
            try
            {
                if (!_isMouseDownFloatingObject)
                {
                    bool flag;
                    bool flag2;
                    KeyboardHelper.GetMetaKeyState(out flag, out flag2);
                    if (((flag2 || flag) && !(floatingObject.Locked && ActiveSheet.Protect)) && floatingObject.IsSelected)
                    {
                        floatingObject.IsSelected = false;
                    }
                }
            }
            finally
            {
                _isMouseDownFloatingObject = false;
            }
        }

        void ContinueFloatingObjectsMoving()
        {
            if (IsTouching)
            {
                if (!IsTouchingMovingFloatingObjects)
                {
                    return;
                }
            }
            else if (!IsMovingFloatingOjects)
            {
                return;
            }
            if ((_movingResizingFloatingObjects != null) && (_movingResizingFloatingObjects.Count != 0))
            {
                UpdateFloatingObjectsMovingResizingToViewports();
                UpdateFloatingObjectsMovingResizingToCoordicates();
                RefreshViewportFloatingObjectsContainerMoving();
                ProcessScrollTimer();
            }
        }

        void ContinueFloatingObjectsResizing()
        {
            if (IsTouching)
            {
                if (!IsTouchingResizingFloatingObjects)
                {
                    return;
                }
            }
            else if (!IsResizingFloatingObjects)
            {
                return;
            }
            if ((_movingResizingFloatingObjects != null) && (_movingResizingFloatingObjects.Count != 0))
            {
                UpdateFloatingObjectsMovingResizingToViewports();
                UpdateFloatingObjectsMovingResizingToCoordicates();
                RefreshViewportFloatingObjectsContainerResizing();
                ProcessScrollTimer();
            }
        }

        void DoDragFloatingObjects()
        {
            SuspendFloatingObjectsInvalidate();
            _floatingObjectsMovingResizingOffset = CalcMoveOffset(_dragStartRowViewport, _dragStartColumnViewport, _floatingObjectsMovingResizingStartRow, _floatingObjectsMovingResizingStartColumn, _floatingObjectsMovingResizingStartPoint, _dragToRowViewport, _dragToColumnViewport, _dragToRow, _dragToColumn, MousePosition);
            if ((_movingResizingFloatingObjects != null) && (_movingResizingFloatingObjects.Count > 0))
            {
                List<string> list = new List<string>();
                foreach (FloatingObject obj2 in _movingResizingFloatingObjects)
                {
                    list.Add(obj2.Name);
                }
                MoveFloatingObjectExtent extent = new MoveFloatingObjectExtent(list.ToArray(), _floatingObjectsMovingResizingOffset.X, _floatingObjectsMovingResizingOffset.Y);
                DoCommand(new DragFloatingObjectUndoAction(ActiveSheet, extent));
            }
            ResumeFloatingObjectsInvalidate();
        }

        void DoMoveFloatingObjects()
        {
            SuspendFloatingObjectsInvalidate();
            _floatingObjectsMovingResizingOffset = CalcMoveOffset(_dragStartRowViewport, _dragStartColumnViewport, _floatingObjectsMovingResizingStartRow, _floatingObjectsMovingResizingStartColumn, _floatingObjectsMovingResizingStartPoint, _dragToRowViewport, _dragToColumnViewport, _dragToRow, _dragToColumn, MousePosition);
            if ((_movingResizingFloatingObjects != null) && (_movingResizingFloatingObjects.Count > 0))
            {
                List<string> list = new List<string>();
                foreach (FloatingObject obj2 in _movingResizingFloatingObjects)
                {
                    list.Add(obj2.Name);
                }
                MoveFloatingObjectExtent extent = new MoveFloatingObjectExtent(list.ToArray(), _floatingObjectsMovingResizingOffset.X, _floatingObjectsMovingResizingOffset.Y);
                DoCommand(new MoveFloatingObjectUndoAction(ActiveSheet, extent));
            }
            ResumeFloatingObjectsInvalidate();
        }

        void DoResizeFloatingObjects()
        {
            SuspendFloatingObjectsInvalidate();
            if ((_movingResizingFloatingObjects != null) && (_movingResizingFloatingObjects.Count > 0))
            {
                int activeRowViewportIndex = GetActiveRowViewportIndex();
                int activeColumnViewportIndex = GetActiveColumnViewportIndex();
                Rect[] floatingObjectsResizingRects = GetFloatingObjectsResizingRects(activeRowViewportIndex, activeColumnViewportIndex);
                List<string> list = new List<string>();
                List<Rect> list2 = new List<Rect>();
                for (int i = 0; (i < _movingResizingFloatingObjects.Count) && (i < floatingObjectsResizingRects.Length); i++)
                {
                    FloatingObject obj2 = _movingResizingFloatingObjects[i];
                    Rect rect = new Rect(floatingObjectsResizingRects[i].X, floatingObjectsResizingRects[i].Y, floatingObjectsResizingRects[i].Width, floatingObjectsResizingRects[i].Height);
                    RowLayout viewportRowLayoutNearY = GetViewportRowLayoutNearY(activeRowViewportIndex, rect.Y);
                    if (viewportRowLayoutNearY == null)
                    {
                        viewportRowLayoutNearY = GetViewportRowLayoutNearY(-1, rect.Y);
                    }
                    int row = 0;
                    if (viewportRowLayoutNearY != null)
                    {
                        row = viewportRowLayoutNearY.Row;
                    }
                    double num5 = rect.Y - viewportRowLayoutNearY.Y;
                    double y = 0.0;
                    for (int j = 0; j < row; j++)
                    {
                        double num8 = Math.Ceiling((double)(ActiveSheet.GetActualRowHeight(j, SheetArea.Cells) * ZoomFactor));
                        y += num8;
                    }
                    y += num5;
                    ColumnLayout viewportColumnLayoutNearX = GetViewportColumnLayoutNearX(activeColumnViewportIndex, rect.X);
                    if (viewportColumnLayoutNearX == null)
                    {
                        viewportColumnLayoutNearX = GetViewportColumnLayoutNearX(-1, rect.X);
                    }
                    double column = 0.0;
                    if (viewportColumnLayoutNearX != null)
                    {
                        column = viewportColumnLayoutNearX.Column;
                    }
                    double num10 = rect.X - viewportColumnLayoutNearX.X;
                    double x = 0.0;
                    for (int k = 0; k < column; k++)
                    {
                        double num13 = Math.Ceiling((double)(ActiveSheet.GetActualColumnWidth(k, SheetArea.Cells) * ZoomFactor));
                        x += num13;
                    }
                    x += num10;
                    x = Math.Floor((double)(x / ((double)ZoomFactor)));
                    y = Math.Floor((double)(y / ((double)ZoomFactor)));
                    double width = Math.Floor((double)(rect.Width / ((double)ZoomFactor)));
                    double height = Math.Floor((double)(rect.Height / ((double)ZoomFactor)));
                    list.Add(obj2.Name);
                    list2.Add(new Rect(x, y, width, height));
                }
                ResizeFloatingObjectExtent extent = new ResizeFloatingObjectExtent(list.ToArray(), list2.ToArray());
                DoCommand(new ResizeFloatingObjectUndoAction(ActiveSheet, extent));
            }
            ResumeFloatingObjectsInvalidate();
        }

        void EndFloatingObjectResizing()
        {
            if ((((_dragStartRowViewport == -2) || (_dragStartColumnViewport == -2)) || ((_floatingObjectsMovingResizingStartRow == -2) || (_floatingObjectsMovingResizingStartColumn == -2))) || (((_dragToRowViewport == -2) || (_dragToColumnViewport == -2)) || ((_dragToRow == -2) || (_dragToColumn == -2))))
            {
                ResetFloatingObjectsMovingResizing();
                StopScrollTimer();
            }
            else
            {
                DoResizeFloatingObjects();
                InvalidateFloatingObjectsLayoutModel();
                RefreshViewportFloatingObjectsLayout();
                ResetFloatingObjectsMovingResizing();
                StopScrollTimer();
            }
        }

        void EndFloatingObjectsMoving()
        {
            if ((((_dragStartRowViewport == -2) || (_dragStartColumnViewport == -2)) || ((_floatingObjectsMovingResizingStartRow == -2) || (_floatingObjectsMovingResizingStartColumn == -2))) || (((_dragToRowViewport == -2) || (_dragToColumnViewport == -2)) || ((_dragToRow == -2) || (_dragToColumn == -2))))
            {
                ResetFloatingObjectsMovingResizing();
                StopScrollTimer();
            }
            else
            {
                bool ctrl = false;
                bool shift = false;
                KeyboardHelper.GetMetaKeyState(out shift, out ctrl);
                if (ctrl)
                {
                    DoDragFloatingObjects();
                }
                else
                {
                    DoMoveFloatingObjects();
                }
                InvalidateFloatingObjectsLayoutModel();
                RefreshViewportFloatingObjectsLayout();
                ResetFloatingObjectsMovingResizing();
                StopScrollTimer();
                if (InputDeviceType == InputDeviceType.Touch)
                {
                    base.InvalidateMeasure();
                }
            }
        }

        Rect[] GetFloatingObjectsBottomCenterResizingRects(int rowViewport, int columnViewport, Point mousePosition)
        {
            List<Rect> list = new List<Rect>();
            FloatingObjectLayoutModel cacheFloatingObjectsMovingResizingLayoutModels = GetCacheFloatingObjectsMovingResizingLayoutModels(rowViewport, columnViewport);
            FloatingObjectLayoutModel viewportFloatingObjectLayoutModel = GetViewportFloatingObjectLayoutModel(rowViewport, columnViewport);
            Point point = new Point(mousePosition.X - _floatingObjectsMovingResizingStartPoint.X, mousePosition.Y - _floatingObjectsMovingResizingStartPoint.Y);
            foreach (FloatingObject obj2 in _movingResizingFloatingObjects)
            {
                FloatingObjectLayout layout = cacheFloatingObjectsMovingResizingLayoutModels.Find(obj2.Name);
                FloatingObjectLayout layout2 = viewportFloatingObjectLayoutModel.Find(obj2.Name);
                Point point2 = new Point(layout.X + layout.Width, layout.Y + layout.Height);
                Point point3 = new Point(point2.X + point.X, point2.Y + point.Y);
                Point point4 = new Point(layout2.X, layout2.Y);
                double y = Math.Min(point3.Y, point4.Y);
                double height = Math.Abs((double)(point3.Y - point4.Y));
                double width = layout2.Width;
                Rect rect = new Rect(layout2.X, y, width, height);
                list.Add(rect);
            }
            return list.ToArray();
        }

        Rect[] GetFloatingObjectsBottomLeftResizingRects(int rowViewport, int columnViewport, Point mousePosition)
        {
            List<Rect> list = new List<Rect>();
            FloatingObjectLayoutModel cacheFloatingObjectsMovingResizingLayoutModels = GetCacheFloatingObjectsMovingResizingLayoutModels(rowViewport, columnViewport);
            FloatingObjectLayoutModel viewportFloatingObjectLayoutModel = GetViewportFloatingObjectLayoutModel(rowViewport, columnViewport);
            Point point = new Point(mousePosition.X - _floatingObjectsMovingResizingStartPoint.X, mousePosition.Y - _floatingObjectsMovingResizingStartPoint.Y);
            foreach (FloatingObject obj2 in _movingResizingFloatingObjects)
            {
                FloatingObjectLayout layout = cacheFloatingObjectsMovingResizingLayoutModels.Find(obj2.Name);
                FloatingObjectLayout layout2 = viewportFloatingObjectLayoutModel.Find(obj2.Name);
                Point point2 = new Point(layout.X, layout.Y + layout.Height);
                Point point3 = new Point(point2.X + point.X, point2.Y + point.Y);
                Point point4 = new Point(layout2.X + layout2.Width, layout2.Y);
                double x = Math.Min(point3.X, point4.X);
                double y = Math.Min(point3.Y, point4.Y);
                double width = Math.Abs((double)(point4.X - point3.X));
                double height = Math.Abs((double)(point4.Y - point3.Y));
                Rect rect = new Rect(x, y, width, height);
                list.Add(rect);
            }
            return list.ToArray();
        }

        Rect[] GetFloatingObjectsBottomRighResizingRects(int rowViewport, int columnViewport, Point mousePosition)
        {
            List<Rect> list = new List<Rect>();
            FloatingObjectLayoutModel cacheFloatingObjectsMovingResizingLayoutModels = GetCacheFloatingObjectsMovingResizingLayoutModels(rowViewport, columnViewport);
            FloatingObjectLayoutModel viewportFloatingObjectLayoutModel = GetViewportFloatingObjectLayoutModel(rowViewport, columnViewport);
            Point point = new Point(mousePosition.X - _floatingObjectsMovingResizingStartPoint.X, mousePosition.Y - _floatingObjectsMovingResizingStartPoint.Y);
            foreach (FloatingObject obj2 in _movingResizingFloatingObjects)
            {
                FloatingObjectLayout layout = cacheFloatingObjectsMovingResizingLayoutModels.Find(obj2.Name);
                FloatingObjectLayout layout2 = viewportFloatingObjectLayoutModel.Find(obj2.Name);
                Point point2 = new Point(layout.X + layout.Width, layout.Y + layout.Height);
                Point point3 = new Point(point2.X + point.X, point2.Y + point.Y);
                Point point4 = new Point(layout2.X, layout2.Y);
                double x = Math.Min(point3.X, point4.X);
                double y = Math.Min(point3.Y, point4.Y);
                double width = Math.Abs((double)(point3.X - point4.X));
                double height = Math.Abs((double)(point3.Y - point4.Y));
                Rect rect = new Rect(x, y, width, height);
                list.Add(rect);
            }
            return list.ToArray();
        }

        Rect[] GetFloatingObjectsMiddleLeftResizingRects(int rowViewport, int columnViewport, Point mousePosition)
        {
            List<Rect> list = new List<Rect>();
            FloatingObjectLayoutModel cacheFloatingObjectsMovingResizingLayoutModels = GetCacheFloatingObjectsMovingResizingLayoutModels(rowViewport, columnViewport);
            FloatingObjectLayoutModel viewportFloatingObjectLayoutModel = GetViewportFloatingObjectLayoutModel(rowViewport, columnViewport);
            Point point = new Point(mousePosition.X - _floatingObjectsMovingResizingStartPoint.X, mousePosition.Y - _floatingObjectsMovingResizingStartPoint.Y);
            foreach (FloatingObject obj2 in _movingResizingFloatingObjects)
            {
                FloatingObjectLayout layout = cacheFloatingObjectsMovingResizingLayoutModels.Find(obj2.Name);
                FloatingObjectLayout layout2 = viewportFloatingObjectLayoutModel.Find(obj2.Name);
                Point point2 = new Point(layout.X, layout.Y);
                Point point3 = new Point(point2.X + point.X, point2.Y + point.Y);
                Point point4 = new Point(layout2.X + layout2.Width, layout2.Y + layout2.Height);
                double x = Math.Min(point3.X, point4.X);
                double width = Math.Abs((double)(point4.X - point3.X));
                double height = layout2.Height;
                Rect rect = new Rect(x, layout2.Y, width, height);
                list.Add(rect);
            }
            return list.ToArray();
        }

        Rect[] GetFloatingObjectsMiddleRightResizingRects(int rowViewport, int columnViewport, Point mousePosition)
        {
            List<Rect> list = new List<Rect>();
            FloatingObjectLayoutModel cacheFloatingObjectsMovingResizingLayoutModels = GetCacheFloatingObjectsMovingResizingLayoutModels(rowViewport, columnViewport);
            FloatingObjectLayoutModel viewportFloatingObjectLayoutModel = GetViewportFloatingObjectLayoutModel(rowViewport, columnViewport);
            Point point = new Point(mousePosition.X - _floatingObjectsMovingResizingStartPoint.X, mousePosition.Y - _floatingObjectsMovingResizingStartPoint.Y);
            foreach (FloatingObject obj2 in _movingResizingFloatingObjects)
            {
                FloatingObjectLayout layout = cacheFloatingObjectsMovingResizingLayoutModels.Find(obj2.Name);
                FloatingObjectLayout layout2 = viewportFloatingObjectLayoutModel.Find(obj2.Name);
                Point point2 = new Point(layout.X + layout.Width, layout.Y + layout.Height);
                Point point3 = new Point(point2.X + point.X, point2.Y + point.Y);
                Point point4 = new Point(layout2.X, layout2.Y);
                double x = Math.Min(point3.X, point4.X);
                double width = Math.Abs((double)(point3.X - point4.X));
                double height = layout2.Height;
                Rect rect = new Rect(x, layout2.Y, width, height);
                list.Add(rect);
            }
            return list.ToArray();
        }

        internal Rect[] GetFloatingObjectsMovingFrameRects(int rowViewport, int columnViewport)
        {
            var allSelectedFloatingObjects = GetAllSelectedFloatingObjects();
            if ((allSelectedFloatingObjects == null) || (allSelectedFloatingObjects.Count == 0))
            {
                return null;
            }
            List<Rect> list = new List<Rect>();
            Point mousePosition = MousePosition;
            new Point(mousePosition.X - _floatingObjectsMovingResizingStartPoint.X, mousePosition.Y - _floatingObjectsMovingResizingStartPoint.Y);
            FloatingObjectLayoutModel cacheFloatingObjectsMovingResizingLayoutModels = GetCacheFloatingObjectsMovingResizingLayoutModels(rowViewport, columnViewport);
            foreach (FloatingObject obj2 in allSelectedFloatingObjects)
            {
                bool flag;
                bool flag2;
                FloatingObjectLayout layout = cacheFloatingObjectsMovingResizingLayoutModels.Find(obj2.Name);
                Point point2 = new Point(_floatingObjectsMovingResizingStartPoint.X - layout.X, _floatingObjectsMovingResizingStartPoint.Y - layout.Y);
                double x = mousePosition.X - point2.X;
                double y = mousePosition.Y - point2.Y;
                KeyboardHelper.GetMetaKeyState(out flag, out flag2);
                if (flag)
                {
                    double num3 = x - layout.X;
                    double num4 = y - layout.Y;
                    if (Math.Abs(num3) > Math.Abs(num4))
                    {
                        y = layout.Y;
                    }
                    else
                    {
                        x = layout.X;
                    }
                }
                list.Add(new Rect(x, y, layout.Width, layout.Height));
            }
            return list.ToArray();
        }

        internal Rect[] GetFloatingObjectsResizingRects(int rowViewport, int columnViewport)
        {
            if ((_movingResizingFloatingObjects == null) || (_movingResizingFloatingObjects.Count == 0))
            {
                return null;
            }
            Point mousePosition = MousePosition;
            HitTestInformation savedHitTestInformation = GetHitInfo();
            if (IsTouchingResizingFloatingObjects || IsTouchingMovingFloatingObjects)
            {
                savedHitTestInformation = _touchStartHitTestInfo;
            }
            if (savedHitTestInformation.FloatingObjectInfo == null)
            {
                Debugger.Break();
            }
            if (savedHitTestInformation.FloatingObjectInfo.InTopNWSEResize)
            {
                return GetFloatingObjectsTopleftResizingRects(rowViewport, columnViewport, mousePosition);
            }
            if (savedHitTestInformation.FloatingObjectInfo.InTopNSResize)
            {
                return GetFloatingObjectsTopCenterResizingRects(rowViewport, columnViewport, mousePosition);
            }
            if (savedHitTestInformation.FloatingObjectInfo.InTopNESWResize)
            {
                return GetFloatingObjectsTopRightResizingRects(rowViewport, columnViewport, mousePosition);
            }
            if (savedHitTestInformation.FloatingObjectInfo.InLeftWEResize)
            {
                return GetFloatingObjectsMiddleLeftResizingRects(rowViewport, columnViewport, mousePosition);
            }
            if (savedHitTestInformation.FloatingObjectInfo.InRightWEResize)
            {
                return GetFloatingObjectsMiddleRightResizingRects(rowViewport, columnViewport, mousePosition);
            }
            if (savedHitTestInformation.FloatingObjectInfo.InBottomNESWResize)
            {
                return GetFloatingObjectsBottomLeftResizingRects(rowViewport, columnViewport, mousePosition);
            }
            if (savedHitTestInformation.FloatingObjectInfo.InBottomNSResize)
            {
                return GetFloatingObjectsBottomCenterResizingRects(rowViewport, columnViewport, mousePosition);
            }
            if (savedHitTestInformation.FloatingObjectInfo.InBottomNWSEResize)
            {
                return GetFloatingObjectsBottomRighResizingRects(rowViewport, columnViewport, mousePosition);
            }
            return new List<Rect>().ToArray();
        }

        Rect[] GetFloatingObjectsTopCenterResizingRects(int rowViewport, int columnViewport, Point mousePosition)
        {
            List<Rect> list = new List<Rect>();
            FloatingObjectLayoutModel cacheFloatingObjectsMovingResizingLayoutModels = GetCacheFloatingObjectsMovingResizingLayoutModels(rowViewport, columnViewport);
            FloatingObjectLayoutModel viewportFloatingObjectLayoutModel = GetViewportFloatingObjectLayoutModel(rowViewport, columnViewport);
            Point point = new Point(mousePosition.X - _floatingObjectsMovingResizingStartPoint.X, mousePosition.Y - _floatingObjectsMovingResizingStartPoint.Y);
            foreach (FloatingObject obj2 in _movingResizingFloatingObjects)
            {
                FloatingObjectLayout layout = cacheFloatingObjectsMovingResizingLayoutModels.Find(obj2.Name);
                FloatingObjectLayout layout2 = viewportFloatingObjectLayoutModel.Find(obj2.Name);
                Point point2 = new Point(layout.X, layout.Y);
                Point point3 = new Point(point2.X + point.X, point2.Y + point.Y);
                Point point4 = new Point(layout2.X + layout2.Width, layout2.Y + layout2.Height);
                double y = Math.Min(point3.Y, point4.Y);
                double height = Math.Abs((double)(point4.Y - point3.Y));
                double width = layout2.Width;
                Rect rect = new Rect(layout2.X, y, width, height);
                list.Add(rect);
            }
            return list.ToArray();
        }

        Rect[] GetFloatingObjectsTopleftResizingRects(int rowViewport, int columnViewport, Point mousePosition)
        {
            List<Rect> list = new List<Rect>();
            FloatingObjectLayoutModel cacheFloatingObjectsMovingResizingLayoutModels = GetCacheFloatingObjectsMovingResizingLayoutModels(rowViewport, columnViewport);
            FloatingObjectLayoutModel viewportFloatingObjectLayoutModel = GetViewportFloatingObjectLayoutModel(rowViewport, columnViewport);
            Point point = new Point(mousePosition.X - _floatingObjectsMovingResizingStartPoint.X, mousePosition.Y - _floatingObjectsMovingResizingStartPoint.Y);
            foreach (FloatingObject obj2 in _movingResizingFloatingObjects)
            {
                FloatingObjectLayout layout = cacheFloatingObjectsMovingResizingLayoutModels.Find(obj2.Name);
                FloatingObjectLayout layout2 = viewportFloatingObjectLayoutModel.Find(obj2.Name);
                Point point2 = new Point(layout.X, layout.Y);
                Point point3 = new Point(point2.X + point.X, point2.Y + point.Y);
                Point point4 = new Point(layout2.X + layout2.Width, layout2.Y + layout2.Height);
                double x = Math.Min(point3.X, point4.X);
                double y = Math.Min(point3.Y, point4.Y);
                double width = Math.Abs((double)(point4.X - point3.X));
                double height = Math.Abs((double)(point4.Y - point3.Y));
                Rect rect = new Rect(x, y, width, height);
                list.Add(rect);
            }
            return list.ToArray();
        }

        Rect[] GetFloatingObjectsTopRightResizingRects(int rowViewport, int columnViewport, Point mousePosition)
        {
            List<Rect> list = new List<Rect>();
            FloatingObjectLayoutModel cacheFloatingObjectsMovingResizingLayoutModels = GetCacheFloatingObjectsMovingResizingLayoutModels(rowViewport, columnViewport);
            FloatingObjectLayoutModel viewportFloatingObjectLayoutModel = GetViewportFloatingObjectLayoutModel(rowViewport, columnViewport);
            Point point = new Point(mousePosition.X - _floatingObjectsMovingResizingStartPoint.X, mousePosition.Y - _floatingObjectsMovingResizingStartPoint.Y);
            foreach (FloatingObject obj2 in _movingResizingFloatingObjects)
            {
                FloatingObjectLayout layout = cacheFloatingObjectsMovingResizingLayoutModels.Find(obj2.Name);
                FloatingObjectLayout layout2 = viewportFloatingObjectLayoutModel.Find(obj2.Name);
                Point point2 = new Point(layout.X + layout.Width, layout.Y);
                Point point3 = new Point(point2.X + point.X, point2.Y + point.Y);
                Point point4 = new Point(layout2.X, layout2.Y + layout2.Height);
                double x = Math.Min(point3.X, point4.X);
                double y = Math.Min(point3.Y, point4.Y);
                double width = Math.Abs((double)(point4.X - point3.X));
                double height = Math.Abs((double)(point4.Y - point3.Y));
                Rect rect = new Rect(x, y, width, height);
                list.Add(rect);
            }
            return list.ToArray();
        }

        /// <summary>
        /// Gets the index of the floating object Z.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public int GetFloatingObjectZIndex(string name)
        {
            int activeRowViewportIndex = GetActiveRowViewportIndex();
            int activeColumnViewportIndex = GetActiveColumnViewportIndex();
            CellsPanel viewportRowsPresenter = GetViewportRowsPresenter(activeRowViewportIndex, activeColumnViewportIndex);
            if (viewportRowsPresenter != null)
            {
                return viewportRowsPresenter.GetFlotingObjectZIndex(name);
            }
            return -1;
        }
    }
}

