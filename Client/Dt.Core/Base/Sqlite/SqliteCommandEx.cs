#region �ļ�����
/******************************************************************************
* ����: Daoting
* ժҪ: 
* ��־: 2017-12-06 ����
******************************************************************************/
#endregion

#region ��������
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Core.Sqlite
{
    class SqliteCommandEx : SqliteCommand
    {
        #region ���췽��
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

        #region ����Table
        /// <summary>
        /// ִ�в�ѯ���������ݼ�
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Table<TEntity> ExecuteQuery<TEntity>()
            where TEntity : Entity
        {
            Table<TEntity> tbl = new Table<TEntity>();
            var map = SqliteConnectionEx.GetMapping(typeof(TEntity));

            // �ж���
            foreach (var col in map.Columns)
            {
                tbl.Add(col.Name, col.ColumnType);
            }

            var reader = ExecuteReader();
            if (reader != null && reader.FieldCount > 0)
            {
                while (reader.Read())
                {
                    // �޲������췽������Ϊprivate����ʵ������
                    var row = (TEntity)Activator.CreateInstance(typeof(TEntity), true);
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var name = reader.GetName(i);
                        var col = map.FindColumn(name);

                        if (col == null)
                        {
                            // ӳ�������в����ڸ����ԣ�ʹ�����ݵ�ʵ������
                            Cell cell;
                            if (reader.IsDBNull(i))
                                cell = new Cell(row, name, typeof(Nullable<>).MakeGenericType(reader.GetFieldType(i)));
                            else
                                cell = new Cell(row, name, reader.GetFieldType(i), reader.GetValue(i));

                            // ����ȱ�ٵ���
                            if (!tbl.Columns.Contains(name))
                                tbl.Add(name, cell.Type);
                        }
                        else
                        {
                            // ��ӳ�����Ե�����������Ϊ׼����sqlie���������٣��޿ɿ����͡�bool��DateTime��
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
        /// ִ�в�ѯ���������ݼ�
        /// </summary>
        /// <returns></returns>
        public Table ExecuteQuery()
        {
            Table tbl = new Table();
            var reader = ExecuteReader();
            if (reader != null && reader.FieldCount > 0)
            {
                // �ж���
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    // Microsoft.Data.Sqlite ����5.0.3��ʹ�� GetFieldType ��������ͨ��GetDataTypeName��bigint���ͣ�
                    // ԭ�����μ�ComposeDt
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
                            // ��Ϊ�ɿ�����ʱ���ã���sqlie�޿ɿ�����
                            if (Nullable.GetUnderlyingType(col.Type) == null)
                                col.Type = typeof(Nullable<>).MakeGenericType(col.Type);
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
        /// ��ʱ���ز�ѯ����������������
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
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
                    // �޲������췽������Ϊprivate����ʵ������
                    var row = (TRow)Activator.CreateInstance(typeof(TRow), true);
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (map == null)
                        {
                            // Row
                            // sqlite���޿ɿ����ͣ�ֻ������DBNull���ɿ����ͣ����������Row�������Ͳ�ͬ��
                            if (reader.IsDBNull(i))
                                new Cell(row, reader.GetName(i), typeof(Nullable<>).MakeGenericType(reader.GetFieldType(i)));
                            else
                                new Cell(row, reader.GetName(i), reader.GetFieldType(i), reader.GetValue(i));
                        }
                        else
                        {
                            // Entity
                            var name = reader.GetName(i);
                            var col = map.FindColumn(name);

                            if (col == null)
                            {
                                // ӳ�������в����ڸ����ԣ�ʹ�����ݵ�ʵ������
                                if (reader.IsDBNull(i))
                                    new Cell(row, name, typeof(Nullable<>).MakeGenericType(reader.GetFieldType(i)));
                                else
                                    new Cell(row, name, reader.GetFieldType(i), reader.GetValue(i));
                            }
                            else
                            {
                                // ��ӳ�����Ե�����������Ϊ׼����sqlie���������٣��޿ɿ����͡�bool��DateTime��
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
        /// ȡ�ò�ѯ���ݿ��е���ֵ��
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
        /// ���ص�һ�����ݣ���ת��Ϊָ������
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
        /// ���ص�һ��ö�٣�������
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
        /// ����SqliteDataReader�Ľ���ֶ�����Table���нṹ��Ϣ.
        /// �����е�������tablemapping�д�����ʱ������������ͱ���һ�£��������˵�tablemapping�Ϳͻ��˵���һ�µġ�
        /// </summary>
        /// <param name="p_dr"></param>
        /// <returns></returns>
        Table ComposeDt(SqliteDataReader p_dr)
        {
            Table dt = new Table();
            var cols = dt.Columns;
            for (int i = 0; i < p_dr.FieldCount; i++)
            {
                // Microsoft.Data.Sqlite ����5.0.3��ʹ�� GetFieldType ��������ͨ��GetDataTypeName��bigint���ͣ�
                cols.Add(new Column(p_dr.GetName(i), p_dr.GetFieldType(i)));

                // ������һ������½�ֹʹ��SqliteDataReader �� GetFieldType���������ؽ���ڱ����������ݳ���dbnull��ʱ����ȷ��
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
                //        throw new Exception("������������͡�");
                //}
            }
            return dt;
        }
        */
    }
}