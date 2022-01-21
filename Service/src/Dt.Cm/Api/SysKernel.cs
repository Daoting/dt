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
    /// 系统内核Api
    /// </summary>
    [Api]
    public class SysKernel : BaseApi
    {
        static readonly SqliteModelHandler _modelHandler = Kit.GetObj<SqliteModelHandler>();
        static Dict _svcUrls;

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
                if (_svcUrls == null)
                    LoadSvcUrls();
                ls.Add(_svcUrls);
            }
            ls.Add(_modelHandler.Version);
            return ls;
        }

        /// <summary>
        /// 更新模型库文件
        /// </summary>
        /// <returns></returns>
        public bool UpdateModelDbFile()
        {
            return _modelHandler.Refresh();
        }

        static void LoadSvcUrls()
        {
            // service.json 调整后支持实时更新服务地址
            Kit.ConfigChanged -= LoadSvcUrls;
            Kit.ConfigChanged += LoadSvcUrls;

            Dict dt = new Dict();
            foreach (var item in Kit.Config.GetSection("SvcUrls").GetChildren())
            {
                dt[item.Key] = item.Value;
            }
            _svcUrls = dt;
        }
    }
}
