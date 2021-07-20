#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-12 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 数据表，行集合(行为同构)，提供简单的数据表管理功能
    /// </summary>
#if !SERVER
    [Windows.UI.Xaml.Data.Bindable]
#endif
    public partial class Table : ObservableCollection<Row>, IRpcJson
    {
        #region 成员变量
        protected readonly ColumnList _columns;
        bool _isChanged = false;
        bool _delayCheckChanges = false;
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
        /// 获取修改的和新增的行列表
        /// </summary>
        public IEnumerable<Row> ChangedAndAddedRows
        {
            get
            {
                return from item in this
                       where item.IsChanged || item.IsAdded
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
        /// 获取是否存在新增的行
        /// </summary>
        public bool ExistAdded
        {
            get
            {
                return (from item in this
                        where item.IsAdded
                        select item).Any();
            }
        }

        /// <summary>
        /// 获取设置用于存储与此对象相关的任意对象值
        /// </summary>
        public object Tag { get; set; }
        #endregion

        #region 创建表结构
        /// <summary>
        /// 添加列，主要为支持手动创建Table时初始化器的写法！如：
        /// Table tbl = new Table
        /// {
        ///     { "id" },
        ///     { "xm" },
        ///     { "bh", typeof(int)
        /// };
        /// </summary>
        /// <param name="p_colName">列名</param>
        /// <param name="p_colType">列数据类型, 默认typeof(string)</param>
        public void Add(string p_colName, Type p_colType = null)
        {
            _columns.Add(new Column(p_colName, p_colType));
        }

        /// <summary>
        /// 根据表名创建空Table
        /// </summary>
        /// <param name="p_tblName">表名</param>
        /// <returns></returns>
        public static Table Create(string p_tblName)
        {
            Throw.IfNullOrEmpty(p_tblName);
            Table tbl = new Table();
#if SERVER
            var schema = DbSchema.GetTableSchema(p_tblName);
            foreach (var col in schema.PrimaryKey.Concat(schema.Columns))
            {
                tbl._columns.Add(new Column(col.Name, col.Type));
            }
#else
            foreach (var col in AtModel.EachColumns(p_tblName))
            {
                tbl._columns.Add(new Column(col.ColName, GetColType(col.DbType)));
            }
#endif
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

        /// <summary>
        /// 根据json文件流内容创建Table对象
        /// </summary>
        /// <param name="p_stream">json流</param>
        /// <returns></returns>
        public static Table Create(Stream p_stream)
        {
            return Create<Table>(p_stream);
        }

        /// <summary>
        /// 根据json流内容创建Table对象
        /// </summary>
        /// <typeparam name="T">Table类型</typeparam>
        /// <param name="p_stream"></param>
        /// <returns></returns>
        public static T Create<T>(Stream p_stream)
            where T : Table
        {
            if (p_stream == null)
                return null;

            using (var sr = new StreamReader(p_stream))
            {
                return ParseJsonData<T>(Encoding.UTF8.GetBytes(sr.ReadToEnd()));
            }
        }

        /// <summary>
        /// 根据json字符串创建Table对象
        /// </summary>
        /// <param name="p_json"></param>
        /// <returns></returns>
        public static Table CreateFromJson(string p_json)
        {
            if (string.IsNullOrEmpty(p_json))
                return null;
            return ParseJsonData<Table>(Encoding.UTF8.GetBytes(p_json));
        }

        /// <summary>
        /// 根据json字符串创建Table对象
        /// </summary>
        /// <typeparam name="T">Table类型</typeparam>
        /// <param name="p_json"></param>
        /// <returns></returns>
        public static T CreateFromJson<T>(string p_json)
            where T : Table
        {
            if (string.IsNullOrEmpty(p_json))
                return null;
            return ParseJsonData<T>(Encoding.UTF8.GetBytes(p_json));
        }

        static T ParseJsonData<T>(byte[] p_data)
            where T : Table
        {
            var tbl = Activator.CreateInstance<T>();
            Utf8JsonReader reader = new Utf8JsonReader(p_data);
            // [
            reader.Read();
            // "#tbl"
            reader.Read();
            ((IRpcJson)tbl).ReadRpcJson(ref reader);
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
            row.Table = this;
            base.Add(row);
            return row;
        }

        /// <summary>
        /// 创建独立行并设置初始值，未添加到当前Table！已设置IsAdded标志！参数null时为空行
        /// <para>有参数时将参数的属性值作为初始值，前提是属性名和列名相同(不区分大小写)且类型相同</para>
        /// <para>支持匿名对象，主要为简化编码</para>
        /// <para>不再支持多参数按顺序赋值！</para>
        /// </summary>
        /// <param name="p_init">含初始值的对象，一般为匿名对象</param>
        /// <returns>返回独立行</returns>
        public Row NewRow(object p_init = null)
        {
            Row row = CreateRowInstance();
            row.IsAdded = true;

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
        /// 创建独立行并设置初始值，已设置IsAdded标志！参数null时为空行
        /// <para>有参数时将参数的属性值作为初始值，前提是属性名和列名相同(不区分大小写)且类型相同</para>
        /// <para>支持匿名对象，主要为简化编码</para>
        /// </summary>
        /// <param name="p_tblName">表名</param>
        /// <param name="p_init">含初始值的对象，一般为匿名对象</param>
        /// <returns>返回独立行</returns>
        public static Row NewRow(string p_tblName, object p_init = null)
        {
            return Create(p_tblName).NewRow(p_init);
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
            foreach (Row row in ChangedAndAddedRows)
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
        /// 深度克隆表结构及数据，返回同类型的Table
        /// </summary>
        /// <returns></returns>
        public Table Clone()
        {
            if (Count > 0)
                return CloneTo(this[0].GetType());

            // 空表
            // 当前可能为Table<TRow>
            Table tbl = (Table)Activator.CreateInstance(GetType());
            // 添加列
            foreach (var col in _columns)
            {
                tbl._columns.Add(new Column(col.ID, col.Type));
            }
            return tbl;
        }

        /// <summary>
        /// 将表结构及数据深度克隆到新实体类型的表，返回新实体表，一般类型转换时用
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <returns>返回新实体表</returns>
        public Table<TEntity> CloneTo<TEntity>()
            where TEntity : Entity
        {
            return (Table<TEntity>)CloneTo(typeof(TEntity));
        }

        /// <summary>
        /// 将表结构及数据深度克隆到新实体类型的表，返回新实体表，一般类型转换时用
        /// </summary>
        /// <param name="p_rowType">实体类型</param>
        /// <returns>返回新实体表</returns>
        public Table CloneTo(Type p_rowType)
        {
            Table tbl;
            if (p_rowType == typeof(Row))
                tbl = new Table();
            else
                tbl = (Table)Activator.CreateInstance(typeof(Table<>).MakeGenericType(p_rowType));

            // 添加列
            foreach (var col in _columns)
            {
                tbl._columns.Add(new Column(col.ID, col.Type));
            }

            // 复制数据
            foreach (var row in this)
            {
                Row clone = row.CloneTo(p_rowType);
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

            // tinyint(4)，无符号对应byte类型，程序中当作枚举类型
            if (p_type.IsEnum)
                return "Byte";

            return p_type.Name;
        }

        /// <summary>
        /// string -> Type，大小写敏感
        /// </summary>
        /// <param name="p_name"></param>
        /// <returns></returns>
        public static Type GetColType(string p_name)
        {
            if (p_name.EndsWith("?"))
            {
                Type tp = Type.GetType("System." + p_name.TrimEnd('?'), true, false);
                return typeof(Nullable<>).MakeGenericType(tp);
            }
            return Type.GetType("System." + p_name, true, false);
        }
        #endregion

        #region 外部方法
        /// <summary>
        /// 将当前表数据转成矩阵形式的表，如：
        /// xm       subject    score
        /// 张建国   语文       35
        /// 张建国   数学       78
        /// 张建国   外语       87
        /// 于凤琴   语文       98
        /// 于凤琴   数学       86
        /// 于凤琴   外语       68
        /// 
        /// 转成
        /// 
        /// xm       语文    数学    外语
        /// 张建国   35      78      87
        /// 于凤琴   98      86      68
        /// </summary>
        /// <param name="p_rowHeader">行头列名</param>
        /// <param name="p_colHeader">自动创建列的列名</param>
        /// <param name="p_valCol">取值列名</param>
        /// <returns>转置后的Table</returns>
        public Table CreateMatrix(string p_rowHeader, string p_colHeader, string p_valCol)
        {
            if (string.IsNullOrEmpty(p_rowHeader)
                || !_columns.Contains(p_rowHeader)
                || string.IsNullOrEmpty(p_colHeader)
                || !_columns.Contains(p_colHeader)
                || string.IsNullOrEmpty(p_valCol)
                || !_columns.Contains(p_valCol))
                return null;

            Row dr;
            Dictionary<string, Row> rowDict = new Dictionary<string, Row>();

            Table tbl = new Table();
            // 添加行头列
            tbl._columns.Add(new Column(p_rowHeader, typeof(string)));
            Type valColType = _columns[p_valCol].Type;

            foreach (Row row in this)
            {
                string rowHeader = row.Str(p_rowHeader);
                if (string.IsNullOrEmpty(rowHeader))
                    continue;

                if (!rowDict.TryGetValue(rowHeader, out dr))
                {
                    dr = tbl.AddRow();
                    dr[0] = rowHeader;
                    rowDict[rowHeader] = dr;
                }

                string colHeader = row.Str(p_colHeader);
                if (string.IsNullOrEmpty(colHeader))
                    continue;

                if (!tbl._columns.Contains(colHeader))
                    tbl._columns.Add(new Column(colHeader, valColType));
                dr.Cells[colHeader].InitVal(row[p_valCol]);
            }
            return tbl;
        }
        #endregion

        #region 记录删除行
        /// <summary>
        /// 开始记录被删除的行，IsAdded为true的行不参加，当被删除的行重新添加时移除记录
        /// </summary>
        public void StartRecordDelRows()
        {
            DeletedRows = new List<Row>();
            CollectionChanged += OnCollectionChanged;
        }

        /// <summary>
        /// 停止记录被删除的行
        /// </summary>
        public void StopRecordDelRows()
        {
            // 未清空或重置 DeletedRows，可能历史记录仍有用！
            CollectionChanged -= OnCollectionChanged;
        }

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add
                && e.NewItems[0] is Row row)
            {
                if (!row.IsAdded)
                    DeletedRows.Remove(row);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove
                && e.OldItems[0] is Row row2)
            {
                if (!row2.IsAdded)
                    DeletedRows.Add(row2);
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                DeletedRows.Clear();
            }
        }

        /// <summary>
        /// 获取已被删除的行列表
        /// </summary>
        public List<Row> DeletedRows { get; private set; }

        /// <summary>
        /// 获取是否存在被删除的行
        /// </summary>
        public bool ExistDeleted
        {
            get { return DeletedRows != null && DeletedRows.Count > 0; }
        }
        #endregion

        #region IRpcJson
        /// <summary>
        /// 反序列化读取Rpc Json数据
        /// </summary>
        void IRpcJson.ReadRpcJson(ref Utf8JsonReader p_reader)
        {
            // Entity类型
            Type tpEntity = null;
            if (GetType().IsGenericType)
            {
                tpEntity = GetType().GetGenericArguments()[0];
            }

            // cols外层 [
            p_reader.Read();
            // 列[
            while (p_reader.Read())
            {
                // cols外层 ]
                if (p_reader.TokenType == JsonTokenType.EndArray)
                    break;

                int index = 0;
                string colName = null;
                Type colType = null;

                // 列 [
                while (p_reader.Read())
                {
                    // 列 ]
                    if (p_reader.TokenType == JsonTokenType.EndArray)
                        break;

                    if (index == 0)
                    {
                        colName = p_reader.GetString();
                    }
                    else
                    {
                        colType = GetColType(p_reader.GetString());
                        if (colType == typeof(byte) && tpEntity != null)
                        {
                            // Entity 时根据属性类型将 byte 自动转为 enum 类型
                            var prop = tpEntity.GetProperty(colName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.IgnoreCase);
                            if (prop != null)
                                colType = prop.PropertyType;
                        }
                    }
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
                if (p_reader.TokenType == JsonTokenType.EndArray)
                    break;

                int index = 0;
                Row row = AddRow();
                row.IsAdded = false;
                // 行 [
                while (p_reader.Read())
                {
                    // 行 ]
                    if (p_reader.TokenType == JsonTokenType.EndArray)
                        break;

                    if (index < colCnt)
                    {
                        // 值变化时两值 [原始值,当前值]
                        if (p_reader.TokenType == JsonTokenType.StartArray)
                        {
                            // 两值 [
                            p_reader.Read();
                            row.Cells[index].OriginalVal = JsonRpcSerializer.Deserialize(ref p_reader, _columns[index].Type);
                            p_reader.Read();
                            row.Cells[index].SetVal(JsonRpcSerializer.Deserialize(ref p_reader, _columns[index].Type));
                            // 两值 ]
                            p_reader.Read();
                        }
                        else
                        {
                            // 不会引起连续IsChanged改变
                            row.Cells[index].InitVal(JsonRpcSerializer.Deserialize(ref p_reader, _columns[index].Type));
                        }
                    }
                    else
                    {
                        // 超出的列值为行状态
                        string state = p_reader.GetString();
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
        void IRpcJson.WriteRpcJson(Utf8JsonWriter p_writer)
        {
            p_writer.WriteStartArray();
            // 类型
            p_writer.WriteStringValue("#tbl");

            // 列
            p_writer.WriteStartArray();
            foreach (Column column in _columns)
            {
                p_writer.WriteStartArray();
                p_writer.WriteStringValue(column.ID);
                if (column.Type != typeof(string))
                    p_writer.WriteStringValue(GetColTypeAlias(column.Type));
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
        void SerializeJsonRow(Row p_dataRow, Utf8JsonWriter p_writer)
        {
            p_writer.WriteStartArray();
            foreach (var cell in p_dataRow.Cells)
            {
                if (cell.IsChanged)
                {
                    // 值变化时传递两值数组 [原始值,当前值]
                    p_writer.WriteStartArray();
                    JsonRpcSerializer.Serialize(cell.OriginalVal, p_writer);
                    JsonRpcSerializer.Serialize(cell.Val, p_writer);
                    p_writer.WriteEndArray();
                }
                else
                {
                    JsonRpcSerializer.Serialize(cell.Val, p_writer);
                }
            }
            // 行状态，多出的列
            if (p_dataRow.IsAdded)
                p_writer.WriteStringValue("Added");
            else if (p_dataRow.IsChanged)
                p_writer.WriteStringValue("Modified");
            p_writer.WriteEndArray();
        }
        #endregion
    }
}
