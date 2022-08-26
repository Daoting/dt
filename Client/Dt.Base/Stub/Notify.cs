#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation.Collections;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 提示信息相关
    /// </summary>
    public abstract partial class DefaultStub : Stub
    {
        /// <summary>
        /// 获取提示信息列表
        /// </summary>
        public override ItemList<NotifyInfo> NotifyList { get; } = new ItemList<NotifyInfo>();

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
        public override NotifyInfo Msg(string p_content, int p_delaySeconds)
        {
            if (string.IsNullOrEmpty(p_content))
                return null;

            NotifyInfo notify = new NotifyInfo();
            notify.Message = p_content;
            notify.NotifyType = NotifyType.Information;
            notify.Delay = p_delaySeconds;
            Kit.RunAsync(() => NotifyList.Add(notify));
            return notify;
        }

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
        public override NotifyInfo Warn(string p_content, int p_delaySeconds)
        {
            if (string.IsNullOrEmpty(p_content))
                return null;

            NotifyInfo notify = new NotifyInfo();
            notify.Message = p_content;
            notify.NotifyType = NotifyType.Warning;
            notify.Delay = p_delaySeconds;
            Kit.RunAsync(() => NotifyList.Add(notify));
            return notify;
        }

        /// <summary>
        /// 发布消息提示
        /// </summary>
        /// <param name="p_notify">消息提示实例</param>
        public override void Notify(NotifyInfo p_notify)
        {
            if (p_notify != null && !string.IsNullOrEmpty(p_notify.Message))
                Kit.RunAsync(() => NotifyList.Add(p_notify));
        }

        /// <summary>
        /// 关闭消息提示，通常在连接按钮中执行关闭
        /// </summary>
        /// <param name="p_notify"></param>
        public override void CloseNotify(NotifyInfo p_notify)
        {
            if (p_notify != null)
                Kit.RunAsync(() => NotifyList.Remove(p_notify));
        }

        void InitNotify()
        {
            NotifyList.ItemsChanged += OnNotifyItemsChanged;
        }

        void OnNotifyItemsChanged(object sender, ItemListChangedArgs e)
        {
            if (e.CollectionChange == CollectionChange.ItemInserted || e.CollectionChange == CollectionChange.ItemChanged)
            {
                var info = ((ItemList<NotifyInfo>)sender)[e.Index];
                UITree.NotifyPanel.Children.Insert(e.Index, new NotifyItem(info));
            }
            else if (e.CollectionChange == CollectionChange.ItemRemoved)
            {
                UITree.NotifyPanel.Children.RemoveAt(e.Index);
            }
            else
            {
                UITree.NotifyPanel.Children.Clear();
            }
        }
    }
}