#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2015-07-14 创建
******************************************************************************/
#endregion

#region 引用命名
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 提供给子元素获取实际区域大小，如子元素Lv嵌套在ScrollViewer中时
    /// </summary>
    public partial class SizedPresenter : ContentPresenter
    {
        /// <summary>
        /// 内容的有效区域
        /// </summary>
        public Size AvailableSize { get; set; }

        protected override Size MeasureOverride(Size availableSize)
        {
            AvailableSize = availableSize;
            return base.MeasureOverride(availableSize);
        }
    }
}
