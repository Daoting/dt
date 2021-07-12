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
        Timer _timer;
        DateTime _startTime;

        public VideoCaller()
        {
            InitializeComponent();
            InitHtml();
            Inst = this;
        }

        public static VideoCaller Inst { get; private set; }

        public long OtherID => _detail.OtherID;

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

            // 确认设备权限
            if (!await ExistMediaDevice())
            {
                Close();
                Kit.Warn("打开摄像头或麦克风出错！");
                return;
            }

            // 确认在线
            int retry = 1;
            while (true)
            {
                bool suc = await AtMsg.RequestRtcConnection(Kit.UserID, p_detail.OtherID);
                if (suc)
                    break;

                // 不在线重试
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

        public void OnAcceptConnection()
        {
            _tbInfo.Text = "已接受邀请，正在连线...";
            InitCallerConnection();
        }

        public void OnHangUp()
        {
            Close();
            Kit.Warn("对方已挂断！");
        }

        public void OnRecvAnswer(string p_answer)
        {
            var js = $"element.PeerConnection.SetAnswer({p_answer});";
            this.ExecuteJavascript(js);
        }

        public void OnRecvIceCandidate(string p_iceCandidate)
        {
            var js = $"element.PeerConnection.AddIceCandidate({p_iceCandidate});";
            this.ExecuteJavascript(js);
        }

        void OnEnd(object sender, RoutedEventArgs e)
        {
            Close();
            AtMsg.HangUp(Kit.UserID, _detail.OtherID, false);
        }

        /// <summary>
        /// 判断摄像头和麦克设备是否允许使用
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> ExistMediaDevice()
        {
            var js = @"
					(async () => {{
						try {{
                            await navigator.mediaDevices.getUserMedia({ video: true, audio: false });
                            return 'true';
						}}
                        catch (e) {{ }}
                        return 'false';
					}})();";
            return await WebAssemblyRuntime.InvokeAsync(js) == "true";
        }

        void InitCallerConnection()
        {
            var js = @"
					(async () => {{
						if(element.PeerConnection) {{
							element.PeerConnection.Close();
						}}
						element.PeerConnection = await Dt.PeerConnection.CreateCaller(element);
					}})();";
            this.ExecuteJavascriptAsync(js);
        }

        #region DataChannel事件
        void InitHtml()
        {
            _gridRecv.SetHtmlContent("<video id=\"received_video\" autoplay></video>");
            _gridLocal.SetHtmlContent("<video id=\"local_video\" autoplay muted></video>");

            this.RegisterHtmlCustomEventHandler("DeviceError", OnDeviceError);
            this.RegisterHtmlCustomEventHandler("Offer", OnOffer, true);
            this.RegisterHtmlCustomEventHandler("IceCandidate", OnIceCandidate, true);
            this.RegisterHtmlEventHandler("Track", OnTrack);
            this.RegisterHtmlEventHandler("Closed", OnConnectionClosed);
        }

        void OnDeviceError(object sender, HtmlCustomEventArgs e)
        {
            Close();
            Kit.Warn("打开摄像头或麦克风出错：" + e.Detail);
        }

        void OnOffer(object sender, HtmlCustomEventArgs e)
        {
            _tbInfo.Text = $"正在呼叫 [{_detail.Other.Name}]...";
            AtMsg.SendRtcOffer(Kit.UserID, _detail.OtherID, e.Detail);
        }

        void OnIceCandidate(object sender, HtmlCustomEventArgs e)
        {
            AtMsg.SendIceCandidate(Kit.UserID, _detail.OtherID, e.Detail, false);
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

        protected override void OnClosed(bool p_result)
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