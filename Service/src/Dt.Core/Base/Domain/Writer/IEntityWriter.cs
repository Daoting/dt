#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-28 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 实体写入器，对所有实体的的持久化做统一管理，解决领域模型存储和变更工作
    /// <para>写入器在一个事务内批量处理所有待保存、待删除的实体数据，失败时回滚</para>
    /// <para>无论提交成功失败都清空状态，准备下次提交！</para>
    /// </summary>
    public interface IEntityWriter
    {
        #region 保存
        /// <summary>
        /// 添加待保存的实体，最后由Commit统一提交
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_entity">待保存的实体</param>
        /// <returns></returns>
        Task Save<TEntity>(TEntity p_entity)
            where TEntity : Entity;

        /// <summary>
        /// 添加Table中新增、修改、删除的待保存实体，最后由Commit统一提交
        /// <para>删除行通过Table的ExistDeleted DeletedRows判断获取</para>
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_tbl">实体表</param>
        /// <returns></returns>
        Task Save<TEntity>(Table<TEntity> p_tbl)
            where TEntity : Entity;

        /// <summary>
        /// 批量添加一对多的父实体和所有子实体中新增、修改、删除的待保存实体，最后由Commit统一提交
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="p_entity"></param>
        /// <returns></returns>
        Task SaveWithChild<TEntity>(TEntity p_entity)
            where TEntity : Entity;

        /// <summary>
        /// 添加列表中新增、修改的待保存实体，最后由Commit统一提交
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_list">实体列表</param>
        /// <returns></returns>
        Task Save<TEntity>(IEnumerable<TEntity> p_list)
            where TEntity : Entity;
        #endregion

        #region 删除
        /// <summary>
        /// 添加待删除的实体，最后由Commit统一提交
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_entity">待删除的实体</param>
        /// <returns></returns>
        Task Delete<TEntity>(TEntity p_entity)
            where TEntity : Entity;

        /// <summary>
        /// 批量添加待删除的实体，最后由Commit统一提交
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_list">待删除实体列表</param>
        /// <returns></returns>
        Task Delete<TEntity>(IList<TEntity> p_list)
            where TEntity : Entity;

        /// <summary>
        /// 先根据主键获取该实体，然后添加到待删除，最后由Commit统一提交
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_id">主键</param>
        /// <returns></returns>
        Task DelByID<TEntity>(object p_id)
            where TEntity : Entity;

        /// <summary>
        /// 先根据主键批量获取该实体，然后添加到待删除，最后由Commit统一提交
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_ids">主键列表</param>
        /// <returns></returns>
        Task DelByIDs<TEntity>(IList p_ids)
            where TEntity : Entity;
        #endregion

        #region 提交
        /// <summary>
        /// 是否有数据需要提交
        /// </summary>
        bool NeedCommit { get; }

        /// <summary>
        /// 一个事务内批量处理所有待保存、待删除的实体数据，失败时回滚
        /// <para>无论提交成功失败都清空状态，准备下次提交！</para>
        /// <para>处理成功后，对于每个实体：</para>
        /// <para>1. 调用保存后或删除后回调</para>
        /// <para>2. 对于新增或修改的实体进行状态复位</para>
        /// <para>3. 若存在领域事件，则发布事件</para>
        /// </summary>
        /// <param name="p_isNotify">是否提示保存结果，客户端有效</param>
        /// <param name="p_notifyIfNotNeed">没有需要保存的数据时是否提示，null时由p_isNotify控制，客户端有效</param>
        /// <returns>是否成功</returns>
        Task<bool> Commit(bool p_isNotify = true, bool? p_notifyIfNotNeed = null);
        #endregion
    }
}
