#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls;
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Xls.OOXml
{
    internal class RangeComparer : IEqualityComparer<IRange>
    {
        public bool Equals(IRange x, IRange y)
        {
            if (object.ReferenceEquals(x, y))
            {
                return true;
            }
            if (object.ReferenceEquals(x, null) || object.ReferenceEquals(null, y))
            {
                return false;
            }
            return ((((x.Row == y.Row) && (x.Column == y.Column)) && (x.RowSpan == y.RowSpan)) && (x.ColumnSpan == y.ColumnSpan));
        }

        public int GetHashCode(IRange obj)
        {
            if (object.ReferenceEquals(obj, null))
            {
                return 0;
            }
            return (((((int) obj.Row).GetHashCode() ^ ((int) obj.Column).GetHashCode()) ^ ((int) obj.RowSpan).GetHashCode()) ^ ((int) obj.ColumnSpan).GetHashCode());
        }
    }
}

