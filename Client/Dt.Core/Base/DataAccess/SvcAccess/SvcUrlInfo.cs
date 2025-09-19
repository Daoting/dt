#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-10-21 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Text.RegularExpressions;
#endregion

namespace Dt.Core
{
    class SvcUrlInfo
    {
        #region 成员变量
        // cm服务地址
        string _urlCm;
        // do服务地址
        string _urlDefSvc;
        
        bool _isSingletonSvc = false;
        Dictionary<string, string> _urlDict;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_url">cm服务地址</param>
        public SvcUrlInfo(string p_url)
        {
            _urlCm = p_url;
        }

        /// <summary>
        /// 获取服务地址，末尾无/，如：https://10.10.1.16/dt-cm
        /// </summary>
        /// <param name="p_svcName">服务名称，如cm</param>
        /// <returns></returns>
        public string GetSvcUrl(string p_svcName)
        {
            // 单体服务(所有微服务合并成一个服务)
            if (_isSingletonSvc || "cm".Equals(p_svcName, StringComparison.OrdinalIgnoreCase))
                return _urlCm;

            // da da+dbkey
            if (p_svcName.StartsWith(At.OriginSvc, StringComparison.OrdinalIgnoreCase))
                return _urlDefSvc;
            
            if (_urlDict.TryGetValue(p_svcName, out var url))
                return url;

            throw new Exception($"[{p_svcName}]服务地址不存在！");
        }

        /// <summary>
        /// 初始化所有微服务地址
        /// </summary>
        /// <param name="p_svcUrls"></param>
        public void InitSvcUrls(Dict p_svcUrls)
        {
            if (p_svcUrls == null)
            {
                // 单体服务
                _isSingletonSvc = true;
            }
            else if (!string.IsNullOrWhiteSpace(_urlCm))
            {
                var match = Regex.Match(_urlCm, @"^http[s]?://[^\s:/]+");
                string prefix = "";
                if (match.Success)
                    prefix = match.Value;

                _urlDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var item in p_svcUrls)
                {
                    var url = item.Value as string;
                    if (url.StartsWith("*"))
                        url = prefix + url.Substring(1).TrimEnd('\\');
                    _urlDict[item.Key] = url;
                }
                
                if (!_urlDict.TryGetValue(At.OriginSvc, out _urlDefSvc))
                    throw new Exception($"[{At.OriginSvc}] 服务地址不存在！");
            }
        }
    }
}
