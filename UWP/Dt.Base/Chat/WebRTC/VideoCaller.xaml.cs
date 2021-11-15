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
using Uno.Foundation;
using Windows.UI.Xaml;
#endregion

namespace Dt.Base.Chat
{
    public partial class VideoCaller : Dlg
    {
        #region 成员变量
        ChatDetail _detail;
        Timer _timer;
        DateTime _startTime;
        #endregion

        #region 构造方法
        public VideoCaller()
        {
            InitializeComponent();
            InitHtml();
            Inst = this;
        }
        #endregion

        #region 静态内容
        public static VideoCaller Inst { get; private set; }

        /// <summary>
        /// 判断摄像头和麦克设备是否允许使用
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> ExistMediaDevice()
        {
            var js = @"
					(async () => {{
						try {{
                            await navigator.mediaDevices.getUserMedia({ video: true, audio: true });
                            return 'true';
						}}
                        catch (e) {{ }}
                        return 'false';
					}})();";
            return await WebAssemblyRuntime.InvokeAsync(js) == "true";
        }
        #endregion

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
                SetSize(600, -60);
            }
            var task = ShowAsync();

            // 确认设备权限
            if (!await ExistMediaDevice())
            {
                Close(true);
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
                    Close(true);
                    Kit.Warn($"等待已超时，[{p_detail.Other.Name}] 未接受邀请！");
                    return;
                }

                await Task.Delay(TimeSpan.FromSeconds(5));
                _tbInfo.Text = $"正在第 {retry} 次呼叫 [{p_detail.Other.Name}]...";
            }

            _tbInfo.Text = $"正在等待 [{p_detail.Other.Name}] 接受邀请...";
            await task;
        }

        #region 接收信令消息
        public void OnAcceptConnection()
        {
            _tbInfo.Text = "已接受邀请，正在连线...";
            InitCallerConnection();
        }

        public void OnHangUp()
        {
            Close(true);
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
        #endregion

        #region WebRTC事件
        void InitHtml()
        {
            _gridRecv.SetHtmlContent("<video id=\"callerRemoteVideo\" autoplay></video>");
            _gridLocal.SetHtmlContent("<video id=\"callerLocalVideo\" autoplay muted></video>");

            this.RegisterHtmlCustomEventHandler("DeviceError", OnDeviceError);
            this.RegisterHtmlCustomEventHandler("Offer", OnOffer, true);
            this.RegisterHtmlCustomEventHandler("IceCandidate", OnIceCandidate, true);
            this.RegisterHtmlEventHandler("Track", OnTrack);
            this.RegisterHtmlEventHandler("Closed", OnConnectionClosed);
        }

        void OnDeviceError(object sender, HtmlCustomEventArgs e)
        {
            Close(false);
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
                AtMsg.HangUp(Kit.UserID, _detail.OtherID, false);

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

        void OnEnd(object sender, RoutedEventArgs e)
        {
            Close(false);
        }

        void InitCallerConnection()
        {
            var js = $@"
					(async () => {{
						if(element.PeerConnection) {{
							element.PeerConnection.Close();
						}}
						element.PeerConnection = await Dt.PeerConnection.CreateCaller(element, '{_gridLocal.GetHtmlId()}', '{_gridRecv.GetHtmlId()}');
					}})();";
            this.ExecuteJavascriptAsync(js);
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