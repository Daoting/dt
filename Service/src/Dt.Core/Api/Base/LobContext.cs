#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Domain;
using Dt.Core.EventBus;
using Dt.Core.Rpc;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 业务线处理上下文
    /// </summary>
    public class LobContext
    {
        #region 成员变量
        const string _lcName = "LobContext";
        readonly ApiInvoker _invoker;
        Db _db;
        List<DomainEvent> _domainEvents;
        #endregion

        #region 构造方法
        internal LobContext(ApiInvoker p_invoker)
        {
            _invoker = p_invoker;
            // 在服务中通过静态Current取出
            _invoker.Context.Items[_lcName] = this;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取当前业务线上下文
        /// </summary>
        public static LobContext Current => (LobContext)Glb.HttpContext.Items[_lcName];

        /// <summary>
        /// http请求上下文
        /// </summary>
        public HttpContext Context
        {
            get { return _invoker.Context; }
        }

        /// <summary>
        /// 获取当前用户标识
        /// </summary>
        public long UserID
        {
            get { return _invoker.UserID; }
        }

        /// <summary>
        /// 日志对象
        /// </summary>
        public ILogger Log
        {
            get { return _invoker.Log; }
        }

        /// <summary>
        /// 获取当前mysql库
        /// <para>根据方法的 Transaction 标签确定是否自动启动事务</para>
        /// <para>整个Api调用结束后自动提交或回滚事务、关闭连接</para>
        /// <para>可通过SetCurrentDb设置其它非默认库</para>
        /// </summary>
        public Db Db
        {
            get
            {
                if (_db == null)
                {
                    _db = new Db(false);
                    if (_invoker.Api.IsTransactional)
                        _db.BeginTrans().Wait();
                }
                return _db;
            }
        }

        /// <summary>
        /// 本地事件总线
        /// </summary>
        public LocalEventBus LocalEB
        {
            get { return Glb.GetSvc<LocalEventBus>(); }
        }

        /// <summary>
        /// 远程事件总线
        /// </summary>
        public RemoteEventBus RemoteEB
        {
            get { return Glb.GetSvc<RemoteEventBus>(); }
        }

        /// <summary>
        /// 当前为匿名用户
        /// </summary>
        public bool IsAnonymous => UserID == -1;
        #endregion

        #region 外部方法
        /// <summary>
        /// 设置当前mysql库，根据键名获取连接串
        /// </summary>
        /// <param name="p_dbKey">数据源键名，null时使用默认连接串</param>
        /// <returns></returns>
        public async Task SetCurrentDb(string p_dbKey)
        {
            if (_db != null)
                await _db.Close(true);

            _db = new Db(p_dbKey, false);
            if (_invoker.Api.IsTransactional)
                _db.BeginTrans().Wait();
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 收集待发布的领域事件
        /// </summary>
        /// <param name="p_events"></param>
        internal void AddDomainEvents(IEnumerable<DomainEvent> p_events)
        {
            if (_domainEvents == null)
                _domainEvents = new List<DomainEvent>();
            _domainEvents.AddRange(p_events);
        }

        internal void AddDomainEvent(DomainEvent p_event)
        {
            if (_domainEvents == null)
                _domainEvents = new List<DomainEvent>();
            _domainEvents.Add(p_event);
        }

        /// <summary>
        /// Api调用结束后释放资源，提交或回滚事务、关闭数据库连接、发布领域事件
        /// </summary>
        /// <param name="p_suc"></param>
        /// <returns></returns>
        internal async Task Close(bool p_suc)
        {
            if (_db != null)
            {
                await _db.Close(p_suc);
                _db = null;
            }

            // 发布领域事件
            if (p_suc && _domainEvents != null)
            {
                var localEB = LocalEB;
                var remoteEB = RemoteEB;
                foreach (var de in _domainEvents)
                {
                    if (de.IsRemoteEvent)
                    {
                        //remoteEB.
                    }
                    else
                    {
                        localEB.Publish(de.Event);
                    }
                }
            }
            _domainEvents?.Clear();
        }
        #endregion
    }
}
