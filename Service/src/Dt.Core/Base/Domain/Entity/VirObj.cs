#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-19 创建
******************************************************************************/
#endregion

#region 引用命名

#endregion

namespace Dt.Core
{
    /// <summary>
    /// 含两个一对一实体的虚拟实体
    /// </summary>
    /// <typeparam name="TEntity1"></typeparam>
    /// <typeparam name="TEntity2"></typeparam>
    public class VirObj<TEntity1, TEntity2> : VirEntity
        where TEntity1 : Entity
        where TEntity2 : Entity
    {
        public VirObj()
        {
            E1 = To<TEntity1>();
            E2 = To<TEntity2>();
        }

        /// <summary>
        /// 第一个实体对象
        /// </summary>
        public TEntity1 E1 { get; }

        /// <summary>
        /// 第二个实体对象
        /// </summary>
        public TEntity2 E2 { get; }

        public override List<Entity> GetEntities()
        {
            return new List<Entity> { E1, E2 };
        }

        #region 静态方法
        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，仅支持单主键，当启用实体缓存时：
        /// <para>1. 首先从缓存中获取，有则直接返回</para>
        /// <para>2. 无则查询数据库，并将查询结果添加到缓存以备下次使用</para>
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<VirObj<TEntity1, TEntity2>> GetByID(string p_id)
        {
            return EntityEx.GetByID<VirObj<TEntity1, TEntity2>>(p_id);
        }

        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，仅支持单主键，当启用实体缓存时：
        /// <para>1. 首先从缓存中获取，有则直接返回</para>
        /// <para>2. 无则查询数据库，并将查询结果添加到缓存以备下次使用</para>
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<VirObj<TEntity1, TEntity2>> GetByID(long p_id)
        {
            return EntityEx.GetByID<VirObj<TEntity1, TEntity2>>(p_id);
        }

        /// <summary>
        /// 根据主键或唯一索引列获得实体对象(包含所有列值)，仅支持单主键，当启用实体缓存时：
        /// <para>1. 首先从缓存中获取，有则直接返回</para>
        /// <para>2. 无则查询数据库，并将查询结果添加到缓存以备下次使用</para>
        /// </summary>
        /// <param name="p_keyName">主键或唯一索引列名</param>
        /// <param name="p_keyVal">键值</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<VirObj<TEntity1, TEntity2>> GetByKey(string p_keyName, string p_keyVal)
        {
            return EntityEx.GetByKey<VirObj<TEntity1, TEntity2>>(p_keyName, p_keyVal);
        }

        /// <summary>
        /// 根据主键删除实体对象，仅支持单主键，删除前先根据主键获取该实体对象，并非直接删除！！！
        /// <para>删除成功后：</para>
        /// <para>1. 若存在领域事件，则发布事件</para>
        /// <para>2. 若已设置服务端缓存，则删除缓存</para>
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <param name="p_isNotify">是否提示删除结果</param>
        /// <returns>true 删除成功</returns>
        public static Task<bool> DelByID(string p_id, bool p_isNotify = true)
        {
            return EntityEx.DelByID<VirObj<TEntity1, TEntity2>>(p_id, p_isNotify);
        }

        /// <summary>
        /// 根据主键删除实体对象，仅支持单主键，删除前先根据主键获取该实体对象，并非直接删除！！！
        /// <para>删除成功后：</para>
        /// <para>1. 若存在领域事件，则发布事件</para>
        /// <para>2. 若已设置服务端缓存，则删除缓存</para>
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <param name="p_isNotify">是否提示删除结果</param>
        /// <returns>true 删除成功</returns>
        public static Task<bool> DelByID(long p_id, bool p_isNotify = true)
        {
            return EntityEx.DelByID<VirObj<TEntity1, TEntity2>>(p_id, p_isNotify);
        }

        /// <summary>
        /// 查询实体列表，每个实体包含所有列值，过滤条件null或空时返回所有实体
        /// </summary>
        /// <param name="p_filter">过滤串，where后面的部分，null或空返回所有实体</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体列表</returns>
        public static Task<Table<VirObj<TEntity1, TEntity2>>> Query(string p_filter = null, object p_params = null)
        {
            return EntityEx.Query<VirObj<TEntity1, TEntity2>>(p_filter, p_params);
        }

        /// <summary>
        /// 按页查询实体列表，每个实体包含所有列值
        /// </summary>
        /// <param name="p_starRow">起始行号：mysql中第一行为0行</param>
        /// <param name="p_pageSize">每页显示行数</param>
        /// <param name="p_filter">过滤串，where后面的部分</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体列表</returns>
        public static Task<Table<VirObj<TEntity1, TEntity2>>> Page(int p_starRow, int p_pageSize, string p_filter = null, object p_params = null)
        {
            return EntityEx.Page<VirObj<TEntity1, TEntity2>>(p_starRow, p_pageSize, p_filter, p_params);
        }

        /// <summary>
        /// 返回符合条件的第一个实体对象，每个实体包含所有列值，不存在时返回null
        /// </summary>
        /// <param name="p_filter">过滤串，where后面的部分，null或空返回所有中的第一行</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<VirObj<TEntity1, TEntity2>> First(string p_filter, object p_params = null)
        {
            return EntityEx.First<VirObj<TEntity1, TEntity2>>(p_filter, p_params);
        }
        #endregion
    }

    /// <summary>
    /// 含三个一对一实体的虚拟实体
    /// </summary>
    /// <typeparam name="TEntity1"></typeparam>
    /// <typeparam name="TEntity2"></typeparam>
    /// <typeparam name="TEntity3"></typeparam>
    public class VirObj<TEntity1, TEntity2, TEntity3> : VirEntity
        where TEntity1 : Entity
        where TEntity2 : Entity
        where TEntity3 : Entity
    {
        public VirObj()
        {
            E1 = To<TEntity1>();
            E2 = To<TEntity2>();
            E3 = To<TEntity3>();
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

        public override List<Entity> GetEntities()
        {
            return new List<Entity> { E1, E2, E3 };
        }

        #region 静态方法
        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，仅支持单主键，当启用实体缓存时：
        /// <para>1. 首先从缓存中获取，有则直接返回</para>
        /// <para>2. 无则查询数据库，并将查询结果添加到缓存以备下次使用</para>
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<VirObj<TEntity1, TEntity2, TEntity3>> GetByID(string p_id)
        {
            return EntityEx.GetByID<VirObj<TEntity1, TEntity2, TEntity3>>(p_id);
        }

        /// <summary>
        /// 根据主键获得实体对象(包含所有列值)，仅支持单主键，当启用实体缓存时：
        /// <para>1. 首先从缓存中获取，有则直接返回</para>
        /// <para>2. 无则查询数据库，并将查询结果添加到缓存以备下次使用</para>
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<VirObj<TEntity1, TEntity2, TEntity3>> GetByID(long p_id)
        {
            return EntityEx.GetByID<VirObj<TEntity1, TEntity2, TEntity3>>(p_id);
        }

        /// <summary>
        /// 根据主键或唯一索引列获得实体对象(包含所有列值)，仅支持单主键，当启用实体缓存时：
        /// <para>1. 首先从缓存中获取，有则直接返回</para>
        /// <para>2. 无则查询数据库，并将查询结果添加到缓存以备下次使用</para>
        /// </summary>
        /// <param name="p_keyName">主键或唯一索引列名</param>
        /// <param name="p_keyVal">键值</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<VirObj<TEntity1, TEntity2, TEntity3>> GetByKey(string p_keyName, string p_keyVal)
        {
            return EntityEx.GetByKey<VirObj<TEntity1, TEntity2, TEntity3>>(p_keyName, p_keyVal);
        }

        /// <summary>
        /// 根据主键删除实体对象，仅支持单主键，删除前先根据主键获取该实体对象，并非直接删除！！！
        /// <para>删除成功后：</para>
        /// <para>1. 若存在领域事件，则发布事件</para>
        /// <para>2. 若已设置服务端缓存，则删除缓存</para>
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <param name="p_isNotify">是否提示删除结果</param>
        /// <returns>true 删除成功</returns>
        public static Task<bool> DelByID(string p_id, bool p_isNotify = true)
        {
            return EntityEx.DelByID<VirObj<TEntity1, TEntity2, TEntity3>>(p_id, p_isNotify);
        }

        /// <summary>
        /// 根据主键删除实体对象，仅支持单主键，删除前先根据主键获取该实体对象，并非直接删除！！！
        /// <para>删除成功后：</para>
        /// <para>1. 若存在领域事件，则发布事件</para>
        /// <para>2. 若已设置服务端缓存，则删除缓存</para>
        /// </summary>
        /// <param name="p_id">主键</param>
        /// <param name="p_isNotify">是否提示删除结果</param>
        /// <returns>true 删除成功</returns>
        public static Task<bool> DelByID(long p_id, bool p_isNotify = true)
        {
            return EntityEx.DelByID<VirObj<TEntity1, TEntity2, TEntity3>>(p_id, p_isNotify);
        }

        /// <summary>
        /// 查询实体列表，每个实体包含所有列值，过滤条件null或空时返回所有实体
        /// </summary>
        /// <param name="p_filter">过滤串，where后面的部分，null或空返回所有实体</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体列表</returns>
        public static Task<Table<VirObj<TEntity1, TEntity2, TEntity3>>> Query(string p_filter = null, object p_params = null)
        {
            return EntityEx.Query<VirObj<TEntity1, TEntity2, TEntity3>>(p_filter, p_params);
        }

        /// <summary>
        /// 按页查询实体列表，每个实体包含所有列值
        /// </summary>
        /// <param name="p_starRow">起始行号：mysql中第一行为0行</param>
        /// <param name="p_pageSize">每页显示行数</param>
        /// <param name="p_filter">过滤串，where后面的部分</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体列表</returns>
        public static Task<Table<VirObj<TEntity1, TEntity2, TEntity3>>> Page(int p_starRow, int p_pageSize, string p_filter = null, object p_params = null)
        {
            return EntityEx.Page<VirObj<TEntity1, TEntity2, TEntity3>>(p_starRow, p_pageSize, p_filter, p_params);
        }

        /// <summary>
        /// 返回符合条件的第一个实体对象，每个实体包含所有列值，不存在时返回null
        /// </summary>
        /// <param name="p_filter">过滤串，where后面的部分，null或空返回所有中的第一行</param>
        /// <param name="p_params">参数值，支持Dict或匿名对象，默认null</param>
        /// <returns>返回实体对象或null</returns>
        public static Task<VirObj<TEntity1, TEntity2, TEntity3>> First(string p_filter, object p_params = null)
        {
            return EntityEx.First<VirObj<TEntity1, TEntity2, TEntity3>>(p_filter, p_params);
        }
        #endregion
    }
}
