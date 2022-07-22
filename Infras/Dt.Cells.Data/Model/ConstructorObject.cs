#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a constructor object for XML serialization.
    /// </summary>
    internal class ConstructorObject : IXmlSerializable
    {
        object[] args;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.ConstructorObject" /> class.
        /// </summary>
        public ConstructorObject()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.ConstructorObject" /> class.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public ConstructorObject(params object[] args)
        {
            this.args = args;
        }

        /// <summary>
        /// This method is reserved and should not be used. When implementing the <see cref="T:System.Xml.Serialization.IXmlSerializable" /> interface, you should return a null reference (Nothing in Visual Basic) from this method.
        /// </summary>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        public void ReadXml(XmlReader reader)
        {
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        public void WriteXml(XmlWriter writer)
        {
        }

        /// <summary>
        /// Gets the arguments.
        /// </summary>
        /// <value>The arguments.</value>
        public object[] Arguments
        {
            get { return  this.args; }
        }
    }
}

