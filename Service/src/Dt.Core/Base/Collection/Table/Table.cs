#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 数据表，行集合(行为同构)，提供简单的数据表管理功能
    /// </summary>
    public class Table : List<Row>, IRpcJson
    {
        #region 构造方法
        public Table()
        {
            Columns = new ColumnList(this);
        }
        #endregion

        #region 属性
        /// <summary>
        /// 是否只序列化需要增删改的行
        /// </summary>
        public bool SerializeChanged { get; set; }

        /// <summary>
        /// 获取是否存在行被修改或删除
        /// </summary>
        public bool IsChanged
        {
            get
            {
                return (from item in this
                        where item.IsChanged
                        select item).Any();
            }
        }

        /// <summary>
        /// 获取数据已被修改的行列表
        /// </summary>
        public IEnumerable<Row> ChangedRows
        {
            get
            {
                return from item in this
                       where item.IsChanged
                       select item;
            }
        }

        /// <summary>
        /// 列结构集合
        /// </summary>
        public ColumnList Columns { get; }

        /// <summary>
        /// 获取设置用于存储与此对象相关的任意对象值
        /// </summary>
        public object Tag { get; set; }
        #endregion

        #region 创建表结构
        /// <summary>
        /// 添加列，主要为支持手动创建Table时初始化器的写法！
        /// </summary>
        /// <param name="p_colName">列名</param>
        /// <param name="p_colType">列数据类型, 默认typeof(string)</param>
        public void Add(string p_colName, Type p_colType = null)
        {
            Columns.Add(new Column(p_colName, p_colType));
        }

        /// <summary>
        /// 根据表名创建空Table
        /// </summary>
        /// <param name="p_tblName">表名</param>
        /// <returns></returns>
        public static Table Create(string p_tblName)
        {
            if (string.IsNullOrEmpty(p_tblName))
                throw new Exception("根据表名创建空Table时表名不可为空！");

            Table tbl = new Table();
            var schema = DbSchema.GetTableSchema(p_tblName);
            foreach (var row in schema.PrimaryKey)
            {
                tbl.Columns.Add(new Column(row.Name, row.Type));
            }
            foreach (var row in schema.Columns)
            {
                tbl.Columns.Add(new Column(row.Name, row.Type));
            }
            return tbl;
        }

        /// <summary>
        /// 通过复制创建空Table（不复制数据！）
        /// </summary>
        /// <param name="p_tbl"></param>
        /// <returns></returns>
        public static Table Create(Table p_tbl)
        {
            Table tbl = new Table();
            if (p_tbl != null && p_tbl.Columns.Count > 0)
            {
                foreach (var col in p_tbl.Columns)
                {
                    tbl.Columns.Add(new Column(col.ID, col.Type));
                }
            }
            return tbl;
        }

        /// <summary>
        /// 根据行创建空Table
        /// </summary>
        /// <param name="p_row"></param>
        /// <returns></returns>
        public static Table Create(Row p_row)
        {
            Table tbl = new Table();
            if (p_row != null)
            {
                foreach (var cell in p_row.Cells)
                {
                    tbl.Columns.Add(new Column(cell.ID, cell.Type));
                }
            }
            return tbl;
        }
        #endregion

        #region 行操作
        /// <summary>
        /// 添加新行并设置初始值，无参数时为空行，有参数时分两种情况，主要为简化编码：
        /// <para>1. 只一个参数且为匿名对象时，将匿名对象的属性作为初始值进行赋值，属性名称不区分大小写</para>
        /// <para>2. 按参数顺序进行赋值，跳过参数为null的情况，支持部分列值，支持列值的类型转换</para>
        /// </summary>
        /// <param name="p_params">按顺序的列值或匿名对象</param>
        /// <returns>返回新行</returns>
        public Row NewRow(params object[] p_params)
        {
            Row row = CreateRow(p_params);
            row.IsAdded = true;
            row.Table = this;
            base.Add(row);
            return row;
        }

        /// <summary>
        /// 创建独立行并设置初始值，未添加到当前Table！未设置IsAdded标志！无参数时为空行，有参数时分两种情况，主要为简化编码：
        /// <para>1. 只一个参数且为匿名对象时，将匿名对象的属性作为初始值进行赋值，属性名称不区分大小写</para>
        /// <para>2. 按参数顺序进行赋值，跳过参数为null的情况，支持部分列值，支持列值的类型转换</para>
        /// </summary>
        /// <param name="p_params">按顺序的列值或匿名对象</param>
        /// <returns>返回独立行</returns>
        public Row CreateRow(params object[] p_params)
        {
            Row row = new Row();
            if (Columns.Count > 0)
            {
                if (p_params.Length < 2)
                {
                    // 空行
                    foreach (var col in Columns)
                    {
                        new Cell(row, col.ID, col.Type);
                    }

                    object val;
                    if (p_params.Length == 1 && (val = p_params[0]) != null)
                    {
                        // 一个参数时，处理匿名对象赋值情况！判断条件未全用！
                        Type type = val.GetType();
                        if (type.Namespace == null && type.IsNotPublic)
                        {
                            // 参数为匿名对象
                            foreach (var prop in type.GetProperties())
                            {
                                row.Cells[prop.Name].InitVal(prop.GetValue(val));
                            }
                        }
                        else
                        {
                            // 普通参数
                            row.Cells[0].InitVal(val);
                        }
                    }
                }
                else
                {
                    // 多参数时按顺序赋值
                    for (int i = 0; i < Columns.Count; i++)
                    {
                        var col = Columns[i];
                        var cell = new Cell(row, col.ID, col.Type);
                        if (i < p_params.Length && p_params[i] != null)
                            cell.InitVal(p_params[i]);
                    }
                }
            }
            return row;
        }

        /// <summary>
        /// 将行添加到表，不检查结构，不改变状态
        /// </summary>
        /// <param name="p_row"></param>
        new public void Add(Row p_row)
        {
            if (p_row != null)
            {
                p_row.Table = this;
                base.Add(p_row);
            }
        }

        /// <summary>
        /// 插入行，不检查结构，不改变状态
        /// </summary>
        /// <param name="p_index"></param>
        /// <param name="p_row"></param>
        new public void Insert(int p_index, Row p_row)
        {
            if (p_row != null && p_index >= 0 && p_index <= Count)
            {
                p_row.Table = this;
                base.Insert(p_index, p_row);
            }
        }
        #endregion

        #region 列操作
        /// <summary>
        /// 重置列类型
        /// </summary>
        /// <param name="p_colName">列名</param>
        /// <param name="p_tgtType">转换到的目标类型</param>
        /// <returns>true 重置成功</returns>
        public bool ResetType(string p_colName, Type p_tgtType)
        {
            bool suc = true;
            foreach (var row in this)
            {
                if (!row.ResetType(p_colName, p_tgtType))
                {
                    suc = false;
                    break;
                }
            }
            if (suc)
                Columns[p_colName].Type = p_tgtType;
            return suc;
        }
        #endregion

        #region 数据操作
        /// <summary>
        /// 提交自上次调用以来对该表进行的所有更改，清空已删除的行，重置各行IsAdded和IsChanged状态
        /// </summary>
        public void AcceptChanges()
        {
            foreach (Row row in ChangedRows)
            {
                row.AcceptChanges();
            }
        }

        /// <summary>
        /// 回滚自该表加载以来或上次调用以来对该表进行的所有更改，恢复所有被删除的行。
        /// </summary>
        public void RejectChanges()
        {
            foreach (Row row in ChangedRows)
            {
                row.RejectChanges();
            }
        }

        /// <summary>
        /// 复制表结构及数据，深度克隆
        /// </summary>
        /// <returns></returns>
        public Table Clone()
        {
            Table tbl = new Table();
            // 添加列
            foreach (var col in Columns)
            {
                tbl.Columns.Add(new Column(col.ID, col.Type));
            }

            // 复制数据
            foreach (var row in this)
            {
                Row clone = row.Clone();
                clone.Table = tbl;
                tbl.Add(clone);
            }
            return tbl;
        }
        #endregion

        #region IRpcJson
        void IRpcJson.ReadRpcJson(JsonReader p_reader)
        {
            // cols外层 [
            p_reader.Read();
            // 列[
            while (p_reader.Read())
            {
                // cols外层 ]
                if (p_reader.TokenType == JsonToken.EndArray)
                    break;

                int index = 0;
                string colName = null;
                Type colType = null;

                // 列 [
                while (p_reader.Read())
                {
                    // 列 ]
                    if (p_reader.TokenType == JsonToken.EndArray)
                        break;

                    if (index == 0)
                        colName = p_reader.Value.ToString();
                    else
                        colType = TableKit.GetColType(p_reader.Value.ToString());
                    index++;
                }
                Columns.Add(new Column(colName, colType));
            }

            // rows外层 [
            p_reader.Read();
            int colCnt = Columns.Count;
            // [
            while (p_reader.Read())
            {
                // rows外层 ]
                if (p_reader.TokenType == JsonToken.EndArray)
                    break;

                int index = 0;
                Row row = NewRow();
                row.IsAdded = false;
                // 行 [
                while (p_reader.Read())
                {
                    // 行 ]
                    if (p_reader.TokenType == JsonToken.EndArray)
                        break;

                    if (index < colCnt)
                    {
                        // 不会引起连续IsChanged改变
                        row.Cells[index].InitVal(p_reader.Value);
                    }
                    else
                    {
                        // 超出的列值为行状态
                        string state = p_reader.Value.ToString();
                        if (state == "Added")
                            row.IsAdded = true;
                        else if (state == "Modified")
                            row.IsChanged = true;
                    }
                    index++;
                }
            }

            // 最外层 ]
            p_reader.Read();
        }

        void IRpcJson.WriteRpcJson(JsonWriter p_writer)
        {
            p_writer.WriteStartArray();
            // 类型
            p_writer.WriteValue("#tbl");

            // 列
            p_writer.WriteStartArray();
            foreach (Column column in Columns)
            {
                p_writer.WriteStartArray();
                p_writer.WriteValue(column.ID);
                if (column.Type != typeof(string))
                    p_writer.WriteValue(TableKit.GetColTypeAlias(column.Type));
                p_writer.WriteEndArray();
            }
            p_writer.WriteEndArray();

            // 行
            p_writer.WriteStartArray();
            if (SerializeChanged)
            {
                // 只序列化需要增加修改的行
                foreach (Row row in this)
                {
                    if (row.IsChanged)
                        SerializeJsonRow(row, p_writer);
                }
            }
            else
            {
                foreach (Row row in this)
                {
                    SerializeJsonRow(row, p_writer);
                }
            }
            p_writer.WriteEndArray();

            p_writer.WriteEndArray();
        }

        /// <summary>
        /// 序列化行数据
        /// </summary>
        /// <param name="p_dataRow"></param>
        /// <param name="p_writer"></param>
        void SerializeJsonRow(Row p_dataRow, JsonWriter p_writer)
        {
            p_writer.WriteStartArray();
            foreach (var cell in p_dataRow.Cells)
            {
                if (cell.Val != null)
                {
                    if (cell.Type == typeof(DateTime))
                        p_writer.WriteValue(((DateTime)cell.Val).ToString("yyyy-MM-ddTHH:mm:ss.ffffff"));
                    else if (cell.Type == typeof(byte[]))
                        p_writer.WriteValue(Convert.ToBase64String((byte[])cell.Val));
                    else
                        p_writer.WriteValue(cell.Val);
                }
                else
                {
                    p_writer.WriteNull();
                }
            }
            // 行状态
            if (p_dataRow.IsAdded)
                p_writer.WriteValue("Added");
            else if (p_dataRow.IsChanged)
                p_writer.WriteValue("Modified");
            p_writer.WriteEndArray();
        }
        #endregion
    }
}
