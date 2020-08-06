#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine.Expressions;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// 
    /// </summary>
    internal class Watermark : IXmlSerializable
    {
        GcLabel innerLabel;
        LayoutType layoutType;
        Brush outlineFillEffect;
        int outlineWidth;
        bool showOutline;

        public Watermark()
        {
            this.Init();
        }

        public ContentAlignment Alignment
        {
            get { return  this.innerLabel.Alignment; }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue((string)null)]
        public Brush Background
        {
            get { return  this.innerLabel.Background; }
            set { this.innerLabel.Background = value; }
        }

        public Font Font
        {
            get { return  this.innerLabel.Font; }
            set { this.innerLabel.Font = value; }
        }

        public Brush Foreground
        {
            get { return  this.innerLabel.Foreground; }
            set { this.innerLabel.Foreground = value; }
        }

        internal int Height
        {
            get { return  this.innerLabel.Height; }
            set { this.innerLabel.Height = value; }
        }

        internal bool IsEmpty
        {
            get { return  string.IsNullOrEmpty(this.Text); }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(1)]
        public LayoutType Layout
        {
            get { return  this.layoutType; }
            set
            {
                if (this.layoutType != value)
                {
                    this.SetLayoutInner(value);
                }
                this.layoutType = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(1)]
        public int OutlineWidth
        {
            get { return  this.outlineWidth; }
            set { this.outlineWidth = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(true)]
        public bool ShowOutline
        {
            get { return  this.showOutline; }
            set { this.showOutline = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue("")]
        public string Text
        {
            get { return  this.innerLabel.Text; }
            set { this.innerLabel.Text = value; }
        }

        internal int Width
        {
            get { return  this.innerLabel.Width; }
            set { this.innerLabel.Width = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(null)]
        public Brush OutlineFillEffect
        {
            get { return  this.outlineFillEffect; }
            set { this.outlineFillEffect = value; }
        }

        internal GcBlock GetBlock(GcReportContext context)
        {
            return this.innerLabel.GetBlock(context);
        }

        internal void Init()
        {
            this.innerLabel = new GcLabel();
            this.innerLabel.Font = new Font(GcReportContext.defaultFont.FontFamilyName, 40.0);
            this.innerLabel.Foreground = new SolidColorBrush(Color.FromArgb(0x66, 0x7f, 0xc9, 0xff));
            this.innerLabel.Alignment.HorizontalAlignment = TextHorizontalAlignment.Center;
            this.innerLabel.Alignment.VerticalAlignment = TextVerticalAlignment.Center;
            this.innerLabel.Alignment.TextRotationAngle = 45.0;
            this.innerLabel.Alignment.TextOrientation = TextOrientation.TextRotateCustom;
            this.outlineFillEffect = FillEffects.Black;
            this.showOutline = true;
            this.layoutType = LayoutType.Diagonal;
            this.outlineWidth = 1;
        }

        protected virtual void ReadXmlBase(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (reader.NodeType == ((XmlNodeType)((int)XmlNodeType.None)))
            {
                reader.Read();
            }
            if (reader.NodeType == ((XmlNodeType)((int)XmlNodeType.XmlDeclaration)))
            {
                reader.Read();
            }
            this.SetLayoutInner(this.layoutType);
        }

        void SetLayoutInner(LayoutType layout)
        {
            switch (layout)
            {
                case LayoutType.Diagonal:
                    this.innerLabel.Alignment.TextRotationAngle = 45.0;
                    this.innerLabel.Alignment.TextOrientation = TextOrientation.TextRotateCustom;
                    return;

                case LayoutType.Horizontal:
                    this.innerLabel.Alignment.TextRotationAngle = 0.0;
                    this.innerLabel.Alignment.TextOrientation = TextOrientation.TextHorizontal;
                    return;
            }
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (reader.NodeType == ((XmlNodeType)((int)XmlNodeType.XmlDeclaration)))
            {
                reader.Read();
            }
            while (reader.Read())
            {
                if (reader.NodeType == ((XmlNodeType)((int)XmlNodeType.Element)))
                {
                    this.ReadXmlBase(reader);
                }
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            this.WriteXmlBase(writer);
        }

        protected virtual void WriteXmlBase(XmlWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
        }
        
        public enum LayoutType
        {
            Diagonal = 1,
            Horizontal = 2
        }

    }
}