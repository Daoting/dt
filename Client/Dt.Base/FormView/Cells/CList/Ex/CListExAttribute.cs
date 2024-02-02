#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-12-12 创建
******************************************************************************/
#endregion

#region 引用命名

#endregion

namespace Dt.Base
{
    /// <summary>
    /// CList功能扩展的类型标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CListExAttribute : TypeAliasAttribute
    {
        public CListExAttribute()
        {
        }
    }
}
