#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-04-18 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Data;
#endregion

namespace Dts.Core
{
    /// <summary>
    /// DbType Type value DbTypeName TypeName之间的转换
    /// </summary>
    public static class DbTypeConverter
    {
        /// <summary>
        /// value -> DbType
        /// </summary>
        /// <param name="p_value"></param>
        /// <returns></returns>
        public static DbType GetDbTypeByValue(object p_value)
        {
            if (p_value == null)
                return DbType.String;

            Type type = p_value.GetType();
            if (type == typeof(string) || type == typeof(char))
                return DbType.String;

            if (type == typeof(decimal)
                || type == typeof(double)
                || type == typeof(float))
                return DbType.Double;

            if (type == typeof(DateTime))
                return DbType.DateTime;

            if (type == typeof(int)
                || type == typeof(byte)
                || type == typeof(sbyte)
                || type == typeof(uint)
                || type == typeof(long)
                || type == typeof(ulong)
                || type == typeof(short)
                || type == typeof(ushort))
                return DbType.Int32;

            if (type == typeof(byte[]))
                return DbType.Binary;
            return DbType.String;
        }

        /// <summary>
        /// DbType -> Type
        /// </summary>
        /// <param name="p_dbType"></param>
        /// <returns></returns>
        public static Type GetTypeByDbType(DbType p_dbType)
        {
            switch (p_dbType)
            {
                case DbType.AnsiStringFixedLength:
                case DbType.AnsiString:
                case DbType.String:
                case DbType.StringFixedLength:
                    return typeof(string);

                case DbType.Int32:
                    return typeof(int);

                case DbType.Decimal:
                case DbType.Double:
                    return typeof(double);

                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTimeOffset:
                    return typeof(DateTime);

                case DbType.Binary:
                    return typeof(byte[]);
            }
            return typeof(string);
        }

        /// <summary>
        /// DbTypeName -> Type
        /// </summary>
        /// <param name="p_dbTypeName"></param>
        /// <returns></returns>
        public static Type GetTypeByDbTypeName(string p_dbTypeName)
        {
            switch (p_dbTypeName.ToLower())
            {
                case "char":
                case "varchar":
                case "text":
                case "tinytext":
                case "mediumtext":
                case "longtext":
                case "enum":
                case "set":
                    return typeof(string);

                case "tinyint":
                    return typeof(bool);

                case "int":
                case "integer":
                case "mediumint":
                case "smallint":
                case "year":
                    return typeof(int);

                case "double":
                case "numeric":
                case "float":
                case "decimal":
                case "real":
                    return typeof(double);

                case "bigint":
                    return typeof(long);

                case "date":
                case "datetime":
                case "timestamp":
                case "time":
                    return typeof(DateTime);

                case "blob":
                case "tinyblob":
                case "mediumblob":
                case "longblob":
                case "bit":
                case "binary":
                case "varbinary":
                    return typeof(byte[]);

                default:
                    return typeof(string);
            }
        }

        /// <summary>
        /// DbTypeName -> TypeName
        /// </summary>
        /// <param name="p_dbTypeName"></param>
        /// <returns></returns>
        public static string GetTypeNameByDbTypeName(string p_dbTypeName)
        {
            switch (p_dbTypeName.ToLower())
            {
                case "char":
                case "varchar":
                case "text":
                case "tinytext":
                case "mediumtext":
                case "longtext":
                case "enum":
                case "set":
                    return "string";

                case "tinyint":
                    return "bool";

                case "int":
                case "integer":
                case "mediumint":
                case "smallint":
                case "year":
                    return "int";

                case "double":
                case "numeric":
                case "float":
                case "decimal":
                case "real":
                    return "double";

                case "bigint":
                    return "long";

                case "date":
                case "datetime":
                case "timestamp":
                case "time":
                    return "date";

                case "blob":
                case "tinyblob":
                case "mediumblob":
                case "longblob":
                case "bit":
                case "binary":
                case "varbinary":
                    return "blob";

                default:
                    return "string";
            }
        }
    }
}
