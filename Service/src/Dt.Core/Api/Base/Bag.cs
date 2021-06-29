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
using System.Collections.Concurrent;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 整个http请求期间有效的数据包，为方便使用，所有属性和方法都为静态
    /// </summary>
    public class Bag
    {
        #region 成员变量
        const string _lcName = "ApiContext";
        readonly ApiInvoker _invoker;
        DataProvider _dp;
        ConcurrentDictionary<string, object> _data;
        #endregion

        #region 构造方法
        internal Bag(ApiInvoker p_invoker)
        {
            _invoker = p_invoker;
            // 在服务中通过静态Current取出
            _invoker.Context.Items[_lcName] = this;
        }
        #endregion

        /// <summary>
        /// 获取当前数据提供者
        /// </summary>
        public static DataProvider Dp => ((Bag)Kit.HttpContext.Items[_lcName]).GetDataProvider();

        /// <summary>
        /// 获取当前用户标识
        /// </summary>
        public static long UserID => ((Bag)Kit.HttpContext.Items[_lcName])._invoker.UserID;

        /// <summary>
        /// 日志对象，日志中比静态Log类多出Api名称和当前UserID
        /// </summary>
        public static ILogger Log => ((Bag)Kit.HttpContext.Items[_lcName])._invoker.Log;

        /// <summary>
        /// 本地事件总线
        /// </summary>
        public static LocalEventBus LocalEB => Kit.GetSvc<LocalEventBus>();

        /// <summary>
        /// 远程事件总线
        /// </summary>
        public static RemoteEventBus RemoteEB => Kit.GetSvc<RemoteEventBus>();

        /// <summary>
        /// 当前为匿名用户
        /// </summary>
        public static bool IsAnonymous => ((Bag)Kit.HttpContext.Items[_lcName])._invoker.UserID == -1;

        /// <summary>
        /// 获取当前http请求上下文
        /// </summary>
        public static HttpContext Context => Kit.HttpContext;

        /// <summary>
        /// 添加共享数据项，在整个请求期间有效
        /// </summary>
        /// <param name="p_name">共享项名称</param>
        /// <param name="p_value">共享项</param>
        public static void AddItem(string p_name, object p_value)
        {
            Throw.IfNullOrEmpty(p_name);
            Bag bag = (Bag)Kit.HttpContext.Items[_lcName];
            if (bag._data == null)
                bag._data = new ConcurrentDictionary<string, object>();
            bag._data[p_name] = p_value;
        }

        /// <summary>
        /// 获取共享数据项，在整个请求期间有效
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_name">共享项名称</param>
        /// <returns></returns>
        public static T GetItem<T>(string p_name)
        {
            Throw.IfNullOrEmpty(p_name);
            Bag bag = (Bag)Kit.HttpContext.Items[_lcName];
            if (bag._data != null && bag._data.TryGetValue(p_name, out var val))
                return (T)val;
            return default(T);
        }

        #region 内部方法
        /// <summary>
        /// Api调用结束后释放资源，提交或回滚事务、关闭数据库连接、发布领域事件
        /// </summary>
        /// <param name="p_suc"></param>
        /// <returns></returns>
        internal Task Close(bool p_suc)
        {
            if (_dp != null)
                return _dp.Close(p_suc);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 获取当前数据仓库
        /// </summary>
        DataProvider GetDataProvider()
        {
            if (_dp == null)
                _dp = new DataProvider(_invoker.Api.IsTransactional);
            return _dp;
        }
        #endregion
    }
}
