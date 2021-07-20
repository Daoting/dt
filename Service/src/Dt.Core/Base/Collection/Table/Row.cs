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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Xml;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 数据行管理类
    /// </summary>
#if !SERVER
    [Windows.UI.Xaml.Data.Bindable]
#endif
    public class Row : INotifyPropertyChanged, IRpcJson
    {
        #region 成员变量
        const string _indexError = "索引值{0}超出范围(0-{1})！";
        protected readonly CellList _cells;
        bool _delayCheckChanges;
        bool _isChanged;
        #endregion

        #region 构造方法
        /// <summary>
        /// 用来构造独立行，脱离Table使用
        /// </summary>
        public Row()
        {
            _cells = new CellList();
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
        /// 根据列名获取设置列值
        /// <para>1. 触发属性值变化事件，改变IsChanged状态</para>
        /// <para>2. 设置列的默认值时，请使用InitVal方法</para>
        /// </summary>
        /// <param name="p_colName">列名</param>
        /// <returns>列值</returns>
        public object this[string p_colName]
        {
            get { return _cells[p_colName].Val; }
            set { _cells[p_colName].SetVal(value); }
        }

        /// <summary>
        /// 根据索引获取设置列值
        /// <para>1. 触发属性值变化事件，改变IsChanged状态</para>
        /// <para>2. 设置列的默认值时，请使用InitVal方法</para>
        /// </summary>
        /// <param name="p_index">列索引</param>
        /// <returns>列值</returns>
        public object this[int p_index]
        {
            get
            {
                if (p_index < 0 || p_index >= _cells.Count)
                    throw new Exception(string.Format(_indexError, p_index, _cells.Count));
                return _cells[p_index].Val;
            }
            set
            {
                if (p_index < 0 || p_index >= _cells.Count)
                    throw new Exception(string.Format(_indexError, p_index, _cells.Count));
                _cells[p_index].SetVal(value);
            }
        }

        /// <summary>
        /// 获取设置id列的值，常用的实体属性
        /// </summary>
        public long ID
        {
            get { return (long)_cells["id"].Val; }
            set { _cells["id"].SetVal(value); }
        }

        /// <summary>
        /// 获取当前所有数据项
        /// </summary>
        public CellList Cells
        {
            get { return _cells; }
        }

        /// <summary>
        /// 获取当前行是否已发生更改。
        /// </summary>
        public bool IsChanged
        {
            get { return _isChanged; }
            set
            {
                if (_isChanged == value)
                    return;

                _isChanged = value;
                if (Table != null)
                {
                    if (_isChanged)
                        Table.IsChanged = true;
                    else
                        Table.CheckChanges();
                }
                // 触发当前行业务数据发生变化
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsChanged"));
            }
        }

        /// <summary>
        /// 获取设置当前行是否为新增
        /// </summary>
        public bool IsAdded { get; set; }

        /// <summary>
        /// 获得已改变的Cell
        /// </summary>
        /// <returns>Cell列表</returns>
        public IEnumerable<Cell> ChangedCells
        {
            get
            {
                return from item in _cells
                       where item.IsChanged
                       select item;
            }
        }

        /// <summary>
        /// 获取在Table中的序号
        /// </summary>
        public int Index
        {
            get
            {
                if (Table != null)
                    return Table.IndexOf(this);
                return -1;
            }
        }

        /// <summary>
        /// 获取业务对象的描述信息
        /// </summary>
        public string Description
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("共{0}列：\r\n", _cells.Count);
                foreach (var cell in _cells)
                {
                    sb.AppendFormat("{0}：{1}\r\n", cell.ID, cell.Val);
                }
                return sb.ToString();
            }
        }

        /// <summary>
        /// 当前行所属父集合
        /// </summary>
        public Table Table { get; internal set; }

        /// <summary>
        /// 获取设置用于存储与此对象相关的任意对象值
        /// </summary>
        public object Tag { get; set; }
        #endregion

        #region 外部方法
        /// <summary>
        /// 提交自上次调用以来对该行进行的所有更改。
        /// </summary>
        public void AcceptChanges()
        {
            foreach (var cell in ChangedCells)
            {
                cell.AcceptChanges();
            }

            IsAdded = false;
            IsChanged = false;
        }

        /// <summary>
        /// 回滚自该表加载以来或上次调用 AcceptChanges 以来对该行进行的所有更改。
        /// </summary>
        public void RejectChanges()
        {
            _delayCheckChanges = true;
            // 回滚过程中可能判断行IsChanged，原来放在回滚后！
            IsChanged = false;
            foreach (var cell in ChangedCells)
            {
                cell.RejectChanges();
            }
            _delayCheckChanges = false;
        }

        /// <summary>
        /// 添加新数据项
        /// </summary>
        /// <typeparam name="T">Cell的数据类型</typeparam>
        /// <param name="p_cellName">字段名，不可为空，作为键值</param>
        /// <param name="p_value">初始值</param>
        public Cell AddCell<T>(string p_cellName, T p_value = default)
        {
            if (Contains(p_cellName))
                throw new Exception($"已包含{p_cellName}列！");
            return new Cell(this, p_cellName, typeof(T), p_value);
        }

        /// <summary>
        /// 深度克隆行对象，返回同类型的独立行
        /// </summary>
        /// <returns>返回独立行</returns>
        public Row Clone()
        {
            // 当前可能为Row的派生类
            return CloneTo(GetType());
        }

        /// <summary>
        /// 将当前对象深度克隆到指定类型的新实体对象，返回新独立实体，一般类型转换时用
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <returns>返回新独立实体</returns>
        public TEntity CloneTo<TEntity>()
            where TEntity : Entity
        {
            return (TEntity)CloneTo(typeof(TEntity));
        }

        /// <summary>
        /// 将当前对象深度克隆到指定类型的新实体对象，返回新独立实体，一般类型转换时用
        /// </summary>
        /// <param name="p_type">实体类型</param>
        /// <returns>返回新独立实体</returns>
        public Row CloneTo(Type p_type)
        {
            bool isEntity = p_type.IsSubclassOf(typeof(Entity));

            // 无参数构造方法可能为private，如实体类型
            Row row = (Row)Activator.CreateInstance(p_type, true);
            row.IsAdded = IsAdded;
            foreach (var item in _cells)
            {
                var tp = item.Type;
                if (tp == typeof(byte) && isEntity)
                {
                    // Entity 时根据属性类型将 byte 自动转为 enum 类型
                    var prop = p_type.GetProperty(item.ID, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.IgnoreCase);
                    if (prop != null)
                        tp = prop.PropertyType;
                }

                if (item.IsChanged)
                    new Cell(row, item.ID, tp, item.OriginalVal).SetVal(item.Val);
                else
                    new Cell(row, item.ID, tp, item.OriginalVal);
            }
            return row;
        }

        /// <summary>
        /// 判断是否包含给定的列
        /// </summary>
        /// <param name="p_columnName">列名</param>
        /// <returns>true 包含</returns>
        public bool Contains(string p_columnName)
        {
            Throw.IfNullOrEmpty(p_columnName);
            return _cells.Contains(p_columnName);
        }

        /// <summary>
        /// 删除当前行
        /// </summary>
        public void Remove()
        {
            if (Table != null)
                Table.Remove(this);
        }

        /// <summary>
        /// Row转换成Dict对象
        /// </summary>
        /// <returns>Dict对象</returns>
        public Dict ToDict()
        {
            Dict dt = new Dict();
            foreach (var cell in _cells)
            {
                dt[cell.ID] = cell.Val;
            }
            return dt;
        }
        #endregion

        #region 值操作
        /// <summary>
        /// 批量设置单元格默认值，恢复单元格IsChanged=false状态，为简化编码支持匿名对象
        /// </summary>
        /// <param name="p_anonyVal">含值的对象，将对象属性值作为初始值</param>
        public void InitVal(object p_anonyVal)
        {
            if (p_anonyVal == null)
                return;

            Type type = p_anonyVal.GetType();
            foreach (var prop in type.GetProperties())
            {
                _cells[prop.Name].InitVal(prop.GetValue(p_anonyVal));
            }
        }

        /// <summary>
        /// 设置单元格默认值，恢复单元格IsChanged=false状态
        /// </summary>
        /// <param name="p_colName">列名</param>
        /// <param name="p_val">列值</param>
        public void InitVal(string p_colName, object p_val)
        {
            _cells[p_colName].InitVal(p_val);
        }

        /// <summary>
        /// 按照索引设置单元格默认值，恢复单元格IsChanged=false状态
        /// </summary>
        /// <param name="p_index">列索引</param>
        /// <param name="p_val">列值</param>
        public void InitVal(int p_index, object p_val)
        {
            if (p_index < 0 || p_index >= _cells.Count)
                throw new Exception(string.Format(_indexError, p_index, _cells.Count));
            _cells[p_index].InitVal(p_val);
        }

        /// <summary>
        /// 复制给定行数据的对应列值
        /// </summary>
        /// <param name="p_src"></param>
        public void Copy(Row p_src)
        {
            if (p_src == null)
                return;

            foreach (var item in _cells)
            {
                if (p_src.Contains(item.ID))
                    item.InitVal(p_src[item.ID]);
            }
        }

        /// <summary>
        /// 重置列类型
        /// </summary>
        /// <param name="p_colName">列名</param>
        /// <param name="p_tgtType">转换到的目标类型</param>
        /// <returns>true 重置成功</returns>
        public bool ResetType(string p_colName, Type p_tgtType)
        {
            try
            {
                return _cells[p_colName].ResetType(p_tgtType);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 根据索引获取指定列的字符串值，为null时返回string.Empty！！！
        /// </summary>
        /// <param name="p_index">索引</param>
        /// <returns>字符串值</returns>
        public string Str(int p_index)
        {
            return GetVal<string>(p_index);
        }

        /// <summary>
        /// 根据列名获取指定列的字符串值，为null时返回string.Empty！！！
        /// </summary>
        /// <param name="p_columnName">列名</param>
        /// <returns>字符串值</returns>
        public string Str(string p_columnName)
        {
            return GetVal<string>(p_columnName);
        }

        /// <summary>
        /// 指定列的字符串值是否为空
        /// </summary>
        /// <param name="p_index">索引</param>
        /// <returns>true 表示列值为空</returns>
        public bool IsEmpty(int p_index)
        {
            return string.IsNullOrEmpty(GetVal<string>(p_index));
        }

        /// <summary>
        /// 指定列的字符串值是否为空
        /// </summary>
        /// <param name="p_columnName">列名</param>
        /// <returns>true 表示列值为空</returns>
        public bool IsEmpty(string p_columnName)
        {
            return string.IsNullOrEmpty(GetVal<string>(p_columnName));
        }

        /// <summary>
        /// 指定列的字符串值是否为0
        /// </summary>
        /// <param name="p_index">索引</param>
        /// <returns>true 表示列值为空</returns>
        public bool IsZero(int p_index)
        {
            return GetVal<string>(p_index) == "0";
        }

        /// <summary>
        /// 指定列的字符串值是否为0
        /// </summary>
        /// <param name="p_columnName">列名</param>
        /// <returns>true 表示列值为空</returns>
        public bool IsZero(string p_columnName)
        {
            return GetVal<string>(p_columnName) == "0";
        }

        /// <summary>
        /// 指定列的值是否为1或true
        /// </summary>
        /// <param name="p_index">索引</param>
        /// <returns>true 表示列值为空</returns>
        public bool Bool(int p_index)
        {
            return GetVal<bool>(p_index);
        }

        /// <summary>
        /// 指定列的值是否为1或true
        /// </summary>
        /// <param name="p_columnName">列名</param>
        /// <returns>true 表示列值为空</returns>
        public bool Bool(string p_columnName)
        {
            return GetVal<bool>(p_columnName);
        }

        /// <summary>
        /// 根据索引获取指定列的double值，为null时返回零即default(double)！！！
        /// </summary>
        /// <param name="p_index">索引</param>
        /// <returns>double值</returns>
        public double Double(int p_index)
        {
            return GetVal<double>(p_index);
        }

        /// <summary>
        /// 根据列名获取指定列的double值，为null时返回零即default(double)！！！
        /// </summary>
        /// <param name="p_columnName">列名</param>
        /// <returns>double值</returns>
        public double Double(string p_columnName)
        {
            return GetVal<double>(p_columnName);
        }

        /// <summary>
        /// 根据索引获取指定列的整数值，为null时返回零即default(int)！！！
        /// </summary>
        /// <param name="p_index">索引</param>
        /// <returns>整数值</returns>
        public int Int(int p_index)
        {
            return GetVal<int>(p_index);
        }

        /// <summary>
        /// 根据列名获取指定列的整数值，为null时返回零即default(int)！！！
        /// </summary>
        /// <param name="p_columnName">列名</param>
        /// <returns>整数值</returns>
        public int Int(string p_columnName)
        {
            return GetVal<int>(p_columnName);
        }

        /// <summary>
        /// 根据索引获取指定列的64位整数值，为null时返回零即default(long)！！！
        /// </summary>
        /// <param name="p_index">索引</param>
        /// <returns>整数值</returns>
        public long Long(int p_index)
        {
            return GetVal<long>(p_index);
        }

        /// <summary>
        /// 根据列名获取指定列的64位整数值，为null时返回零即default(long)！！！
        /// </summary>
        /// <param name="p_columnName">列名</param>
        /// <returns>整数值</returns>
        public long Long(string p_columnName)
        {
            return GetVal<long>(p_columnName);
        }

        /// <summary>
        /// 根据索引获取指定列的日期值，为null时返回DateTime.MinValue，即default(DateTime)！！！
        /// </summary>
        /// <param name="p_index">索引</param>
        /// <returns>日期值</returns>
        public DateTime Date(int p_index)
        {
            return GetVal<DateTime>(p_index);
        }

        /// <summary>
        /// 根据列名获取指定列的日期值，为null时返回DateTime.MinValue，即default(DateTime)！！！
        /// </summary>
        /// <param name="p_columnName">列名</param>
        /// <returns>日期值</returns>
        public DateTime Date(string p_columnName)
        {
            return GetVal<DateTime>(p_columnName);
        }

        /// <summary>
        /// 根据索引获取指定列的可空类型值
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="p_index">索引</param>
        /// <returns>可空类型值</returns>
        public T? Nullable<T>(int p_index)
            where T : struct
        {
            return GetVal<T?>(p_index);
        }

        /// <summary>
        /// 根据列名获取指定列的可空类型值
        /// </summary>
        /// <typeparam name="T">值类型</typeparam>
        /// <param name="p_columnName">列名</param>
        /// <returns>可空类型值</returns>
        public T? Nullable<T>(string p_columnName)
            where T : struct
        {
            return GetVal<T?>(p_columnName);
        }

        /// <summary>
        /// 根据索引获取指定列的值，若指定类型和当前类型匹配：
        /// <para>string类型为null时返回string.Empty</para>
        /// <para>其它类型为null时返回default(T)，即引用类型返回 null，数值类型会返回零</para>
        /// <para>有其它需要时请自行处理</para>
        /// <para>另外，只提供从其它类型到string类型转换</para>
        /// </summary>
        /// <typeparam name="T">返回值的类型</typeparam>
        /// <param name="p_index">索引</param>
        /// <returns>返回已转换为指定类型的值</returns>
        public T GetVal<T>(int p_index)
        {
            if (p_index < 0 || p_index >= _cells.Count)
                throw new Exception(string.Format(_indexError, p_index, _cells.Count));
            var cell = _cells[p_index];
            if (cell != null)
            {
                return cell.GetVal<T>();
            }
            return default(T);
        }

        /// <summary>
        /// 根据列名获取指定列的值，若指定类型和当前类型匹配：
        /// <para>string类型为null时返回string.Empty</para>
        /// <para>其它类型为null时返回default(T)，即引用类型返回 null，数值类型会返回零</para>
        /// <para>有其它需要时请自行处理</para>
        /// <para>另外，只提供从其它类型到string类型转换</para>
        /// </summary>
        /// <typeparam name="T">返回值的类型</typeparam>
        /// <param name="p_columnName">列名</param>
        /// <returns>返回已转换为指定类型的值</returns>
        public T GetVal<T>(string p_columnName)
        {
            if (string.IsNullOrEmpty(p_columnName))
                throw new Exception("获取列值时列名不能为空！");
            var cell = _cells[p_columnName];
            if (cell != null)
            {
                return cell.GetVal<T>();
            }
            return default(T);
        }

        /// <summary>
        /// 根据列名获取指定列的原始值
        /// </summary>
        /// <typeparam name="T">返回值的类型</typeparam>
        /// <param name="p_columnName">列名</param>
        /// <returns>返回已转换为指定类型的值</returns>
        public T GetOriginalVal<T>(string p_columnName)
        {
            if (string.IsNullOrEmpty(p_columnName))
                throw new Exception("获取列值时列名不能为空！");
            var cell = _cells[p_columnName];
            if (cell != null)
            {
                return cell.GetOriginalVal<T>();
            }
            return default(T);
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 检查当前行数据是否有变化，同时更新IsChanged属性
        /// </summary>
        internal void CheckChanges()
        {
            if (_delayCheckChanges)
                return;

            bool changed = false;
            foreach (var cell in _cells)
            {
                if (cell.IsChanged)
                {
                    changed = true;
                    break;
                }
            }
            // 触发连带判断
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
        #endregion

        #region IRpcJson
        void IRpcJson.ReadRpcJson(ref Utf8JsonReader p_reader)
        {
            try
            {
                // 可能为状态或 {
                p_reader.Read();
                // 状态
                if (p_reader.TokenType == JsonTokenType.String)
                {
                    string state = p_reader.GetString();
                    if (state == "Added")
                        IsAdded = true;
                    else if (state == "Modified")
                        IsChanged = true;
                    // {
                    p_reader.Read();
                }

                bool isEntity = GetType().IsSubclassOf(typeof(Entity));
                while (p_reader.Read() && p_reader.TokenType == JsonTokenType.PropertyName)
                {
                    string id = p_reader.GetString();
                    p_reader.Read();
                    if (p_reader.TokenType == JsonTokenType.StartArray)
                    {
                        // 数组内容： ["类型", "当前值", "原始值"] 或 ["类型", "当前值"]

                        // 类型
                        p_reader.Read();
                        Type type = Table.GetColType(p_reader.GetString());
                        if (type == typeof(byte) && isEntity)
                        {
                            // Entity 时根据属性类型将 byte 自动转为 enum 类型
                            var prop = GetType().GetProperty(id, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.IgnoreCase);
                            if (prop != null)
                                type = prop.PropertyType;
                        }

                        // 当前值
                        p_reader.Read();
                        object val = JsonRpcSerializer.Deserialize(ref p_reader, type);

                        p_reader.Read();
                        if (p_reader.TokenType == JsonTokenType.EndArray)
                        {
                            // 非string类型，值无变化 ["类型", "当前值"]
                            new Cell(this, id, type, val);
                        }
                        else
                        {
                            // 值变化时传递完整信息 ["类型", "当前值", "原始值"]
                            new Cell(this, id, type, JsonRpcSerializer.Deserialize(ref p_reader, type)).SetVal(val);
                            // ]
                            p_reader.Read();
                        }
                    }
                    else
                    {
                        // string类型，值无变化
                        new Cell(this, id, typeof(string), JsonRpcSerializer.Deserialize(ref p_reader));
                    }
                }
                // 最外层 ]
                p_reader.Read();
                AttachHook();
            }
            catch (Exception ex)
            {
                throw new Exception($"反序列化Row时异常: {ex.Message}");
            }
        }

        /// <summary>
        /// 反序列化时Entity附加Hook方法
        /// </summary>
        protected virtual void AttachHook()
        {
        }

        void IRpcJson.WriteRpcJson(Utf8JsonWriter p_writer)
        {
            p_writer.WriteStartArray();
            p_writer.WriteStringValue("#row");

            // 行状态
            if (IsAdded)
                p_writer.WriteStringValue("Added");
            else if (IsChanged)
                p_writer.WriteStringValue("Modified");

            p_writer.WriteStartObject();
            foreach (var cell in _cells)
            {
                p_writer.WritePropertyName(cell.ID);
                if (cell.IsChanged)
                {
                    // 值变化时传递完整信息 ["类型", "当前值", "原始值"]
                    p_writer.WriteStartArray();
                    p_writer.WriteStringValue(Table.GetColTypeAlias(cell.Type));
                    JsonRpcSerializer.Serialize(cell.Val, p_writer);
                    JsonRpcSerializer.Serialize(cell.OriginalVal, p_writer);
                    p_writer.WriteEndArray();
                }
                else if (cell.Type == typeof(string))
                {
                    // string类型，值无变化
                    p_writer.WriteStringValue((string)cell.Val);
                }
                else
                {
                    // 非string类型，值无变化 ["类型", "当前值"]
                    p_writer.WriteStartArray();
                    p_writer.WriteStringValue(Table.GetColTypeAlias(cell.Type));
                    JsonRpcSerializer.Serialize(cell.Val, p_writer);
                    p_writer.WriteEndArray();
                }
            }
            p_writer.WriteEndObject();

            p_writer.WriteEndArray();
        }
        #endregion

        #region INotifyPropertyChanged
        /// <summary>
        /// 属性 IsChanged 变化事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
