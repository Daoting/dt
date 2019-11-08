#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
using System.Xml;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// Table工具类
    /// </summary>
    public static class TableKit
    {
        static XmlReaderSettings _readerSettings;

        /// <summary>
        /// XmlReader默认设置
        /// </summary>
        public static XmlReaderSettings ReaderSettings
        {
            get
            {
                if (_readerSettings == null)
                    _readerSettings = new XmlReaderSettings() { IgnoreWhitespace = true, IgnoreComments = true, IgnoreProcessingInstructions = true };
                return _readerSettings;
            }
        }

        /// <summary>
        /// Type -> string
        /// </summary>
        /// <param name="p_type"></param>
        /// <returns></returns>
        public static string GetAlias(Type p_type)
        {
            if (p_type == typeof(decimal)
                || p_type == typeof(double)
                || p_type == typeof(float))
                return "double";

            if (p_type == typeof(DateTime))
                return "date";

            if (p_type == typeof(int)
                || p_type == typeof(byte)
                || p_type == typeof(sbyte)
                || p_type == typeof(uint)
                || p_type == typeof(long)
                || p_type == typeof(ulong)
                || p_type == typeof(short)
                || p_type == typeof(ushort))
                return "int";

            if (p_type == typeof(byte[]))
                return "blob";

            if (p_type == typeof(string) || p_type == typeof(char))
                return "string";
            throw new Exception("无法映射的数据类型:" + p_type.ToString());
        }

        /// <summary>
        /// string -> Type
        /// </summary>
        /// <param name="p_name"></param>
        /// <returns></returns>
        public static Type GetType(string p_name)
        {
            switch (p_name)
            {
                case "string":
                    return typeof(string);

                case "double":
                    return typeof(double);

                case "int":
                    return typeof(int);

                case "bool":
                    return typeof(bool);

                case "long":
                    return typeof(long);

                case "DateTime":
                    return typeof(DateTime);

                case "byte[]":
                    return typeof(byte[]);

                default:
                    return typeof(string);
            }
        }

        /// <summary>
        /// 抛出含位置的异常信息
        /// </summary>
        /// <param name="p_msg">异常信息内容</param>
        /// <param name="p_memberName">发生异常的方法名称</param>
        /// <param name="p_filePath">源文件路径</param>
        /// <param name="p_lineNumber">行号</param>
        public static void Throw(string p_msg,
            [CallerMemberName] string p_memberName = "",
            [CallerFilePath] string p_filePath = "",
            [CallerLineNumber] int p_lineNumber = 0)
        {
            throw new FriendlyException($"位置：{p_memberName}\r\n➡{p_filePath}\r\n➡{p_lineNumber}行\r\n{p_msg}");
        }
    }
}
