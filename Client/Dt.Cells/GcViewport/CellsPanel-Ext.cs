#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using Dt.Cells.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.UI
{
    internal partial class CellsPanel : Panel
    {
        internal void InvalidateDecorationPanel()
        {
            if (_decoratinPanel != null)
            {
                _decoratinPanel.InvalidateMeasure();
            }
        }

        /// <summary>
        /// 设置打印时隐藏分页线
        /// </summary>
        internal bool HideDecorationWhenPrinting
        {
            set
            {
                if (_decoratinPanel != null)
                {
                    if (value)
                        _decoratinPanel.Visibility = Visibility.Collapsed;
                    else
                        _decoratinPanel.Visibility = Visibility.Visible;
                }
            }
        }
    }
}

