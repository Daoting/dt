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
    /// <summary>
    /// Represents an interface for support data binding, it can be use to sheet implementation.
    /// </summary>
    internal interface IBindableSheet
    {
        void GenerateColumns(string[] fields);

        int ColumnCount { get; }

        int RowCount { get; set; }
    }
}

