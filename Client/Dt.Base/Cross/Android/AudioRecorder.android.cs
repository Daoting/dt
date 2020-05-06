#if ANDROID
#region �ļ�����
/******************************************************************************
* ����: Daoting
* ժҪ: 
* ��־: 2020-04-29 ����
******************************************************************************/
#endregion

#region ��������
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
        /// �Ƿ�����˷�
        /// </summary>
        public Task<bool> CanRecordAudio = Task.FromResult(Platform.AppContext.PackageManager.HasSystemFeature(PackageManager.FeatureMicrophone));

        /// <summary>
        /// �Ƿ�����¼��
        /// </summary>
        public bool IsRecording { get; set; }

        public Task PlatformRecordAsync()
        {
            _recorder = new MediaRecorder();
            _recorder.SetAudioSource(AudioSource.Mic);
            _recorder.SetOutputFormat(OutputFormat.Mpeg4);
            _recorder.SetAudioEncoder(AudioEncoder.Aac);
            _audioFilePath = Path.Combine(AtLocal.CachePath, AtKit.NewID + ".m4a");
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