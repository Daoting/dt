#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2015-10-06 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 异步任务等待事件参数
    /// <para>using (e.Wait())</para>
    /// <para>{</para>
    /// <para>    await Fun();</para>
    /// <para>}</para>
    /// </summary>
    public class AsyncEventArgs : EventArgs
    {
        List<Task> _tasks;

        /// <summary>
        /// 异步等待
        /// <para>using (e.Wait())</para>
        /// <para>{</para>
        /// <para>    await Fun();</para>
        /// <para>}</para>
        /// </summary>
        /// <returns></returns>
        public IDisposable Wait()
        {
            return new Deferral(this);
        }

        /// <summary>
        /// 等待外部处理的所有任务都结束，触发事件时内部用
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
        /// 附加多个处理时可能有多个异步等待
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
            AsyncEventArgs _owner;
            TaskCompletionSource<bool> _taskSrc;

            public Deferral(AsyncEventArgs p_owner)
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

