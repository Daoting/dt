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
        string _sqlSelect;
        string _sqlDelete;
        #endregion

        public TableSchema(string p_name)
        {
            Name = p_name;
        }

        /// <summary>
        /// 表名，小写
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 主键列列表
        /// </summary>
        public List<TableCol> PrimaryKey { get; } = new List<TableCol>();

        /// <summary>
        /// 普通列列表
        /// </summary>
        public List<TableCol> Columns { get; } = new List<TableCol>();

        /// <summary>
        /// 根据主键查询实体的sql，只支持单主键
        /// </summary>
        public string SqlSelect
        {
            get
            {
                if (_sqlSelect == null || PrimaryKey.Count == 1)
                {
                    // 主键变量名固定为id
                    _sqlSelect = $"select * from `{Name}` where {PrimaryKey[0].Name}=@id";
                }
                return _sqlSelect;
            }
        }

        /// <summary>
        /// 根据主键删除实体的sql，只支持单主键
        /// </summary>
        public string SqlDelete
        {
            get
            {
                if (_sqlDelete == null || PrimaryKey.Count == 1)
                {
                    // 主键变量名固定为id
                    _sqlDelete = $"delete from `{Name}` where {PrimaryKey[0].Name}=@id";
                }
                return _sqlDelete;
            }
        }

        /// <summary>
        /// 生成保存实体的sql及参数，返回Dict的结构：包含两个键text和params，text值为sql字符串，params值为sql参数Dict
        /// </summary>
        /// <param name="p_row">待保存的行</param>
        /// <returns>返回提交参数</returns>
        internal Dict GetSaveSql(Row p_row)
        {
            // 不再重复判断
            //if (p_row == null)
            //    throw new Exception(_saveError);

            //// 无需保存
            //if (!p_row.IsAdded && !p_row.IsChanged)
            //    return null;

            // 检查是否包含主键
            foreach (var col in PrimaryKey)
            {
                if (!p_row.Contains(col.Name))
                    throw new Exception(string.Format(_primaryError, p_row.GetType().Name, col.Name));
            }

            Dict dtParams = new Dict();
            StringBuilder sql = new StringBuilder();
            if (p_row.IsAdded)
            {
                // 插入
                StringBuilder insertCol = new StringBuilder();
                StringBuilder insertVal = new StringBuilder();
                foreach (var col in PrimaryKey.Concat(Columns))
                {
                    if (!p_row.Contains(col.Name))
                        continue;

                    if (insertCol.Length > 0)
                        insertCol.Append(",");
                    insertCol.Append(col.Name);

                    if (insertVal.Length > 0)
                        insertVal.Append(",");
                    insertVal.Append("@");
                    insertVal.Append(col.Name);

                    dtParams[col.Name] = p_row[col.Name];
                }
                sql.Append("insert into ");
                sql.Append(Name);
                sql.Append("(");
                sql.Append(insertCol.ToString());
                sql.Append(") values (");
                sql.Append(insertVal.ToString());
                sql.Append(")");
            }
            else
            {
                // 更新
                StringBuilder updateVal = new StringBuilder();
                StringBuilder whereVal = new StringBuilder();
                int updateIndex = 1;

                foreach (var col in PrimaryKey.Concat(Columns))
                {
                    if (!p_row.Contains(col.Name) || !p_row.Cells[col.Name].IsChanged)
                        continue;

                    // 只更新变化的列
                    if (updateVal.Length > 0)
                        updateVal.Append(", ");
                    updateVal.Append(col.Name);
                    updateVal.Append("=@");
                    updateVal.Append(updateIndex);

                    // 更新主键时避免重复
                    dtParams[updateIndex.ToString()] = p_row[col.Name];
                    updateIndex++;
                }

                // 主键
                foreach (var col in PrimaryKey)
                {
                    if (whereVal.Length > 0)
                        whereVal.Append(" and ");
                    whereVal.Append(col.Name);
                    whereVal.Append("=@");
                    whereVal.Append(col.Name);

                    // 主键可能被更新
                    dtParams[col.Name] = p_row.Cells[col.Name].OriginalVal;
                }

                sql.Append("update ");
                sql.Append(Name);
                sql.Append(" set ");
                sql.Append(updateVal.ToString());
                sql.Append(" where ");
                sql.Append(whereVal.ToString());
            }

            Dict dt = new Dict();
            dt["text"] = sql.ToString();
            dt["params"] = dtParams;
            return dt;
        }

        /// <summary>
        /// 生成保存实体列表的sql及参数，Dict结构：text(值为sql模板)，params(值为List`Dict`，每个Dict为sql参数)
        /// </summary>
        /// <param name="p_rows">实体列表</param>
        /// <returns></returns>
        internal List<Dict> GetBatchSaveSql(IList p_rows)
        {
            if (p_rows == null)
                return null;

            Row first;
            if (p_rows.Count == 0)
            {
                // Table<Entity>只包含删除列表的情况！
                if (p_rows is Table tb && tb.ExistDeleted)
                    first = tb.DeletedRows[0];
                else
                    return null;
            }
            else
            {
                first = p_rows[0] as Row;
            }

            // 检查是否包含主键
            foreach (var col in PrimaryKey)
            {
                if (!first.Contains(col.Name))
                    throw new Exception(string.Format(_primaryError, p_rows[0].GetType().Name, col.Name));
            }

            // 分类整理增改的行列表，记录需要更新的列
            bool insert = false;
            bool update = false;
            bool delete = false;
            List<Row> insertRows = new List<Row>();
            List<Row> updateRows = new List<Row>();
            List<string> updateCols = new List<string>();
            foreach (Row row in p_rows)
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
                    foreach (var cell in row.ChangedCells)
                    {
                        if (!updateCols.Contains(cell.ID))
                            updateCols.Add(cell.ID);
                    }
                }
            }
            if (p_rows is Table tbl)
                delete = tbl.ExistDeleted;

            // 不需要保存
            if (!insert && !update && !delete)
                return null;

            // 构造sql及参数列表
            StringBuilder insertCol = null;
            StringBuilder insertVal = null;
            StringBuilder updateVal = null;
            StringBuilder whereVal = null;
            Dict dtInsert = null;
            Dict dtUpdate = null;
            Dict dtDelete = null;
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

            if (delete)
            {
                if (whereVal == null)
                    whereVal = new StringBuilder();
                dtDelete = new Dict();
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
                    insertCol.Append(col.Name);

                    if (insertVal.Length > 0)
                        insertVal.Append(",");
                    insertVal.Append("@");
                    insertVal.Append(col.Name);

                    dtInsert[col.Name] = (from row in insertRows
                                          select row[col.Name]).ToArray();
                }

                if (update && updateCols.Contains(col.Name, StringComparer.OrdinalIgnoreCase))
                {
                    if (updateVal.Length > 0)
                        updateVal.Append(", ");
                    updateVal.Append(col.Name);
                    updateVal.Append("=@");
                    updateVal.Append(updateIndex);

                    // 更新主键时避免重复
                    dtUpdate[updateIndex.ToString()] = (from row in updateRows
                                                        select row[col.Name]).ToArray();
                    updateIndex++;
                }
            }

            // 主键
            if (update || delete)
            {
                foreach (var col in PrimaryKey)
                {
                    if (whereVal.Length > 0)
                        whereVal.Append(" and ");
                    whereVal.Append(col.Name);
                    whereVal.Append("=@");
                    whereVal.Append(col.Name);

                    if (update)
                    {
                        // 主键可能被更新
                        dtUpdate[col.Name] = (from row in updateRows
                                              select row.Cells[col.Name].OriginalVal).ToArray();
                    }

                    if (delete)
                    {
                        dtDelete[col.Name] = (from row in ((Table)p_rows).DeletedRows
                                              select row.Cells[col.Name].OriginalVal).ToArray();
                    }
                }
            }

            Dict dt = null;
            StringBuilder sql = new StringBuilder();
            List<Dict> dts = new List<Dict>();

            if (insert)
            {
                sql.Append("insert into ");
                sql.Append(Name);
                sql.Append("(");
                sql.Append(insertCol.ToString());
                sql.Append(") values (");
                sql.Append(insertVal.ToString());
                sql.Append(")");

                dt = new Dict();
                dt["text"] = sql.ToString();
                dt["params"] = GenRowParm(dtInsert);
                dts.Add(dt);
            }

            if (update)
            {
                string upVals = updateVal.ToString();
                // 当updateVal为空的时候，避免出现 ”update table set where “（set 和where 中间无内容的情况）
                if (!string.IsNullOrEmpty(upVals))
                {
                    sql = new StringBuilder();
                    sql.Append("update ");
                    sql.Append(Name);
                    sql.Append(" set ");
                    sql.Append(upVals);
                    sql.Append(" where ");
                    sql.Append(whereVal.ToString());

                    dt = new Dict();
                    dt["text"] = sql.ToString();
                    dt["params"] = GenRowParm(dtUpdate);
                    dts.Add(dt);
                }
            }

            if (delete)
            {
                sql = new StringBuilder();
                sql.Append("delete from ");
                sql.Append(Name);
                sql.Append(" where ");
                sql.Append(whereVal.ToString());

                dt = new Dict();
                dt["text"] = sql.ToString();
                dt["params"] = GenRowParm(dtDelete);
                dts.Add(dt);
            }
            return dts;
        }

        /// <summary>
        /// 生成删除行数据的sql，Dict结构：text(值为sql模板)，params(值为List`Dict`，每个Dict为sql参数)
        /// </summary>
        /// <param name="p_rows"></param>
        /// <returns></returns>
        internal Dict GetDeleteSql(IList p_rows)
        {
            // 不再重复判断
            //if (p_rows == null || p_rows.Count == 0)
            //    throw new Exception(_saveError);

            Dict dtParams = new Dict();
            var first = p_rows[0] as Row;
            StringBuilder whereVal = new StringBuilder();
            foreach (var col in PrimaryKey)
            {
                // 检查是否包含主键
                if (!first.Contains(col.Name))
                    throw new Exception(string.Format(_primaryError, p_rows[0].GetType().Name, col.Name));

                if (whereVal.Length > 0)
                    whereVal.Append(" and ");
                whereVal.Append(col.Name);
                whereVal.Append("=@");
                whereVal.Append(col.Name);

                // 主键可能已修改
                dtParams[col.Name] = (from row in p_rows.OfType<Row>()
                                      select row.Cells[col.Name].OriginalVal).ToArray();
            }

            StringBuilder sql = new StringBuilder();
            sql.Append("delete from ");
            sql.Append(Name);
            sql.Append(" where ");
            sql.Append(whereVal.ToString());

            Dict result = new Dict();
            result["text"] = sql.ToString();
            result["params"] = GenRowParm(dtParams);
            return result;
        }

        /// <summary>
        /// 选择所有数据的sql
        /// </summary>
        /// <returns></returns>
        internal string GetSelectAllSql()
        {
            return $"select * from `{Name}`";
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
}
