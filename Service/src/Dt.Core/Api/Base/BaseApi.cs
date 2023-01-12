﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-26 创建
******************************************************************************/
#endregion

#region 引用命名

#endregion

namespace Dt.Core
{
    /// <summary>
    /// Api抽象基类
    /// </summary>
    public abstract class BaseApi
    {
        Bag _bag;

        /// <summary>
        /// 获取当前数据提供者
        /// </summary>
        protected IDataProvider _dp => _bag.Dp;

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
            return _bag.Dp.Close(p_suc);
        }
    }
}
