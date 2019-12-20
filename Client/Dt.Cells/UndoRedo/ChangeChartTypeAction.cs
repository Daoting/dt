#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using Dt.Cells.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Windows.Foundation;
#endregion

namespace Dt.Cells.UndoRedo
{
    /// <summary>
    /// 
    /// </summary>
    public class ChangeChartTypeAction : ActionBase, IUndo
    {
        private SpreadChart _chart;
        private SpreadChartType _newType;
        private SpreadChartType _oldType;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.UndoRedo.ChangeChartTypeAction" /> class.
        /// </summary>
        /// <param name="chart">The chart.</param>
        /// <param name="newType">The new type.</param>
        public ChangeChartTypeAction(SpreadChart chart, SpreadChartType newType)
        {
            this._chart = chart;
            this._oldType = chart.ChartType;
            this._newType = newType;
        }

        /// <summary>
        /// Defines the method that determines whether the action can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the action. If the action does not require data to be passed, this object can be set to null.</param>
        /// <returns>
        /// <c>true</c> if this action can be executed; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        private bool ChangeChartType(SpreadChartType fromChartType, SpreadChartType toChartType)
        {
            this._chart.SuspendEvents();
            bool flag = false;
            try
            {
                int dataDimension = Dt.Cells.Data.SpreadChartUtility.GetDataDimension(fromChartType);
                int num2 = Dt.Cells.Data.SpreadChartUtility.GetDataDimension(toChartType);
                bool flag2 = fromChartType.ToString().ToLower().Contains("bar");
                bool flag3 = toChartType.ToString().ToLower().Contains("bar");
                if (dataDimension != num2)
                {
                    if (toChartType == SpreadChartType.StockHighLowOpenClose)
                    {
                        flag = this.ChangeToStockChart(fromChartType, toChartType);
                    }
                    else if (fromChartType == SpreadChartType.StockHighLowOpenClose)
                    {
                        flag = this.ChangeFromStockChart(fromChartType, toChartType);
                    }
                    else if (num2 != 1)
                    {
                        flag = this.ChangeChartTypeToMulti(fromChartType, toChartType);
                    }
                    else if (num2 == 1)
                    {
                        flag = this.ChangeChartTypeOne(fromChartType, toChartType);
                    }
                }
                else
                {
                    if (flag2 != flag3)
                    {
                        AxisType axisType = this._chart.AxisY.AxisType;
                        this._chart.AxisY.AxisType = this._chart.AxisX.AxisType;
                        this._chart.AxisX.AxisType = axisType;
                        string itemsFormula = this._chart.AxisY.ItemsFormula;
                        this._chart.AxisY.ItemsFormula = this._chart.AxisX.ItemsFormula;
                        this._chart.AxisX.ItemsFormula = itemsFormula;
                    }
                    this._chart.ChartType = toChartType;
                }
                if (flag2 != flag3)
                {
                    if (flag3)
                    {
                        if (this._chart.AxisX != null)
                        {
                            this._chart.AxisX.ShowMajorGridlines = true;
                        }
                        if (this._chart.AxisY != null)
                        {
                            this._chart.AxisY.ShowMajorGridlines = false;
                        }
                    }
                    if (flag2)
                    {
                        if (this._chart.AxisY != null)
                        {
                            this._chart.AxisY.ShowMajorGridlines = true;
                        }
                        if (this._chart.AxisX != null)
                        {
                            this._chart.AxisX.ShowMajorGridlines = false;
                        }
                    }
                }
                if (!Dt.Cells.Data.SpreadChartUtility.IsChartWithMarker(toChartType))
                {
                    return flag;
                }
                foreach (SpreadDataSeries series in this._chart.DataSeries)
                {
                    if (series.MarkerType == MarkerType.None)
                    {
                        series.MarkerType = MarkerType.Automatic;
                    }
                    if ((series.MarkerType != MarkerType.None) && ((series.MarkerSize.IsEmpty || (series.MarkerSize.Width == 0.0)) || (series.MarkerSize.Height == 0.0)))
                    {
                        series.MarkerSize = new Windows.Foundation.Size(7.0, 7.0);
                    }
                }
            }
            finally
            {
                this._chart.ResumeEvents();
            }
            return flag;
        }

        private bool ChangeChartTypeOne(SpreadChartType fromChartType, SpreadChartType toChartType)
        {
            SpreadChartUtility.GetDataDimension(fromChartType);
            SpreadChartUtility.GetDataDimension(toChartType);
            string itemsFormula = this._chart.ItemsFormula;
            string xValueFormula = null;
            this._chart.ChartType = toChartType;
            this.ResetDataSeriesChartType();
            List<SpreadDataSeries> list = new List<SpreadDataSeries>();
            foreach (SpreadDataSeries series in this._chart.DataSeries)
            {
                MemoryStream stream = new MemoryStream();
                XmlWriter writer = XmlWriter.Create((Stream) stream);
                if (writer != null)
                {
                    Serializer.SerializeObj(series, "DataSeries", writer);
                    writer.Close();
                    stream.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
                    XmlReader @this = XmlReader.Create((Stream) stream);
                    if (@this != null)
                    {
                        SpreadDataSeries series2 = Dt.Cells.Data.ChangeChartTypeHelper.CreateDataSeries(toChartType);
                        ((IXmlSerializable) series2).ReadXml(@this);
                        if (string.IsNullOrEmpty(xValueFormula) && (series is SpreadXYDataSeries))
                        {
                            xValueFormula = (series as SpreadXYDataSeries).XValueFormula;
                        }
                        series2.ResetChartType();
                        @this.Close();
                        list.Add(series2);
                    }
                }
            }
            if (string.IsNullOrEmpty(itemsFormula) && (itemsFormula != xValueFormula))
            {
                if (toChartType.ToString().ToLower().Contains("bar"))
                {
                    this._chart.AxisY.ItemsFormula = xValueFormula;
                    this._chart.AxisX.ItemsFormula = null;
                }
                else
                {
                    this._chart.AxisX.ItemsFormula = xValueFormula;
                    this._chart.AxisY.ItemsFormula = null;
                }
            }
            if (list.Count > 0)
            {
                this._chart.DataSeries.Clear();
                this._chart.DataSeries.AddRange((IList<SpreadDataSeries>) list);
            }
            return true;
        }

        private bool ChangeChartTypeToMulti(SpreadChartType fromChartType, SpreadChartType toChartType)
        {
            Dt.Cells.Data.SpreadChartUtility.GetDataDimension(fromChartType);
            Dt.Cells.Data.SpreadChartUtility.GetDataDimension(toChartType);
            string itemsFormula = this._chart.ItemsFormula;
            this._chart.ChartType = toChartType;
            this.ResetDataSeriesChartType();
            List<SpreadDataSeries> list = new List<SpreadDataSeries>();
            foreach (SpreadDataSeries series in this._chart.DataSeries)
            {
                MemoryStream stream = new MemoryStream();
                XmlWriter writer = XmlWriter.Create((Stream) stream);
                if (writer != null)
                {
                    Serializer.SerializeObj(series, "DataSeries", writer);
                    writer.Close();
                    stream.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
                    XmlReader @this = XmlReader.Create((Stream) stream);
                    if (@this != null)
                    {
                        SpreadXYDataSeries series2 = Dt.Cells.Data.ChangeChartTypeHelper.CreateDataSeries(toChartType) as SpreadXYDataSeries;
                        ((IXmlSerializable) series2).ReadXml(@this);
                        if (!string.IsNullOrEmpty(itemsFormula))
                        {
                            series2.XValueFormula = itemsFormula;
                        }
                        series2.ResetChartType();
                        @this.Close();
                        list.Add(series2);
                    }
                }
            }
            this._chart.AxisX.ItemsFormula = null;
            this._chart.AxisY.ItemsFormula = null;
            if (list.Count > 0)
            {
                this._chart.DataSeries.Clear();
                this._chart.DataSeries.AddRange((IList<SpreadDataSeries>) list);
            }
            return true;
        }

        private bool ChangeFromStockChart(SpreadChartType fromChartType, SpreadChartType toChartType)
        {
            Dt.Cells.Data.SpreadChartUtility.GetDataDimension(fromChartType);
            int dataDimension = Dt.Cells.Data.SpreadChartUtility.GetDataDimension(toChartType);
            if ((this._chart.DataSeries.Count <= 0) || !(this._chart.DataSeries[0] is SpreadOpenHighLowCloseSeries))
            {
                return false;
            }
            string itemsFormula = this._chart.ItemsFormula;
            this.ResetDataSeriesChartType();
            this._chart.ChartType = toChartType;
            List<SpreadDataSeries> list = new List<SpreadDataSeries>();
            SpreadOpenHighLowCloseSeries series = this._chart.DataSeries[0] as SpreadOpenHighLowCloseSeries;
            for (int i = 0; i < 4; i++)
            {
                SpreadDataSeries openSeries = null;
                switch (i)
                {
                    case 0:
                        openSeries = series.OpenSeries;
                        break;

                    case 1:
                        openSeries = series.HighSeries;
                        break;

                    case 2:
                        openSeries = series.LowSeries;
                        break;

                    case 3:
                        openSeries = series.CloseSeries;
                        break;
                }
                if (openSeries.Stroke == null)
                {
                    openSeries.IsAutomaticStroke = true;
                }
                MemoryStream stream = new MemoryStream();
                XmlWriter writer = XmlWriter.Create((Stream) stream);
                if (writer != null)
                {
                    Serializer.SerializeObj(openSeries, "DataSeires", writer);
                    writer.Close();
                    stream.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
                    XmlReader @this = XmlReader.Create((Stream) stream);
                    if (@this != null)
                    {
                        SpreadDataSeries series3 = Dt.Cells.Data.ChangeChartTypeHelper.CreateDataSeries(toChartType);
                        ((IXmlSerializable) series3).ReadXml(@this);
                        if (series3 is SpreadXYDataSeries)
                        {
                            SpreadXYDataSeries series4 = series3 as SpreadXYDataSeries;
                            if (!string.IsNullOrEmpty(series.XValueFormula))
                            {
                                series4.XValueFormula = series.XValueFormula;
                            }
                            else if (series.XValues.Count > 0)
                            {
                                series4.XValues = series.XValues;
                            }
                        }
                        series3.ResetChartType();
                        @this.Close();
                        list.Add(series3);
                    }
                }
            }
            if (((dataDimension == 1) && string.IsNullOrEmpty(itemsFormula)) && (itemsFormula != series.XValueFormula))
            {
                if (toChartType.ToString().ToLower().Contains("bar"))
                {
                    this._chart.AxisY.ItemsFormula = series.XValueFormula;
                    this._chart.AxisX.ItemsFormula = null;
                }
                else
                {
                    this._chart.AxisX.ItemsFormula = series.XValueFormula;
                    this._chart.AxisY.ItemsFormula = null;
                }
            }
            if (list.Count > 0)
            {
                this._chart.DataSeries.Clear();
                this._chart.DataSeries.AddRange((IList<SpreadDataSeries>) list);
            }
            return true;
        }

        private bool ChangeToStockChart(SpreadChartType fromChartType, SpreadChartType toChartType)
        {
            Dt.Cells.Data.SpreadChartUtility.GetDataDimension(fromChartType);
            Dt.Cells.Data.SpreadChartUtility.GetDataDimension(toChartType);
            if (this._chart.DataSeries.Count < 4)
            {
                return false;
            }
            string itemsFormula = this._chart.ItemsFormula;
            this._chart.ChartType = toChartType;
            List<SpreadDataSeries> list = new List<SpreadDataSeries>();
            SpreadDataSeries openSeries = this._chart.DataSeries[0];
            SpreadDataSeries highSeries = this._chart.DataSeries[1];
            SpreadDataSeries lowSeries = this._chart.DataSeries[2];
            SpreadDataSeries closeSeries = this._chart.DataSeries[3];
            openSeries.Stroke = null;
            highSeries.Stroke = null;
            lowSeries.Stroke = null;
            closeSeries.Stroke = null;
            SpreadOpenHighLowCloseSeries series5 = new SpreadOpenHighLowCloseSeries(openSeries, highSeries, lowSeries, closeSeries);
            if (!string.IsNullOrEmpty(itemsFormula))
            {
                series5.XValueFormula = itemsFormula;
            }
            else if (openSeries is SpreadXYDataSeries)
            {
                SpreadXYDataSeries series6 = openSeries as SpreadXYDataSeries;
                if (!string.IsNullOrEmpty(series6.XValueFormula))
                {
                    series5.XValueFormula = series6.XValueFormula;
                }
                else if (series6.XValues.Count > 0)
                {
                    series5.XValues = series6.XValues;
                }
            }
            MemoryStream stream = new MemoryStream();
            XmlWriter writer = XmlWriter.Create((Stream) stream);
            if (writer != null)
            {
                Serializer.SerializeObj(series5, "DataSeries", writer);
                writer.Close();
                stream.Seek(0L, (SeekOrigin) SeekOrigin.Begin);
                XmlReader @this = XmlReader.Create((Stream) stream);
                if (@this != null)
                {
                    SpreadOpenHighLowCloseSeries series7 = Dt.Cells.Data.ChangeChartTypeHelper.CreateDataSeries(toChartType) as SpreadOpenHighLowCloseSeries;
                    ((IXmlSerializable) series7).ReadXml(@this);
                    @this.Close();
                    list.Add(series7);
                }
            }
            this._chart.AxisX.ItemsFormula = null;
            this._chart.AxisY.ItemsFormula = null;
            if (list.Count > 0)
            {
                this._chart.DataSeries.Clear();
                this._chart.DataSeries.AddRange((IList<SpreadDataSeries>) list);
            }
            return true;
        }

        /// <summary>
        /// Defines the method to be called when the action is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the action. If the action does not require data to be passed, this object can be set to null.</param>
        public override void Execute(object parameter)
        {
            this.ChangeChartType(this._oldType, this._newType);
            SheetView view = parameter as SheetView;
            if (view != null)
            {
                view.InvalidateCharts(new SpreadChart[] { this._chart });
            }
        }

        private void ResetDataSeriesChartType()
        {
            using (IEnumerator<SpreadDataSeries> enumerator = this._chart.DataSeries.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    enumerator.Current.ResetChartType();
                }
            }
        }

        /// <summary>
        /// Saves the state for undoing the command or operation.
        /// </summary>
        public void SaveState()
        {
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ResourceStrings.ChangeChartType;
        }

        /// <summary>
        /// Undoes the command or operation.
        /// </summary>
        /// <param name="parameter">The parameter to undo the command or operation.</param>
        /// <returns>
        /// <c>true</c> if an undo operation on the command or operation succeeds; otherwise, <c>false</c>.
        /// </returns>
        public bool Undo(object parameter)
        {
            this.ChangeChartType(this._newType, this._oldType);
            SheetView view = parameter as SheetView;
            if (view != null)
            {
                view.InvalidateCharts(new SpreadChart[] { this._chart });
            }
            return true;
        }

        /// <summary>
        /// Gets a value that indicates whether the command or operation can be undone.
        /// </summary>
        public bool CanUndo
        {
            get { return  true; }
        }
    }
}

