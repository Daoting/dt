#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021/7/5 13:04:20 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Uno.Extensions;
using Uno.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#endregion

namespace Dt.Base.Chat
{
    public partial class VideoRecver : Dlg
    {
        ChatMember _other;
        Timer _timer;
        DateTime _startTime;

        public VideoRecver()
        {
            InitializeComponent();
            InitHtml();
            Inst = this;
        }

        public static VideoRecver Inst { get; private set; }

        public async Task ShowDlg(long p_fromUserID)
        {
            if (Kit.IsPhoneUI)
            {
                HideTitleBar = true;
            }
            else
            {
                IsPinned = true;
                Height = 800;
                Width = 600;
            }

            _other = AtState.First<ChatMember>($"select * from ChatMember where id={p_fromUserID}");
            _tbInfo.Text = $"[{_other.Name}] 邀请您视频通话...";
            await ShowAsync();
        }

        async void OnAccept(object sender, RoutedEventArgs e)
        {
            // 确认设备权限
            if (!await VideoCaller.ExistMediaDevice())
            {
                Close();
                Kit.Warn("打开摄像头或麦克风出错！");
                await AtMsg.RefuseRtcConnection(Kit.UserID, _other.ID);
                return;
            }

            await AtMsg.AcceptRtcConnection(Kit.UserID, _other.ID);
            _gridBtn.Visibility = Visibility.Collapsed;
            _tbInfo.Text = $"已接受 [{_other.Name}] 的邀请...";
            _btnEnd.Visibility = Visibility.Visible;
        }

        void OnRefuse(object sender, RoutedEventArgs e)
        {
            Close();
            AtMsg.RefuseRtcConnection(Kit.UserID, _other.ID);
        }

        void OnEnd(object sender, RoutedEventArgs e)
        {
            Close();
            AtMsg.HangUp(Kit.UserID, _other.ID, true);
        }

        public void OnRecvOffer(string p_offer)
        {
            var js = $@"
					(async () => {{
						if(element.PeerConnection) {{
							element.PeerConnection.Close();
						}}
						element.PeerConnection = await Dt.PeerConnection.CreateRemote(element, {p_offer});
					}})();";
            this.ExecuteJavascriptAsync(js);
        }

        public void OnHangUp()
        {
            Close();
            Kit.Warn("对方已挂断！");
        }

        #region DataChannel事件
        void InitHtml()
        {
            _gridRecv.SetHtmlContent("<video id=\"received_video\" autoplay></video>");
            _gridLocal.SetHtmlContent("<video id=\"local_video\" autoplay muted></video>");

            this.RegisterHtmlCustomEventHandler("DeviceError", OnDeviceError);
            this.RegisterHtmlCustomEventHandler("Answer", OnAnswer, true);
            this.RegisterHtmlCustomEventHandler("IceCandidate", OnIceCandidate, true);
            this.RegisterHtmlEventHandler("Track", OnTrack);
            this.RegisterHtmlEventHandler("Closed", OnConnectionClosed);
        }

        void OnDeviceError(object sender, HtmlCustomEventArgs e)
        {
            Close();
            Kit.Warn("打开摄像头或麦克风出错：" + e.Detail);
        }

        async void OnAnswer(object sender, HtmlCustomEventArgs e)
        {
            if (!await AtMsg.SendRtcAnswer(Kit.UserID, _other.ID, e.Detail))
            {
                Close();
                Kit.Warn($"呼叫失败，对方不在线！");
            }
        }

        void OnIceCandidate(object sender, HtmlCustomEventArgs e)
        {
            AtMsg.SendIceCandidate(Kit.UserID, _other.ID, e.Detail, true);
        }

        void OnTrack(object sender, EventArgs e)
        {
            _tbInfo.VerticalAlignment = VerticalAlignment.Bottom;
            _tbInfo.Margin = new Thickness(0, 0, 0, 40);

            _startTime = DateTime.Now;
            _timer = new Timer(UpdateTimeStr, null, 0, 1000);
        }

        void OnConnectionClosed(object sender, EventArgs e)
        {
            Close();
            Kit.Warn("连线已断开！");
        }
        #endregion

        protected override Task<bool> OnClosing()
        {
            Inst = null;
            var js = @"if(element.PeerConnection) element.PeerConnection.Close();";
            this.ExecuteJavascript(js);

            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
            return Task.FromResult(true);
        }

        void UpdateTimeStr(object state)
        {
            Kit.RunAsync(() =>
            {
                TimeSpan span = DateTime.Now - _startTime;
                _tbInfo.Text = string.Format("{0:mm:ss}", new DateTime(span.Ticks));
            });
        }
    }
}