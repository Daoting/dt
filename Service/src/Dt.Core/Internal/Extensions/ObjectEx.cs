#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2010-08-19 创建
**************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 扩展类
    /// </summary>
    public static class ObjectEx
    {
        /// <summary>
        /// 类型转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p_obj"></param>
        /// <returns></returns>
        public static T To<T>(this object p_obj)
        {
            Type tgtType = typeof(T);

            // null时
            if (p_obj == null)
            {
                // 字符串返回Empty！！！
                if (tgtType == typeof(string))
                    return (T)(object)string.Empty;

                // 值类型，可空类型Nullable<>也是值类型
                if (tgtType.IsValueType)
                    return (T)Activator.CreateInstance(tgtType);

                return default(T);
            }

            // 若指定类型和当前类型匹配
            if (tgtType.IsAssignableFrom(p_obj.GetType()))
                return (T)p_obj;

            // 枚举类型
            if (tgtType.IsEnum)
            {
                if (p_obj is string str)
                    return (str == string.Empty) ? (T)Enum.ToObject(tgtType, 0) : (T)Enum.Parse(tgtType, str);
                return (T)Enum.ToObject(tgtType, p_obj);
            }

            // 可空类型
            if (tgtType.IsGenericType && tgtType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                Type tp = tgtType.GetGenericArguments()[0];
                // 参数类型不同时先转换
                if (tp != p_obj.GetType())
                    return (T)Convert.ChangeType(p_obj, tp);
                return (T)p_obj;
            }

            // bool特殊处理
            if (tgtType == typeof(bool))
            {
                string str = p_obj.ToString().ToLower();
                return (T)(object)(str == "1" || str == "true");
            }

            // 执行转换
            if (p_obj is IConvertible)
                return (T)Convert.ChangeType(p_obj, tgtType);

            throw new Exception($"无法将【{p_obj}】转换到【{typeof(T).FullName}】类型！");
        }
    }
}
