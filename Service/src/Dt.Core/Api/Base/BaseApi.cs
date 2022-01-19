#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-26 创建
******************************************************************************/
#endregion

#region 引用命名
using Serilog;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// Api抽象基类
    /// </summary>
    public abstract class BaseApi
    {
        DataProvider _dp;

        /// <summary>
        /// 获取当前数据提供者
        /// </summary>
        public DataProvider Dp
        {
            get
            {
                if (_dp == null)
                    _dp = new DataProvider(IsTransactional);
                return _dp;
            }
        }

        /// <summary>
        /// 获取当前用户标识
        /// </summary>
        public long UserID { get; internal set; }

        /// <summary>
        /// 当前http请求是否为匿名用户
        /// </summary>
        public bool IsAnonymous => UserID == -1;

        /// <summary>
        /// 是否自动为方法启用事务
        /// </summary>
        internal bool IsTransactional { get; set; }

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
    }
}
