#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls;
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.Data
{
    internal class ExcelColumn : IExcelColumn
    {
        public ExcelColumn(int index)
        {
            this.Index = index;
        }

        public void SetFormatId(int id)
        {
            this.FormatId = id;
        }

        public bool Collapsed { get; set; }

        public IExtendedFormat Format { get; set; }

        public int FormatId { get; internal set; }

        public int Index { get; internal set; }

        public byte OutLineLevel { get; set; }

        public bool PageBreak { get; set; }

        public bool Visible { get; set; }

        public double Width { get; set; }
    }
}

