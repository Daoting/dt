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
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Xamarin.Essentials;
#endregion

namespace Dt.Base.Chat
{
    /// <summary>
    /// 聊天内容
    /// </summary>
    public sealed partial class ChatDetail : UserControl
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
        ChatMember _other;
        Menu _msgMenu;
        Menu _fileMenu;
        #endregion

        #region 构造方法
        public ChatDetail()
        {
            InitializeComponent();

            _lv.View = new MsgItemSelector(this);
            _inputBar.Owner = this;

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

        internal ChatMember Other => _other;

        internal Lv Lv => _lv;
        #endregion

        #region 外部方法
        /// <summary>
        /// 显示聊天对话框
        /// </summary>
        /// <param name="p_otherID">对方ID</param>
        /// <param name="p_otherName">null时自动查询</param>
        public static void ShowDlg(long p_otherID, string p_otherName = null)
        {
            if (string.IsNullOrEmpty(p_otherName))
            {
                p_otherName = AtState.GetScalar<string>($"select name from ChatMember where id={p_otherID}");
                if (string.IsNullOrEmpty(p_otherName))
                    p_otherName = p_otherID.ToString();
            }

            Dlg dlg;
            if (Kit.IsPhoneUI)
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

        internal object GetResource(string p_key)
        {
            return Resources[p_key];
        }
        #endregion

        #region 加载消息
        void OnLoaded(object sender, RoutedEventArgs e)
        {
            LetterManager.NewLetter += OnNewLetter;
            LetterManager.UndoLetter += OnRecvUndoLetter;
        }

        void OnUnloaded(object sender, RoutedEventArgs e)
        {
            // 页面卸载停止接收新信息
            LetterManager.NewLetter -= OnNewLetter;
            LetterManager.UndoLetter -= OnRecvUndoLetter;
        }

        /// <summary>
        /// 加载信息
        /// </summary>
        async void LoadMsg()
        {
            if (OtherID < 0)
                return;

            string sql = $"select * from ChatMember where id={OtherID}";
            _other = AtState.First<ChatMember>(sql);
            if (_other == null)
            {
                // 初次打开，还未下载好友列表
                await FriendMemberList.Refresh();
                _other = AtState.First<ChatMember>(sql);

                // 不在好友列表时，创建虚拟
                if (_other == null)
                {
                    _other = new ChatMember(
                        ID: OtherID,
                        Name: OtherID.ToString(),
                        Photo: "photo/profilephoto.jpg");
                }
            }

            // 不是好友时无法发送
            //_inputBar.Visibility = (_other == null) ? Visibility.Collapsed : Visibility.Visible;

            LetterManager.ClearUnreadFlag(OtherID);
            _lv.PageData = new PageData { NextPage = OnNextPage, InsertTop = true };
        }

        /// <summary>
        /// 分页加载信息
        /// </summary>
        /// <param name="e"></param>
        void OnNextPage(PageData e)
        {
            int cnt = AtState.GetScalar<int>("select count(*) from Letter where otherid=@otherid and loginid=@loginid",
                new Dict
                {
                    { "otherid", OtherID },
                    { "loginid",  Kit.UserID }
                });

            int start = cnt - (e.PageNo + 1) * e.PageSize;
            int limit = e.PageSize;
            if (start < 0)
                limit = cnt - e.PageNo * e.PageSize;

            Nl<Letter> data = new Nl<Letter>();
            var ls = AtState.Each<Letter>($"select * from Letter where otherid={OtherID} and loginid={Kit.UserID} order by stime limit {limit} offset {start}");
            foreach (var l in ls)
            {
                var photo = l.IsReceived ? _other.Photo : Kit.UserPhoto;
                if (string.IsNullOrEmpty(photo))
                    photo = "photo/profilephoto.jpg";
                l.Photo = photo;
                data.Add(l);
            }
            e.LoadPageData(data);
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
            }
            else if (p_letter.LetterType == LetterType.Text)
            {
                // 当前登录人发出的，文件类型的行已经添加！
                p_letter.Photo = Kit.UserPhoto;
                _lv.Data.Add(p_letter);
            }
        }

        /// <summary>
        /// 收到撤回消息
        /// </summary>
        /// <param name="p_letter"></param>
        void OnRecvUndoLetter(Letter p_letter)
        {
            if (p_letter.OtherID != OtherID)
                return;

            for (int i = 0; i < _lv.Data.Count; i++)
            {
                var l = (Letter)_lv.Data[i];
                if (l.ID == p_letter.ID)
                {
                    _lv.Data.RemoveAt(i);
                    break;
                }
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
                STime = Kit.Now,
                Photo = Kit.UserPhoto,
            };
            _lv.Data.Add(l);

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
                l.MsgID = nl.MsgID;
                l.Content = nl.Content;
            }
            else
            {
                _lv.Data.Remove(l);
            }
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

        #region 菜单
        void OnMsgTapped(object sender, TappedRoutedEventArgs e)
        {
            ShowMsgMenu((Letter)((LvItem)((Dot)sender).DataContext).Data, e);
        }

        void OnMsgRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ShowMsgMenu((Letter)((LvItem)((Dot)sender).DataContext).Data, e);
        }

        void ShowMsgMenu(Letter p_letter, RoutedEventArgs e)
        {
            if (_msgMenu == null)
            {
                _msgMenu = new Menu { IsContextMenu = true };
                Mi mi = new Mi { ID = "复制", Icon = Icons.复制 };
                mi.Click += OnCopyMsg;
                _msgMenu.Items.Add(mi);

                mi = new Mi { ID = "撤回", Icon = Icons.撤消 };
                mi.Click += OnUndoMsg;
                _msgMenu.Items.Add(mi);

                mi = new Mi { ID = "分享", Icon = Icons.分享 };
                mi.Click += OnShareMsg;
                _msgMenu.Items.Add(mi);

                mi = new Mi { ID = "删除", Icon = Icons.删除 };
                mi.Click += OnDelMsg;
                _msgMenu.Items.Add(mi);
            }

            if (p_letter.IsReceived)
                _msgMenu.Hide("撤回");
            else
                _msgMenu.Show("撤回");
            _msgMenu.DataContext = p_letter;
            if (Kit.IsPhoneUI)
            {
                _ = _msgMenu.OpenContextMenu();
            }
            else
            {
                Point pos;
                if (e is TappedRoutedEventArgs t)
                    pos = t.GetPosition(null);
                else if (e is RightTappedRoutedEventArgs rt)
                    pos = rt.GetPosition(null);
                else
                    pos = new Point();
                _ = _msgMenu.OpenContextMenu(pos);
            }
        }

        void OnCopyMsg(object sender, Mi e)
        {
            var l = (Letter)e.DataContext;
            DataPackage data = new DataPackage();
            data.SetText(l.Content);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(data);
        }

        async void OnUndoMsg(object sender, Mi e)
        {
            var l = (Letter)e.DataContext;
            if ((Kit.Now - l.STime).TotalMinutes > 2)
            {
                Kit.Warn("超过2分钟无法撤回");
            }
            else if (await LetterManager.SendUndoLetter(l))
            {
                _lv.Data.Remove(l);
            }
        }

        void OnDelMsg(object sender, Mi e)
        {
            var l = (Letter)e.DataContext;
            AtState.Exec($"delete from Letter where ID={l.ID}");
            _lv.Data.Remove(l);
        }

        async void OnShareMsg(object sender, Mi e)
        {
            var l = (Letter)e.DataContext;
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = l.Content,
                Subject = "分享内容"
            });
        }

        void OnFileRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ShowFileMsgMenu((FileList)sender, e.GetPosition(SysVisual.RootContent));
        }

        void ShowFileMsgMenu(FileList p_fileList, Point p_pos)
        {
            FileItem item = null;
            foreach (var fi in p_fileList.Items)
            {
                Point pt = fi.TransformToVisual(SysVisual.RootContent).TransformPoint(new Point());
                if (p_pos.X > pt.X
                    && p_pos.X < pt.X + fi.ActualWidth
                    && p_pos.Y > pt.Y
                    && p_pos.Y < pt.Y + fi.ActualHeight)
                {
                    item = fi;
                    break;
                }
            }
            if (item == null)
                return;

            Mi mi;
            if (_fileMenu == null)
            {
                _fileMenu = new Menu { IsContextMenu = true };
                mi = new Mi { ID = "保存", Icon = Icons.保存 };
                mi.SetBinding(Mi.CmdProperty, new Binding { Path = new PropertyPath("CmdSaveAs") });
                _fileMenu.Items.Add(mi);

                mi = new Mi { ID = "撤回", Icon = Icons.撤消 };
                mi.Click += OnUndoMsg;
                _fileMenu.Items.Add(mi);

                mi = new Mi { ID = "分享", Icon = Icons.分享 };
                mi.SetBinding(Mi.CmdProperty, new Binding { Path = new PropertyPath("CmdShare") });
                _fileMenu.Items.Add(mi);

                mi = new Mi { ID = "删除", Icon = Icons.删除 };
                mi.Click += OnDelMsg;
                _fileMenu.Items.Add(mi);
            }

            _fileMenu.DataContext = item;
            Letter letter = (Letter)((LvItem)p_fileList.DataContext).Data;
            _fileMenu["删除"].DataContext = letter;
            mi = _fileMenu["撤回"];
            if (letter.IsReceived)
            {
                mi.Visibility = Visibility.Collapsed;
                mi.DataContext = null;
            }
            else
            {
                mi.Visibility = Visibility.Visible;
                mi.DataContext = letter;
            }

            if (Kit.IsPhoneUI)
                _ = _fileMenu.OpenContextMenu();
            else
                _ = _fileMenu.OpenContextMenu(p_pos);
        }
        #endregion
    }
}
