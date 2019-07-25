#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-25 创建
******************************************************************************/
#endregion

#region 引用命名
using Serilog.Core;
using Serilog.Events;
#endregion

namespace Dts.Core.Rpc
{
    /// <summary>
    /// 业务线处理日志的附加属性
    /// </summary>
    class LobLogEnricher : ILogEventEnricher
    {
        LobContext _lc;
        string _userId;

        public LobLogEnricher(LobContext p_lc, string p_userId)
        {
            _lc = p_lc;
            _userId = p_userId;
        }

        public void Enrich(LogEvent p_logEvent, ILogEventPropertyFactory p_propertyFactory)
        {
            if (p_logEvent == null || p_propertyFactory == null)
                return;

            LogEventProperty property;
            if (!string.IsNullOrEmpty(_lc.ApiName))
            {
                property = p_propertyFactory.CreateProperty("Api", _lc.ApiName);
                p_logEvent.AddPropertyIfAbsent(property);
            }
            if (!string.IsNullOrEmpty(_userId))
            {
                property = p_propertyFactory.CreateProperty("User", _userId);
                p_logEvent.AddPropertyIfAbsent(property);
            }
        }
    }
}