#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Xls.Utils
{
    internal static class TypeExtension
    {
        internal static readonly TypeCode[] KnownCodes = new TypeCode[] { 
            TypeCode.Boolean, TypeCode.Char, TypeCode.SByte, TypeCode.Byte, TypeCode.Int16, TypeCode.UInt16, TypeCode.Int32, TypeCode.UInt32, TypeCode.Int64, TypeCode.UInt64, TypeCode.Single, TypeCode.Double, TypeCode.Decimal, TypeCode.String, TypeCode.DateTime, TypeCode.TimeSpan, 
            TypeCode.Guid, TypeCode.Uri, TypeCode.ByteArray, TypeCode.Type
         };
        internal static readonly Type[] KnownTypes = new Type[] { 
            typeof(bool), typeof(char), typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(decimal), typeof(string), typeof(DateTime), typeof(TimeSpan), 
            typeof(Guid), typeof(Uri), typeof(byte[]), typeof(Type)
         };
    }
}

