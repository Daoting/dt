#if ANDROID
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-04-29 创建
******************************************************************************/
#endregion

#region 引用命名
using Android.Content.PM;
using Android.Media;
using Dt.Core;
using Java.IO;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Encoding = Android.Media.Encoding;
#endregion

namespace Dt.Base
{
    class AudioRecorder
    {
        MediaRecorder _recorder;
        string _audioFilePath;

        /// <summary>
        /// 是否有麦克风
        /// </summary>
        public Task<bool> CanRecordAudio = Task.FromResult(Platform.AppContext.PackageManager.HasSystemFeature(PackageManager.FeatureMicrophone));

        /// <summary>
        /// 是否正在录音
        /// </summary>
        public bool IsRecording { get; set; }

        public Task PlatformRecordAsync()
        {
            _recorder = new MediaRecorder();
            _recorder.SetAudioSource(AudioSource.Mic);
            _recorder.SetOutputFormat(OutputFormat.Mpeg4);
            _recorder.SetAudioEncoder(AudioEncoder.Aac);
            _audioFilePath = Path.Combine(Kit.CachePath, Kit.NewGuid + ".m4a");
            _recorder.SetOutputFile(_audioFilePath);
            _recorder.Prepare();
            _recorder.Start();
            return Task.CompletedTask;
        }

        public Task<FileData> PlatformStopAsync()
        {
            if (_recorder == null)
                return Task.FromResult(default(FileData));

            _recorder.Stop();
            _recorder.Release();
            _recorder = null;

            Java.IO.File file = new Java.IO.File(_audioFilePath);
            return Task.FromResult(new FileData(_audioFilePath, file.Name, (ulong)file.Length()));
        }
    }
}
#endif