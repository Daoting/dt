#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-10-21 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
#endregion

namespace Dt.UIDemo
{
    class RpcConfig : IRpcConfig
    {
        /// <summary>
        /// 获取服务地址，末尾无/，如：https://10.10.1.16/dt-cm
        /// </summary>
        /// <param name="p_svcName">服务名称，如cm</param>
        /// <returns></returns>
        public string GetSvcUrl(string p_svcName)
        {
            return "https://x13382a571.oicp.vip/sample";
        }

        /// <summary>
        /// 默认服务名，当前进行实体保存使用的服务名
        /// </summary>
        public string SvcName { get; set; } = "lob";

        /// <summary>
        /// 重置默认服务名为最初
        /// </summary>
        public void ResetSvcName()
        {
        }

        /// <summary>
        /// 调用时服务端返回无访问权限，可以提示用户、跳转到登录页面等
        /// </summary>
        /// <param name="p_methodName"></param>
        public void OnRpcUnauthorized(string p_methodName)
        {
            Throw.Msg($"⚡对【{p_methodName}】无访问权限！");
        }

        /// <summary>
        /// 获取可选的服务地址列表，用于切换服务时选择，切换服务也支持手动填写
        /// </summary>
        /// <returns></returns>
        public List<string> GetSvcUrlOptions()
        {
            return null;
        }
    }
}
