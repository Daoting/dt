#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021/7/5 13:04:20 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Threading;
using System.Threading.Tasks;
using Uno.Extensions;
using Windows.UI.Xaml;
#endregion

namespace Dt.Base.Chat
{
    public partial class VideoRecver : Dlg
    {
        #region 成员变量
        ChatMember _other;
        Timer _timer;
        DateTime _startTime;
        #endregion

        #region 构造方法
        public VideoRecver()
        {
            InitializeComponent();
            InitHtml();
            Inst = this;
        }
        #endregion

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
                SetSize(600, -60);
            }

            _other = AtState.First<ChatMember>($"select * from ChatMember where id={p_fromUserID}");
            _tbInfo.Text = $"[{_other.Name}] 邀请您视频通话...";
            await ShowAsync();
        }

        #region 接收信令消息
        public void OnRecvOffer(string p_offer)
        {
            var js = $@"
					(async () => {{
						if(element.PeerConnection) {{
							element.PeerConnection.Close();
						}}
						element.PeerConnection = await Dt.PeerConnection.CreateReceiver(element, {p_offer}, '{_gridLocal.GetHtmlId()}', '{_gridRecv.GetHtmlId()}');
					}})();";
            this.ExecuteJavascriptAsync(js);
        }

        public void OnHangUp()
        {
            Close(true);
            Kit.Warn("对方已挂断！");
        }
        #endregion

        #region 按钮事件
        async void OnAccept(object sender, RoutedEventArgs e)
        {
            // 确认设备权限
            if (!await VideoCaller.ExistMediaDevice())
            {
                Close(true);
                Kit.Warn("打开摄像头或麦克风出错！");
                await AtMsg.RefuseRtcConnection(Kit.UserID, _other.ID);
                return;
            }

            _tbInfo.Text = $"已接受 [{_other.Name}] 的邀请...";
            await AtMsg.AcceptRtcConnection(Kit.UserID, _other.ID);
            _gridBtn.Visibility = Visibility.Collapsed;
            _btnEnd.Visibility = Visibility.Visible;
        }

        void OnRefuse(object sender, RoutedEventArgs e)
        {
            Close(true);
            AtMsg.RefuseRtcConnection(Kit.UserID, _other.ID);
        }

        void OnEnd(object sender, RoutedEventArgs e)
        {
            Close(false);
        }
        #endregion

        #region WebRTC事件
        void InitHtml()
        {
            _gridRecv.SetHtmlContent("<video id=\"recverRemoteVideo\" autoplay></video>");
            _gridLocal.SetHtmlContent("<video id=\"recverLocalVideo\" autoplay muted></video>");

            this.RegisterHtmlCustomEventHandler("DeviceError", OnDeviceError);
            this.RegisterHtmlCustomEventHandler("Answer", OnAnswer, true);
            this.RegisterHtmlCustomEventHandler("IceCandidate", OnIceCandidate, true);
            this.RegisterHtmlEventHandler("Track", OnTrack);
            this.RegisterHtmlEventHandler("Closed", OnConnectionClosed);
        }

        void OnDeviceError(object sender, HtmlCustomEventArgs e)
        {
            Close(false);
            Kit.Warn("打开摄像头或麦克风出错：" + e.Detail);
        }

        async void OnAnswer(object sender, HtmlCustomEventArgs e)
        {
            await AtMsg.SendRtcAnswer(Kit.UserID, _other.ID, e.Detail);
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
            Close(true);
            Kit.Warn("连线已断开！");
        }
        #endregion

        #region 内部方法
        protected override void OnClosed(bool p_result)
        {
            Inst = null;

            // 未挂断时，请求挂断
            if (!p_result)
                AtMsg.HangUp(Kit.UserID, _other.ID, true);

            var js = @"if(element.PeerConnection) element.PeerConnection.Close();";
            this.ExecuteJavascript(js);

            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }

        void UpdateTimeStr(object state)
        {
            Kit.RunAsync(() =>
            {
                TimeSpan span = DateTime.Now - _startTime;
                _tbInfo.Text = string.Format("{0:mm:ss}", new DateTime(span.Ticks));
            });
        }
        #endregion
    }
}