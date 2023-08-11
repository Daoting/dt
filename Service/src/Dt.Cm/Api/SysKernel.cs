#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-27 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Microsoft.Extensions.Configuration;
#endregion

namespace Dt.Cm
{
    /// <summary>
    /// 系统内核Api
    /// </summary>
    [Api]
    public class SysKernel : DomainSvc
    {
        static readonly SqliteModelHandler _modelHandler = Kit.GetService<SqliteModelHandler>();

        /// <summary>
        /// 获取参数配置，包括服务器时间、所有服务地址、模型文件版本号
        /// </summary>
        /// <returns></returns>
        public List<object> GetConfig()
        {
            var ls = new List<object> { Kit.Now };
            if (Kit.IsSingletonSvc)
            {
                // 单体服务只传标志
                ls.Add(true);
            }
            else
            {
                if (SvcUrls == null)
                    Throw.Msg("缺少url.json文件，无法获取所有服务的地址信息");
                ls.Add(SvcUrls);
            }
            ls.Add(_modelHandler.Version);
            return ls;
        }

        /// <summary>
        /// 更新服务端表结构缓存和sqlite模型库文件
        /// </summary>
        /// <returns></returns>
        public bool UpdateModel()
        {
            return _modelHandler.Refresh();
        }

        internal static Dict SvcUrls { get; set; }
    }
}
