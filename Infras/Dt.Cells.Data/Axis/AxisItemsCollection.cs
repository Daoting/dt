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
using System.Reflection;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Provides a container for a SeriesDataCollection of object.
    /// </summary>
    public sealed class AxisItemsCollection : SeriesDataCollection<object>
    {
        Dt.Cells.Data.Axis _axis;

        /// <summary>
        /// 
        /// </summary>
        internal AxisItemsCollection(Dt.Cells.Data.Axis axis)
        {
            this._axis = axis;
            base.ValuesSeperator = Environment.NewLine;
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public override void Add(object item)
        {
            base.Add(item);
            this.AdjustAxisMinMax(this.Axis, item);
        }

        void AdjustAxisMinMax(Dt.Cells.Data.Axis axis, object value)
        {
            double? nullable = FormatConverter.TryDouble(value, true);
            if (nullable.HasValue)
            {
                if (nullable.Value > axis.Max)
                {
                    if (axis.AutoMax)
                    {
                        if (axis.AxisType == AxisType.Value)
                        {
                            nullable = new double?(AxisUtility.CalculateMaximum(axis.Min, nullable.Value, axis.MajorUnit, false, axis.LogBase, false));
                        }
                        double? nullable2 = nullable;
                        if ((((double) nullable2.GetValueOrDefault()) != double.MinValue) || !nullable2.HasValue)
                        {
                            axis.SetMaxInternal(nullable.Value);
                        }
                        if ((axis.AutoMajorUnit || axis.AutoMinorUnit) && ((nullable.Value != double.MaxValue) && (nullable.Value != double.MinValue)))
                        {
                            axis.UpdateMajorMinorUnit(axis.Min, nullable.Value);
                        }
                    }
                }
                else if ((nullable.Value < axis.Min) && axis.AutoMin)
                {
                    if (axis.AxisType == AxisType.Value)
                    {
                        nullable = new double?(AxisUtility.CalculateMinimum(nullable.Value, axis.Max, axis.MajorUnit, false, axis.LogBase));
                    }
                    if (nullable.Value != double.MaxValue)
                    {
                        axis.SetMinInternal(nullable.Value);
                    }
                    if ((axis.AutoMajorUnit || axis.AutoMinorUnit) && ((nullable.Value != double.MaxValue) && (nullable.Value != double.MinValue)))
                    {
                        axis.UpdateMajorMinorUnit(nullable.Value, axis.Max);
                    }
                }
            }
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            this.UpdateAxisMinMax();
        }

        /// <summary>
        /// Converts the value.
        /// </summary>
        /// <param name="valueIndex">Index of the value.</param>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        protected override object ConvertValue(int valueIndex, object obj)
        {
            return obj;
        }

        int GetNumbersCount(double number)
        {
            return ((double) number).ToString().Length;
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        public override void Insert(int index, object item)
        {
            base.Insert(index, item);
            this.AdjustAxisMinMax(this.Axis, item);
        }

        internal override void OnIsDateTimeSeriesChanged()
        {
            if ((this.Axis != null) && (this.Axis.Chart != null))
            {
                this.Axis.Chart.UpdateAxisesAutoType();
                this.Axis.Chart.UpdateAxisFormatter(this.Axis, null);
            }
            base.OnIsDateTimeSeriesChanged();
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public override bool Remove(object item)
        {
            bool flag = base.Remove(item);
            this.AdjustAxisMinMax(this.Axis, item);
            return flag;
        }

        internal void UpdateAxisMinMax()
        {
            if ((this.Axis.AxisType == AxisType.Date) || (this.Axis.AxisType == AxisType.Value))
            {
                this.UpdateValueAxisMinMax();
            }
            else
            {
                this.UpdateCategoryAxisMinMax();
            }
        }

        void UpdateCategoryAxisMinMax()
        {
            Dt.Cells.Data.Axis axis = this.Axis;
            double min = 0.0;
            double maxDataPointCount = axis.MaxDataPointCount;
            if (axis.AutoMax)
            {
                axis.SetMaxInternal(maxDataPointCount);
            }
            if (axis.AutoMin)
            {
                axis.SetMinInternal(min);
            }
            if (axis.AutoMajorUnit)
            {
                axis.SetMajorUnitInteral(1.0);
            }
            if (axis.AutoMinorUnit)
            {
                axis.SetMinorUnitInternal(0.5);
            }
        }

        internal override void UpdateCollection()
        {
            base.UpdateCollection();
            if (((base.items != null) && (base.items.Count > 0)) && (this.Axis != null))
            {
                if ((this.Axis.AxisType == AxisType.Date) || (this.Axis.AxisType == AxisType.Value))
                {
                    this.UpdateAxisMinMax();
                    this.UpdateItemsForAxis();
                }
                else
                {
                    this.UpdateCategoryAxisMinMax();
                }
            }
        }

        void UpdateDateTimeItems()
        {
            double num;
            double num2;
            List<DateTime> list = new List<DateTime>();
            this.Axis.AdjustMinMax(out num, out num2);
            DateTime time = DateTimeExtension.FromOADate(num);
            DateTime time2 = DateTimeExtension.FromOADate(num2);
            while (time < time2)
            {
                list.Add(time);
                switch (this.Axis.MajorTimeUnit)
                {
                    case AxisTimeUnit.Days:
                        time = time.AddDays((double) ((int) this.Axis.MajorUnit));
                        break;

                    case AxisTimeUnit.Months:
                        time = time.AddMonths((int) this.Axis.MajorUnit);
                        break;

                    case AxisTimeUnit.Years:
                        time = time.AddYears((int) this.Axis.MajorUnit);
                        break;
                }
            }
            list.Add(time);
            base.items.Clear();
            foreach (DateTime time3 in list)
            {
                base.items.Add(time3);
            }
            this.Axis.NotifyAxisChanged("Items");
        }

        void UpdateDoubleItems()
        {
            double num;
            double num2;
            List<double> list = new List<double>();
            this.Axis.AdjustMinMax(out num, out num2);
            double num3 = num;
            while (num3 < num2)
            {
                list.Add(num3);
                if (this.Axis.AxisType == AxisType.Value)
                {
                    num3 += this.Axis.MajorUnit;
                }
                else
                {
                    num3++;
                }
            }
            list.Add(num3);
            base.items.Clear();
            foreach (double num4 in list)
            {
                base.items.Add((double) num4);
            }
            this.Axis.NotifyAxisChanged("Items");
        }

        internal void UpdateItemsForAxis()
        {
            if (!this.Axis.UseCustomItems)
            {
                if (this.Axis.AxisType == AxisType.Value)
                {
                    this.UpdateDoubleItems();
                }
                else if (this.Axis.AxisType == AxisType.Date)
                {
                    this.UpdateDateTimeItems();
                }
                else if (this.Axis.AxisType == AxisType.Category)
                {
                    this.UpdateDoubleItems();
                }
            }
        }

        internal void UpdateValueAxisMinMax()
        {
            if (base.items.Count != 0)
            {
                double maxValue = double.MaxValue;
                double minValue = double.MinValue;
                Dt.Cells.Data.Axis axis = this.Axis;
                using (IEnumerator<object> enumerator = base.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        double? nullable = FormatConverter.TryDouble(enumerator.Current, true);
                        if (nullable.HasValue)
                        {
                            if (nullable.Value < maxValue)
                            {
                                maxValue = nullable.Value;
                            }
                            if (nullable.Value > minValue)
                            {
                                minValue = nullable.Value;
                            }
                        }
                    }
                }
                if (axis.AxisType == AxisType.Value)
                {
                    maxValue = AxisUtility.CalculateValidMinimum(maxValue, minValue, false, axis.LogBase, axis.AutoMin, axis.AutoMax);
                    minValue = AxisUtility.CalculateValidMaximum(maxValue, minValue, false, axis.LogBase);
                    maxValue = AxisUtility.CalculateMinimum(maxValue, minValue, false, axis.LogBase);
                    minValue = AxisUtility.CalculateMaximum(maxValue, minValue, false, axis.LogBase);
                }
                if (axis.AutoMin)
                {
                    if (axis.AxisType == AxisType.Value)
                    {
                        maxValue = AxisUtility.CalculateMinimum(maxValue, minValue, axis.MajorUnit, false, axis.LogBase);
                    }
                    if (maxValue != double.MaxValue)
                    {
                        axis.SetMinInternal(maxValue);
                    }
                }
                if (axis.AutoMax)
                {
                    if (axis.AxisType == AxisType.Value)
                    {
                        minValue = AxisUtility.CalculateMaximum(maxValue, minValue, axis.MajorUnit, false, axis.LogBase, false);
                    }
                    if (minValue != double.MinValue)
                    {
                        axis.SetMaxInternal(minValue);
                    }
                }
                if ((axis.AutoMajorUnit || axis.AutoMinorUnit) && ((maxValue != double.MaxValue) && (minValue != double.MinValue)))
                {
                    axis.UpdateMajorMinorUnit(maxValue, minValue);
                }
            }
        }

        internal Dt.Cells.Data.Axis Axis
        {
            get { return  this._axis; }
        }

        /// <summary>
        /// Gets or sets the <see cref="T:System.Object" /> at the specified index.
        /// </summary>
        /// <value>
        /// The <see cref="T:System.Object" />.
        /// </value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public override object this[int index]
        {
            get { return  base[index]; }
            set
            {
                base[index] = value;
                this.AdjustAxisMinMax(this.Axis, value);
            }
        }
    }
}

