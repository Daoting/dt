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
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml.Linq;
#endregion

namespace Dt.Xls.Utils
{
    internal static class XElementExtentions
    {
        internal static IEnumerable<XElement> AsEnumerable(this XElement element)
        {
            if ((element != null) && element.HasElements)
            {
                foreach (XElement iteratorVariable0 in element.Elements())
                {
                    yield return iteratorVariable0;
                }
            }
        }

        internal static byte[] GetAttributeValueOfByteArray(this XElement element, string attribute)
        {
            XAttribute attribute2 = element.Attribute((XName) attribute);
            if (attribute2 != null)
            {
                return Convert.FromBase64String(attribute2.Value);
            }
            return new byte[0];
        }

        internal static object GetAttributeValueOrDefault<T>(this XElement element, string attribute, T defaultValueIfMissingAttribute)
        {
            XAttribute attribute2 = element.Attribute((XName) attribute);
            if (attribute2 == null)
            {
                return defaultValueIfMissingAttribute;
            }
            int index = Array.IndexOf<Type>(TypeExtension.KnownTypes, typeof(T));
            if (index < 0)
            {
                return defaultValueIfMissingAttribute;
            }
            switch (TypeExtension.KnownCodes[index])
            {
                case TypeCode.Boolean:
                {
                    bool result = false;
                    bool.TryParse(attribute2.Value, out result);
                    if (attribute2.Value == "1")
                    {
                        result = true;
                    }
                    return (bool) result;
                }
                case TypeCode.Char:
                {
                    char ch = '\0';
                    char.TryParse(attribute2.Value, out ch);
                    return (char) ch;
                }
                case TypeCode.Byte:
                {
                    byte num2 = 0;
                    byte.TryParse(attribute2.Value, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num2);
                    return (byte) num2;
                }
                case TypeCode.Int16:
                {
                    short num6 = -32768;
                    short.TryParse(attribute2.Value, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num6);
                    return (short) num6;
                }
                case TypeCode.UInt16:
                {
                    ushort num5 = 0;
                    ushort.TryParse(attribute2.Value, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num5);
                    return (ushort) num5;
                }
                case TypeCode.Int32:
                {
                    int num8 = -2147483648;
                    int.TryParse(attribute2.Value, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num8);
                    return (int) num8;
                }
                case TypeCode.UInt32:
                {
                    uint num7 = 0;
                    uint.TryParse(attribute2.Value, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num7);
                    return (uint) num7;
                }
                case TypeCode.Single:
                {
                    float naN = float.NaN;
                    float.TryParse(attribute2.Value, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out naN);
                    return (float) naN;
                }
                case TypeCode.Double:
                {
                    double num4 = double.NaN;
                    double.TryParse(attribute2.Value, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num4);
                    return (double) num4;
                }
                case TypeCode.String:
                    return attribute2.Value;
            }
            return attribute2.Value;
        }

        internal static object GetAttributeValueOrDefault<T>(this XElement element, XName attribute, T defaultValue)
        {
            XAttribute attribute2 = element.Attribute(attribute);
            if (attribute2 == null)
            {
                return defaultValue;
            }
            int index = Array.IndexOf<Type>(TypeExtension.KnownTypes, typeof(T));
            if (index <= 0)
            {
                return defaultValue;
            }
            switch (TypeExtension.KnownCodes[index])
            {
                case TypeCode.Boolean:
                {
                    bool result = false;
                    if (attribute2.Value != "1")
                    {
                        if (attribute2.Value == "0")
                        {
                            return (bool) false;
                        }
                        bool.TryParse(attribute2.Value, out result);
                        return (bool) result;
                    }
                    return (bool) true;
                }
                case TypeCode.Char:
                {
                    char ch = '\0';
                    char.TryParse(attribute2.Value, out ch);
                    return (char) ch;
                }
                case TypeCode.Byte:
                {
                    byte num2 = 0;
                    byte.TryParse(attribute2.Value, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num2);
                    return (byte) num2;
                }
                case TypeCode.Int16:
                {
                    short num6 = -32768;
                    short.TryParse(attribute2.Value, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num6);
                    return (short) num6;
                }
                case TypeCode.UInt16:
                {
                    ushort num5 = 0;
                    ushort.TryParse(attribute2.Value, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num5);
                    return (ushort) num5;
                }
                case TypeCode.Int32:
                {
                    int num8 = -2147483648;
                    int.TryParse(attribute2.Value, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num8);
                    return (int) num8;
                }
                case TypeCode.UInt32:
                {
                    uint num7 = 0;
                    uint.TryParse(attribute2.Value, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num7);
                    return (uint) num7;
                }
                case TypeCode.Single:
                {
                    float naN = float.NaN;
                    float.TryParse(attribute2.Value, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out naN);
                    return (float) naN;
                }
                case TypeCode.Double:
                {
                    double num4 = double.NaN;
                    double.TryParse(attribute2.Value, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num4);
                    return (double) num4;
                }
                case TypeCode.String:
                    return attribute2.Value;
            }
            return attribute2.Value;
        }

        internal static bool GetAttributeValueOrDefaultOfBooleanType(this XElement element, string attribute, bool defaultValue)
        {
            XAttribute attribute2 = element.Attribute((XName) attribute);
            if (attribute2 == null)
            {
                return defaultValue;
            }
            if (attribute2.Value == "1")
            {
                return true;
            }
            if (attribute2.Value == "0")
            {
                return false;
            }
            bool result = defaultValue;
            bool.TryParse(attribute2.Value, out result);
            return result;
        }

        internal static bool GetAttributeValueOrDefaultOfBooleanType(this XElement element, XName attribute, bool defaultValue)
        {
            XAttribute attribute2 = element.Attribute(attribute);
            if (attribute2 == null)
            {
                return defaultValue;
            }
            if (attribute2.Value == "1")
            {
                return true;
            }
            if (attribute2.Value == "0")
            {
                return false;
            }
            bool result = defaultValue;
            bool.TryParse(attribute2.Value, out result);
            return result;
        }

        internal static byte GetAttributeValueOrDefaultOfByteType(this XElement element, string attribute, byte defaultValue)
        {
            XAttribute attribute2 = element.Attribute((XName) attribute);
            if (attribute2 != null)
            {
                byte num = defaultValue;
                byte.TryParse(attribute2.Value, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num);
                return num;
            }
            return defaultValue;
        }

        internal static byte GetAttributeValueOrDefaultOfByteType(this XElement element, XName attribute, byte defaultValue)
        {
            XAttribute attribute2 = element.Attribute(attribute);
            if (attribute2 != null)
            {
                byte num = defaultValue;
                byte.TryParse(attribute2.Value, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num);
                return num;
            }
            return defaultValue;
        }

        internal static double GetAttributeValueOrDefaultOfDoubleType(this XElement element, string attribute, double defaultValue)
        {
            XAttribute attribute2 = element.Attribute((XName) attribute);
            if (attribute2 != null)
            {
                double num = defaultValue;
                double.TryParse(attribute2.Value, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num);
                return num;
            }
            return defaultValue;
        }

        internal static double GetAttributeValueOrDefaultOfDoubleType(this XElement element, XName attribute, double defaultValue)
        {
            XAttribute attribute2 = element.Attribute(attribute);
            if (attribute2 != null)
            {
                double num = defaultValue;
                double.TryParse(attribute2.Value, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num);
                return num;
            }
            return defaultValue;
        }

        internal static float GetAttributeValueOrDefaultOfFloatType(this XElement element, string attribute, float defaultValue)
        {
            XAttribute attribute2 = element.Attribute((XName) attribute);
            if (attribute2 != null)
            {
                float num = defaultValue;
                float.TryParse(attribute2.Value, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num);
                return num;
            }
            return defaultValue;
        }

        internal static float GetAttributeValueOrDefaultOfFloatType(this XElement element, XName attribute, float defaultValue)
        {
            XAttribute attribute2 = element.Attribute(attribute);
            if (attribute2 != null)
            {
                float num = defaultValue;
                float.TryParse(attribute2.Value, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num);
                return num;
            }
            return defaultValue;
        }

        internal static short GetAttributeValueOrDefaultOfInt16Type(this XElement element, string attribute, short defaultValue)
        {
            XAttribute attribute2 = element.Attribute((XName) attribute);
            if (attribute2 != null)
            {
                short num = defaultValue;
                short.TryParse(attribute2.Value, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num);
                return num;
            }
            return defaultValue;
        }

        internal static short GetAttributeValueOrDefaultOfInt16Type(this XElement element, XName attribute, short defaultValue)
        {
            XAttribute attribute2 = element.Attribute(attribute);
            if (attribute2 != null)
            {
                short num = defaultValue;
                short.TryParse(attribute2.Value, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num);
                return num;
            }
            return defaultValue;
        }

        internal static int GetAttributeValueOrDefaultOfInt32Type(this XElement element, string attribute, int defaultValue)
        {
            XAttribute attribute2 = element.Attribute((XName) attribute);
            if (attribute2 != null)
            {
                int num = defaultValue;
                int.TryParse(attribute2.Value, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num);
                return num;
            }
            return defaultValue;
        }

        internal static int GetAttributeValueOrDefaultOfInt32Type(this XElement element, XName attribute, int defaultValue)
        {
            XAttribute attribute2 = element.Attribute(attribute);
            if (attribute2 != null)
            {
                int num = defaultValue;
                int.TryParse(attribute2.Value, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num);
                return num;
            }
            return defaultValue;
        }

        internal static string GetAttributeValueOrDefaultOfStringType(this XElement element, string attribute, string defaultValue = null)
        {
            XAttribute attribute2 = element.Attribute((XName) attribute);
            if (attribute2 != null)
            {
                return attribute2.Value;
            }
            foreach (XAttribute attribute3 in element.Attributes())
            {
                if (attribute3.Name.LocalName == attribute)
                {
                    return attribute3.Value;
                }
            }
            return defaultValue;
        }

        internal static string GetAttributeValueOrDefaultOfStringType(this XElement element, XName attribute, string defaultValue = null)
        {
            XAttribute attribute2 = element.Attribute(attribute);
            if (attribute2 != null)
            {
                return attribute2.Value;
            }
            return defaultValue;
        }

        internal static ushort GetAttributeValueOrDefaultOfUInt16Type(this XElement element, string attribute, ushort defaultValue)
        {
            XAttribute attribute2 = element.Attribute((XName) attribute);
            if (attribute2 != null)
            {
                ushort num = defaultValue;
                ushort.TryParse(attribute2.Value, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num);
                return num;
            }
            return defaultValue;
        }

        internal static ushort GetAttributeValueOrDefaultOfUInt16Type(this XElement element, XName attribute, ushort defaultValue)
        {
            XAttribute attribute2 = element.Attribute(attribute);
            if (attribute2 != null)
            {
                ushort num = defaultValue;
                ushort.TryParse(attribute2.Value, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num);
                return num;
            }
            return defaultValue;
        }

        internal static uint GetAttributeValueOrDefaultOfUInt32Type(this XElement element, XName attribute, uint defaultValue)
        {
            XAttribute attribute2 = element.Attribute(attribute);
            if (attribute2 != null)
            {
                uint num = defaultValue;
                uint.TryParse(attribute2.Value, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num);
                return num;
            }
            return defaultValue;
        }

        internal static uint GetAttributeValueOrDefaultOfUInt32Type(this XElement element, string attribute, uint defaultValue, bool isHexNumber = false)
        {
            XAttribute attribute2 = element.Attribute((XName) attribute);
            if (attribute2 == null)
            {
                return defaultValue;
            }
            if (!isHexNumber)
            {
                uint num = defaultValue;
                uint.TryParse(attribute2.Value, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num);
                return num;
            }
            uint num2 = defaultValue;
            uint.TryParse(attribute2.Value, (NumberStyles) NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num2);
            return num2;
        }

        internal static object GetChildElementAttributeValueOrDefault<T>(this XElement element, string child, string attributeName)
        {
            foreach (XElement element2 in element.Elements())
            {
                if (element2.Name.LocalName == child)
                {
                    return element2.GetAttributeValueOrDefault<T>(attributeName, default(T));
                }
            }
            return default(T);
        }

        internal static string GetChildElementValue(this XElement element, string child)
        {
            foreach (XElement element2 in element.Elements())
            {
                if (element2.Name.LocalName == child)
                {
                    return element2.Value;
                }
            }
            return string.Empty;
        }

        internal static object GetChildElementValueOrDefault<T>(this XElement element, string child, T defaultValue)
        {
            string childElementValue = element.GetChildElementValue(child);
            if (string.IsNullOrWhiteSpace(childElementValue))
            {
                return defaultValue;
            }
            int index = Array.IndexOf<Type>(TypeExtension.KnownTypes, typeof(T));
            if (index < 0)
            {
                return defaultValue;
            }
            switch (TypeExtension.KnownCodes[index])
            {
                case TypeCode.Boolean:
                {
                    bool result = false;
                    bool.TryParse(childElementValue, out result);
                    if (childElementValue == "1")
                    {
                        result = true;
                    }
                    return (bool) result;
                }
                case TypeCode.Char:
                {
                    char ch = '\0';
                    char.TryParse(childElementValue, out ch);
                    return (char) ch;
                }
                case TypeCode.SByte:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Decimal:
                case TypeCode.DateTime:
                case (TypeCode.DateTime | TypeCode.Unknown):
                    return childElementValue;

                case TypeCode.Byte:
                {
                    byte num2 = 0;
                    byte.TryParse(childElementValue, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num2);
                    return (byte) num2;
                }
                case TypeCode.Int16:
                {
                    short num6 = -32768;
                    short.TryParse(childElementValue, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num6);
                    return (short) num6;
                }
                case TypeCode.UInt16:
                {
                    ushort num5 = 0;
                    ushort.TryParse(childElementValue, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num5);
                    return (ushort) num5;
                }
                case TypeCode.Int32:
                {
                    int num8 = -2147483648;
                    int.TryParse(childElementValue, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num8);
                    return (int) num8;
                }
                case TypeCode.UInt32:
                {
                    uint num7 = 0;
                    uint.TryParse(childElementValue, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num7);
                    return (uint) num7;
                }
                case TypeCode.Single:
                {
                    float naN = float.NaN;
                    float.TryParse(childElementValue, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out naN);
                    return (float) naN;
                }
                case TypeCode.Double:
                {
                    double num4 = double.NaN;
                    double.TryParse(childElementValue, (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture.NumberFormat, out num4);
                    return (double) num4;
                }
                case TypeCode.String:
                    return childElementValue;
            }
            return childElementValue;
        }

        internal static bool TryGetAttributeValue(this XElement element, string attribute, out string result)
        {
            XAttribute attribute2 = element.Attribute((XName) attribute);
            if (attribute2 != null)
            {
                result = attribute2.Value;
                return true;
            }
            result = null;
            return false;
        }

        internal static XElement TryGetChildElement(this XElement element, string childName)
        {
            foreach (XElement element2 in element.Elements())
            {
                if (element2.Name.LocalName == childName)
                {
                    return element2;
                }
            }
            return null;
        }
    }
}

