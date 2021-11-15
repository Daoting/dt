#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;
#endregion

namespace Dt.Pdf.Utility
{
    /// <remarks />
    public class CT_Width
    {
        private int indexField;
        private float valueField;

        /// <remarks />
        [XmlAttribute]
        public int Index
        {
            get { return  this.indexField; }
            set { this.indexField = value; }
        }
        
        /// <remarks />
        [XmlAttribute]
        public float Value
        {
            get { return  this.valueField; }
            set { this.valueField = value; }
        }
    }
}

