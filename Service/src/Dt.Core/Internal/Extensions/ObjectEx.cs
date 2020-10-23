#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2010-08-19 创建
**************************************************************************/
#endregion

#region 引用命名
using System;
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
            if (p_obj == null)
                return default(T);

            if (p_obj is T t)
                return t;

            if (p_obj is IConvertible)
                return (T)Convert.ChangeType(p_obj, typeof(T));

            throw new Exception($"无法将【{p_obj}】转换到【{typeof(T).FullName}】类型！");
        }
    }
}
