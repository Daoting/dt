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
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a section that is used for displaying all worksheets in the workbook.
    /// </summary>
    internal class GcAllSheetSection : GcSection
    {
        bool flagName;
        Dt.Cells.Data.Workbook workbook;
        string workbookName;

        /// <summary>
        /// Creates a new section that contains all the worksheets for the workbook.
        /// </summary>
        public GcAllSheetSection()
        {
            this.Init();
        }

        /// <summary>
        /// Creates a new section that contains all the worksheets for the specified workbook.
        /// </summary>
        /// <param name="workbook">The <see cref="P:Dt.Cells.Data.GcAllSheetSection.Workbook" /> object.</param>
        public GcAllSheetSection(Dt.Cells.Data.Workbook workbook)
        {
            if (workbook == null)
            {
                throw new ArgumentNullException("workbook");
            }
            this.Init();
            this.workbook = workbook;
            this.workbookName = this.workbook.Name;
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Init()
        {
            base.Init();
            this.workbook = null;
            this.workbookName = string.Empty;
            this.flagName = false;
        }

        /// <summary>
        /// Reads the XML base.
        /// </summary>
        /// <param name="reader">The reader.</param>
        protected override void ReadXmlBase(XmlReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.None)))
            {
                reader.Read();
            }
            if (reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.XmlDeclaration)))
            {
                reader.Read();
            }
            if (reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element)))
            {
                switch (reader.Name)
                {
                    case "WorkbookName":
                        this.workbookName = Serializer.ReadAttribute("value", reader);
                        return;

                    case "FlagName":
                        this.flagName = Serializer.ReadAttributeBoolean("value", false, reader);
                        return;
                }
                base.ReadXmlBase(reader);
            }
        }

        /// <summary>
        /// Writes the XML base.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void WriteXmlBase(XmlWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            base.WriteXmlBase(writer);
            if (!this.flagName)
            {
                this.workbookName = (this.workbook == null) ? string.Empty : this.workbook.Name;
            }
        }

        /// <summary>
        /// Overrides the <see cref="T:Dt.Cells.Data.GcSection" /> property.
        /// </summary>
        /// <value>
        /// This property is always <c>false</c>.
        /// </value>
        /// <remarks>This property is read-only.</remarks>
        public override bool CanGrow
        {
            get { return  false; }
            set
            {
            }
        }

        /// <summary>
        /// Overrides the <see cref="T:Dt.Cells.Data.GcSection" /> property.
        /// </summary>
        /// <value>
        /// This property is always <c>false</c>.
        /// </value>
        /// <remarks>This property is read-only.</remarks>
        public override bool CanShrink
        {
            get { return  false; }
            set
            {
            }
        }

        /// <summary>
        /// Overrides the <see cref="T:Dt.Cells.Data.GcSection" /> property.
        /// </summary>
        /// <value>
        /// This property is always <c>null</c>.
        /// </value>
        /// <remarks>This property is read-only.</remarks>
        public override ControlCollection Controls
        {
            get { return  null; }
        }

        /// <summary>
        /// Internal only.
        /// Gets or sets a value indicating whether [flag name].
        /// </summary>
        /// <value><c>true</c> if [flag name]; otherwise, <c>false</c></value>
        internal bool FlagName
        {
            get { return  this.flagName; }
            set { this.flagName = value; }
        }

        /// <summary>
        /// Overrides the <see cref="T:Dt.Cells.Data.GcSection" /> property.
        /// </summary>
        /// <value>This property is always <c>-1</c>.</value>
        /// <remarks>This property is read-only.</remarks>
        public override int Height
        {
            get { return  -1; }
            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the workbook.
        /// </summary>
        /// <value>The <see cref="P:Dt.Cells.Data.GcAllSheetSection.Workbook" /> object.</value>
        public Dt.Cells.Data.Workbook Workbook
        {
            get { return  this.workbook; }
            set
            {
                this.workbookName = (value != null) ? value.Name : string.Empty;
                this.workbook = value;
            }
        }

        /// <summary>
        /// Internal only.
        /// Gets or sets the name of the workbook.
        /// </summary>
        /// <value>The name of the workbook</value>
        internal string WorkbookName
        {
            get { return  this.workbookName; }
            set { this.workbookName = value; }
        }
    }
}

