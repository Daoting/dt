#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Xls
{
    internal class ExternalRangeExpression : RangeExpression
    {
        private object source;

        public ExternalRangeExpression(object source, int row, int column, int rowCount, int columnCount) : this(source, row, column, rowCount, columnCount, false, false)
        {
        }

        public ExternalRangeExpression(object source, int row, int column, int rowCount, int columnCount, bool rowRelative, bool columnRelative) : base(row, column, rowCount, columnCount, rowRelative, columnRelative)
        {
            this.source = source;
        }

        public object Source
        {
            get { return  this.source; }
        }
    }
}

