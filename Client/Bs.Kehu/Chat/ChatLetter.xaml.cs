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
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
#endregion

namespace Dt.Kehu
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ChatLetter : PageWin
    {
        public ChatLetter()
        {
            InitializeComponent();

            _lv.View = new MsgItemSelector
            {
#if UWP
                OtherMsg = (DataTemplate)Resources["OtherMsgTemplate"],
                OtherLink = (DataTemplate)Resources["OtherLinkTemplate"],
                MyMsg = (DataTemplate)Resources["MyMsgTemplate"],
#else
                OtherMsg = StaticResources.OtherMsgTemplate,
                OtherLink = StaticResources.OtherLinkTemplate,
                MyMsg = StaticResources.MyMsgTemplate,
#endif
            };

            List<Letter> ls = new List<Letter>();
            ls.Add(new Letter
            {
                ID = 0,
                Icon = "ms-appx:///Bs.Kehu/Assets/u7878.png",
                IsReceived = true,
                LetterType = LetterType.Text,
                Content = "效果挺好，有进步",
                STime = DateTime.Now
            });

            ls.Add(new Letter
            {
                ID = 1,
                Icon = "ms-appx:///Bs.Kehu/Assets/u7884.png",
                IsReceived = false,
                LetterType = LetterType.Text,
                Content = "继续坚持",
                STime = DateTime.Now
            });

            ls.Add(new Letter
            {
                ID = 2,
                Icon = "ms-appx:///Bs.Kehu/Assets/u7884.png",
                IsReceived = true,
                LetterType = LetterType.Link,
                STime = DateTime.Now
            });

            ls.Add(new Letter
            {
                ID = 3,
                Icon = "ms-appx:///Bs.Kehu/Assets/u7878.png",
                IsReceived = true,
                LetterType = LetterType.Text,
                Content = "长内容：客户端框架结构 \r\n" +
                    "- 页面窗口，继承自PageWin，基础内容控件，包括标题、图标、工具栏和页面内容；手机上承载在PhonePage内进行导航\r\n" +
                    "- 普通窗口 ，继承自Win，窗口在PC上分上下左右和主区五个区域，由Tab承载内容，拖动时自动停靠；手机上自适应为可导航多页面\r\n" +
                    "- 导航窗口 ，停靠式窗口包含多个子Tab，手机上在Tab之间实现页面导航，将Tab承载在PhonePage内\r\n" +
                    "- 对话框 ，模拟传统对话框，PC上显示在窗口上层，可拖动、调整大小、自动关闭等，手机上承载在PhonePage内\r\n" +
                    "- 提示信息，提供两个级别的提示信息（普通、警告），在对话框上层显示，可自动关闭，最多可显示一个操作按钮\r\n" +
                    "- 浮动面板 ，显示在最上层的面板容器，内部使用Popup实现，始终有遮罩",
                STime = DateTime.Now
            });

            ls.Add(new Letter
            {
                ID = 0,
                Icon = "ms-appx:///Bs.Kehu/Assets/u7884.png",
                IsReceived = false,
                LetterType = LetterType.Text,
                Content = "继续坚持",
                STime = DateTime.Now
            });

            _lv.Data = ls;
        }

        void OnItemClick(object sender, ItemClickArgs e)
        {
            Letter l = (Letter)e.Data;
            if (l.ID == 2)
                AtApp.OpenWin(typeof(FriendPtMain));
        }

        void OnSetting(object sender, Mi e)
        {
            AtApp.OpenWin(typeof(SettingFriend));
        }
    }

    public class MsgItemSelector : DataTemplateSelector
    {
        public DataTemplate MyMsg;
        public DataTemplate MyFile;
        public DataTemplate MyLink;
        public DataTemplate MyUndoMsg;

        public DataTemplate OtherMsg;
        public DataTemplate OtherFile;
        public DataTemplate OtherLink;
        public DataTemplate OtherUndoMsg;

        protected override DataTemplate SelectTemplateCore(object item)
        {
            Letter l = (Letter)((LvItem)item).Data;
            switch (l.LetterType)
            {
                case LetterType.Text:
                    return l.IsReceived ? OtherMsg : MyMsg;
                case LetterType.Link:
                    return l.IsReceived ? OtherLink : MyLink;
                case LetterType.Undo:
                    return l.IsReceived ? OtherUndoMsg : MyUndoMsg;
                default:
                    return l.IsReceived ? OtherFile : MyFile;
            }
        }
    }
}