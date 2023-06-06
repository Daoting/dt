﻿#region 文件描述
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
        public Bag(ApiInvoker p_invoker)
        {
            UserID = p_invoker.UserID;
            Log = p_invoker.Log;
            CreateDataAccess(p_invoker.SvcName);

            // RabbitMQ或本地调用时，无法通过HttpContext获取Bag
            if (p_invoker is HttpApiInvoker ha)
            {
                // 在服务中通过静态Current取出
                ha.Context.Items[Kit.ContextItemName] = this;
            }
        }

        public Bag(string p_svcName, long p_userID, ILogger p_log)
        {
            UserID = p_userID;
            Log = p_log;
            CreateDataAccess(p_svcName);
        }

        /// <summary>
        /// 获取当前数据访问对象
        /// </summary>
        public IDataAccess DataAccess { get; private set; }

        /// <summary>
        /// 获取当前用户标识，UI客户端rpc为实际登录用户ID
        /// <para>特殊标识：110为admin页面，111为RabbitMQ rpc，112为本地调用</para>
        /// </summary>
        public long UserID { get; }

        /// <summary>
        /// 日志对象，日志中比静态Log类多出Api名称和当前UserID
        /// </summary>
        public ILogger Log { get; }

        void CreateDataAccess(string p_svcName)
        {
            if (string.IsNullOrEmpty(p_svcName) || Kit.SingletonSvcDbs == null)
            {
                DataAccess = Kit.DefaultDbInfo.NewDataAccess();
            }
            else if (Kit.SingletonSvcDbs.TryGetValue(p_svcName, out var di))
            {
                DataAccess = di.NewDataAccess();
            }
            else
            {
                throw new Exception($"服务[{p_svcName}]未设置默认数据源键名！");
            }
            DataAccess.AutoClose = false;
        }
    }
}
