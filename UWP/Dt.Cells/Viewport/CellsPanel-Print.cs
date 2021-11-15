#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Cells.UI
{
    internal partial class CellsPanel : Panel
    {
        internal void InvalidateDecorationPanel()
        {
            if (_decorationLayer != null)
            {
                _decorationLayer.InvalidateMeasure();
            }
        }

        /// <summary>
        /// 设置打印时隐藏分页线
        /// </summary>
        internal bool HideDecorationWhenPrinting
        {
            set
            {
                if (_decorationLayer != null)
                {
                    if (value)
                        _decorationLayer.Visibility = Visibility.Collapsed;
                    else
                        _decorationLayer.Visibility = Visibility.Visible;
                }
            }
        }
    }
}

