#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a data series with x, y, high, and low values.
    /// </summary>
    /// <remarks>
    /// The HighLowSeries class has four sets of data values (x-coordinates, y-coordinates, high values, and low values).
    /// </remarks>
    public class SpreadHighLowSeries : SpreadXYDataSeries
    {
        SpreadDataSeries _highSeries;
        SpreadDataSeries _lowSeries;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.SpreadHighLowSeries" /> class.
        /// </summary>
        public SpreadHighLowSeries()
        {
            base.Init();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.SpreadHighLowSeries" /> class.
        /// </summary>
        /// <param name="highSeries">The high series.</param>
        /// <param name="lowSeris">The low series.</param>
        public SpreadHighLowSeries(SpreadDataSeries highSeries, SpreadDataSeries lowSeris)
        {
            this.Init();
            this._highSeries = highSeries;
            this._lowSeries = lowSeris;
            this._highSeries.MarkerType = MarkerType.None;
            this._highSeries.Stroke = null;
            this._lowSeries.MarkerType = MarkerType.None;
            this._lowSeries.Stroke = null;
        }

        internal override void AfterReadXml()
        {
            base.AfterReadXml();
            if (this._highSeries != null)
            {
                this._highSeries.Chart = base.Chart;
            }
            else
            {
                this._highSeries = new SpreadDataSeries();
            }
            if (this._lowSeries != null)
            {
                this._lowSeries.Chart = base.Chart;
            }
            else
            {
                this._lowSeries = new SpreadDataSeries();
            }
        }

        internal override DataLabel CreateDataLabelOnNeeded(int pointIndex)
        {
            return null;
        }

        internal override DataMarker CreateDataMarkerOnNeeded(int pointIndex)
        {
            return null;
        }

        internal override DataPoint CreateDataPointOnNeeded(int pointIndex)
        {
            return null;
        }

        internal override void DataLabelSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ShowSeriesName")
            {
                this.HighSeries.DataLabelSettings.ShowSeriesName = base.DataLabelSettings.ShowSeriesName;
                this.LowSeries.DataLabelSettings.ShowSeriesName = base.DataLabelSettings.ShowSeriesName;
            }
            if (e.PropertyName == "ShowPercent")
            {
                this.HighSeries.DataLabelSettings.ShowPercent = base.DataLabelSettings.ShowPercent;
                this.LowSeries.DataLabelSettings.ShowPercent = base.DataLabelSettings.ShowPercent;
            }
            if (e.PropertyName == "ShowBubbleSize")
            {
                this.HighSeries.DataLabelSettings.ShowBubbleSize = base.DataLabelSettings.ShowBubbleSize;
                this.LowSeries.DataLabelSettings.ShowBubbleSize = base.DataLabelSettings.ShowBubbleSize;
            }
            if (e.PropertyName == "ShowValue")
            {
                this.HighSeries.DataLabelSettings.ShowValue = base.DataLabelSettings.ShowValue;
                this.LowSeries.DataLabelSettings.ShowValue = base.DataLabelSettings.ShowValue;
            }
            if (e.PropertyName == "ShowCategoryName")
            {
                this.HighSeries.DataLabelSettings.ShowCategoryName = base.DataLabelSettings.ShowCategoryName;
                this.LowSeries.DataLabelSettings.ShowCategoryName = base.DataLabelSettings.ShowCategoryName;
            }
            if (e.PropertyName == "Separator")
            {
                this.HighSeries.DataLabelSettings.Separator = base.DataLabelSettings.Separator;
                this.LowSeries.DataLabelSettings.Separator = base.DataLabelSettings.Separator;
            }
            base.DataLabelSettings_PropertyChanged(sender, e);
        }

        internal override List<List<SheetCellRange>> GetDataRanges()
        {
            List<List<SheetCellRange>> dataRanges = base.GetDataRanges();
            List<List<SheetCellRange>> list2 = this.HighSeries.GetDataRanges();
            List<List<SheetCellRange>> list3 = this.LowSeries.GetDataRanges();
            foreach (List<SheetCellRange> list4 in list2)
            {
                dataRanges.Add(list4);
            }
            foreach (List<SheetCellRange> list5 in list3)
            {
                dataRanges.Add(list5);
            }
            return dataRanges;
        }

        internal override void Init()
        {
            base.Init();
            this._highSeries = new SpreadDataSeries();
            this._highSeries.MarkerType = MarkerType.None;
            this._highSeries.Stroke = null;
            this._lowSeries = new SpreadDataSeries();
            this._lowSeries.MarkerType = MarkerType.None;
            this._lowSeries.Stroke = null;
        }

        internal override void OnAddColumnRange(int column, int columnCount)
        {
            base.OnAddColumnRange(column, columnCount);
            this.HighSeries.OnAddColumnRange(column, columnCount);
            this.LowSeries.OnAddColumnRange(column, columnCount);
        }

        internal override void OnAddRowRange(int row, int rowCount)
        {
            base.OnAddRowRange(row, rowCount);
            this.HighSeries.OnAddRowRange(row, rowCount);
            this.LowSeries.OnAddRowRange(row, rowCount);
        }

        internal override void OnChartChanged()
        {
            base.OnChartChanged();
            this.HighSeries.Chart = base.Chart;
            this.LowSeries.Chart = base.Chart;
        }

        internal override void OnDisposed()
        {
            base.OnDisposed();
            this.HighSeries.Dispose();
            this.LowSeries.Dispose();
        }

        internal override void OnRemoveColumnRange(int column, int columnCount)
        {
            base.OnRemoveColumnRange(column, columnCount);
            this.HighSeries.OnRemoveColumnRange(column, columnCount);
            this.LowSeries.OnRemoveColumnRange(column, columnCount);
        }

        internal override void OnRemoveRowRange(int row, int rowCount)
        {
            base.OnRemoveRowRange(row, rowCount);
            this.HighSeries.OnRemoveRowRange(row, rowCount);
            this.LowSeries.OnRemoveRowRange(row, rowCount);
        }

        internal override void ReadXmlInternal(XmlReader reader)
        {
            string str;
            base.ReadXmlInternal(reader);
            if ((reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element))) && ((str = reader.Name) != null))
            {
                if (str != "HighSeries")
                {
                    if (str != "LowSeries")
                    {
                        return;
                    }
                }
                else
                {
                    this._highSeries = (SpreadDataSeries) Serializer.DeserializeObj(typeof(SpreadDataSeries), reader);
                    return;
                }
                this._lowSeries = (SpreadDataSeries) Serializer.DeserializeObj(typeof(SpreadDataSeries), reader);
            }
        }

        internal override void RefreshValues()
        {
            base.RefreshValues();
            if ((this.HighSeries.Values != null) && (this.HighSeries.Values.DataSeries != null))
            {
                this.HighSeries.RefreshValues();
            }
            if ((this.LowSeries.Values != null) && (this.LowSeries.Values.DataSeries != null))
            {
                this.LowSeries.RefreshValues();
            }
        }

        internal override void UpdateReferences()
        {
            base.UpdateReferences();
            this.HighSeries.UpdateReferences();
            this.LowSeries.UpdateReferences();
        }

        internal override void WriteXmlInternal(XmlWriter writer)
        {
            base.WriteXmlInternal(writer);
            Serializer.SerializeObj(this.HighSeries, "HighSeries", writer);
            Serializer.SerializeObj(this.LowSeries, "LowSeries", writer);
        }

        /// <summary>
        /// Gets the high series.
        /// </summary>
        /// <value>
        /// The high series.
        /// </value>
        public SpreadDataSeries HighSeries
        {
            get { return  this._highSeries; }
            internal set { this._highSeries = value; }
        }

        /// <summary>
        /// Gets a value that indicates whether this instance is hidden.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is hidden; otherwise, <c>false</c>.
        /// </value>
        public override bool IsHidden
        {
            get
            {
                if (!base.IsHidden)
                {
                    return false;
                }
                return (this._highSeries.IsHidden && this._lowSeries.IsHidden);
            }
        }

        /// <summary>
        /// Gets the low series.
        /// </summary>
        /// <value>
        /// The low series.
        /// </value>
        public SpreadDataSeries LowSeries
        {
            get { return  this._lowSeries; }
            internal set { this._lowSeries = value; }
        }
    }
}

