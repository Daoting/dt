#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 提示信息相关
    /// </summary>
    public interface INotify
    {
        /// <summary>
        /// 获取提示信息列表
        /// </summary>
        ItemList<NotifyInfo> NotifyList { get; }

        /// <summary>
        /// 发布消息提示
        /// </summary>
        /// <param name="p_content">显示内容</param>
        /// <param name="p_delaySeconds">
        /// 几秒后自动关闭，默认3秒
        /// <para>大于0：启动定时器自动关闭，点击也关闭</para>
        /// <para>0：不自动关闭，但点击关闭</para>
        /// <para>小于0：始终不关闭，只有程序控制关闭</para>
        /// </param>
        NotifyInfo Msg(string p_content, int p_delaySeconds);

        /// <summary>
        /// 警告提示
        /// </summary>
        /// <param name="p_content">显示内容</param>
        /// <param name="p_delaySeconds">
        /// 几秒后自动关闭，默认5秒
        /// <para>大于0：启动定时器自动关闭，点击也关闭</para>
        /// <para>0：不自动关闭，但点击关闭</para>
        /// <para>小于0：始终不关闭，只有程序控制关闭</para>
        /// </param>
        NotifyInfo Warn(string p_content, int p_delaySeconds);

        /// <summary>
        /// 发布消息提示
        /// </summary>
        /// <param name="p_notify">消息提示实例</param>
        void Notify(NotifyInfo p_notify);

        /// <summary>
        /// 关闭消息提示，通常在连接按钮中执行关闭
        /// </summary>
        /// <param name="p_notify"></param>
        void CloseNotify(NotifyInfo p_notify);
    }
}