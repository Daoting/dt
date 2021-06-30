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
        /// <summary>
        /// 数据提供者
        /// </summary>
        protected DataProvider _dp = Kit.ContextDp;

        /// <summary>
        /// 日志对象，日志中比静态Log类多出Api名称和当前UserID
        /// </summary>
        protected ILogger _log => Kit.ContextLog;
    }
}
