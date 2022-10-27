#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Mgr
{
    /// <summary>
    /// 标志处理服务器推送的标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PushApiAttribute : TypeAliasAttribute
    {
        public PushApiAttribute()
        {
        }
    }
}
