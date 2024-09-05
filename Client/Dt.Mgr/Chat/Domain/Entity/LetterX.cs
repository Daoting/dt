#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-02-18 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
#endregion

namespace Dt.Mgr.Chat
{
    /// <summary>
    /// 聊天内容
    /// </summary>
    public partial class LetterX
    {
        public static async Task<LetterX> New(
            long LoginID = default,
            string MsgID = default,
            long OtherID = default,
            string OtherName = default,
            bool IsReceived = default,
            bool Unread = default,
            LetterType LetterType = default,
            bool OtherIsOnline = default,
            string Content = default,
            DateTime STime = default,
            string Photo = default)
        {
            return new LetterX(
                ID: await NewID(),
                LoginID: LoginID,
                MsgID: MsgID,
                OtherID: OtherID,
                OtherName: OtherName,
                IsReceived: IsReceived,
                Unread: Unread,
                LetterType: LetterType,
                OtherIsOnline: OtherIsOnline,
                Content: Content,
                STime: STime,
                Photo: Photo);
        }
    }
}
