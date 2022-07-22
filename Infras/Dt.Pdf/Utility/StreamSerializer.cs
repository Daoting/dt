#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf;
using System;
using System.IO;
using System.Xml.Serialization;
using System.Reflection;
#endregion

namespace Dt.Pdf.Utility
{
    /// <summary>
    /// stream serializer
    /// </summary>
    internal class StreamSerializer : IStreamSerializer
    {
        private System.Type _type;
        private System.Xml.Serialization.XmlSerializer _xmlSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:StreamSerializer" /> class.
        /// </summary>
        /// <param name="type">type</param>
        public StreamSerializer(System.Type type)
        {
            this._type = type;
        }

        /// <summary>
        /// Deserializes the specified stream.
        /// </summary>
        /// <param name="stream">stream</param>
        /// <returns>deserialized object</returns>
        public object Deserialize(Stream stream)
        {
            if ((stream != null) && stream.CanRead)
            {
                System.Xml.Serialization.XmlSerializer xmlSerializer = this.XmlSerializer;
                if (xmlSerializer != null)
                {
                    try
                    {
                        object instance = this.DoDeserialize(xmlSerializer, stream);
                        if ((instance != null) && IsInstanceOfType(instance, Type))
                        {
                            return instance;
                        }
                    }
                    catch
                    {
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Does the deserialize.
        /// </summary>
        /// <param name="serializer">serializer</param>
        /// <param name="stream">stream</param>
        /// <returns></returns>
        protected virtual object DoDeserialize(System.Xml.Serialization.XmlSerializer serializer, Stream stream)
        {
            return serializer.Deserialize(stream);
        }

        /// <summary>
        /// Does the serialize.
        /// </summary>
        /// <param name="serializer">serializer</param>
        /// <param name="stream">stream</param>
        /// <param name="obj">obj</param>
        protected virtual void DoSerialize(System.Xml.Serialization.XmlSerializer serializer, Stream stream, object obj)
        {
            serializer.Serialize(stream, obj);
        }

        /// <summary>
        /// Serializes the specified stream.
        /// </summary>
        /// <param name="stream">stream</param>
        /// <param name="obj">obj</param>
        public void Serialize(Stream stream, object obj)
        {
            if (((obj != null) && (stream != null)) && stream.CanWrite)
            {
                System.Xml.Serialization.XmlSerializer xmlSerializer = this.XmlSerializer;
                if ((xmlSerializer != null) && IsInstanceOfType(obj, Type))
                {
                    this.DoSerialize(xmlSerializer, stream, obj);
                }
            }
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public System.Type Type
        {
            get { return  this._type; }
        }

        /// <summary>
        /// Gets the XML serializer.
        /// </summary>
        /// <value>The XML serializer.</value>
        protected System.Xml.Serialization.XmlSerializer XmlSerializer
        {
            get
            {
                if (this._xmlSerializer == null)
                {
                    this._xmlSerializer = new System.Xml.Serialization.XmlSerializer(this.Type);
                }
                return this._xmlSerializer;
            }
        }

        public static bool IsInstanceOfType(object p_obj, Type p_type)
        {
            return (!object.ReferenceEquals(p_obj, null)
                && !object.ReferenceEquals(p_type, null)
                && p_obj.GetType().GetTypeInfo().IsAssignableFrom(p_type.GetTypeInfo()));
        }
    }
}