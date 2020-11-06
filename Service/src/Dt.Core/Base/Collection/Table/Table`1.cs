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
using System.Collections.Specialized;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 泛型数据表，因涉及对UI的MVVM支持、序列化支持、DDD支持，设计时继承关系比较诡异，正常应该为Table的基类！
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    public class Table<TEntity> : Table, IList<TEntity>
        where TEntity : Entity
    {
        /// <summary>
        /// 创建行
        /// </summary>
        /// <returns></returns>
        protected override Row CreateRowInstance()
        {
            // 实体的无参数构造方法为private
            return (TEntity)Activator.CreateInstance(typeof(TEntity), true);
        }

        #region 记录删除行
        /// <summary>
        /// 开始记录被删除的行，IsAdded为true的行不参加，当被删除的行重新添加时移除记录
        /// 可通过 DeletedList 属性获得所有被删除的行
        /// </summary>
        public override void StartRecordDelRows()
        {
            DeletedList = new List<TEntity>();
            CollectionChanged += OnCollectionChanged;
        }

        /// <summary>
        /// 停止记录被删除的行
        /// </summary>
        public override void StopRecordDelRows()
        {
            DeletedList = null;
            CollectionChanged -= OnCollectionChanged;
        }

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add
                && e.NewItems[0] is TEntity row)
            {
                if (!row.IsAdded)
                    DeletedList.Remove(row);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove
                && e.OldItems[0] is TEntity row2)
            {
                if (!row2.IsAdded)
                    DeletedList.Add(row2);
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                DeletedList.Clear();
            }
        }

        /// <summary>
        /// 获取已被删除的行列表
        /// </summary>
        new public List<TEntity> DeletedList { get; private set; }

        /// <summary>
        /// 获取是否存在被删除的行
        /// </summary>
        public override bool ExistDeleted
        {
            get { return DeletedList != null && DeletedList.Count > 0; }
        }
        #endregion

        #region IList<TEntity>
        /// <summary>
        /// 通过索引获取的类型为TRow
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        new public TEntity this[int index] { get => (TEntity)base[index]; set => base[index] = value; }

        /// <summary>
        /// 确保 foreach 时类型为TRow
        /// </summary>
        /// <returns></returns>
        new public IEnumerator<TEntity> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return (TEntity)base[i];
            }
        }

        public bool IsReadOnly => false;

        public void Add(TEntity item)
        {
            base.Add(item);
        }

        public bool Contains(TEntity item)
        {
            return base.Contains(item);
        }

        public void CopyTo(TEntity[] array, int arrayIndex)
        {
            base.CopyTo(array, arrayIndex);
        }

        public int IndexOf(TEntity item)
        {
            return base.IndexOf(item);
        }

        public void Insert(int index, TEntity item)
        {
            base.Insert(index, item);
        }

        public bool Remove(TEntity item)
        {
            return base.Remove(item);
        }
        #endregion
    }
}