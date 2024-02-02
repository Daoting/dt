#if IOS
#region �ļ�����
/******************************************************************************
* ����: Daoting
* ժҪ: 
* ��־: 2020-04-29 ����
******************************************************************************/
#endregion

#region ��������
using AVFoundation;
using Dt.Core;
using Foundation;
using System;
using System.IO;
using System.Threading.Tasks;
using UIKit;
#endregion

namespace Dt.Base
{
    class AudioRecorder
    {
        AVAudioRecorder _recorder;
        string _audioFilePath;

        /// <summary>
        /// �Ƿ�����˷�
        /// </summary>
        public Task<bool> CanRecordAudio = Task.FromResult(AVAudioSession.SharedInstance().InputAvailable);

        /// <summary>
        /// �Ƿ�����¼��
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
            // ��Ƶ��ʽ m4a
            NSNumber.FromInt32((int)AudioToolbox.AudioFormatType.MPEG4AAC),
            // ������
            NSNumber.FromInt32(1),
            // ������:8000(AM�㲥���͵�¼��Ч��)��16000��22050����44100(CD�����Ĳ�����)
            NSNumber.FromInt32(16000),
        });
#pragma warning restore SA1118 // Parameter should not span multiple lines
    }
}
#endif