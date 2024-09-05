#region �ļ�����
/******************************************************************************
* ����: Daoting
* ժҪ: 
* ��־: 2015-10-06 ����
******************************************************************************/
#endregion

#region ��������
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// �ɳ������첽�ȴ��¼�����
    /// <para>using (e.Wait())</para>
    /// <para>{</para>
    /// <para>    await Fun();</para>
    /// <para>}</para>
    /// </summary>
    public class AsyncCancelArgs : CancelEventArgs
    {
        List<Task> _tasks;

        /// <summary>
        /// �첽�ȴ�
        /// using (e.Wait())
        /// {
        ///     await Fun();
        /// }
        /// </summary>
        /// <returns></returns>
        public IDisposable Wait()
        {
            return new Deferral(this);
        }

        /// <summary>
        /// �ȴ��ⲿ������������񶼽����������¼�ʱ�ڲ���
        /// </summary>
        public Task EnsureAllCompleted()
        {
            if (_tasks == null || _tasks.Count == 0)
                return Task.CompletedTask;
            if (_tasks.Count == 1)
                return _tasks[0];
            return Task.WhenAll(_tasks);
        }

        /// <summary>
        /// ���Ӷ������ʱ�����ж���첽�ȴ�
        /// </summary>
        /// <param name="p_task"></param>
        void AddTask(Task p_task)
        {
            if (_tasks == null)
                _tasks = new List<Task>();
            _tasks.Add(p_task);
        }

        class Deferral : IDisposable
        {
            AsyncCancelArgs _owner;
            TaskCompletionSource<bool> _taskSrc;

            public Deferral(AsyncCancelArgs p_owner)
            {
                _owner = p_owner;
                _taskSrc = new TaskCompletionSource<bool>();
                _owner.AddTask(_taskSrc.Task);
            }

            public void Dispose()
            {
                if (_taskSrc != null && !_taskSrc.Task.IsCompleted)
                {
                    _taskSrc.SetResult(true);
                    _taskSrc = null;
                }
            }
        }
    }
}