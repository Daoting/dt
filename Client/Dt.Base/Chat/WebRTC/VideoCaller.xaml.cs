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
    public partial class VideoCaller : Dlg
    {
        const string _deviceError = "DeviceError";
        ChatDetail _detail;
        TaskCompletionSource<string> _iceTcs;
        Timer _timer;
        DateTime _startTime;

        public VideoCaller()
        {
            InitializeComponent();
            InitHtml();
            Inst = this;
        }

        public static VideoCaller Inst { get; private set; }

        public async Task ShowDlg(ChatDetail p_detail)
        {
            _detail = p_detail;
            _tbInfo.Text = $"准备呼叫 [{p_detail.Other.Name}]...";

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
            var task = ShowAsync();

            var offer = await InitCallerConnection();
            if (offer == _deviceError)
            {
                Close();
                return;
            }

            int retry = 1;
            while (true)
            {
                bool suc = await AtMsg.SendRtcOffer(Kit.UserID, p_detail.OtherID, offer);
                if (suc)
                    break;

                if (retry++ > 4)
                {
                    Close();
                    Kit.Warn($"等待已超时，[{p_detail.Other.Name}] 未接受邀请！");
                    return;
                }

                await Task.Delay(TimeSpan.FromSeconds(5));
                _tbInfo.Text = $"正在第 {retry} 次呼叫 [{p_detail.Other.Name}]...";
            }

            _tbInfo.Text = $"正在等待 [{p_detail.Other.Name}] 接受邀请...";
            await task;
        }

        public void OnAnswer(string p_answer)
        {
            _tbInfo.Text = "已接受邀请，正在加载视频...";
            var js = $"element.PeerConnection.SetAnswer({p_answer});";
            this.ExecuteJavascript(js);
        }

        public void OnRefuse()
        {
            Close();
            Kit.Warn("对方已挂断！");
        }

        void OnEnd(object sender, RoutedEventArgs e)
        {
            Close();
        }

        async Task<string> InitCallerConnection()
        {
            var js = @"
					(async () => {{
						if(element.PeerConnection) {{
							element.PeerConnection.Close();
						}}
						element.PeerConnection = await Dt.PeerConnection.CreateCaller(element);
					}})();";
            await this.ExecuteJavascriptAsync(js);

            _iceTcs = new TaskCompletionSource<string>();

            var offer = await _iceTcs.Task;

            return offer;
        }

        #region DataChannel事件
        void InitHtml()
        {
            _gridRecv.SetHtmlContent("<video id=\"received_video\" autoplay></video>");
            _gridLocal.SetHtmlContent("<video id=\"local_video\" autoplay muted></video>");

            this.RegisterHtmlCustomEventHandler("DeviceError", OnDeviceError);
            this.RegisterHtmlCustomEventHandler("IceCandidate", OnConnectionIceCandidate, true);
            this.RegisterHtmlEventHandler("Track", OnTrack);
            this.RegisterHtmlEventHandler("Closed", OnConnectionClosed);
        }

        void OnDeviceError(object sender, HtmlCustomEventArgs e)
        {
            var tcs = Interlocked.Exchange(ref _iceTcs, null);
            tcs?.TrySetResult(_deviceError);
            Kit.Warn("打开摄像头或麦克风出错：" + e.Detail);
        }

        void OnConnectionIceCandidate(object sender, HtmlCustomEventArgs e)
        {
            _tbInfo.Text = $"正在呼叫 [{_detail.Other.Name}]...";
            var tcs = Interlocked.Exchange(ref _iceTcs, null);
            tcs?.TrySetResult(e.Detail);
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

                TimeSpan span = DateTime.Now - _startTime;
                string msg = string.Format("通话时长 {0:mm:ss}", new DateTime(span.Ticks));
                _detail.SendMsg(msg);
            }
            else
            {
                _detail.SendMsg("取消通话");
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