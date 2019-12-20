#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Cells.UndoRedo
{
    internal class CopyMoveColumnsInfo
    {
        private Dictionary<int, string> _bindingFields = new Dictionary<int, string>();
        private bool _bindingFieldSaved;
        private Dictionary<int, bool> _collapsed = new Dictionary<int, bool>();
        private int _columnCount;
        private Dictionary<int, object> _headerColumnStyles = new Dictionary<int, object>();
        private bool _headerColumnStyleSaved;
        private Dictionary<int, int> _levels = new Dictionary<int, int>();
        private bool _rangeGroupSaved;
        private Dictionary<int, bool> _resizables = new Dictionary<int, bool>();
        private bool _resizableSaved;
        private Dictionary<int, object> _tags = new Dictionary<int, object>();
        private bool _tagSaved;
        private Dictionary<int, object> _viewportColumnStyles = new Dictionary<int, object>();
        private bool _viewportColumnStyleSaved;
        private Dictionary<int, bool> _visibles = new Dictionary<int, bool>();
        private bool _visibleSaved;
        private Dictionary<int, double> _widths = new Dictionary<int, double>();
        private bool _widthSaved;

        public CopyMoveColumnsInfo(int columnCount)
        {
            this._columnCount = columnCount;
        }

        public bool GetBindingField(int index, out string fieldName)
        {
            fieldName = null;
            if (this._bindingFields.ContainsKey(index))
            {
                fieldName = this._bindingFields[index];
                return true;
            }
            return false;
        }

        public object GetHeaderColumnStyle(int index)
        {
            if (this._headerColumnStyles.ContainsKey(index))
            {
                return this._headerColumnStyles[index];
            }
            return null;
        }

        public void GetRangeGroup(int index, out int level, out bool collapsed)
        {
            level = -1;
            collapsed = false;
            if (this._levels.ContainsKey(index))
            {
                level = this._levels[index];
            }
            if (this._collapsed.ContainsKey(index))
            {
                collapsed = this._collapsed[index];
            }
        }

        public bool GetResizable(int index)
        {
            if (this._resizables.ContainsKey(index))
            {
                return this._resizables[index];
            }
            return true;
        }

        public object GetTag(int index)
        {
            if (this._tags.ContainsKey(index))
            {
                return this._tags[index];
            }
            return null;
        }

        public object GetViewportColumnStyle(int index)
        {
            if (this._viewportColumnStyles.ContainsKey(index))
            {
                return this._viewportColumnStyles[index];
            }
            return null;
        }

        public bool GetVisible(int index)
        {
            if (this._visibles.ContainsKey(index))
            {
                return this._visibles[index];
            }
            return true;
        }

        public double GetWidth(int index)
        {
            if (this._widths.ContainsKey(index))
            {
                return this._widths[index];
            }
            return -1.0;
        }

        public bool IsBindingFieldSaved()
        {
            return this._bindingFieldSaved;
        }

        public bool IsHeaderColumnStyleSaved()
        {
            return this._headerColumnStyleSaved;
        }

        public bool IsRangeGroupSaved()
        {
            return this._rangeGroupSaved;
        }

        public bool IsResizableSaved()
        {
            return this._resizableSaved;
        }

        public bool IsTagSaved()
        {
            return this._tagSaved;
        }

        public bool IsViewportColumnStyleSaved()
        {
            return this._viewportColumnStyleSaved;
        }

        public bool IsVisibleSaved()
        {
            return this._visibleSaved;
        }

        public bool IsWidthSaved()
        {
            return this._widthSaved;
        }

        public void SaveBindingField(int index, string fieldName)
        {
            if (this._bindingFields.ContainsKey(index))
            {
                this._bindingFields[index] = fieldName;
            }
            else
            {
                this._bindingFields.Add(index, fieldName);
            }
            this._bindingFieldSaved = true;
        }

        public void SaveHeaderColumnStyle(int index, object style)
        {
            if (this._headerColumnStyles.ContainsKey(index))
            {
                if (style == null)
                {
                    this._headerColumnStyles.Remove(index);
                }
                else
                {
                    this._headerColumnStyles[index] = style;
                }
            }
            else if (style != null)
            {
                this._headerColumnStyles.Add(index, style);
            }
            this._headerColumnStyleSaved = true;
        }

        public void SaveRangeGroup(int index, int level, bool collapsed)
        {
            if (this._levels.ContainsKey(index))
            {
                if (level < 0)
                {
                    this._levels.Remove(index);
                }
                else
                {
                    this._levels[index] = level;
                }
            }
            else if (level >= 0)
            {
                this._levels.Add(index, level);
            }
            if (this._collapsed.ContainsKey(index))
            {
                if (collapsed)
                {
                    this._collapsed[index] = collapsed;
                }
                else
                {
                    this._collapsed.Remove(index);
                }
            }
            else if (collapsed)
            {
                this._collapsed.Add(index, collapsed);
            }
            this._rangeGroupSaved = true;
        }

        public void SaveResizable(int index, bool resizable)
        {
            if (this._resizables.ContainsKey(index))
            {
                if (resizable)
                {
                    this._resizables.Remove(index);
                }
                else
                {
                    this._resizables[index] = resizable;
                }
            }
            else if (!resizable)
            {
                this._resizables.Add(index, resizable);
            }
            this._resizableSaved = true;
        }

        public void SaveTag(int index, object tag)
        {
            if (this._tags.ContainsKey(index))
            {
                if (tag == null)
                {
                    this._tags.Remove(index);
                }
                else
                {
                    this._tags[index] = tag;
                }
            }
            else if (tag != null)
            {
                this._tags.Add(index, tag);
            }
            this._tagSaved = true;
        }

        public void SaveViewportColumnStyle(int index, object style)
        {
            if (this._viewportColumnStyles.ContainsKey(index))
            {
                if (style == null)
                {
                    this._viewportColumnStyles.Remove(index);
                }
                else
                {
                    this._viewportColumnStyles[index] = style;
                }
            }
            else if (style != null)
            {
                this._viewportColumnStyles.Add(index, style);
            }
            this._viewportColumnStyleSaved = true;
        }

        public void SaveVisible(int index, bool visible)
        {
            if (this._visibles.ContainsKey(index))
            {
                if (visible)
                {
                    this._visibles.Remove(index);
                }
                else
                {
                    this._visibles[index] = visible;
                }
            }
            else if (!visible)
            {
                this._visibles.Add(index, visible);
            }
            this._visibleSaved = true;
        }

        public void SaveWidth(int index, double width)
        {
            if (this._widths.ContainsKey(index))
            {
                if (width < 0.0)
                {
                    this._widths.Remove(index);
                }
                else
                {
                    this._widths[index] = width;
                }
            }
            else if (width >= 0.0)
            {
                this._widths.Add(index, width);
            }
            this._widthSaved = true;
        }

        public int ColumnCount
        {
            get { return  this._columnCount; }
        }
    }
}

