#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Charts;
using System;
using System.Collections;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 财务图表
    /// </summary>
    [Windows.UI.Xaml.Data.Bindable]
    public partial class HighLowOpenCloseSeries : HighLowSeries
    {
        /// <summary>
        /// 
        /// </summary>
        static DependencyProperty CloseValuesProperty = DependencyProperty.Register(
           "CloseValues",
           typeof(DoubleCollection),
           typeof(HighLowOpenCloseSeries),
           new PropertyMetadata(null, new PropertyChangedCallback(HighLowOpenCloseSeries.CloseValuesChanged)));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty CloseValuesSourceProperty = Utils.RegisterProperty(
            "CloseValuesSource",
            typeof(IEnumerable),
            typeof(HighLowOpenCloseSeries),
            new PropertyChangedCallback(DataSeries.OnChangeValues));

        /// <summary>
        /// 
        /// </summary>
        static DependencyProperty OpenValuesProperty = DependencyProperty.Register(
            "OpenValues",
            typeof(DoubleCollection),
            typeof(HighLowOpenCloseSeries),
            new PropertyMetadata(null, new PropertyChangedCallback(HighLowOpenCloseSeries.OpenValuesChanged)));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty OpenValuesSourceProperty = Utils.RegisterProperty(
            "OpenValuesSource",
            typeof(IEnumerable),
            typeof(HighLowOpenCloseSeries),
            new PropertyChangedCallback(DataSeries.OnChangeValues));

        Binding _closeValuesBinding;
        Binding _openValuesBinding;
        protected List<object> alCl = new List<object>();
        protected List<object> alOp = new List<object>();
       

        static void CloseValuesChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((DataSeries) obj).FirePropertyChanged("CloseValues");
        }

        internal override DataPoint CreateDataPoint(int i, int j)
        {
            return new HLOCDataPoint(this, i, j, GetDataNamesInternal());
        }

        internal override string[] GetDataNamesInternal()
        {
            return new string[] { "Values", "XValues", "LowValues", "HighValues", "OpenValues", "CloseValues" };
        }

        internal override ValueCoordinate[] GetValueCoordinates(bool check)
        {
            if (check)
            {
                return new ValueCoordinate[] { base.Check(ValueCoordinate.Y), base.Check(ValueCoordinate.X), base.Check(ValueCoordinate.Y), base.Check(ValueCoordinate.Y), base.Check(ValueCoordinate.Y), base.Check(ValueCoordinate.Y) };
            }
            return new ValueCoordinate[] { ValueCoordinate.Y, ValueCoordinate.X, ValueCoordinate.Y, ValueCoordinate.Y, ValueCoordinate.Y, ValueCoordinate.Y };
        }

        internal override double[,] GetValues()
        {
            if (base.datavalues == null)
            {
                if (base.ValueBinding == null)
                {
                    DataSeries.InitList(base.listY, base.ValuesSource, (IList<double>) base.Values);
                }
                if (base.XValueBinding == null)
                {
                    DataSeries.InitList(base.listX, base.XValuesSource, (IList<double>) base.XValues);
                }
                if (base.listX.Count == 0)
                {
                    for (int i = 0; i < base.listY.Count; i++)
                    {
                        base.listX.Add((int) i);
                    }
                }
                if (base.LowValueBinding == null)
                {
                    DataSeries.InitList(base.alLo, base.LowValuesSource, (IList<double>) base.LowValues);
                }
                if (base.HighValueBinding == null)
                {
                    DataSeries.InitList(base.alHi, base.HighValuesSource, (IList<double>) base.HighValues);
                }
                if (OpenValueBinding == null)
                {
                    DataSeries.InitList(alOp, OpenValuesSource, (IList<double>) OpenValues);
                }
                if (CloseValueBinding == null)
                {
                    DataSeries.InitList(alCl, CloseValuesSource, (IList<double>) CloseValues);
                }
                if (base.listY.Count == 0)
                {
                    base.datavalues = base.CreateValues(new IList[] { alOp, base.listX, base.alLo, base.alHi, alOp, alCl });
                }
                else
                {
                    base.datavalues = base.CreateValues(new IList[] { base.listY, base.listX, base.alLo, base.alHi, alOp, alCl });
                }
                if (base.isTimeValues == null)
                {
                    base.isTimeValues = new bool[6];
                }
                base.isTimeValues[0] = base.IsTimeData((base.listY.Count == 0) ? alOp : base.listY);
                base.isTimeValues[1] = base.IsTimeData(base.listX);
                base.isTimeValues[2] = base.IsTimeData(base.alLo);
                base.isTimeValues[3] = base.IsTimeData(base.alHi);
                base.isTimeValues[4] = base.IsTimeData(alOp);
                base.isTimeValues[5] = base.IsTimeData(alCl);
            }
            return base.datavalues;
        }

        static void OpenValuesChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((DataSeries) obj).FirePropertyChanged("OpenValues");
        }

        internal override void SetResolvedValues(int index, object[] vals)
        {
            base.SetResolvedValues(index, vals);
            switch (index)
            {
                case 4:
                    alOp.Clear();
                    alOp.AddRange(vals);
                    return;

                case 5:
                    alCl.Clear();
                    alCl.AddRange(vals);
                    return;
            }
        }

        public Binding CloseValueBinding
        {
            get { return  _closeValuesBinding; }
            set
            {
                if (_closeValuesBinding != value)
                {
                    _closeValuesBinding = value;
                    base.Dirty = true;
                    base.FirePropertyChanged("CloseValueBinding");
                }
            }
        }

        public DoubleCollection CloseValues
        {
            get { return  (DoubleCollection) base.GetValue(CloseValuesProperty); }
            set { base.SetValue(CloseValuesProperty, value); }
        }

        public IEnumerable CloseValuesSource
        {
            get { return  (IEnumerable) base.GetValue(CloseValuesSourceProperty); }
            set { base.SetValue(CloseValuesSourceProperty, value); }
        }

        public override Binding[] MemberPaths
        {
            get
            {
                if ((((base.ValueBinding == null) && (base.XValueBinding == null)) && ((base.HighValueBinding == null) && (base.LowValueBinding == null))) && ((OpenValueBinding == null) && (CloseValueBinding == null)))
                {
                    return null;
                }
                return new Binding[] { base.ValueBinding, base.XValueBinding, base.LowValueBinding, base.HighValueBinding, OpenValueBinding, CloseValueBinding };
            }
        }

        public Binding OpenValueBinding
        {
            get { return  _openValuesBinding; }
            set
            {
                if (_openValuesBinding != value)
                {
                    _openValuesBinding = value;
                    base.Dirty = true;
                    base.FirePropertyChanged("OpenValueBinding");
                }
            }
        }

        public DoubleCollection OpenValues
        {
            get { return  (DoubleCollection) base.GetValue(OpenValuesProperty); }
            set { base.SetValue(OpenValuesProperty, value); }
        }

        public IEnumerable OpenValuesSource
        {
            get { return  (IEnumerable) base.GetValue(OpenValuesSourceProperty); }
            set { base.SetValue(OpenValuesSourceProperty, value); }
        }
    }


    [Windows.UI.Xaml.Data.Bindable]
    public class HLOCDataPoint : XYDataPoint
    {
        internal HLOCDataPoint(DataSeries ds, int seriesIndex, int pointIndex, string[] names) : base(ds, seriesIndex, pointIndex, names)
        {
        }

        public object Close
        {
            get { return base.Series.GetValue(5, base.PointIndex); }
        }

        public object High
        {
            get { return base.Series.GetValue(3, base.PointIndex); }
        }

        public object Low
        {
            get { return base.Series.GetValue(2, base.PointIndex); }
        }

        public object Open
        {
            get { return base.Series.GetValue(4, base.PointIndex); }
        }
    }
}

