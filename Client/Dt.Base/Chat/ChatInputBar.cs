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
    /// 消息发送栏
    /// </summary>
    public partial class ChatInputBar : Control
    {
        #region 成员变量
        TextBox _tbMsg;
        Lv _lvExt;
        Lv _lvFaces;
        Grid _rootGrid;
        #endregion

        #region 构造方法
        public ChatInputBar()
        {
            DefaultStyleKey = typeof(ChatInputBar);
        }
        #endregion

        #region 属性

        internal ChatDetail Owner { get; set; }
        #endregion

        #region 重写方法
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _rootGrid = (Grid)GetTemplateChild("RootGrid");
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

            // android屏幕键盘只支持KeyUp为Enter的情况，并且AcceptsReturn不能为true！
            if (_tbMsg != null)
            {
                _tbMsg.KeyUp -= OnMsgKeyUp;
                _tbMsg.GotFocus -= OnMsgGotFocus;
            }
            _tbMsg = (TextBox)GetTemplateChild("MsgBox");
            _tbMsg.KeyUp += OnMsgKeyUp;
            _tbMsg.GotFocus += OnMsgGotFocus;
        }
        #endregion

        #region 消息框
        void OnMsgKeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key != VirtualKey.Enter)
                return;

            string msg = _tbMsg.Text.Trim();
            if (!string.IsNullOrEmpty(msg))
            {
                Owner.SendMsg(msg);
                _tbMsg.Text = "";
            }
        }

        void OnMsgGotFocus(object sender, RoutedEventArgs e)
        {
            ClearBottom();
        }
        #endregion

        #region +按钮
        void OnShowExtPanel(object sender, RoutedEventArgs e)
        {
            if (_lvExt == null)
                CreateExtLv();
            SetBottom(_lvExt);
        }

        void CreateExtLv()
        {
            _lvExt = new Lv
            {
                ViewMode = ViewMode.Tile,
                ShowItemBorder = false,
                SelectionMode = SelectionMode.None,
                MinItemWidth = 90,
                View = Application.Current.Resources["InputBarExtTemplate"],
            };
            if (!AtSys.IsPhoneUI)
            {
                _lvExt.MaxWidth = 360;
                _lvExt.HorizontalAlignment = HorizontalAlignment.Right;
            }

            Table tbl = new Table { { "icon", typeof(int) }, { "title" } };
            tbl.AddRow(new { icon = Icons.图片, title = "图片" });
            tbl.AddRow(new { icon = Icons.视频, title = "视频" });
            tbl.AddRow(new { icon = Icons.文件, title = "文件" });

            tbl.AddRow(new { icon = Icons.图片, title = "图片" });
            tbl.AddRow(new { icon = Icons.视频, title = "视频" });
            tbl.AddRow(new { icon = Icons.文件, title = "文件" });
            _lvExt.Data = tbl;
            _lvExt.ItemClick += OnExtClick;
            Grid.SetRow(_lvExt, 1);
        }

        async void OnExtClick(object sender, ItemClickArgs e)
        {
            ClearBottom();
            List<FileData> files = null;
            switch (e.Row.Str("title"))
            {
                case "图片":
                    files = await FileKit.PickImages();
                    break;
                case "视频":
                    files = await FileKit.PickMedias();
                    break;
                case "文件":
                    files = await FileKit.PickFiles();
                    break;
            }
            if (files != null && files.Count > 0)
                Owner.SendFiles(files);
        }
        #endregion

        #region 表情
        void OnShowFacePanel(object sender, RoutedEventArgs e)
        {
            if (_lvFaces == null)
                CreateFaces();
            SetBottom(_lvFaces);
        }

        void CreateFaces()
        {
            _lvFaces = new Lv();
            Grid.SetRow(_lvFaces, 1);
        }
        #endregion

        #region 录音
        void OnAudioCapture(object sender, RoutedEventArgs e)
        {
            ClearBottom();
        }
        #endregion

        #region 底部扩展
        /// <summary>
        /// 用ContentPresenter方式绑定属性在IOS上区域大小不变化，因此采用直接增删方式！！！
        /// </summary>
        /// <param name="p_elem"></param>
        void SetBottom(FrameworkElement p_elem)
        {
            if (_rootGrid.Children.Count > 1)
            {
                if (_rootGrid.Children[1] == p_elem)
                    return;

                _rootGrid.Children.RemoveAt(1);
                _rootGrid.Children.Add(p_elem);
            }
            else
            {
                _rootGrid.Children.Add(p_elem);
                Owner.AttachPressedEvent();
            }
        }

        internal void ClearBottom()
        {
            if (_rootGrid.Children.Count > 1)
            {
                _rootGrid.Children.RemoveAt(1);
                Owner.DetachPressedEvent();
            }
        }
        #endregion
    }
}
