#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-11-12 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 客户端数据项
    /// </summary>
    public partial class Cell : ICell
    {
        /// <summary>
        /// 提示消息或警告信息事件
        /// </summary>
        public event EventHandler<CellMessageArgs> Message;

        /// <summary>
        /// 触发提示消息事件
        /// </summary>
        /// <param name="p_msg"></param>
        /// <returns>是否有订阅该事件的接收者</returns>
        public bool Msg(string p_msg)
        {
            if (Message != null)
            {
                Message(this, new CellMessageArgs(false, p_msg));
                return true;
            }
            return false;
        }

        /// <summary>
        /// 触发警告信息事件
        /// </summary>
        /// <param name="p_msg"></param>
        /// <returns>是否有订阅该事件的接收者</returns>
        public bool Warn(string p_msg)
        {
            if (Message != null)
            {
                Message(this, new CellMessageArgs(true, p_msg));
                return true;
            }
            return false;
        }
    }

    public class CellMessageArgs : EventArgs
    {
        public CellMessageArgs(bool p_isWarning, string p_msg)
        {
            IsWarning = p_isWarning;
            Message = p_msg;
        }

        /// <summary>
        /// 是否为警告信息
        /// </summary>
        public bool IsWarning { get; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; }
    }
}
