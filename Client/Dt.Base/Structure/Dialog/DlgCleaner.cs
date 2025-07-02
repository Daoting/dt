#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-10-09 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.Docking;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Input;
using System.Collections.Concurrent;
#endregion

namespace Dt.Base
{
    class DlgCleaner
    {
#if WIN
        static readonly BlockingCollection<Dlg> _queue;
        
        static DlgCleaner()
        {
            _queue = new BlockingCollection<Dlg>();
            Task.Run(Clean);
        }
        
        public static bool Add(Dlg p_dlg)
        {
            if (p_dlg == null || p_dlg.Content == null)
                return false;
            return _queue.TryAdd(p_dlg);
        }

        static void Clean()
        {
            while (true)
            {
                try
                {
                    var dlg = _queue.Take();
                    Kit.RunSync(() =>
                    {
                        try
                        {
                            if (dlg.Content is IDestroy tc)
                            {
                                tc.Destroy();
                            }
                            else if (dlg.Content is UIElement elem)
                            {
                                foreach (var cl in elem.FindChildrenByType<IDestroy>())
                                {
                                    cl.Destroy();
                                }
                            }
                        }
                        catch { }

                        dlg.Content = null;
                        dlg = null;
                    });
                }
                catch { }
            }
        }
#else
        public static bool Add(Dlg p_dlg) => false;
#endif
    }
}
