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
    [XmlRoot("Types", Namespace="http://schemas.openxmlformats.org/package/2006/content-types", IsNullable=false), XmlType(Namespace="http://schemas.openxmlformats.org/package/2006/content-types")]
    public class CT_Types
    {
        private object[] itemsField;

        /// <summary>
        /// 
        /// </summary>
        [XmlElement("Default", typeof(CT_Default)), XmlElement("Override", typeof(CT_Override))]
        public object[] Items
        {
            get { return  this.itemsField; }
            set { this.itemsField = value; }
        }
    }
}

