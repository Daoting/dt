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
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 气泡图表，可以自定义气泡大小
    /// </summary>
    public partial class BubbleSeries : XYDataSeries
    {
        /// <summary>
        /// 
        /// </summary>
        static DependencyProperty SizeValuesProperty = Utils.RegisterProperty(
           "SizeValues",
           typeof(DoubleCollection),
           typeof(BubbleSeries),
           new PropertyChangedCallback(DataSeries.OnSeriesChanged));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SizeValuesSourceProperty = Utils.RegisterProperty(
            "SizeValuesSource",
            typeof(IEnumerable),
            typeof(BubbleSeries),
            new PropertyChangedCallback(DataSeries.OnChangeValues));

        Binding _sizeValuesBinding;
        static Size DefMaxSize = new Size(50.0, 50.0);
        static Size DefMinSize = new Size(5.0, 5.0);
        protected List<object> listSize = new List<object>();
       

        internal override string[] GetDataNamesInternal()
        {
            return new string[] { "Values", "XValues", "SizeValues" };
        }

        internal Size GetMaxSize(DataInfo dataInfo)
        {
            Size symbolMaxSize = dataInfo.SymbolMaxSize;
            if (symbolMaxSize.IsEmpty)
            {
                symbolMaxSize = DefMaxSize;
            }
            return symbolMaxSize;
        }

        internal Size GetMinSize(DataInfo dataInfo)
        {
            Size symbolMinSize = dataInfo.SymbolMinSize;
            if (symbolMinSize.IsEmpty)
            {
                symbolMinSize = DefMinSize;
            }
            return symbolMinSize;
        }

        internal override Size GetSymbolSize(int pointIndex, DataInfo dataInfo, Chart chart)
        {
            Size size = new Size();
            if ((dataInfo.MinVals.Length < 3) || (dataInfo.MaxVals.Length < 3))
            {
                return size;
            }
            double num = dataInfo.MinVals[2];
            double num2 = dataInfo.MaxVals[2];
            Size minSize = GetMinSize(dataInfo);
            Size maxSize = GetMaxSize(dataInfo);
            double[,] values = GetValues();
            if (values == null)
            {
                return size;
            }
            int length = values.GetLength(0);
            int num4 = values.GetLength(1);
            if (((length < 3) || (pointIndex < 0)) || (pointIndex >= num4))
            {
                return size;
            }
            double d = values[2, pointIndex];
            if (double.IsNaN(d))
            {
                return size;
            }
            double num6 = (num2 == num) ? 1.0 : ((d - num) / (num2 - num));
            if ((chart != null) && (BubbleOptions.GetScale(chart) == BubbleScale.Area))
            {
                num6 = Math.Sqrt(num6);
            }
            return new Size(minSize.Width + (num6 * (maxSize.Width - minSize.Width)), minSize.Height + (num6 * (maxSize.Height - minSize.Height)));
        }

        internal override ValueCoordinate[] GetValueCoordinates(bool check)
        {
            if (check)
            {
                return new ValueCoordinate[] { base.Check(ValueCoordinate.Y), base.Check(ValueCoordinate.X), base.Check(ValueCoordinate.Size) };
            }
            return new ValueCoordinate[] { ValueCoordinate.Y, ValueCoordinate.X, ValueCoordinate.Size };
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
                if (SizeValueBinding == null)
                {
                    DataSeries.InitList(listSize, SizeValuesSource, (IList<double>) SizeValues);
                }
                if (base.listX.Count == 0)
                {
                    for (int i = 0; i < base.listY.Count; i++)
                    {
                        base.listX.Add((int) i);
                    }
                }
                base.datavalues = base.CreateValues(new IList[] { base.listY, base.listX, listSize });
                if (base.isTimeValues == null)
                {
                    base.isTimeValues = new bool[3];
                }
                base.isTimeValues[0] = base.IsTimeData(base.listY);
                base.isTimeValues[1] = base.IsTimeData(base.listX);
                base.isTimeValues[2] = base.IsTimeData(listSize);
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
                    base.listX.Clear();
                    base.listX.AddRange(vals);
                    return;

                case 2:
                    listSize.Clear();
                    listSize.AddRange(vals);
                    return;
            }
        }

        public override Binding[] MemberPaths
        {
            get
            {
                if (((base.ValueBinding == null) && (base.XValueBinding == null)) && (SizeValueBinding == null))
                {
                    return null;
                }
                return new Binding[] { base.ValueBinding, base.XValueBinding, SizeValueBinding };
            }
        }

        public Binding SizeValueBinding
        {
            get { return  _sizeValuesBinding; }
            set
            {
                if (_sizeValuesBinding != value)
                {
                    _sizeValuesBinding = value;
                    base.Dirty = true;
                    base.FirePropertyChanged("SizeValueBinding");
                }
            }
        }

        public DoubleCollection SizeValues
        {
            get { return  (DoubleCollection) base.GetValue(SizeValuesProperty); }
            set { base.SetValue(SizeValuesProperty, value); }
        }

        public IEnumerable SizeValuesSource
        {
            get { return  (IEnumerable) base.GetValue(SizeValuesSourceProperty); }
            set { base.SetValue(SizeValuesSourceProperty, value); }
        }
    }
}

