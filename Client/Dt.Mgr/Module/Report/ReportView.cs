#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-10-19 创建
**************************************************************************/
#endregion

#region 命名空间
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
#endregion

namespace Dt.Mgr
{
    /// <summary>
    /// 报表预览视图，可由菜单命令启动或自定义启动
    /// </summary>
    [View(LobViews.通用报表)]
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

            Icons icon = p_icon == Icons.None ? Icons.折线图 : p_icon;
            var ls = GetRptViewParams(p_params.ToString());
            if (ls.Count == 1)
            {
                var info = await CreateInfo(ls[0]);
                Rpt.Show(info, false, p_title, icon);
            }
            else
            {
                List<RptInfo> infos = new List<RptInfo>();
                foreach (var kv in ls)
                {
                    var info = await CreateInfo(kv);
                    infos.Add(info);
                }
                Rpt.Show(infos, false, p_title, icon);
            }
        }
        
        async Task<RptInfo> CreateInfo(RptViewParam p_param)
        {
            var info = new RptInfo { Uri = p_param.Uri };
            if (p_param.Params.Count > 0)
            {
                await info.Init();
                if (info.Params != null)
                {
                    foreach (var kv in p_param.Params)
                    {
                        // 初始参数值
                        if (info.Params.ContainsKey(kv.Key))
                            info.Params[kv.Key] = kv.Value;
                    }
                }
            }
            return info;
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
        internal static List<RptViewParam> GetRptViewParams(string p_params)
        {
            List<RptViewParam> ls = new List<RptViewParam>();

            try
            {
                Utf8JsonReader reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(p_params));
                // {
                reader.Read();

                while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
                {
                    var rv = new RptViewParam();
                    rv.Uri = reader.GetString();

                    // 参数外层 {
                    reader.Read();
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndObject)
                            break;

                        rv.Params[reader.GetString()] = reader.Read() ? reader.GetString() : null;
                    }
                    ls.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "报表参数json格式错误！");
            }
            
            return ls;
        }
    }
    
    class RptViewParam
    {
        public string Uri { get; set; }
        
        public Dictionary<string, string> Params { get; } = new Dictionary<string, string>();

    }
}
