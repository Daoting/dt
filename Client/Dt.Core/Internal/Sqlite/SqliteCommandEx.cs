#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.Data.Sqlite;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#endregion

namespace Dt.Core.Sqlite
{
    public class SqliteCommandEx : SqliteCommand
    {
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

        /// <summary>
        /// 执行查询，返回类型List。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> ExecuteQuery<T>() where T : class
        {
            List<T> rtn = new List<T>();
            SqliteDataReader dataReader = ExecuteReader();
            Dictionary<String, PropertyInfo> dataColumns = GetDataColumns<T>(dataReader);

            if (dataColumns.Count > 0)
            {
                while (dataReader.Read())
                {
                    T temp = Activator.CreateInstance<T>();
                    int colIndex = 0;
                    foreach (var item in dataColumns)
                    {
                        colIndex = dataReader.GetOrdinal(item.Key);
                        item.Value.SetValue(temp, GetFieldValue(dataReader, colIndex, item.Value.PropertyType));
                    }
                    rtn.Add(temp);
                }
            }
            return rtn;
        }

        /// <summary>
        /// 延时返回查询对象，利于性能提升。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> DeferredQuery<T>() where T : class
        {
            SqliteDataReader dataReader = ExecuteReader();
            Dictionary<String, PropertyInfo> dataColumns = GetDataColumns<T>(dataReader);
            if (dataColumns.Count > 0)
            {
                while (dataReader.Read())
                {
                    T temp = Activator.CreateInstance<T>();
                    int colIndex = 0;
                    foreach (var item in dataColumns)
                    {
                        colIndex = dataReader.GetOrdinal(item.Key);
                        item.Value.SetValue(temp, GetFieldValue(dataReader, colIndex, item.Value.PropertyType));
                    }
                    yield return temp;
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
            SqliteDataReader dataReader = ExecuteReader();
            if (dataReader.Read())
            {
                var rtn = GetFieldValue(dataReader, 0, typeof(T));
                if (rtn == null)
                    return default(T);
                return (T)rtn;
            }
            return default(T);
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <returns></returns>
        public Table ExecuteQuery()
        {
            Table dt = null;
            SqliteDataReader dataReader = ExecuteReader();
            if (dataReader != null && dataReader.FieldCount > 0)
            {
                Row tmpDr = null;
                dt = ComposeDt(dataReader);
                while (dataReader.Read())
                {
                    tmpDr = dt.NewRow();
                    tmpDr.IsAdded = false;
                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        tmpDr.Cells[i].InitVal(dataReader[i] == DBNull.Value ? null : dataReader[i]);
                    }
                }
            }
            return dt;
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
                    dr = dt.NewRow();
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
                return raw.sqlite3_column_blob(p_dataReader.Handle, p_ordinal);
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
                if (p_dataReader.GetDataTypeName(p_ordinal).ToLower() == "varchar")
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
        /// 根据SqliteDataReader的结果字段生成Table的列结构信息.
        /// 数据列的类型与tablemapping中创建表时候的数据列类型保持一致，服务器端的tablemapping和客户端
        /// 的是一致的。程序中一般情况下禁止使用SqliteDataReader 的 GetFieldType方法，返回结果在表的首行数据出现dbnull的时候不正确。
        /// </summary>
        /// <param name="p_dr"></param>
        /// <returns></returns>
        Table ComposeDt(SqliteDataReader p_dr)
        {
            Table dt = new Table();
            var cols = dt.Columns;
            for (int i = 0; i < p_dr.FieldCount; i++)
            {
                string colTypeName = p_dr.GetDataTypeName(i).ToLower();
                switch (colTypeName)
                {
                    case "integer":
                        cols.Add(new Column(p_dr.GetName(i), typeof(int)));
                        break;
                    case "bigint":
                        cols.Add(new Column(p_dr.GetName(i), typeof(long)));
                        break;
                    case "float":
                    case "real":
                        cols.Add(new Column(p_dr.GetName(i), typeof(double)));
                        break;
                    case "text":
                    case "varchar":
                        cols.Add(new Column(p_dr.GetName(i), typeof(string)));
                        break;
                    case "datetime":
                        cols.Add(new Column(p_dr.GetName(i), typeof(DateTime)));
                        break;
                    case "blob":
                        cols.Add(new Column(p_dr.GetName(i), typeof(byte[])));
                        break;
                    default:
                        throw new Exception("意外的数据类型。");
                }
            }
            return dt;
        }
    }
}
