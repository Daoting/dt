#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Xml;
using Windows.UI;
using Windows.UI.Xaml;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// 为Serializer类提供类型管理
    /// </summary>
    internal static class TypePool
    {
        static Assembly[] assemblys = new Assembly[] { IntrospectionExtensions.GetTypeInfo((Type) typeof(Worksheet)).Assembly, IntrospectionExtensions.GetTypeInfo((Type) typeof(CalcError)).Assembly, IntrospectionExtensions.GetTypeInfo((Type) typeof(int)).Assembly, IntrospectionExtensions.GetTypeInfo((Type) typeof(Uri)).Assembly, IntrospectionExtensions.GetTypeInfo((Type) typeof(CallInfo)).Assembly, IntrospectionExtensions.GetTypeInfo((Type) typeof(XmlException)).Assembly, IntrospectionExtensions.GetTypeInfo((Type) typeof(DependencyObject)).Assembly, IntrospectionExtensions.GetTypeInfo((Type) typeof(Color)).Assembly };
        static Dictionary<string, Type> typeMap = new Dictionary<string, Type>();

        public static void CacheType(string typeName, Type type)
        {
            typeMap[typeName] = type;
        }

        public static Type FindType(string typeName)
        {
            return FindType(typeName, true);
        }

        public static Type FindType(string typeName, bool findInAssemblies)
        {
            if (typeMap.ContainsKey(typeName))
            {
                return typeMap[typeName];
            }
            if (findInAssemblies)
            {
                foreach (Assembly assembly in assemblys)
                {
                    Type type = assembly.GetType(typeName);
                    if (type != null)
                    {
                        typeMap.Add(typeName, type);
                        return type;
                    }
                }
            }
            return null;
        }

        public static Type FindXamlType(string typeName)
        {
            Func<TypeInfo, bool> func = null;
            string lookupName;
            if (!string.IsNullOrWhiteSpace(typeName))
            {
                lookupName = Enumerable.LastOrDefault<string>(typeName.Split(new char[] { '.' }));
                if (string.IsNullOrWhiteSpace(lookupName))
                {
                    return null;
                }
                foreach (Assembly assembly in assemblys)
                {
                    if (func == null)
                    {
                        func = delegate (TypeInfo t) {
                            return (t.Name == lookupName) && t.IsPublic;
                        };
                    }
                    TypeInfo info = Enumerable.FirstOrDefault<TypeInfo>(Enumerable.Where<TypeInfo>(assembly.DefinedTypes, func));
                    if (info != null)
                    {
                        Type type = assembly.GetType(info.FullName);
                        if (type != null)
                        {
                            typeMap[typeName] = type;
                            return type;
                        }
                    }
                }
            }
            return null;
        }

        public static bool IsSupported(Type type)
        {
            foreach (Assembly assembly in assemblys)
            {
                if (IntrospectionExtensions.GetTypeInfo(type).Assembly == assembly)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsSXType(Type type)
        {
            // hdt
            return type.Namespace.StartsWith("Dt.Cells");
        }
    }
}

