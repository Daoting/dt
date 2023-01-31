#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-09 创建
******************************************************************************/
#endregion

#region 引用命名

#endregion

namespace Dt.Core
{
    /// <summary>
    /// Entity扩展
    /// </summary>
    public static class EntityEx
    {
        #region 保存
        /// <summary>
        /// 保存实体数据，成功后：
        /// <para>1. 若存在领域事件，则发布事件</para>
        /// <para>2. 若已设置服务端缓存，则删除缓存</para>
        /// <para>3. 实体状态复位 AcceptChanges </para>
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_entity">待保存的实体</param>
        /// <param name="p_isNotify">是否提示保存结果，客户端有效</param>
        /// <returns>是否成功</returns>
        public static async Task<bool> Save<TEntity>(this TEntity p_entity, bool p_isNotify = true)
            where TEntity : Entity
        {
            var ew = new EntityWriter();
            await ew.Save(p_entity);
            return await ew.Commit(p_isNotify);
        }

        /// <summary>
        /// 一个事务内批量保存Table中新增、修改、删除的实体数据
        /// <para>删除行通过Table的ExistDeleted DeletedRows判断获取</para>
        /// <para>保存成功后，对于每个保存的实体：</para>
        /// <para>1. 若存在领域事件，则发布事件</para>
        /// <para>2. 若已设置服务端缓存，则删除缓存</para>
        /// <para>3. 实体状态复位 AcceptChanges </para>
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_tbl">实体表</param>
        /// <param name="p_isNotify">是否提示保存结果，客户端有效</param>
        /// <returns>是否成功</returns>
        public static async Task<bool> Save<TEntity>(this Table<TEntity> p_tbl, bool p_isNotify = true)
            where TEntity : Entity
        {
            var ew = new EntityWriter();
            await ew.Save(p_tbl);
            return await ew.Commit(p_isNotify);
        }

        /// <summary>
        /// 一个事务内批量保存实体数据，根据实体状态执行新增、修改
        /// <para>保存成功后，对于每个保存的实体：</para>
        /// <para>1. 若存在领域事件，则发布事件</para>
        /// <para>2. 若已设置服务端缓存，则删除缓存</para>
        /// <para>3. 实体状态复位 AcceptChanges </para>
        /// </summary>
        /// <param name="p_list">待保存列表</param>
        /// <param name="p_isNotify">是否提示保存结果，客户端有效</param>
        /// <returns>true 保存成功</returns>
        public static async Task<bool> Save<TEntity>(this IEnumerable<TEntity> p_list, bool p_isNotify = true)
            where TEntity : Entity
        {
            var ew = new EntityWriter();
            await ew.Save(p_list);
            return await ew.Commit(p_isNotify);
        }

        /// <summary>
        /// 一个事务内批量保存一对多的父实体数据和所有子实体数据
        /// <para>保存成功后，对于每个保存的实体：</para>
        /// <para>1. 若存在领域事件，则发布事件</para>
        /// <para>2. 若已设置服务端缓存，则删除缓存</para>
        /// <para>3. 实体状态复位 AcceptChanges </para>
        /// </summary>
        /// <typeparam name="TEntity">父实体类型</typeparam>
        /// <param name="p_entity">待保存的父实体</param>
        /// <param name="p_isNotify">是否提示保存结果，客户端有效</param>
        /// <returns>是否成功</returns>
        public static async Task<bool> SaveWithChild<TEntity>(this TEntity p_entity, bool p_isNotify = true)
            where TEntity : Entity
        {
            var ew = new EntityWriter();
            await ew.SaveWithChild(p_entity);
            return await ew.Commit(p_isNotify);
        }
        #endregion

        #region 删除
        /// <summary>
        /// 删除实体，依靠数据库的级联删除自动删除子实体，成功后：
        /// <para>1. 若存在领域事件，则发布事件</para>
        /// <para>2. 若已设置服务端缓存，则删除缓存</para>
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_entity">待删除的实体</param>
        /// <param name="p_isNotify">是否提示删除结果，客户端有效</param>
        /// <returns>true 删除成功</returns>
        public static async Task<bool> Delete<TEntity>(this TEntity p_entity, bool p_isNotify = true)
            where TEntity : Entity
        {
            var ew = new EntityWriter();
            await ew.Delete(p_entity);
            return await ew.Commit(p_isNotify);
        }

        /// <summary>
        /// 一个事务内批量删除实体数据，成功后对于每个删除的实体：
        /// <para>1. 若存在领域事件，则发布事件</para>
        /// <para>2. 若已设置服务端缓存，则删除缓存</para>
        /// </summary>
        /// <param name="p_list">待删除实体列表</param>
        /// <param name="p_isNotify">是否提示删除结果，客户端有效</param>
        /// <returns>true 删除成功</returns>
        public static async Task<bool> Delete<TEntity>(this IList<TEntity> p_list, bool p_isNotify = true)
            where TEntity : Entity
        {
            var ew = new EntityWriter();
            await ew.Delete(p_list);
            return await ew.Commit(p_isNotify);
        }
        #endregion
    }
}
