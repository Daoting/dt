#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-06-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Rpc;
using System.Collections.Concurrent;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 整个http请求期间有效的数据包
    /// </summary>
    class Bag
    {
        #region 成员变量
        DataProvider _dp;
        ConcurrentDictionary<string, object> _data;
        #endregion

        #region 构造方法
        public Bag(ApiInvoker p_invoker)
        {
            Invoker = p_invoker;
            // 在服务中通过静态Current取出
            Invoker.Context.Items[Kit.ContextItemName] = this;
        }
        #endregion

        /// <summary>
        /// Api调用处理对象
        /// </summary>
        public ApiInvoker Invoker { get; }

        /// <summary>
        /// 获取当前数据仓库
        /// </summary>
        public DataProvider GetDataProvider()
        {
            if (_dp == null)
                _dp = new DataProvider(Invoker.Api.IsTransactional);
            return _dp;
        }

        /// <summary>
        /// 添加共享数据项，在整个请求期间有效
        /// </summary>
        /// <param name="p_name">共享项名称</param>
        /// <param name="p_value">共享项</param>
        public void AddItem(string p_name, object p_value)
        {
            if (_data == null)
                _data = new ConcurrentDictionary<string, object>();
            _data[p_name] = p_value;
        }

        /// <summary>
        /// 获取共享数据项，在整个请求期间有效
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_name">共享项名称</param>
        /// <returns></returns>
        public T GetItem<T>(string p_name)
        {
            if (_data != null && _data.TryGetValue(p_name, out var val))
                return (T)val;
            return default(T);
        }

        /// <summary>
        /// Api调用结束后释放资源，提交或回滚事务、关闭数据库连接、发布领域事件
        /// </summary>
        /// <param name="p_suc"></param>
        /// <returns></returns>
        public Task Close(bool p_suc)
        {
            if (_dp != null)
                return _dp.Close(p_suc);
            return Task.CompletedTask;
        }
    }
}
