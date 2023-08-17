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
using System.Text.RegularExpressions;
#endregion

namespace Dt.Mgr
{
    class LobRpcConfig : IRpcConfig
    {
        #region 成员变量
        bool _isSingletonSvc = false;
        Dictionary<string, string> _urlDict;
        #endregion

        public string SvcUrl { get; set; }

        /// <summary>
        /// 获取服务地址，末尾无/，如：https://10.10.1.16/dt-cm
        /// </summary>
        /// <param name="p_svcName">服务名称，如cm</param>
        /// <returns></returns>
        public string GetSvcUrl(string p_svcName)
        {
            // 单体服务(所有微服务合并成一个服务)
            if (_isSingletonSvc || "cm".Equals(p_svcName, StringComparison.OrdinalIgnoreCase))
                return SvcUrl;

            if (_urlDict.TryGetValue(p_svcName, out var url))
                return url;

            throw new Exception($"[{p_svcName}]服务地址不存在！");
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
        /// 初始化所有微服务地址
        /// </summary>
        /// <param name="p_svcUrls"></param>
        internal void InitSvcUrls(Dict p_svcUrls)
        {
            if (p_svcUrls == null)
            {
                // 单体服务
                _isSingletonSvc = true;
            }
            else if (!string.IsNullOrWhiteSpace(SvcUrl))
            {
                var match = Regex.Match(SvcUrl, @"^http[s]?://[^\s/]+");
                string prefix = "";
                if (match.Success)
                    prefix = match.Value;

                _urlDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var item in p_svcUrls)
                {
                    var url = item.Value as string;
                    if (url.StartsWith("*/"))
                        url = prefix + url.Substring(1).TrimEnd('\\');
                    _urlDict[item.Key] = url;
                }
            }
        }
    }
}
