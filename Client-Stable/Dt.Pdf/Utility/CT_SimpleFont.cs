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
    public class CT_SimpleFont
    {
        private float ascentField;
        private float bBox0Field;
        private float bBox1Field;
        private float bBox2Field;
        private float bBox3Field;
        private float capHeightField;
        private float descentField;
        private int flagsField;
        private float italicAngleField;
        private string nameField;
        private float stemVField;
        private CT_Width[] widthsField;

        /// <remarks />
        [XmlAttribute]
        public float Ascent
        {
            get { return  this.ascentField; }
            set { this.ascentField = value; }
        }

        /// <remarks />
        [XmlAttribute]
        public float BBox0
        {
            get { return  this.bBox0Field; }
            set { this.bBox0Field = value; }
        }
        
        /// <remarks />
        [XmlAttribute]
        public float BBox1
        {
            get { return  this.bBox1Field; }
            set { this.bBox1Field = value; }
        }
        
        /// <remarks />
        [XmlAttribute]
        public float BBox2
        {
            get { return  this.bBox2Field; }
            set { this.bBox2Field = value; }
        }
        
        /// <remarks />
        [XmlAttribute]
        public float BBox3
        {
            get { return  this.bBox3Field; }
            set { this.bBox3Field = value; }
        }
        
        /// <remarks />
        [XmlAttribute]
        public float CapHeight
        {
            get { return  this.capHeightField; }
            set { this.capHeightField = value; }
        }
        
        /// <remarks />
        [XmlAttribute]
        public float Descent
        {
            get { return  this.descentField; }
            set { this.descentField = value; }
        }
        
        /// <remarks />
        [XmlAttribute]
        public int Flags
        {
            get { return  this.flagsField; }
            set { this.flagsField = value; }
        }
        
        /// <remarks />
        [XmlAttribute]
        public float ItalicAngle
        {
            get { return  this.italicAngleField; }
            set { this.italicAngleField = value; }
        }
        
        /// <remarks />
        [XmlAttribute]
        public string Name
        {
            get { return  this.nameField; }
            set { this.nameField = value; }
        }

        /// <remarks />
        [XmlAttribute]
        public float StemV
        {
            get { return  this.stemVField; }
            set { this.stemVField = value; }
        }
        
        /// <remarks />
        [XmlElement("Widths")]
        public CT_Width[] Widths
        {
            get { return  this.widthsField; }
            set { this.widthsField = value; }
        }
    }
}

