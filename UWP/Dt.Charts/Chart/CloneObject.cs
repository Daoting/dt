#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Charts
{
    internal static class CloneObject
    {
        static TT CloneCreate<TT>(TT source)
        {
            try
            {
                Array array = source as Array;
                if (array == null)
                {
                    return (TT) Activator.CreateInstance(source.GetType());
                }
                if (array.Rank == 1)
                {
                    return (TT)Activator.CreateInstance(source.GetType().GetElementType(), new int[] { array.GetLength(0) });
                }
                if (array.Rank == 2)
                {
                    return (TT)Activator.CreateInstance(source.GetType().GetElementType(), new int[] { array.GetLength(0), array.GetLength(1) });
                }
            }
            catch
            {
                // hdt
            }
            return default(TT);
        }

        static void CloneDependencyProperty(DependencyObject sourceObject, DependencyObject cloneObject, FieldInfo field)
        {
            try
            {
                if ((field.Name != "NameProperty") || !(sourceObject is FrameworkElement))
                {
                    DependencyProperty dp = field.GetValue(sourceObject) as DependencyProperty;
                    if (dp != null)
                    {
                        object source = null;
                        try
                        {
                            source = sourceObject.GetValue(dp);
                        }
                        catch (Exception)
                        {
                        }
                        if (source != null)
                        {
                            bool flag = true;
                            if (field.Name == "DataContextProperty")
                            {
                                flag = false;
                            }
                            object obj3 = flag ? CloneRecursive<object>(source) : source;
                            cloneObject.SetValue(dp, obj3);
                        }
                    }
                }
            }
            catch
            {
                // hdt
            }
        }

        static void CloneList(IList sourceList, IList cloneList)
        {
            try
            {
                IEnumerator enumerator = sourceList.GetEnumerator();
                Array array = sourceList as Array;
                Array array2 = cloneList as Array;
                int num = ((array != null) && (array.Rank > 0)) ? array.GetLowerBound(0) : 0;
                int lowerBound = ((array != null) && (array.Rank > 1)) ? array.GetLowerBound(1) : 0;
                while (enumerator.MoveNext())
                {
                    object current = enumerator.Current;
                    object obj3 = CloneRecursive<object>(current);
                    if (array == null)
                    {
                        cloneList.Add(obj3);
                    }
                    else
                    {
                        if (array.Rank == 1)
                        {
                            array2.SetValue(obj3, new int[] { num++ });
                            continue;
                        }
                        if (array.Rank == 2)
                        {
                            array2.SetValue(obj3, new int[] { num, lowerBound });
                            if (++lowerBound > array.GetUpperBound(1))
                            {
                                lowerBound = array.GetLowerBound(1);
                                if (++num > array.GetUpperBound(0))
                                {
                                    num = array.GetLowerBound(0);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                // hdt
            }
        }

        static void CloneProperties(object source, object clone)
        {
            if (source is DependencyObject)
            {
                DependencyObject sourceObject = source as DependencyObject;
                DependencyObject cloneObject = clone as DependencyObject;
                foreach (FieldInfo info in GetFields(source.GetType()))
                {
                    CloneDependencyProperty(sourceObject, cloneObject, info);
                }
            }
            foreach (PropertyInfo info2 in GetProperties(source.GetType()))
            {
                CloneProperty(source, clone, info2);
            }
        }

        static void CloneProperty(object source, object clone, PropertyInfo property)
        {
            try
            {
                if (((((property.CanRead && property.CanWrite) && (property.GetIndexParameters().Length == 0)) && ((property.Name != "Name") || !(source is FrameworkElement))) && (((property.Name != "InputScope") || !(source is TextBox)) && ((property.Name != "Watermark") || !(source is TextBox)))) && (((property.Name != "Source") || !(source is ResourceDictionary)) && ((property.Name != "TargetType") || !(source is ControlTemplate))))
                {
                    bool flag = GetMethod(source.GetType(), "put_" + property.Name, property.PropertyType) != null;
                    bool flag2 = GetInterface(property.PropertyType, "IList") != null;
                    if (flag || flag2)
                    {
                        object obj2 = property.GetValue(source, null);
                        if (obj2 != null)
                        {
                            if (!flag && flag2)
                            {
                                IList cloneList = property.GetValue(clone, null) as IList;
                                if (cloneList != null)
                                {
                                    CloneList(obj2 as IList, cloneList);
                                }
                            }
                            else
                            {
                                bool flag3 = true;
                                if ((source is FrameworkElement) && (property.Name == "DataContext"))
                                {
                                    flag3 = false;
                                }
                                object obj3 = flag3 ? CloneRecursive<object>(obj2) : obj2;
                                property.SetValue(clone, obj3, null);
                            }
                        }
                    }
                }
            }
            catch
            {
                // hdt
            }
        }

        static TT CloneRecursive<TT>(TT source)
        {
            if ((source == null) || IsValueType(source.GetType()))
            {
                return source;
            }
            if (((source is string) || (source is Type)) || ((source is Uri) || (source is DependencyProperty)))
            {
                return source;
            }
            TT clone = CloneCreate<TT>(source);
            if (clone == null)
            {
                return source;
            }
            if (source is IList)
            {
                CloneList(source as IList, clone as IList);
            }
            CloneProperties(source, clone);
            return clone;
        }

        internal static TT DeepClone<TT>(this TT source)
        {
            TT local = CloneRecursive<TT>(source);
            if (local is FrameworkElement)
            {
                FrameworkElement element = local as FrameworkElement;
                element.Arrange(new Rect(0.0, 0.0, element.ActualWidth, element.ActualHeight));
            }
            return local;
        }

        static FieldInfo[] GetFields(Type type)
        {
            return Enumerable.ToArray<FieldInfo>(RuntimeReflectionExtensions.GetRuntimeFields(type));
        }

        static Type GetInterface(Type type, string name)
        {
            foreach (Type type2 in IntrospectionExtensions.GetTypeInfo(type).ImplementedInterfaces)
            {
                if (type2.Name == name)
                {
                    return type2;
                }
            }
            return null;
        }

        static MethodInfo GetMethod(Type type, string name, Type parameterType)
        {
            return RuntimeReflectionExtensions.GetRuntimeMethod(type, name, new Type[] { parameterType });
        }

        static PropertyInfo[] GetProperties(Type type)
        {
            return Enumerable.ToArray<PropertyInfo>(RuntimeReflectionExtensions.GetRuntimeProperties(type));
        }

        static bool IsValueType(Type type)
        {
            return IntrospectionExtensions.GetTypeInfo(type).IsValueType;
        }

        static string SimpleType(Type type)
        {
            string str = type.ToString();
            int startIndex = str.LastIndexOf('[');
            if (startIndex < 0)
            {
                return str.Substring(str.LastIndexOf('.') + 1);
            }
            string str2 = str.Substring(startIndex);
            str2 = str2.Substring(str2.LastIndexOf('.') + 1);
            str = str.Substring(0, startIndex);
            return (str.Substring(str.LastIndexOf('.') + 1) + ((char) '[') + str2);
        }
    }
}

