#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine.Expressions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Threading;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Provides a base collection for series data.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SeriesDataCollection<T> : NotifyCollection<T>, IDisposable
    {
        List<ISeriesDataProvider> _dataProviders;
        IDataSeries _dataSeries;
        bool _isDateTimeSeries;
        string _valuesSperator;

        internal event EventHandler IsDateTimeSeriesChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.SeriesDataCollection`1" /> class.
        /// </summary>
        public SeriesDataCollection()
        {
            this._valuesSperator = "";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.SeriesDataCollection`1" /> class.
        /// </summary>
        /// <param name="values">The values.</param>
        public SeriesDataCollection(IEnumerable<T> values)
        {
            this._valuesSperator = "";
            base.items.AddRange(values);
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public override void Add(T item)
        {
            this.CheckDataConnecting();
            base.Add(item);
            this.UpdateIsDateTimeSeries();
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="items">The items.</param>
        public override void AddRange(IList<T> items)
        {
            this.CheckDataConnecting();
            base.SuspendEvent();
            base.AddRange(items);
            base.ResumeEvent();
            this.UpdateIsDateTimeSeries();
        }

        void CheckDataConnecting()
        {
            if (this.IsBoundToDataSeries)
            {
                throw new InvalidOperationException("Collections can not be modified!");
            }
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public override void Clear()
        {
            this.CheckDataConnecting();
            base.Clear();
            this.UpdateIsDateTimeSeries();
        }

        /// <summary>
        /// Converts the value.
        /// </summary>
        /// <param name="valueIndex">Index of the value.</param>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        protected virtual T ConvertValue(int valueIndex, object obj)
        {
            return default(T);
        }

        internal virtual WorksheetSeriesDataProvider CreateSeriesDataProvider(IDataSeries dataSeries)
        {
            return new DataSeiresDataProvider(dataSeries);
        }

        void DataProvider_DataChanged(object sender, EventArgs e)
        {
            this.UpdateCollection();
            this.RaiseCollectionChanged(this, new NotifyCollectionChangedEventArgs((NotifyCollectionChangedAction) NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this._dataProviders != null)
            {
                foreach (ISeriesDataProvider provider in this._dataProviders)
                {
                    if (provider is IDisposable)
                    {
                        (provider as IDisposable).Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// Gets the underlying formatter.
        /// </summary>
        /// <param name="valueIndex">Index of the value.</param>
        /// <returns></returns>
        internal IFormatter GetUnderlyingFormatter(int valueIndex)
        {
            if (this._dataProviders != null)
            {
                int num = 0;
                foreach (ISeriesDataProvider provider in this._dataProviders)
                {
                    int valuesCount = provider.ValuesCount;
                    for (int i = 0; i < valuesCount; i++)
                    {
                        if ((num == valueIndex) && (provider is WorksheetSeriesDataProvider))
                        {
                            return (provider as WorksheetSeriesDataProvider).GetFormatter(valueIndex);
                        }
                        num++;
                    }
                }
            }
            return null;
        }

        internal string GetUnderlyingText(int valueIndex)
        {
            if (this._dataProviders != null)
            {
                int num = 0;
                foreach (ISeriesDataProvider provider in this._dataProviders)
                {
                    int valuesCount = provider.ValuesCount;
                    for (int i = 0; i < valuesCount; i++)
                    {
                        if (num == valueIndex)
                        {
                            string str = "";
                            for (int j = 0; j < provider.SeriesCount; j++)
                            {
                                str = str + provider.GetText(j, num);
                            }
                            return str;
                        }
                        num++;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the underlying value.
        /// </summary>
        /// <param name="valueIndex">Index of the value.</param>
        /// <returns></returns>
        internal object GetUnderlyingValue(int valueIndex)
        {
            if (this._dataProviders != null)
            {
                int num = 0;
                foreach (ISeriesDataProvider provider in this._dataProviders)
                {
                    int valuesCount = provider.ValuesCount;
                    for (int i = 0; i < valuesCount; i++)
                    {
                        if (num == valueIndex)
                        {
                            return provider.GetValue(i);
                        }
                        num++;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Inserts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        public override void Insert(int index, T item)
        {
            this.CheckDataConnecting();
            base.Insert(index, item);
            this.UpdateIsDateTimeSeries();
        }

        internal virtual void OnIsDateTimeSeriesChanged()
        {
            if (!base.IsEventSuspended && (this.IsDateTimeSeriesChanged != null))
            {
                this.IsDateTimeSeriesChanged(this, EventArgs.Empty);
            }
        }

        internal void RefreshData()
        {
            this.UpdateDataProviders();
            this.UpdateCollection();
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public override bool Remove(T item)
        {
            this.CheckDataConnecting();
            bool flag = base.Remove(item);
            this.UpdateIsDateTimeSeries();
            return flag;
        }

        /// <summary>
        /// Removes at.
        /// </summary>
        /// <param name="index">The index.</param>
        public override void RemoveAt(int index)
        {
            this.CheckDataConnecting();
            base.RemoveAt(index);
            this.UpdateIsDateTimeSeries();
        }

        void ResumeWorksheetChartsEvent()
        {
            if (this._dataProviders != null)
            {
                foreach (WorksheetSeriesDataProvider provider2 in this._dataProviders)
                {
                    if (provider2 != null)
                    {
                        provider2.Worksheet.Charts.ResumeItemsEvent();
                    }
                }
            }
        }

        void SuspendWorksheetChartsEvent()
        {
            if (this._dataProviders != null)
            {
                foreach (WorksheetSeriesDataProvider provider2 in this._dataProviders)
                {
                    if (provider2 != null)
                    {
                        provider2.Worksheet.Charts.SuspendItemsEvent();
                    }
                }
            }
        }

        internal virtual void UpdateCollection()
        {
            List<T> list = new List<T>();
            if ((this._dataProviders != null) && (this._dataProviders.Count > 0))
            {
                bool flag = true;
                foreach (ISeriesDataProvider provider in this._dataProviders)
                {
                    int valuesCount = provider.ValuesCount;
                    int seriesCount = provider.SeriesCount;
                    for (int i = 0; i < valuesCount; i++)
                    {
                        object obj2 = null;
                        string str = null;
                        bool flag2 = true;
                        for (int j = 0; j < seriesCount; j++)
                        {
                            flag2 = this.DataSeries.DisplayHiddenData || provider.IsValueVisible(j, i);
                            if (flag2)
                            {
                                object obj3 = provider.GetValue(j, i);
                                string text = provider.GetText(j, i);
                                if (j == 0)
                                {
                                    obj2 = obj3;
                                    str = text;
                                }
                                else
                                {
                                    string str3 = (obj2 != null) ? obj2.ToString() : "";
                                    string str4 = (obj3 != null) ? obj3.ToString() : "";
                                    if (!string.IsNullOrEmpty(str4))
                                    {
                                        obj2 = str3 + this.ValuesSeperator + str4;
                                    }
                                    else
                                    {
                                        obj2 = str3;
                                    }
                                    flag = false;
                                }
                            }
                        }
                        if (flag2)
                        {
                            if (!(obj2 is DateTime))
                            {
                                if (FormatConverter.IsNumber(obj2))
                                {
                                    DateTime time;
                                    if (!string.IsNullOrEmpty(str) && !DateTime.TryParse(str, (IFormatProvider)CultureInfo.CurrentCulture, (DateTimeStyles)DateTimeStyles.None, out time))
                                    {
                                        flag = false;
                                    }
                                }
                                else
                                {
                                    flag = false;
                                }
                            }
                            T local = this.ConvertValue(i, obj2);
                            list.Add(local);
                        }
                    }
                }
                base.items.Clear();
                base.items.AddRange((IEnumerable<T>) list);
                if ((flag != this.IsDateTimeSeries) && (base.items.Count > 0))
                {
                    this.IsDateTimeSeries = flag;
                }
            }
            else if ((this.DataSeries != null) && (this.DataSeries.DataReference is CalcArrayExpression))
            {
                bool flag3 = true;
                CalcArrayExpression dataReference = this.DataSeries.DataReference as CalcArrayExpression;
                if (dataReference.ArrayValue != null)
                {
                    int length = dataReference.ArrayValue.Length;
                    for (int k = 0; k < length; k++)
                    {
                        T local2 = this.ConvertValue(k, dataReference.ArrayValue.GetValue(k));
                        if (flag3 && !(local2 is DateTime))
                        {
                            flag3 = false;
                        }
                        list.Add(local2);
                    }
                }
                base.items.Clear();
                base.items.AddRange((IEnumerable<T>) list);
                if ((flag3 != this.IsDateTimeSeries) && (base.items.Count > 0))
                {
                    this.IsDateTimeSeries = flag3;
                }
            }
            else if ((this.DataSeries != null) && (this.DataSeries.DataReference is CalcConstantExpression))
            {
                CalcConstantExpression expression2 = this.DataSeries.DataReference as CalcConstantExpression;
                T local3 = this.ConvertValue(0, expression2.Value);
                base.items.Clear();
                base.items.Add(local3);
                bool flag4 = local3 is DateTime;
                if (flag4 != this.IsDateTimeSeries)
                {
                    this.IsDateTimeSeries = flag4;
                }
            }
            else
            {
                base.items.Clear();
                this.IsDateTimeSeries = false;
            }
        }

        void UpdateDataProviders()
        {
            if (this._dataProviders != null)
            {
                foreach (ISeriesDataProvider provider in this._dataProviders)
                {
                    if (provider != null)
                    {
                        if (provider is WorksheetSeriesDataProvider)
                        {
                            (provider as WorksheetSeriesDataProvider).Worksheet = null;
                        }
                        provider.DataChanged -= new EventHandler(this.DataProvider_DataChanged);
                    }
                }
            }
            this._dataProviders = null;
            if ((this.DataSeries != null) && (this.DataSeries.DataReference != null))
            {
                CalcReferenceExpression[] expressions = null;
                FormulaUtility.ExtractAllReferenceExpression(this._dataSeries.Evaluator, this._dataSeries.DataReference, out expressions);
                if ((expressions != null) && (expressions.Length > 0))
                {
                    this._dataProviders = new List<ISeriesDataProvider>();
                    foreach (CalcReferenceExpression expression in expressions)
                    {
                        DataOrientation vertical = DataOrientation.Vertical;
                        if (this._dataSeries.DataOrientation.HasValue)
                        {
                            vertical = this._dataSeries.DataOrientation.Value;
                        }
                        else if (expression is CalcRangeExpression)
                        {
                            CalcRangeExpression expression2 = expression as CalcRangeExpression;
                            int num = (expression2.EndColumn - expression2.StartColumn) + 1;
                            int num2 = (expression2.EndRow - expression2.StartRow) + 1;
                            vertical = (num > num2) ? DataOrientation.Horizontal : DataOrientation.Vertical;
                        }
                        WorksheetSeriesDataProvider provider2 = this.CreateSeriesDataProvider(new DefaultDataSeries(expression, vertical, this._dataSeries.DisplayHiddenData, this._dataSeries.EmptyValueStyle, null));
                        this._dataProviders.Add(provider2);
                        if (expression is CalcExternalExpression)
                        {
                            provider2.Worksheet = (expression as CalcExternalExpression).Source as Worksheet;
                        }
                        provider2.DataChanged += new EventHandler(this.DataProvider_DataChanged);
                    }
                }
            }
        }

        void UpdateIsDateTimeSeries()
        {
            bool flag = true;
            if (base.items.Count != 0)
            {
                using (List<T>.Enumerator enumerator = base.items.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (!(enumerator.Current is DateTime))
                        {
                            flag = false;
                            break;
                        }
                    }
                    goto Label_0051;
                }
            }
            flag = false;
        Label_0051:
            if (flag != this.IsDateTimeSeries)
            {
                this.IsDateTimeSeries = flag;
            }
        }

        internal bool AreAllDataSeriesInVisible
        {
            get
            {
                if ((this._dataProviders == null) || (this._dataProviders.Count == 0))
                {
                    return false;
                }
                foreach (ISeriesDataProvider provider in this._dataProviders)
                {
                    if (provider != null)
                    {
                        for (int i = 0; i < provider.SeriesCount; i++)
                        {
                            if (provider.IsSeriesVisible(i))
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Gets or sets the data series.
        /// </summary>
        /// <value>
        /// The data series.
        /// </value>
        public IDataSeries DataSeries
        {
            get { return  this._dataSeries; }
            set
            {
                if (value != this.DataSeries)
                {
                    this._dataSeries = value;
                    this.RefreshData();
                }
            }
        }

        internal bool IsBoundToDataSeries
        {
            get { return  (this.DataSeries != null); }
        }

        internal bool IsDateTimeSeries
        {
            get { return  this._isDateTimeSeries; }
            set
            {
                if (value != this.IsDateTimeSeries)
                {
                    this._isDateTimeSeries = value;
                    this.OnIsDateTimeSeriesChanged();
                }
            }
        }

        internal string ValuesSeperator
        {
            get { return  this._valuesSperator; }
            set
            {
                if (value != this.ValuesSeperator)
                {
                    this._valuesSperator = value;
                }
            }
        }
    }
}

