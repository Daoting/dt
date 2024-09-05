#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-04-25 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Reflection;
#endregion

namespace Dt.Base
{
    static class ValueCall
    {
        public static async Task<object> GetValue(string p_call)
        {
            // 调用外部方法
            var arr = p_call.TrimEnd(')').Split('(');
            var mi = GetMethod(arr[0]);
            if (arr.Length > 2)
                Throw.Msg($"报表中自定义获取值的方法名 @{p_call} 不符合规范");

            object[] args = null;
            if (arr.Length == 2 && arr[1] != "")
                args = new object[] { arr[1] };

            object result;
            if (typeof(Task).IsAssignableFrom(mi.ReturnType))
            {
                // 异步有返回值
                var task = (Task)mi.Invoke(null, args);
                await task;
                result = task.GetType().GetProperty("Result").GetValue(task);
            }
            else
            {
                // 调用同步方法
                result = mi.Invoke(null, args);
            }
            return result;
        }

        static readonly Dictionary<string, MethodInfo> _methods = new Dictionary<string, MethodInfo>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_method">静态方法名，形如：Def.Icon</param>
        /// <returns></returns>
        static MethodInfo GetMethod(string p_method)
        {
            if (_methods.TryGetValue(p_method, out var mi))
                return mi;

            var arr = p_method.Split('.');
            if (arr.Length != 2)
            {
                Throw.Msg($"报表中自定义获取值的方法名 {p_method} 不符合规范");
            }

            mi = Kit.GetMethodByAlias(typeof(ValueCallAttribute), arr[0], arr[1], BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase);
            if (mi != null)
            {
                _methods[p_method] = mi;
                return mi;
            }

            Throw.Msg("未找到报表中自定义获取值的方法：" + p_method);
            return null;
        }
    }
}
