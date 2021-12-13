#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.Data
{
    internal static class TypeExternsion
    {
        public static Type GetInterface(this Type This, string name, bool ignoreCase)
        {
            Type searched = null;
            EnumerateTypeWithAllBaseType(This, delegate(TypeInfo type)
            {
                foreach (Type type2 in type.ImplementedInterfaces)
                {
                    if (type2.Name.Equals(name, ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
                    {
                        searched = type2;
                        return false;
                    }
                }
                return true;
            });
            return searched;
        }

        public static bool IsInstanceOfType(this object p_obj, Type p_type)
        {
            return (!object.ReferenceEquals(p_obj, null)
                && !object.ReferenceEquals(p_type, null)
                && p_obj.GetType().GetTypeInfo().IsAssignableFrom(p_type.GetTypeInfo()));
        }

        internal static void EnumerateTypeWithAllBaseType(Type current, Func<TypeInfo, bool> action)
        {
            for (TypeInfo info = IntrospectionExtensions.GetTypeInfo(current);
                info != null;
                info = (info.BaseType != null) ? IntrospectionExtensions.GetTypeInfo(info.BaseType) : null)
            {
                if (!action(info))
                {
                    return;
                }
            }
        }
    }
}

