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
    /// 客户端数据表部分
    /// </summary>
    public partial class Table : ITreeData
    {
        #region 创建表结构
        /// <summary>
        /// 根据表名创建空Table
        /// </summary>
        /// <param name="p_tblName">表名</param>
        /// <returns></returns>
        public static Table Create(string p_tblName)
        {
            Check.NotNullOrEmpty(p_tblName);
            Table tbl = new Table();
            foreach (var col in AtLocal.QueryColumns(p_tblName))
            {
                tbl._columns.Add(new Column(col.ColName, GetColType(col.DbType)));
            }
            return tbl;
        }

        /// <summary>
        /// 根据本地库表名创建空Table
        /// </summary>
        /// <param name="p_tblName">本地库表名</param>
        /// <returns></returns>
        public static Table CreateLocal(string p_tblName)
        {
            Check.NotNullOrEmpty(p_tblName);
            return AtLocal.Query($"select * from {p_tblName} where 1!=1");
        }
        #endregion

        #region ITreeData
        IEnumerable<object> ITreeData.GetTreeRoot()
        {
            // 固定字段 id, parentid
            if (_columns.Contains("parentid") && Count > 0)
            {
                // parentid类型可以为long?
                return from row in this
                       where row.Str("parentid") == string.Empty
                       select row;
            }
            return null;
        }

        IEnumerable<object> ITreeData.GetTreeItemChildren(object p_parent)
        {
            Row parent = p_parent as Row;
            if (parent != null && parent.Contains("id"))
            {
                // id, parentid类型可以为long, string等
                return from row in this
                       where row.Str("parentid") == parent.Str("id")
                       select row;
            }
            return null;
        }
        #endregion
    }
}
