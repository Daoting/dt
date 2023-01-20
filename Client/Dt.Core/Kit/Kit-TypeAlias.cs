#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-05-17 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Reflection;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 类型别名
    /// </summary>
    public partial class Kit
    {
        /// <summary>
        /// 根据别名获取视图类型
        /// </summary>
        /// <param name="p_alias">类型别名</param>
        /// <returns>返回类型</returns>
        public static Type GetViewTypeByAlias(string p_alias)
        {
            return Stub.Inst.GetTypesByAlias(typeof(ViewAttribute), p_alias).FirstOrDefault();
        }

        /// <summary>
        /// 根据别名获取视图类型
        /// </summary>
        /// <param name="p_enumAlias">别名取枚举成员名称</param>
        /// <returns>返回类型</returns>
        public static Type GetViewTypeByAlias(Enum p_enumAlias)
        {
            return Stub.Inst.GetTypesByAlias(typeof(ViewAttribute), p_enumAlias.ToString()).FirstOrDefault();
        }

        /// <summary>
        /// 根据类型别名获取所有类型列表
        /// </summary>
        /// <param name="p_attrType">标签类型</param>
        /// <param name="p_alias">别名</param>
        /// <returns>返回类型</returns>
        public static List<Type> GetAllTypesByAlias(Type p_attrType, string p_alias)
        {
            return Stub.Inst.GetTypesByAlias(p_attrType, p_alias);
        }

        /// <summary>
        /// 根据类型别名和方法名获取方法定义，取列表中第一个匹配项
        /// </summary>
        /// <param name="p_attrType">标签类型</param>
        /// <param name="p_alias">别名</param>
        /// <param name="p_methodName">方法名</param>
        /// <param name="p_flags">要包含BindingFlags.Public 或 BindingFlags.NonPublic，否则返回null，静态的方法还要有BindingFlags.Static</param>
        /// <returns></returns>
        public static MethodInfo GetMethodByAlias(Type p_attrType, string p_alias, string p_methodName, BindingFlags p_flags)
        {
            var ls = Stub.Inst.GetTypesByAlias(p_attrType, p_alias);
            return (from tp in ls
                    let mi = tp.GetMethod(p_methodName, p_flags)
                    where mi != null
                    select mi).FirstOrDefault();
        }

        /// <summary>
        /// 发布本地事件，不等待
        /// </summary>
        /// <param name="p_event">事件对象，禁止事件为泛型</param>
        public static async void PublishEvent(IEvent p_event)
        {
            if (p_event == null)
                return;

            var ls = GetAllTypesByAlias(typeof(EventHandlerAttribute), p_event.GetType().Name);
            if (ls.Count > 0)
            {
                foreach (var tp in ls)
                {
                    var mi = tp.GetMethod("Handle");
                    if (mi != null )
                    {
                        try
                        {
                            // 按顺序调用
                            var tgt = Activator.CreateInstance(tp);
                            await(Task)mi.Invoke(tgt, new object[] { p_event });
                        }
                        catch (Exception e)
                        {
                            Log.Warning(e, $"{tp.Name}处理本地事件时异常！");
                        }
                    }
                }
            }
        }
    }
}
