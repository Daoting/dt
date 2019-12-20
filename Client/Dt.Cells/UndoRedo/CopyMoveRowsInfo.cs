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
    internal class CopyMoveRowsInfo
    {
        private Dictionary<int, bool> _collapsed = new Dictionary<int, bool>();
        private Dictionary<int, object> _headerRowStyles = new Dictionary<int, object>();
        private bool _headerRowStyleSaved;
        private Dictionary<int, double> _heights = new Dictionary<int, double>();
        private bool _heightSaved;
        private Dictionary<int, int> _levels = new Dictionary<int, int>();
        private bool _rangeGroupSaved;
        private Dictionary<int, bool> _resizables = new Dictionary<int, bool>();
        private bool _resizableSaved;
        private int _rowCount;
        private Dictionary<int, object> _tags = new Dictionary<int, object>();
        private bool _tagSaved;
        private Dictionary<int, object> _viewportRowStyles = new Dictionary<int, object>();
        private bool _viewportRowStyleSaved;
        private Dictionary<int, bool> _visibles = new Dictionary<int, bool>();
        private bool _visibleSaved;

        public CopyMoveRowsInfo(int rowCount)
        {
            this._rowCount = rowCount;
        }

        public object GetHeaderRowStyle(int index)
        {
            if (this._headerRowStyles.ContainsKey(index))
            {
                return this._headerRowStyles[index];
            }
            return null;
        }

        public double GetHeight(int index)
        {
            if (this._heights.ContainsKey(index))
            {
                return this._heights[index];
            }
            return -1.0;
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

        public object GetViewportRowStyle(int index)
        {
            if (this._viewportRowStyles.ContainsKey(index))
            {
                return this._viewportRowStyles[index];
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

        public bool IsHeaderRowStyleSaved()
        {
            return this._headerRowStyleSaved;
        }

        public bool IsHeightSaved()
        {
            return this._heightSaved;
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

        public bool IsViewportRowStyleSaved()
        {
            return this._viewportRowStyleSaved;
        }

        public bool IsVisibleSaved()
        {
            return this._visibleSaved;
        }

        public void SaveHeaderRowStyle(int index, object style)
        {
            if (this._headerRowStyles.ContainsKey(index))
            {
                if (style == null)
                {
                    this._headerRowStyles.Remove(index);
                }
                else
                {
                    this._headerRowStyles[index] = style;
                }
            }
            else if (style != null)
            {
                this._headerRowStyles.Add(index, style);
            }
            this._headerRowStyleSaved = true;
        }

        public void SaveHeight(int index, double height)
        {
            if (this._heights.ContainsKey(index))
            {
                if (height < 0.0)
                {
                    this._heights.Remove(index);
                }
                else
                {
                    this._heights[index] = height;
                }
            }
            else if (height >= 0.0)
            {
                this._heights.Add(index, height);
            }
            this._heightSaved = true;
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

        public void SaveViewportRowStyle(int index, object style)
        {
            if (this._viewportRowStyles.ContainsKey(index))
            {
                if (style == null)
                {
                    this._viewportRowStyles.Remove(index);
                }
                else
                {
                    this._viewportRowStyles[index] = style;
                }
            }
            else if (style != null)
            {
                this._viewportRowStyles.Add(index, style);
            }
            this._viewportRowStyleSaved = true;
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

        public int RowCount
        {
            get { return  this._rowCount; }
        }
    }
}

