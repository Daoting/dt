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
    [XmlType(Namespace="http://schemas.openxmlformats.org/package/2006/relationships"), XmlRoot("Relationships", Namespace="http://schemas.openxmlformats.org/package/2006/relationships", IsNullable=false)]
    public class CT_Relationships
    {
        private Dt.Xls.OOXml.Relationship[] relationshipField;

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("Relationship")]
        public Dt.Xls.OOXml.Relationship[] Relationship
        {
            get { return  this.relationshipField; }
            set { this.relationshipField = value; }
        }
    }
}

