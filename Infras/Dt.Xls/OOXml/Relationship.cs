#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
#endregion

namespace Dt.Xls.OOXml
{
    /// <summary>
    /// 
    /// </summary>
    [XmlType(Namespace="http://schemas.openxmlformats.org/package/2006/relationships"), XmlRoot("Relationship", Namespace="http://schemas.openxmlformats.org/package/2006/relationships", IsNullable=false)]
    public class Relationship
    {
        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute(DataType="ID")]
        public string Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute(DataType="anyURI")]
        public string Target { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlText]
        public string TargetMode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute(DataType="anyURI")]
        public string Type { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [XmlText]
        public string Value { get; set; }
    }
}

