#if UWP
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
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
#endregion

namespace Dt.Base
{
    public static partial class AudioRecorder
    {
        static Task<bool> PlatformCanRecordAudio => Task.FromResult(true);

        static MediaCapture _mediaCapture;
        static string _audioFilePath;

        static async Task PlatformRecordAsync()
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

                _audioFilePath = Path.Combine(AtLocal.CachePath, AtKit.NewID + ".m4a");
                var file = await StorageFile.GetFileFromPathAsync(_audioFilePath);
                await _mediaCapture.StartRecordToStorageFileAsync(MediaEncodingProfile.CreateM4a(AudioEncodingQuality.Auto), file);
            }
            catch
            {
                DeleteMediaCapture();
                throw;
            }
        }

        static async Task<FileData> PlatformStopAsync()
        {
            if (_mediaCapture == null)
                return null;

            await _mediaCapture.StopRecordAsync();
            _mediaCapture.Dispose();
            _mediaCapture = null;
            var file = await StorageFile.GetFileFromPathAsync(_audioFilePath);
            return new FileData(_audioFilePath, file.Name, (await file.GetBasicPropertiesAsync()).Size); ;
        }

        static void DeleteMediaCapture()
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

            _audioFilePath = string.Empty;
            _mediaCapture = null;
        }
    }
}
#endif