#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine;
using Dt.CalcEngine.Expressions;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents the Sparkline object.
    /// </summary>
    public class Sparkline : IXmlSerializable, IThemeContextSupport
    {
        string cachedFormula;
        ISparklineData data;
        Dt.Cells.Data.DataOrientation dataOrientation;
        CalcExpression dataReference;
        SparklineGroup group;
        bool isDataSet;

        /// <summary>
        /// Occurs when the sparkline has changed.
        /// </summary>
        public event EventHandler SparklineChanged;

        /// <summary>
        /// This is only used for XML serialization. Do not use it in other cases.
        /// </summary>
        public Sparkline()
        {
            this.Init();
        }

        internal Sparkline(int row, int column, CalcExpression dataReference, Dt.Cells.Data.DataOrientation dataOrientation, Dt.Cells.Data.SparklineType type, SparklineSetting setting)
        {
            this.Row = row;
            this.Column = column;
            this.DataReference = dataReference;
            this.DataOrientation = dataOrientation;
            this.group = new SparklineGroup();
            this.group.Add(this);
            this.SparklineType = type;
            this.Setting = setting;
        }

        internal Sparkline(int row, int column, CalcExpression dataReference, Dt.Cells.Data.DataOrientation dataOrientation, Dt.Cells.Data.SparklineType type, CalcExpression dateAxisReference, Dt.Cells.Data.DataOrientation dateAxisOrientation, SparklineSetting setting)
        {
            this.Row = row;
            this.Column = column;
            this.DataReference = dataReference;
            this.DataOrientation = dataOrientation;
            this.group = new SparklineGroup();
            this.group.Add(this);
            this.SparklineType = type;
            this.Setting = setting;
            this.DateAxisReference = dateAxisReference;
            this.DateAxisOrientation = dateAxisOrientation;
        }

        internal Sparkline Clone()
        {
            Sparkline sparkline = new Sparkline {
                Row = this.Row,
                Column = this.Column
            };
            if (this.isDataSet)
            {
                sparkline.Data = this.Data;
            }
            sparkline.DataOrientation = this.DataOrientation;
            sparkline.DataReference = this.DataReference;
            sparkline.Group = this.group.Clone();
            return sparkline;
        }

        void data_DataChanged(object sender, EventArgs e)
        {
            this.OnSparklineChanged();
        }

        internal string GetDataFormula(CalcParserContext context)
        {
            if (this.dataReference == null)
            {
                return null;
            }
            if ((this.Group.SparklineGroupManager != null) && (this.Group.SparklineGroupManager.CalcEvaluator != null))
            {
                return this.Group.SparklineGroupManager.CalcEvaluator.Expression2Formula(this.DataReference, 0, 0);
            }
            CalcParser parser = new CalcParser();
            return parser.Unparse(this.dataReference, context);
        }

        /// <summary>
        /// Gets the theme context.
        /// </summary>
        /// <returns>Returns the theme context.</returns>
        IThemeSupport IThemeContextSupport.GetContext()
        {
            IThemeContextSupport setting = this.Setting;
            if (setting != null)
            {
                return setting.GetContext();
            }
            return null;
        }

        /// <summary>
        /// Sets the theme context.
        /// </summary>
        /// <param name="context">The theme context object.</param>
        void IThemeContextSupport.SetContext(IThemeSupport context)
        {
            IThemeContextSupport setting = this.Setting;
            if ((setting != null) && (setting.GetContext() != context))
            {
                setting.SetContext(context);
            }
        }

        internal void Init()
        {
            this.data = null;
            this.group = null;
            this.dataReference = null;
            this.dataOrientation = Dt.Cells.Data.DataOrientation.Vertical;
            this.Row = 0;
            this.Column = 0;
            this.cachedFormula = null;
        }

        internal void OnSparklineChanged()
        {
            SheetSparklineDataProvider data = this.Data as SheetSparklineDataProvider;
            if (data != null)
            {
                data.ClearCache();
            }
            if (this.SparklineChanged != null)
            {
                this.SparklineChanged(this, EventArgs.Empty);
            }
        }

        internal void ResumeAfterDeserialization()
        {
            if ((this.Group.SparklineGroupManager != null) && (this.Group.SparklineGroupManager.CalcEvaluator != null))
            {
                this.DataReference = this.Group.SparklineGroupManager.CalcEvaluator.Formula2Expression(this.cachedFormula, 0, 0) as CalcReferenceExpression;
            }
        }

        /// <summary>
        /// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute" /> to the class.
        /// </summary>
        /// <returns>A <see cref="T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" /> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)" /> method.</returns>
        /// <remarks></remarks>
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized.</param>
        /// <remarks></remarks>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.Init();
            Serializer.InitReader(reader);
            this.Row = Serializer.ReadAttributeInt("r", reader).Value;
            this.Column = Serializer.ReadAttributeInt("c", reader).Value;
            while (reader.Read())
            {
                XmlReader reader3;
                string str;
                if ((reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element))) && ((str = reader.Name) != null))
                {
                    if (str == "DataOrientation")
                    {
                        XmlReader @this = Serializer.ExtractNode(reader);
                        this.DataOrientation = (Dt.Cells.Data.DataOrientation) Serializer.DeserializeObj(typeof(Dt.Cells.Data.DataOrientation), reader);
                        @this.Close();
                    }
                    else if (str == "Formula")
                    {
                        goto Label_009F;
                    }
                }
                continue;
            Label_009F:
                reader3 = Serializer.ExtractNode(reader);
                this.cachedFormula = (string) ((string) Serializer.DeserializeObj(typeof(string), reader3));
                reader3.Close();
            }
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized.</param>
        /// <remarks></remarks>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            Serializer.InitWriter(writer);
            Serializer.WriteAttribute("r", this.Row, writer);
            Serializer.WriteAttribute("c", this.Column, writer);
            if (this.dataOrientation != Dt.Cells.Data.DataOrientation.Vertical)
            {
                Serializer.SerializeObj(this.DataOrientation, "DataOrientation", writer);
            }
            string dataFormula = this.GetDataFormula(null);
            if (!string.IsNullOrEmpty(dataFormula))
            {
                Serializer.SerializeObj(dataFormula, "Formula", writer);
            }
        }

        /// <summary>
        /// Gets the column.
        /// </summary>
        public int Column { get; internal set; }

        /// <summary>
        /// Gets or sets the data object.
        /// </summary>
        /// <value>The data object which provides values to the sparkline.</value>
        public ISparklineData Data
        {
            get
            {
                if (this.data == null)
                {
                    this.data = new SheetSparklineDataProvider(this);
                    this.data.DataChanged += new EventHandler(this.data_DataChanged);
                }
                return this.data;
            }
            set
            {
                if (this.data != value)
                {
                    if (this.data != null)
                    {
                        this.data.DataChanged -= new EventHandler(this.data_DataChanged);
                    }
                    this.data = value;
                    if (this.data != null)
                    {
                        this.data.DataChanged += new EventHandler(this.data_DataChanged);
                        this.isDataSet = true;
                    }
                    else
                    {
                        this.isDataSet = false;
                    }
                    this.OnSparklineChanged();
                }
            }
        }

        /// <summary>
        /// Gets the data orientation.
        /// </summary>
        public Dt.Cells.Data.DataOrientation DataOrientation
        {
            get { return  this.dataOrientation; }
            private set
            {
                if (this.dataOrientation != value)
                {
                    this.dataOrientation = value;
                    this.OnSparklineChanged();
                }
            }
        }

        internal CalcExpression DataReference
        {
            get { return  this.dataReference; }
            set
            {
                if (this.dataReference != value)
                {
                    this.dataReference = value;
                    this.OnSparklineChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the date axis data object.
        /// </summary>
        /// <value>The date axis data which provides a date time value to the sparkline.</value>
        public ISparklineData DateAxisData
        {
            get { return  this.Group.DateAxisData; }
            set
            {
                if (this.Group.DateAxisData != value)
                {
                    this.Group.DateAxisData = value;
                    this.OnSparklineChanged();
                }
            }
        }

        /// <summary>
        /// Gets the date axis orientation.
        /// </summary>
        public Dt.Cells.Data.DataOrientation DateAxisOrientation
        {
            get { return  this.Group.DateAxisOrientation; }
            private set
            {
                if (this.Group.DateAxisOrientation != value)
                {
                    this.Group.DateAxisOrientation = value;
                }
            }
        }

        internal CalcExpression DateAxisReference
        {
            get { return  this.Group.DateAxisReference; }
            set
            {
                if (this.Group.DateAxisReference != value)
                {
                    this.Group.DateAxisReference = value;
                }
            }
        }

        /// <summary>
        /// Gets a value that indicates whether to display the date axis.
        /// </summary>
        public bool DisplayDateAxis
        {
            get { return  this.Group.DisplayDateAxis; }
        }

        /// <summary>
        /// Gets the sparkline group.
        /// </summary>
        public SparklineGroup Group
        {
            get
            {
                if (this.group == null)
                {
                    SparklineGroup group = new SparklineGroup();
                    group.Add(this);
                    this.group = group;
                }
                return this.group;
            }
            internal set
            {
                if (value != this.group)
                {
                    if (this.group != null)
                    {
                        this.group.Remove(this);
                    }
                    this.group = value;
                    if ((this.group != null) && !this.group.Contains(this))
                    {
                        this.group.Add(this);
                    }
                    this.OnSparklineChanged();
                }
            }
        }

        /// <summary>
        /// Gets the row.
        /// </summary>
        public int Row { get; internal set; }

        /// <summary>
        /// Gets the sparkline setting of the cell.
        /// </summary>
        public SparklineSetting Setting
        {
            get { return  this.Group.Setting; }
            set { this.Group.Setting = value; }
        }

        /// <summary>
        /// Gets or sets the type of the sparkline.
        /// </summary>
        /// <value>The type of the sparkline.</value>
        public Dt.Cells.Data.SparklineType SparklineType
        {
            get { return  this.Group.SparklineType; }
            set
            {
                if (this.Group.SparklineType != value)
                {
                    this.Group.SparklineType = value;
                    this.OnSparklineChanged();
                }
            }
        }
    }
}

