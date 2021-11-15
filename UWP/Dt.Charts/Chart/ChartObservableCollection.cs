#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
#endregion

namespace Dt.Charts
{
    internal class ChartObservableCollection : ObservableCollection<UIElement>
    {
        internal event EventHandler OnClear;

        protected override void ClearItems()
        {
            if (OnClear != null)
            {
                OnClear(this, EventArgs.Empty);
            }
            base.ClearItems();
        }
    }
}

