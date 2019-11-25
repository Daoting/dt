#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-12 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
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
        static Dictionary<string, Table> _tblTemplate = new Dictionary<string, Table>();

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
        /// 创建独立行并设置初始值，已设置IsAdded标志！参数null时为空行
        /// <para>有参数时将参数的属性值作为初始值，前提是属性名和列名相同(不区分大小写)且类型相同</para>
        /// <para>支持匿名对象，主要为简化编码</para>
        /// </summary>
        /// <param name="p_tblName">表名</param>
        /// <param name="p_init">含初始值的对象，一般为匿名对象</param>
        /// <returns>返回独立行</returns>
        public static Row NewRow(string p_tblName, object p_init = null)
        {
            Check.NotNullOrEmpty(p_tblName);
            string tblName = p_tblName.ToLower();
            Table tbl;
            if (!_tblTemplate.TryGetValue(tblName, out tbl))
            {
                tbl = Create(tblName);
                _tblTemplate[tblName] = tbl;
            }
            return tbl.NewRow(p_init);
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
