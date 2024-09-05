#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-04-25 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 自定义获取值的类型标签，被标注的类型为静态类型，每个公共方法可通过xaml或字符串中的描述调用，常用在：
    /// <para>1.报表中提供给参数缺省值 或 直接用在报表中</para>
    /// <para>2.CList的sql扩展中，提供给sql参数</para>
    /// <para>调用字符串格式：@类名.方法(空 或 字符串参数)</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ValueCallAttribute : TypeAliasAttribute
    {
        public ValueCallAttribute()
        { }
    }
}
