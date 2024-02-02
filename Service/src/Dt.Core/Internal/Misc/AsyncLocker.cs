#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 异步资源锁，用法：
    /// AsyncLocker _locker = new AsyncLocker();
    /// using (await _locker.LockAsync())
    /// {
    ///     await Func();
    /// }
    /// </summary>
    public class AsyncLocker
    {
        readonly Queue<TaskCompletionSource<bool>> _waiters;
        readonly Task<Releaser> _releaser;
        int _currentCount = 1;

        public AsyncLocker()
        {
            _waiters = new Queue<TaskCompletionSource<bool>>();
            _releaser = Task.FromResult(new Releaser(this));
        }

        /// <summary>
        /// 异步加锁
        /// </summary>
        /// <returns></returns>
        public Task<Releaser> LockAsync()
        {
            if (_currentCount > 0)
            {
                // 无需等待，返回已结束的Task
                _currentCount--;
                return _releaser;
            }

            lock (_waiters)
            {
                var waiter = new TaskCompletionSource<bool>();
                _waiters.Enqueue(waiter);

                return waiter.Task.ContinueWith(
                    (p_task, p_state) => new Releaser((AsyncLocker)p_state),
                    this,
                    CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Default);
            }
        }

        /// <summary>
        /// 当前是否已锁定
        /// </summary>
        public bool IsLocked
        {
            get { return _currentCount == 0; }
        }

        public class Releaser : IDisposable
        {
            readonly AsyncLocker _locker;

            internal Releaser(AsyncLocker p_locker)
            {
                _locker = p_locker;
            }

            public void Dispose()
            {
                if (_locker != null)
                {
                    TaskCompletionSource<bool> taskSource = null;
                    lock (_locker._waiters)
                    {
                        if (_locker._waiters.Count > 0)
                            taskSource = _locker._waiters.Dequeue();
                        else
                            _locker._currentCount++;
                    }
                    if (taskSource != null)
                        taskSource.SetResult(true);
                }
            }
        }
    }
}
