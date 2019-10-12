#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-07-25 创建
******************************************************************************/
#endregion

#region 引用命名
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 所有服务内部使用的工具Api
    /// </summary>
    [Api(GroupName = "系统工具", AgentMode = AgentMode.Generic)]
    public class SysTools : BaseApi
    {
        /// <summary>
        /// 重新加载Cache.db中的sql语句
        /// </summary>
        public void 刷新sql缓存()
        {
            Silo.LoadCacheSql();
        }

    }
}
