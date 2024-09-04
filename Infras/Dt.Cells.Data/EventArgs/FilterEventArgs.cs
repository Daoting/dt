#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.Data
{
    internal class FilterEventArgs : EventArgs
    {
        FilterAction _filterAction;

        public FilterEventArgs(FilterAction filterAction)
        {
            this._filterAction = filterAction;
        }

        public FilterAction Action
        {
            get { return  this._filterAction; }
        }
    }
}

