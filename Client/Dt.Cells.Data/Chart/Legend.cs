#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls.Chart;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a legend area in the chart. 
    /// </summary>
    public class Legend : SpreadChartTextElement
    {
        LegendAlignment _alignment;
        Windows.UI.Xaml.Controls.Orientation _orientation;
        bool _overlapChart;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.Legend" /> class.
        /// </summary>
        public Legend()
        {
            this._alignment = LegendAlignment.MiddleRight;
        }

        internal Legend(SpreadChart owner) : base(owner)
        {
            this._alignment = LegendAlignment.MiddleRight;
        }

        void FormatInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ((ISpreadChartElement) this).NotifyElementChanged(e.PropertyName);
        }

        internal override void ReadXmlInternal(XmlReader reader)
        {
            string str;
            base.ReadXmlInternal(reader);
            if ((reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element))) && ((str = reader.Name) != null))
            {
                if (str != "Orientation")
                {
                    if (str != "Alignment")
                    {
                        if (str == "OverlapChart")
                        {
                            this._overlapChart = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                        }
                        return;
                    }
                }
                else
                {
                    this._orientation = (Windows.UI.Xaml.Controls.Orientation) Serializer.DeserializeObj(typeof(Windows.UI.Xaml.Controls.Orientation), reader);
                    return;
                }
                this._alignment = (LegendAlignment) Serializer.DeserializeObj(typeof(LegendAlignment), reader);
            }
        }

        internal override void WriteXmlInternal(XmlWriter writer)
        {
            base.WriteXmlInternal(writer);
            if (this._orientation != Windows.UI.Xaml.Controls.Orientation.Vertical)
            {
                Serializer.SerializeObj(this._orientation, "Orientation", writer);
            }
            if (this._alignment != LegendAlignment.MiddleRight)
            {
                Serializer.SerializeObj(this._alignment, "Alignment", writer);
            }
            if (this._overlapChart)
            {
                Serializer.SerializeObj((bool) this._overlapChart, "OverlapChart", writer);
            }
        }

        /// <summary>
        /// Gets the actual <see cref="T:Windows.UI.Xaml.Media.Brush" /> object that describes the background for the legend.
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
        /// Gets or sets the alignment for the legend.
        /// </summary>
        public LegendAlignment Alignment
        {
            get { return  this._alignment; }
            set
            {
                if (value != this.Alignment)
                {
                    this._alignment = value;
                    ((ISpreadChartElement) this).NotifyElementChanged("Aligment");
                }
            }
        }

        /// <summary>
        /// Gets the actual <see cref="T:Windows.UI.Xaml.Media.Brush" /> object that describes the outline for the legend.
        /// </summary>
        public override Brush AutomaticFill
        {
            get
            {
                return new SolidColorBrush(Colors.Transparent);
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="T:Windows.UI.Xaml.Media.Brush" /> object that describes the automatic outline for the legend.
        /// </summary>
        /// <value>The <see cref="T:Windows.UI.Xaml.Media.Brush" /> object that describes the automatic outline for the legend.</value>
        public override Brush AutomaticStroke
        {
            get
            {
                return new SolidColorBrush(Colors.Transparent);
            }
        }

        internal SpreadChart Chart
        {
            get { return  (this.Chart as SpreadChart); }
            set { this.Chart = value; }
        }

        internal override Dt.Cells.Data.ChartArea ChartArea
        {
            get { return  Dt.Cells.Data.ChartArea.Lengend; }
        }

        internal Dt.Cells.Data.Layout Layout { get; set; }

        internal List<IExcelLegendEntry> LegendEntries { get; set; }

        /// <summary>
        /// Gets or sets the layout orientation for the legend.
        /// </summary>
        public Windows.UI.Xaml.Controls.Orientation Orientation
        {
            get { return  this._orientation; }
            set
            {
                if (value != this.Orientation)
                {
                    this._orientation = value;
                    ((ISpreadChartElement) this).NotifyElementChanged("Orientation");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [overlap chart].
        /// </summary>
        /// <value>
        /// <c>true</c> if [overlap chart]; otherwise, <c>false</c>.
        /// </value>
        internal bool OverlapChart
        {
            get { return  this._overlapChart; }
            set
            {
                if (value != this.OverlapChart)
                {
                    this._overlapChart = value;
                    ((ISpreadChartElement) this).NotifyElementChanged("OverlapChart");
                }
            }
        }

        internal IExcelTextFormat TextFormat { get; set; }
    }
}

