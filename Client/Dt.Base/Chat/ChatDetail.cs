﻿#region 文件描述
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
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 聊天内容
    /// </summary>
    public partial class ChatDetail : Control
    {
        #region 静态成员
        public static readonly DependencyProperty OtherIDProperty = DependencyProperty.Register(
            "OtherID",
            typeof(long),
            typeof(ChatDetail),
            new PropertyMetadata(-1L, OnOtherIDChanged));

        static void OnOtherIDChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ChatDetail)d).LoadMsg();
        }
        #endregion

        #region 成员变量
        Lv _lv;
        ChatInputBar _inputBar;
        ChatMember _other;
        #endregion

        #region 构造方法
        public ChatDetail()
        {
            DefaultStyleKey = typeof(ChatDetail);

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取对方ID
        /// </summary>
        public long OtherID
        {
            get { return (long)GetValue(OtherIDProperty); }
            set { SetValue(OtherIDProperty, value); }
        }
        #endregion

        /// <summary>
        /// 显示聊天对话框
        /// </summary>
        /// <param name="p_otherID">对方ID</param>
        /// <param name="p_otherName">null时自动查询</param>
        public static void ShowDlg(long p_otherID, string p_otherName = null)
        {
            if (string.IsNullOrEmpty(p_otherName))
                p_otherName = AtLocal.GetScalar<string>($"select name from ChatMember where id={p_otherID}");

            Dlg dlg;
            if (AtSys.IsPhoneUI)
            {
                dlg = new Dlg { Title = p_otherName, };
            }
            else
            {
                dlg = new Dlg()
                {
                    IsPinned = true,
                    Height = 500,
                    Width = 400,
                    Title = p_otherName,
                };
            }

            ChatDetail chat = new ChatDetail();
            chat.OtherID = p_otherID;
            dlg.Content = chat;
            dlg.Show();
        }

        #region 重写方法
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _lv = (Lv)GetTemplateChild("Lv");
            _lv.View = new MsgItemSelector();

            _inputBar = (ChatInputBar)GetTemplateChild("InputBar");
            _inputBar.Owner = this;
        }
        #endregion

        #region 加载消息
        void OnLoaded(object sender, RoutedEventArgs e)
        {
            LoadMsg();
            LetterManager.NewLetter += OnNewLetter;
        }

        void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // 页面卸载停止接收新信息
            LetterManager.NewLetter -= OnNewLetter;
        }

        /// <summary>
        /// 加载信息
        /// </summary>
        void LoadMsg()
        {
            if (_lv == null || OtherID < 0)
                return;

            _other = AtLocal.GetFirst<ChatMember>("select * from ChatMember where id=@id", new Dict { { "id", OtherID } });
            // 不是好友时无法发送
            _inputBar.Visibility = (_other == null) ? Visibility.Collapsed : Visibility.Visible;

            LetterManager.ClearUnreadFlag(OtherID);
            _lv.PageData = new PageData { NextPage = OnNextPage, InsertTop = true };
        }

        /// <summary>
        /// 分页加载信息
        /// </summary>
        /// <param name="e"></param>
        void OnNextPage(PageData e)
        {
            int cnt = AtLocal.GetScalar<int>("select count(*) from Letter where otherid=@otherid and loginid=@loginid",
                new Dict
                {
                    { "otherid", OtherID },
                    { "loginid",  AtUser.ID }
                });

            int start = cnt - (e.PageNo + 1) * e.PageSize;
            int limit = e.PageSize;
            if (start < 0)
                limit = cnt - e.PageNo * e.PageSize;

            Nl<Letter> data = new Nl<Letter>();
            var ls = AtLocal.DeferredQuery<Letter>($"select * from Letter where otherid={OtherID} and loginid={AtUser.ID} order by stime limit {limit} offset {start}");
            foreach (var l in ls)
            {
                l.Photo = l.IsReceived ? _other.Photo : AtUser.Photo;
                data.Add(l);
            }

            e.LoadPageData(data);
            if (data != null && data.Count == 0)
                _lv.Data = data;
            if (e.PageNo == 0)
                _lv.ScrollBottom();
        }
        #endregion

        #region 接收新消息
        /// <summary>
        /// 增加聊天消息事件
        /// </summary>
        /// <param name="p_letter"></param>
        void OnNewLetter(Letter p_letter)
        {
            // 是否为当前聊天人
            if (p_letter.OtherID != OtherID)
                return;

            if (p_letter.IsReceived)
            {
                // 已读标志
                p_letter.Unread = false;
                p_letter.Photo = _other.Photo;
                _lv.Data.Add(p_letter);
                _lv.ScrollBottom();
            }
            else if (p_letter.LetterType == LetterType.Text)
            {
                // 当前登录人发出的，文件类型的行已经添加！
                p_letter.Photo = AtUser.Photo;
                _lv.Data.Add(p_letter);
                _lv.ScrollBottom();
            }
        }
        #endregion

        #region 发送消息
        /// <summary>
        /// 发送普通消息
        /// </summary>
        /// <param name="p_msg"></param>
        public async void SendMsg(string p_msg)
        {
            await LetterManager.SendLetter(OtherID, _other.Name, p_msg, LetterType.Text);
        }

        /// <summary>
        /// 发送文件
        /// </summary>
        /// <param name="p_files"></param>
        public async void SendFiles(List<FileData> p_files)
        {
            Letter l = new Letter
            {
                OtherID = OtherID,
                OtherName = _other.Name,
                IsReceived = false,
                Unread = false,
                LetterType = GetLetterType(p_files),
                STime = AtSys.Now,
                Photo = AtUser.Photo,
            };
            _lv.Data.Add(l);
            _lv.ScrollBottom();

            FileList fl;
            var elem = _lv.GetRowUI(_lv.Data.Count - 1);
            if (elem == null || (fl = elem.FindChildByType<FileList>()) == null)
            {
                _lv.Data.Remove(l);
                return;
            }

            bool suc = await fl.UploadFiles(p_files);
            if (suc)
            {
                var nl = await LetterManager.SendLetter(OtherID, _other.Name, fl.Data, l.LetterType);
                l.ID = nl.ID;
            }
            else
            {
                _lv.Data.Remove(l);
            }
            _lv.ScrollBottom();
        }

        LetterType GetLetterType(List<FileData> p_files)
        {
            LetterType type = LetterType.File;
            if (p_files.All(p_file => FileFilter.UwpImage.Contains(p_file.Ext.ToLower())))
            {
                type = LetterType.Image;
            }
            else if (p_files.All(p_file => FileFilter.UwpVideo.Contains(p_file.Ext.ToLower())))
            {
                type = LetterType.Video;
            }
            else if (p_files.All(p_file => FileFilter.UwpAudio.Contains(p_file.Ext.ToLower())))
            {
                type = LetterType.Voice;
            }
            return type;
        }
        #endregion

        #region 隐藏底部
        PointerEventHandler _pressedHandler;
        internal void AttachPressedEvent()
        {
            if (_pressedHandler == null)
                _pressedHandler = new PointerEventHandler(OnPanelPointerPressed);
#if IOS
            // iOS的ScrollViewer无法AddHandler，暂时取内容，bug
            (_lv.Scroll.Content as UIElement).AddHandler(PointerPressedEvent, _pressedHandler, true);
#else
            _lv.AddHandler(PointerPressedEvent, _pressedHandler, true);
#endif
        }

        internal void DetachPressedEvent()
        {
#if IOS
            (_lv.Scroll.Content as UIElement).RemoveHandler(PointerPressedEvent, _pressedHandler);
#else
            _lv.RemoveHandler(PointerPressedEvent, _pressedHandler);
#endif
        }

        void OnPanelPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _inputBar.ClearBottom();
        }
        #endregion
    }
}
