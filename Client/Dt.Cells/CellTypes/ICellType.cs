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
using Windows.UI.Xaml;
#endregion

namespace Dt.Cells.CellTypes
{
    internal interface ICellType
    {
        bool ApplyEditing(SheetView sheetView, bool allowFormula);
        FrameworkElement GetDisplayElement();
        FrameworkElement GetEditingElement();
        bool HasEditingElement();
        void InitDisplayElement(string text);
        void InitEditingElement();
        void SetEditingElement(FrameworkElement editingElement);

        Cell DataContext { get; set; }
    }
}

