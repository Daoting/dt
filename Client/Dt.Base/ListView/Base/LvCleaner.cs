#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-06-11 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections;
using System.Collections.Concurrent;
#endregion

namespace Dt.Base.ListView
{
    /// <summary>
    /// 负责所有Lv的资源释放，独立线程排队释放，提高UI切换速度，避免cpu占用过高
    /// </summary>
    class LvCleaner
    {
        readonly BlockingCollection<IList> _queue;

        public LvCleaner()
        {
            _queue = new BlockingCollection<IList>();
            Task.Run(Clean);
        }

        public bool Add(IList p_target)
        {
            if (p_target == null || p_target.Count == 0)
                return false;

            return _queue.TryAdd(p_target);
        }

        void Clean()
        {
            while (true)
            {
                try
                {
                    var ls = _queue.Take();
                    Kit.RunSync(() =>
                    {
                        while (ls.Count > 0)
                        {
                            if (ls[0] is ILvCleaner cl)
                                cl.Unload();
                            ls.RemoveAt(0);
                        }
                        ls = null;
                    });
                }
                catch { }
            }
        }
    }

    /// <summary>
    /// 支持通过 LvCleaner 释放资源的接口
    /// </summary>
    interface ILvCleaner
    {
        /// <summary>
        /// 卸载
        /// </summary>
        void Unload();
    }
}
