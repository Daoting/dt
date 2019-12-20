#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using System;
using System.Collections;
using System.Globalization;
using Windows.Foundation;
#endregion

namespace Dt.Charts
{
    public class DataPoint
    {
        private DataSeries _ds;
        private object _item;
        private Dt.Charts.Keywords _kwords;
        private string _name;
        private string[] _names;
        private int _pi;
        private int _si;
        private double _val;
        internal Point LabelPoint;
        private static object NoValue = double.NaN;
        internal Point Point;

        internal DataPoint(double value, object item)
        {
            _si = -1;
            _pi = -1;
            _val = value;
            _names = new string[] { "Value" };
            _item = item;
        }

        internal DataPoint(DataSeries ds, int seriesIndex, int pointIndex, string[] names)
        {
            _si = -1;
            _pi = -1;
            _ds = ds;
            _si = seriesIndex;
            _pi = pointIndex;
            _names = names;
        }

        private void InitKeywords()
        {
            int num = (_names != null) ? _names.Length : 0;
            for (int i = 0; i < num; i++)
            {
                Keywords["#" + _names[i]] = Series.GetValue(i, PointIndex);
            }
            Keywords["#YValue"] = (double) Value;
            Keywords["#Value"] = (double) Value;
            Keywords["#PointIndex"] = (int) PointIndex;
            Keywords["#SeriesIndex"] = (int) SeriesIndex;
            if (_ds != null)
            {
                Keywords["#SeriesLabel"] = _ds.Label;
            }
            Keywords["#NewLine"] = "\r\n";
        }

        public override string ToString()
        {
            if (!double.IsNaN(Value))
            {
                return ((double) Value).ToString();
            }
            return base.ToString();
        }

        public object DataObject
        {
            get
            {
                if (((_item == null) && (_ds != null)) && (_pi >= 0))
                {
                    IEnumerable enumerable = _ds.ItemsSource ?? ((_ds.ParentData != null) ? _ds.ParentData.ItemsSource : null);
                    if (enumerable != null)
                    {
                        IList list = enumerable as IList;
                        if (list != null)
                        {
                            if (_pi < list.Count)
                            {
                                _item = list[_pi];
                            }
                        }
                        else
                        {
                            IEnumerator enumerator = enumerable.GetEnumerator();
                            DataUtils.TryReset(enumerator);
                            int num = 0;
                            while (enumerator.MoveNext())
                            {
                                if (num++ == _pi)
                                {
                                    _item = enumerator.Current;
                                    break;
                                }
                            }
                        }
                    }
                }
                return _item;
            }
        }

        public object this[string name]
        {
            get
            {
                int length = _names.Length;
                for (int i = 0; i < length; i++)
                {
                    if (name == _names[i])
                    {
                        return Series.GetValue(i, PointIndex);
                    }
                }
                return NoValue;
            }
        }

        public object this[int index]
        {
            get
            {
                int length = _names.Length;
                if (index < length)
                {
                    return Series.GetValue(index, PointIndex);
                }
                return NoValue;
            }
        }

        public object this[string name, string format]
        {
            get
            {
                int length = _names.Length;
                for (int i = 0; i < length; i++)
                {
                    if (name == _names[i])
                    {
                        IFormattable formattable = Series.GetValue(i, PointIndex) as IFormattable;
                        if (formattable != null)
                        {
                            return formattable.ToString(format, (IFormatProvider) CultureInfo.CurrentCulture);
                        }
                    }
                }
                return NoValue;
            }
        }

        internal Dt.Charts.Keywords Keywords
        {
            get
            {
                if (_kwords == null)
                {
                    _kwords = new Dt.Charts.Keywords();
                    InitKeywords();
                }
                return _kwords;
            }
        }

        public string Name
        {
            get { return  _name; }
            internal set { _name = value; }
        }

        public double PercentagePoint
        {
            get
            {
                if (_ds != null)
                {
                    return (Value / _ds.GetPointSum(PointIndex));
                }
                return double.NaN;
            }
        }

        public double PercentageSeries
        {
            get
            {
                if (_ds != null)
                {
                    return (Value / _ds.GetSum());
                }
                return double.NaN;
            }
        }

        public int PointIndex
        {
            get { return  _pi; }
        }

        public DataSeries Series
        {
            get { return  _ds; }
        }

        public int SeriesIndex
        {
            get { return  _si; }
        }

        public double Value
        {
            get
            {
                if (Series == null)
                {
                    return _val;
                }
                object obj2 = Series.GetValue(0, PointIndex);
                if (obj2 is double)
                {
                    return (double) ((double) obj2);
                }
                if (obj2 is DateTime)
                {
                    return ((DateTime) obj2).ToOADate();
                }
                return (double) ((double) NoValue);
            }
        }
    }
}

