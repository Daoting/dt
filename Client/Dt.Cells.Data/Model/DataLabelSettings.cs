#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls.Chart;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Defines the settings for data label.
    /// </summary>
    public class DataLabelSettings : INotifyPropertyChanged, IXmlSerializable
    {
        Dt.Cells.Data.DataLabelPosition _dataLabelPosition = Dt.Cells.Data.DataLabelPosition.BestFit;
        bool _includeLeaderLines;
        bool _includeLegentKey;
        bool _isSeparatorSet;
        bool _isShowBubbleSizeSet;
        bool _isShowCategoryNameSet;
        bool _isShowPercentSet;
        bool _isShowSeriesNameSet;
        bool _isShowValueSet;
        string _separator = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
        bool _showBubbleSize;
        bool _showCategoryName;
        bool _showPercent;
        bool _showSeriesName;
        bool _showValue;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        void RaisePropertyChanged(string property)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        /// <summary>
        /// Resets the seperator property value.
        /// </summary>
        public void ResetSeperator()
        {
            this._separator = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
            this._isSeparatorSet = false;
        }

        /// <summary>
        /// Resets the ShowBubbleSize property value.
        /// </summary>
        public void ResetShowBubbleSize()
        {
            this._showBubbleSize = false;
            this._isShowBubbleSizeSet = false;
        }

        /// <summary>
        /// Resets the ShowCategoryName property value.
        /// </summary>
        public void ResetShowCategoryName()
        {
            this._showCategoryName = false;
            this._isShowCategoryNameSet = false;
        }

        /// <summary>
        /// Resets the ShowPercent property value.
        /// </summary>
        public void ResetShowPercent()
        {
            this._showPercent = false;
            this._isShowPercentSet = false;
        }

        /// <summary>
        /// Resets the ShowSeriesName property value.
        /// </summary>
        public void ResetShowSeriesName()
        {
            this._showSeriesName = false;
            this._isShowSeriesNameSet = false;
        }

        /// <summary>
        /// Resets the ShowValue property value.
        /// </summary>
        public void ResetShowValue()
        {
            this._showValue = false;
            this._isShowValueSet = false;
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            throw new NotImplementedException();
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            while (reader.Read())
            {
                string str;
                if ((reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element))) && ((str = reader.Name) != null))
                {
                    if (str != "ShowSeriesName")
                    {
                        if (str == "ShowPercent")
                        {
                            goto Label_00A4;
                        }
                        if (str == "ShowBubbleSize")
                        {
                            goto Label_00CB;
                        }
                        if (str == "ShowValue")
                        {
                            goto Label_00EF;
                        }
                        if (str == "ShowCategoryName")
                        {
                            goto Label_0113;
                        }
                        if (str == "DataLabelSeparator")
                        {
                            goto Label_0137;
                        }
                    }
                    else
                    {
                        this._showSeriesName = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                        this._isShowSeriesNameSet = true;
                    }
                }
                continue;
            Label_00A4:
                this._showPercent = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                this._isShowPercentSet = true;
                continue;
            Label_00CB:
                this._showBubbleSize = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                this._isShowBubbleSizeSet = true;
                continue;
            Label_00EF:
                this._showValue = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                this._isShowValueSet = true;
                continue;
            Label_0113:
                this._showCategoryName = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                this._isShowCategoryNameSet = true;
                continue;
            Label_0137:
                this._separator = (string) ((string) Serializer.DeserializeObj(typeof(string), reader));
                this._isSeparatorSet = true;
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            if (this.IsShowSeriesNameSet)
            {
                Serializer.SerializeObj((bool) this.ShowSeriesName, "ShowSeriesName", writer);
            }
            if (this.IsShowPercentSet)
            {
                Serializer.SerializeObj((bool) this.ShowPercent, "ShowPercent", writer);
            }
            if (this.IsShowBubbleSizeSet)
            {
                Serializer.SerializeObj((bool) this.ShowBubbleSize, "ShowBubbleSize", writer);
            }
            if (this.IsShowValueSet)
            {
                Serializer.SerializeObj((bool) this.ShowValue, "ShowValue", writer);
            }
            if (this.ShowCategoryName)
            {
                Serializer.SerializeObj((bool) this.ShowCategoryName, "ShowCategoryName", writer);
            }
            if (this.IsSeparatorSet)
            {
                Serializer.SerializeObj(this.Separator, "DataLabelSeparator", writer);
            }
        }

        internal List<IExcelDataLabel> DataLabelList { get; set; }

        internal FloatingObjectStyleInfo DataLabelsStyle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [include leader liens].
        /// </summary>
        /// <value>
        /// <c>true</c> if [include leader liens]; otherwise, <c>false</c>.
        /// </value>
        internal bool IncludeLeaderLines
        {
            get { return  this._includeLeaderLines; }
            set
            {
                this._includeLeaderLines = value;
                this.RaisePropertyChanged("IncludeLeaderLiens");
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to include the legend key.
        /// </summary>
        /// <value>
        /// <c>true</c> if [include legend key]; otherwise, <c>false</c>.
        /// </value>
        internal bool IncludeLegendKey
        {
            get { return  this._includeLegentKey; }
            set
            {
                this._includeLegentKey = value;
                this.RaisePropertyChanged("IncludeLegendKey");
            }
        }

        internal bool IsSeparatorSet
        {
            get { return  this._isSeparatorSet; }
        }

        internal bool IsShowBubbleSizeSet
        {
            get { return  this._isShowBubbleSizeSet; }
        }

        internal bool IsShowCategoryNameSet
        {
            get { return  this._isShowCategoryNameSet; }
        }

        internal bool IsShowPercentSet
        {
            get { return  this._isShowPercentSet; }
        }

        internal bool IsShowSeriesNameSet
        {
            get { return  this._isShowSeriesNameSet; }
        }

        internal bool IsShowValueSet
        {
            get { return  this._isShowValueSet; }
        }

        internal bool NumberFormatLinked { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        internal Dt.Cells.Data.DataLabelPosition Position
        {
            get { return  this._dataLabelPosition; }
            set
            {
                if (value != this._dataLabelPosition)
                {
                    this._dataLabelPosition = value;
                    this.RaisePropertyChanged("Position");
                }
            }
        }

        /// <summary>
        /// Gets or sets the separator.
        /// </summary>
        /// <value>
        /// The separator.
        /// </value>
        public string Separator
        {
            get { return  this._separator; }
            set
            {
                if (value != this.Separator)
                {
                    this._isSeparatorSet = true;
                    this._separator = value;
                    this.RaisePropertyChanged("Separator");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to show the bubble size.
        /// </summary>
        /// <value>
        /// <c>true</c> to show bubble size; otherwise, <c>false</c>.
        /// </value>
        public bool ShowBubbleSize
        {
            get { return  this._showBubbleSize; }
            set
            {
                this._isShowBubbleSizeSet = true;
                this._showBubbleSize = value;
                this.RaisePropertyChanged("ShowBubbleSize");
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to show the category name.
        /// </summary>
        /// <value>
        /// <c>true</c> to show the category name; otherwise, <c>false</c>.
        /// </value>
        public bool ShowCategoryName
        {
            get { return  this._showCategoryName; }
            set
            {
                this._isShowCategoryNameSet = true;
                this._showCategoryName = value;
                this.RaisePropertyChanged("ShowCategoryName");
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to show the percent.
        /// </summary>
        /// <value>
        /// <c>true</c> to show the percent; otherwise, <c>false</c>.
        /// </value>
        public bool ShowPercent
        {
            get { return  this._showPercent; }
            set
            {
                this._isShowPercentSet = true;
                this._showPercent = value;
                this.RaisePropertyChanged("ShowPercent");
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to show the series name.
        /// </summary>
        /// <value>
        /// <c>true</c> to show the series name; otherwise, <c>false</c>.
        /// </value>
        public bool ShowSeriesName
        {
            get { return  this._showSeriesName; }
            set
            {
                this._isShowSeriesNameSet = true;
                this._showSeriesName = value;
                this.RaisePropertyChanged("ShowSeriesName");
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to show the value.
        /// </summary>
        /// <value>
        /// <c>true</c> to show the value; otherwise, <c>false</c>.
        /// </value>
        public bool ShowValue
        {
            get { return  this._showValue; }
            set
            {
                this._isShowValueSet = true;
                this._showValue = value;
                this.RaisePropertyChanged("ShowValue");
            }
        }

        internal IExcelTextFormat TextFormat { get; set; }
    }
}

