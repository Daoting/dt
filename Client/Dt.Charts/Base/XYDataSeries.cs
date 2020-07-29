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
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 极线图表
    /// </summary>
    [Windows.UI.Xaml.Data.Bindable]
    public partial class XYDataSeries : DataSeries
    {
        /// <summary>
        /// 
        /// </summary>
        static DependencyProperty XValuesProperty = DependencyProperty.Register(
            "XValues",
            typeof(DoubleCollection),
            typeof(XYDataSeries),
            new PropertyMetadata(null, new PropertyChangedCallback(XYDataSeries.XValuesChanged)));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty XValuesSourceProperty = Utils.RegisterProperty(
            "XValuesSource",
            typeof(IEnumerable),
            typeof(XYDataSeries),
            new PropertyChangedCallback(DataSeries.OnChangeValues));

        Binding _xvaluesBinding;
        protected List<object> listX = new List<object>();
        

        internal override DataPoint CreateDataPoint(int i, int j)
        {
            return new XYDataPoint(this, i, j, GetDataNamesInternal());
        }

        internal override string[] GetDataNamesInternal()
        {
            return new string[] { "Values", "XValues" };
        }

        internal override ValueCoordinate[] GetValueCoordinates(bool check)
        {
            if (check)
            {
                return new ValueCoordinate[] { base.Check(ValueCoordinate.Y), base.Check(ValueCoordinate.X) };
            }
            return new ValueCoordinate[] { ValueCoordinate.Y, ValueCoordinate.X };
        }

        internal override double[,] GetValues()
        {
            if (base.datavalues == null)
            {
                if (base.ValueBinding == null)
                {
                    DataSeries.InitList(base.listY, base.ValuesSource, (IList<double>) base.Values);
                }
                if (XValueBinding == null)
                {
                    DataSeries.InitList(listX, XValuesSource, (IList<double>) XValues);
                }
                if (listX.Count == 0)
                {
                    for (int i = 0; i < base.listY.Count; i++)
                    {
                        listX.Add((int) i);
                    }
                }
                base.datavalues = base.CreateValues(new IList[] { base.listY, listX });
                if (base.isTimeValues == null)
                {
                    base.isTimeValues = new bool[2];
                }
                base.isTimeValues[0] = base.IsTimeData(base.listY);
                base.isTimeValues[1] = base.IsTimeData(listX);
                base.datavalues = base.AggregateValues(base.datavalues);
                base.datavalues = ProcessValues(base.datavalues);
            }
            return base.datavalues;
        }

        internal override void SetResolvedValues(int index, object[] vals)
        {
            switch (index)
            {
                case 0:
                    base.listY.Clear();
                    base.listY.AddRange(vals);
                    return;

                case 1:
                    listX.Clear();
                    listX.AddRange(vals);
                    return;
            }
        }

        static void XValuesChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((DataSeries) obj).FirePropertyChanged("XValues");
        }

        public override Binding[] MemberPaths
        {
            get
            {
                if ((base.ValueBinding == null) && (XValueBinding == null))
                {
                    return null;
                }
                return new Binding[] { base.ValueBinding, XValueBinding };
            }
        }

        public Binding XValueBinding
        {
            get { return  _xvaluesBinding; }
            set
            {
                if (_xvaluesBinding != value)
                {
                    _xvaluesBinding = value;
                    base.Dirty = true;
                    base.FirePropertyChanged("XValueBinding");
                }
            }
        }

        public DoubleCollection XValues
        {
            get { return  (DoubleCollection) base.GetValue(XValuesProperty); }
            set { base.SetValue(XValuesProperty, value); }
        }

        public IEnumerable XValuesSource
        {
            get { return  (IEnumerable) base.GetValue(XValuesSourceProperty); }
            set { base.SetValue(XValuesSourceProperty, value); }
        }
    }


    [Windows.UI.Xaml.Data.Bindable]
    public class XYDataPoint : DataPoint
    {
        internal XYDataPoint(DataSeries ds, int seriesIndex, int pointIndex, string[] names) : base(ds, seriesIndex, pointIndex, names)
        {
        }

        public object X
        {
            get { return base.Series.GetValue(1, base.PointIndex); }
        }

        public string XAsString
        {
            get { return base.Series.GetFormattedValue(1, base.PointIndex); }
        }

        public object Y
        {
            get { return base.Series.GetValue(0, base.PointIndex); }
        }

        public string YAsString
        {
            get { return base.Series.GetFormattedValue(0, base.PointIndex); }
        }
    }
}

