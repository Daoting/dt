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

namespace Dt.Base
{
    /// <summary>
    /// 聊天消息模板选择器
    /// </summary>
    internal class MsgItemSelector : DataTemplateSelector
    {
        static readonly DataTemplate _myMsg;
        static readonly DataTemplate _myFile;
        static readonly DataTemplate _myLink;
        static readonly DataTemplate _myUndoMsg;

        static readonly DataTemplate _otherMsg;
        static readonly DataTemplate _otherFile;
        static readonly DataTemplate _otherLink;
        static readonly DataTemplate _otherUndoMsg;

        static MsgItemSelector()
        {
            var dict = Application.Current.Resources;
            _myMsg = (DataTemplate)dict["MyMsgTemplate"];
            _myFile = (DataTemplate)dict["MyFileTemplate"];
            _myLink = (DataTemplate)dict["MyLinkTemplate"];
            _myUndoMsg = (DataTemplate)dict["MyUndoTemplate"];
            _otherMsg = (DataTemplate)dict["OtherMsgTemplate"];
            _otherFile = (DataTemplate)dict["OtherFileTemplate"];
            _otherLink = (DataTemplate)dict["OtherLinkTemplate"];
            _otherUndoMsg = (DataTemplate)dict["OtherUndoTemplate"];
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
                case LetterType.Undo:
                    return l.IsReceived ? _otherUndoMsg : _myUndoMsg;
                default:
                    return l.IsReceived ? _otherFile : _myFile;
            }
        }
    }
}
