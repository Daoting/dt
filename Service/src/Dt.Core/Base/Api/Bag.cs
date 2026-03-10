#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-01-12 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Rpc;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 整个http请求期间有效的数据包
    /// </summary>
    class Bag
    {
        ApiInvoker _invoker;

        public Bag(ApiInvoker p_invoker)
        {
            _invoker = p_invoker;
            CreateDataAccess(p_invoker.SvcName);

            // RabbitMQ或本地调用时，无法通过HttpContext获取Bag
            if (p_invoker is HttpApiInvoker ha)
            {
                // 在服务中通过静态Current取出
                ha.Context.Items[Kit.ContextItemName] = this;
            }
        }
        
        /// <summary>
        /// 获取当前数据访问对象
        /// </summary>
        public IDataAccess DataAccess { get; private set; }

        /// <summary>
        /// 获取当前用户标识，UI客户端rpc为实际登录用户ID
        /// <para>特殊标识：110为admin页面，111为RabbitMQ rpc，112为本地调用</para>
        /// </summary>
        public long UserID => _invoker.UserID;

        /// <summary>
        /// 日志对象，日志中比静态Log类多出Api名称和当前UserID
        /// </summary>
        public ILogger Log => _invoker.Log;

        void CreateDataAccess(string p_svcName)
        {
            int index;
            if (string.IsNullOrEmpty(p_svcName))
            {
                DataAccess = Kit.DefaultDbInfo.GetDa();
            }
            else if ((index = p_svcName.IndexOf('+')) > 0
                && index < p_svcName.Length - 1)
            {
                // 服务名+数据源键名，如：do+pgdt ，以键名为准，实体系统用到
                DataAccess = Kit.AllDbInfo[p_svcName.Substring(index + 1)].GetDa();
            }
            else if (Kit.Svcs.TryGetValue(p_svcName, out var svc))
            {
                DataAccess = svc.DbInfo.GetDa();
            }
            else
            {
                throw new Exception($"服务 {p_svcName} 不存在！");
            }
            DataAccess.AutoClose = false;
        }
    }
}
