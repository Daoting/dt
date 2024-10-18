#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-11-15 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.ListView;
using Dt.Base.TreeViews;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 接口
    /// </summary>
    public partial class Tv : IDestroy
    {
        /// <summary>
        /// 负责切换数据源时的释放
        /// </summary>
        internal static readonly TvCleaner Cleaner = new TvCleaner();

        public void Destroy()
        {
            KeyDown -= OnKeyDown;
            if (Scroll != null)
                Scroll.ViewChanged -= OnScrollViewChanged;
            
            _panel?.Destroy();

            if (RootItems?.Count > 0)
            {
                RootItems.Destroy();
            }
            ClearSelectionOnDataChanged();

            if (_dataView != null)
            {
                _dataView.Destroy();
                _dataView = null;
            }
        }
    }
}