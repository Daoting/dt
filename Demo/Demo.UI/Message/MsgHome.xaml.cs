#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dt.Base;
using Dt.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.UI
{
    public sealed partial class MsgHome : Win
    {
        public MsgHome()
        {
            InitializeComponent();
            _nav.Data = Dir;
        }

        public static Nl<Nav> Dir { get; } = new Nl<Nav>
        {
            new Nav("网络消息", typeof(FsmDemo), Icons.耳麦) { Desc = "处理客户端和服务端之间的实时消息交互" },
            new Nav("本地消息", typeof(LocalEventDemo), Icons.双绞线) { Desc = "本地事件，客户端全局事件定义及处理" },
            new Nav("提示信息", typeof(NotifyDemo), Icons.留言) { Desc = "普通信息、警告信息，可带按钮，延时关闭可设置" },
            new Nav("托盘通知", typeof(TrayDemo), Icons.发送) { Desc = "任务栏托盘通知" },
            new Nav("操作系统通知", typeof(ToastDemo), Icons.信件) { Desc = "操作系统级通知" },
        };
    }
}
