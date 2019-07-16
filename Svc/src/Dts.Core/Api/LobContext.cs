#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Dts.Core.EventBus;
using Dts.Core.Rpc;
using Serilog;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dts.Core
{
    /// <summary>
    /// 业务线处理上下文
    /// </summary>
    public class LobContext
    {
        #region 成员变量
        ILogger _logger;
        Db _defaultDb;
        Dictionary<string, Db> _dbs;
        bool _intercepted;
        #endregion

        #region 属性
        /// <summary>
        /// 获取当前业务线上下文
        /// </summary>
        public static LobContext Current => (LobContext)Glb.HttpContext.Items["lc"];

        /// <summary>
        /// 获取mysql默认库
        /// </summary>
        public Db Db
        {
            get
            {
                if (_defaultDb == null)
                {
                    _defaultDb = new Db(false);
                    if (Api.IsTransactional)
                        _defaultDb.BeginTrans().Wait();
                }
                return _defaultDb;
            }
        }

        /// <summary>
        /// 日志对象
        /// </summary>
        public ILogger Log
        {
            get
            {
                if (_logger == null)
                    _logger = Serilog.Log.ForContext(new LobLogEnricher(this, Glb.HttpContext.User.FindFirst("uid")?.Value));
                return _logger;
            }
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
        /// 当前Api方法
        /// </summary>
        public ApiMethod Api { get; internal set; }

        /// <summary>
        /// 调用的Api名称
        /// </summary>
        public string ApiName { get; internal set; }

        /// <summary>
        /// 当前拦截状态
        /// </summary>
        public InterceptStatus Status { get; private set; }
        #endregion

        #region 外部方法
        /// <summary>
        /// 获取当前用户标识
        /// </summary>
        public long GetUserID()
        {
            long id = -1;
            var claim = Glb.HttpContext.User.FindFirst("uid");
            if (claim != null)
                long.TryParse(claim.Value, out id);
            return id;
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
                if (Api.IsTransactional)
                    db.BeginTrans().Wait();
                _dbs[p_dbKey] = db;
            }
            return db;
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 是否已被拦截过，确保只拦截一次
        /// </summary>
        /// <returns></returns>
        internal bool IsIntercepted()
        {
            if (_intercepted)
                return true;

            _intercepted = true;
            return false;
        }

        /// <summary>
        /// 拦截结束，提交或回滚事务、关闭数据库连接
        /// </summary>
        /// <param name="p_suc"></param>
        /// <returns></returns>
        internal async Task Complete(bool p_suc)
        {
            if (Status != InterceptStatus.Intercepting)
                return;

            Status = p_suc ? InterceptStatus.Successful : InterceptStatus.Failed;
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

    /// <summary>
    /// 拦截状态
    /// </summary>
    public enum InterceptStatus
    {
        /// <summary>
        /// 拦截中
        /// </summary>
        Intercepting,

        /// <summary>
        /// 成功
        /// </summary>
        Successful,

        /// <summary>
        /// 过程中异常
        /// </summary>
        Failed
    }
}
