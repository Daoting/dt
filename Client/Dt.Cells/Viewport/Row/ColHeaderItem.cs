#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Cells.UI
{
    internal partial class ColHeaderItem : RowItem
    {
        [ThreadStatic]
        static List<CellItemBase> _recycledColHeaderCells;

        public ColHeaderItem(CellsPanel viewPort) : base(viewPort)
        {
        }

        protected override CellItemBase GenerateNewCell()
        {
            return new ColHeaderCell();
        }

        protected override SheetSpanModelBase GetCellSpanModel()
        {
            return OwningPresenter.Sheet.ActiveSheet.ColumnHeaderSpanModel;
        }

        public override ColumnLayoutModel GetColumnLayoutModel()
        {
            if (OwningPresenter.SheetArea == SheetArea.ColumnHeader)
            {
                return OwningPresenter.Sheet.GetColumnHeaderViewportColumnLayoutModel(OwningPresenter.ColumnViewportIndex);
            }
            return OwningPresenter.Sheet.GetViewportColumnLayoutModel(OwningPresenter.ColumnViewportIndex);
        }

        protected override List<CellItemBase> RecycledCells
        {
            get
            {
                if (_recycledColHeaderCells == null)
                {
                    _recycledColHeaderCells = new List<CellItemBase>();
                }
                return _recycledColHeaderCells;
            }
        }
    }
}

