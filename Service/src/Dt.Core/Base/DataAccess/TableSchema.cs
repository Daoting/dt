#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-17 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 存储表结构信息，分主键列列表和普通列列表
    /// </summary>
    public class TableSchema
    {
        #region 成员变量
        const string _primaryError = "实体【{0}】中不包含主键列【{1}】！";
        string _sqlDelete;
        #endregion

        public TableSchema(string p_name, DatabaseType p_dbType)
        {
            Name = p_name;
            DbType = p_dbType;

            switch (p_dbType)
            {
                case DatabaseType.MySql:
                case DatabaseType.Sqlite:
                    Prefix = Postfix = "`";
                    VarPrefix = "@";
                    break;

                case DatabaseType.Oracle:
                    Prefix = Postfix = "\"";
                    VarPrefix = ":";
                    break;

                case DatabaseType.SqlServer:
                    Prefix = "[";
                    Postfix = "]";
                    VarPrefix = "@";
                    break;
            }
        }

        /// <summary>
        /// 表名，库里原始大小写
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DatabaseType DbType { get; }

        /// <summary>
        /// 主键列列表
        /// </summary>
        public List<TableCol> PrimaryKey { get; } = new List<TableCol>();

        /// <summary>
        /// 普通列列表
        /// </summary>
        public List<TableCol> Columns { get; } = new List<TableCol>();

        /// <summary>
        /// 表注释
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// sql语句中表名或列名的前缀
        /// </summary>
        public string Prefix { get; }

        /// <summary>
        /// sql语句中表名或列名的后缀
        /// </summary>
        public string Postfix { get; }

        /// <summary>
        /// sql语句中参数前缀
        /// </summary>
        public string VarPrefix { get; }

        /// <summary>
        /// 根据主键删除实体的sql，只支持单主键
        /// </summary>
        public string GetDeleteByIDSql()
        {
            if (_sqlDelete == null)
            {
                if (PrimaryKey.Count == 1)
                {
                    // 主键变量名固定为id
                    _sqlDelete = $"delete from {Prefix}{Name}{Postfix} where {PrimaryKey[0].Name}={VarPrefix}id";
                }
                else
                {
                    throw new Exception("根据主键删除实体的sql，只支持单主键！");
                }
            }
            return _sqlDelete;
        }

        /// <summary>
        /// 生成保存实体列表的sql及参数，Dict结构：text(值为sql模板)，params(值为List`Dict`，每个Dict为sql参数)
        /// </summary>
        /// <param name="p_rows">实体列表</param>
        /// <returns></returns>
        public List<Dict> GetSaveSql<TEntity>(IList<TEntity> p_rows)
            where TEntity : Entity
        {
            // 不再重复判断
            //if (p_rows == null || p_rows.Count == 0)
            //    return null;

            var first = p_rows[0];

            // 检查是否包含主键
            foreach (var col in PrimaryKey)
            {
                if (!first.Contains(col.Name))
                    throw new Exception(string.Format(_primaryError, p_rows[0].GetType().Name, col.Name));
            }

            // 分类整理增改的行列表，记录需要更新的列
            bool insert = false;
            bool update = false;
            List<Row> insertRows = new List<Row>();
            List<Row> updateRows = new List<Row>();
            List<string> updateCols = new List<string>();
            foreach (var row in p_rows)
            {
                if (row.IsAdded)
                {
                    insert = true;
                    insertRows.Add(row);
                }
                else if (row.IsChanged)
                {
                    update = true;
                    updateRows.Add(row);

                    // 合并所有行中需要更新的列
                    foreach (var cell in row.ChangedCells)
                    {
                        if (!updateCols.Contains(cell.ID))
                            updateCols.Add(cell.ID);
                    }
                }
            }

            // 不需要保存
            if (!insert && !update)
                return null;

            // 构造sql及参数列表
            StringBuilder insertCol = null;
            StringBuilder insertVal = null;
            StringBuilder updateVal = null;
            StringBuilder whereVal = null;
            Dict dtInsert = null;
            Dict dtUpdate = null;
            int updateIndex = 1;

            if (insert)
            {
                insertCol = new StringBuilder();
                insertVal = new StringBuilder();
                dtInsert = new Dict();
            }

            if (update)
            {
                updateVal = new StringBuilder();
                whereVal = new StringBuilder();
                dtUpdate = new Dict();
            }

            // 所有列
            foreach (var col in PrimaryKey.Concat(Columns))
            {
                // 不包含的列不参加保存
                if (!first.Contains(col.Name))
                    continue;

                if (insert)
                {
                    if (insertCol.Length > 0)
                        insertCol.Append(",");
                    insertCol.Append(Prefix);
                    insertCol.Append(col.Name);
                    insertCol.Append(Postfix);

                    if (insertVal.Length > 0)
                        insertVal.Append(",");
                    insertVal.Append(VarPrefix);
                    insertVal.Append(col.Name);

                    dtInsert[col.Name] = (from row in insertRows
                                          select row[col.Name]).ToArray();
                }

                if (update && updateCols.Contains(col.Name, StringComparer.OrdinalIgnoreCase))
                {
                    if (updateVal.Length > 0)
                        updateVal.Append(", ");
                    updateVal.Append(Prefix);
                    updateVal.Append(col.Name);
                    updateVal.Append(Postfix);
                    updateVal.Append("=");
                    updateVal.Append(VarPrefix);
                    updateVal.Append(updateIndex);

                    // 更新主键时避免重复
                    dtUpdate[updateIndex.ToString()] = (from row in updateRows
                                                        select row[col.Name]).ToArray();
                    updateIndex++;
                }
            }

            // 主键
            if (update)
            {
                foreach (var col in PrimaryKey)
                {
                    if (whereVal.Length > 0)
                        whereVal.Append(" and ");
                    whereVal.Append(Prefix);
                    whereVal.Append(col.Name);
                    whereVal.Append(Postfix);
                    whereVal.Append("=");
                    whereVal.Append(VarPrefix);
                    whereVal.Append(col.Name);

                    // 主键可能被更新
                    dtUpdate[col.Name] = (from row in updateRows
                                          select row.Cells[col.Name].OriginalVal).ToArray();
                }
            }

            Dict dt = null;
            StringBuilder sql = new StringBuilder();
            List<Dict> dts = new List<Dict>();

            if (insert)
            {
                sql.Append("insert into ");
                sql.Append(Prefix);
                sql.Append(Name);
                sql.Append(Postfix);
                sql.Append("(");
                sql.Append(insertCol.ToString());
                sql.Append(") values (");
                sql.Append(insertVal.ToString());
                sql.Append(")");

                dt = new Dict();
                dt["text"] = sql.ToString();
                var pls = GenRowParm(dtInsert);
                dt["params"] = pls.Count == 1 ? pls[0] : pls;
                dts.Add(dt);
            }

            // 可能存在即使有修改标志，也无需update的情况！
            // 如虚拟实体中的多个实体可能未全部修改，但只要有一个修改就置标志
            if (update)
            {
                string upVals = updateVal.ToString();
                // 当updateVal为空的时候，避免出现 ”update table set where “（set 和where 中间无内容的情况）
                if (!string.IsNullOrEmpty(upVals))
                {
                    sql = new StringBuilder();
                    sql.Append("update ");
                    sql.Append(Prefix);
                    sql.Append(Name);
                    sql.Append(Postfix);
                    sql.Append(" set ");
                    sql.Append(upVals);
                    sql.Append(" where ");
                    sql.Append(whereVal.ToString());

                    dt = new Dict();
                    dt["text"] = sql.ToString();
                    var pls = GenRowParm(dtUpdate);
                    dt["params"] = pls.Count == 1 ? pls[0] : pls;
                    dts.Add(dt);
                }
            }
            return dts;
        }

        /// <summary>
        /// 生成删除行数据的sql，Dict结构：text(值为sql模板)，params(值为List`Dict`，每个Dict为sql参数)
        /// </summary>
        /// <param name="p_rows"></param>
        /// <returns></returns>
        public Dict GetDeleteSql<TEntity>(IList<TEntity> p_rows)
            where TEntity : Entity
        {
            // 不再重复判断
            //if (p_rows == null || p_rows.Count == 0)
            //    throw new Exception(_saveError);

            Dict dtParams = new Dict();
            var first = p_rows[0];
            StringBuilder whereVal = new StringBuilder();
            foreach (var col in PrimaryKey)
            {
                // 检查是否包含主键
                if (!first.Contains(col.Name))
                    throw new Exception(string.Format(_primaryError, p_rows[0].GetType().Name, col.Name));

                if (whereVal.Length > 0)
                    whereVal.Append(" and ");
                whereVal.Append(Prefix);
                whereVal.Append(col.Name);
                whereVal.Append(Postfix);
                whereVal.Append("=");
                whereVal.Append(VarPrefix);
                whereVal.Append(col.Name);

                // 主键可能已修改
                dtParams[col.Name] = (from row in p_rows.OfType<Row>()
                                      select row.Cells[col.Name].OriginalVal).ToArray();
            }

            StringBuilder sql = new StringBuilder();
            sql.Append("delete from ");
            sql.Append(Prefix);
            sql.Append(Name);
            sql.Append(Postfix);
            sql.Append(" where ");
            sql.Append(whereVal.ToString());

            Dict result = new Dict();
            result["text"] = sql.ToString();
            var pls = GenRowParm(dtParams);
            result["params"] = pls.Count == 1 ? pls[0] : pls;
            return result;
        }

        /// <summary>
        /// 生成删除行数据的sql，Dict结构：text(值为sql模板)，params(值为List`Dict`，每个Dict为sql参数)
        /// </summary>
        /// <param name="p_ids">主键列表</param>
        /// <returns></returns>
        public Dict GetDelSqlByIDs(IList p_ids)
        {
            List<Dict> ls = new List<Dict>();
            foreach (var id in p_ids)
            {
                if (id != null && !string.IsNullOrEmpty(id.ToString()))
                {
                    ls.Add(new Dict { { "id", id.ToString() } });
                }
            }
            if (ls.Count == 0)
                return null;

            Dict result = new Dict();
            result["text"] = GetDeleteByIDSql();
            result["params"] = ls.Count == 1 ? ls[0] : ls;
            return result;
        }

        /// <summary>
        /// 选择所有数据的sql
        /// </summary>
        /// <returns></returns>
        public string GetSelectAllSql()
        {
            return $"select * from {Prefix}{Name}{Postfix} a";
        }

        /// <summary>
        /// 获取按列过滤的sql
        /// </summary>
        /// <param name="p_keyName"></param>
        /// <returns></returns>
        public string GetSelectByKeySql(string p_keyName)
        {
            return $"select * from {Prefix}{Name}{Postfix} where {Prefix}{p_keyName}{Postfix}={VarPrefix}{p_keyName}";
        }

        /// <summary>
        /// 获取行数的sql
        /// </summary>
        /// <returns></returns>
        public string GetCountSql()
        {
            return $"select count(*) from {Prefix}{Name}{Postfix}";
        }

        /// <summary>
        /// 将纵向保存的列值转换成横向保存的列值。
        /// </summary>
        /// <param name="p_parm">dict[string,array]</param>
        /// <returns></returns>
        List<Dict> GenRowParm(Dict p_parm)
        {
            if (p_parm == null || p_parm.Count == 0 || ((Array)p_parm.Values.First()).Length == 0)
                return null;

            List<Dict> rtn = new List<Dict>();
            int rowCount = ((Array)p_parm.Values.First()).Length;
            for (int i = 0; i < rowCount; i++)
            {
                Dict parm = new Dict();
                foreach (var item in p_parm)
                {
                    parm[item.Key] = ((object[])item.Value)[i];
                }
                rtn.Add(parm);
            }
            return rtn;
        }
    }

    /// <summary>
    /// 列描述类
    /// </summary>
    public class TableCol
    {
        /// <summary>
        /// 列名，数据库中的原始写法，未调整到小写
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 列类型
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// 列长度，只字符类型有效
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 列是否允许为空
        /// </summary>
        public bool Nullable { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public string Default { get; set; }

        /// <summary>
        /// 列注释
        /// </summary>
        public string Comments { get; set; }
    }

    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DatabaseType
    {
        MySql,
        Oracle,
        SqlServer,
        Sqlite
    }
}
