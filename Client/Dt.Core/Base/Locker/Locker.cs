#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 排斥锁，若锁定状态则放弃后续的调用，支持同步和异步，用法：
    /// 
    /// 同步：
    /// _locker.Call(() => action());
    /// 
    /// 异步：
    /// _locker.Call(async () => await action());
    /// </summary>
    public class Locker
    {
        bool _locked;

        /// <summary>
        /// 同步排斥锁，若锁定状态则放弃后续的调用
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
                // 放过 KnownException 类型的异常
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("排斥锁调用时异常：{0}", ex.Message));
            }
            finally
            {
                _locked = false;
            }
        }

        /// <summary>
        /// 异步排斥锁，若锁定状态则放弃后续的异步调用
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
                // 放过 KnownException 类型的异常
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("排斥锁调用时异常：{0}", ex.Message));
            }
            finally
            {
                _locked = false;
            }
        }
        
        /// <summary>
        /// 当前是否已锁定
        /// </summary>
        public bool IsLocked => _locked;
    }
}

