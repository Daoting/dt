#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Cells.Data;
using Dt.Cells.UI;
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
        SpreadChart _chart;
        SpreadChartType _newType;
        SpreadChartType _oldType;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.UndoRedo.ChangeChartTypeAction" /> class.
        /// </summary>
        /// <param name="chart">The chart.</param>
        /// <param name="newType">The new type.</param>
        public ChangeChartTypeAction(SpreadChart chart, SpreadChartType newType)
        {
            _chart = chart;
            _oldType = chart.ChartType;
            _newType = newType;
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

        bool ChangeChartType(SpreadChartType fromChartType, SpreadChartType toChartType)
        {
            _chart.SuspendEvents();
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
                        flag = ChangeToStockChart(fromChartType, toChartType);
                    }
                    else if (fromChartType == SpreadChartType.StockHighLowOpenClose)
                    {
                        flag = ChangeFromStockChart(fromChartType, toChartType);
                    }
                    else if (num2 != 1)
                    {
                        flag = ChangeChartTypeToMulti(fromChartType, toChartType);
                    }
                    else if (num2 == 1)
                    {
                        flag = ChangeChartTypeOne(fromChartType, toChartType);
                    }
                }
                else
                {
                    if (flag2 != flag3)
                    {
                        AxisType axisType = _chart.AxisY.AxisType;
                        _chart.AxisY.AxisType = _chart.AxisX.AxisType;
                        _chart.AxisX.AxisType = axisType;
                        string itemsFormula = _chart.AxisY.ItemsFormula;
                        _chart.AxisY.ItemsFormula = _chart.AxisX.ItemsFormula;
                        _chart.AxisX.ItemsFormula = itemsFormula;
                    }
                    _chart.ChartType = toChartType;
                }
                if (flag2 != flag3)
                {
                    if (flag3)
                    {
                        if (_chart.AxisX != null)
                        {
                            _chart.AxisX.ShowMajorGridlines = true;
                        }
                        if (_chart.AxisY != null)
                        {
                            _chart.AxisY.ShowMajorGridlines = false;
                        }
                    }
                    if (flag2)
                    {
                        if (_chart.AxisY != null)
                        {
                            _chart.AxisY.ShowMajorGridlines = true;
                        }
                        if (_chart.AxisX != null)
                        {
                            _chart.AxisX.ShowMajorGridlines = false;
                        }
                    }
                }
                if (!Dt.Cells.Data.SpreadChartUtility.IsChartWithMarker(toChartType))
                {
                    return flag;
                }
                foreach (SpreadDataSeries series in _chart.DataSeries)
                {
                    if (series.MarkerType == MarkerType.None)
                    {
                        series.MarkerType = MarkerType.Automatic;
                    }
                    if ((series.MarkerType != MarkerType.None) && ((series.MarkerSize.IsEmpty || (series.MarkerSize.Width == 0.0)) || (series.MarkerSize.Height == 0.0)))
                    {
                        series.MarkerSize = new Size(7.0, 7.0);
                    }
                }
            }
            finally
            {
                _chart.ResumeEvents();
            }
            return flag;
        }

        bool ChangeChartTypeOne(SpreadChartType fromChartType, SpreadChartType toChartType)
        {
            SpreadChartUtility.GetDataDimension(fromChartType);
            SpreadChartUtility.GetDataDimension(toChartType);
            string itemsFormula = _chart.ItemsFormula;
            string xValueFormula = null;
            _chart.ChartType = toChartType;
            ResetDataSeriesChartType();
            List<SpreadDataSeries> list = new List<SpreadDataSeries>();
            foreach (SpreadDataSeries series in _chart.DataSeries)
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
                    _chart.AxisY.ItemsFormula = xValueFormula;
                    _chart.AxisX.ItemsFormula = null;
                }
                else
                {
                    _chart.AxisX.ItemsFormula = xValueFormula;
                    _chart.AxisY.ItemsFormula = null;
                }
            }
            if (list.Count > 0)
            {
                _chart.DataSeries.Clear();
                _chart.DataSeries.AddRange((IList<SpreadDataSeries>) list);
            }
            return true;
        }

        bool ChangeChartTypeToMulti(SpreadChartType fromChartType, SpreadChartType toChartType)
        {
            Dt.Cells.Data.SpreadChartUtility.GetDataDimension(fromChartType);
            Dt.Cells.Data.SpreadChartUtility.GetDataDimension(toChartType);
            string itemsFormula = _chart.ItemsFormula;
            _chart.ChartType = toChartType;
            ResetDataSeriesChartType();
            List<SpreadDataSeries> list = new List<SpreadDataSeries>();
            foreach (SpreadDataSeries series in _chart.DataSeries)
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
            _chart.AxisX.ItemsFormula = null;
            _chart.AxisY.ItemsFormula = null;
            if (list.Count > 0)
            {
                _chart.DataSeries.Clear();
                _chart.DataSeries.AddRange((IList<SpreadDataSeries>) list);
            }
            return true;
        }

        bool ChangeFromStockChart(SpreadChartType fromChartType, SpreadChartType toChartType)
        {
            Dt.Cells.Data.SpreadChartUtility.GetDataDimension(fromChartType);
            int dataDimension = Dt.Cells.Data.SpreadChartUtility.GetDataDimension(toChartType);
            if ((_chart.DataSeries.Count <= 0) || !(_chart.DataSeries[0] is SpreadOpenHighLowCloseSeries))
            {
                return false;
            }
            string itemsFormula = _chart.ItemsFormula;
            ResetDataSeriesChartType();
            _chart.ChartType = toChartType;
            List<SpreadDataSeries> list = new List<SpreadDataSeries>();
            SpreadOpenHighLowCloseSeries series = _chart.DataSeries[0] as SpreadOpenHighLowCloseSeries;
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
                    _chart.AxisY.ItemsFormula = series.XValueFormula;
                    _chart.AxisX.ItemsFormula = null;
                }
                else
                {
                    _chart.AxisX.ItemsFormula = series.XValueFormula;
                    _chart.AxisY.ItemsFormula = null;
                }
            }
            if (list.Count > 0)
            {
                _chart.DataSeries.Clear();
                _chart.DataSeries.AddRange((IList<SpreadDataSeries>) list);
            }
            return true;
        }

        bool ChangeToStockChart(SpreadChartType fromChartType, SpreadChartType toChartType)
        {
            Dt.Cells.Data.SpreadChartUtility.GetDataDimension(fromChartType);
            Dt.Cells.Data.SpreadChartUtility.GetDataDimension(toChartType);
            if (_chart.DataSeries.Count < 4)
            {
                return false;
            }
            string itemsFormula = _chart.ItemsFormula;
            _chart.ChartType = toChartType;
            List<SpreadDataSeries> list = new List<SpreadDataSeries>();
            SpreadDataSeries openSeries = _chart.DataSeries[0];
            SpreadDataSeries highSeries = _chart.DataSeries[1];
            SpreadDataSeries lowSeries = _chart.DataSeries[2];
            SpreadDataSeries closeSeries = _chart.DataSeries[3];
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
            _chart.AxisX.ItemsFormula = null;
            _chart.AxisY.ItemsFormula = null;
            if (list.Count > 0)
            {
                _chart.DataSeries.Clear();
                _chart.DataSeries.AddRange((IList<SpreadDataSeries>) list);
            }
            return true;
        }

        /// <summary>
        /// Defines the method to be called when the action is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the action. If the action does not require data to be passed, this object can be set to null.</param>
        public override void Execute(object parameter)
        {
            ChangeChartType(_oldType, _newType);
            var excel = parameter as Excel;
            if (excel != null)
            {
                excel.RefreshCharts(new SpreadChart[] { _chart });
            }
        }

        void ResetDataSeriesChartType()
        {
            using (IEnumerator<SpreadDataSeries> enumerator = _chart.DataSeries.GetEnumerator())
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
            ChangeChartType(_newType, _oldType);
            var excel = parameter as Excel;
            if (excel != null)
            {
                excel.RefreshCharts(new SpreadChart[] { _chart });
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

