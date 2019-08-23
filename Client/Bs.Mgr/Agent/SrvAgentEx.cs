#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Model;
using Dt.Core.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Bs.Mgr
{
    /// <summary>
    /// 所有服务的公共Api代理类的扩展方法，非自动生成
    /// </summary>
    /// <typeparam name="TSrv">服务类型</typeparam>
    public partial class SrvAgent<TSrv>
    {
        /// <summary>
        /// 保存单行数据，确保已指定表名并包含主键列，客户端生成sql，节省服务器资源但不安全
        /// </summary>
        /// <param name="p_row">待保存的行</param>
        /// <param name="p_isNotify">是否提示保存结果</param>
        /// <returns>是否成功</returns>
        public static async Task<bool> Save(Row p_row, bool p_isNotify = true)
        {
            if (p_row == null
                || p_row.Table == null
                || string.IsNullOrEmpty(p_row.Table.Name)
                || (!p_row.IsAdded && !p_row.IsChanged))
            {
                if (p_isNotify)
                    AtKit.Warn(_saveError);
                return false;
            }

            Dict dt = BuildRowSqlDict(p_row);
            int cnt = await new UnaryRpc(typeof(TSrv).Name, "Db.ExecSql", (string)dt["text"], (Dict)dt["params"]).Call<int>();
            if (cnt > 0)
            {
                p_row.AcceptChanges();
                if (p_isNotify)
                    AtKit.Msg("保存成功！");
                return true;
            }

            if (p_isNotify)
                AtKit.Msg("保存失败！");
            return false;
        }

        /// <summary>
        /// 一对多父子表批量保存(确保同一数据库)
        /// </summary>
        /// <param name="p_row">待保存的父行</param>
        /// <param name="p_childTbls">待保存的子表</param>
        /// <param name="p_isNotify">是否提示保存结果</param>
        /// <returns>是否成功</returns>
        public static async Task<bool> Save(Row p_row, List<Table> p_childTbls, bool p_isNotify = true)
        {
            if (p_row == null
                || p_row.Table == null
                || string.IsNullOrEmpty(p_row.Table.Name)
                || p_childTbls == null
                || p_childTbls.Count == 0)
            {
                if (p_isNotify)
                    AtKit.Warn(_saveError);
                return false;
            }

            // 父行
            List<Dict> dts = new List<Dict>();
            if (p_row.IsChanged)
                dts.Add(BuildRowSqlDict(p_row));

            // 整理一对多父子表批量保存参数
            List<Dict> dtsChild = new List<Dict>();
            foreach (Table child in p_childTbls)
            {
                if (!string.IsNullOrEmpty(child.Name))
                {
                    List<Dict> ls = BuildSqlDict(child);
                    if (ls != null && ls.Count > 0)
                        dtsChild.AddRange(ls);
                }
            }

            if (dtsChild.Count == 0 && dts.Count == 0)
            {
                // 不需要保存
                if (p_isNotify)
                    AtKit.Msg(_unchangedMsg);
                return true;
            }

            bool suc = false;
            if (dtsChild.Count == 0)
            {
                // 只保存父行
                int cnt = await new UnaryRpc(typeof(TSrv).Name, "Db.ExecSql", (string)dts[0]["text"], (Dict)dts[0]["params"]).Call<int>();
                suc = cnt > 0;
            }
            else
            {
                if (dts.Count != 0)
                {
                    // 父子都需保存
                    suc = await new UnaryRpc(typeof(TSrv).Name, "Db.RelationExecs", dts, dtsChild).Call<bool>();
                }
                else
                {
                    // 只保存子表
                    suc = (await new UnaryRpc(typeof(TSrv).Name, "Db.BatchExecs", dtsChild).Call<int>()) > 0;
                }
            }

            if (suc)
            {
                p_row.AcceptChanges();
                foreach (Table child in p_childTbls)
                {
                    child.AcceptChanges();
                }
                if (p_isNotify)
                    AtKit.Msg("保存成功！");
                return true;
            }

            if (p_isNotify)
                AtKit.Msg("保存失败！");
            return false;
        }

        /// <summary>
        /// 一个事务内批量保存数据，根据行状态执行增删改，确保已指定表名并包含主键列
        /// </summary>
        /// <param name="p_tbl">待保存</param>
        /// <param name="p_isNotify">是否提示保存结果</param>
        /// <returns>true 保存成功</returns>
        public static async Task<bool> Save(Table p_tbl, bool p_isNotify = true)
        {
            if (p_tbl == null || string.IsNullOrEmpty(p_tbl.Name))
            {
                if (p_isNotify)
                    AtKit.Warn(_saveError);
                return false;
            }

            bool suc = false;
            List<Dict> dts = BuildSqlDict(p_tbl);

            // 不需要保存
            if (dts == null || dts.Count == 0)
            {
                if (p_isNotify)
                    AtKit.Msg(_unchangedMsg);
                return true;
            }

            int count = 0;
            if (dts.Count == 1)
                count = await new UnaryRpc(typeof(TSrv).Name, "Db.BatchExec", (string)dts[0]["text"], (List<Dict>)dts[0]["params"]).Call<int>();
            else
                count = await new UnaryRpc(typeof(TSrv).Name, "Db.BatchExecs", dts).Call<int>();
            suc = count > 0;

            if (suc)
            {
                p_tbl.AcceptChanges();
                if (p_isNotify)
                    AtKit.Msg("保存成功！");
                return true;
            }

            if (p_isNotify)
                AtKit.Msg("保存失败！");
            return false;
        }

        /// <summary>
        /// 一个事务内批量保存多个表数据(确保同一数据库)
        /// <para>按List中的顺序进行过滤需要保存的表，确保要保存的表中已指定表名并包含主键列</para>
        /// </summary>
        /// <param name="p_tbls">待保存</param>
        /// <param name="p_isNotify">是否提示保存结果</param>
        /// <returns>true 保存成功</returns>
        public static async Task<bool> Save(List<Table> p_tbls, bool p_isNotify = true)
        {
            if (p_tbls == null || p_tbls.Count == 0)
            {
                if (p_isNotify)
                    AtKit.Warn(_saveError);
                return false;
            }

            List<Dict> dts = new List<Dict>();
            foreach (Table tbl in p_tbls)
            {
                if (tbl == null || !tbl.IsChanged || string.IsNullOrEmpty(tbl.Name))
                    continue;

                List<Dict> ls = BuildSqlDict(tbl);
                if (ls != null && ls.Count > 0)
                    dts.AddRange(ls);
            }

            // 不需要保存
            if (dts.Count == 0)
            {
                if (p_isNotify)
                    AtKit.Msg(_unchangedMsg);
                return true;
            }

            int count = 0;
            if (dts.Count == 1)
                count = await new UnaryRpc(typeof(TSrv).Name, "Db.BatchExec", (string)dts[0]["text"], (Dict)dts[0]["params"]).Call<int>();
            else
                count = await new UnaryRpc(typeof(TSrv).Name, "Db.BatchExecs", dts).Call<int>();

            if (count > 0)
            {
                foreach (Table tbl in p_tbls)
                {
                    tbl.AcceptChanges();
                }
                if (p_isNotify)
                    AtKit.Msg("保存成功！");
                return true;
            }

            if (p_isNotify)
                AtKit.Msg("保存失败！");
            return false;
        }

        /// <summary>
        /// 删除单行数据，确保已指定表名并包含主键列
        /// </summary>
        /// <param name="p_row">待删除的行</param>
        /// <param name="p_isNotify">是否提示删除结果</param>
        /// <returns>true 删除成功</returns>
        public static async Task<bool> Delete(Row p_row, bool p_isNotify = true)
        {
            if (p_row == null
                || p_row.Table == null
                || string.IsNullOrEmpty(p_row.Table.Name))
            {
                if (p_isNotify)
                    AtKit.Warn(_saveError);
                return false;
            }

            Dict dt = BuildDeleteDict(new List<Row> { p_row });
            bool suc = (await new UnaryRpc(typeof(TSrv).Name, "Db.ExecSql", (string)dt["text"], ((List<Dict>)dt["params"]).First()).Call<int>()) > 0;
            if (p_isNotify)
            {
                if (suc)
                    AtKit.Msg("删除成功！");
                else
                    AtKit.Msg("删除失败！");
            }
            return suc;
        }

        /// <summary>
        /// 同一表删除多行数据，确保行数据中包含表的主键列和表名
        /// </summary>
        /// <param name="p_rows">多行数据</param>
        /// <param name="p_isNotify">是否提示删除结果</param>
        /// <returns>true 删除成功</returns>
        public static async Task<bool> Delete(List<Row> p_rows, bool p_isNotify = true)
        {
            if (p_rows == null
                || p_rows.Count == 0
                || p_rows[0].Table == null
                || string.IsNullOrEmpty(p_rows[0].Table.Name))
            {
                if (p_isNotify)
                    AtKit.Warn(_saveError);
                return false;
            }

            bool suc = false;
            Dict dt = BuildDeleteDict(p_rows);
            if (p_rows.Count == 1)
            {
                // 单行
                suc = (await new UnaryRpc(typeof(TSrv).Name, "Db.ExecSql", (string)dt["text"], ((List<Dict>)dt["params"]).First()).Call<int>()) > 0;
            }
            else
            {
                // 多行
                suc = (await new UnaryRpc(typeof(TSrv).Name, "Db.BatchExec", (string)dt["text"], (List<Dict>)dt["params"]).Call<int>()) > 0;
            }

            if (p_isNotify)
            {
                if (suc)
                    AtKit.Msg("删除成功！");
                else
                    AtKit.Msg("删除失败！");
            }
            return suc;
        }

        #region 内部方法
        /// <summary>
        /// 根据RowState构造批量更新的参数
        /// </summary>
        /// <param name="p_tbl">待保存数据源</param>
        /// <returns>批量更新需要的参数</returns>
        static List<Dict> BuildSqlDict(Table p_tbl)
        {
            // 取出主键列和普通列
            List<OmColumn> priCols = new List<OmColumn>();
            List<OmColumn> allCols = new List<OmColumn>();
            foreach (var col in AtLocal.QueryColumns(p_tbl.Name))
            {
                allCols.Add(col);
                if (col.IsPrimary)
                {
                    // 检查是否包含主键
                    if (!p_tbl.Columns.Contains(col.ColName))
                        AtKit.Throw(string.Format(_primaryError, p_tbl.Name, col.ColName));
                    priCols.Add(col);
                }
            }

            // 分类整理增删改的行列表，记录需要更新的列
            bool insert = false;
            bool update = false;
            List<Row> insertRows = new List<Row>();
            List<Row> updateRows = new List<Row>();
            List<string> updateCols = new List<string>();
            foreach (Row row in p_tbl)
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
            foreach (OmColumn col in allCols)
            {
                // Table中不包含的列不参加保存
                if (!p_tbl.Columns.Contains(col.ColName))
                    continue;

                Type tp = p_tbl.Columns[col.ColName].Type;
                if (insert)
                {
                    insertCol.Append(col.ColName);
                    insertCol.Append(",");
                    insertVal.Append("@");
                    insertVal.Append(col.ColName);
                    insertVal.Append(",");
                    dtInsert[col.ColName] = (from row in insertRows
                                             select row[col.ColName]).ToArray();
                }

                if (update && updateCols.Contains(col.ColName))
                {
                    updateVal.Append(" ");
                    updateVal.Append(col.ColName);
                    updateVal.Append("=@");
                    updateVal.Append(updateIndex);
                    updateVal.Append(",");
                    // 更新主键时避免重复
                    dtUpdate[updateIndex.ToString()] = (from row in updateRows
                                                        select row[col.ColName]).ToArray();
                    updateIndex++;
                }
            }

            // 主键
            if (update)
            {
                foreach (OmColumn col in priCols)
                {
                    whereVal.Append(" ");
                    whereVal.Append(col.ColName);
                    whereVal.Append("=@");
                    whereVal.Append(col.ColName);
                    whereVal.Append(" and");
                    dtUpdate[col.ColName] = (from row in updateRows
                                             select row.Cells[col.ColName].OriginalVal).ToArray();
                }
            }

            string whereStr = null;
            Dict dt = null;
            StringBuilder sql = new StringBuilder();
            List<Dict> dts = new List<Dict>();

            if (insert)
            {
                sql.Append("insert into ");
                sql.Append(p_tbl.Name);
                sql.Append("(");
                sql.Append(insertCol.ToString().TrimEnd(','));
                sql.Append(") values (");
                sql.Append(insertVal.ToString().TrimEnd(','));
                sql.Append(")");

                dt = new Dict();
                dt["text"] = sql.ToString();
                dt["params"] = GenRowParm(dtInsert);
                dts.Add(dt);
            }

            if (update)
            {
                whereStr = whereVal.ToString().TrimEnd("and".ToCharArray());
                string upVals = updateVal.ToString();
                // 当updateVal为空的时候，避免出现 ”update table set where “（set 和where 中间无内容的情况）
                if (!string.IsNullOrEmpty(upVals))
                {
                    sql = new StringBuilder();
                    sql.Append("update ");
                    sql.Append(p_tbl.Name);
                    sql.Append(" set");
                    sql.Append(upVals.TrimEnd(','));
                    sql.Append(" where");
                    sql.Append(whereStr);

                    dt = new Dict();
                    dt["text"] = sql.ToString();
                    dt["params"] = GenRowParm(dtUpdate);
                    dts.Add(dt);
                }
            }
            return dts;
        }

        /// <summary>
        /// 将纵向保存的列值转换成横向保存的列值。
        /// </summary>
        /// <param name="p_parm">dict[string,array]</param>
        /// <returns></returns>
        static List<Dict> GenRowParm(Dict p_parm)
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

        /// <summary>
        /// 构造单行数据sql参数
        /// </summary>
        /// <param name="p_row">待保存的行</param>
        /// <param name="p_tblName">表名</param>
        /// <returns>提交参数</returns>
        static Dict BuildRowSqlDict(Row p_row)
        {
            Table tbl = p_row.Table;
            Dict result = new Dict();
            Dict dt = new Dict();

            // 根据表名生成sql
            StringBuilder sql = new StringBuilder();
            if (p_row.IsAdded)
            {
                StringBuilder insertCol = new StringBuilder();
                StringBuilder insertVal = new StringBuilder();
                foreach (var col in AtLocal.QueryColumns(tbl.Name))
                {
                    // 检查是否包含主键
                    if (col.IsPrimary && !tbl.Columns.Contains(col.ColName))
                        AtKit.Throw(string.Format(_primaryError, tbl.Name, col.ColName));

                    if (tbl.Columns.Contains(col.ColName))
                    {
                        insertCol.Append(col.ColName);
                        insertCol.Append(",");
                        insertVal.Append("@");
                        insertVal.Append(col.ColName);
                        insertVal.Append(",");
                        dt[col.ColName] = p_row[col.ColName];
                    }
                }
                sql.Append("insert into ");
                sql.Append(tbl.Name);
                sql.Append("(");
                sql.Append(insertCol.ToString().TrimEnd(new char[] { ',' }));
                sql.Append(") values (");
                sql.Append(insertVal.ToString().TrimEnd(new char[] { ',' }));
                sql.Append(")");
            }
            else
            {
                StringBuilder updateVal = new StringBuilder();
                StringBuilder whereVal = new StringBuilder();
                List<Cell> priCells = new List<Cell>();
                int updateIndex = 1;

                foreach (var col in AtLocal.QueryColumns(tbl.Name))
                {
                    // 检查是否包含主键
                    if (col.IsPrimary && !tbl.Columns.Contains(col.ColName))
                        AtKit.Throw(string.Format(_primaryError, tbl.Name, col.ColName));

                    if (tbl.Columns.Contains(col.ColName))
                    {
                        if (p_row.Cells[col.ColName].IsChanged)
                        {
                            // 只更新变化的列
                            updateVal.Append(" ");
                            updateVal.Append(col.ColName);
                            updateVal.Append("=@");
                            updateVal.Append(updateIndex);
                            updateVal.Append(",");
                            // 更新主键时避免重复
                            dt[updateIndex.ToString()] = p_row[col.ColName];
                            updateIndex++;
                        }

                        if (col.IsPrimary)
                        {
                            whereVal.Append(" ");
                            whereVal.Append(col.ColName);
                            whereVal.Append("=@");
                            whereVal.Append(col.ColName);
                            whereVal.Append(" and");
                            priCells.Add(p_row.Cells[col.ColName]);
                        }
                    }
                }
                sql.Append("update ");
                sql.Append(tbl.Name);
                sql.Append(" set");
                sql.Append(updateVal.ToString().TrimEnd(new char[] { ',' }));
                sql.Append(" where");
                sql.Append(whereVal.ToString().TrimEnd("and".ToCharArray()));
                // 确保参数顺序一致
                foreach (var cell in priCells)
                {
                    // 避免更新主键
                    dt[cell.ID] = cell.OriginalVal;
                }
            }

            result["text"] = sql.ToString();
            result["params"] = dt;
            return result;
        }

        /// <summary>
        /// 构造删除行数据的sql
        /// </summary>
        /// <param name="p_rows"></param>
        /// <returns></returns>
        static Dict BuildDeleteDict(List<Row> p_rows)
        {
            string tblName = p_rows[0].Table.Name;
            Dict result = new Dict();
            Dict dt = new Dict();
            StringBuilder whereVal = new StringBuilder();
            var cols = p_rows[0].Table.Columns;
            foreach (var col in AtLocal.QueryPrimaryColumns(tblName))
            {
                // 检查是否包含主键
                if (!cols.Contains(col.ColName))
                    AtKit.Throw(string.Format(_primaryError, tblName, col.ColName));

                whereVal.Append(" ");
                whereVal.Append(col.ColName);
                whereVal.Append("=@");
                whereVal.Append(col.ColName);
                whereVal.Append(" and");
                dt[col.ColName] = (from row in p_rows
                                   select row.Cells[col.ColName].OriginalVal).ToArray();
            }

            StringBuilder sql = new StringBuilder();
            sql.Append("delete from ");
            sql.Append(tblName);
            sql.Append(" where");
            sql.Append(whereVal.Remove(whereVal.Length - 3, 3).ToString());

            result["text"] = sql.ToString();
            result["params"] = GenRowParm(dt);
            return result;
        }
        #endregion

        #region 成员变量
        const string _saveError = "Table和表名不可为空！";
        const string _primaryError = "数据源表【{0}】中不包含主键列【{1}】！";
        const string _unchangedMsg = "没有需要保存的数据！";
        #endregion
    }
}
