#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-12 创建
******************************************************************************/
#endregion

#region 引用命名
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 数据表，行集合(行为同构)，提供简单的数据表管理功能
    /// </summary>
    public partial class Table : ObservableCollection<Row>, IRpcJson
    {
        #region 成员变量
        protected readonly ColumnList _columns;
        bool _isChanged = false;
        bool _delayCheckChanges = false;
        int _updating;
        #endregion

        #region 构造方法
        public Table()
        {
            _columns = new ColumnList(this);
        }
        #endregion

        #region 事件
        /// <summary>
        /// 内部单元格的值发生变化
        /// </summary>
        public event EventHandler<Cell> Changed;
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
            get { return _isChanged; }
            set
            {
                if (_isChanged != value)
                {
                    _isChanged = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("IsChanged"));
                }
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
        public ColumnList Columns
        {
            get { return _columns; }
        }

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
            _columns.Add(new Column(p_colName, p_colType));
        }

        /// <summary>
        /// 通过复制创建空Table（不复制数据！）
        /// </summary>
        /// <param name="p_tbl"></param>
        /// <returns></returns>
        public static Table Create(Table p_tbl)
        {
            Table tbl = new Table();
            if (p_tbl != null && p_tbl._columns.Count > 0)
            {
                foreach (var col in p_tbl._columns)
                {
                    tbl._columns.Add(new Column(col.ID, col.Type));
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
                    tbl._columns.Add(new Column(cell.ID, cell.Type));
                }
            }
            return tbl;
        }
        #endregion

        #region 行操作
        /// <summary>
        /// 添加新行并设置初始值，参数null时为空行
        /// <para>有参数时将参数的属性值作为初始值，前提是属性名和列名相同(不区分大小写)且类型相同</para>
        /// <para>支持匿名对象，主要为简化编码</para>
        /// <para>不再支持多参数按顺序赋值！</para>
        /// </summary>
        /// <param name="p_init">含初始值的对象，一般为匿名对象</param>
        /// <returns>返回新行</returns>
        public Row AddRow(object p_init = null)
        {
            Row row = NewRow(p_init);
            row.IsAdded = true;
            row.Table = this;
            base.Add(row);
            return row;
        }

        /// <summary>
        /// 创建独立行并设置初始值，未添加到当前Table！未设置IsAdded标志！参数null时为空行
        /// <para>有参数时将参数的属性值作为初始值，前提是属性名和列名相同(不区分大小写)且类型相同</para>
        /// <para>支持匿名对象，主要为简化编码</para>
        /// <para>不再支持多参数按顺序赋值！</para>
        /// </summary>
        /// <param name="p_init">含初始值的对象，一般为匿名对象</param>
        /// <returns>返回独立行</returns>
        public Row NewRow(object p_init = null)
        {
            Row row = CreateRowInstance();

            // 空行
            if (p_init == null)
            {
                foreach (var col in _columns)
                {
                    new Cell(row, col.ID, col.Type);
                }
                return row;
            }

            // 匿名对象无法在GetProperty时指定BindingFlags！
            var props = p_init.GetType().GetProperties().ToList();
            foreach (var col in _columns)
            {
                if (props.Count > 0)
                {
                    object val = null;
                    for (int i = 0; i < props.Count; i++)
                    {
                        var pi = props[i];
                        if (pi.Name.Equals(col.ID, StringComparison.OrdinalIgnoreCase))
                        {
                            // 存在同名属性，减小下次查询范围
                            props.RemoveAt(i);
                            val = pi.GetValue(p_init);
                            break;
                        }
                    }
                    new Cell(row, col.ID, col.Type, val);
                }
                else
                {
                    new Cell(row, col.ID, col.Type);
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

        /// <summary>
        /// 删除行，更新数据变化状态
        /// </summary>
        /// <param name="p_row">行实体</param>
        new public bool Remove(Row p_row)
        {
            bool success = base.Remove(p_row);
            if (success)
            {
                p_row.Table = null;
                CheckChanges();
            }
            return success;
        }

        /// <summary>
        /// 删除行，更新数据变化状态
        /// </summary>
        /// <param name="p_index"></param>
        new public void RemoveAt(int p_index)
        {
            if (p_index > -1 && p_index < Count)
            {
                this[p_index].Table = null;
                base.RemoveAt(p_index);
                CheckChanges();
            }
        }

        /// <summary>
        /// 清空所有行
        /// </summary>
        new public void Clear()
        {
            while (Count > 0)
            {
                this[0].Table = null;
                base.RemoveAt(0);
            }
            IsChanged = false;
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
                _columns[p_colName].Type = p_tgtType;
            return suc;
        }
        #endregion

        #region 数据操作
        /// <summary>
        /// 提交自上次调用以来对该表进行的所有更改，清空已删除的行，重置各行IsAdded和IsChanged状态
        /// </summary>
        public void AcceptChanges()
        {
            _delayCheckChanges = true;
            foreach (Row row in ChangedRows)
            {
                row.AcceptChanges();
            }
            IsChanged = false;
            _delayCheckChanges = false;
        }

        /// <summary>
        /// 回滚自该表加载以来或上次调用以来对该表进行的所有更改，恢复所有被删除的行。
        /// </summary>
        public void RejectChanges()
        {
            _delayCheckChanges = true;
            foreach (Row row in ChangedRows)
            {
                row.RejectChanges();
            }
            IsChanged = false;
            _delayCheckChanges = false;
        }

        /// <summary>
        /// 复制表结构及数据，深度克隆
        /// </summary>
        /// <returns></returns>
        public Table Clone()
        {
            // 当前可能为Table<TRow>
            Table tbl = (Table)Activator.CreateInstance(GetType());
            // 添加列
            foreach (var col in _columns)
            {
                tbl._columns.Add(new Column(col.ID, col.Type));
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

        #region 内部方法
        /// <summary>
        /// 检查当前行数据是否有变化
        /// </summary>
        internal void CheckChanges()
        {
            if (_delayCheckChanges)
                return;

            bool changed = false;
            foreach (Row row in this)
            {
                if (row.IsChanged)
                {
                    changed = true;
                    break;
                }
            }
            IsChanged = changed;
        }

        /// <summary>
        /// 触发单元格值变化事件
        /// </summary>
        /// <param name="p_cell"></param>
        internal void OnValueChanged(Cell p_cell)
        {
            Changed?.Invoke(this, p_cell);
        }

        /// <summary>
        /// 创建行，重写可创建Row的派生行
        /// </summary>
        /// <returns></returns>
        protected virtual Row CreateRowInstance()
        {
            return new Row();
        }
        #endregion

        #region 延迟更新
        /// <summary>
        /// 延迟触发CollectionChanged事件
        /// </summary>
        /// <returns></returns>
        /// <example>
        /// <code>
        /// using (tbl.Defer())
        /// {
        ///     foreach (var row in data)
        ///     {
        ///         tbl.Add(row);
        ///     }
        /// }
        /// </code>
        /// </example>
        public IDisposable Defer()
        {
            return new InternalCls(this);
        }

        /// <summary>
        /// 通过Defer实现延时更新
        /// </summary>
        int Updating
        {
            get { return _updating; }
            set
            {
                _updating = value;
                if (_updating == 0)
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        /// <summary>
        /// 重新触发CollectionChanged的方法
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (_updating <= 0)
            {
                // 符合更新条件，触发基类事件，否则延迟更新
                base.OnCollectionChanged(e);
            }
        }

        class InternalCls : IDisposable
        {
            Table _owner;

            public InternalCls(Table p_owner)
            {
                _owner = p_owner;
                _owner.Updating = _owner.Updating + 1;
            }

            public void Dispose()
            {
                _owner.Updating = _owner.Updating - 1;
            }
        }
        #endregion

        #region 列类型工具
        /// <summary>
        /// Type -> string，大小写敏感
        /// </summary>
        /// <param name="p_type"></param>
        /// <returns></returns>
        public static string GetColTypeAlias(Type p_type)
        {
            if (p_type.IsGenericType)
            {
                if (p_type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    return p_type.GetGenericArguments()[0].Name + "?";
                throw new Exception("无法映射的数据类型:" + p_type.ToString());
            }
            return p_type.Name;
        }

        /// <summary>
        /// string -> Type，大小写敏感
        /// </summary>
        /// <param name="p_name"></param>
        /// <returns></returns>
        public static Type GetColType(string p_name)
        {
            if (p_name.EndsWith('?'))
            {
                Type tp = Type.GetType("System." + p_name.TrimEnd('?'), true, false);
                return typeof(Nullable<>).MakeGenericType(tp);
            }
            return Type.GetType("System." + p_name, true, false);
        }
        #endregion

        #region IRpcJson
        /// <summary>
        /// 反序列化读取Rpc Json数据
        /// </summary>
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
                        colType = GetColType(p_reader.Value.ToString());
                    index++;
                }
                _columns.Add(new Column(colName, colType));
            }

            // rows外层 [
            p_reader.Read();
            int colCnt = _columns.Count;
            // [
            while (p_reader.Read())
            {
                // rows外层 ]
                if (p_reader.TokenType == JsonToken.EndArray)
                    break;

                int index = 0;
                Row row = AddRow();
                row.IsAdded = false;
                // 行 [
                while (p_reader.Read())
                {
                    // 行 ]
                    if (p_reader.TokenType == JsonToken.EndArray)
                        break;

                    if (index < colCnt)
                    {
                        // 值变化时两值 [原始值,当前值]
                        if (p_reader.TokenType == JsonToken.StartArray)
                        {
                            // 两值 [
                            p_reader.Read();
                            row.Cells[index].OriginalVal = p_reader.Value;
                            p_reader.Read();
                            row.Cells[index].Val = p_reader.Value;
                            // 两值 ]
                            p_reader.Read();
                        }
                        else
                        {
                            // 不会引起连续IsChanged改变
                            row.Cells[index].InitVal(p_reader.Value);
                        }
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

        /// <summary>
        /// 将对象按照Rpc Json数据结构进行序列化
        /// </summary>
        void IRpcJson.WriteRpcJson(JsonWriter p_writer)
        {
            p_writer.WriteStartArray();
            // 类型
            p_writer.WriteValue("#tbl");

            // 列
            p_writer.WriteStartArray();
            foreach (Column column in _columns)
            {
                p_writer.WriteStartArray();
                p_writer.WriteValue(column.ID);
                if (column.Type != typeof(string))
                    p_writer.WriteValue(GetColTypeAlias(column.Type));
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
                if (cell.IsChanged)
                {
                    // 值变化时传递两值数组 [原始值,当前值]
                    p_writer.WriteStartArray();
                    p_writer.WriteValue(cell.OriginalVal);
                    p_writer.WriteValue(cell.Val);
                    p_writer.WriteEndArray();
                }
                else
                {
                    p_writer.WriteValue(cell.Val);
                }
            }
            // 行状态，多出的列
            if (p_dataRow.IsAdded)
                p_writer.WriteValue("Added");
            else if (p_dataRow.IsChanged)
                p_writer.WriteValue("Modified");
            p_writer.WriteEndArray();
        }
        #endregion
    }

    /// <summary>
    /// 行类型为强类型的数据表，行类型模拟普通实体类型
    /// </summary>
    /// <typeparam name="TRow">行类型</typeparam>
    public class Table<TRow> : Table
        where TRow : Row
    {
        /// <summary>
        /// 创建行
        /// </summary>
        /// <returns></returns>
        protected override Row CreateRowInstance()
        {
            return Activator.CreateInstance<TRow>();
        }
    }
}
