#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.Chat;
using Dt.Core;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 系统内置推送处理
    /// </summary>
    [PushApi]
    public class SysPushApi
    {
        /// <summary>
        /// 接收服务器推送的聊天信息
        /// </summary>
        /// <param name="p_letter"></param>
        public void ReceiveLetter(LetterInfo p_letter)
        {
            LetterManager.ReceiveLetter(p_letter);
        }

        /// <summary>
        /// 系统警告提示
        /// </summary>
        /// <param name="p_msg"></param>
        public void ShowSysWarning(string p_msg)
        {
            Kit.Warn($"【系统通知】\r\n{p_msg}");
        }
    }
}

namespace Dt.Base.Chat
{
    /// <summary>
    /// 空，占位用，因WebRtc只支持WebAssembly
    /// </summary>
    [PushApi]
    public partial class WebRtcApi
    { }
}