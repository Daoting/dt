#region �ļ�����
/******************************************************************************
* ����: Daoting
* ժҪ: 
* ��־: 2017-12-06 ����
******************************************************************************/
#endregion

#region ��������
#endregion

namespace Dt.Core
{
    /// <summary>
    /// �ų�����������״̬����������ĵ��ã�֧��ͬ�����첽���÷���
    /// 
    /// ͬ����
    /// _locker.Call(() => action());
    /// 
    /// �첽��
    /// _locker.Call(async () => await action());
    /// </summary>
    public class Locker
    {
        bool _locked;

        /// <summary>
        /// ͬ���ų�����������״̬����������ĵ���
        /// </summary>
        /// <param name="p_action"></param>
        public void Call(Action p_action)
        {
            if (_locked)
                return;

            try
            {
                _locked = true;
                p_action();
            }
            catch (KnownException)
            {
                // �Ź� KnownException ���͵��쳣
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("�ų�������ʱ�쳣��{0}", ex.Message));
            }
            finally
            {
                _locked = false;
            }
        }

        /// <summary>
        /// �첽�ų�����������״̬������������첽����
        /// </summary>
        /// <param name="p_func"></param>
        public async void Call(Func<Task> p_func)
        {
            if (_locked)
                return;

            try
            {
                _locked = true;
                await p_func();
            }
            catch (KnownException)
            {
                // �Ź� KnownException ���͵��쳣
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("�ų�������ʱ�쳣��{0}", ex.Message));
            }
            finally
            {
                _locked = false;
            }
        }
        
        /// <summary>
        /// ��ǰ�Ƿ�������
        /// </summary>
        public bool IsLocked => _locked;
    }
}

