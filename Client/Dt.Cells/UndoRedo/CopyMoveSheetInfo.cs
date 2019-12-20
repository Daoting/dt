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
        private double _columnHeaderDefaultRowHeight;
        private bool _columnHeaderDefaultRowHeightSaved;
        private StyleInfo _columnHeaderDefaultStyle;
        private bool _columnHeaderDefaultStyleSaved;
        private double _defaultColumnWidth;
        private bool _defaultColumnWidthSaved;
        private double _defaultRowHeight;
        private bool _defaultRowHeightSaved;
        private StyleInfo _defaultStyle;
        private bool _defaultStyleSaved;
        private double _rowHeaderDefaultColumnWidth;
        private bool _rowHeaderDefaultColumnWidthSaved;
        private StyleInfo _rowHeaderDefaultStyle;
        private bool _rowHeaderDefaultStyleSaved;

        public double GetColumnHeaderDefaultRowHeight()
        {
            return this._columnHeaderDefaultRowHeight;
        }

        public StyleInfo GetColumnHeaderDefaultStyle()
        {
            return this._columnHeaderDefaultStyle;
        }

        public double GetDefaultColumnWidth()
        {
            return this._defaultColumnWidth;
        }

        public double GetDefaultRowHeight()
        {
            return this._defaultRowHeight;
        }

        public StyleInfo GetDefaultStyle()
        {
            return this._defaultStyle;
        }

        public double GetRowHeaderDefaultColumnWidth()
        {
            return this._rowHeaderDefaultColumnWidth;
        }

        public StyleInfo GetRowHeaderDefaultStyle()
        {
            return this._rowHeaderDefaultStyle;
        }

        public bool IsColumnHeaderDefaultRowHeightSaved()
        {
            return this._columnHeaderDefaultRowHeightSaved;
        }

        public bool IsColumnHeaderDefaultStyleSaved()
        {
            return this._columnHeaderDefaultStyleSaved;
        }

        public bool IsDefaultColumnWidthSaved()
        {
            return this._defaultColumnWidthSaved;
        }

        public bool IsDefaultRowHeightSaved()
        {
            return this._defaultRowHeightSaved;
        }

        public bool IsDefaultStyleSaved()
        {
            return this._defaultStyleSaved;
        }

        public bool IsRowHeaderDefaultColumnWidthSaved()
        {
            return this._rowHeaderDefaultColumnWidthSaved;
        }

        public bool IsRowHeaderDefaultStyleSaved()
        {
            return this._rowHeaderDefaultStyleSaved;
        }

        public void SaveColumnHeaderDefaultRowHeight(double height)
        {
            this._columnHeaderDefaultRowHeight = height;
            this._columnHeaderDefaultRowHeightSaved = true;
        }

        public void SaveColumnHeaderDefaultStyle(StyleInfo style)
        {
            this._columnHeaderDefaultStyle = style;
            this._columnHeaderDefaultStyleSaved = true;
        }

        public void SaveDefaultColumnWidth(double width)
        {
            this._defaultColumnWidth = width;
            this._defaultColumnWidthSaved = true;
        }

        public void SaveDefaultRowHeight(double height)
        {
            this._defaultRowHeight = height;
            this._defaultRowHeightSaved = true;
        }

        public void SaveDefaultStyle(StyleInfo style)
        {
            this._defaultStyle = style;
            this._defaultStyleSaved = true;
        }

        public void SaveRowHeaderDefaultColumnWidth(double width)
        {
            this._rowHeaderDefaultColumnWidth = width;
            this._rowHeaderDefaultColumnWidthSaved = true;
        }

        public void SaveRowHeaderDefaultStyle(StyleInfo style)
        {
            this._rowHeaderDefaultStyle = style;
            this._rowHeaderDefaultStyleSaved = true;
        }
    }
}

