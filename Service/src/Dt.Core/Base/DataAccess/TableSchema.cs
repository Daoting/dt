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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
                    // 默认始终全大写，不需要用引号括
                    Prefix = Postfix = "";
                    VarPrefix = ":";
                    break;

                case DatabaseType.SqlServer:
                    Prefix = "[";
                    Postfix = "]";
                    VarPrefix = "@";
                    break;

                case DatabaseType.PostgreSql:
                    Prefix = Postfix = "\"";
                    // @ 和 : 都可以
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
        public TableColList Columns { get; } = new TableColList();

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
                    // 判断该列是否需要insert
                    // 列可为null 且 所有值都null时不需要insert
                    bool isNeedInsert = false;
                    List<Cell> ls;
                    if (col.Nullable)
                    {
                        ls = new List<Cell>();
                        foreach (var row in insertRows)
                        {
                            var c = row.Cells[col.Name];
                            ls.Add(c);
                            if (c.Val != null)
                                isNeedInsert = true;
                        }
                    }
                    else
                    {
                        isNeedInsert = true;
                        ls = (from row in insertRows
                              select row.Cells[col.Name]).ToList();
                    }
                    if (!isNeedInsert)
                        continue;

                    if (insertCol.Length > 0)
                        insertCol.Append(",");
                    insertCol.Append(Prefix);
                    insertCol.Append(col.Name);
                    insertCol.Append(Postfix);

                    if (insertVal.Length > 0)
                        insertVal.Append(",");
                    insertVal.Append(VarPrefix);
                    insertVal.Append(col.Name);

                    dtInsert[col.Name] = ls;
                }

                if (update)
                {
                    // 判断该列是否需要update
                    bool isNeedUpdate = false;
                    List<Cell> ls = new List<Cell>();
                    foreach (var row in updateRows)
                    {
                        var c = row.Cells[col.Name];
                        ls.Add(c);
                        if (c.IsChanged)
                            isNeedUpdate = true;
                    }
                    if (!isNeedUpdate)
                        continue;

                    if (updateVal.Length > 0)
                        updateVal.Append(", ");
                    updateVal.Append(Prefix);
                    updateVal.Append(col.Name);
                    updateVal.Append(Postfix);
                    updateVal.Append("=");
                    updateVal.Append(VarPrefix);
                    updateVal.Append(updateIndex);

                    // 更新主键时避免重复
                    dtUpdate[updateIndex.ToString()] = ls;
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
                                          select row.Cells[col.Name]).ToList();
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
                var pls = (DbType == DatabaseType.Oracle) ? GenOrclInsertParm(dtInsert) : GenInsertParm(dtInsert);
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
                    var pls = (DbType == DatabaseType.Oracle) ? GenOrclUpdateParm(dtUpdate) : GenUpdateParm(dtUpdate);
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
                                      select row.Cells[col.Name]).ToList();
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
            var pls = GenDelParm(dtParams);
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
                ls.Add(new Dict { { "id", id } });
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
        /// 获取列定义
        /// </summary>
        /// <param name="p_colName"></param>
        /// <returns></returns>
        public TableCol GetColumn(string p_colName)
        {
            if (string.IsNullOrEmpty(p_colName))
                return null;

            if (Columns.TryGetValue(p_colName, out var col))
                return col;

            return (from c in PrimaryKey
                    where p_colName.Equals(c.Name, StringComparison.OrdinalIgnoreCase)
                    select c).FirstOrDefault();
        }

        /// <summary>
        /// 将纵向保存的列值转换成横向保存的列值。
        /// </summary>
        /// <param name="p_parm">dict[string, List`Cell`]</param>
        /// <returns></returns>
        List<Dict> GenInsertParm(Dict p_parm)
        {
            return GenParams(p_parm, (dt, key, cell) =>
            {
                dt[key] = cell.Val;
            });
        }

        List<Dict> GenOrclInsertParm(Dict p_parm)
        {
            return GenParams(p_parm, (dt, key, cell) =>
            {
                // oracle中 bool 类型需要转换成 1 或 0
                dt[key] = cell.Type == typeof(bool) ? ((bool)cell.Val ? "1" : "0") : cell.Val;
            });
        }

        List<Dict> GenUpdateParm(Dict p_parm)
        {
            return GenParams(p_parm, (dt, key, cell) =>
            {
                if (int.TryParse(key, out int index))
                {
                    // 更新的列值
                    dt[key] = cell.Val;
                }
                else
                {
                    // where后的主键，主键可能被更新
                    dt[key] = cell.OriginalVal;
                }
            });
        }

        List<Dict> GenOrclUpdateParm(Dict p_parm)
        {
            return GenParams(p_parm, (dt, key, cell) =>
            {
                if (int.TryParse(key, out int index))
                {
                    // oracle中 bool 类型需要转换成 1 或 0
                    dt[key] = cell.Type == typeof(bool) ? ((bool)cell.Val ? "1" : "0") : cell.Val;
                }
                else
                {
                    // where后的主键，主键可能被更新
                    dt[key] = cell.OriginalVal;
                }
            });
        }

        List<Dict> GenDelParm(Dict p_parm)
        {
            return GenParams(p_parm, (dt, key, cell) =>
            {
                // where后的主键，主键可能被更新
                dt[key] = cell.OriginalVal;
            });
        }

        List<Dict> GenParams(Dict p_parm, Action<Dict, string, Cell> p_callback)
        {
            if (p_parm == null || p_parm.Count == 0)
                return null;

            var first = p_parm.Values.First() as List<Cell>;
            if (first == null || first.Count == 0)
                return null;

            List<Dict> rtn = new List<Dict>();
            for (int i = 0; i < first.Count; i++)
            {
                Dict parm = new Dict();
                foreach (var item in p_parm)
                {
                    p_callback(parm, item.Key, ((List<Cell>)item.Value)[i]);
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
        public TableCol(TableSchema p_owner)
        {
            Owner = p_owner;
        }

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

        /// <summary>
        /// 所属表
        /// </summary>
        public TableSchema Owner { get; }

        /// <summary>
        /// 类中的属性名
        /// </summary>
        public string GetPropertyName()
        {
            if (IsChinessName)
                return Name;

            string name = "";
            var arr = Name.ToLower().Split('_');
            for (int i = 0; i < arr.Length; i++)
            {
                var str = arr[i];
                if (str == "id")
                {
                    // id特殊
                    name += "ID";
                }
                else
                {
                    name += char.ToUpper(str[0]) + str.Substring(1);
                }
            }
            return name;
        }

        /// <summary>
        /// 列类型的名称
        /// </summary>
        /// <returns></returns>
        public string GetTypeName()
        {
            return IsEnumCol ? GetEnumName() : GetTypeNameStr(Type);
        }

        /// <summary>
        /// 该列是否为枚举类型
        /// </summary>
        public bool IsEnumCol
        {
            get
            {
                bool isEnumType = false;
                Type tp = Type;
                if (Type.IsGenericType && Type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    tp = Type.GetGenericArguments()[0];

                switch (Owner.DbType)
                {
                    case DatabaseType.MySql:
                    case DatabaseType.SqlServer:
                        isEnumType = tp == typeof(byte);
                        break;

                    case DatabaseType.Oracle:
                    case DatabaseType.PostgreSql:
                        isEnumType = tp == typeof(short);
                        break;

                }

                return isEnumType
                    && !string.IsNullOrEmpty(Comments)
                    && Regex.IsMatch(Comments, @"^#[^\s#]+");
            }
        }

        /// <summary>
        /// 枚举类型名
        /// </summary>
        /// <returns></returns>
        public string GetEnumName()
        {
            if (!string.IsNullOrEmpty(Comments))
            {
                var match = Regex.Match(Comments, @"^#[^\s#]+");
                if (match.Success)
                {
                    var name = match.Value.Trim('#');
                    if (Type.IsGenericType && Type.GetGenericTypeDefinition() == typeof(Nullable<>))
                        name += "?";
                    return name;
                }
            }
            return "byte";
        }

        /// <summary>
        /// 字段名是否为中文
        /// </summary>
        /// <returns></returns>
        public bool IsChinessName
        {
            get
            {
                foreach (char vChar in Name)
                {
                    if (vChar > 255)
                        return true;
                }
                return false;
            }
        }

        string GetTypeNameStr(Type p_type)
        {
            if (p_type.IsGenericType && p_type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return GetTypeNameStr(p_type.GetGenericArguments()[0]) + "?";

            if (p_type == typeof(string))
                return "string";
            if (p_type == typeof(bool))
                return "bool";
            if (p_type == typeof(int))
                return "int";
            if (p_type == typeof(long))
                return "long";
            if (p_type == typeof(short))
                return "short";
            if (p_type == typeof(double))
                return "double";
            if (p_type == typeof(float))
                return "float";
            if (p_type == typeof(byte))
                return "byte";
            if (p_type == typeof(sbyte))
                return "sbyte";
            return p_type.Name;
        }
    }

    public class TableColList : KeyedCollection<string, TableCol>
    {
        /// <summary>
        /// 构造方法，键比较时忽略大小写
        /// </summary>
        public TableColList()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        /// <summary>
        /// 根据数据列获得列字段名
        /// </summary>
        /// <param name="item">数据列</param>
        /// <returns>列字段名</returns>
        protected override string GetKeyForItem(TableCol item)
        {
            return item.Name;
        }
    }

    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DatabaseType
    {
        MySql,
        Oracle,
        SqlServer,
        PostgreSql,
        Sqlite
    }
}
