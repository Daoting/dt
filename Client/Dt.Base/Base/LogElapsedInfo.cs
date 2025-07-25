#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2025-08-08 创建
******************************************************************************/
#endregion

#region 引用命名
using Serilog.Extensions.ElapsedTime;
#endregion

namespace Dt.Base
{
    class LogElapsedInfo : ILogElapsedInfo
    {
        public int StartTick { get; set; } = Environment.TickCount;
        
        public Type AppType => typeof(AppBase);

        public string LogPath => Kit.DataPath;

        public string Desc => $"AppType：{Kit.AppType}";
    }
}
