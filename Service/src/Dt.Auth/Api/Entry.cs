#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-05-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Model;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
#endregion

namespace Dt.Auth
{
    /// <summary>
    /// 管理端入口Api
    /// </summary>
    [Api]
    public class Entry : BaseApi
    {
        /// <summary>
        /// 获取参数配置，包括模型文件版本号、服务器时间
        /// </summary>
        /// <param name="p_prefix">模型文件前缀</param>
        /// <returns></returns>
        public Dict GetConfig(string p_prefix)
        {
            return new Dict { { "ver", SqliteModel.Version }, { "now", Glb.Now } };
        }
    }
}
