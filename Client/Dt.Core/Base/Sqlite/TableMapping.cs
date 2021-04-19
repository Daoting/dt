#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
#endregion

namespace Dt.Core.Sqlite
{
    /// <summary>
    /// 表与类的映射ORM
    /// </summary>
    class TableMapping
    {
        Type _type;
        string _tableName;
        Column _autoPk = null;
        string _sqlInsert;
        string _sqlInsertOrUpdate;
        string _sqlGetByPK;

        public TableMapping(Type type)
        {
            _type = type;
            _tableName = _type.Name;

            // 不包括继承的属性
            var props = _type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty | BindingFlags.DeclaredOnly);
            var cols = new List<Column>();
            var insertCols = new List<Column>();
            foreach (var p in props)
            {
                if (!p.CanWrite || p.GetCustomAttribute<IgnoreAttribute>(false) != null)
                    continue;

                var c = new Column(p);
                cols.Add(c);
                if (!c.IsAutoInc)
                    insertCols.Add(c);

                if (c.IsAutoInc && c.IsPK)
                {
                    _autoPk = c;
                }
                if (c.IsPK)
                {
                    PK = c;
                }
            }
            Columns = cols;
            InsertColumns = insertCols;
        }

        /// <summary>
        /// 表名
        /// </summary>
        public string TableName
        {
            get { return _tableName; }
        }

        /// <summary>
        /// 所有列
        /// </summary>
        public List<Column> Columns { get; }

        /// <summary>
        /// 主键列
        /// </summary>
        public Column PK { get; set; }

        /// <summary>
        /// 根据主键列获取数据的sql
        /// </summary>
        public string SqlGetByPK
        {
            get
            {
                if (_sqlGetByPK == null)
                {
                    if (PK != null)
                        _sqlGetByPK = $"select * from \"{_tableName}\" where \"{PK.Name}\" = ?";
                    else
                        _sqlGetByPK = $"select * from \"{_tableName}\" limit 1";
                }
                return _sqlGetByPK;
            }
        }

        /// <summary>
        /// 是否有自增主键
        /// </summary>
        public bool HasAutoIncPK => _autoPk != null;

        /// <summary>
        /// 设置自增主键的值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="id"></param>
        public void SetAutoIncPK(object obj, long id)
        {
            if (_autoPk != null)
                _autoPk.SetValue(obj, Convert.ChangeType(id, _autoPk.ColumnType, null));
        }

        /// <summary>
        /// 获取除自增列外的所有列
        /// </summary>
        public List<Column> InsertColumns { get; }

        /// <summary>
        /// 查找列
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public Column FindColumnWithPropertyName(string propertyName)
        {
            var exact = Columns.Where(c => c.PropertyName == propertyName).FirstOrDefault();
            return exact;
        }

        /// <summary>
        /// 查找列
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public Column FindColumn(string columnName)
        {
            var exact = Columns.Where(c => c.Name == columnName).FirstOrDefault();
            return exact;
        }

        /// <summary>
        /// 获取create table语句
        /// </summary>
        /// <returns></returns>
        public string GetCreateSql()
        {
            string pk = "";
            bool autoIncPk = false;
            StringBuilder sb = new StringBuilder();
            sb.Append("create table if not exists ");
            sb.Append(_tableName);
            sb.Append(" (\n");
            foreach (var col in Columns)
            {
                sb.Append(SqlDecl(col));
                sb.Append(",\n");
                if (col.IsPK)
                {
                    if (col.IsAutoInc)
                    {
                        autoIncPk = true;
                    }
                    else
                    {
                        if (pk == "")
                            pk = col.Name;
                        else
                            pk += "," + col.Name;
                    }
                }
            }

            if (!autoIncPk && !string.IsNullOrEmpty(pk))
            {
                // 联合主键时必须放在语句最后，不允许有AutoInc列！
                sb.Append("primary key (");
                sb.Append(pk);
                sb.Append("))");
            }
            else
            {
                sb.Remove(sb.Length - 2, 2);
                sb.Append(")");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取insert语句
        /// </summary>
        /// <param name="p_autoUpdate">是否包含OR REPLACE</param>
        /// <returns></returns>
        public string GetInsertSql(bool p_autoUpdate)
        {
            if (p_autoUpdate)
            {
                if (string.IsNullOrEmpty(_sqlInsertOrUpdate))
                    _sqlInsertOrUpdate = CreateInsertSql(true);
                return _sqlInsertOrUpdate;
            }

            if (string.IsNullOrEmpty(_sqlInsert))
                _sqlInsert = CreateInsertSql(false);
            return _sqlInsert;
        }

        string CreateInsertSql(bool p_autoUpdate)
        {
            var cols = p_autoUpdate ? Columns : InsertColumns;
            return string.Format("insert {0} into {1} ({2}) values ({3})",
                p_autoUpdate ? "OR REPLACE" : "",
                _tableName,
                string.Join(",", (from c in cols select c.Name).ToArray()),
                string.Join(",", (from c in cols select ":" + c.Name).ToArray()));
        }

        public class Column
        {
            PropertyInfo _prop;

            public Column(PropertyInfo prop)
            {
                _prop = prop;
                Name = prop.Name;
                ColumnType = prop.PropertyType;
                IsPK = IsPK(prop);
                IsAutoInc = IsAutoInc(prop);
                Indices = GetIndices(prop);
                IsNullable = !IsPK;
            }

            /// <summary>
            /// 列名
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 属性名
            /// </summary>
            public string PropertyName { get { return _prop.Name; } }

            /// <summary>
            /// 列类型
            /// </summary>
            public Type ColumnType { get; set; }

            /// <summary>
            /// 是否为自增列
            /// </summary>
            public bool IsAutoInc { get; set; }

            /// <summary>
            /// 是否为主键列
            /// </summary>
            public bool IsPK { get; set; }

            /// <summary>
            /// 列的所有索引项
            /// </summary>
            public IEnumerable<IndexedAttribute> Indices { get; set; }

            /// <summary>
            /// 列是否可空
            /// </summary>
            public bool IsNullable { get; set; }

            public void SetValue(object obj, object val)
            {
                _prop.SetValue(obj, val, null);
            }

            public object GetValue(object obj)
            {
                return _prop.GetValue(obj) ?? DBNull.Value;
            }
        }

        /// <summary>
        /// 获取创建表时列的描述串
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static string SqlDecl(Column p)
        {
            string decl = p.Name + " " + SqlType(p);

            // 联合主键时必须放在语句最后！
            if (p.IsPK && p.IsAutoInc)
            {
                decl += " primary key autoincrement";
            }
            if (!p.IsNullable)
            {
                decl += " not null";
            }
            return decl;
        }

        /// <summary>
        /// 获取列的数据类型
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static string SqlType(Column p)
        {
            var clrType = p.ColumnType;
            if (clrType == typeof(string) || clrType == typeof(DateTime))
                return "text";

            if (clrType == typeof(Boolean)
                || clrType == typeof(Byte)
                || clrType == typeof(UInt16)
                || clrType == typeof(SByte)
                || clrType == typeof(Int16)
                || clrType == typeof(Int32)
                || clrType == typeof(UInt32)
                || clrType == typeof(Int64)
                || clrType.IsEnum)
                return "integer";

            if (clrType == typeof(Single) || clrType == typeof(Double) || clrType == typeof(Decimal))
                return "real";

            if (clrType == typeof(byte[]))
                return "blob";

            return "text";
        }

        /// <summary>
        /// 是否为主键列
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool IsPK(MemberInfo p)
        {
            return p.GetCustomAttribute<PrimaryKeyAttribute>(false) != null;
        }

        /// <summary>
        /// 是否为自增列
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool IsAutoInc(MemberInfo p)
        {
            return p.GetCustomAttribute<AutoIncrementAttribute>(false) != null;
        }

        /// <summary>
        /// 获取列的所有索引项
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static IEnumerable<IndexedAttribute> GetIndices(MemberInfo p)
        {
            return p.GetCustomAttributes<IndexedAttribute>(false);
        }
    }
}
