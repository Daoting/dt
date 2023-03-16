#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Chat
{
    /// <summary>
    /// 通讯录
    /// </summary>
    [View(LobViews.通讯录)]
    public partial class ChatHome : Win
    {
        public ChatHome()
        {
            InitializeComponent();
        }

        void OnItemClick(object sender, long e)
        {
            _detail.OtherID = e;
            NaviTo("聊天内容");
        }

        #region 菜单提示信息
        public static void AttachMenuTip(OmMenu p_om)
        {
            _om = p_om;
            // 菜单重置时会重复调用AttachMenuTip，故先移除
            ChatDs.StateChanged -= OnStateChanged;
            ChatDs.StateChanged += OnStateChanged;
            UpdateMenuTip();
        }

        static void OnStateChanged(long p_otherID)
        {
            UpdateMenuTip();
        }

        static async void UpdateMenuTip()
        {
            int cnt = await AtLob.GetScalar<int>("select count(*) from letter where loginid = @loginid and IsReceived=1 and Unread=1", new { loginid = Kit.UserID });
            _om.SetWarningNum(cnt);
        }
        static OmMenu _om;
        #endregion
    }
}