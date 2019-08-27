#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Sqlite;
using System;
#endregion

namespace Dt.Base.FormView
{
    /// <summary>
    /// 记录Fv单元格最后编辑的值
    /// </summary>
    [StateTable]
    public class CellLastVal
    {
        /// <summary>
        /// 单元格唯一标识：BaseUri + Fv.Name + FvCell.ID
        /// </summary>
        [PrimaryKey]
        public string ID { get; set; }

        /// <summary>
        /// 单元格最后编辑的值
        /// </summary>
        public string Val { get; set; }
    }
}
