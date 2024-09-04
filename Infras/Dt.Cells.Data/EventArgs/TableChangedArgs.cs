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
    internal class TableChangedArgs : EventArgs
    {
        string propertyName;
        SheetTable sheetTable;

        public TableChangedArgs(SheetTable table, string property)
        {
            this.sheetTable = table;
            this.propertyName = property;
        }

        public string PropertyName
        {
            get { return  this.propertyName; }
        }

        public SheetTable Table
        {
            get { return  this.sheetTable; }
        }
    }
}

