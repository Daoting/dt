#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Core.Sqlite
{
    class SqliteCommandEx : SqliteCommand
    {
        #region 构造方法
        public SqliteCommandEx()
        {
        }

        public SqliteCommandEx(string commandText)
            : base(commandText)
        {
        }

        public SqliteCommandEx(string commandText, SqliteConnection connection)
            : base(commandText, connection)
        {
        }

        public SqliteCommandEx(string commandText, SqliteConnection connection, SqliteTransaction transaction)
            : base(commandText, connection, transaction)
        {
        }
        #endregion

        #region 返回Table
        /// <summary>
        /// 执行查询，返回数据集
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public Table<TEntity> ExecuteQuery<TEntity>()
            where TEntity : Entity
        {
            Table<TEntity> tbl = new Table<TEntity>();
            var map = SqliteConnectionEx.GetMapping(typeof(TEntity));

            // 列定义
            foreach (var col in map.Columns)
            {
                tbl.Add(col.Name, col.ColumnType);
            }

            var reader = ExecuteReader();
            if (reader != null && reader.FieldCount > 0)
            {
                while (reader.Read())
                {
                    // 无参数构造方法可能为private，如实体类型
                    var row = (TEntity)Activator.CreateInstance(typeof(TEntity), true);
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var name = reader.GetName(i);
                        var col = map.FindColumn(name);

                        if (col == null)
                        {
                            // 映射类型中不存在该属性，使用数据的实际类型
                            Cell cell;
                            var tp = reader.GetFieldType(i);
                            if (reader.IsDBNull(i))
                            {
                                // 列为可空类型时重置，因sqlie无可空类型
                                if (tp == typeof(byte[]))
                                {
                                    // 返回结果的首行数据出现dbnull时列类型不正确！
                                    if (tbl.Count == 0)
                                        tp = typeof(string);
                                }
                                else if (tp.IsValueType && Nullable.GetUnderlyingType(tp) == null)
                                {
                                    tp = typeof(Nullable<>).MakeGenericType(tp);
                                }
                                cell = new Cell(row, name, tp);
                            }
                            else
                            {
                                cell = new Cell(row, name, tp, reader.GetValue(i));
                            }

                            // 补充缺少的列
                            if (!tbl.Columns.Contains(name))
                                tbl.Add(name, cell.Type);
                        }
                        else
                        {
                            // 有映射属性的以属性类型为准，因sqlie数据类型少，无可空类型、bool、DateTime等
                            if (reader.IsDBNull(i))
                                new Cell(row, name, col.ColumnType);
                            else
                                new Cell(row, name, col.ColumnType, reader.GetValue(i));
                        }
                    }
                    tbl.Add(row);
                }
            }
            return tbl;
        }

        /// <summary>
        /// 执行查询，返回数据集
        /// </summary>
        /// <returns></returns>
        public Table ExecuteQuery()
        {
            Table tbl = new Table();
            var reader = ExecuteReader();
            if (reader != null && reader.FieldCount > 0)
            {
                // 列定义
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    // Microsoft.Data.Sqlite 升级5.0.3后使用 GetFieldType 方法，因通过GetDataTypeName无bigint类型！
                    // 返回结果在表的首行数据出现dbnull的时候不正确！
                    // 原方法参见ComposeDt
                    tbl.Add(reader.GetName(i), reader.GetFieldType(i));
                }

                while (reader.Read())
                {
                    var row = new Row();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var col = tbl.Columns[i];
                        if (reader.IsDBNull(i))
                        {
                            // 列为可空类型时重置，因sqlie无可空类型
                            if (col.Type == typeof(byte[]))
                            {
                                // 返回结果的首行数据出现dbnull时列类型不正确！
                                if (tbl.Count == 0)
                                    col.Type = typeof(string);
                            }
                            else if (col.Type.IsValueType && Nullable.GetUnderlyingType(col.Type) == null)
                            {
                                col.Type = typeof(Nullable<>).MakeGenericType(col.Type);
                            }
                            new Cell(row, col.ID, col.Type);
                        }
                        else
                        {
                            new Cell(row, col.ID, col.Type, reader.GetValue(i));
                        }
                    }
                    tbl.Add(row);
                }
            }
            return tbl;
        }
        #endregion

        /// <summary>
        /// 延时返回查询对象，利于性能提升
        /// </summary>
        /// <typeparam name="TRow"></typeparam>
        /// <returns></returns>
        public IEnumerable<TRow> ForEach<TRow>()
            where TRow : Row
        {
            var reader = ExecuteReader();
            if (reader != null && reader.FieldCount > 0)
            {
                var map = typeof(TRow).IsSubclassOf(typeof(Entity)) ? SqliteConnectionEx.GetMapping(typeof(TRow)) : null;
                while (reader.Read())
                {
                    // 无参数构造方法可能为private，如实体类型
                    var row = (TRow)Activator.CreateInstance(typeof(TRow), true);
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (map == null)
                        {
                            // Row
                            var tp = reader.GetFieldType(i);
                            if (reader.IsDBNull(i))
                            {
                                // sqlite中无可空类型，只能遇到DBNull按可空类型，会造成所有Row的列类型不同！
                                // 列为可空类型时重置，因sqlie无可空类型
                                if (tp == typeof(byte[]))
                                {
                                    // 返回结果的首行数据出现dbnull时列类型不正确！
                                    tp = typeof(string);
                                }
                                else if (tp.IsValueType && Nullable.GetUnderlyingType(tp) == null)
                                {
                                    tp = typeof(Nullable<>).MakeGenericType(tp);
                                }
                                new Cell(row, reader.GetName(i), tp);
                            }
                            else
                            {
                                new Cell(row, reader.GetName(i), tp, reader.GetValue(i));
                            }
                        }
                        else
                        {
                            // Entity
                            var name = reader.GetName(i);
                            var col = map.FindColumn(name);

                            if (col == null)
                            {
                                // 映射类型中不存在该属性，使用数据的实际类型
                                var tp = reader.GetFieldType(i);
                                if (reader.IsDBNull(i))
                                {
                                    // 列为可空类型时重置，因sqlie无可空类型
                                    if (tp == typeof(byte[]))
                                    {
                                        // 返回结果的首行数据出现dbnull时列类型不正确！
                                        tp = typeof(string);
                                    }
                                    else if (tp.IsValueType && Nullable.GetUnderlyingType(tp) == null)
                                    {
                                        tp = typeof(Nullable<>).MakeGenericType(tp);
                                    }
                                    new Cell(row, name, tp);
                                }
                                else
                                {
                                    new Cell(row, name, tp, reader.GetValue(i));
                                }
                            }
                            else
                            {
                                // 有映射属性的以属性类型为准，因sqlie数据类型少，无可空类型、bool、DateTime等
                                if (reader.IsDBNull(i))
                                    new Cell(row, name, col.ColumnType);
                                else
                                    new Cell(row, name, col.ColumnType, reader.GetValue(i));
                            }
                        }
                    }
                    yield return row;
                }
            }
        }

        /// <summary>
        /// 取得查询数据库中单个值。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ExecuteScalar<T>()
        {
            var reader = ExecuteReader();
            if (reader != null
                && reader.FieldCount > 0
                && reader.Read())
            {
                if (reader.GetFieldType(0) == typeof(T))
                    return reader.GetFieldValue<T>(0);

                return (T)Convert.ChangeType(reader.GetValue(0), typeof(T));
            }
            return default(T);
        }

        /// <summary>
        /// 返回第一列数据，并转换为指定类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> GetFirstCol<T>()
        {
            List<T> ls = new List<T>();
            var reader = ExecuteReader();
            if (reader != null && reader.FieldCount > 0)
            {
                if (reader.GetFieldType(0) == typeof(T))
                {
                    while (reader.Read())
                    {
                        ls.Add(reader.GetFieldValue<T>(0));
                    }
                }
                else
                {
                    while (reader.Read())
                    {
                        ls.Add((T)Convert.ChangeType(reader.GetValue(0), typeof(T)));
                    }
                }
            }
            return ls;
        }

        /// <summary>
        /// 返回第一列枚举，高性能
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> EachFirstCol<T>()
        {
            var reader = ExecuteReader();
            if (reader != null && reader.FieldCount > 0)
            {
                if (reader.GetFieldType(0) == typeof(T))
                {
                    while (reader.Read())
                    {
                        yield return reader.GetFieldValue<T>(0);
                    }
                }
                else
                {
                    while (reader.Read())
                    {
                        yield return (T)Convert.ChangeType(reader.GetValue(0), typeof(T));
                    }
                }
            }
        }

        /*
        /// <summary>
        /// 根据需要返回的类型，查询数据库中的值，以object的形式返回
        /// </summary>
        /// <param name="p_dataReader"></param>
        /// <param name="p_ordinal">数据列序号</param>
        /// <param name="p_type">要取得的数据类型</param>
        /// <returns></returns>
        object GetFieldValue(SqliteDataReader p_dataReader, int p_ordinal, Type p_type)
        {
            if (p_dataReader.IsDBNull(p_ordinal) && (!p_type.IsValueType || (p_type.IsGenericType && p_type.GetGenericTypeDefinition() == typeof(Nullable<>))))
                return null;

            // 由于上一句排除了dbnull的情况，此时用GetFieldType方法会获得正确结果。
            if (p_dataReader.GetFieldType(p_ordinal) == p_type)
                return p_dataReader[p_ordinal];

            // 先把null的情况返回，然后枚举型变成基本类型，然后把基本类型由可为null变为不可为null的类型，然后取值，把string，int，long，bool放在前面提高性能。
            Type type = p_type.GetTypeInfo().IsEnum ? Enum.GetUnderlyingType(p_type) : p_type;
            type = Nullable.GetUnderlyingType(type) ?? type;
            if (type == typeof(int))
            {
                return p_dataReader.GetInt32(p_ordinal);
            }
            if (type == typeof(string))
            {
                return p_dataReader.GetString(p_ordinal);
            }
            if (type == typeof(long))
            {
                return p_dataReader.GetInt64(p_ordinal);
            }
            if (type == typeof(bool))
            {
                return p_dataReader.GetBoolean(p_ordinal);
            }
            if (type == typeof(byte))
            {
                return p_dataReader.GetByte(p_ordinal);
            }
            if (type == typeof(byte[]))
            {
                //return p_dataReader.GetBlob(ordinal);getBlob不可访问。
                // Microsoft.Data.Sqlite升级到3.1.5后
                return raw.sqlite3_column_blob(p_dataReader.Handle, p_ordinal).ToArray();
            }
            if (type == typeof(char))
            {
                return p_dataReader.GetChar(p_ordinal);
            }
            if (type == typeof(DateTime))
            {
                return p_dataReader.GetDateTime(p_ordinal);
            }
            if (type == typeof(DateTimeOffset))
            {
                return p_dataReader.GetDateTimeOffset(p_ordinal);
            }
            if (type == typeof(decimal))
            {
                return p_dataReader.GetDecimal(p_ordinal);
            }
            if (type == typeof(double))
            {
                return p_dataReader.GetDouble(p_ordinal);
            }
            if (type == typeof(float))
            {
                return p_dataReader.GetFloat(p_ordinal);
            }
            if (type == typeof(Guid))
            {
                return p_dataReader.GetGuid(p_ordinal);
            }
            if (type == typeof(sbyte))
            {
                return ((sbyte)p_dataReader.GetInt64(p_ordinal));
            }
            if (type == typeof(short))
            {
                return p_dataReader.GetInt16(p_ordinal);
            }
            if (type == typeof(TimeSpan))
            {
                if (p_dataReader.GetFieldType(p_ordinal) == typeof(string))
                {
                    return TimeSpan.Parse(p_dataReader.GetString(p_ordinal));
                }
                else
                {
                    return TimeSpan.FromDays(p_dataReader.GetDouble(p_ordinal));
                }
            }
            if (type == typeof(uint))
            {
                return ((uint)p_dataReader.GetInt64(p_ordinal));
            }
            if (type == typeof(ulong))
            {
                return ((ulong)p_dataReader.GetInt64(p_ordinal));
            }
            if (type == typeof(ushort))
            {
                return ((ushort)p_dataReader.GetInt64(p_ordinal));
            }
            return p_dataReader.GetValue(p_ordinal);
        }
        
        /// <summary>
        /// 只返回第一行
        /// </summary>
        /// <returns></returns>
        public Row GetFirstRow()
        {
            Row dr = null;
            SqliteDataReader dataReader = ExecuteReader();
            if (dataReader != null && dataReader.FieldCount > 0)
            {
                Table dt = ComposeDt(dataReader);
                if (dataReader.Read())
                {
                    dr = dt.AddRow();
                    dr.IsAdded = false;
                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        dr.Cells[i].InitVal(dataReader[i] == DBNull.Value ? null : dataReader[i]);
                    }
                }
            }
            return dr;
        }

        /// <summary>
        /// 根据类型（class）T的公共属性填充Dictionary
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        Dictionary<String, PropertyInfo> GetDataColumns<T>(SqliteDataReader p_dataReader) where T : class
        {
            if (p_dataReader == null || p_dataReader.FieldCount == 0)
                return null;

            Dictionary<String, PropertyInfo> dataColumns = new Dictionary<String, PropertyInfo>();
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
            for (int i = 0; i < p_dataReader.FieldCount; i++)
            {
                string name = p_dataReader.GetName(i);
                var prop = (from item in props
                            where item.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
                            select item).FirstOrDefault();
                if (prop != null)
                    dataColumns.Add(name, prop);
            }
            return dataColumns;
        }

        /// <summary>
        /// 根据SqliteDataReader的结果字段生成Table的列结构信息.
        /// 数据列的类型与tablemapping中创建表时候的数据列类型保持一致，服务器端的tablemapping和客户端的是一致的。
        /// </summary>
        /// <param name="p_dr"></param>
        /// <returns></returns>
        Table ComposeDt(SqliteDataReader p_dr)
        {
            Table dt = new Table();
            var cols = dt.Columns;
            for (int i = 0; i < p_dr.FieldCount; i++)
            {
                // Microsoft.Data.Sqlite 升级5.0.3后使用 GetFieldType 方法，因通过GetDataTypeName无bigint类型！
                cols.Add(new Column(p_dr.GetName(i), p_dr.GetFieldType(i)));

                // 程序中一般情况下禁止使用SqliteDataReader 的 GetFieldType方法，返回结果在表的首行数据出现dbnull的时候不正确。
                //string colTypeName = p_dr.GetDataTypeName(i).ToLower();
                //switch (colTypeName)
                //{
                //    case "integer":
                //        cols.Add(new Column(p_dr.GetName(i), typeof(int)));
                //        break;
                //    case "bigint":
                //        cols.Add(new Column(p_dr.GetName(i), typeof(long)));
                //        break;
                //    case "float":
                //    case "real":
                //        cols.Add(new Column(p_dr.GetName(i), typeof(double)));
                //        break;
                //    case "text":
                //    case "varchar":
                //        cols.Add(new Column(p_dr.GetName(i), typeof(string)));
                //        break;
                //    case "datetime":
                //        cols.Add(new Column(p_dr.GetName(i), typeof(DateTime)));
                //        break;
                //    case "blob":
                //        cols.Add(new Column(p_dr.GetName(i), typeof(byte[])));
                //        break;
                //    default:
                //        throw new Exception("意外的数据类型。");
                //}
            }
            return dt;
        }
        */
    }
}
