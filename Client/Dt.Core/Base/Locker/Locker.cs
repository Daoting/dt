#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 资源锁
    /// </summary>
    public class Locker
    {
        int _lockCount;

        /// <summary>
        /// 加锁
        /// </summary>
        public void Lock()
        {
            _lockCount++;
        }

        /// <summary>
        /// 解锁
        /// </summary>
        public void Unlock()
        {
            if (IsLocked)
            {
                _lockCount--;
            }
        }

        /// <summary>
        /// 锁定并执行调用方法
        /// </summary>
        /// <param name="action">调用方法</param>
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
        /// 若未有锁则执行方法
        /// </summary>
        /// <param name="action">调用方法</param>
        public void DoIfNotLocked(Action action)
        {
            if (!IsLocked)
            {
                action.Invoke();
            }
        }

        /// <summary>
        /// 当前是否已锁定
        /// </summary>
        public bool IsLocked
        {
            get { return (_lockCount > 0); }
        }
    }
}

