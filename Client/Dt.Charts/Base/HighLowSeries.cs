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
    /// 甘特图图表
    /// </summary>
    public partial class HighLowSeries : XYDataSeries, IMeasureSymbolWidth
    {
        /// <summary>
        /// 
        /// </summary>
        static DependencyProperty HighValuesProperty = DependencyProperty.Register(
           "HighValues",
           typeof(DoubleCollection),
           typeof(HighLowSeries),
           new PropertyMetadata(null, new PropertyChangedCallback(HighLowSeries.HighValuesChanged)));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty HighValuesSourceProperty = Utils.RegisterProperty(
            "HighValuesSource",
            typeof(IEnumerable),
            typeof(HighLowSeries),
            new PropertyChangedCallback(DataSeries.OnChangeValues));

        /// <summary>
        /// 
        /// </summary>
        static DependencyProperty LowValuesProperty = DependencyProperty.Register(
            "LowValues",
            typeof(DoubleCollection),
            typeof(HighLowSeries),
            new PropertyMetadata(null, new PropertyChangedCallback(HighLowSeries.LowValuesChanged)));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty LowValuesSourceProperty = Utils.RegisterProperty(
            "LowValuesSource",
            typeof(IEnumerable),
            typeof(HighLowSeries),
            new PropertyChangedCallback(DataSeries.OnChangeValues));

        Binding _highValuesBinding;
        Binding _lowValuesBinding;
        protected List<object> alHi = new List<object>();
        protected List<object> alLo = new List<object>();
        internal int Index;
        internal bool IsGantt;

        internal override string[] GetDataNamesInternal()
        {
            return new string[] { "Values", "XValues", "LowValues", "HighValues" };
        }

        internal override ValueCoordinate[] GetValueCoordinates(bool check)
        {
            if (check)
            {
                return new ValueCoordinate[] { base.Check(ValueCoordinate.Y), base.Check(ValueCoordinate.X), base.Check(ValueCoordinate.Y), base.Check(ValueCoordinate.Y) };
            }
            return new ValueCoordinate[] { ValueCoordinate.Y, ValueCoordinate.X, ValueCoordinate.Y, ValueCoordinate.Y };
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
                if (LowValueBinding == null)
                {
                    DataSeries.InitList(alLo, LowValuesSource, (IList<double>) LowValues);
                }
                if (HighValueBinding == null)
                {
                    DataSeries.InitList(alHi, HighValuesSource, (IList<double>) HighValues);
                }
                int maxCount = DataSeries.GetMaxCount(new IList[] { base.listY, base.listX, alLo, alHi });
                if (base.listX.Count == 0)
                {
                    if (IsGantt)
                    {
                        for (int i = 0; i < maxCount; i++)
                        {
                            base.listX.Add((int) Index);
                        }
                    }
                    else
                    {
                        for (int j = 0; j < maxCount; j++)
                        {
                            base.listX.Add((int) j);
                        }
                    }
                }
                if (base.listY.Count == 0)
                {
                    base.datavalues = base.CreateValues(new IList[] { alLo, base.listX, alLo, alHi });
                }
                else
                {
                    base.datavalues = base.CreateValues(new IList[] { base.listY, base.listX, alLo, alHi });
                }
                if (base.isTimeValues == null)
                {
                    base.isTimeValues = new bool[4];
                }
                base.isTimeValues[0] = base.IsTimeData(base.listY);
                base.isTimeValues[1] = base.IsTimeData(base.listX);
                base.isTimeValues[2] = base.IsTimeData(alLo);
                base.isTimeValues[3] = base.IsTimeData(alHi);
            }
            return base.datavalues;
        }

        static void HighValuesChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((DataSeries) obj).FirePropertyChanged("HighValues");
        }

        static void LowValuesChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((DataSeries) obj).FirePropertyChanged("LowValues");
        }

        internal override void SetResolvedValues(int index, object[] vals)
        {
            base.SetResolvedValues(index, vals);
            switch (index)
            {
                case 2:
                    alLo.Clear();
                    alLo.AddRange(vals);
                    return;

                case 3:
                    alHi.Clear();
                    alHi.AddRange(vals);
                    return;
            }
        }

        public Binding HighValueBinding
        {
            get { return  _highValuesBinding; }
            set
            {
                if (_highValuesBinding != value)
                {
                    _highValuesBinding = value;
                    base.Dirty = true;
                    base.FirePropertyChanged("HighValueBinding");
                }
            }
        }

        public DoubleCollection HighValues
        {
            get { return  (DoubleCollection) base.GetValue(HighValuesProperty); }
            set { base.SetValue(HighValuesProperty, value); }
        }

        public IEnumerable HighValuesSource
        {
            get { return  (IEnumerable) base.GetValue(HighValuesSourceProperty); }
            set { base.SetValue(HighValuesSourceProperty, value); }
        }

        public Binding LowValueBinding
        {
            get { return  _lowValuesBinding; }
            set
            {
                if (_lowValuesBinding != value)
                {
                    _lowValuesBinding = value;
                    base.Dirty = true;
                    base.FirePropertyChanged("LowValueBinding");
                }
            }
        }

        public DoubleCollection LowValues
        {
            get { return  (DoubleCollection) base.GetValue(LowValuesProperty); }
            set { base.SetValue(LowValuesProperty, value); }
        }

        public IEnumerable LowValuesSource
        {
            get { return  (IEnumerable) base.GetValue(LowValuesSourceProperty); }
            set { base.SetValue(LowValuesSourceProperty, value); }
        }

        public override Binding[] MemberPaths
        {
            get
            {
                if (((base.ValueBinding == null) && (base.XValueBinding == null)) && ((HighValueBinding == null) && (LowValueBinding == null)))
                {
                    return null;
                }
                return new Binding[] { base.ValueBinding, base.XValueBinding, LowValueBinding, HighValueBinding };
            }
        }
    }
}

