#region �ļ�����
/******************************************************************************
* ����: Daoting
* ժҪ: 
* ��־: 2017-12-06 ����
******************************************************************************/
#endregion

#region ��������
using System;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// ��Դ��
    /// </summary>
    public class Locker
    {
        int _lockCount;

        /// <summary>
        /// ����
        /// </summary>
        public void Lock()
        {
            _lockCount++;
        }

        /// <summary>
        /// ����
        /// </summary>
        public void Unlock()
        {
            if (IsLocked)
            {
                _lockCount--;
            }
        }

        /// <summary>
        /// ������ִ�е��÷���
        /// </summary>
        /// <param name="action">���÷���</param>
        public void DoLockedAction(Action action)
        {
            Lock();
            try
            {
                action.Invoke();
            }
            finally
            {
                Unlock();
            }
        }

        /// <summary>
        /// ��δ������ִ�з���
        /// </summary>
        /// <param name="action">���÷���</param>
        public void DoIfNotLocked(Action action)
        {
            if (!IsLocked)
            {
                action.Invoke();
            }
        }

        /// <summary>
        /// ��ǰ�Ƿ�������
        /// </summary>
        public bool IsLocked
        {
            get { return (_lockCount > 0); }
        }
    }
}

