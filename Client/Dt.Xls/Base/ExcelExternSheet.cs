#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Xls
{
    internal class ExcelExternSheet : IEqualityComparer<ExcelExternSheet>
    {
        internal int beginSheetIndex;
        internal string beginSheetName;
        internal int endSheetIndex;
        internal string endSheetName;
        internal string fileName;
        internal int supBookIndex;

        internal ExcelExternSheet()
        {
        }

        internal ExcelExternSheet(string fn, string begin, string end)
        {
            this.fileName = fn;
            this.beginSheetName = begin;
            this.endSheetName = end;
        }

        public override bool Equals(object obj)
        {
            ExcelExternSheet right = obj as ExcelExternSheet;
            if (obj == null)
            {
                return false;
            }
            return this.Equals(this, right);
        }

        public bool Equals(ExcelExternSheet left, ExcelExternSheet right)
        {
            if (object.ReferenceEquals(left, right))
            {
                return true;
            }
            if ((left == null) || (right == null))
            {
                return false;
            }
            return ((((left.fileName == right.fileName) && (left.beginSheetIndex == right.beginSheetIndex)) && (left.endSheetIndex == right.endSheetIndex)) && (left.supBookIndex == right.supBookIndex));
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            if (this.fileName != null)
            {
                hashCode = this.fileName.GetHashCode();
            }
            return (((hashCode ^ (((int) this.beginSheetIndex).GetHashCode() << 8)) ^ (((int) this.endSheetIndex).GetHashCode() << 0x10)) ^ (((int) this.supBookIndex).GetHashCode() << 0x18));
        }

        public int GetHashCode(ExcelExternSheet obj)
        {
            if (obj == null)
            {
                return 0;
            }
            return obj.GetHashCode();
        }
    }
}

