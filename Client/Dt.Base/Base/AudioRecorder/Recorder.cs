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
        /// <returns></returns>
        public static async Task Start()
        {
            if (IsRecording)
                throw new InvalidOperationException("已启动录音");

            if (!await CanRecordAudio)
                throw new InvalidOperationException("设备禁止录音！");

            try
            {
                await Permissions.RequestAsync<Permissions.Microphone>();
            }
            catch
            {
                throw new InvalidOperationException("设备禁止录音！");
            }

            await PlatformRecordAsync();
            IsRecording = true;
        }

        /// <summary>
        /// 停止录音，返回录音文件信息
        /// </summary>
        /// <returns>录音文件信息</returns>
        public static async Task<FileData> Stop()
        {
            var recording = await PlatformStopAsync();
            IsRecording = false;
            return recording;
        }
    }
}