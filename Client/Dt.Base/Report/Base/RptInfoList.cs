#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-09-25 创建
**************************************************************************/
#endregion

#region 命名空间
using System.Collections.Generic;
#endregion

namespace Dt.Base.Report
{
    /// <summary>
    /// 报表描述信息列表，只为打开窗口时识别窗口用
    /// </summary>
    internal class RptInfoList : List<RptInfo>
    {
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is RptInfoList))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            var ls = (RptInfoList)obj;
            if (Count != ls.Count)
                return false;

            for (int i = 0; i < Count; i++)
            {
                if (this[i].Name != ls[i].Name)
                    return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
