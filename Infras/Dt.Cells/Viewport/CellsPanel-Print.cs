#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
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
    }
}

