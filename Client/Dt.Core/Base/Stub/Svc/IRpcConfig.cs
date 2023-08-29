#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-10-21 创建
******************************************************************************/
#endregion

#region 引用命名

#endregion

namespace Dt.Core
{
    /// <summary>
    /// Rpc配置接口
    /// </summary>
    public interface IRpcConfig
    {
        /// <summary>
        /// 获取服务地址，末尾无/，如：https://10.10.1.16/dt-cm
        /// </summary>
        /// <param name="p_svcName">服务名称，如cm</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        string GetSvcUrl(string p_svcName);

        /// <summary>
        /// 调用时服务端返回无访问权限，可以提示用户、跳转到登录页面等
        /// </summary>
        /// <param name="p_methodName"></param>
        void OnRpcUnauthorized(string p_methodName);

        /// <summary>
        /// 获取可选的服务地址列表，用于切换服务时选择，切换服务也支持手动填写
        /// </summary>
        /// <returns></returns>
        List<string> GetSvcUrlOptions();
    }
}