#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-26 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 领域服务的抽象基类
    /// </summary>
    /// <typeparam name="TDomainSvc">当前领域服务的类型，保证静态变量属于各自的领域服务类型</typeparam>
    /// <typeparam name="TAccessInfo">用于获取IDataAccess对象</typeparam>
    public abstract class DomainSvc<TDomainSvc, TAccessInfo>
        where TDomainSvc : class
        where TAccessInfo : AccessInfo, new()
    {
        /**********************************************************************************************************************************************************/
        // 泛型类型：
        // 对象是类的实例，提供具体类型参数的泛型类是泛型类型的实例
        // 若将当前类型作为泛型类的类型参数，如 AbcDs : DomainSvc<AbcDs, Info>
        // 则AbcDs是该泛型基类的实例类，泛型基类中保证有一套只属于AbcDs类的静态变量实例！
        // 因此类型参数相同的泛型类的静态成员相同
        /***********************************************************************************************************************************************************/

        #region 成员变量
        /// <summary>
        /// 日志对象，日志属性中包含来源
        /// </summary>
        protected static readonly ILogger _log = Log.ForContext<TDomainSvc>();

        /// <summary>
        /// 获取领域层数据访问对象
        /// </summary>
        protected static readonly IDataAccess _da = new TAccessInfo().GetDataAccess();

        static readonly EntityWriter _writer = new EntityWriter();
        #endregion

        #region 保存
        /// <summary>
        /// 添加待保存的实体，最后由Commit统一提交
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_entity">待保存的实体</param>
        /// <returns></returns>
        protected static Task Save<TEntity>(TEntity p_entity)
            where TEntity : Entity
        {
            return _writer.Save(p_entity);
        }

        /// <summary>
        /// 添加Table中新增、修改、删除的待保存实体，最后由Commit统一提交
        /// <para>删除行通过Table的ExistDeleted DeletedRows判断获取</para>
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_tbl">实体表</param>
        /// <returns></returns>
        protected static Task Save<TEntity>(Table<TEntity> p_tbl)
            where TEntity : Entity
        {
            return _writer.Save<TEntity>(p_tbl);
        }

        /// <summary>
        /// 批量添加一对多的父实体和所有子实体中新增、修改、删除的待保存实体，最后由Commit统一提交
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="p_entity"></param>
        /// <returns></returns>
        protected static Task SaveWithChild<TEntity>(TEntity p_entity)
            where TEntity : Entity
        {
            return _writer.SaveWithChild(p_entity);
        }

        /// <summary>
        /// 添加列表中新增、修改的待保存实体，最后由Commit统一提交
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_list">实体列表</param>
        /// <returns></returns>
        protected static Task Save<TEntity>(IEnumerable<TEntity> p_list)
            where TEntity : Entity
        {
            return _writer.Save(p_list);
        }
        #endregion

        #region 删除
        /// <summary>
        /// 添加待删除的实体，最后由Commit统一提交
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_entity">待删除的实体</param>
        /// <returns></returns>
        protected static Task Delete<TEntity>(TEntity p_entity)
            where TEntity : Entity
        {
            return _writer.Delete(p_entity);
        }

        /// <summary>
        /// 批量添加待删除的实体，最后由Commit统一提交
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_list">待删除实体列表</param>
        /// <returns></returns>
        protected static Task Delete<TEntity>(IList<TEntity> p_list)
            where TEntity : Entity
        {
            return _writer.Delete(p_list);
        }

        /// <summary>
        /// 先根据主键获取该实体，然后添加到待删除，最后由Commit统一提交
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_id">主键</param>
        /// <returns></returns>
        protected static Task DelByID<TEntity>(object p_id)
            where TEntity : Entity
        {
            return _writer.DelByID<TEntity>(p_id);
        }

        /// <summary>
        /// 先根据主键批量获取该实体，然后添加到待删除，最后由Commit统一提交
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_ids">主键列表</param>
        /// <returns></returns>
        protected static Task DelByIDs<TEntity>(IList p_ids)
            where TEntity : Entity
        {
            return _writer.DelByID<TEntity>(p_ids);
        }
        #endregion

        #region 提交
        /// <summary>
        /// 一个事务内批量处理所有待保存、待删除的实体数据，失败时回滚
        /// <para>无论提交成功失败都清空状态，准备下次提交！</para>
        /// <para>处理成功后，对于每个实体：</para>
        /// <para>1. 对于新增、修改的实体进行状态复位</para>
        /// <para>2. 若存在领域事件，则发布事件</para>
        /// <para>3. 若已设置服务端缓存，则删除缓存</para>
        /// </summary>
        /// <param name="p_isNotify">是否提示保存结果，客户端有效</param>
        /// <returns>是否成功</returns>
        protected static Task<bool> Commit(bool p_isNotify = true)
        {
            return _writer.Commit(p_isNotify);
        }
        #endregion
    }
}
