#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Mgr.Chat;
using Microsoft.UI.Xaml;
#endregion

namespace Demo.UI
{
    public sealed partial class FsmDemo : Win
    {
        public FsmDemo()
        {
            InitializeComponent();
        }
        
        void OnChat(object sender, RoutedEventArgs e)
        {
            ChatDs.SendLetter(1, new LetterInfo
            {
                ID = Kit.NewGuid,
                SenderID = Kit.UserID,
                SenderName = Kit.UserName,
                LetterType = LetterType.Image,
                Content = "[[\"photo/1.jpg\",\"1\",\"300 x 300 (.jpg)\",49179,\"daoting\",\"2020-03-13 10:37\"]]",
                SendTime = Kit.Now,
            });
        }

        void OnSysMsg(object sender, RoutedEventArgs e)
        {
            AtMsg.SendMsg(1, "Hello");
        }

        void OnCmdMsg(object sender, RoutedEventArgs e)
        {
            AtMsg.SendCmd(1, new MsgInfo
            {
                MethodName = "PushApi.Hello",
                Params = new List<object> { "参数字符串" },
            });
        }
    }
}
