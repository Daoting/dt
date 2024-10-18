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

namespace Dt.Base.TreeViews
{
    /// <summary>
    /// 负责所有Tv的资源释放，独立线程排队释放，避免影响 UI 切换速度，避免cpu占用过高
    /// </summary>
    class TvCleaner
    {
        readonly BlockingCollection<TvRootItems> _queue;

        public TvCleaner()
        {
            _queue = new BlockingCollection<TvRootItems>();
            Task.Run(Clean);
        }

        public bool Add(TvRootItems p_target)
        {
            if (p_target != null)
                return _queue.TryAdd(p_target);
            return false;
        }

        void Clean()
        {
            while (true)
            {
                try
                {
                    var root = _queue.Take();
                    Kit.RunSync(() =>
                    {
                        root.Destroy();
                        root = null;
                    });
                }
                catch { }
            }
        }
    }
}
