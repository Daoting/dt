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
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="p_isEmpty">是否为空结构，false时创建所有内部实体的Cell并设置默认值</param>
        public VirX(bool p_isEmpty) : this()
        {
            if (!p_isEmpty)
            {
                AddEntityCells(this, typeof(TEntity1));
                AddEntityCells(this, typeof(TEntity2));
                IsAdded = true;
            }
        }

        /// <summary>
        /// 创建虚拟实体，包括内部实体的Cell并设置默认值，主键为long类型时自动赋值
        /// </summary>
        /// <returns></returns>
        public static async Task<VirX<TEntity1, TEntity2>> New()
        {
            var x = new VirX<TEntity1, TEntity2>(false);

            var col = EntitySchema.Get(typeof(TEntity1)).Schema.PrimaryKey[0];
            if (col.Type == typeof(long))
            {
                // 设置主键值
                x.E1.Cells[col.Name].InitVal(await NewID());
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
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="p_isEmpty">是否为空结构，默认false，false时创建所有内部实体的Cell并设置默认值</param>
        public VirX(bool p_isEmpty = false) : this()
        {
            if (!p_isEmpty)
            {
                AddEntityCells(this, typeof(TEntity1));
                AddEntityCells(this, typeof(TEntity2));
                AddEntityCells(this, typeof(TEntity3));
                IsAdded = true;
            }
        }

        /// <summary>
        /// 创建虚拟实体，包括内部实体的Cell并设置默认值，主键为long类型时自动赋值
        /// </summary>
        /// <returns></returns>
        public static async Task<VirX<TEntity1, TEntity2, TEntity3>> New()
        {
            var x = new VirX<TEntity1, TEntity2, TEntity3>(false);

            var col = EntitySchema.Get(typeof(TEntity1)).Schema.PrimaryKey[0];
            if (col.Type == typeof(long))
            {
                // 设置主键值
                x.E1.Cells[col.Name].InitVal(await NewID());
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

        public List<Entity> GetEntities()
        {
            return new List<Entity> { E1, E2, E3 };
        }

        void VirObj_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsChanged")
            {
                // 和内部实体同步IsChanged状态
                E1.IsChanged = IsChanged;
                E2.IsChanged = IsChanged;
                E3.IsChanged = IsChanged;
            }
        }
    }
}
