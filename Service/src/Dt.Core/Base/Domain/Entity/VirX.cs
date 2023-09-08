#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-19 创建
******************************************************************************/
#endregion

#region 引用命名
using System.ComponentModel;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 含两个一对一实体的虚拟实体，内部实体的限制：
    /// <para>1. 单主键，且主键名称相同</para>
    /// <para>2. 不同实体的属性名不重复</para>
    /// </summary>
    /// <typeparam name="TEntity1"></typeparam>
    /// <typeparam name="TEntity2"></typeparam>
    public class VirX<TEntity1, TEntity2> : EntityX<VirX<TEntity1, TEntity2>>, IVirEntity
        where TEntity1 : Entity
        where TEntity2 : Entity
    {
        VirX()
        {
            E1 = To<TEntity1>();
            E2 = To<TEntity2>();

            // 和内部实体同步IsChanged状态
            PropertyChanged += VirObj_PropertyChanged;
            E1.PropertyChanged += E1_PropertyChanged;
            E2.PropertyChanged += E2_PropertyChanged;
        }

        /// <summary>
        /// 创建虚拟实体，包括内部实体的Cell并设置默认值，主键为long类型时自动赋值
        /// </summary>
        /// <param name="p_isEmpty">是否为空结构，false时创建所有内部实体的Cell并设置默认值</param>
        /// <returns></returns>
        public static async Task<VirX<TEntity1, TEntity2>> New(bool p_isEmpty = false)
        {
            var x = new VirX<TEntity1, TEntity2>();
            if (!p_isEmpty)
            {
                await AddEntityCells(x, typeof(TEntity1));
                await AddEntityCells(x, typeof(TEntity2));

                var col = (await EntitySchema.Get(typeof(TEntity1))).Schema.PrimaryKey[0];
                if (col.Type == typeof(long))
                {
                    // 设置主键值
                    x.E1.Cells[col.Name].InitVal(await NewID());
                }
                x.IsAdded = true;
            }
            return x;
        }

        /// <summary>
        /// 第一个实体对象
        /// </summary>
        public TEntity1 E1 { get; }

        /// <summary>
        /// 第二个实体对象
        /// </summary>
        public TEntity2 E2 { get; }

        /// <summary>
        /// 获取设置当前行是否为新增
        /// </summary>
        public override bool IsAdded
        {
            get { return E1.IsAdded; }
            set
            {
                E1.IsAdded = value;
                E2.IsAdded = value;
            }
        }

        /// <summary>
        /// 获取Cell.Val值变化前的回调方法，查找内部实体
        /// </summary>
        /// <param name="p_id">Cell.ID</param>
        /// <returns></returns>
        public override Action<CellValChangingArgs> GetCellHook(string p_id)
        {
            var hook = E1.GetCellHook(p_id);
            if (hook == null)
                hook = E2.GetCellHook(p_id);
            return hook;
        }

        public List<Entity> GetEntities()
        {
            return new List<Entity> { E1, E2 };
        }

        void VirObj_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChanged")
            {
                // 和内部实体同步IsChanged状态
                E1.IsChanged = IsChanged;
                E2.IsChanged = IsChanged;
            }
        }

        void E1_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // 当内部实体保存后AcceptChanges时，需要重新检查虚拟实体的IsChanged状态
            if (e.PropertyName == "IsChanged" && !E1.IsChanged)
            {
                CheckChanges();
            }
        }

        void E2_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChanged" && !E2.IsChanged)
            {
                CheckChanges();
            }
        }

    }

    /// <summary>
    /// 含三个一对一实体的虚拟实体，内部实体的限制：
    /// <para>1. 单主键，且主键名称相同</para>
    /// <para>2. 不同实体的属性名不重复</para>
    /// </summary>
    /// <typeparam name="TEntity1"></typeparam>
    /// <typeparam name="TEntity2"></typeparam>
    /// <typeparam name="TEntity3"></typeparam>
    public class VirX<TEntity1, TEntity2, TEntity3> : EntityX<VirX<TEntity1, TEntity2, TEntity3>>, IVirEntity
        where TEntity1 : Entity
        where TEntity2 : Entity
        where TEntity3 : Entity
    {
        VirX()
        {
            E1 = To<TEntity1>();
            E2 = To<TEntity2>();
            E3 = To<TEntity3>();

            // 和内部实体同步IsChanged状态
            PropertyChanged += VirObj_PropertyChanged;
            E1.PropertyChanged += E1_PropertyChanged;
            E2.PropertyChanged += E2_PropertyChanged;
            E3.PropertyChanged += E3_PropertyChanged;
        }

        /// <summary>
        /// 创建虚拟实体，包括内部实体的Cell并设置默认值，主键为long类型时自动赋值
        /// </summary>
        /// <param name="p_isEmpty">是否为空结构，默认false，false时创建所有内部实体的Cell并设置默认值</param>
        /// <returns></returns>
        public static async Task<VirX<TEntity1, TEntity2, TEntity3>> New(bool p_isEmpty = false)
        {
            var x = new VirX<TEntity1, TEntity2, TEntity3>();
            if (!p_isEmpty)
            {
                await AddEntityCells(x, typeof(TEntity1));
                await AddEntityCells(x, typeof(TEntity2));
                await AddEntityCells(x, typeof(TEntity3));

                var col = (await EntitySchema.Get(typeof(TEntity1))).Schema.PrimaryKey[0];
                if (col.Type == typeof(long))
                {
                    // 设置主键值
                    x.E1.Cells[col.Name].InitVal(await NewID());
                }
                x.IsAdded = true;
            }
            return x;
        }

        /// <summary>
        /// 第一个实体对象
        /// </summary>
        public TEntity1 E1 { get; }

        /// <summary>
        /// 第二个实体对象
        /// </summary>
        public TEntity2 E2 { get; }

        /// <summary>
        /// 第三个实体对象
        /// </summary>
        public TEntity3 E3 { get; }

        /// <summary>
        /// 获取设置当前行是否为新增
        /// </summary>
        public override bool IsAdded
        {
            get { return E1.IsAdded; }
            set
            {
                E1.IsAdded = value;
                E2.IsAdded = value;
                E3.IsAdded = value;
            }
        }

        /// <summary>
        /// 获取Cell.Val值变化前的回调方法，查找内部实体
        /// </summary>
        /// <param name="p_id">Cell.ID</param>
        /// <returns></returns>
        public override Action<CellValChangingArgs> GetCellHook(string p_id)
        {
            var hook = E1.GetCellHook(p_id);
            if (hook == null)
                hook = E2.GetCellHook(p_id);
            if (hook == null)
                hook = E3.GetCellHook(p_id);
            return hook;
        }

        public List<Entity> GetEntities()
        {
            return new List<Entity> { E1, E2, E3 };
        }

        void VirObj_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChanged")
            {
                // 和内部实体同步IsChanged状态
                // 会造成有的实体未变化IsChanged可能为true，保存时多检查一遍
                // 否则会造成实体漏保存的情况，虽然Cell值变化但内部实体IsChanged为false！！！
                E1.IsChanged = IsChanged;
                E2.IsChanged = IsChanged;
                E3.IsChanged = IsChanged;
            }
        }

        void E1_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // 当内部实体保存后AcceptChanges时，需要重新检查虚拟实体的IsChanged状态
            if (e.PropertyName == "IsChanged" && !E1.IsChanged)
            {
                CheckChanges();
            }
        }

        void E2_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChanged" && !E2.IsChanged)
            {
                CheckChanges();
            }
        }

        void E3_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChanged" && !E3.IsChanged)
            {
                CheckChanges();
            }
        }
    }
}
