#if WIN
#region �ļ�����
/******************************************************************************
* ����: Daoting
* ժҪ: 
* ��־: 2020-04-29 ����
******************************************************************************/
#endregion

#region ��������
using Dt.Core;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
#endregion

namespace Dt.Base
{
    class AudioRecorder
    {
        MediaCapture _mediaCapture;
        string _audioFilePath;

        /// <summary>
        /// �Ƿ�����˷�
        /// </summary>
        public Task<bool> CanRecordAudio = Task.FromResult(true);

        /// <summary>
        /// �Ƿ�����¼��
        /// </summary>
        public bool IsRecording { get; set; }

        public async Task PlatformRecordAsync()
        {
            try
            {
                _mediaCapture = new MediaCapture();
                await _mediaCapture.InitializeAsync(new MediaCaptureInitializationSettings { StreamingCaptureMode = StreamingCaptureMode.Audio });
                _mediaCapture.RecordLimitationExceeded += sender =>
                {
                    DeleteMediaCapture();
                    throw new Exception("Record Limitation Exceeded");
                };

                _mediaCapture.Failed += (sender, errorEventArgs) =>
                {
                    DeleteMediaCapture();
                    throw new Exception($"Audio recording failed: {errorEventArgs.Code}. {errorEventArgs.Message}");
                };

                _audioFilePath = Path.Combine(Kit.CachePath, Kit.NewGuid + ".m4a");
                var file = await StorageFile.GetFileFromPathAsync(_audioFilePath);
                await _mediaCapture.StartRecordToStorageFileAsync(MediaEncodingProfile.CreateM4a(AudioEncodingQuality.Auto), file);
            }
            catch
            {
                DeleteMediaCapture();
                throw;
            }
        }

        public async Task<FileData> PlatformStopAsync()
        {
            if (_mediaCapture == null)
                return null;

            await _mediaCapture.StopRecordAsync();
            _mediaCapture.Dispose();
            _mediaCapture = null;
            var file = await StorageFile.GetFileFromPathAsync(_audioFilePath);
            return new FileData(_audioFilePath, file.Name, (await file.GetBasicPropertiesAsync()).Size); ;
        }

        public void DeleteMediaCapture()
        {
            _mediaCapture?.Dispose();
            try
            {
                if (!string.IsNullOrWhiteSpace(_audioFilePath) && File.Exists(_audioFilePath))
                    File.Delete(_audioFilePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting audio file: {ex}");
            }

            _audioFilePath = "";
            _mediaCapture = null;
        }
    }
}
#endif