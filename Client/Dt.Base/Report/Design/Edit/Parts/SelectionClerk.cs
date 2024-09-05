#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-06-21 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Cells.Data;
using Dt.Cells.UI;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Windows.Foundation;
using Windows.UI;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Input;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 报表项选择状态管理
    /// </summary>
    internal class SelectionClerk
    {
        RptDesignHome _owner;
        RptItem _drgItem;
        BlankAreaMenu _menuBlank;
        HeaderFooterMenu _menuHeader;
        RptItemContextMenu _menuItemContext;
        
        public SelectionClerk(RptDesignHome p_owner)
        {
            _owner = p_owner;
            Excel excel = _owner.Excel;
            excel.SelectionChanged += (s, e) => UpdateSelection();
            excel.ItemStartDrag += OnItemStartDrag;
            excel.ItemDropped += OnItemDropped;
            excel.RightTapped += OnRightTapped;
            // 左键点击图片时选择所在区域
            excel.PointerPressed += OnPointerPressed;
        }

        /// <summary>
        /// 更新选择区域
        /// </summary>
        public void UpdateSelection()
        {
            CellRange range;
            Excel excel = _owner.Excel;
            Worksheet sheet = excel.ActiveSheet;
            excel.DecorationRange = null;

            // 无选择及选择整行整列 : 返回。无选择可能是选中浮动对象造成的，现不在此处处理。
            if (sheet.Selections.Count == 0
                || (range = sheet.Selections[0]).Row == -1
                || range.Column == -1)
            {
                _owner.ClearForms();
                return;
            }

            // 三种情况，在已有对象内部、相交、空白区域
            RptItem curItem = null;
            RptPart container = _owner.GetContainer();
            foreach (RptItem item in container.Items)
            {
                // 选择区域在报表项内
                if (item.Contains(range))
                {
                    curItem = item;
                    break;
                }
                // 相交，无操作，返回
                if (range.Intersects(item.Row, item.Col, item.RowSpan, item.ColSpan))
                {
                    _owner.ClearForms();
                    return;
                }
            }

            if (curItem != null)
            {
                // 在对象内部
                excel.DecorationRange = new CellRange(curItem.Row, curItem.Col, curItem.RowSpan, curItem.ColSpan);
                _owner.LoadForms(curItem, range);
            }
        }

        /// <summary>
        /// 显示右键菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            CellRange range;
            Excel excel = _owner.Excel;
            Worksheet sheet = excel.ActiveSheet;

            // 无选择及选择整行整列 : 返回。无选择可能是选中浮动对象造成的，现不在此处处理。
            if (sheet.Selections.Count == 0
                || (range = sheet.Selections[0]).Row == -1
                || range.Column == -1)
            {
                return;
            }

            // 三种情况，在已有对象内部、相交、空白区域
            List<RptItem> items = new List<RptItem>();
            RptPart container = _owner.GetContainer();
            foreach (RptItem item in container.Items)
            {
                // 在报表项内 相交，无操作，返回
                if (item.Contains(range)
                    || range.Intersects(item.Row, item.Col, item.RowSpan, item.ColSpan))
                {
                    items.Add(item);
                }
            }

            if (items.Count == 0)
            {
                // 在空白区域
                if (container.PartType == RptPartType.Body)
                {
                    if (_menuBlank == null)
                        _menuBlank = new BlankAreaMenu(_owner);
                    ShowMenu(_menuBlank, range);
                }
                else
                {
                    // 页眉页脚菜单
                    if (_menuHeader == null)
                        _menuHeader = new HeaderFooterMenu(_owner);
                    ShowMenu(_menuHeader, range);
                }
                _owner.ClearForms();
            }
            else
            {
                // 报表项上下文菜单
                if (_menuItemContext == null)
                    _menuItemContext = new RptItemContextMenu(_owner);
                _menuItemContext.TargetItems = items;
                ShowMenu(_menuItemContext, range);
            }
        }

        /// <summary>
        /// 显示上下文菜单
        /// </summary>
        /// <param name="p_menu"></param>
        /// <param name="p_range"></param>
        void ShowMenu(Menu p_menu, CellRange p_range)
        {
            Excel excel = _owner.Excel;
            Point topLeft = excel.GetAbsolutePosition();
            Rect rc = excel.ActiveSheet.GetRangeBound(p_range);
            Point pos = new Point(topLeft.X + rc.X + rc.Width + 5, topLeft.Y + rc.Y);
            _ = p_menu.OpenContextMenu(pos);
        }

        /// <summary>
        /// 开始拖拽
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnItemStartDrag(object sender, EventArgs e)
        {
            CellRange dragRng = _owner.Excel.DecorationRange;
            if (dragRng == null)
            {
                _drgItem = null;
                return;
            }

            foreach (RptItem item in _owner.GetContainer().Items)
            {
                if (item.Row == dragRng.Row
                    && item.Col == dragRng.Column
                    && item.RowSpan == dragRng.RowCount
                    && item.ColSpan == dragRng.ColumnCount)
                {
                    _drgItem = item;
                    break;
                }
            }
            if (_drgItem == null)
            {
                _owner.Excel.DecorationRange = null;
            }
        }

        /// <summary>
        /// 拖放结束
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnItemDropped(object sender, CellEventArgs e)
        {
            if (_drgItem == null)
            {
                _owner.Excel.DecorationRange = null;
                return;
            }

            if (_drgItem.TestMovIntersect(e.Row, e.Column))
            {
                Kit.Warn("此对象移动后的位置后会与其他对象重叠，请先移动可能重叠的对象后重试。");
                _drgItem = null;
                return;
            }

            MoveRptItemArgs moveAgs = new MoveRptItemArgs(_drgItem, e);
            _owner.Info.ExecuteCmd(RptCmds.MoveRptItemCmd, moveAgs);
            _drgItem = null;
        }

        /// <summary>
        /// 左键点击图片时选择所在区域
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var curPoint = e.GetCurrentPoint(_owner.Excel);
            if (!curPoint.Properties.IsLeftButtonPressed)
                return;

            var pos = curPoint.Position;
            var hi = _owner.Excel.HitTest(pos.X, pos.Y);
            if (hi.HitTestType == HitTestType.FloatingObject
                && hi.FloatingObjectInfo != null
                && hi.FloatingObjectInfo.FloatingObject is Picture pic)
            {
                _owner.Excel.SetSelection(pic.StartRow, pic.StartColumn, pic.EndRow - pic.StartRow, pic.EndColumn - pic.StartColumn);
                UpdateSelection();
            }
        }
    }
}
