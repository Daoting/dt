#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-06-30 创建
******************************************************************************/
#endregion

#region 引用命名
using Serilog;
using System;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 功能测试Api
    /// </summary>
    [Api(GroupName = "功能测试", AgentMode = AgentMode.Generic)]
    public class TestLog : BaseApi
    {
        /// <summary>
        /// 记录普通日志
        /// </summary>
        /// <param name="p_msg"></param>
        public void LogInfo(string p_msg)
        {
            _log.Information(p_msg);
        }

        /// <summary>
        /// 记录警告信息
        /// </summary>
        public void LogWarning()
        {
            try
            {
                throw new Exception("异常对象的警告信息");
            }
            catch (Exception ex)
            {
                _log.Warning(ex, "测试警告日志");
            }
        }

        /// <summary>
        /// 记录出错信息
        /// </summary>
        public void LogError()
        {
            throw new Exception("异常对象的错误信息");
        }
    }
}
