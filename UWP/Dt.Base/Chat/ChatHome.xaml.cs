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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Chat
{
    /// <summary>
    /// 通讯录
    /// </summary>
    [View("通讯录")]
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
    }
}