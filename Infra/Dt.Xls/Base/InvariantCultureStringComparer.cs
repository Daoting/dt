#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Xls
{
    internal class InvariantCultureStringComparer : StringComparer
    {
        public InvariantCultureStringComparer(bool ignoreCase = false)
        {
            this.IgnoreCase = ignoreCase;
        }

        public override int Compare(string x, string y)
        {
            return CultureInfo.InvariantCulture.CompareInfo.Compare(x, y, this.IgnoreCase ? ((CompareOptions) CompareOptions.IgnoreCase) : ((CompareOptions) CompareOptions.None));
        }

        public override bool Equals(string x, string y)
        {
            return (CultureInfo.InvariantCulture.CompareInfo.Compare(x, y, this.IgnoreCase ? ((CompareOptions) CompareOptions.IgnoreCase) : ((CompareOptions) CompareOptions.None)) == 0);
        }

        public override int GetHashCode(string obj)
        {
            if (!string.IsNullOrEmpty(obj))
            {
                return obj.GetHashCode();
            }
            return 0;
        }

        public bool IgnoreCase { get; private set; }
    }
}

