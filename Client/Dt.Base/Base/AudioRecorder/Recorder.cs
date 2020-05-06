#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-04-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Xamarin.Essentials;
#endregion

namespace Dt.Base
{
    public static partial class AudioRecorder
    {
        public static Task<bool> CanRecordAudio => PlatformCanRecordAudio;

        /// <summary>
        /// 是否正在录音
        /// </summary>
        public static bool IsRecording { get; private set; }

        /// <summary>
        /// 开始录音
        /// </summary>
        /// <param name="p_target">计时对话框居中的目标</param>
        /// <returns>录音文件信息，失败或放弃时返回null</returns>
        public static async Task<FileData> Start(FrameworkElement p_target)
        {
            if (IsRecording)
            {
                AtKit.Warn("已启动录音");
                return null;
            }

            if (!await CanRecordAudio)
            {
                AtKit.Warn("无麦克风设备，无法录音！");
                return null;
            }

            try
            {
                await Permissions.RequestAsync<Permissions.Microphone>();
            }
            catch
            {
                AtKit.Warn("设备禁止录音！");
                return null;
            }

            IsRecording = true;
            await PlatformRecordAsync();

            // 显示计时框
            var dlg = new AudioRecordDlg();
            dlg.PlacementTarget = p_target;
            bool isOk = await dlg.ShowAsync();

            // 计时框关闭，停止录音
            FileData fd = await PlatformStopAsync();
            // 录音时长
            fd.Desc = dlg.Duration;
            IsRecording = false;

            return isOk ? fd : null;
        }
    }
}