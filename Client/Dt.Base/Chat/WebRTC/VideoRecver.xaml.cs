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
        TaskCompletionSource<string> _iceTcs;
        string _offer;
        Timer _timer;
        DateTime _startTime;

        public VideoRecver()
        {
            InitializeComponent();
            InitHtml();
            Inst = this;
        }

        public static VideoRecver Inst { get; private set; }

        public async Task ShowDlg(long p_fromUserID, string p_offer)
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

            _offer = p_offer;
            _other = AtState.First<ChatMember>($"select * from ChatMember where id={p_fromUserID}");
            _tbInfo.Text = $"[{_other.Name}] 邀请您视频通话...";
            await ShowAsync();
        }

        void OnRefuse(object sender, RoutedEventArgs e)
        {
            Close();
            AtMsg.RefuseRtcOffer(Kit.UserID, _other.ID);
        }

        async void OnAccept(object sender, RoutedEventArgs e)
        {
            _gridBtn.Visibility = Visibility.Collapsed;
            _tbInfo.Text = $"已接受 [{_other.Name}] 的邀请...";
            var iceAnswer = await InitPeerRemoteConnection();
            await AtMsg.SendRtcAnswer(Kit.UserID, _other.ID, iceAnswer);
            _btnEnd.Visibility = Visibility.Visible;
        }

        void OnEnd(object sender, RoutedEventArgs e)
        {
            Close();
        }

        async Task<string> InitPeerRemoteConnection()
        {
            var js = $@"
					(async () => {{
						if(element.PeerConnection) {{
							element.PeerConnection.Close();
						}}
						element.PeerConnection = await Dt.PeerConnection.CreateRemote(element, {_offer});
					}})();";
            await this.ExecuteJavascriptAsync(js);

            _iceTcs = new TaskCompletionSource<string>();

            var answer = await _iceTcs.Task;

            return answer;
        }

        #region DataChannel事件
        void InitHtml()
        {
            _peer.SetHtmlContent("<video id=\"vPeer\" />");
            _self.SetHtmlContent("<video id=\"vSelf\" />");

            this.RegisterHtmlEventHandler("Opened", OnConnectionOpened);
            this.RegisterHtmlCustomEventHandler("Error", OnConnectionError);
            this.RegisterHtmlEventHandler("Closed", OnConnectionClosed);
            this.RegisterHtmlCustomEventHandler("IceCandidate", OnConnectionIceCandidate, true);
        }

        void OnConnectionOpened(object sender, EventArgs e)
        {
            _tbInfo.Text = "已打开连接";
            _tbInfo.VerticalAlignment = VerticalAlignment.Bottom;
            _tbInfo.Margin = new Thickness(0, 0, 0, 40);

            _startTime = DateTime.Now;
            _timer = new Timer(UpdateTimeStr, null, 0, 1000);
        }

        void OnConnectionError(object sender, HtmlCustomEventArgs e)
        {
            _tbInfo.Text = "连接出错：" + e.Detail;
        }

        void OnConnectionClosed(object sender, EventArgs e)
        {
            Close();
        }

        void OnConnectionIceCandidate(object sender, HtmlCustomEventArgs e)
        {
            _tbInfo.Text = $"正在回复 [{_other.Name}] ...";
            var tcs = Interlocked.Exchange(ref _iceTcs, null);
            tcs?.TrySetResult(e.Detail);
        }
        #endregion

        protected override void OnClosed()
        {
            Inst = null;
            this.ExecuteJavascriptAsync(@"(() => {{ if(element.PeerConnection) element.PeerConnection.Close(); }})();");
            StopTimer();
        }

        void UpdateTimeStr(object state)
        {
            Kit.RunAsync(() =>
            {
                TimeSpan span = DateTime.Now - _startTime;
                _tbInfo.Text = string.Format("{0:mm:ss}", new DateTime(span.Ticks));
            });
        }

        void StopTimer()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }
    }
}