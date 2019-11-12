#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-28 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Model;
using Dt.Core.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 数据访问类
    /// </summary>
    public partial class DataAccess
    {
        #region 成员变量
        const string _saveError = "数据源不可为空！";
        const string _primaryError = "数据源表【{0}】中不包含主键列【{1}】！";
        const string _unchangedMsg = "没有需要保存的数据！";
        // 服务名称
        protected readonly string _svc;
        // 表名
        protected readonly string _tbl;
        List<OmColumn> _columns;
        #endregion

        #region 构造方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_svcName">服务名称</param>
        /// <param name="p_tblName">表名</param>
        public DataAccess(string p_svcName, string p_tblName)
        {
            if (string.IsNullOrEmpty(p_svcName) || string.IsNullOrEmpty(p_tblName))
                throw new Exception("服务名或表名不可为空！");
            _svc = p_svcName;
            _tbl = p_tblName;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取当前表的所有列
        /// </summary>
        List<OmColumn> Columns
        {
            get
            {
                if (_columns == null)
                    _columns = AtLocal.QueryColumns(_tbl).ToList();
                return _columns;
            }
        }
        #endregion

        #region 查询
        /// <summary>
        /// 以参数值方式执行Sql语句，返回结果集
        /// </summary>
        /// <typeparam name="TRow">实体类型</typeparam>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Table数据集</returns>
        public Task<Table<TRow>> Query<TRow>(string p_keyOrSql, object p_params = null)
            where TRow : Row
        {
            return new UnaryRpc(
                _svc,
                "Da.Query",
                p_keyOrSql,
                p_params
            ).Call<Table<TRow>>();
        }

        /// <summary>
        /// 按页查询数据
        /// </summary>
        /// <typeparam name="TRow">实体类型</typeparam>
        /// <param name="p_starRow">起始行号：mysql中第一行为0行</param>
        /// <param name="p_pageSize">每页显示行数</param>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回Table数据集</returns>
        public Task<Table<TRow>> GetPage<TRow>(int p_starRow, int p_pageSize, string p_keyOrSql, object p_params = null)
            where TRow : Row
        {
            return new UnaryRpc(
                _svc,
                "Da.GetPage",
                p_starRow,
                p_pageSize,
                p_keyOrSql,
                p_params
            ).Call<Table<TRow>>();
        }

        /// <summary>
        /// 以参数值方式执行Sql语句，只返回第一行数据
        /// </summary>
        /// <typeparam name="TRow">实体类型</typeparam>
        /// <param name="p_keyOrSql">Sql字典中的键名(无空格) 或 Sql语句</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回第一行Row或null</returns>
        public Task<TRow> GetRow<TRow>(string p_keyOrSql, object p_params = null)
            where TRow : Row
        {
            return new UnaryRpc(
                _svc,
                "Da.GetRow",
                p_keyOrSql,
                p_params
            ).Call<TRow>();
        }
        #endregion

        #region 保存
        /// <summary>
        /// 保存单行数据
        /// </summary>
        /// <param name="p_row">待保存的行</param>
        /// <param name="p_isNotify">是否提示保存结果</param>
        /// <returns>是否成功</returns>
        public async Task<bool> Save(Row p_row, bool p_isNotify = true)
        {
            if (p_row == null || (!p_row.IsAdded && !p_row.IsChanged))
            {
                if (p_isNotify)
                    AtKit.Warn(_unchangedMsg);
                return false;
            }

            Dict dt = BuildRowSqlDict(p_row);
            int cnt = await Exec((string)dt["text"], (Dict)dt["params"]);
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
        /// 一个事务内批量保存数据，根据行状态执行增改
        /// </summary>
        /// <param name="p_tbl">待保存</param>
        /// <param name="p_isNotify">是否提示保存结果</param>
        /// <returns>true 保存成功</returns>
        public async Task<bool> Save(Table p_tbl, bool p_isNotify = true)
        {
            if (p_tbl == null)
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

            suc = await BatchExec(dts);
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
        /// 一对多父子表批量保存(确保同一数据库)，在主表所在的Da中执行
        /// </summary>
        /// <param name="p_row">待保存的父行</param>
        /// <param name="p_childTbls">待保存的子表</param>
        /// <param name="p_isNotify">是否提示保存结果</param>
        /// <returns>是否成功</returns>
        public async Task<bool> BatchSave(Row p_row, List<ChildTable> p_childTbls, bool p_isNotify = true)
        {
            if (p_row == null
                || p_childTbls == null
                || p_childTbls.Count == 0)
            {
                if (p_isNotify)
                    AtKit.Warn(_saveError);
                return false;
            }

            // 父行
            List<Dict> dts = new List<Dict>();
            if (p_row.IsChanged || p_row.IsAdded)
                dts.Add(BuildRowSqlDict(p_row));

            // 整理一对多父子表批量保存参数
            List<Dict> dtsChild = new List<Dict>();
            foreach (var child in p_childTbls)
            {
                List<Dict> ls = child.Da.BuildSqlDict(child.Data);
                if (ls != null && ls.Count > 0)
                    dtsChild.AddRange(ls);
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
                suc = await Exec((string)dts[0]["text"], (Dict)dts[0]["params"]) > 0;
            }
            else
            {
                if (dts.Count != 0)
                {
                    // 父子都需保存，注意顺序，主表在前
                    dts.AddRange(dtsChild);
                    suc = await BatchExec(dts);
                }
                else
                {
                    // 只保存子表
                    suc = await BatchExec(dtsChild);
                }
            }

            if (suc)
            {
                p_row.AcceptChanges();
                foreach (var child in p_childTbls)
                {
                    child.Data.AcceptChanges();
                }
                if (p_isNotify)
                    AtKit.Msg("保存成功！");
                return true;
            }

            if (p_isNotify)
                AtKit.Msg("保存失败！");
            return false;
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除单行数据，确保包含主键列
        /// </summary>
        /// <param name="p_row">待删除的行</param>
        /// <param name="p_isNotify">是否提示删除结果</param>
        /// <returns>true 删除成功</returns>
        public async Task<bool> Delete(Row p_row, bool p_isNotify = true)
        {
            if (p_row == null)
            {
                if (p_isNotify)
                    AtKit.Warn(_saveError);
                return false;
            }

            Dict dt = BuildDeleteDict(new List<Row> { p_row });
            bool suc = await Exec((string)dt["text"], ((List<Dict>)dt["params"]).First()) > 0;
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
        /// 删除多行数据，确保行数据中包含主键列
        /// </summary>
        /// <param name="p_rows">多行数据</param>
        /// <param name="p_isNotify">是否提示删除结果</param>
        /// <returns>true 删除成功</returns>
        public async Task<bool> Delete(List<Row> p_rows, bool p_isNotify = true)
        {
            if (p_rows == null || p_rows.Count == 0)
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
                suc = (await Exec((string)dt["text"], ((List<Dict>)dt["params"]).First())) > 0;
            }
            else
            {
                // 多行
                suc = await BatchExec(new List<Dict> { dt });
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
        #endregion

        #region 工具方法
        /// <summary>
        /// 创建独立行并设置初始值，已设置IsAdded标志！无所属Table！
        /// <para>参数null时为空行</para>
        /// <para>有参数时将参数的属性值作为初始值，前提是属性名和列名相同(不区分大小写)且类型相同</para>
        /// <para>支持匿名对象，主要为简化编码</para>
        /// </summary>
        /// <param name="p_init">含初始值的对象，一般为匿名对象</param>
        /// <returns>返回独立行</returns>
        public Row NewRow(object p_init = null)
        {
            return NewRow<Row>(p_init);
        }

        /// <summary>
        /// 创建独立行并设置初始值，已设置IsAdded标志！无所属Table！
        /// <para>参数null时为空行</para>
        /// <para>有参数时将参数的属性值作为初始值，前提是属性名和列名相同(不区分大小写)且类型相同</para>
        /// <para>支持匿名对象，主要为简化编码</para>
        /// </summary>
        /// <typeparam name="TRow">实体行类型</typeparam>
        /// <param name="p_init">含初始值的对象，一般为匿名对象</param>
        /// <returns>返回独立行</returns>
        public TRow NewRow<TRow>(object p_init = null)
            where TRow : Row
        {
            TRow row = Activator.CreateInstance<TRow>();
            row.IsAdded = true;

            // 空行
            if (p_init == null)
            {
                foreach (var col in Columns)
                {
                    new Cell(row, col.ColName, Table.GetColType(col.DbType));
                }
                return row;
            }

            // 匿名对象无法在GetProperty时指定BindingFlags！
            var props = p_init.GetType().GetProperties().ToList();
            foreach (var col in Columns)
            {
                Type colType = Table.GetColType(col.DbType);

                if (props.Count > 0)
                {
                    int index = -1;
                    PropertyInfo pi = null;
                    for (int i = 0; i < props.Count; i++)
                    {
                        pi = props[i];
                        if (pi.Name.Equals(col.ColName, StringComparison.OrdinalIgnoreCase))
                        {
                            index = i;
                            break;
                        }
                    }

                    // 存在同名属性
                    if (index > -1)
                    {
                        // 减小下次查询范围
                        props.RemoveAt(index);
                        var val = pi.GetValue(p_init);

                        // 类型相同
                        if (pi.PropertyType == colType)
                        {
                            new Cell(col.ColName, val, row);
                            continue;
                        }

                        // 类型不同先转换，转换失败不赋值
                        try
                        {
                            var obj = Convert.ChangeType(val, colType);
                            new Cell(col.ColName, obj, row);
                            continue;
                        }
                        catch { }
                    }
                }

                new Cell(row, col.ColName, colType);
            }
            return row;
        }

        /// <summary>
        /// 创建空Table
        /// </summary>
        /// <returns>空表</returns>
        public Table NewTable()
        {
            return NewTable<Row>();
        }

        /// <summary>
        /// 创建空Table
        /// </summary>
        /// <typeparam name="TRow">实体行类型</typeparam>
        /// <returns>空表</returns>
        public Table<TRow> NewTable<TRow>()
            where TRow : Row
        {
            var tbl = new Table<TRow>();
            foreach (var col in Columns)
            {
                tbl.Columns.Add(new Column(col.ColName, Table.GetColType(col.DbType)));
            }
            return tbl;
        }

        /// <summary>
        /// 互换两行的显示位置，确保包含 id,dispidx 列
        /// </summary>
        /// <param name="p_src"></param>
        /// <param name="p_tgt"></param>
        /// <returns>true 互换成功</returns>
        public Task<bool> ExchangeDispidx(Row p_src, Row p_tgt)
        {
            Table tbl = new Table { { "id", typeof(long) }, { "dispidx", typeof(int) } };

            var save = tbl.AddRow(new { id = p_src.ID });
            save.AcceptChanges();
            save["dispidx"] = p_tgt["dispidx"];

            save = tbl.AddRow(new { id = p_tgt.ID });
            save.AcceptChanges();
            save["dispidx"] = p_src["dispidx"];

            return Save(tbl, false);
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 构造单行数据sql参数
        /// </summary>
        /// <param name="p_row">待保存的行</param>
        /// <returns>返回提交参数</returns>
        Dict BuildRowSqlDict(Row p_row)
        {
            Dict result = new Dict();
            Dict dt = new Dict();

            // 根据表名生成sql
            StringBuilder sql = new StringBuilder();
            if (p_row.IsAdded)
            {
                StringBuilder insertCol = new StringBuilder();
                StringBuilder insertVal = new StringBuilder();
                foreach (var col in Columns)
                {
                    // 检查是否包含主键
                    if (col.IsPrimary && !p_row.Contains(col.ColName))
                        AtKit.Throw(string.Format(_primaryError, _tbl, col.ColName));

                    if (p_row.Contains(col.ColName))
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
                sql.Append(_tbl);
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

                foreach (var col in Columns)
                {
                    // 检查是否包含主键
                    if (col.IsPrimary && !p_row.Contains(col.ColName))
                        AtKit.Throw(string.Format(_primaryError, _tbl, col.ColName));

                    if (p_row.Contains(col.ColName))
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
                sql.Append(_tbl);
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
        /// 根据RowState构造批量更新的参数
        /// </summary>
        /// <param name="p_tbl">待保存数据源</param>
        /// <returns>批量更新需要的参数</returns>
        List<Dict> BuildSqlDict(Table p_tbl)
        {
            // 取出主键列和普通列
            List<OmColumn> priCols = new List<OmColumn>();
            List<OmColumn> allCols = new List<OmColumn>();
            foreach (var col in Columns)
            {
                allCols.Add(col);
                if (col.IsPrimary)
                {
                    // 检查是否包含主键
                    if (!p_tbl.Columns.Contains(col.ColName))
                        AtKit.Throw(string.Format(_primaryError, _tbl, col.ColName));
                    priCols.Add(col);
                }
            }

            // 分类整理增改的行列表，记录需要更新的列
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
                sql.Append(_tbl);
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
                    sql.Append(_tbl);
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
        /// 构造删除行数据的sql
        /// </summary>
        /// <param name="p_rows"></param>
        /// <returns></returns>
        Dict BuildDeleteDict(List<Row> p_rows)
        {
            Dict result = new Dict();
            Dict dt = new Dict();
            StringBuilder whereVal = new StringBuilder();
            foreach (var col in AtLocal.QueryPrimaryColumns(_tbl))
            {
                // 检查是否包含主键
                if (!p_rows[0].Contains(col.ColName))
                    AtKit.Throw(string.Format(_primaryError, _tbl, col.ColName));

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
            sql.Append(_tbl);
            sql.Append(" where");
            sql.Append(whereVal.Remove(whereVal.Length - 3, 3).ToString());

            result["text"] = sql.ToString();
            result["params"] = GenRowParm(dt);
            return result;
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
        #endregion
    }
}