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
    internal partial class ColumnHeaderRowPresenter : RowPresenter
    {
        [ThreadStatic]
        static List<CellPresenterBase> _recycledColHeaderCells;

        public ColumnHeaderRowPresenter(GcViewport viewPort) : base(viewPort)
        {
        }

        protected override CellPresenterBase GenerateNewCell()
        {
            return new ColumnHeaderCellPresenter();
        }

        protected override SheetSpanModelBase GetCellSpanModel()
        {
            return OwningPresenter.Sheet.Worksheet.ColumnHeaderSpanModel;
        }

        public override ColumnLayoutModel GetColumnLayoutModel()
        {
            if (OwningPresenter.SheetArea == SheetArea.ColumnHeader)
            {
                return OwningPresenter.Sheet.GetColumnHeaderViewportColumnLayoutModel(OwningPresenter.ColumnViewportIndex);
            }
            return OwningPresenter.Sheet.GetViewportColumnLayoutModel(OwningPresenter.ColumnViewportIndex);
        }

        protected override List<CellPresenterBase> RecycledCells
        {
            get
            {
                if (_recycledColHeaderCells == null)
                {
                    _recycledColHeaderCells = new List<CellPresenterBase>();
                }
                return _recycledColHeaderCells;
            }
        }
    }
}

