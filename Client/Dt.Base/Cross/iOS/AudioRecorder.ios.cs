#if IOS
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-04-29 创建
******************************************************************************/
#endregion

#region 引用命名
using AVFoundation;
using Dt.Core;
using Foundation;
using System;
using System.IO;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Essentials;
#endregion

namespace Dt.Base
{
    class AudioRecorder
    {
        AVAudioRecorder _recorder;
        string _audioFilePath;

        /// <summary>
        /// 是否有麦克风
        /// </summary>
        public Task<bool> CanRecordAudio = Task.FromResult(AVAudioSession.SharedInstance().InputAvailable);

        /// <summary>
        /// 是否正在录音
        /// </summary>
        public bool IsRecording { get; set; }

        public Task PlatformRecordAsync()
        {
            InitAudioSession();

            _audioFilePath = Path.Combine(Kit.CachePath, Kit.NewGuid + ".m4a");
            var url = NSUrl.FromFilename(_audioFilePath);
            _recorder = AVAudioRecorder.Create(url, new AudioSettings(_settings), out var error);
            if (error != null)
                ThrowNSError(error);
            _recorder.Record();
            return Task.CompletedTask;
        }

        public Task<FileData> PlatformStopAsync()
        {
            if (_recorder == null)
                return Task.FromResult(default(FileData));

            _recorder.Stop();
            _recorder.Dispose();
            _recorder = null;
            AVAudioSession.SharedInstance().SetActive(false);

            FileInfo fi = new FileInfo(_audioFilePath);
            return Task.FromResult(new FileData(_audioFilePath, fi.Name, (ulong)fi.Length));
        }

        void InitAudioSession()
        {
            var audioSession = AVAudioSession.SharedInstance();
            var err = audioSession.SetCategory(AVAudioSessionCategory.Record);
            if (err != null)
                ThrowNSError(err);

            err = audioSession.SetActive(true);
            if (err != null)
                ThrowNSError(err);
        }

        static void ThrowNSError(NSError error)
            => throw new Exception(error.ToString());

        static readonly NSDictionary<NSString, NSObject> _settings = new NSDictionary<NSString, NSObject>(
        new[]
        {
            AVAudioSettings.AVFormatIDKey,
            AVAudioSettings.AVNumberOfChannelsKey,
            AVAudioSettings.AVSampleRateKey,
#pragma warning disable SA1118 // Parameter should not span multiple lines
        }, new NSObject[]
        {
            // 音频格式 m4a
            NSNumber.FromInt32((int)AudioToolbox.AudioFormatType.MPEG4AAC),
            // 声道数
            NSNumber.FromInt32(1),
            // 采样率:8000(AM广播类型的录制效果)，16000，22050或者44100(CD质量的采样率)
            NSNumber.FromInt32(16000),
        });
#pragma warning restore SA1118 // Parameter should not span multiple lines
    }
}
#endif