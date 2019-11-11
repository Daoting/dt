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

namespace Dt.Core
{
    /// <summary>
    /// Table工具类
    /// </summary>
    public static class TableKit
    {
        /// <summary>
        /// Type -> string，大小写敏感
        /// </summary>
        /// <param name="p_type"></param>
        /// <returns></returns>
        public static string GetColTypeAlias(Type p_type)
        {
            if (p_type.IsGenericType)
            {
                if (p_type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    return p_type.GetGenericArguments()[0].Name + "?";
                throw new Exception("无法映射的数据类型:" + p_type.ToString());
            }
            return p_type.Name;
        }

        /// <summary>
        /// string -> Type，大小写敏感
        /// </summary>
        /// <param name="p_name"></param>
        /// <returns></returns>
        public static Type GetColType(string p_name)
        {
            if (p_name.EndsWith('?'))
            {
                Type tp = Type.GetType("System." + p_name.TrimEnd('?'), true, false);
                return typeof(Nullable<>).MakeGenericType(tp);
            }
            return Type.GetType("System." + p_name, true, false);
        }
    }
}
