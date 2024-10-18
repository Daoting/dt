#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-06-11 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using System.Collections.Concurrent;
#endregion

namespace Dt.Base.Docking
{
    /// <summary>
    /// 负责所有Win内部资源的释放，独立线程排队释放，避免影响 UI 切换速度，避免cpu占用过高
    /// </summary>
    class WinCleaner
    {
        readonly BlockingCollection<Win> _queue;

        public WinCleaner()
        {
            _queue = new BlockingCollection<Win>();
            Task.Run(Clean);
        }

        public bool Add(Win p_win)
        {
            if (p_win == null)
                return false;
            return _queue.TryAdd(p_win);
        }

        void Clean()
        {
            while (true)
            {
                try
                {
                    var win = _queue.Take();
                    Kit.RunSync(() =>
                    {
                        try
                        {
                            if (win.AllTabs != null)
                            {
                                foreach (var tab in win.AllTabs.Values)
                                {
                                    // NavList
                                    if (tab is IDestroy tc)
                                    {
                                        tc.Destroy();
                                    }
                                    else if (tab.Content is IDestroy wc)
                                    {
                                        wc.Destroy();
                                    }
                                    else if (tab.Content is UIElement elem)
                                    {
                                        foreach (var cl in elem.FindChildrenByType<IDestroy>())
                                        {
                                            cl.Destroy();
                                        }
                                    }
                                }
                            }
                            win.OnDestroyed();
                        }
                        catch { }
                        win = null;
                    });

                    // 以窗口为单位释放
                    GC.Collect();
                }
                catch { }
            }
        }
    }
}