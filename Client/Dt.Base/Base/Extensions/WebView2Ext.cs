﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2015-05-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml.Controls;
using System.Text.Json;
#endregion

namespace Dt.Base
{
    public static class WebView2Ext
    {
        /// <summary>
        /// 升级到WebView2的扩展方法
        /// </summary>
        /// <param name="webView2"></param>
        /// <param name="p_functionName"></param>
        /// <param name="p_parameters"></param>
        /// <returns></returns>
        public static async Task<string> InvokeScriptAsync(this WebView2 webView2, string p_functionName, params object[] p_parameters)
        {
            string script = p_functionName + "(";
            if (p_parameters != null && p_parameters.Length > 0)
            {
                for (int i = 0; i < p_parameters.Length; i++)
                {
                    script += JsonSerializer.Serialize(p_parameters[i], JsonOptions.UnsafeSerializer);
                    if (i < p_parameters.Length - 1)
                    {
                        script += ", ";
                    }
                }
            }
            script += ");";

            // 返回json编码的串，undefined时返回"null"
            string str = await webView2.ExecuteScriptAsync(script);
            // json解码
            var val = JsonSerializer.Deserialize<dynamic>(str);
            return val is null ? "" : val.ToString();
        }
    }
}
