#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
#endregion

namespace Dt.Core
{
    
    [Api]
    public class SysCall
    {

    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ApiAttribute : TypeAliasAttribute
    {
        public ApiAttribute()
        {
        }
    }
}
