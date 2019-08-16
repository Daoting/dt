#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using static Dt.Core.TableKit;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 数据表，行集合(行为同构)，提供简单的数据表管理功能
    /// </summary>
    public class Table : ObservableCollection<Row>, IRpcJson, ITreeData
    {
        #region 成员变量
        readonly ColumnList _columns;
        bool _isChanged = false;
        bool _delayCheckChanges = false;
        int _updating;
        #endregion

        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
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
        /// 获取设置表名
        /// </summary>
        public string Name { get; set; }

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
        /// 根据表名创建空Table
        /// </summary>
        /// <param name="p_tblName">表名</param>
        /// <returns></returns>
        public static Table Create(string p_tblName)
        {
            if (string.IsNullOrEmpty(p_tblName))
                Throw("表名不可为空！");

            Table tbl = new Table();
            tbl.Name = p_tblName.ToLower();
            foreach (var col in AtLocal.QueryColumns(p_tblName))
            {
                tbl.Columns.Add(new Column(col.ColName, TableKit.GetType(col.DbType)));
            }
            return tbl;
        }

        /// <summary>
        /// 根据本地库表名创建空Table
        /// </summary>
        /// <param name="p_tblName">本地库表名</param>
        /// <returns></returns>
        public static Table CreateLocal(string p_tblName)
        {
            if (string.IsNullOrEmpty(p_tblName))
                Throw("表名不可为空！");

            Table tbl = AtLocal.Query($"select * from {p_tblName} where 1!=1");
            tbl.Name = p_tblName.ToLower();
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
        #endregion

        #region Xml创建表
        /// <summary>
        /// 根据Xml文件内容创建Table对象
        /// </summary>
        /// <param name="p_file"></param>
        /// <returns></returns>
        public static Table CreateFromXmlFile(string p_file)
        {
            if (string.IsNullOrEmpty(p_file))
                return null;

            Table tbl = new Table();
            using (Stream stream = File.OpenRead(p_file))
            using (XmlReader reader = XmlReader.Create(stream, ReaderSettings))
            {
                tbl.ReadRpcXml(reader);
            }
            return tbl;
        }

        /// <summary>
        /// 由Xml字符串创建Table对象
        /// </summary>
        /// <param name="p_xml"></param>
        /// <returns></returns>
        public static Table CreateFromXml(string p_xml)
        {
            Table tbl = new Table();
            using (StringReader stream = new StringReader(p_xml))
            using (XmlReader reader = XmlReader.Create(stream, ReaderSettings))
            {
                tbl.ReadRpcXml(reader);
            }
            return tbl;
        }

        /// <summary>
        /// 根据嵌入资源的Xml文件内容创建Table对象
        /// </summary>
        /// <param name="p_assembly">嵌入资源所属的程序集</param>
        /// <param name="p_path">资源路径</param>
        /// <returns></returns>
        public static Table CreateFromXmlRes(Assembly p_assembly, string p_path)
        {
            if (p_assembly == null || string.IsNullOrEmpty(p_path))
                return null;

            Table tbl = new Table();
            using (Stream stream = p_assembly.GetManifestResourceStream(p_path))
            using (XmlReader reader = XmlReader.Create(stream, ReaderSettings))
            {
                tbl.ReadRpcXml(reader);
            }
            return tbl;
        }

        /// <summary>
        /// 读取xml中的行数据，根元素和行元素名称任意，xml内容形如：
        /// Params>
        ///   Param id="参数标识" name="参数名" type="double" />
        /// Params>
        /// </summary>
        /// <param name="p_reader"></param>
        public void ReadXml(XmlReader p_reader)
        {
            if (p_reader == null)
                return;

            if (p_reader.IsEmptyElement)
            {
                p_reader.Read();
                return;
            }

            string root = p_reader.Name;
            p_reader.Read();
            string rowName = p_reader.Name;
            while (p_reader.NodeType != XmlNodeType.None)
            {
                if (p_reader.NodeType == XmlNodeType.EndElement && p_reader.Name == root)
                    break;

                Row row = NewRow();
                row.IsAdded = false;
                for (int i = 0; i < p_reader.AttributeCount; i++)
                {
                    p_reader.MoveToAttribute(i);
                    string id = p_reader.Name;
                    if (row.Cells.Contains(id))
                        row.Cells[id].InitVal(p_reader.Value);
                }

                p_reader.ReadToNextSibling(rowName);
            }
            p_reader.Read();
        }

        /// <summary>
        /// 按指定元素名称序列化行数据xml
        /// </summary>
        /// <param name="p_writer"></param>
        /// <param name="p_rootName">根节点名称</param>
        /// <param name="p_rowName">行节点名称</param>
        public void WriteXml(XmlWriter p_writer, string p_rootName, string p_rowName)
        {
            p_writer.WriteStartElement(p_rootName);
            foreach (Row row in this)
            {
                p_writer.WriteStartElement(p_rowName);
                foreach (var cell in row.Cells)
                {
                    // 为空或与默认值相同时不输出
                    if (cell.Val == null)
                        continue;

                    // 列值以属性输出
                    if (cell.Type == typeof(DateTime))
                        p_writer.WriteAttributeString(cell.ID, ((DateTime)cell.Val).ToString("yyyy-MM-ddTHH:mm:ss.ffffff"));
                    else
                        p_writer.WriteAttributeString(cell.ID, cell.Val.ToString());
                }
                p_writer.WriteEndElement();
            }
            p_writer.WriteEndElement();
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
            if (_columns.Count > 0)
            {
                if (p_params.Length < 2)
                {
                    // 空行
                    foreach (var col in _columns)
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
                    for (int i = 0; i < _columns.Count; i++)
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

        /// <summary>
        /// 依次调整指定列的值，拖拽调序时用
        /// </summary>
        /// <param name="p_srcIndex">被拖拽的行</param>
        /// <param name="p_tgtIndex">要放入的行</param>
        /// <param name="p_colName">要依次调整值的列名</param>
        /// <returns></returns>
        public bool ExchangeVal(int p_srcIndex, int p_tgtIndex, string p_colName)
        {
            if (p_srcIndex < 0
                || p_srcIndex >= this.Count
                || p_tgtIndex < 0
                || p_tgtIndex >= this.Count
                || p_srcIndex == p_tgtIndex
                || string.IsNullOrEmpty(p_colName)
                || !Columns.Contains(p_colName))
                return false;

            if (p_srcIndex > p_tgtIndex)
            {
                // 向上拖拽行
                object max = this[p_srcIndex][p_colName];
                this[p_srcIndex][p_colName] = this[p_tgtIndex][p_colName];
                for (int i = p_tgtIndex; i < p_srcIndex - 1; i++)
                {
                    this[i][p_colName] = this[i + 1][p_colName];
                }
                this[p_srcIndex - 1][p_colName] = max;
            }
            else
            {
                // 向下拖拽行
                object min = this[p_srcIndex][p_colName];
                this[p_srcIndex][p_colName] = this[p_tgtIndex][p_colName];
                for (int i = p_tgtIndex; i > p_srcIndex + 1; i--)
                {
                    this[i]["dispidx"] = this[i - 1][p_colName];
                }
                this[p_srcIndex + 1]["dispidx"] = min;
            }
            return true;
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
            Table tbl = new Table();
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

        /// <summary>
        /// 查找某行的指定一列或多列值相同的第一个匹配数据行，
        /// 但不是传入的数据行本身。
        /// </summary>
        /// <param name="p_dr">指定的数据行</param>
        /// <param name="p_colNames">匹配的列名数组。</param>
        /// <returns>返回相同数据行的行号，无匹配返回 -1</returns>
        public int FirstSameRow(Row p_dr, params string[] p_colNames)
        {
            int i;
            int colsCount = p_colNames.Length;
            if (colsCount == 0)
                Throw("未指定查询列名。");
            string[] vals = new string[colsCount];
            for (i = 0; i < colsCount; i++)
            {
                if (!Columns.Contains(p_colNames[i]) || !p_dr.Contains(p_colNames[i]))
                    Throw("指定的列名不存在。");
                vals[i] = p_dr.Str(p_colNames[i]);
            }
            foreach (Row dr in this)
            {
                for (i = 0; i < colsCount; i++)
                {
                    if (!dr.Str(p_colNames[i]).Equals(vals[i]))
                    {
                        break;
                    }
                }
                if (i != colsCount)  //有列未匹配，比较下一行
                    continue;
                else//所有列都匹配了
                {
                    if (this.Contains(p_dr) && dr == p_dr)//是传入的行则继续查找比较下一行
                        continue;
                    else
                        return dr.Index;
                }
            }
            return -1;
        }

        /// <summary>
        /// 值数据源中查找指定列名匹配值的第一行，返回行号，支持从指定行号开始查找。
        /// </summary>
        /// <param name="p_colNames">匹配的列名</param>
        /// <param name="p_vals">匹配值</param>
        /// <param name="p_startRow">起始行，默认从第0行开始。</param>
        /// <returns>返回查找行的行号，无匹配返回-1</returns>
        public int FindFistRow(string[] p_colNames, string[] p_vals, int p_startRow = 0)
        {
            int colsCount = p_colNames.Length;
            int i, j;
            if (colsCount == 0)
                Throw("未指定查询列名。");
            if (colsCount != p_vals.Length)
                Throw("查询列名数与查询值数目不等。");
            for (i = 0; i < colsCount; i++)
            {
                if (!Columns.Contains(p_colNames[i]))
                    Throw("指定的列名不存在。");
            }
            for (i = p_startRow; i < this.Count; i++)
            {
                for (j = 0; j < colsCount; j++)
                {
                    if (!this[i].Str(p_colNames[j]).Equals(p_vals[j]))
                        break;
                }
                if (j != colsCount)  //有列未匹配，比较下一行
                    continue;
                else//所有列都匹配了
                    return i;
            }
            return -1;
        }

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
                    dr = tbl.NewRow(rowHeader);
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

        /// <summary>
        /// 获取所有行的实体包装对象
        /// </summary>
        public IEnumerable<T> Defs<T>() where T : RowEntity
        {
            return from item in this
                   select item.Def<T>();
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
        /// 判断行是否为异构
        /// </summary>
        /// <param name="p_row"></param>
        bool IsSameSchema(Row p_row)
        {
            if (_columns.Count == p_row.Cells.Count)
            {
                // 判断新增行是否为异构
                bool same = true;
                if (_columns.Count > 0)
                {
                    for (int i = 0; i < _columns.Count; i++)
                    {
                        var col = _columns[i];
                        var cell = p_row.Cells[i];
                        if (col.ID != cell.ID || col.Type != cell.Type)
                        {
                            same = false;
                            break;
                        }
                    }
                }
                return same;
            }
            return false;
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

        #region IRpcXml
        /// <summary>
        /// 反序列化读取Rpc XML数据
        /// </summary>
        /// <param name="p_reader"></param>
        public void ReadRpcXml(XmlReader p_reader)
        {
            // <Table>
            p_reader.Read();
            // 表名
            Name = p_reader.GetAttribute("id");
            // Cols
            p_reader.Read();

            // 列反序列化
            if (p_reader.Name == "Cols" && !p_reader.IsEmptyElement)
            {
                p_reader.Read();
                while (p_reader.NodeType != XmlNodeType.None)
                {
                    if (p_reader.NodeType == XmlNodeType.EndElement && p_reader.Name == "Cols")
                        break;

                    string colName = null;
                    Type colType = null;
                    for (int i = 0; i < p_reader.AttributeCount; i++)
                    {
                        p_reader.MoveToAttribute(i);
                        string name = p_reader.Name;
                        string val = p_reader.Value;
                        if (name == "id")
                            colName = val;
                        else if (name == "type")
                            colType = TableKit.GetType(val);
                    }
                    _columns.Add(new Column(colName, colType));
                    p_reader.ReadToNextSibling("Col");
                }
            }
            p_reader.Read();

            // 行反序列化
            if (p_reader.Name == "Rows" && !p_reader.IsEmptyElement)
            {
                p_reader.Read();
                while (p_reader.NodeType != XmlNodeType.None)
                {
                    if (p_reader.NodeType == XmlNodeType.EndElement && p_reader.Name == "Rows")
                        break;

                    Row row = NewRow();
                    // 置未修改状态
                    row.IsAdded = false;
                    // 列值
                    for (int i = 0; i < p_reader.AttributeCount; i++)
                    {
                        p_reader.MoveToAttribute(i);
                        string id = p_reader.Name;
                        if (row.Cells.Contains(id))
                        {
                            // 不会引起连续IsChanged改变
                            row.Cells[id].InitVal(p_reader.Value);
                        }
                    }
                    p_reader.ReadToNextSibling("Row");
                }
            }
            p_reader.Read();
            p_reader.Read();
        }

        /// <summary>
        /// 将对象按照Rpc Xml数据结构进行序列化
        /// </summary>
        /// <param name="p_writer"></param>
        public void WriteRpcXml(XmlWriter p_writer)
        {
            p_writer.WriteStartElement("struct");
            p_writer.WriteAttributeString("type", "tbl");
            p_writer.WriteStartElement("Table");
            if (!string.IsNullOrEmpty(Name))
                p_writer.WriteAttributeString("id", Name);

            p_writer.WriteStartElement("Cols");
            foreach (Column column in _columns)
            {
                p_writer.WriteStartElement("Col");
                p_writer.WriteAttributeString("id", column.ID);
                if (column.Type != typeof(string))
                    p_writer.WriteAttributeString("type", TableKit.GetAlias(column.Type));
                p_writer.WriteEndElement();
            }
            p_writer.WriteEndElement();

            p_writer.WriteStartElement("Rows");
            if (SerializeChanged)
            {
                // 只序列化需要增加修改的行
                foreach (Row row in this)
                {
                    if (row.IsChanged)
                        SerializeXmlRow(row, p_writer);
                }
            }
            else
            {
                foreach (Row row in this)
                {
                    SerializeXmlRow(row, p_writer);
                }
            }
            p_writer.WriteEndElement();

            p_writer.WriteEndElement();
            p_writer.WriteEndElement();
        }

        /// <summary>
        /// 序列化行数据
        /// </summary>
        /// <param name="p_dataRow"></param>
        /// <param name="p_writer"></param>
        void SerializeXmlRow(Row p_dataRow, XmlWriter p_writer)
        {
            p_writer.WriteStartElement("Row");
            // 行状态
            if (p_dataRow.IsAdded)
                p_writer.WriteAttributeString("rowState", "Added");
            else if (p_dataRow.IsChanged)
                p_writer.WriteAttributeString("rowState", "Modified");

            foreach (var cell in p_dataRow.Cells)
            {
                if (cell.Val != null)
                {
                    // 列值以属性输出
                    if (cell.Type == typeof(DateTime))
                        p_writer.WriteAttributeString(cell.ID, ((DateTime)cell.Val).ToString("yyyy-MM-ddTHH:mm:ss.ffffff"));
                    else if (cell.Type == typeof(byte[]))
                        p_writer.WriteAttributeString(cell.ID, Convert.ToBase64String((byte[])cell.Val));
                    else
                        p_writer.WriteAttributeString(cell.ID, cell.Val.ToString());
                }
            }
            p_writer.WriteEndElement();
        }
        #endregion

        #region IRpcJson
        /// <summary>
        /// 反序列化读取Rpc Json数据
        /// </summary>
        public void ReadRpcJson(JsonReader p_reader)
        {
            // 可能为id或cols外层 [
            p_reader.Read();
            // 表名id
            if (p_reader.TokenType == JsonToken.String)
            {
                Name = p_reader.Value.ToString();
                // cols外层 [
                p_reader.Read();
            }

            // 列
            // [
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
                        colType = TableKit.GetType(p_reader.Value.ToString());
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

        /// <summary>
        /// 将对象按照Rpc Json数据结构进行序列化
        /// </summary>
        public void WriteRpcJson(JsonWriter p_writer)
        {
            p_writer.WriteStartArray();
            // 类型
            p_writer.WriteValue("#tbl");
            // 表名id
            if (!string.IsNullOrEmpty(Name))
                p_writer.WriteValue(Name);

            // 列
            p_writer.WriteStartArray();
            foreach (Column column in _columns)
            {
                p_writer.WriteStartArray();
                p_writer.WriteValue(column.ID);
                if (column.Type != typeof(string))
                    p_writer.WriteValue(TableKit.GetAlias(column.Type));
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

        #region ITreeData
        IEnumerable<object> ITreeData.GetTreeRoot()
        {
            // 固定字段 id, parentid
            if (_columns.Contains("parentid") && Count > 0)
            {
                return from row in this
                       where row.Str("parentid") == string.Empty
                       select row;
            }
            return null;
        }

        IEnumerable<object> ITreeData.GetTreeItemChildren(object p_parent)
        {
            Row parent = p_parent as Row;
            if (parent != null && parent.Contains("id"))
            {
                return from row in this
                       where row.Str("parentid") == parent.Str("id")
                       select row;
            }
            return null;
        }
        #endregion
    }
}
