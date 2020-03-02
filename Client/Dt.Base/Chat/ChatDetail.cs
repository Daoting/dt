#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-02-18 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;
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

        public static readonly DependencyProperty BottomContentProperty = DependencyProperty.Register(
            "BottomContent",
            typeof(object),
            typeof(ChatDetail),
            new PropertyMetadata(null));

        static void OnOtherIDChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ChatDetail)d).LoadMsg();
        }
        #endregion

        Lv _lv;
        TextBox _tbMsg;
        Grid _inputGrid;
        ChatMember _other;

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

        /// <summary>
        /// 发送栏底部的内容，表情符或扩展内容，内部绑定用
        /// </summary>
        internal object BottomContent
        {
            get { return GetValue(BottomContentProperty); }
            set { SetValue(BottomContentProperty, value); }
        }
        #endregion

        #region 重写方法
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _lv = (Lv)GetTemplateChild("Lv");
            _lv.View = new MsgItemSelector();

            _inputGrid = (Grid)GetTemplateChild("InputGrid");
            var btn = (Button)GetTemplateChild("BtnVoice");
            if (btn != null)
            {
                btn.Click -= OnAudioCapture;
                btn.Click += OnAudioCapture;
            }

            btn = (Button)GetTemplateChild("BtnFace");
            if (btn != null)
            {
                btn.Click -= OnShowFacePanel;
                btn.Click += OnShowFacePanel;
            }

            btn = (Button)GetTemplateChild("BtnAdd");
            if (btn != null)
            {
                btn.Click -= OnShowExtPanel;
                btn.Click += OnShowExtPanel;
            }

            if (_tbMsg != null)
                _tbMsg.KeyDown -= OnMsgKeyDown;
            _tbMsg = (TextBox)GetTemplateChild("MsgBox");
            if (_tbMsg != null)
                _tbMsg.KeyDown += OnMsgKeyDown;
        }
        #endregion

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
            _inputGrid.Visibility = (_other == null) ? Visibility.Collapsed : Visibility.Visible;

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
            var data = AtLocal.Query<Letter>($"select * from Letter where otherid={OtherID} and loginid={AtUser.ID} order by stime limit {limit} offset {start}");
            e.LoadPageData(data);
            if (data != null && data.Count == 0)
                _lv.Data = data;
            if (e.PageNo == 0)
                _lv.ScrollBottom();
        }

        /// <summary>
        /// 增加聊天消息事件
        /// </summary>
        /// <param name="p_letter"></param>
        void OnNewLetter(Letter p_letter)
        {
            if (p_letter.IsReceived)
            {
                // 是否为当前聊天人
                if (p_letter.OtherID == OtherID)
                {
                    // 已读标志
                    p_letter.Unread = false;
                    if (p_letter.LetterType == LetterType.Undo)
                    {
                        // 对方撤回消息
                        var ls = (List<Letter>)_lv.Data;
                        var letter = ls.FirstOrDefault(l => l.ID == p_letter.ID);
                        if (letter != null)
                        {
                            letter.LetterType = LetterType.Undo;
                            int index = ls.IndexOf(letter);
                            ls.RemoveAt(index);
                            _lv.InsertRow(letter, index);
                        }
                        return;
                    }
                    AddLetter(p_letter);
                }
            }
            else
            {
                // 当前登录人发出的，文件类型的行已经添加！
                if (p_letter.LetterType == LetterType.Text)
                    AddLetter(p_letter);
            }
        }

        /// <summary>
        /// 添加信息行
        /// </summary>
        /// <param name="p_letter"></param>
        void AddLetter(Letter p_letter)
        {
            _lv.InsertRow(p_letter);
            _lv.ScrollBottom();
        }


        #region 发送栏
        void OnAudioCapture(object sender, RoutedEventArgs e)
        {

        }

        void OnShowFacePanel(object sender, RoutedEventArgs e)
        {

        }

        void OnShowExtPanel(object sender, RoutedEventArgs e)
        {

        }

        async void OnMsgKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key != VirtualKey.Enter)
                return;

            string msg = _tbMsg.Text.Trim();
            if (!string.IsNullOrEmpty(msg))
            {
                await LetterManager.SendLetter(OtherID, _other.Name, msg, LetterType.Text);
                _tbMsg.Text = "";
            }
        }
        #endregion

    }
}
