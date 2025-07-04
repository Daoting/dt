﻿#region 文件描述
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
    /// 负责所有Lv的资源释放，独立线程排队释放，避免影响 UI 切换速度，避免cpu占用过高
    /// </summary>
    class LvCleaner
    {
#if WIN
        static readonly BlockingCollection<IList> _queue;
        
        static LvCleaner()
        {
            _queue = new BlockingCollection<IList>();
            Task.Run(Clean);
        }
        
        public static bool Add(IList p_target)
        {
            if (p_target == null || p_target.Count == 0)
                return false;

            return _queue.TryAdd(p_target);
        }

        static void Clean()
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
                            if (ls[0] is ILvDestroy cl)
                                cl.Destroy();
                            ls.RemoveAt(0);
                        }
                        ls = null;
                    });
                }
                catch { }
            }
        }
#else
        public static bool Add(IList p_target) => false;
#endif
    }

    /// <summary>
    /// 支持通过 LvCleaner 释放资源的接口
    /// </summary>
    interface ILvDestroy
    {
        /// <summary>
        /// 卸载
        /// </summary>
        void Destroy();
    }
}
