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
using Windows.UI.Xaml;
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
        /// <param name="p_target">��ʱ�Ի�����е�Ŀ��</param>
        /// <returns>¼���ļ���Ϣ��ʧ�ܻ����ʱ����null</returns>
        public static async Task<FileData> Start(FrameworkElement p_target)
        {
            if (IsRecording)
            {
                AtKit.Warn("������¼��");
                return null;
            }

            if (!await CanRecordAudio)
            {
                AtKit.Warn("����˷��豸���޷�¼����");
                return null;
            }

            try
            {
                await Permissions.RequestAsync<Permissions.Microphone>();
            }
            catch
            {
                AtKit.Warn("�豸��ֹ¼����");
                return null;
            }

            IsRecording = true;
            await PlatformRecordAsync();

            // ��ʾ��ʱ��
            var dlg = new AudioRecordDlg();
            dlg.PlacementTarget = p_target;
            bool isOk = await dlg.ShowAsync();

            // ��ʱ��رգ�ֹͣ¼��
            FileData fd = await PlatformStopAsync();
            // ¼��ʱ��
            fd.Desc = dlg.Duration;
            IsRecording = false;

            return isOk ? fd : null;
        }
    }
}