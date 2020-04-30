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
using System.Threading.Tasks;
using Xamarin.Essentials;
#endregion

namespace Dt.Base
{
    public static partial class AudioRecorder
    {
        public static Task<bool> CanRecordAudio => PlatformCanRecordAudio;

        /// <summary>
        /// �Ƿ�����¼��
        /// </summary>
        public static bool IsRecording { get; private set; }

        /// <summary>
        /// ��ʼ¼��
        /// </summary>
        /// <returns></returns>
        public static async Task Start()
        {
            if (IsRecording)
                throw new InvalidOperationException("������¼��");

            if (!await CanRecordAudio)
                throw new InvalidOperationException("�豸��ֹ¼����");

            try
            {
                await Permissions.RequestAsync<Permissions.Microphone>();
            }
            catch
            {
                throw new InvalidOperationException("�豸��ֹ¼����");
            }

            await PlatformRecordAsync();
            IsRecording = true;
        }

        /// <summary>
        /// ֹͣ¼��������¼���ļ���Ϣ
        /// </summary>
        /// <returns>¼���ļ���Ϣ</returns>
        public static async Task<FileData> Stop()
        {
            var recording = await PlatformStopAsync();
            IsRecording = false;
            return recording;
        }
    }
}