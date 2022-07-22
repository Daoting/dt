#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections;
#endregion

namespace Dt.Charts
{
    internal static class DataUtils
    {
        public static void TryReset(IEnumerator enumerator)
        {
            try
            {
                enumerator.Reset();
            }
            catch
            {
            }
        }
    }
}

