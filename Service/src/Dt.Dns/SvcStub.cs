#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Sqlite;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Reflection;
#endregion

namespace Dt.Dns
{
    /// <summary>
    /// 服务存根
    /// </summary>
    public class SvcStub : Stub
    {
        /// <summary>
        /// 获取服务名称，小写
        /// </summary>
        public override string SvcName => "dns";

    }
}
