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
#endregion

namespace Dt.Cells.UndoRedo
{
    internal class CopyMoveSheetInfo
    {
        double _columnHeaderDefaultRowHeight;
        bool _columnHeaderDefaultRowHeightSaved;
        StyleInfo _columnHeaderDefaultStyle;
        bool _columnHeaderDefaultStyleSaved;
        double _defaultColumnWidth;
        bool _defaultColumnWidthSaved;
        double _defaultRowHeight;
        bool _defaultRowHeightSaved;
        StyleInfo _defaultStyle;
        bool _defaultStyleSaved;
        double _rowHeaderDefaultColumnWidth;
        bool _rowHeaderDefaultColumnWidthSaved;
        StyleInfo _rowHeaderDefaultStyle;
        bool _rowHeaderDefaultStyleSaved;

        public double GetColumnHeaderDefaultRowHeight()
        {
            return _columnHeaderDefaultRowHeight;
        }

        public StyleInfo GetColumnHeaderDefaultStyle()
        {
            return _columnHeaderDefaultStyle;
        }

        public double GetDefaultColumnWidth()
        {
            return _defaultColumnWidth;
        }

        public double GetDefaultRowHeight()
        {
            return _defaultRowHeight;
        }

        public StyleInfo GetDefaultStyle()
        {
            return _defaultStyle;
        }

        public double GetRowHeaderDefaultColumnWidth()
        {
            return _rowHeaderDefaultColumnWidth;
        }

        public StyleInfo GetRowHeaderDefaultStyle()
        {
            return _rowHeaderDefaultStyle;
        }

        public bool IsColumnHeaderDefaultRowHeightSaved()
        {
            return _columnHeaderDefaultRowHeightSaved;
        }

        public bool IsColumnHeaderDefaultStyleSaved()
        {
            return _columnHeaderDefaultStyleSaved;
        }

        public bool IsDefaultColumnWidthSaved()
        {
            return _defaultColumnWidthSaved;
        }

        public bool IsDefaultRowHeightSaved()
        {
            return _defaultRowHeightSaved;
        }

        public bool IsDefaultStyleSaved()
        {
            return _defaultStyleSaved;
        }

        public bool IsRowHeaderDefaultColumnWidthSaved()
        {
            return _rowHeaderDefaultColumnWidthSaved;
        }

        public bool IsRowHeaderDefaultStyleSaved()
        {
            return _rowHeaderDefaultStyleSaved;
        }

        public void SaveColumnHeaderDefaultRowHeight(double height)
        {
            _columnHeaderDefaultRowHeight = height;
            _columnHeaderDefaultRowHeightSaved = true;
        }

        public void SaveColumnHeaderDefaultStyle(StyleInfo style)
        {
            _columnHeaderDefaultStyle = style;
            _columnHeaderDefaultStyleSaved = true;
        }

        public void SaveDefaultColumnWidth(double width)
        {
            _defaultColumnWidth = width;
            _defaultColumnWidthSaved = true;
        }

        public void SaveDefaultRowHeight(double height)
        {
            _defaultRowHeight = height;
            _defaultRowHeightSaved = true;
        }

        public void SaveDefaultStyle(StyleInfo style)
        {
            _defaultStyle = style;
            _defaultStyleSaved = true;
        }

        public void SaveRowHeaderDefaultColumnWidth(double width)
        {
            _rowHeaderDefaultColumnWidth = width;
            _rowHeaderDefaultColumnWidthSaved = true;
        }

        public void SaveRowHeaderDefaultStyle(StyleInfo style)
        {
            _rowHeaderDefaultStyle = style;
            _rowHeaderDefaultStyleSaved = true;
        }
    }
}

