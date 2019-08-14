#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-11-30 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#endregion

namespace Dt.Cm.Sqlite
{
    /// <summary>
    /// 表与类的映射ORM
    /// </summary>
    public class TableMapping
    {
        Type _type;
        string _tableName;
        Column _autoPk = null;
        Column[] _insertColumns = null;
        string _insertSql;

        public TableMapping(Type type)
        {
            _type = type;
            var attr = type.GetCustomAttribute<TableAttribute>(false);
            _tableName = (attr != null && !string.IsNullOrEmpty(attr.Name)) ? attr.Name : _type.Name;

            var props = _type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
            var cols = new List<Column>();
            foreach (var p in props)
            {
                if (p.CanWrite && p.GetCustomAttribute<IgnoreAttribute>(false) == null)
                {
                    cols.Add(new Column(p));
                }
            }
            Columns = cols.ToArray();
            foreach (var c in Columns)
            {
                if (c.IsAutoInc && c.IsPK)
                {
                    _autoPk = c;
                }
                if (c.IsPK)
                {
                    PK = c;
                }
            }

            HasAutoIncPK = _autoPk != null;

            if (PK != null)
            {
                GetByPrimaryKeySql = $"select * from \"{_tableName}\" where \"{PK.Name}\" = ?";
            }
            else
            {
                GetByPrimaryKeySql = $"select * from \"{_tableName}\" limit 1";
            }
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
        public Column[] Columns { get; set; }

        /// <summary>
        /// 主键列
        /// </summary>
        public Column PK { get; set; }

        /// <summary>
        /// 根据主键列获取数据的sql
        /// </summary>
        public string GetByPrimaryKeySql { get; set; }

        /// <summary>
        /// 是否有自增主键
        /// </summary>
        public bool HasAutoIncPK { get; set; }

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
        public Column[] InsertColumns
        {
            get
            {
                if (_insertColumns == null)
                    _insertColumns = Columns.Where(c => !c.IsAutoInc).ToArray();
                return _insertColumns;
            }
        }

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
            var decls = Columns.Select(p => SqlDecl(p, false));
            var decl = string.Join(",\n", decls.ToArray());
            return $"create table if not exists {_tableName} (\n{decl})";
        }

        /// <summary>
        /// 获取insert语句
        /// </summary>
        /// <returns></returns>
        public string GetInsertSql()
        {
            if (string.IsNullOrEmpty(_insertSql))
                _insertSql = CreateInsertSql();
            return _insertSql;
        }

        string CreateInsertSql()
        {
            var cols = InsertColumns;
            return string.Format("insert into {0} ({1}) values ({2})",
                _tableName,
                string.Join(",", (from c in cols select c.Name).ToArray()),
                string.Join(",", (from c in cols select ":" + c.Name).ToArray()));
        }
        
        public class Column
        {
            PropertyInfo _prop;

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
            /// 列的比较函数
            /// </summary>
            public string Collation { get; set; }

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

            /// <summary>
            /// 列的最大长度
            /// </summary>
            public int MaxStringLength { get; set; }

            public Column(PropertyInfo prop)
            {
                _prop = prop;
                var colAttr = prop.GetCustomAttribute<ColumnAttribute>(false);
                Name = (colAttr != null && !string.IsNullOrEmpty(colAttr.Name)) ? colAttr.Name : prop.Name;
                ColumnType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                Collation = Collation(prop);
                IsAutoInc = IsAutoInc(prop);
                IsPK = IsPK(prop);
                Indices = GetIndices(prop);
                IsNullable = !IsPK;
                MaxStringLength = MaxStringLength(prop);
            }

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
        /// <param name="storeDateTimeAsTicks"></param>
        /// <returns></returns>
        public static string SqlDecl(Column p, bool storeDateTimeAsTicks)
        {
            string decl = p.Name + " " + SqlType(p, storeDateTimeAsTicks);

            if (p.IsPK)
            {
                decl += " primary key";
            }
            if (p.IsAutoInc)
            {
                decl += " autoincrement";
            }
            if (!p.IsNullable)
            {
                decl += " not null";
            }
            if (!string.IsNullOrEmpty(p.Collation))
            {
                decl += " collate " + p.Collation;
            }
            return decl;
        }

        /// <summary>
        /// 获取列的数据类型
        /// </summary>
        /// <param name="p"></param>
        /// <param name="storeDateTimeAsTicks"></param>
        /// <returns></returns>
        public static string SqlType(Column p, bool storeDateTimeAsTicks)
        {
            var clrType = p.ColumnType;
            if (clrType == typeof(Boolean) || clrType == typeof(Byte) || clrType == typeof(UInt16) || clrType == typeof(SByte) || clrType == typeof(Int16) || clrType == typeof(Int32))
                return "integer";

            if (clrType == typeof(UInt32) || clrType == typeof(Int64))
                return "bigint";

            if (clrType == typeof(Single) || clrType == typeof(Double) || clrType == typeof(Decimal))
                return "float";

            if (clrType == typeof(string))
            {
                int len = p.MaxStringLength;
                return "varchar(" + len + ")";
            }

            if (clrType == typeof(DateTime))
                return storeDateTimeAsTicks ? "bigint" : "datetime";

            if (clrType.IsEnum)
                return "integer";

            if (clrType == typeof(byte[]))
                return "blob";
            throw new NotSupportedException("未知的类型 " + clrType);
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
        /// 获取列的比较函数
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static string Collation(MemberInfo p)
        {
            var attr = p.GetCustomAttribute<CollationAttribute>(false);
            return (attr != null && !string.IsNullOrEmpty(attr.Value)) ? attr.Value : string.Empty;
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

        /// <summary>
        /// 获取列的最大长度
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static int MaxStringLength(PropertyInfo p)
        {
            var attrs = p.GetCustomAttribute<MaxLengthAttribute>(false);
            return (attrs != null && attrs.Value > 0) ? attrs.Value : 140;
        }
    }
}
