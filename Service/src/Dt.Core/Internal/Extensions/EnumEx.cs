#region 文件描述
/**************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2010-08-19 创建
**************************************************************************/
#endregion

#region 引用命名
using System;
using System.Linq;
using System.Reflection;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 枚举扩展类
    /// </summary>
    public static class EnumEx
    {
        /// <summary>
        /// 得到类型的字段数组
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static Array GetValues(this Enum e)
        {
            return GetValues(e.GetType());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T[] GetTypedValues<T>()
        {
            Array values = GetValues(typeof(T));
            T[] localArray = new T[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                localArray[i] = (T)values.GetValue(i);
            }
            return localArray;
        }

        /// <summary>
        /// 根据枚举类型获取字段数据数组
        /// </summary>
        /// <param name="enumType">枚举类型</param>
        /// <returns>字段值数组</returns>
        public static Array GetValues(Type enumType)
        {
            FieldInfo[] fields = (from field in enumType.GetRuntimeFields()
                                  where ((field.Attributes & FieldAttributes.Public) == FieldAttributes.Public || (field.Attributes & FieldAttributes.Static) == FieldAttributes.Static)
                                  select field).ToArray();
            object[] objArray = new object[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                objArray[i] = fields[i].GetValue(null);
            }
            return objArray;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="valueAsString"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool EnumTryParse<TEnum>(string valueAsString, out TEnum value) where TEnum : struct
        {
            return EnumTryParse<TEnum>(valueAsString, false, out value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="valueAsString"></param>
        /// <param name="ignoreCase"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool EnumTryParse<TEnum>(string valueAsString, bool ignoreCase, out TEnum value) where TEnum : struct
        {
            return Enum.TryParse<TEnum>(valueAsString, ignoreCase, out value);
        }
    }
}
