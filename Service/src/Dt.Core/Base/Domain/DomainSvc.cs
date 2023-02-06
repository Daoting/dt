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
    /// 领域服务的抽象基类，也是Rpc Api入口
    /// </summary>
    public abstract class DomainSvc
    {
        #region 成员变量
        Bag _bag;
        readonly EntityWriter _writer = new EntityWriter();
        #endregion

        #region 属性
        /// <summary>
        /// 获取领域层数据访问对象
        /// </summary>
        protected IDataAccess _da => _bag.DataAccess;

        /// <summary>
        /// 获取当前用户标识，UI客户端rpc为实际登录用户ID
        /// <para>特殊标识：110为admin页面，111为RabbitMQ rpc，112为本地调用</para>
        /// </summary>
        protected long _userID => _bag.UserID;

        /// <summary>
        /// 日志对象，日志中比静态Log类多出Api名称和当前UserID
        /// </summary>
        protected ILogger _log => _bag.Log;

        /// <summary>
        /// 当前http请求是否为匿名用户
        /// </summary>
        protected bool _isAnonymous => _bag.UserID == -1;
        #endregion

        #region 保存
        /// <summary>
        /// 添加待保存的实体，最后由Commit统一提交
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="p_entity">待保存的实体</param>
        /// <returns></returns>
        protected Task Save<TEntity>(TEntity p_entity)
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
        protected Task Save<TEntity>(Table<TEntity> p_tbl)
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
        protected Task SaveWithChild<TEntity>(TEntity p_entity)
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
        protected Task Save<TEntity>(IEnumerable<TEntity> p_list)
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
        protected Task Delete<TEntity>(TEntity p_entity)
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
        protected Task Delete<TEntity>(IList<TEntity> p_list)
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
        protected Task DelByID<TEntity>(object p_id)
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
        protected Task DelByIDs<TEntity>(IList p_ids)
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
        protected Task<bool> Commit(bool p_isNotify = true)
        {
            return _writer.Commit(p_isNotify);
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 初始化整个调用期间有效的数据包
        /// </summary>
        /// <param name="p_bag"></param>
        internal void Init(Bag p_bag)
        {
            _bag= p_bag;
        }

        /// <summary>
        /// Api调用结束后释放资源，提交或回滚事务、关闭数据库连接
        /// </summary>
        /// <param name="p_suc"></param>
        /// <returns></returns>
        internal Task Close(bool p_suc)
        {
            return _bag.DataAccess.Close(p_suc);
        }
        #endregion
    }
}
