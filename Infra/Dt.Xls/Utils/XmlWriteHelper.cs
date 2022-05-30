#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml;
#endregion

namespace Dt.Xls.Utils
{
    internal static class XmlWriteHelper
    {
        private static IDisposable CreatePairedAction(Action startAction, Action endAction)
        {
            startAction();
            return new PairdAction { EndAction = endAction };
        }

        private static IDisposable CreatePairedAction<T>(Action<T> startAction, T arg, Action endAction)
        {
            startAction(arg);
            return new PairdAction { EndAction = endAction };
        }

        private static IDisposable CreatePairedAction<T1, T2>(Action<T1, T2> startAction, T1 arg1, T2 arg2, Action endAction)
        {
            startAction(arg1, arg2);
            return new PairdAction { EndAction = endAction };
        }

        private static IDisposable CreatePairedAction<T1, T2, T3>(Action<T3, T1, T2> startAction, T1 arg1, T2 arg2, T3 arg3, Action endAction)
        {
            startAction(arg3, arg1, arg2);
            return new PairdAction { EndAction = endAction };
        }

        public static IDisposable WriteDocument(this XmlWriter This, bool? standalone = new bool?())
        {
            if (!standalone.HasValue)
            {
                return CreatePairedAction(new Action(This.WriteStartDocument), new Action(This.WriteEndDocument));
            }
            return CreatePairedAction<bool>(new Action<bool>(This.WriteStartDocument), standalone.Value, new Action(This.WriteEndDocument));
        }

        public static IDisposable WriteElement(this XmlWriter This, string name)
        {
            return CreatePairedAction<string>(new Action<string>(This.WriteStartElement), name, new Action(This.WriteEndElement));
        }

        public static IDisposable WriteElement(this XmlWriter This, string name, string ns)
        {
            return CreatePairedAction<string, string>(new Action<string, string>(This.WriteStartElement), name, ns, new Action(This.WriteEndElement));
        }

        public static IDisposable WriteElement(this XmlWriter This, string name, string ns, string prefix)
        {
            return CreatePairedAction<string, string, string>(new Action<string, string, string>(This.WriteStartElement), name, ns, prefix, new Action(This.WriteEndElement));
        }

        public static void WriteLeafElement(this XmlWriter This, string name)
        {
            This.WriteStartElement(name);
            This.WriteEndElement();
        }

        public static void WriteLeafElement(this XmlWriter This, string name, string ns, string prefix)
        {
            This.WriteStartElement(prefix, name, ns);
            This.WriteEndElement();
        }

        public static void WriteLeafElementWithAttribute(this XmlWriter This, string name, string attribute, string value)
        {
            This.WriteStartElement(name);
            This.WriteAttributeString(attribute, value.ToSpecialEncodeForXML());
            This.WriteEndElement();
        }

        public static void WriteLeafElementWithAttribute(this XmlWriter This, string name, string ns, string prefix, string attribute, string value)
        {
            This.WriteStartElement(prefix, name, ns);
            This.WriteAttributeString(attribute, value);
            This.WriteEndElement();
        }

        public static void WriteLeafElementWithAttributes(this XmlWriter This, string name, params KeyValuePair<string, string>[] attributes)
        {
            This.WriteStartElement(name);
            if (attributes != null)
            {
                foreach (KeyValuePair<string, string> pair in attributes)
                {
                    string localName = pair.Key;
                    This.WriteAttributeString(localName, pair.Value);
                }
            }
            This.WriteEndElement();
        }

        public static void WriteLeafElementWithAttributes(this XmlWriter This, string name, string ns, string prefix, params KeyValuePair<string, string>[] attributes)
        {
            This.WriteStartElement(prefix, name, ns);
            if (attributes != null)
            {
                foreach (KeyValuePair<string, string> pair in attributes)
                {
                    string localName = pair.Key;
                    This.WriteAttributeString(localName, pair.Value);
                }
            }
            This.WriteEndElement();
        }

        public static void WriteLeafElementWithPrefixedAttribute(this XmlWriter This, string name, string ns, string prefix, string attribute, string value)
        {
            This.WriteStartElement(name);
            This.WriteAttributeString(prefix, attribute, ns, value);
            This.WriteEndElement();
        }

        public static void WriteStringExt(this XmlWriter This, string str, IExcelWriter writer)
        {
            try
            {
                if (str.Length <= 0x7fff)
                {
                    This.WriteString(str);
                }
                else
                {
                    This.WriteString(str.Substring(0, 0x7fff));
                }
            }
            catch (Exception exception)
            {
                writer.OnExcelSaveError(new ExcelWarning(exception.Message, ExcelWarningCode.General, -1, -1, -1, exception));
            }
        }

        private class PairdAction : IDisposable
        {
            void IDisposable.Dispose()
            {
                if (this.EndAction != null)
                {
                    this.EndAction();
                }
            }

            public Action EndAction { get; set; }
        }
    }
}

