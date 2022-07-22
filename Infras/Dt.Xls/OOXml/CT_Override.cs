#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Xml.Serialization;
#endregion

namespace Dt.Xls.OOXml
{
    /// <summary>
    /// 
    /// </summary>
    [XmlRoot("Override", Namespace="http://schemas.openxmlformats.org/package/2006/content-types", IsNullable=false), XmlType(Namespace="http://schemas.openxmlformats.org/package/2006/content-types")]
    public class CT_Override
    {
        private string contentTypeField;
        private string partNameField;

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute]
        public string ContentType
        {
            get { return  this.contentTypeField; }
            set { this.contentTypeField = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute(DataType="anyURI")]
        public string PartName
        {
            get { return  this.partNameField; }
            set { this.partNameField = value; }
        }
    }
}

