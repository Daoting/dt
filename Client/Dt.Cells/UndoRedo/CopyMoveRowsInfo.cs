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
        Dictionary<int, bool> _collapsed = new Dictionary<int, bool>();
        Dictionary<int, object> _headerRowStyles = new Dictionary<int, object>();
        bool _headerRowStyleSaved;
        Dictionary<int, double> _heights = new Dictionary<int, double>();
        bool _heightSaved;
        Dictionary<int, int> _levels = new Dictionary<int, int>();
        bool _rangeGroupSaved;
        Dictionary<int, bool> _resizables = new Dictionary<int, bool>();
        bool _resizableSaved;
        int _rowCount;
        Dictionary<int, object> _tags = new Dictionary<int, object>();
        bool _tagSaved;
        Dictionary<int, object> _viewportRowStyles = new Dictionary<int, object>();
        bool _viewportRowStyleSaved;
        Dictionary<int, bool> _visibles = new Dictionary<int, bool>();
        bool _visibleSaved;

        public CopyMoveRowsInfo(int rowCount)
        {
            _rowCount = rowCount;
        }

        public object GetHeaderRowStyle(int index)
        {
            if (_headerRowStyles.ContainsKey(index))
            {
                return _headerRowStyles[index];
            }
            return null;
        }

        public double GetHeight(int index)
        {
            if (_heights.ContainsKey(index))
            {
                return _heights[index];
            }
            return -1.0;
        }

        public void GetRangeGroup(int index, out int level, out bool collapsed)
        {
            level = -1;
            collapsed = false;
            if (_levels.ContainsKey(index))
            {
                level = _levels[index];
            }
            if (_collapsed.ContainsKey(index))
            {
                collapsed = _collapsed[index];
            }
        }

        public bool GetResizable(int index)
        {
            if (_resizables.ContainsKey(index))
            {
                return _resizables[index];
            }
            return true;
        }

        public object GetTag(int index)
        {
            if (_tags.ContainsKey(index))
            {
                return _tags[index];
            }
            return null;
        }

        public object GetViewportRowStyle(int index)
        {
            if (_viewportRowStyles.ContainsKey(index))
            {
                return _viewportRowStyles[index];
            }
            return null;
        }

        public bool GetVisible(int index)
        {
            if (_visibles.ContainsKey(index))
            {
                return _visibles[index];
            }
            return true;
        }

        public bool IsHeaderRowStyleSaved()
        {
            return _headerRowStyleSaved;
        }

        public bool IsHeightSaved()
        {
            return _heightSaved;
        }

        public bool IsRangeGroupSaved()
        {
            return _rangeGroupSaved;
        }

        public bool IsResizableSaved()
        {
            return _resizableSaved;
        }

        public bool IsTagSaved()
        {
            return _tagSaved;
        }

        public bool IsViewportRowStyleSaved()
        {
            return _viewportRowStyleSaved;
        }

        public bool IsVisibleSaved()
        {
            return _visibleSaved;
        }

        public void SaveHeaderRowStyle(int index, object style)
        {
            if (_headerRowStyles.ContainsKey(index))
            {
                if (style == null)
                {
                    _headerRowStyles.Remove(index);
                }
                else
                {
                    _headerRowStyles[index] = style;
                }
            }
            else if (style != null)
            {
                _headerRowStyles.Add(index, style);
            }
            _headerRowStyleSaved = true;
        }

        public void SaveHeight(int index, double height)
        {
            if (_heights.ContainsKey(index))
            {
                if (height < 0.0)
                {
                    _heights.Remove(index);
                }
                else
                {
                    _heights[index] = height;
                }
            }
            else if (height >= 0.0)
            {
                _heights.Add(index, height);
            }
            _heightSaved = true;
        }

        public void SaveRangeGroup(int index, int level, bool collapsed)
        {
            if (_levels.ContainsKey(index))
            {
                if (level < 0)
                {
                    _levels.Remove(index);
                }
                else
                {
                    _levels[index] = level;
                }
            }
            else if (level >= 0)
            {
                _levels.Add(index, level);
            }
            if (_collapsed.ContainsKey(index))
            {
                if (collapsed)
                {
                    _collapsed[index] = collapsed;
                }
                else
                {
                    _collapsed.Remove(index);
                }
            }
            else if (collapsed)
            {
                _collapsed.Add(index, collapsed);
            }
            _rangeGroupSaved = true;
        }

        public void SaveResizable(int index, bool resizable)
        {
            if (_resizables.ContainsKey(index))
            {
                if (resizable)
                {
                    _resizables.Remove(index);
                }
                else
                {
                    _resizables[index] = resizable;
                }
            }
            else if (!resizable)
            {
                _resizables.Add(index, resizable);
            }
            _resizableSaved = true;
        }

        public void SaveTag(int index, object tag)
        {
            if (_tags.ContainsKey(index))
            {
                if (tag == null)
                {
                    _tags.Remove(index);
                }
                else
                {
                    _tags[index] = tag;
                }
            }
            else if (tag != null)
            {
                _tags.Add(index, tag);
            }
            _tagSaved = true;
        }

        public void SaveViewportRowStyle(int index, object style)
        {
            if (_viewportRowStyles.ContainsKey(index))
            {
                if (style == null)
                {
                    _viewportRowStyles.Remove(index);
                }
                else
                {
                    _viewportRowStyles[index] = style;
                }
            }
            else if (style != null)
            {
                _viewportRowStyles.Add(index, style);
            }
            _viewportRowStyleSaved = true;
        }

        public void SaveVisible(int index, bool visible)
        {
            if (_visibles.ContainsKey(index))
            {
                if (visible)
                {
                    _visibles.Remove(index);
                }
                else
                {
                    _visibles[index] = visible;
                }
            }
            else if (!visible)
            {
                _visibles.Add(index, visible);
            }
            _visibleSaved = true;
        }

        public int RowCount
        {
            get { return  _rowCount; }
        }
    }
}

