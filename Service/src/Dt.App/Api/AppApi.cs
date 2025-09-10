#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-29 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.App
{
    /// <summary>
    /// App服务Api
    /// </summary>
    [Api]
    public class AppApi : DomainSvc
    {
        /// <summary>
        /// 获取windows应用的版本信息
        /// </summary>
        /// <returns></returns>
        public Dict GetWinAppVer()
        {
            return null; // MsixCfg.WinAppVer;
        }
    }
}
