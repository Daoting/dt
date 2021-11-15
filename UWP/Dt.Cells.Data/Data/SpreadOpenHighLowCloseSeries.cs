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
using Windows.UI;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents an open-high-low-close series in a Y plot area.
    /// </summary>
    /// <remarks>
    /// Each point contains four values: open, high, low, and close. Attributes for the up and down lines can be assigned for the series or for a point in the series with null (Nothing in VB) indicating unassigned. 
    /// Bar width is measured relative to the floor grid cell (with values in the range 0 to 1).
    /// </remarks>
    public class SpreadOpenHighLowCloseSeries : SpreadHighLowSeries
    {
        SpreadDataSeries _closeSeries;
        bool _isOpenEmpty;
        SpreadDataSeries _openSeries;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.SpreadOpenHighLowCloseSeries" /> class.
        /// </summary>
        public SpreadOpenHighLowCloseSeries()
        {
            base.Init();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.SpreadOpenHighLowCloseSeries" /> class.
        /// </summary>
        /// <param name="openSeries">The open series.</param>
        /// <param name="highSeries">The high series.</param>
        /// <param name="lowSeries">The low series.</param>
        /// <param name="closeSeries">The close series.</param>
        public SpreadOpenHighLowCloseSeries(SpreadDataSeries openSeries, SpreadDataSeries highSeries, SpreadDataSeries lowSeries, SpreadDataSeries closeSeries) : base(highSeries, lowSeries)
        {
            this._openSeries = openSeries;
            this._closeSeries = closeSeries;
            this._openSeries.MarkerType = MarkerType.None;
            this._openSeries.Stroke = null;
            this._closeSeries.MarkerType = MarkerType.None;
            this._closeSeries.Stroke = null;
            this._isOpenEmpty = false;
        }

        internal override void AfterReadXml()
        {
            base.AfterReadXml();
            this._openSeries.Chart = base.Chart;
            this._closeSeries.Chart = base.Chart;
        }

        internal override DataLabel CreateDataLabelOnNeeded(int pointIndex)
        {
            throw new NotSupportedException("HightLowOpenClose chart does not support data lable settings");
        }

        internal override DataMarker CreateDataMarkerOnNeeded(int pointIndex)
        {
            throw new NotSupportedException("HightLowOpenClose chart does not support data marker settings");
        }

        internal override DataPoint CreateDataPointOnNeeded(int pointIndex)
        {
            throw new NotSupportedException("HightLowOpenClose chart does not support data point settings");
        }

        internal override void DataLabelSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ShowSeriesName")
            {
                this.OpenSeries.DataLabelSettings.ShowSeriesName = base.DataLabelSettings.ShowSeriesName;
                this.CloseSeries.DataLabelSettings.ShowSeriesName = base.DataLabelSettings.ShowSeriesName;
            }
            if (e.PropertyName == "ShowPercent")
            {
                this.OpenSeries.DataLabelSettings.ShowPercent = base.DataLabelSettings.ShowPercent;
                this.CloseSeries.DataLabelSettings.ShowPercent = base.DataLabelSettings.ShowPercent;
            }
            if (e.PropertyName == "ShowBubbleSize")
            {
                this.OpenSeries.DataLabelSettings.ShowBubbleSize = base.DataLabelSettings.ShowBubbleSize;
                this.CloseSeries.DataLabelSettings.ShowBubbleSize = base.DataLabelSettings.ShowBubbleSize;
            }
            if (e.PropertyName == "ShowValue")
            {
                this.OpenSeries.DataLabelSettings.ShowValue = base.DataLabelSettings.ShowValue;
                this.CloseSeries.DataLabelSettings.ShowValue = base.DataLabelSettings.ShowValue;
            }
            if (e.PropertyName == "ShowCategoryName")
            {
                this.OpenSeries.DataLabelSettings.ShowCategoryName = base.DataLabelSettings.ShowCategoryName;
                this.CloseSeries.DataLabelSettings.ShowCategoryName = base.DataLabelSettings.ShowCategoryName;
            }
            if (e.PropertyName == "Separator")
            {
                this.OpenSeries.DataLabelSettings.Separator = base.DataLabelSettings.Separator;
                this.CloseSeries.DataLabelSettings.Separator = base.DataLabelSettings.Separator;
            }
            base.DataLabelSettings_PropertyChanged(sender, e);
        }

        internal override List<List<SheetCellRange>> GetDataRanges()
        {
            List<List<SheetCellRange>> list = new List<List<SheetCellRange>>();
            if ((base.XValuesRange != null) && (base.XValuesRange.Length > 0))
            {
                list.Add(new List<SheetCellRange>(base.XValuesRange));
            }
            foreach (List<SheetCellRange> list3 in this.OpenSeries.GetDataRanges())
            {
                list.Add(list3);
            }
            foreach (List<SheetCellRange> list5 in base.HighSeries.GetDataRanges())
            {
                list.Add(list5);
            }
            foreach (List<SheetCellRange> list7 in base.LowSeries.GetDataRanges())
            {
                list.Add(list7);
            }
            foreach (List<SheetCellRange> list9 in this.CloseSeries.GetDataRanges())
            {
                list.Add(list9);
            }
            return list;
        }

        internal override void Init()
        {
            base.Init();
            this._openSeries = new SpreadDataSeries();
            this._openSeries.MarkerType = MarkerType.None;
            this._openSeries.Stroke = null;
            this._closeSeries = new SpreadDataSeries();
            this._closeSeries.MarkerType = MarkerType.None;
            this._closeSeries.Stroke = null;
            this._isOpenEmpty = false;
        }

        internal override void OnAddColumnRange(int column, int columnCount)
        {
            base.OnAddColumnRange(column, columnCount);
            this.OpenSeries.OnAddColumnRange(column, columnCount);
            this.CloseSeries.OnAddColumnRange(column, columnCount);
        }

        internal override void OnAddRowRange(int row, int rowCount)
        {
            base.OnAddRowRange(row, rowCount);
            this.OpenSeries.OnAddRowRange(row, rowCount);
            this.CloseSeries.OnAddRowRange(row, rowCount);
        }

        internal override void OnChartChanged()
        {
            base.OnChartChanged();
            this.OpenSeries.Chart = base.Chart;
            this.CloseSeries.Chart = base.Chart;
        }

        internal override void OnDisposed()
        {
            base.OnDisposed();
            this.OpenSeries.Dispose();
            this.CloseSeries.Dispose();
        }

        internal override void OnRemoveColumnRange(int column, int columnCount)
        {
            base.OnRemoveColumnRange(column, columnCount);
            this.OpenSeries.OnRemoveColumnRange(column, columnCount);
            this.CloseSeries.OnRemoveColumnRange(column, columnCount);
        }

        internal override void OnRemoveRowRange(int row, int rowCount)
        {
            base.OnRemoveRowRange(row, rowCount);
            this.OpenSeries.OnRemoveRowRange(row, rowCount);
            this.CloseSeries.OnRemoveRowRange(row, rowCount);
        }

        internal override void ReadXmlInternal(XmlReader reader)
        {
            string str;
            base.ReadXmlInternal(reader);
            if ((reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element))) && ((str = reader.Name) != null))
            {
                if (str != "OpenSeries")
                {
                    if (str != "CloseSeries")
                    {
                        return;
                    }
                }
                else
                {
                    this._openSeries = (SpreadDataSeries) Serializer.DeserializeObj(typeof(SpreadDataSeries), reader);
                    return;
                }
                this._closeSeries = (SpreadDataSeries) Serializer.DeserializeObj(typeof(SpreadDataSeries), reader);
            }
        }

        internal override void RefreshValues()
        {
            base.RefreshValues();
            if ((this.OpenSeries.Values != null) && (this.OpenSeries.Values.DataSeries != null))
            {
                this.OpenSeries.RefreshValues();
            }
            if ((this.CloseSeries.Values != null) && (this.CloseSeries.Values.DataSeries != null))
            {
                this.CloseSeries.RefreshValues();
            }
        }

        internal override void UpdateReferences()
        {
            base.UpdateReferences();
            this.OpenSeries.UpdateReferences();
            this.CloseSeries.UpdateReferences();
        }

        internal override void WriteXmlInternal(XmlWriter writer)
        {
            base.WriteXmlInternal(writer);
            Serializer.SerializeObj(this.OpenSeries, "OpenSeries", writer);
            Serializer.SerializeObj(this.CloseSeries, "CloseSeries", writer);
        }

        /// <summary>
        /// Gets the fill brush that is created automatically.
        /// </summary>
        public override Brush AutomaticFill
        {
            get
            {
                return new SolidColorBrush(Colors.Black);
            }
        }

        /// <summary>
        /// Gets the close series.
        /// </summary>
        /// <value>
        /// The close series.
        /// </value>
        public SpreadDataSeries CloseSeries
        {
            get { return  this._closeSeries; }
            internal set { this._closeSeries = value; }
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
                return (this._openSeries.IsHidden && this._closeSeries.IsHidden);
            }
        }

        internal bool IsOpenSeriesAutomatic
        {
            get { return  this._isOpenEmpty; }
            set { this._isOpenEmpty = value; }
        }

        /// <summary>
        /// Gets the open series.
        /// </summary>
        /// <value>
        /// The open series.
        /// </value>
        public SpreadDataSeries OpenSeries
        {
            get { return  this._openSeries; }
            internal set
            {
                this._openSeries = value;
                this.IsOpenSeriesAutomatic = false;
            }
        }
    }
}

