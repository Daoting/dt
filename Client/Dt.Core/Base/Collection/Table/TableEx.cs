#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-12 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 客户端数据表部分
    /// </summary>
    public partial class Table : ITreeData, INotifyList
    {
        #region 创建表结构
        static Dictionary<string, Table> _tblTemplate = new Dictionary<string, Table>();

        /// <summary>
        /// 根据表名创建空Table
        /// </summary>
        /// <param name="p_tblName">表名</param>
        /// <returns></returns>
        public static Table Create(string p_tblName)
        {
            Throw.IfNullOrEmpty(p_tblName);
            Table tbl = new Table();
            foreach (var col in AtLocal.QueryColumns(p_tblName))
            {
                tbl._columns.Add(new Column(col.ColName, GetColType(col.DbType)));
            }
            return tbl;
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
            Throw.IfNullOrEmpty(p_tblName);
            string tblName = p_tblName.ToLower();
            Table tbl;
            if (!_tblTemplate.TryGetValue(tblName, out tbl))
            {
                tbl = Create(tblName);
                _tblTemplate[tblName] = tbl;
            }
            return tbl.NewRow(p_init);
        }

        /// <summary>
        /// 根据本地库表名创建空Table
        /// </summary>
        /// <param name="p_tblName">本地库表名</param>
        /// <returns></returns>
        public static Table CreateLocal(string p_tblName)
        {
            Throw.IfNullOrEmpty(p_tblName);
            return AtLocal.Query($"select * from {p_tblName} where 1!=1");
        }
        #endregion

        #region INotifyList
        /// <summary>
        /// 在末尾批量添加数据，统一触发 NotifyCollectionChangedAction.Add
        /// </summary>
        /// <param name="p_items"></param>
        public void AddRange(IList p_items)
        {
            InsertRange(Count, p_items);
        }

        /// <summary>
        /// 在指定的索引处批量插入数据，统一触发 NotifyCollectionChangedAction.Add
        /// </summary>
        /// <param name="p_index">插入位置，范围在0到当前总数之间</param>
        /// <param name="p_items">数据行</param>
        public void InsertRange(int p_index, IList p_items)
        {
            if (p_items == null || p_items.Count == 0)
                return;

            if (p_index < 0 || p_index > Count)
                throw new Exception("数据待插入的索引超出范围！");

            // 支持外部嵌套Defer()
            int old = _updating;
            _updating = 1;
            for (int i = 0; i < p_items.Count; i++)
            {
                if (p_items[i] is Row row)
                {
                    row.Table = this;
                    base.Insert(i + p_index, row);
                }
                else
                {
                    throw new Exception("插入数据类型应为Row");
                }
            }
            _updating = 0;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, p_items, p_index));
            _updating = old;
        }

        /// <summary>
        /// 批量删除给定的行，统一触发 NotifyCollectionChangedAction.Reset
        /// </summary>
        /// <param name="p_items">数据行</param>
        /// <returns>实际删除的行数</returns>
        public int RemoveRange(IList p_items)
        {
            if (p_items == null || p_items.Count == 0)
                return 0;

            // 支持外部嵌套Defer()
            int old = _updating;
            _updating = 1;
            List<int> ls = new List<int>();
            foreach (var row in p_items.OfType<Row>())
            {
                row.Table = null;
                int index = base.IndexOf(row);
                if (index > -1)
                    ls.Add(index);
            }

            if (ls.Count > 0)
            {
                // 删除行按索引排序
                ls.Sort();
                // 从后向前删除
                for (int i = ls.Count - 1; i >= 0; i--)
                {
                    base.RemoveAt(ls[i]);
                }
                    
                CheckChanges();
                _updating = 0;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, ls));
            }
            _updating = old;
            return ls.Count;
        }

        /// <summary>
        /// 批量删除指定范围的行，统一触发 NotifyCollectionChangedAction.Remove
        /// </summary>
        /// <param name="p_index">起始索引</param>
        /// <param name="p_count">行数</param>
        public void RemoveRange(int p_index, int p_count)
        {
            if (p_index < 0
                || p_index >= Count
                || p_count <= 0
                || p_index + p_count >= Count)
                throw new Exception("删除范围的索引超出范围！");

            // 支持外部嵌套Defer()
            int old = _updating;
            _updating = 1;
            List<int> ls = new List<int>();
            // 从后向前删除
            for (int i = p_count - 1; i >= 0; i--)
            {
                ls.Insert(0, p_index + i);
                base.RemoveAt(p_index + i);
            }
            CheckChanges();
            _updating = 0;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, ls));
            _updating = old;
        }

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
            _collectionChangedArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            return new InternalCls(this);
        }

        int _updating;
        NotifyCollectionChangedEventArgs _collectionChangedArgs;

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
                    OnCollectionChanged(_collectionChangedArgs);
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

        #region ITreeData
        IEnumerable<object> ITreeData.GetTreeRoot()
        {
            // 固定字段 id, parentid
            if (_columns.Contains("parentid") && Count > 0)
            {
                // parentid类型可以为long?
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
                // id, parentid类型可以为long, string等
                return from row in this
                       where row.Str("parentid") == parent.Str("id")
                       select row;
            }
            return null;
        }
        #endregion
    }
}
