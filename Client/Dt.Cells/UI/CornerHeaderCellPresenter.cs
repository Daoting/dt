using Dt.Cells.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dt.Cells.UI
{
    public partial class CornerHeaderCellPresenter : HeaderCellPresenter
    {
        public CornerHeaderCellPresenter()
        {
            base.DefaultStyleKey = typeof(CornerHeaderCellPresenter);
        }

        internal override bool IsHightlighted
        {
            get { return false; }
        }

        protected override bool IsMouseOver
        {
            get { return base.OwningRow.OwningPresenter.Sheet.HoverManager.IsMouseOverCornerHeaders; }
        }

        protected override bool IsSelected
        {
            get
            {
                Worksheet worksheet = base.OwningRow.OwningPresenter.Sheet.Worksheet;
                if (worksheet.Selections.Count != 1)
                {
                    return false;
                }
                CellRange range = worksheet.Selections[0];
                return ((((range.Column == -1) && (range.Row == -1)) && (range.RowCount == -1)) && (range.ColumnCount == -1));
            }
        }
    }
}
