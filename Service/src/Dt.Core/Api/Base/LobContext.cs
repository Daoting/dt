#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-06 创建
******************************************************************************/
#endregion

#region 引用命名
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
        Db _defaultDb;
        Dictionary<string, Db> _dbs;
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
        /// 本地事件总线
        /// </summary>
        public LocalEventBus Local
        {
            get { return Glb.GetSvc<LocalEventBus>(); }
        }

        /// <summary>
        /// 远程事件总线
        /// </summary>
        public RemoteEventBus Remote
        {
            get { return Glb.GetSvc<RemoteEventBus>(); }
        }

        /// <summary>
        /// 当前为匿名用户
        /// </summary>
        public bool IsAnonymous => UserID == -1;
        #endregion

        #region Db
        /// <summary>
        /// 获取mysql默认库，根据方法的 Transaction 标签确定是否自动启动事务，整个Api调用结束后提交或回滚事务、关闭数据库连接
        /// </summary>
        public Db Db
        {
            get
            {
                if (_defaultDb == null)
                {
                    _defaultDb = new Db(false);
                    if (_invoker.Api.IsTransactional)
                        _defaultDb.BeginTrans().Wait();
                }
                return _defaultDb;
            }
        }

        /// <summary>
        /// 根据键名获取Db对象
        /// </summary>
        /// <param name="p_dbKey">数据源键名，在json配置DbList节</param>
        /// <returns></returns>
        public Db GetDbByKey(string p_dbKey)
        {
            Check.NotNull(p_dbKey);
            if (_dbs == null)
                _dbs = new Dictionary<string, Db>();

            Db db;
            if (!_dbs.TryGetValue(p_dbKey, out db))
            {
                db = new Db(p_dbKey, false);
                if (_invoker.Api.IsTransactional)
                    db.BeginTrans().Wait();
                _dbs[p_dbKey] = db;
            }
            return db;
        }
        #endregion

        #region Rpc
        /// <summary>
        /// Api调用结束后释放资源，提交或回滚事务、关闭数据库连接
        /// </summary>
        /// <param name="p_suc"></param>
        /// <returns></returns>
        internal async Task Close(bool p_suc)
        {
            if (_defaultDb != null)
            {
                await _defaultDb.Close(p_suc);
                _defaultDb = null;
            }

            if (_dbs != null && _dbs.Count > 0)
            {
                foreach (var db in _dbs.Values)
                {
                    await db.Close(p_suc);
                }
                _dbs.Clear();
            }
        }
        #endregion
    }
}
