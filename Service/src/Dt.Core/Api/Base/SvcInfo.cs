#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-27 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
#endregion

namespace Dt.Cm
{
    /// <summary>
    /// 当前服务信息Api
    /// </summary>
    [Api(AgentMode = AgentMode.Generic)]
    public class SvcInfo : DomainSvc
    {
        /// <summary>
        /// 获取当前服务的默认数据源键名
        /// </summary>
        /// <returns></returns>
        public string GetDefaultDbKey()
        {
            return Kit.DefaultDbInfo.Key;
        }

        /// <summary>
        /// 是否为单体服务(所有微服务合并成一个服务)
        /// </summary>
        /// <returns></returns>
        public bool IsSingletonSvc()
        {
            return Kit.IsSingletonSvc;
        }
    }
}
