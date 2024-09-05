#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Mgr.Chat;
using Dt.Mgr.Workflow;
#endregion

namespace Dt.Mgr
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
            ChatDs.ReceiveLetter(p_letter);
        }

        /// <summary>
        /// 系统警告提示
        /// </summary>
        /// <param name="p_msg"></param>
        public void ShowSysWarning(string p_msg)
        {
            Kit.Warn($"【系统通知】\r\n{p_msg}");
        }

        /// <summary>
        /// 新任务提醒
        /// </summary>
        /// <param name="p_itemID">工作项id</param>
        /// <param name="p_sender"></param>
        public void WfNotify(long p_itemID, string p_sender)
        {
            WfiDs.ReceiveNewTask(p_itemID, p_sender);
        }
    }
}

namespace Dt.Mgr.Chat
{
    /// <summary>
    /// 空，占位用，因WebRtc只支持WebAssembly
    /// </summary>
    public partial class WebRtcApi
    { }
}