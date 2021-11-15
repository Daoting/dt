#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml;
using Windows.UI;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents an element that specifies how the title of a chart is painted.
    /// </summary>
    public class ChartTitle : SpreadChartTextElement
    {
        Dt.Cells.Data.TitleType _titleType;
        string DefaultAxisTitle;
        string DefaultChartTitle;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.ChartTitle" /> class.
        /// </summary>
        public ChartTitle()
        {
            this.DefaultAxisTitle = ResourceStrings.DefaultAxisTitle;
            this.DefaultChartTitle = ResourceStrings.DefaultChartTitle;
        }

        internal ChartTitle(SpreadChartBase owner) : base(owner)
        {
            this.DefaultAxisTitle = ResourceStrings.DefaultAxisTitle;
            this.DefaultChartTitle = ResourceStrings.DefaultChartTitle;
        }

        void FormatInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ((ISpreadChartElement) this).NotifyElementChanged(e.PropertyName);
        }

        internal override void ReadXmlInternal(XmlReader reader)
        {
            string str;
            base.ReadXmlInternal(reader);
            if (((reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element))) && ((str = reader.Name) != null)) && (str == "TitleType"))
            {
                this._titleType = (Dt.Cells.Data.TitleType) Serializer.DeserializeObj(typeof(Dt.Cells.Data.TitleType), reader);
            }
        }

        internal override void WriteXmlInternal(XmlWriter writer)
        {
            base.WriteXmlInternal(writer);
            Serializer.SerializeObj(this._titleType, "TitleType", writer);
        }

        /// <summary>
        /// Gets the actual brush that specifies how the title's interior is painted.
        /// </summary>
        public override Brush ActualFill
        {
            get
            {
                Brush actualFill = base.ActualFill;
                if ((actualFill == null) && (this.Chart != null))
                {
                    return this.Chart.ActualFill;
                }
                return actualFill;
            }
        }

        /// <summary>
        /// Gets the automatically generated brush that specifies how the title's interior is painted.
        /// </summary>
        public override Brush AutomaticFill
        {
            get
            {
                return new SolidColorBrush(Colors.Transparent);
            }
        }

        /// <summary>
        /// Gets the automatically generated brush that specifies how the title outline is painted.
        /// </summary>
        public override Brush AutomaticStroke
        {
            get
            {
                return new SolidColorBrush(Colors.Transparent);
            }
        }

        internal SpreadChart Chart
        {
            get { return  (base.ChartBase as SpreadChart); }
            set { base.ChartBase = value; }
        }

        internal override Dt.Cells.Data.ChartArea ChartArea
        {
            get
            {
                if (this.TitleType == Dt.Cells.Data.TitleType.AxisTitle)
                {
                    return Dt.Cells.Data.ChartArea.AxisTitle;
                }
                return Dt.Cells.Data.ChartArea.ChartTitle;
            }
        }

        internal Dt.Cells.Data.Layout Layout { get; set; }

        internal bool Overlay { get; set; }

        /// <summary>
        /// Gets or sets the type of the title.
        /// </summary>
        /// <value>
        /// The type of the title.
        /// </value>
        public Dt.Cells.Data.TitleType TitleType
        {
            get { return  this._titleType; }
            internal set
            {
                if (value != this.TitleType)
                {
                    this._titleType = value;
                }
            }
        }
    }
}

