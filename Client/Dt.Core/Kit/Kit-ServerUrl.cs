#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-12-31 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 服务地址
    /// </summary>
    public partial class Kit
    {
        #region 成员变量
        static bool _isSingletonSvc;
        static string _cmSvcUrl;
        static Dictionary<string, string> _urlDict;
        #endregion

        /// <summary>
        /// 获取服务地址
        /// </summary>
        /// <param name="p_svcName">服务名称，如cm</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string GetSvcUrl(string p_svcName)
        {
            // 单体服务(所有微服务合并成一个服务)
            if (_isSingletonSvc || "cm".Equals(p_svcName, StringComparison.OrdinalIgnoreCase))
                return _cmSvcUrl;

            if (_urlDict.TryGetValue(p_svcName, out var url))
                return url;

            throw new Exception($"[{p_svcName}]服务地址不存在！");
        }

        /// <summary>
        /// 设置cm服务地址，如：https://10.10.1.16/fz-cm
        /// </summary>
        /// <param name="p_url"></param>
        /// <exception cref="Exception"></exception>
        internal static void InitCmSvcUrl(string p_url)
        {
            if (string.IsNullOrEmpty(p_url))
                throw new Exception("cm服务地址不可为空！");
            _cmSvcUrl = p_url.TrimEnd('\\');
        }

        /// <summary>
        /// 初始化所有微服务地址
        /// </summary>
        /// <param name="p_cfg"></param>
        internal static void InitSvcUrls(object p_cfg)
        {
            if (p_cfg is bool)
            {
                // 单体服务
                _isSingletonSvc = true;
            }
            else if (p_cfg is Dict dt)
            {
                var match = Regex.Match(_cmSvcUrl, @"^http[s]?://[^\s/]+");
                string prefix = "";
                if (match.Success)
                    prefix = match.Value;

                _urlDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var item in dt)
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