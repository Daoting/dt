#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-12 创建
******************************************************************************/
#endregion

#region 引用命名
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 服务端数据表部分
    /// </summary>
    public partial class Table
    {
        /// <summary>
        /// 根据表名创建空Table
        /// </summary>
        /// <param name="p_tblName">表名</param>
        /// <returns></returns>
        public static Table Create(string p_tblName)
        {
            if (string.IsNullOrEmpty(p_tblName))
                throw new Exception("根据表名创建空Table时表名不可为空！");

            Table tbl = new Table();
            var schema = DbSchema.GetTableSchema(p_tblName);
            foreach (var row in schema.PrimaryKey)
            {
                tbl._columns.Add(new Column(row.Name, row.Type));
            }
            foreach (var row in schema.Columns)
            {
                tbl._columns.Add(new Column(row.Name, row.Type));
            }
            return tbl;
        }

    }
}
