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
    internal partial class RowHeaderItem : RowItem
    {
        [ThreadStatic]
        static List<CellPresenterBase> _recycledRowHeaderCells;

        public RowHeaderItem(CellsPanel viewport) : base(viewport)
        {
        }

        protected override CellPresenterBase GenerateNewCell()
        {
            return new RowHeaderCellPresenter();
        }

        protected override SheetSpanModelBase GetCellSpanModel()
        {
            return OwningPresenter.Sheet.ActiveSheet.RowHeaderSpanModel;
        }

        public override ColumnLayoutModel GetColumnLayoutModel()
        {
            return OwningPresenter.Sheet.GetRowHeaderColumnLayoutModel();
        }

        protected override List<CellPresenterBase> RecycledCells
        {
            get
            {
                if (_recycledRowHeaderCells == null)
                {
                    _recycledRowHeaderCells = new List<CellPresenterBase>();
                }
                return _recycledRowHeaderCells;
            }
        }
    }
}

