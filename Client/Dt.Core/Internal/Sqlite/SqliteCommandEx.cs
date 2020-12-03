#region �ļ�����
/******************************************************************************
* ����: Daoting
* ժҪ: 
* ��־: 2017-12-06 ����
******************************************************************************/
#endregion

#region ��������
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
        /// ִ�в�ѯ����������List��
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
        /// ��ʱ���ز�ѯ������������������
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
        /// ȡ�ò�ѯ���ݿ��е���ֵ��
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
        /// ִ������
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
                    tmpDr = dt.AddRow();
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
        /// ֻ���ص�һ��
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
        /// ���ص�һ�����ݣ���ת��Ϊָ������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> GetFirstCol<T>()
        {
            List<T> rtn = new List<T>();
            SqliteDataReader dataReader = ExecuteReader();
            if (dataReader != null && dataReader.FieldCount > 0)
            {
                while (dataReader.Read())
                {
                    var val = GetFieldValue(dataReader, 0, typeof(T));
                    if (val == null)
                        rtn.Add(default(T));
                    else
                        rtn.Add((T)val);
                }
            }
            return rtn;
        }

        /// <summary>
        /// �������ͣ�class��T�Ĺ����������Dictionary
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
        /// ������Ҫ���ص����ͣ���ѯ���ݿ��е�ֵ����object����ʽ����
        /// </summary>
        /// <param name="p_dataReader"></param>
        /// <param name="p_ordinal">���������</param>
        /// <param name="p_type">Ҫȡ�õ���������</param>
        /// <returns></returns>
        object GetFieldValue(SqliteDataReader p_dataReader, int p_ordinal, Type p_type)
        {
            if (p_dataReader.IsDBNull(p_ordinal) && (!p_type.IsValueType || (p_type.IsGenericType && p_type.GetGenericTypeDefinition() == typeof(Nullable<>))))
                return null;
            
            // ������һ���ų���dbnull���������ʱ��GetFieldType����������ȷ�����
            if (p_dataReader.GetFieldType(p_ordinal) == p_type)
                return p_dataReader[p_ordinal];

            // �Ȱ�null��������أ�Ȼ��ö���ͱ�ɻ������ͣ�Ȼ��ѻ��������ɿ�Ϊnull��Ϊ����Ϊnull�����ͣ�Ȼ��ȡֵ����string��int��long��bool����ǰ��������ܡ�
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
                //return p_dataReader.GetBlob(ordinal);getBlob���ɷ��ʡ�
                // Microsoft.Data.Sqlite������3.1.5��
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
        /// ����SqliteDataReader�Ľ���ֶ�����Table���нṹ��Ϣ.
        /// �����е�������tablemapping�д�����ʱ������������ͱ���һ�£��������˵�tablemapping�Ϳͻ���
        /// ����һ�µġ�������һ������½�ֹʹ��SqliteDataReader �� GetFieldType���������ؽ���ڱ���������ݳ���dbnull��ʱ����ȷ��
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
                        throw new Exception("������������͡�");
                }
            }
            return dt;
        }
    }
}
