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
using System.Collections.Generic;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 聊天目录
    /// </summary>
    public sealed partial class ChatInputBar : UserControl
    {
        #region 成员变量
        Menu _menu;
        #endregion

        #region 构造方法
        public ChatInputBar()
        {
            InitializeComponent();
#if IOS
            _tbMsg.GotFocus += OnMsgGotFocus;
#endif
        }
        #endregion

        #region 属性
        internal ChatDetail Owner { get; set; }
        #endregion

        #region 消息框
        void OnMsgKeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key != VirtualKey.Enter)
                return;

            e.Handled = true;
            string msg = _tbMsg.Text.Trim();
            if (!string.IsNullOrEmpty(msg))
            {
                Owner.SendMsg(msg);
                _tbMsg.Text = "";
            }
        }
        #endregion

        #region +按钮
        async void OnShowExtPanel(object sender, RoutedEventArgs e)
        {
            if (AtSys.IsPhoneUI)
            {
#if IOS
                ResetTransform();
#endif
                if (_menu == null)
                {
                    _menu = new Menu { IsContextMenu = true };
                    Mi mi = new Mi { ID = "图片", Icon = Icons.图片 };
                    mi.Click += OnAddPhoto;
                    _menu.Items.Add(mi);

                    mi = new Mi { ID = "拍照", Icon = Icons.拍照 };
                    mi.Click += OnTakePhoto;
                    _menu.Items.Add(mi);

                    mi = new Mi { ID = "视频", Icon = Icons.视频 };
                    mi.Click += OnAddVideo;
                    _menu.Items.Add(mi);

                    mi = new Mi { ID = "录像", Icon = Icons.录像 };
                    mi.Click += OnTakeVideo;
                    _menu.Items.Add(mi);

                    mi = new Mi { ID = "文件", Icon = Icons.文件 };
                    mi.Click += OnAddFile;
                    _menu.Items.Add(mi);
                }
                _ = _menu.OpenContextMenu();
            }
            else
            {
                // 直接选择文件
                var files = await CrossKit.PickFiles();
                if (files != null && files.Count > 0)
                    Owner.SendFiles(files);
            }
        }

        async void OnAddPhoto(object sender, Mi e)
        {
            var files = await CrossKit.PickImages();
            if (files != null && files.Count > 0)
                Owner.SendFiles(files);
        }

        async void OnAddVideo(object sender, Mi e)
        {
            var files = await CrossKit.PickMedias();
            if (files != null && files.Count > 0)
                Owner.SendFiles(files);
        }

        async void OnTakeVideo(object sender, Mi e)
        {
            var fd = await CrossKit.TakeVideo();
            if (fd != null)
                Owner.SendFiles(new List<FileData> { fd });
        }

        async void OnTakePhoto(object sender, Mi e)
        {
            var fd = await CrossKit.TakePhoto();
            if (fd != null)
                Owner.SendFiles(new List<FileData> { fd });
        }

        async void OnAddFile(object sender, Mi e)
        {
            var files = await CrossKit.PickFiles();
            if (files != null && files.Count > 0)
                Owner.SendFiles(files);
        }
        #endregion

        #region 录音
        async void OnAudioCapture(object sender, RoutedEventArgs e)
        {
#if IOS
            ResetTransform();
#endif
            var fileData = await CrossKit.StartRecording(Owner);
            if (fileData != null)
            {
                Owner.SendFiles(new List<FileData> { fileData });
            }
        }
        #endregion

        #region iOS软键盘
#if IOS
        void OnMsgGotFocus(object sender, RoutedEventArgs e)
        {
            // iOS中TextBox只有在ScrollViewer中获得焦点输入时不会被软键盘盖住
            // 此处因外部无法使用ScrollViewer，整个内容区域向上串软键盘高度，如何获取软键盘高度？
            AttachPressedEvent();
            Owner.RenderTransform = new TranslateTransform { Y = -300 };
            //UIKit.UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(alert, true, null);
        }

        void ResetTransform()
        {
            Owner.RenderTransform = null;
            DetachPressedEvent();
        }

        PointerEventHandler _pressedHandler;
        void AttachPressedEvent()
        {
            if (_pressedHandler == null)
                _pressedHandler = new PointerEventHandler(OnPanelPointerPressed);
            // iOS的ScrollViewer无法AddHandler，暂时取内容，bug
            (Owner.Lv.Scroll.Content as UIElement).AddHandler(PointerPressedEvent, _pressedHandler, true);
        }

        void DetachPressedEvent()
        {
            (Owner.Lv.Scroll.Content as UIElement).RemoveHandler(PointerPressedEvent, _pressedHandler);
        }

        void OnPanelPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ResetTransform();
        }
#endif
        #endregion
    }
}
