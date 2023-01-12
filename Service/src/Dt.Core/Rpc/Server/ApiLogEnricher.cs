#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-10-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Serilog.Core;
using Serilog.Events;
#endregion

namespace Dt.Core.Rpc
{
    /// <summary>
    /// Api调用过程日志的附加属性
    /// </summary>
    class ApiLogEnricher : ILogEventEnricher
    {
        string _apiName;
        long _userID;

        public ApiLogEnricher(string p_apiName, long p_userID)
        {
            _apiName = p_apiName;
            _userID = p_userID;
        }

        public void Enrich(LogEvent p_logEvent, ILogEventPropertyFactory p_propertyFactory)
        {
            if (p_logEvent == null || p_propertyFactory == null)
                return;

            LogEventProperty property;
            if (!string.IsNullOrEmpty(_apiName))
            {
                property = p_propertyFactory.CreateProperty("Api", _apiName);
                p_logEvent.AddPropertyIfAbsent(property);
            }
            if (_userID != -1)
            {
                property = p_propertyFactory.CreateProperty("User", _userID);
                p_logEvent.AddPropertyIfAbsent(property);
            }
        }
    }
}