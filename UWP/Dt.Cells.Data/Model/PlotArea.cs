#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents the plot areas for the chart.
    /// </summary>
    public class PlotArea : SpreadChartElement, IXmlSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.PlotArea" /> class.
        /// </summary>
        public PlotArea()
        {
        }

        internal PlotArea(SpreadChart owner) : base(owner)
        {
        }

        /// <summary>
        /// Gets the actual <see cref="T:Windows.UI.Xaml.Media.Brush" /> object that describes the background for the plot area.
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
        /// Gets the actual <see cref="T:Windows.UI.Xaml.Media.Brush" /> object that describes the outline for the plot area.
        /// </summary>
        public override Brush AutomaticStroke
        {
            get { return  null; }
        }

        internal SpreadChart Chart
        {
            get { return (((ISpreadChartElement)this).Chart as SpreadChart); }
            set { ((ISpreadChartElement)this).Chart = value; }
        }

        internal override Dt.Cells.Data.ChartArea ChartArea
        {
            get { return  Dt.Cells.Data.ChartArea.PlotArea; }
        }

        internal int Column { get; set; }

        internal int Row { get; set; }
    }
}

