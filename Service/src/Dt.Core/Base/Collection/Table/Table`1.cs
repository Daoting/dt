﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-12 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// 创建空Table
        /// </summary>
        /// <returns>空表</returns>
        public static Table<TEntity> Create()
        {
            var model = EntitySchema.Get(typeof(TEntity));
            var tbl = new Table<TEntity>();
            foreach (var col in model.Schema.PrimaryKey.Concat(model.Schema.Columns))
            {
                tbl.Columns.Add(new Column(col.Name, col.Type));
            }
            return tbl;
        }

        /// <summary>
        /// 实体列表，只为在 linq 中能识别实体类型用！如：
        /// <para>from item in _atvs.Items</para>
        /// </summary>
        new public IList<TEntity> Items => this;

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