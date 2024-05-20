#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-10-19 创建
**************************************************************************/
#endregion

#region 命名空间
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
#endregion

namespace Dt.Mgr
{
    /// <summary>
    /// 报表预览视图，可由菜单命令启动或自定义启动
    /// </summary>
    [View(LobViews.报表)]
    public class ReportView : IView
    {
        /// <summary>
        /// 视图启动入口
        /// </summary>
        /// <param name="p_title">标题</param>
        /// <param name="p_icon">图标</param>
        /// <param name="p_params">启动参数</param>
        public async void Run(string p_title, Icons p_icon, object p_params)
        {
            if (p_params == null || string.IsNullOrEmpty(p_params.ToString()))
                return;

            string param = p_params.ToString();
            Icons icon = p_icon == Icons.None ? Icons.折线图 : p_icon;

            JsonObject obj = null;
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(param)))
            {
                try
                {
                    obj = await JsonSerializer.DeserializeAsync<JsonObject>(ms, JsonOptions.UnsafeSerializer);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "报表参数json格式错误！");
                }
            }
            
            if (obj.Count == 1)
            {
                var info = await CreateInfo(obj.First());
                Rpt.Show(info, false, p_title, icon);
            }
            else
            {
                List<RptInfo> infos = new List<RptInfo>();
                foreach (var kv in obj)
                {
                    var info = await CreateInfo(kv);
                    infos.Add(info);
                }
                Rpt.Show(infos, false, p_title, icon);
            }
        }

        /*
         视图参数格式：
        
         {
             "报表模板Uri1":
             {
                 "参数名1": 值,
                 "参数名2": 值
             },
             "报表模板Uri2":
             {
                 "参数名1": 值,
                 "参数名2": 值
             }
         }
        
         */
        
        async Task<RptInfo> CreateInfo(KeyValuePair<string, JsonNode> p_kv)
        {
            var info = new RptInfo { Uri = p_kv.Key };
            if (p_kv.Value is JsonObject child && child.Count > 0)
            {
                await info.Init();
                if (info.Params != null)
                {
                    foreach (var kv in child)
                    {
                        // 初始参数值
                        if (info.Params.ContainsKey(kv.Key))
                            info.Params[kv.Key] = kv.Value.ToString();
                    }
                }
            }
            return info;
        }
    }
}
