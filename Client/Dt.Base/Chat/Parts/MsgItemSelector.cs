#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-02-18 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Chat
{
    /// <summary>
    /// 聊天消息模板选择器
    /// </summary>
    internal class MsgItemSelector : DataTemplateSelector
    {
        readonly DataTemplate _myMsg;
        readonly DataTemplate _myFile;
        readonly DataTemplate _myLink;

        readonly DataTemplate _otherMsg;
        readonly DataTemplate _otherFile;
        readonly DataTemplate _otherLink;

        public MsgItemSelector(ChatDetail p_owner)
        {
            _myMsg = (DataTemplate)p_owner.GetResource("MyMsgTemplate");
            _myFile = (DataTemplate)p_owner.GetResource("MyFileTemplate");
            _myLink = (DataTemplate)p_owner.GetResource("MyLinkTemplate");
            _otherMsg = (DataTemplate)p_owner.GetResource("OtherMsgTemplate");
            _otherFile = (DataTemplate)p_owner.GetResource("OtherFileTemplate");
            _otherLink = (DataTemplate)p_owner.GetResource("OtherLinkTemplate");
        }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            Letter l = (Letter)((LvItem)item).Data;
            switch (l.LetterType)
            {
                case LetterType.Text:
                    return l.IsReceived ? _otherMsg : _myMsg;
                case LetterType.Link:
                    return l.IsReceived ? _otherLink : _myLink;
                default:
                    return l.IsReceived ? _otherFile : _myFile;
            }
        }
    }
}
