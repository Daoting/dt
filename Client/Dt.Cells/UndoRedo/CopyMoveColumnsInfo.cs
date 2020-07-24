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
        Dictionary<int, string> _bindingFields = new Dictionary<int, string>();
        bool _bindingFieldSaved;
        Dictionary<int, bool> _collapsed = new Dictionary<int, bool>();
        int _columnCount;
        Dictionary<int, object> _headerColumnStyles = new Dictionary<int, object>();
        bool _headerColumnStyleSaved;
        Dictionary<int, int> _levels = new Dictionary<int, int>();
        bool _rangeGroupSaved;
        Dictionary<int, bool> _resizables = new Dictionary<int, bool>();
        bool _resizableSaved;
        Dictionary<int, object> _tags = new Dictionary<int, object>();
        bool _tagSaved;
        Dictionary<int, object> _viewportColumnStyles = new Dictionary<int, object>();
        bool _viewportColumnStyleSaved;
        Dictionary<int, bool> _visibles = new Dictionary<int, bool>();
        bool _visibleSaved;
        Dictionary<int, double> _widths = new Dictionary<int, double>();
        bool _widthSaved;

        public CopyMoveColumnsInfo(int columnCount)
        {
            _columnCount = columnCount;
        }

        public bool GetBindingField(int index, out string fieldName)
        {
            fieldName = null;
            if (_bindingFields.ContainsKey(index))
            {
                fieldName = _bindingFields[index];
                return true;
            }
            return false;
        }

        public object GetHeaderColumnStyle(int index)
        {
            if (_headerColumnStyles.ContainsKey(index))
            {
                return _headerColumnStyles[index];
            }
            return null;
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

        public object GetViewportColumnStyle(int index)
        {
            if (_viewportColumnStyles.ContainsKey(index))
            {
                return _viewportColumnStyles[index];
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

        public double GetWidth(int index)
        {
            if (_widths.ContainsKey(index))
            {
                return _widths[index];
            }
            return -1.0;
        }

        public bool IsBindingFieldSaved()
        {
            return _bindingFieldSaved;
        }

        public bool IsHeaderColumnStyleSaved()
        {
            return _headerColumnStyleSaved;
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

        public bool IsViewportColumnStyleSaved()
        {
            return _viewportColumnStyleSaved;
        }

        public bool IsVisibleSaved()
        {
            return _visibleSaved;
        }

        public bool IsWidthSaved()
        {
            return _widthSaved;
        }

        public void SaveBindingField(int index, string fieldName)
        {
            if (_bindingFields.ContainsKey(index))
            {
                _bindingFields[index] = fieldName;
            }
            else
            {
                _bindingFields.Add(index, fieldName);
            }
            _bindingFieldSaved = true;
        }

        public void SaveHeaderColumnStyle(int index, object style)
        {
            if (_headerColumnStyles.ContainsKey(index))
            {
                if (style == null)
                {
                    _headerColumnStyles.Remove(index);
                }
                else
                {
                    _headerColumnStyles[index] = style;
                }
            }
            else if (style != null)
            {
                _headerColumnStyles.Add(index, style);
            }
            _headerColumnStyleSaved = true;
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

        public void SaveViewportColumnStyle(int index, object style)
        {
            if (_viewportColumnStyles.ContainsKey(index))
            {
                if (style == null)
                {
                    _viewportColumnStyles.Remove(index);
                }
                else
                {
                    _viewportColumnStyles[index] = style;
                }
            }
            else if (style != null)
            {
                _viewportColumnStyles.Add(index, style);
            }
            _viewportColumnStyleSaved = true;
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

        public void SaveWidth(int index, double width)
        {
            if (_widths.ContainsKey(index))
            {
                if (width < 0.0)
                {
                    _widths.Remove(index);
                }
                else
                {
                    _widths[index] = width;
                }
            }
            else if (width >= 0.0)
            {
                _widths.Add(index, width);
            }
            _widthSaved = true;
        }

        public int ColumnCount
        {
            get { return  _columnCount; }
        }
    }
}

