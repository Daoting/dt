#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine;
using Dt.CalcEngine.Expressions;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// The Serializer class.
    /// </summary>
    public static class Serializer
    {
        /// <summary>
        /// Creates the type.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="assembly">The assembly.</param>
        /// <returns></returns>
        internal static Type CreateType(string typeName, string assembly)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return null;
            }
            if (typeName.IndexOf('.') == -1)
            {
                typeName = string.Format("{0}.{1}", (object[])new object[] { typeof(Worksheet).Namespace, typeName });
            }
            if (!string.IsNullOrEmpty(assembly))
            {
                try
                {
                    Type type = TypePool.FindType(typeName, false);
                    if (type == null)
                    {
                        Assembly assembly2 = Assembly.Load(new AssemblyName(assembly));
                        if (assembly2 != null)
                        {
                            type = assembly2.GetType(typeName);
                            if (type != null)
                            {
                                TypePool.CacheType(typeName, type);
                            }
                        }
                    }
                    else
                    {
                        return type;
                    }
                }
                catch
                {
                }
            }
            try
            {
                return TypePool.FindType(typeName);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Creates the type instance.
        /// </summary>
        /// <param name="type">The type</param>
        /// <param name="source">The source</param>
        /// <returns></returns>
        static object CreateTypeInstance(Type type, string source)
        {
            if (IsSimpleType(type))
            {
                if (type == typeof(string))
                {
                    return source;
                }
                if (type == typeof(double))
                {
                    try
                    {
                        return (double)double.Parse(source, (IFormatProvider)CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        throw new InvalidCastException(string.Format(ResourceStrings.SerializerParseTypeNotMatchError, (object[])new object[] { source, type.ToString() }));
                    }
                }
                if (type == typeof(int))
                {
                    try
                    {
                        return (int)int.Parse(source, (IFormatProvider)CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        throw new InvalidCastException(string.Format(ResourceStrings.SerializerParseTypeNotMatchError, (object[])new object[] { source, type.ToString() }));
                    }
                }
                if (type == typeof(short))
                {
                    try
                    {
                        return (short)short.Parse(source, (IFormatProvider)CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        throw new InvalidCastException(string.Format(ResourceStrings.SerializerParseTypeNotMatchError, (object[])new object[] { source, type.ToString() }));
                    }
                }
                if (type == typeof(long))
                {
                    try
                    {
                        return (long)long.Parse(source, (IFormatProvider)CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        throw new InvalidCastException(string.Format(ResourceStrings.SerializerParseTypeNotMatchError, (object[])new object[] { source, type.ToString() }));
                    }
                }
                if (type == typeof(bool))
                {
                    try
                    {
                        return (bool)bool.Parse(source);
                    }
                    catch
                    {
                        throw new InvalidCastException(string.Format(ResourceStrings.SerializerParseTypeNotMatchError, (object[])new object[] { source, type.ToString() }));
                    }
                }
                if (type == typeof(float))
                {
                    try
                    {
                        return (float)float.Parse(source, (IFormatProvider)CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        throw new InvalidCastException(string.Format(ResourceStrings.SerializerParseTypeNotMatchError, (object[])new object[] { source, type.ToString() }));
                    }
                }
                if (type == typeof(char))
                {
                    char result = '\0';
                    if (!char.TryParse(source, out result))
                    {
                        throw new InvalidCastException(string.Format(ResourceStrings.SerializerParseTypeNotMatchError, (object[])new object[] { source, type.ToString() }));
                    }
                    return (char)result;
                }
                if (type == typeof(byte))
                {
                    try
                    {
                        return (byte)byte.Parse(source, (IFormatProvider)CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        throw new InvalidCastException(string.Format(ResourceStrings.SerializerParseTypeNotMatchError, (object[])new object[] { source, type.ToString() }));
                    }
                }
                if (type == typeof(sbyte))
                {
                    try
                    {
                        return (sbyte)sbyte.Parse(source, (IFormatProvider)CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        throw new InvalidCastException(string.Format(ResourceStrings.SerializerParseTypeNotMatchError, (object[])new object[] { source, type.ToString() }));
                    }
                }
                if (type == typeof(ushort))
                {
                    try
                    {
                        return (ushort)ushort.Parse(source, (IFormatProvider)CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        throw new InvalidCastException(string.Format(ResourceStrings.SerializerParseTypeNotMatchError, (object[])new object[] { source, type.ToString() }));
                    }
                }
                if (type == typeof(uint))
                {
                    try
                    {
                        return (uint)uint.Parse(source, (IFormatProvider)CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        throw new InvalidCastException(string.Format(ResourceStrings.SerializerParseTypeNotMatchError, (object[])new object[] { source, type.ToString() }));
                    }
                }
                if (type == typeof(ulong))
                {
                    try
                    {
                        return (ulong)ulong.Parse(source, (IFormatProvider)CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        throw new InvalidCastException(string.Format(ResourceStrings.SerializerParseTypeNotMatchError, (object[])new object[] { source, type.ToString() }));
                    }
                }
                if (type == typeof(DateTime))
                {
                    try
                    {
                        return DateTime.Parse(source, (IFormatProvider)CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        throw new InvalidCastException(string.Format(ResourceStrings.SerializerParseTypeNotMatchError, (object[])new object[] { source, type.ToString() }));
                    }
                }
                if (type == typeof(TimeSpan))
                {
                    try
                    {
                        return TimeSpan.Parse(source, (IFormatProvider)CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        throw new InvalidCastException(string.Format(ResourceStrings.SerializerParseTypeNotMatchError, (object[])new object[] { source, type.ToString() }));
                    }
                }
                if (type != typeof(decimal))
                {
                    goto Label_04CB;
                }
                try
                {
                    return decimal.Parse(source, (IFormatProvider)CultureInfo.InvariantCulture);
                }
                catch
                {
                    throw new InvalidCastException(string.Format(ResourceStrings.SerializerParseTypeNotMatchError, (object[])new object[] { source, type.ToString() }));
                }
            }
            if (IntrospectionExtensions.GetTypeInfo(type).IsEnum)
            {
                try
                {
                    return Enum.Parse(type, source, true);
                }
                catch
                {
                    throw new InvalidCastException(string.Format(ResourceStrings.SerializerParseTypeNotMatchError, (object[])new object[] { source, type.ToString() }));
                }
            }
            Label_04CB:
            throw new InvalidCastException(ResourceStrings.SerializerInvalidCastError);
        }

        internal static void DeserializeArray<T>(SparseArray<T> array, Type fixedType, XmlReader reader) where T : class
        {
            InitReader(reader);
            int? nullable = ReadAttributeInt("length", reader);
            if (nullable.HasValue && (nullable.Value != array.Length))
            {
                array.Length = nullable.Value;
            }
            while (reader.Read())
            {
                string str;
                if (((reader.NodeType == ((XmlNodeType)((int)XmlNodeType.Element))) && ((str = reader.Name) != null)) && (str == "Item"))
                {
                    int? nullable2 = ReadAttributeInt("index", reader);
                    if (!nullable2.HasValue)
                    {
                        throw new XmlSerializationException(ResourceStrings.SerializeDeserializerArrayError);
                    }
                    XmlReader reader2 = ExtractNode(reader);
                    object obj2 = DeserializeObj(fixedType, reader2);
                    if (obj2 is IsEmptySupport)
                    {
                        if (!((IsEmptySupport)obj2).IsEmpty)
                        {
                            array[nullable2.Value] = obj2 as T;
                        }
                    }
                    else
                    {
                        array[nullable2.Value] = obj2 as T;
                    }
                    reader2.Close();
                }
            }
        }

        /// <summary>
        /// Deserialize the array.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        internal static bool[,] DeserializeArrayBool2(XmlReader reader)
        {
            using (XmlReader reader2 = ExtractNode(reader))
            {
                XElement element = XElement.Load(reader2);
                XAttribute attribute = element.Attribute(XName.Get("rank"));
                XAttribute attribute2 = element.Attribute(XName.Get("length"));
                string str = element.Value;
                if ((attribute != null) && (attribute2 != null))
                {
                    try
                    {
                        int? nullable2 = Dt.Cells.Data.FormatConverter.TryInt(attribute.Value, false);
                        int num = nullable2.HasValue ? nullable2.GetValueOrDefault() : -1;
                        int? nullable3 = Dt.Cells.Data.FormatConverter.TryInt(attribute2.Value, false);
                        int num2 = nullable3.HasValue ? nullable3.GetValueOrDefault() : -1;
                        if ((num < 1) || (num2 < 0))
                        {
                            throw new FormatException(ResourceStrings.ArrayFormatIsIllegal);
                        }
                        string[] strArray = str.Split(new char[] { Dt.Cells.Data.DefaultTokens.Comma });
                        if (strArray.Length != (num * num2))
                        {
                            throw new FormatException(ResourceStrings.SerializerDeserializerIllegalArrayFormat);
                        }
                        bool[,] flagArray = new bool[num2, num];
                        int index = 0;
                        for (int i = 0; i < num; i++)
                        {
                            for (int j = 0; j < num2; j++)
                            {
                                int? nullable = Dt.Cells.Data.FormatConverter.TryInt(strArray[index], false);
                                if (!nullable.HasValue)
                                {
                                    break;
                                }
                                bool? nullable4 = Dt.Cells.Data.FormatConverter.TryBool((int)nullable.Value, false);
                                flagArray[j, i] = nullable4.HasValue ? nullable4.GetValueOrDefault() : false;
                                index++;
                            }
                        }
                        return flagArray;
                    }
                    catch
                    {
                    }
                }
            }
            return null;
        }

        internal static void DeserializeCalcStorage(List<List<object>> stores, ICalcEvaluator evaluator, XmlReader reader)
        {
            XmlReader reader2 = ExtractNode(reader);
            InitReader(reader2);
            while (reader2.Read())
            {
                string str2;
                if (((reader2.NodeType == XmlNodeType.Element) && ((str2 = reader2.Name) != null)) && (str2 == "Item"))
                {
                    List<object> list;
                    int num = ReadAttributeInt("r", 0, reader);
                    int num2 = ReadAttributeInt("c", 0, reader);
                    int num3 = ReadAttributeInt("rc", -1, reader);
                    int num4 = ReadAttributeInt("cc", -1, reader);
                    string str = ReadAttribute("formula", reader);
                    list = new List<object> {
                        (int) num,
                        (int) num2,
                        (int) num3,
                        (int) num4,
                        str
                    };
                }
            }
            reader2.Close();
        }

        internal static object DeserializeCellObj(XmlReader reader, Dictionary<string, string> typeMap, Dictionary<string, string> assemblyMap)
        {
            InitReader(reader);
            string typeName = null;
            if (typeMap == null)
            {
                typeName = ReadAttribute("class", reader);
            }
            else
            {
                string str2 = ReadAttribute("type", reader);
                if (str2 == null)
                {
                    return null;
                }
                if (!typeMap.ContainsKey(str2))
                {
                    throw new NotSupportedException(ResourceStrings.SerializeDeserializerCellError);
                }
                typeName = typeMap[str2];
            }
            string assembly = null;
            assemblyMap.TryGetValue(typeName, out assembly);
            string str4 = ReadAttribute("encoded", reader);
            reader.MoveToElement();
            if (str4 != "binary")
            {
                if (str4 == "xml")
                {
                    reader.Read();
                    XmlSerializer serializer = new XmlSerializer(CreateType(typeName, assembly));
                    return serializer.Deserialize(reader);
                }
                if (typeName != null)
                {
                    Type type = CreateType(typeName, assembly);
                    if (type != null)
                    {
                        return DeserializeObj(type, reader);
                    }
                }
            }
            return null;
        }

        internal static void DeserializeConditionalFormats(ConditionalFormat formats, XmlReader reader)
        {
            XmlReader reader2 = ExtractNode(reader);
            InitReader(reader2);
            while (reader2.Read())
            {
                string str;
                if (((reader2.NodeType == ((XmlNodeType)((int)XmlNodeType.Element))) && ((str = reader2.Name) != null)) && (str == "Item"))
                {
                    FormattingRuleBase rule = DeserializeObj(null, reader) as FormattingRuleBase;
                    if (rule != null)
                    {
                        formats.AddRule(rule, false);
                    }
                }
            }
            reader2.Close();
        }

        /// <summary>
        /// Deserialize the date time format information.
        /// </summary>
        /// <param name="reader">The reader.</param>
        internal static DateTimeFormatInfo DeserializeDateTimeFormatInfo(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (reader.NodeType == XmlNodeType.None)
            {
                reader.Read();
            }
            if (reader.NodeType == XmlNodeType.XmlDeclaration)
            {
                reader.Read();
            }
            string[] strArray = null;
            string[] strArray2 = null;
            string[] strArray3 = null;
            string str = null;
            Calendar calendar = null;
            CalendarWeekRule? nullable = null;
            string[] strArray4 = null;
            DayOfWeek? nullable2 = null;
            string str2 = null;
            string str3 = null;
            string str4 = null;
            string str5 = null;
            string[] strArray5 = null;
            string[] strArray6 = null;
            string str6 = null;
            string str7 = null;
            string[] strArray7 = null;
            string str8 = null;
            string str9 = null;
            DateTimeFormatInfo info = new DateTimeFormatInfo();
            while (reader.Read())
            {
                XmlNodeType nodeType = reader.NodeType;
            }
            info.Calendar = calendar;
            info.AbbreviatedDayNames = strArray;
            info.AbbreviatedMonthGenitiveNames = strArray2;
            info.AbbreviatedMonthNames = strArray3;
            info.AMDesignator = str;
            if (nullable.HasValue)
            {
                info.CalendarWeekRule = nullable.Value;
            }
            info.DayNames = strArray4;
            if (nullable2.HasValue)
            {
                info.FirstDayOfWeek = nullable2.Value;
            }
            info.LongDatePattern = str3;
            info.LongTimePattern = str4;
            info.FullDateTimePattern = str2;
            info.MonthDayPattern = str5;
            info.MonthGenitiveNames = strArray5;
            info.MonthNames = strArray6;
            info.PMDesignator = str6;
            info.ShortDatePattern = str7;
            info.ShortestDayNames = strArray7;
            info.ShortTimePattern = str8;
            info.YearMonthPattern = str9;
            return info;
        }

        internal static object DeserializeEnum(Type type, string str)
        {
            if (IntrospectionExtensions.GetTypeInfo(type).IsEnum)
            {
                try
                {
                    return Enum.Parse(type, str, true);
                }
                catch
                {
                }
            }
            return null;
        }

        internal static void DeserializeGenericList<T>(IList<T> list, XmlReader reader)
        {
            InitReader(reader);
            XmlReader reader2 = ExtractNode(reader);
            while (reader2.Read())
            {
                string str;
                if (((reader.NodeType == ((XmlNodeType)((int)XmlNodeType.Element))) && ((str = reader2.Name) != null)) && (str == "Item"))
                {
                    object obj2 = DeserializeObj(typeof(T), reader2);
                    if (obj2 is T)
                    {
                        list.Add((T)obj2);
                    }
                    else
                    {
                        T local = default(T);
                        list.Add(local);
                    }
                }
            }
        }

        /// <summary>
        /// Deserialize the image.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="imageString">The imgae cache string.</param>
        /// <returns>The image source.</returns>
        internal static ImageSource DeserializeImage(XmlReader reader, out string imageString)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (reader.NodeType == ((XmlNodeType)((int)XmlNodeType.None)))
            {
                reader.Read();
            }
            if (reader.NodeType == ((XmlNodeType)((int)XmlNodeType.XmlDeclaration)))
            {
                reader.Read();
            }
            try
            {
                string tpString = null;
                BitmapImage image = new BitmapImage();
                tpString = ReadAttribute("value", reader);
                byte[] buffer = Convert.FromBase64String(tpString);
                Stream imageStream = new MemoryStream();
                imageStream.Write(buffer, 0, buffer.Length);
                imageStream.Seek(0L, SeekOrigin.Begin);
                Utility.InitImageSource(image, imageStream);
                imageString = tpString;
                return image;
            }
            catch (Exception)
            {
                imageString = null;
                return null;
            }
        }

        internal static SparseArray<object> DeserializeIndex(string code)
        {
            SparseArray<object> array = new SparseArray<object>();
            Convert.FromBase64String(code);
            byte[] buffer = null;
            int num = (((buffer[0] << 0x18) | (buffer[1] << 0x10)) | (buffer[2] << 8)) | buffer[3];
            array.Length = num;
            for (int i = 0; i < (buffer.Length / 8); i++)
            {
                int index = 4 + (i * 8);
                int num4 = (((buffer[index] << 0x18) | (buffer[index + 1] << 0x10)) | (buffer[index + 2] << 8)) | buffer[index + 3];
                int num5 = (((buffer[index + 4] << 0x18) | (buffer[index + 5] << 0x10)) | (buffer[index + 6] << 8)) | buffer[index + 7];
                array[num4] = (int)num5;
            }
            return array;
        }

        internal static SparseArray<object> DeserializeIndex(XmlReader reader)
        {
            SparseArray<object> array = new SparseArray<object>();
            using (XmlReader reader2 = ExtractNode(reader))
            {
                XElement element = XElement.Load(reader2);
                if (element != null)
                {
                    return DeserializeIndex(element.Value);
                }
            }
            return array;
        }

        internal static void DeserializeList(IList list, XmlReader reader)
        {
            InitReader(reader);
            XmlReader reader2 = ExtractNode(reader);
            while (reader2.Read())
            {
                string str;
                if (((reader2.NodeType == ((XmlNodeType)((int)XmlNodeType.Element))) && ((str = reader2.Name) != null)) && (str == "Item"))
                {
                    object obj2 = DeserializeObj(null, reader2);
                    list.Add(obj2);
                }
            }
        }

        internal static void DeserializeMatrix<T>(DataMatrix<T> matrix, Type fixedType, XmlReader reader) where T : class
        {
            if (matrix == null)
            {
                throw new ArgumentNullException("matrix");
            }
            InitReader(reader);
            XElement element = XElement.Load(ExtractNode(reader));
            if (element != null)
            {
                Dictionary<string, string> typeMap = new Dictionary<string, string>();
                Dictionary<string, string> assemblyMap = new Dictionary<string, string>();
                XElement element2 = element.Element(XName.Get("Types"));
                if (element2 != null)
                {
                    foreach (XElement element3 in element2.Elements(XName.Get("Type")))
                    {
                        XAttribute attribute = element3.Attribute(XName.Get("id"));
                        XAttribute attribute2 = element3.Attribute(XName.Get("name"));
                        element3.Attribute(XName.Get("assembly"));
                        if (((attribute != null) && (attribute2 != null)) && (!string.IsNullOrEmpty(attribute.Value) && !string.IsNullOrEmpty(attribute2.Value)))
                        {
                            string str = attribute2.Value.Replace("GrapeCity.Windows.SpreadSheet.Data", "Dt.Cells.Data");
                            typeMap.Add(attribute.Value, str);
                        }
                    }
                }
                foreach (XElement element4 in element.Elements(XName.Get("C")))
                {
                    XAttribute attribute3 = element4.Attribute(XName.Get("pos"));
                    if ((attribute3 == null) || string.IsNullOrEmpty(attribute3.Value))
                    {
                        throw new XmlSerializationException(ResourceStrings.SerializerDeserializeMatrixError);
                    }
                    string[] strArray = attribute3.Value.Split(new char[] { ',' });
                    if (strArray.Length != 2)
                    {
                        throw new XmlSerializationException(ResourceStrings.SerializerDeserializeMatrixError);
                    }
                    int? nullable = Dt.Cells.Data.FormatConverter.TryInt(strArray[0], false);
                    int? nullable2 = Dt.Cells.Data.FormatConverter.TryInt(strArray[1], false);
                    if (nullable.HasValue && nullable2.HasValue)
                    {
                        object obj2;
                        XmlReader reader3 = element4.CreateReader();
                        if (fixedType == null)
                        {
                            obj2 = DeserializeCellObj(reader3, typeMap, assemblyMap);
                            matrix.SetValue(nullable.Value, nullable2.Value, obj2 as T);
                        }
                        else
                        {
                            obj2 = DeserializeObj(fixedType, reader3);
                            matrix.SetValue(nullable.Value, nullable2.Value, obj2 as T);
                        }
                        reader3.Close();
                    }
                }
            }
        }

        internal static void DeserializeNameInfos(XmlReader reader, List<List<object>> list)
        {
            XmlReader reader2 = ExtractNode(reader);
            InitReader(reader2);
            while (reader2.Read())
            {
                string str3;
                if (((reader2.NodeType == XmlNodeType.Element) && ((str3 = reader2.Name) != null)) && (str3 == "Item"))
                {
                    string str = ReadAttribute("name", reader);
                    string str2 = ReadAttribute("formula", reader);
                    if (!string.IsNullOrEmpty(str))
                    {
                        List<object> list2;
                        int num = ReadAttributeInt("r", 0, reader);
                        int num2 = ReadAttributeInt("c", 0, reader);
                        list2 = new List<object> {
                            str,
                            str2,
                            (int) num,
                            (int) num2
                        };
                    }
                }
            }
            reader2.Close();
        }

        /// <summary>
        /// Deserialize the number format information.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        internal static NumberFormatInfo DeserializeNumberFormatInfo(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (reader.NodeType == XmlNodeType.None)
            {
                reader.Read();
            }
            if (reader.NodeType == XmlNodeType.XmlDeclaration)
            {
                reader.Read();
            }
            NumberFormatInfo info = new NumberFormatInfo();
            while (reader.Read())
            {
                XmlNodeType nodeType = reader.NodeType;
            }
            return info;
        }

        /// <summary>
        /// Deserializes the current node of the XmlReader as an object of the specified type.
        /// </summary>
        /// <param name="type">The deserialized type. </param>
        /// <param name="reader">The XmlReader with which to access the data.</param>
        /// <returns>The deserialized object.</returns>
        public static object DeserializeObj(Type type, XmlReader reader)
        {
            InitReader(reader);
            if (type == null)
            {
                string str = ReadAttribute("type", reader);
                string str2 = ReadAttribute("assembly", reader);
                type = CreateType(str, str2);
            }
            if ((type != null) && (type.GetInterface("IXmlSerializable", false) != null))
            {
                XmlReader @this = ExtractNode(reader);
                object obj2 = Activator.CreateInstance(type);
                (obj2 as IXmlSerializable).ReadXml(@this);
                @this.Close();
                return obj2;
            }
            if ((type != null) && IsSupportedValueType(type))
            {
                string str3 = ReadAttribute("value", reader);
                return Parse(type, str3);
            }
            if (type == typeof(SparseArray<object>))
            {
                XmlReader reader3 = ExtractNode(reader);
                object obj3 = Activator.CreateInstance(type);
                DeserializeArray<object>(obj3 as SparseArray<object>, null, reader3);
                reader3.Close();
                return obj3;
            }
            if (type == typeof(SparseArray<AxisInfo>))
            {
                XmlReader reader4 = ExtractNode(reader);
                object obj4 = Activator.CreateInstance(type);
                DeserializeArray<AxisInfo>(obj4 as SparseArray<AxisInfo>, typeof(AxisInfo), reader4);
                reader4.Close();
                return obj4;
            }
            if (type == typeof(SparseArray<StyleInfo>))
            {
                XmlReader reader5 = ExtractNode(reader);
                object obj5 = Activator.CreateInstance(type);
                DeserializeArray<StyleInfo>(obj5 as SparseArray<StyleInfo>, typeof(StyleInfo), reader5);
                reader5.Close();
                return obj5;
            }
            if (type == typeof(CellRange))
            {
                string s = ReadAttribute("value", reader);
                CellRange result = null;
                CellRange.TryParse(s, out result);
                return result;
            }
            if (type == typeof(Windows.Foundation.Point))
            {
                double num;
                double num2;
                if (ParseValuePair(ReadAttribute("value", reader), out num, out num2))
                {
                    return new Windows.Foundation.Point(num, num2);
                }
                return new Windows.Foundation.Point();
            }
            if (type == typeof(Windows.Foundation.Size))
            {
                double num3;
                double num4;
                if (ParseValuePair(ReadAttribute("value", reader), out num3, out num4))
                {
                    return new Windows.Foundation.Size(num3, num4);
                }
                return new Windows.Foundation.Size();
            }
            if (type == typeof(GradientStopCollection))
            {
                GradientStopCollection stops = new GradientStopCollection();
                string[] strArray = ReadAttribute("value", reader).Split(new char[] { ',', ' ' });
                try
                {
                    if ((strArray.Length % 2) != 0)
                    {
                        return stops;
                    }
                    for (int i = 0; i < strArray.Length; i += 2)
                    {
                        Windows.UI.Color? nullable = Dt.Cells.Data.ColorHelper.FromStringValue(strArray[i]);
                        double? nullable3 = Dt.Cells.Data.FormatConverter.TryDouble(strArray[i + 1], false);
                        double num6 = nullable3.HasValue ? ((double)nullable3.GetValueOrDefault()) : 0.0;
                        GradientStop stop = new GradientStop();
                        stop.Color = nullable.HasValue ? nullable.Value : Colors.Transparent;
                        stop.Offset = num6;
                        stops.Add(stop);
                    }
                }
                catch
                {
                }
                return stops;
            }
            if ((type != null) && type.GetTypeInfo().IsSubclassOf(typeof(CalcError)))
            {
                return CalcErrors.Parse(ReadAttribute("value", reader), null);
            }
            if (type == typeof(Brush))
            {
                string str9 = ReadAttribute("type", reader);
                string strValue = ReadAttribute("value", reader);
                Brush brush = null;
                if ("ImageBrush.URI".Equals(str9))
                {
                    try
                    {
                        brush = new ImageBrush();
                        (brush as ImageBrush).ImageSource = new BitmapImage(new Uri(strValue, (UriKind)UriKind.RelativeOrAbsolute));
                    }
                    catch
                    {
                        brush = new SolidColorBrush(Colors.Black);
                    }
                    return brush;
                }
                if ("Image".Equals(str9))
                {
                    brush = new ImageBrush();
                    string imageString = null;
                    (brush as ImageBrush).ImageSource = DeserializeImage(reader, out imageString);
                    return brush;
                }
            }
            else if ((type != null) && type.IsArray)
            {
                XmlReader reader6 = ExtractNode(reader);
                List<object> list = new List<object>();
                DeserializeList((IList)list, reader6);
                if (list == null)
                {
                    return null;
                }
                return list.ToArray();
            }
            if ((type != null) && (type.GetInterface("IList", false) != null))
            {
                XmlReader reader7 = ExtractNode(reader);
                object obj6 = Activator.CreateInstance(type);
                DeserializeList(obj6 as IList, reader7);
                reader7.Close();
                return obj6;
            }
            string typeName = ReadAttribute("type", reader);
            string assembly = ReadAttribute("assembly", reader);
            Type type2 = CreateType(typeName, assembly);
            if ((type2 == null) && !string.IsNullOrWhiteSpace(typeName))
            {
                type2 = TypePool.FindXamlType(typeName);
            }
            if (type2 != null)
            {
                object obj7 = Activator.CreateInstance(type2);
                XmlReader reader8 = ExtractNode(reader);
                DeserializePublicProperties(obj7, reader8);
                reader8.Close();
                return obj7;
            }
            return null;
        }

        internal static void DeserializePublicProperties(object obj, XmlReader reader)
        {
            InitReader(reader);
            while (reader.Read())
            {
                if (reader.NodeType == ((XmlNodeType)((int)XmlNodeType.Element)))
                {
                    string name = reader.Name;
                    PropertyInfo info = obj.GetType().GetRuntimeProperty(name);
                    if (info != null)
                    {
                        object obj2 = DeserializeObj(info.PropertyType, reader);
                        info.SetValue(obj, obj2, null);
                    }
                }
            }
        }

        /// <summary>
        /// Deserialize the row filter.
        /// </summary>
        /// <param name="worksheet">The sheet.</param>
        /// <param name="reader">The reader.</param>
        internal static object DeserializeRowFilter(Worksheet worksheet, XmlReader reader)
        {
            return null;
        }

        /// <summary>
        /// Deserializes the object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="reader">The reader.</param>
        public static void DeserializeSerializableObject(object obj, XmlReader reader)
        {
            if (reader.NodeType == ((XmlNodeType)((int)XmlNodeType.None)))
            {
                reader.Read();
            }
            if (reader.NodeType == ((XmlNodeType)((int)XmlNodeType.XmlDeclaration)))
            {
                reader.Read();
            }
            XmlReader @this = ExtractNode(reader);
            IXmlSerializable serializable = obj as IXmlSerializable;
            if (serializable != null)
            {
                serializable.ReadXml(@this);
            }
            @this.Close();
        }

        internal static void DeserializeTables(EricTables tables, XmlReader reader)
        {
            XmlReader reader2 = ExtractNode(reader);
            InitReader(reader2);
            while (reader2.Read())
            {
                string str;
                if (((reader2.NodeType == ((XmlNodeType)((int)XmlNodeType.Element))) && ((str = reader2.Name) != null)) && (str == "Item"))
                {
                    SheetTable table = DeserializeObj(typeof(SheetTable), reader) as SheetTable;
                    if (table != null)
                    {
                        tables.Add(table);
                    }
                    table.LoadData();
                }
            }
            reader2.Close();
        }

        internal static object DeserializeTag(XmlReader reader)
        {
            string typeName = ReadAttribute("type", reader);
            string assembly = ReadAttribute("assembly", reader);
            return DeserializeObj(CreateType(typeName, assembly), reader);
        }

        /// <summary>
        /// Deserialize the URI.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>The URI</returns>
        internal static Uri DeserializeUri(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (reader.NodeType == ((XmlNodeType)((int)XmlNodeType.None)))
            {
                reader.Read();
            }
            if (reader.NodeType == ((XmlNodeType)((int)XmlNodeType.XmlDeclaration)))
            {
                reader.Read();
            }
            using (XmlReader reader2 = ExtractNode(reader))
            {
                XElement element2 = XElement.Load(reader2).Element("Uri");
                if (element2 != null)
                {
                    try
                    {
                        return new Uri(element2.Value, (UriKind)UriKind.RelativeOrAbsolute);
                    }
                    catch
                    {
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Extracts the current node in the specified node reader into
        /// a new node reader and advances that reader to the next node. 
        /// </summary>
        /// <param name="reader">XmlNodeReader with which to access the data.</param>
        public static XmlReader ExtractNode(XmlReader reader)
        {
            reader.MoveToElement();
            XmlReader reader2 = reader.ReadSubtree();
            reader2.Read();
            return reader2;
        }

        internal static ClassType? FindStaticDefinationStruct<ClassType>(Type typeCollection, string name) where ClassType : struct
        {
            if ((typeCollection != null) && !string.IsNullOrEmpty(name))
            {
                PropertyInfo property = typeCollection.GetRuntimeProperty(name);
                if (property != null)
                {
                    return new ClassType?((ClassType)property.GetValue(null, null));
                }
            }
            return null;
        }

        internal static string Format(object obj)
        {
            if (obj == null)
            {
                return "(null)";
            }
            if (!IsSupportedValueType(obj.GetType()))
            {
                throw new NotSupportedException(string.Format(ResourceStrings.SerializerNotSupportError, (object[])new object[] { obj.GetType() }));
            }
            if (IsSimpleType(obj.GetType()))
            {
                if ((!(obj is DateTime) && !(obj is TimeSpan)) && !(obj is string))
                {
                }
                return Convert.ToString(obj, (IFormatProvider)CultureInfo.InvariantCulture);
            }
            if (obj is Enum)
            {
                return obj.ToString();
            }
            if (obj is Windows.UI.Color)
            {
                return obj.ToString();
            }
            if (obj is SolidColorBrush)
            {
                return ((SolidColorBrush)obj).ToString();
            }
            if ((obj is Image) || !(obj is Array))
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder();
            foreach (object obj2 in obj as Array)
            {
                if (builder.Length > 0)
                {
                    builder.Append(CultureInfo.InvariantCulture.TextInfo.ListSeparator);
                }
                builder.Append(Convert.ToString(obj2, (IFormatProvider)CultureInfo.InvariantCulture));
            }
            return builder.ToString();
        }

        /// <summary>
        /// Checks and initializes an XmlReader object.
        /// </summary>
        /// <param name="reader">The XmlReader to be checked and initialized.</param>
        public static void InitReader(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (reader.NodeType == ((XmlNodeType)((int)XmlNodeType.None)))
            {
                reader.Read();
            }
            if (reader.NodeType == ((XmlNodeType)((int)XmlNodeType.XmlDeclaration)))
            {
                reader.Read();
            }
            if (reader.NodeType == ((XmlNodeType)((int)XmlNodeType.Whitespace)))
            {
                reader.Read();
            }
        }

        /// <summary>
        /// Checks and initializes an XmlWriter object.
        /// </summary>
        /// <param name="writer">The XmlWriter to be checked and initialized.</param>
        public static void InitWriter(XmlWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
        }

        static bool IsObjEmpty(object obj)
        {
            if (obj is MatrixTransform)
            {
                bool isEmpty = false;
                MatrixTransform transform = obj as MatrixTransform;
                if ((((transform.Matrix.M11 == 1.0) && (transform.Matrix.M12 == 0.0)) && ((transform.Matrix.M21 == 0.0) && (transform.Matrix.M22 == 1.0))) && ((transform.Matrix.OffsetX == 0.0) && (transform.Matrix.OffsetY == 0.0)))
                {
                    isEmpty = true;
                }
                return isEmpty;
            }
            return false;
        }

        static bool IsSimpleType(Type type)
        {
            if ((((((type != typeof(string)) && (type != typeof(double))) && ((type != typeof(int)) && (type != typeof(short)))) && (((type != typeof(long)) && (type != typeof(bool))) && ((type != typeof(float)) && (type != typeof(char))))) && ((((type != typeof(byte)) && (type != typeof(sbyte))) && ((type != typeof(ushort)) && (type != typeof(uint)))) && (((type != typeof(ulong)) && (type != typeof(DateTime))) && ((type != typeof(TimeSpan)) && (type != typeof(decimal)))))) && ((((type != typeof(double?)) && (type != typeof(int?))) && ((type != typeof(short?)) && (type != typeof(long?)))) && (((type != typeof(bool?)) && (type != typeof(float?))) && ((type != typeof(char?)) && (type != typeof(byte?))))))
            {
                return false;
            }
            return true;
        }

        static bool IsSupportedValueType(Type type)
        {
            if ((!IsSimpleType(type) && !IntrospectionExtensions.GetTypeInfo(type).IsEnum) && ((type != typeof(Image)) && (type != typeof(Windows.UI.Color))))
            {
                return (type == typeof(Array));
            }
            return true;
        }

        static object Parse(Type type, string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            if (IsSimpleType(type))
            {
                if (type == typeof(string))
                {
                    return str;
                }
                if ((type == typeof(double)) || (type == typeof(double?)))
                {
                    try
                    {
                        return (double)double.Parse(str, (IFormatProvider)CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        throw new InvalidCastException(string.Format(string.Format(ResourceStrings.SerializerParseTypeNotMatchError, (object[])new object[] { str, type.ToString() }), (object[])new object[0]));
                    }
                }
                if ((type == typeof(int)) || (type == typeof(int?)))
                {
                    try
                    {
                        return (int)int.Parse(str, (IFormatProvider)CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        throw new InvalidCastException(string.Format(ResourceStrings.SerializerParseTypeNotMatchError, (object[])new object[] { str, type.ToString() }));
                    }
                }
                if ((type == typeof(short)) || (type == typeof(short?)))
                {
                    try
                    {
                        return (short)short.Parse(str, (IFormatProvider)CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        throw new InvalidCastException(string.Format(ResourceStrings.SerializerParseTypeNotMatchError, (object[])new object[] { str, type.ToString() }));
                    }
                }
                if ((type == typeof(long)) || (type == typeof(long?)))
                {
                    try
                    {
                        return (long)long.Parse(str, (IFormatProvider)CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        throw new InvalidCastException(string.Format(ResourceStrings.SerializerParseTypeNotMatchError, (object[])new object[] { str, type.ToString() }));
                    }
                }
                if ((type == typeof(bool)) || (type == typeof(bool?)))
                {
                    try
                    {
                        return (bool)bool.Parse(str);
                    }
                    catch
                    {
                        throw new InvalidCastException(string.Format(ResourceStrings.SerializerParseTypeNotMatchError, (object[])new object[] { str, type.ToString() }));
                    }
                }
                if ((type == typeof(float)) || (type == typeof(float?)))
                {
                    try
                    {
                        return (float)float.Parse(str, (IFormatProvider)CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        throw new InvalidCastException(string.Format(ResourceStrings.SerializerParseTypeNotMatchError, (object[])new object[] { str, type.ToString() }));
                    }
                }
                if ((type == typeof(char)) || (type == typeof(char?)))
                {
                    char result = '\0';
                    if (!char.TryParse(str, out result))
                    {
                        throw new InvalidCastException(string.Format(ResourceStrings.SerializerParseTypeNotMatchError, (object[])new object[] { str, type.ToString() }));
                    }
                    return (char)result;
                }
                if ((type == typeof(byte)) || (type == typeof(byte?)))
                {
                    try
                    {
                        return (byte)byte.Parse(str, (IFormatProvider)CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        throw new InvalidCastException(string.Format(ResourceStrings.SerializerParseTypeNotMatchError, (object[])new object[] { str, type.ToString() }));
                    }
                }
                if (type == typeof(sbyte))
                {
                    try
                    {
                        return (sbyte)sbyte.Parse(str, (IFormatProvider)CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        throw new InvalidCastException(string.Format(ResourceStrings.SerializerParseTypeNotMatchError, (object[])new object[] { str, type.ToString() }));
                    }
                }
                if (type == typeof(ushort))
                {
                    try
                    {
                        return (ushort)ushort.Parse(str, (IFormatProvider)CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        throw new InvalidCastException(string.Format(ResourceStrings.SerializerParseTypeNotMatchError, (object[])new object[] { str, type.ToString() }));
                    }
                }
                if (type == typeof(uint))
                {
                    try
                    {
                        return (uint)uint.Parse(str, (IFormatProvider)CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        throw new InvalidCastException(string.Format(ResourceStrings.SerializerParseTypeNotMatchError, (object[])new object[] { str, type.ToString() }));
                    }
                }
                if (type == typeof(ulong))
                {
                    try
                    {
                        return (ulong)ulong.Parse(str, (IFormatProvider)CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        throw new InvalidCastException(string.Format(ResourceStrings.SerializerParseTypeNotMatchError, (object[])new object[] { str, type.ToString() }));
                    }
                }
                if (type == typeof(DateTime))
                {
                    try
                    {
                        return DateTime.Parse(str, (IFormatProvider)CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        throw new InvalidCastException(string.Format(ResourceStrings.SerializerParseTypeNotMatchError, (object[])new object[] { str, type.ToString() }));
                    }
                }
                if (type == typeof(TimeSpan))
                {
                    try
                    {
                        return TimeSpan.Parse(str, (IFormatProvider)CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        throw new InvalidCastException(string.Format(ResourceStrings.SerializerParseTypeNotMatchError, (object[])new object[] { str, type.ToString() }));
                    }
                }
                if (type != typeof(decimal))
                {
                    goto Label_07BF;
                }
                try
                {
                    return decimal.Parse(str, (IFormatProvider)CultureInfo.InvariantCulture);
                }
                catch
                {
                    throw new InvalidCastException(string.Format(ResourceStrings.SerializerParseTypeNotMatchError, (object[])new object[] { str, type.ToString() }));
                }
            }
            if (IntrospectionExtensions.GetTypeInfo(type).IsEnum)
            {
                try
                {
                    return Enum.Parse(type, str, true);
                }
                catch
                {
                    if ((type != typeof(Visibility)) || (str.ToLower() != "hidden"))
                    {
                        throw new InvalidCastException(string.Format(ResourceStrings.SerializerParseTypeNotMatchError, (object[])new object[] { str, type.ToString() }));
                    }
                    return Visibility.Collapsed;
                }
            }
            if (type == typeof(Image))
            {
                throw new NotSupportedException(ResourceStrings.SerializeImageError);
            }
            if (type == typeof(Windows.UI.Color))
            {
                try
                {
                    Windows.UI.Color? nullable = Dt.Cells.Data.ColorHelper.FromStringValue(str);
                    if (!nullable.HasValue)
                    {
                        throw new InvalidCastException(string.Format(ResourceStrings.SerializerParseTypeNotMatchError, (object[])new object[] { str, type.ToString() }));
                    }
                    return nullable.Value;
                }
                catch
                {
                    throw new InvalidCastException(string.Format(ResourceStrings.SerializerParseTypeNotMatchError, (object[])new object[] { str, type.ToString() }));
                }
            }
            if (type == typeof(SolidColorBrush))
            {
                try
                {
                    Windows.UI.Color? color = Dt.Cells.Data.ColorHelper.FromStringValue(str);
                    if (!color.HasValue)
                    {
                        throw new InvalidCastException(string.Format(ResourceStrings.SerializerParseTypeNotMatchError, (object[])new object[] { str, type.ToString() }));
                    }
                    return new SolidColorBrush(color.Value);
                }
                catch
                {
                    throw new InvalidCastException(string.Format(ResourceStrings.SerializerParseTypeNotMatchError, (object[])new object[] { str, type.ToString() }));
                }
            }
            if (type == typeof(Array))
            {
                Type elementType = type.GetElementType();
                new List<object>();
                string[] strArray = str.Split(new string[] { CultureInfo.InvariantCulture.TextInfo.ListSeparator }, (StringSplitOptions)StringSplitOptions.None);
                if (strArray == null)
                {
                    throw new InvalidCastException(string.Format(ResourceStrings.SerializerParseTypeNotMatchError, (object[])new object[] { str, type.ToString() }));
                }
                Array array = Array.CreateInstance(elementType, new int[] { strArray.Length });
                for (int i = 0; i < strArray.Length; i++)
                {
                    string source = strArray[i];
                    object obj2 = CreateTypeInstance(elementType, source);
                    if (obj2 == null)
                    {
                        throw new Exception(ResourceStrings.SerializeCannotCreateTypeOfArray);
                    }
                    array.SetValue(obj2, new int[] { i });
                }
                return array;
            }
            Label_07BF:
            throw new InvalidCastException(ResourceStrings.SerializerInvalidCastError);
        }

        static bool ParseValuePair(string str, out double a, out double b)
        {
            a = 0.0;
            b = 0.0;
            string[] strArray = str.Split(new char[] { ',' });
            if (strArray.Length != 2)
            {
                return false;
            }
            double? nullable = Dt.Cells.Data.FormatConverter.TryDouble(strArray[0], false);
            double? nullable2 = Dt.Cells.Data.FormatConverter.TryDouble(strArray[1], false);
            if (!nullable.HasValue || !nullable2.HasValue)
            {
                return false;
            }
            a = nullable.Value;
            b = nullable2.Value;
            return true;
        }

        internal static string ReadAttribute(string attributeName, XmlReader reader)
        {
            bool result = false;
            if (reader.MoveToAttribute("Base64" + attributeName))
            {
                string str = reader.Value;
                if (str != null)
                {
                    bool.TryParse(str, out result);
                }
            }
            if (!reader.MoveToAttribute(attributeName))
            {
                return null;
            }
            string s = reader.Value;
            if (result)
            {
                byte[] bytes = Convert.FromBase64String(s);
                return (string)new string(Encoding.Unicode.GetChars(bytes));
            }
            return s;
        }

        /// <summary>
        /// Reads the attribute.
        /// </summary>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="reader">The reader.</param>
        internal static bool ReadAttributeBoolean(string attributeName, bool defaultValue, XmlReader reader)
        {
            string str = ReadAttribute(attributeName, reader);
            if (str != null)
            {
                bool result = false;
                if (bool.TryParse(str, out result))
                {
                    return result;
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// Reads the attribute value.
        /// </summary>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        internal static byte? ReadAttributeByte(string attributeName, XmlReader reader)
        {
            string str = ReadAttribute(attributeName, reader);
            if (str != null)
            {
                byte num = 0;
                if (byte.TryParse(str, (NumberStyles)NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture, out num))
                {
                    return new byte?(num);
                }
            }
            return null;
        }

        /// <summary>
        /// Reads the attribute.
        /// </summary>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="reader">The reader.</param>
        internal static byte ReadAttributeByte(string attributeName, byte defaultValue, XmlReader reader)
        {
            byte? nullable = ReadAttributeByte(attributeName, reader);
            if (nullable.HasValue)
            {
                return nullable.Value;
            }
            return defaultValue;
        }

        /// <summary>
        /// Reads the attribute.
        /// </summary>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="reader">The reader.</param>
        internal static Windows.UI.Color? ReadAttributeColor(string attributeName, XmlReader reader)
        {
            return Dt.Cells.Data.ColorHelper.FromStringValue(ReadAttribute(attributeName, reader));
        }

        /// <summary>
        /// Reads the attribute.
        /// </summary>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="reader">The reader.</param>
        internal static double ReadAttributeDouble(string attributeName, double defaultValue, XmlReader reader)
        {
            string str = ReadAttribute(attributeName, reader);
            if (str != null)
            {
                double num = 0.0;
                if (double.TryParse(str, (NumberStyles)NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture, out num))
                {
                    return num;
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// Reads the attribute enumeration.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        internal static T? ReadAttributeEnum<T>(string attributeName, XmlReader reader) where T : struct
        {
            string str = ReadAttribute(attributeName, reader);
            if (str != null)
            {
                try
                {
                    return new T?((T)Enum.Parse((Type)typeof(T), str, true));
                }
                catch
                {
                }
            }
            return null;
        }

        /// <summary>
        /// Reads the attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        internal static T ReadAttributeEnum<T>(string attributeName, T defaultValue, XmlReader reader) where T : struct
        {
            T? nullable = ReadAttributeEnum<T>(attributeName, reader);
            if (nullable.HasValue)
            {
                return nullable.Value;
            }
            return defaultValue;
        }

        /// <summary>
        /// Reads the attribute.
        /// </summary>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="reader">The reader.</param>
        internal static float ReadAttributeFloat(string attributeName, float defaultValue, XmlReader reader)
        {
            string str = ReadAttribute(attributeName, reader);
            if (str != null)
            {
                float num = 0f;
                if (float.TryParse(str, (NumberStyles)NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture, out num))
                {
                    return num;
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// Reads the attribute value.
        /// </summary>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        internal static int? ReadAttributeInt(string attributeName, XmlReader reader)
        {
            string str = ReadAttribute(attributeName, reader);
            if (str != null)
            {
                int num = 0;
                if (int.TryParse(str, (NumberStyles)NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture, out num))
                {
                    return new int?(num);
                }
            }
            return null;
        }

        /// <summary>
        /// Reads the attribute.
        /// </summary>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="reader">The reader.</param>
        internal static int ReadAttributeInt(string attributeName, int defaultValue, XmlReader reader)
        {
            int? nullable = ReadAttributeInt(attributeName, reader);
            if (nullable.HasValue)
            {
                return nullable.Value;
            }
            return defaultValue;
        }

        internal static Windows.Foundation.Point? ReadAttributeLocation(string attributeName, XmlReader reader)
        {
            string str = ReadAttribute(attributeName, reader);
            if (str != null)
            {
                if (str.Equals("empty", (StringComparison)StringComparison.CurrentCultureIgnoreCase))
                {
                    return new Windows.Foundation.Point(0.0, 0.0);
                }
                string[] strArray = str.Split(new char[] { Dt.Cells.Data.DefaultTokens.Comma });
                if (strArray.Length == 2)
                {
                    double? nullable = Dt.Cells.Data.FormatConverter.TryDouble(strArray[0], false);
                    double? nullable2 = Dt.Cells.Data.FormatConverter.TryDouble(strArray[1], false);
                    if (!nullable.HasValue || !nullable2.HasValue)
                    {
                        return null;
                    }
                    double x = nullable.Value;
                    return new Windows.Foundation.Point(x, nullable2.Value);
                }
            }
            return null;
        }

        internal static Windows.Foundation.Point ReadAttributeLocation(string attributeName, Windows.Foundation.Point defaultValue, XmlReader reader)
        {
            Windows.Foundation.Point? nullable = ReadAttributeLocation(attributeName, reader);
            if (nullable.HasValue)
            {
                return nullable.Value;
            }
            return defaultValue;
        }

        /// <summary>
        /// Reads the attribute.
        /// </summary>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="reader">The reader.</param>
        internal static Windows.Foundation.Size? ReadAttributeSize(string attributeName, XmlReader reader)
        {
            string str = ReadAttribute(attributeName, reader);
            if (str != null)
            {
                if (str.Equals("empty", (StringComparison)StringComparison.CurrentCultureIgnoreCase))
                {
                    return new Windows.Foundation.Size?(Windows.Foundation.Size.Empty);
                }
                string[] strArray = str.Split(new char[] { Dt.Cells.Data.DefaultTokens.Comma });
                if (strArray.Length == 2)
                {
                    double? nullable = Dt.Cells.Data.FormatConverter.TryDouble(strArray[0], false);
                    double? nullable2 = Dt.Cells.Data.FormatConverter.TryDouble(strArray[1], false);
                    if (!nullable.HasValue || !nullable2.HasValue)
                    {
                        return null;
                    }
                    double width = nullable.Value;
                    return new Windows.Foundation.Size(width, nullable2.Value);
                }
            }
            return null;
        }

        /// <summary>
        /// Reads the attribute.
        /// </summary>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="reader">The reader.</param>
        internal static Windows.Foundation.Size ReadAttributeSize(string attributeName, Windows.Foundation.Size defaultValue, XmlReader reader)
        {
            Windows.Foundation.Size? nullable = ReadAttributeSize(attributeName, reader);
            if (nullable.HasValue)
            {
                return nullable.Value;
            }
            return defaultValue;
        }

        /// <summary>
        /// Reads the attribute.
        /// </summary>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="reader">The reader.</param>
        internal static Type ReadAttributeType(string attributeName, XmlReader reader)
        {
            string typeName = ReadAttribute(attributeName, reader);
            if (typeName != null)
            {
                try
                {
                    return CreateType(typeName, null);
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        static Type ReadTypeAttr(XmlReader reader)
        {
            string typeName = ReadAttribute("type", reader);
            string assembly = ReadAttribute("assembly", reader);
            return CreateType(typeName, assembly);
        }

        internal static void SerializeArray<T>(SparseArray<T> array, bool saveType, XmlWriter writer) where T : class
        {
            int num = (array == null) ? 0 : array.Length;
            WriteAttr("length", (int)num, writer);
            if ((array != null) && (array.DataLength > 0))
            {
                int index = -1;
                do
                {
                    int num3 = array.NextNonEmptyIndex(index);
                    if (num3 == -1)
                    {
                        index = -1;
                    }
                    else
                    {
                        if (num3 != -1)
                        {
                            index = num3;
                        }
                        if ((num3 != -1) && (num3 < index))
                        {
                            index = num3;
                        }
                    }
                    if (index != -1)
                    {
                        T local = array[index];
                        SerializeIndexObj(index, local, "Item", saveType, writer);
                    }
                }
                while (index != -1);
            }
        }

        /// <summary>
        /// Serializes the URI.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="writer">The writer.</param>
        internal static void SerializeArrayBool2(bool[,] array, string elementName, XmlWriter writer)
        {
            int rank = array.Rank;
            int length = array.GetLength(0);
            writer.WriteStartElement(elementName);
            WriteAttribute("rank", rank, writer);
            WriteAttribute("length", length, writer);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < rank; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    bool flag = array[j, i];
                    if (builder.Length > 0)
                    {
                        builder.Append(Dt.Cells.Data.DefaultTokens.Comma);
                    }
                    builder.Append(flag ? ((string)"1") : ((string)"0"));
                }
            }
            writer.WriteString(builder.ToString());
            writer.WriteEndElement();
        }

        internal static void SerializeCalcStorage(SpreadCalcStorage storage, string elementName, ICalcEvaluator eveluator, XmlWriter writer)
        {
            IEnumerator<KeyValuePair<CalcLocalIdentity, CalcExpression>> enumerator = storage.GetEnumerator();
            WriteStartObj(elementName, writer);
            while (enumerator.MoveNext())
            {
                CalcLocalIdentity id = enumerator.Current.Key;
                int row = 0;
                int column = 0;
                int rowCount = 1;
                int columnCount = 1;
                id.ExtractIdentity(out row, out column, out rowCount, out columnCount);
                CalcExpression expression = enumerator.Current.Value;
                if ((id != null) && (expression != null))
                {
                    string str = eveluator.Expression2Formula(expression, row, column);
                    if (!string.IsNullOrEmpty(str))
                    {
                        int rowIndex = -1;
                        int columnIndex = -1;
                        int num7 = -1;
                        int num8 = -1;
                        if (id is CalcCellIdentity)
                        {
                            CalcCellIdentity identity2 = id as CalcCellIdentity;
                            rowIndex = identity2.RowIndex;
                            columnIndex = identity2.ColumnIndex;
                        }
                        if ((id is CalcRangeIdentity) && (expression is CalcSharedExpression))
                        {
                            CalcRangeIdentity identity3 = id as CalcRangeIdentity;
                            Worksheet worksheet = eveluator as Worksheet;
                            if (worksheet != null)
                            {
                                int num9 = identity3.IsFullRow ? worksheet.ColumnCount : identity3.ColumnCount;
                                int num10 = identity3.IsFullColumn ? worksheet.RowCount : identity3.RowCount;
                                int num11 = identity3.IsFullColumn ? 0 : identity3.RowIndex;
                                int num12 = identity3.IsFullRow ? 0 : identity3.ColumnIndex;
                                for (int i = 0; i < num10; i++)
                                {
                                    for (int j = 0; j < num9; j++)
                                    {
                                        rowIndex = num11 + i;
                                        columnIndex = num12 + j;
                                        num7 = -1;
                                        num8 = -1;
                                        WriteStartObj("Item", writer);
                                        WriteAttr("r", (int)rowIndex, writer);
                                        WriteAttr("c", (int)columnIndex, writer);
                                        if (num7 > 0)
                                        {
                                            WriteAttr("rc", (int)num7, writer);
                                        }
                                        if (num8 > 0)
                                        {
                                            WriteAttr("cc", (int)num8, writer);
                                        }
                                        WriteAttr("formula", str, writer);
                                        WriteEndObj(writer);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (id is CalcRangeIdentity)
                            {
                                CalcRangeIdentity identity4 = id as CalcRangeIdentity;
                                rowIndex = identity4.RowIndex;
                                columnIndex = identity4.ColumnIndex;
                                num7 = identity4.RowCount;
                                num8 = identity4.ColumnCount;
                            }
                            WriteStartObj("Item", writer);
                            WriteAttr("r", (int)rowIndex, writer);
                            WriteAttr("c", (int)columnIndex, writer);
                            if (num7 > 0)
                            {
                                WriteAttr("rc", (int)num7, writer);
                            }
                            if (num8 > 0)
                            {
                                WriteAttr("cc", (int)num8, writer);
                            }
                            WriteAttr("formula", str, writer);
                            WriteEndObj(writer);
                        }
                    }
                }
            }
            WriteEndObj(writer);
        }

        internal static void SerializeCellObj(object obj, XmlWriter writer, Dictionary<string, string> typeMap, Dictionary<string, string> assemblyMap)
        {
            if (obj != null)
            {
                Type type = obj.GetType();
                string fullName = type.FullName;
                if (typeMap != null)
                {
                    if (typeMap.ContainsKey(fullName))
                    {
                        WriteAttribute("type", typeMap[fullName], writer);
                    }
                    else
                    {
                        string str2 = typeMap.Count.ToString();
                        typeMap.Add(fullName, str2);
                        WriteAttribute("type", str2, writer);
                        if (!TypePool.IsSXType(type))
                        {
                            string name = IntrospectionExtensions.GetTypeInfo(type).Assembly.GetName().Name;
                            if (!string.IsNullOrEmpty(name))
                            {
                                assemblyMap.Add(fullName, name);
                            }
                        }
                    }
                }
                SerializeObj(obj, null, false, writer);
            }
        }

        internal static void SerializeConditionalFormats(ConditionalFormat formats, string elementName, XmlWriter writer)
        {
            WriteStartObj(elementName, writer);
            for (int i = 0; i < formats.RuleCount; i++)
            {
                FormattingRuleBase instance = formats[i];
                if ((instance != null) && (instance.Ranges != null))
                {
                    WriteStartObj("Item", writer);
                    WriteTypeAttr(instance, writer);
                    SerializeObj(instance, null, writer);
                    WriteEndObj(writer);
                }
            }
            WriteEndObj(writer);
        }

        /// <summary>
        /// Serializes the date time format information.
        /// </summary>
        /// <param name="obj">The datetime format information.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="writer">The writer.</param>
        internal static void SerializeDateTimeFormatInfo(DateTimeFormatInfo obj, string elementName, XmlWriter writer)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            if (elementName == null)
            {
                throw new ArgumentNullException("elementName");
            }
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            writer.WriteStartElement(elementName);
            writer.WriteEndElement();
        }

        internal static void SerializeGenericList<T>(IList<T> list, XmlWriter writer)
        {
            foreach (T local in list)
            {
                WriteStartObj("Item", writer);
                WriteTypeAttr(local, writer);
                SerializeObj(local, null, writer);
                WriteEndObj(writer);
            }
        }

        internal static void SerializeGenericList<T>(IList<T> list, string listName, XmlWriter writer)
        {
            if (listName != null)
            {
                writer.WriteStartElement(listName);
            }
            foreach (T local in list)
            {
                WriteStartObj("Item", writer);
                WriteTypeAttr(local, writer);
                SerializeObj(local, null, writer);
                WriteEndObj(writer);
            }
            if (listName != null)
            {
                writer.WriteEndElement();
            }
        }
        
        internal static void SerializeIndex(SparseArray<object> indexes, string elementName, XmlWriter writer)
        {
            if (indexes != null)
            {
                int length = indexes.Length;
                MemoryStream stream = new MemoryStream();
                stream.WriteByte((byte)((length & 0xff000000L) >> 0x18));
                stream.WriteByte((byte)((length & 0xff0000) >> 0x10));
                stream.WriteByte((byte)((length & 0xff00) >> 8));
                stream.WriteByte((byte)(length & 0xff));
                for (int i = indexes.FirstNonEmptyIndex(); i > -1; i = indexes.NextNonEmptyIndex(i))
                {
                    object obj2 = indexes[i];
                    if (obj2 != null)
                    {
                        int num3 = (int)((int)obj2);
                        stream.WriteByte((byte)((i & 0xff000000L) >> 0x18));
                        stream.WriteByte((byte)((i & 0xff0000) >> 0x10));
                        stream.WriteByte((byte)((i & 0xff00) >> 8));
                        stream.WriteByte((byte)(i & 0xff));
                        stream.WriteByte((byte)((num3 & 0xff000000L) >> 0x18));
                        stream.WriteByte((byte)((num3 & 0xff0000) >> 0x10));
                        stream.WriteByte((byte)((num3 & 0xff00) >> 8));
                        stream.WriteByte((byte)(num3 & 0xff));
                    }
                }
                stream.Seek(0L, (SeekOrigin)SeekOrigin.Begin);
                MemoryStream stream2 = new MemoryStream();
                ((Stream)stream).Close();
                byte[] buffer = new byte[stream2.Length];
                stream2.Seek(0L, (SeekOrigin)SeekOrigin.Begin);
                stream2.Read(buffer, 0, (int)stream2.Length);
                stream2.ToArray();
                string str = Convert.ToBase64String(buffer);
                writer.WriteStartElement(elementName);
                writer.WriteValue(str);
                writer.WriteEndElement();
            }
        }

        internal static void SerializeIndexObj(int index, object obj, string elementName, bool saveUnknowType, XmlWriter writer)
        {
            writer.WriteStartElement(elementName);
            WriteAttr("index", (int)index, writer);
            if (saveUnknowType)
            {
                WriteTypeAttr(obj, writer);
            }
            SerializeObj(obj, null, writer);
            writer.WriteEndElement();
        }

        internal static void SerializeList(IList list, XmlWriter writer)
        {
            foreach (object obj2 in list)
            {
                WriteStartObj("Item", writer);
                WriteTypeAttr(obj2, writer);
                SerializeObj(obj2, null, writer);
                WriteEndObj(writer);
            }
        }

        internal static void SerializeList(IList list, string listName, XmlWriter writer)
        {
            if (listName != null)
            {
                writer.WriteStartElement(listName);
            }
            foreach (object obj2 in list)
            {
                WriteStartObj("Item", writer);
                WriteTypeAttr(obj2, writer);
                SerializeObj(obj2, null, writer);
                WriteEndObj(writer);
            }
            if (listName != null)
            {
                writer.WriteEndElement();
            }
        }

        internal static void SerializeMatrix<T>(DataMatrix<T> matrix, XmlWriter writer) where T : class
        {
            SerializeMatrix<T>(matrix, true, writer, false);
        }

        internal static void SerializeMatrix<T>(DataMatrix<T> matrix, bool saveType, XmlWriter writer, bool serializeNullValue) where T : class
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
            if ((matrix != null) && (matrix.ColumnCount > 0))
            {
                int row = -1;
                do
                {
                    int num2 = matrix.NextNonEmptyRow(row);
                    if (num2 == -1)
                    {
                        row = -1;
                    }
                    else
                    {
                        if (num2 != -1)
                        {
                            row = num2;
                        }
                        if ((num2 != -1) && (num2 < row))
                        {
                            row = num2;
                        }
                    }
                    if (row != -1)
                    {
                        int column = -1;
                        do
                        {
                            int num4 = ((column + 1) < matrix.ColumnCount) ? (column + 1) : -1;
                            if (num4 == -1)
                            {
                                column = -1;
                            }
                            else
                            {
                                if (num4 != -1)
                                {
                                    column = num4;
                                }
                                if ((num4 != -1) && (num4 < column))
                                {
                                    column = num4;
                                }
                            }
                            if (column != -1)
                            {
                                object obj2 = matrix.GetValue(row, column);
                                if ((obj2 != null) || serializeNullValue)
                                {
                                    writer.WriteStartElement("C");
                                    WriteAttribute("pos", string.Format("{0},{1}", (object[])new object[] { ((int)row), ((int)column) }), writer);
                                    if (saveType)
                                    {
                                        SerializeCellObj(obj2, writer, saveType ? dictionary : null, saveType ? dictionary2 : null);
                                    }
                                    else
                                    {
                                        SerializeObj(obj2, null, writer);
                                    }
                                    writer.WriteEndElement();
                                }
                            }
                        }
                        while (column != -1);
                    }
                }
                while (row != -1);
            }
            if ((dictionary.Count > 0) && saveType)
            {
                writer.WriteStartElement("Types");
                foreach (string str in dictionary.Keys)
                {
                    writer.WriteStartElement("Type");
                    WriteAttribute("id", dictionary[str], writer);
                    WriteAttribute("name", str, writer);
                    if (dictionary2.ContainsKey(str))
                    {
                        WriteAttribute("assembly", dictionary2[str], writer);
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }

        internal static void SerializeNameInfos(NameInfoCollection names, string elementName, ICalcEvaluator evaluator, XmlWriter writer)
        {
            WriteStartObj(elementName, writer);
            foreach (NameInfo info in (IEnumerable<NameInfo>)names)
            {
                if (info != null)
                {
                    string str = evaluator.Expression2Formula(info.Expression, 0, 0);
                    if (str != null)
                    {
                        WriteStartObj("Item", writer);
                        WriteAttr("name", info.Name, writer);
                        WriteAttr("formula", str, writer);
                        if (info.BaseRow != 0)
                        {
                            WriteAttr("r", (int)info.BaseRow, writer);
                        }
                        if (info.BaseColumn != 0)
                        {
                            WriteAttr("c", (int)info.BaseColumn, writer);
                        }
                        WriteEndObj(writer);
                    }
                }
            }
            WriteEndObj(writer);
        }

        /// <summary>
        /// Serializes the number format information.
        /// </summary>
        /// <param name="obj">The number format information.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="writer">The writer.</param>
        internal static void SerializeNumberFormatInfo(NumberFormatInfo obj, string elementName, XmlWriter writer)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            if (elementName == null)
            {
                throw new ArgumentNullException("elementName");
            }
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            writer.WriteStartElement(elementName);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Serializes the object as an element of XML.
        /// </summary>
        /// <param name="obj">The object to be serialized.</param>
        /// <param name="elementName">The element name of the object to be serialized.</param>
        /// <param name="writer">The XmlWriter with which to write the object.</param>
        public static void SerializeObj(object obj, string elementName, XmlWriter writer)
        {
            SerializeObj(obj, elementName, true, writer);
        }

        internal static void SerializeObj(object obj, string elementName, bool saveUnknowType, XmlWriter writer)
        {
            if ((!(obj is IDataEmptySupport) || !((IDataEmptySupport)obj).IsDataEmpty) && !IsObjEmpty(obj))
            {
                if (elementName != null)
                {
                    writer.WriteStartElement(elementName);
                }
                if (obj is IXmlSerializable)
                {
                    ((IXmlSerializable)obj).WriteXml(writer);
                }
                else if ((obj != null) && IsSupportedValueType(obj.GetType()))
                {
                    WriteAttr("value", obj, writer);
                }
                else if ((obj != null) && (obj is SparseArray<object>))
                {
                    SerializeArray<object>(obj as SparseArray<object>, true, writer);
                }
                else if ((obj != null) && (obj is SparseArray<AxisInfo>))
                {
                    SerializeArray<AxisInfo>(obj as SparseArray<AxisInfo>, false, writer);
                }
                else if ((obj != null) && (obj is SparseArray<StyleInfo>))
                {
                    SerializeArray<StyleInfo>(obj as SparseArray<StyleInfo>, false, writer);
                }
                else if ((obj != null) && (obj is CellRange))
                {
                    WriteAttr("value", ((CellRange)obj).ToString(), writer);
                }
                else if ((obj != null) && (obj is Windows.Foundation.Point))
                {
                    // uno
                    WriteAttr("value", Convert.ToString(obj, (IFormatProvider)CultureInfo.InvariantCulture), writer);
                    //WriteAttr("value", ((Windows.Foundation.Point)obj).ToString((IFormatProvider)CultureInfo.InvariantCulture), writer);
                }
                else if ((obj != null) && (obj is Windows.Foundation.Size))
                {
                    WriteAttr("value", Convert.ToString(obj, (IFormatProvider)CultureInfo.InvariantCulture), writer);
                }
                else if ((obj != null) && (obj is GradientStopCollection))
                {
                    GradientStopCollection stops = obj as GradientStopCollection;
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < stops.Count; i++)
                    {
                        builder.Append(stops[i].Color.ToString()).Append(',').Append(((double)stops[i].Offset).ToString());
                        if (i != (stops.Count - 1))
                        {
                            builder.Append(' ');
                        }
                    }
                    WriteAttr("value", builder.ToString(), writer);
                }
                else if ((obj != null) && (obj is IList))
                {
                    SerializeList(obj as IList, writer);
                }
                else if ((obj != null) && (obj is CalcError))
                {
                    WriteAttr("value", ((CalcError)obj).ToString(), writer);
                }
                else if ((obj != null) && (obj is ImageBrush))
                {
                    ImageBrush brush = obj as ImageBrush;
                    BitmapImage imageSource = brush.ImageSource as BitmapImage;
                    if (imageSource != null)
                    {
                        Stream stream = Utility.GetImageStream(imageSource, ImageFormat.Png, PictureSerializationMode.Compressed);
                        if (stream != null)
                        {
                            WriteAttr("type", "Image", writer);
                            byte[] buffer = new byte[stream.Length];
                            stream.Seek(0L, (SeekOrigin)SeekOrigin.Begin);
                            stream.Read(buffer, 0, (int)stream.Length);
                            string str = Convert.ToBase64String(buffer);
                            WriteAttr("value", str, writer);
                        }
                    }
                }
                else if (obj != null)
                {
                    if (saveUnknowType)
                    {
                        WriteTypeAttr(obj, writer);
                    }
                    SerializePublicProperties(obj, writer);
                }
                if (elementName != null)
                {
                    writer.WriteEndElement();
                }
            }
        }

        internal static void SerializePublicProperties(object obj, XmlWriter writer)
        {
            foreach (PropertyInfo info in obj.GetType().GetRuntimeProperties())
            {
                if (info.CanRead && info.CanWrite)
                {
                    SerializeObj(info.GetValue(obj, new object[0]), info.Name, writer);
                }
            }
        }

        internal static void SerializeStorageBlock(StorageBlock block, string elementName, XmlWriter writer, bool isDataOnly)
        {
            if (block != null)
            {
                if (elementName != null)
                {
                    writer.WriteStartElement(elementName);
                }
                InitWriter(writer);
                WriteAttribute("rc", block.RowCount, writer);
                WriteAttribute("cc", block.ColumnCount, writer);
                block.SerializeData(writer);
                if (!isDataOnly)
                {
                    block.SerializeStyle(writer);
                }
                if (elementName != null)
                {
                    writer.WriteEndElement();
                }
            }
        }

        internal static void SerializeTables(EricTables tables, string elementName, XmlWriter writer)
        {
            WriteStartObj(elementName, writer);
            for (int i = 0; i < tables.Count; i++)
            {
                SheetTable table = tables[i];
                if (table != null)
                {
                    WriteStartObj("Item", writer);
                    SerializeObj(table, null, writer);
                    WriteEndObj(writer);
                }
            }
            WriteEndObj(writer);
        }

        internal static void SerializeTag(XmlWriter writer, object tag)
        {
            writer.WriteStartElement("Tag");
            WriteTypeAttr(tag, writer);
            SerializeObj(tag, null, false, writer);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Serializes the URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="writer">The writer.</param>
        internal static void SerializeUri(Uri uri, string elementName, XmlWriter writer)
        {
            string originalString = uri.OriginalString;
            if (!string.IsNullOrEmpty(originalString))
            {
                writer.WriteStartElement("Uri");
                writer.WriteString(originalString);
                writer.WriteEndElement();
            }
        }

        internal static void UpdateCutomNameString(NameInfoCollection names, ICalcEvaluator evaluator, List<List<object>> reader)
        {
            if (reader != null)
            {
                foreach (List<object> list in reader)
                {
                    string name = (string)(list[0] as string);
                    string formula = (string)(list[1] as string);
                    int baseRow = (int)((int)list[2]);
                    int baseColumn = (int)((int)list[3]);
                    CalcExpression expression = evaluator.Formula2Expression(formula, baseRow, baseColumn) as CalcExpression;
                    names.Add(new NameInfo(name, baseRow, baseColumn, expression));
                }
            }
        }

        internal static void WriteAttr(string attrName, object value, XmlWriter writer)
        {
            bool flag = false;
            string str = null;
            if (value is string)
            {
                str = (string)(value as string);
                for (int i = 0; i < str.Length; i++)
                {
                    char ch = str[i];
                    if ((ch >= '\0') && (ch < ' '))
                    {
                        flag = true;
                        break;
                    }
                }
            }
            if (value is char)
            {
                char ch2 = (char)((char)value);
                if ((ch2 >= '\0') && (ch2 < ' '))
                {
                    flag = true;
                }
                if (flag)
                {
                    str = Convert.ToString(ch2, (IFormatProvider)CultureInfo.InvariantCulture);
                }
            }
            if (flag)
            {
                writer.WriteAttributeString("Base64" + attrName, Format((bool)true));
                string s = (str != null) ? str : Format(value);
                writer.WriteAttributeString(attrName, Convert.ToBase64String(Encoding.Unicode.GetBytes(s)));
            }
            else
            {
                writer.WriteAttributeString(attrName, Format(value));
            }
        }

        /// <summary>
        /// Writes the attribute.
        /// </summary>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="value">The value.</param>
        /// <param name="writer">The writer.</param>
        internal static void WriteAttribute(string attributeName, bool value, XmlWriter writer)
        {
            writer.WriteAttributeString(attributeName, string.Format("{0}", (object[])new object[] { ((bool)value) }));
        }

        /// <summary>
        /// Writes the attribute.
        /// </summary>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="value">The value.</param>
        /// <param name="writer">The writer.</param>
        internal static void WriteAttribute(string attributeName, double value, XmlWriter writer)
        {
            writer.WriteAttributeString(attributeName, string.Format("{0}", (object[])new object[] { ((double)value) }));
        }

        /// <summary>
        /// Writes the attribute.
        /// </summary>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="value">The value.</param>
        /// <param name="writer">The writer.</param>
        internal static void WriteAttribute(string attributeName, Enum value, XmlWriter writer)
        {
            writer.WriteAttributeString(attributeName, value.ToString());
        }

        /// <summary>
        /// Writes the attribute.
        /// </summary>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="value">The value.</param>
        /// <param name="writer">The writer.</param>
        internal static void WriteAttribute(string attributeName, int value, XmlWriter writer)
        {
            writer.WriteAttributeString(attributeName, string.Format("{0}", (object[])new object[] { ((int)value) }));
        }

        /// <summary>
        /// Writes the attribute.
        /// </summary>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="value">The value.</param>
        /// <param name="writer">The writer.</param>
        internal static void WriteAttribute(string attributeName, string value, XmlWriter writer)
        {
            writer.WriteAttributeString(attributeName, value);
        }

        internal static void WriteAttribute(string attributeName, Windows.Foundation.Point value, XmlWriter writer)
        {
            // uno
            writer.WriteAttributeString(attributeName, Convert.ToString(value, (IFormatProvider)CultureInfo.InvariantCulture));
            //writer.WriteAttributeString(attributeName, value.ToString((IFormatProvider)CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Writes the attribute.
        /// </summary>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="value">The value.</param>
        /// <param name="writer">The writer.</param>
        internal static void WriteAttribute(string attributeName, Windows.Foundation.Size value, XmlWriter writer)
        {
            writer.WriteAttributeString(attributeName, value.ToString());
        }

        /// <summary>
        /// Writes the attribute.
        /// </summary>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="value">The value.</param>
        /// <param name="writer">The writer.</param>
        internal static void WriteAttribute(string attributeName, Windows.UI.Color value, XmlWriter writer)
        {
            writer.WriteAttributeString(attributeName, value.ToString());
        }

        /// <summary>
        /// Writes an ending XML element.
        /// </summary>
        /// <param name="writer">The XmlWriter with which to write the object.</param>
        public static void WriteEndObj(XmlWriter writer)
        {
            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes a starting XML element.
        /// </summary>
        /// <param name="eleName">The name of the starting element.</param>
        /// <param name="writer">The XmlWriter with which to write the object.</param>
        public static void WriteStartObj(string eleName, XmlWriter writer)
        {
            writer.WriteStartElement(eleName);
        }

        internal static void WriteTypeAttr(object instance, XmlWriter writer)
        {
            if (instance != null)
            {
                Type type = instance.GetType();
                if (IntrospectionExtensions.GetTypeInfo(instance.GetType()).Module == IntrospectionExtensions.GetTypeInfo((Type)typeof(Worksheet)).Module)
                {
                    WriteAttr("type", type.Name, writer);
                }
                else if (TypePool.IsSupported(type))
                {
                    WriteAttr("type", type.FullName, writer);
                }
                else
                {
                    WriteAttr("type", type.FullName, writer);
                    WriteAttr("assembly", IntrospectionExtensions.GetTypeInfo(type).Assembly.FullName, writer);
                }
            }
        }
    }
}

