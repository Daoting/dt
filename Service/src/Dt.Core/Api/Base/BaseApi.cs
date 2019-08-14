#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-26 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Rpc;
using Microsoft.AspNetCore.Http;
using Serilog;
using System.Web;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// Api抽象基类
    /// </summary>
    public abstract class BaseApi
    {
        /// <summary>
        /// 业务线上下文
        /// </summary>
        protected LobContext _c = LobContext.Current;
    }
}
