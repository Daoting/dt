#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine;
using Dt.CalcEngine.Expressions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a group of Excel-like sparklines.
    /// </summary>
    public class SparklineGroup : IXmlSerializable
    {
        WorksheetSparklineGroupManager _manager;
        DataOrientation axisOrientation;
        string cachedFormula;
        ISparklineData dateAxisData;
        CalcExpression dateAxisReference;
        List<Sparkline> innerList;
        bool isDateAxisDataSet;
        SparklineSetting setting;
        Dt.Cells.Data.SparklineType sparklineType;

        /// <summary>
        /// Initializes the sparkline group.
        /// </summary>
        public SparklineGroup()
        {
            this.Init();
        }

        internal SparklineGroup(Dt.Cells.Data.SparklineType type, SparklineSetting setting)
        {
            this.sparklineType = type;
            this.Setting = setting;
        }

        /// <summary>
        /// Adds a sparkline to the group.
        /// </summary>
        /// <param name="item">The object to add.</param>
        public void Add(Sparkline item)
        {
            if (!this.Contains(item) && (item != null))
            {
                this.InnerList.Add(item);
                item.Group = this;
                this.AdjustGroupMaxMinValue();
                this.OnGroupChanged();
            }
        }

        /// <summary>
        /// Adjusts the auto max min value.
        /// </summary>
        /// <returns>
        /// true if adjusted, false if skip.
        /// </returns>
        internal bool AdjustGroupMaxMinValue()
        {
            this.Setting.GroupMaxValue = double.MinValue;
            this.Setting.GroupMinValue = double.MaxValue;
            bool flag = this.Setting.MaxAxisType == SparklineAxisMinMax.Group;
            bool flag2 = this.Setting.MinAxisType == SparklineAxisMinMax.Group;
            bool flag3 = false;
            if (flag || flag2)
            {
                foreach (Sparkline sparkline in this.InnerList)
                {
                    double minValue = double.MinValue;
                    double maxValue = double.MaxValue;
                    this.GetMaxMinValues(sparkline, out minValue, out maxValue);
                    if (flag && (this.setting.GroupMaxValue < minValue))
                    {
                        this.Setting.GroupMaxValue = minValue;
                        flag3 = true;
                    }
                    if (flag2 && (this.Setting.GroupMinValue > maxValue))
                    {
                        this.Setting.GroupMinValue = maxValue;
                        flag3 = true;
                    }
                }
            }
            return flag3;
        }

        internal void Clear()
        {
            List<Sparkline> list = new List<Sparkline>((IEnumerable<Sparkline>) this.InnerList);
            foreach (Sparkline sparkline in list)
            {
                this.Remove(sparkline);
            }
        }

        internal SparklineGroup Clone()
        {
            SparklineGroup group = new SparklineGroup(this.sparklineType, this.setting.Clone()) {
                dateAxisReference = this.dateAxisReference,
                axisOrientation = this.axisOrientation
            };
            if (this.isDateAxisDataSet)
            {
                group.DateAxisData = this.DateAxisData;
            }
            return group;
        }

        /// <summary>
        /// Determines whether the group contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the group.</param>
        /// <returns>
        /// true if <paramref name="item" /> is found in the group; otherwise, false.
        /// </returns>
        public bool Contains(Sparkline item)
        {
            return this.InnerList.Contains(item);
        }

        internal void CopyTo(Sparkline[] array, int arrayIndex)
        {
            this.InnerList.CopyTo(array, arrayIndex);
        }

        void dateAxisData_DataChanged(object sender, EventArgs e)
        {
            this.OnGroupChanged();
        }

        internal void DetachEvents()
        {
            if (this.setting != null)
            {
                this.setting.PropertyChanged -= new PropertyChangedEventHandler(this.setting_PropertyChanged);
            }
            if (this.dateAxisData != null)
            {
                this.dateAxisData.DataChanged -= new EventHandler(this.dateAxisData_DataChanged);
            }
        }

        internal string GetDateAxisFormula(CalcParserContext context)
        {
            if (this.dateAxisReference == null)
            {
                return null;
            }
            if ((this.SparklineGroupManager != null) && (this.SparklineGroupManager.CalcEvaluator != null))
            {
                return this.SparklineGroupManager.CalcEvaluator.Expression2Formula(this.DateAxisReference, 0, 0);
            }
            CalcParser parser = new CalcParser();
            return parser.Unparse(this.dateAxisReference, context);
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        public IEnumerator<Sparkline> GetEnumerator()
        {
            return (IEnumerator<Sparkline>) this.InnerList.GetEnumerator();
        }

        void GetMaxMinValues(Sparkline sparkline, out double max, out double min)
        {
            max = double.MinValue;
            min = double.MaxValue;
            ISparklineData data = sparkline.Data;
            if (data != null)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    object obj2 = data.GetValue(i);
                    if (obj2 is double)
                    {
                        double num2 = (double) ((double) obj2);
                        max = (max < num2) ? num2 : max;
                        min = (min > num2) ? num2 : min;
                    }
                }
            }
        }

        internal void Init()
        {
            this.setting = null;
            this.innerList = null;
            this.sparklineType = Dt.Cells.Data.SparklineType.Line;
            this.dateAxisData = null;
            this.dateAxisReference = null;
            this.axisOrientation = DataOrientation.Vertical;
            this.SparklineGroupManager = null;
            this.cachedFormula = null;
        }

        bool IsNeedAdjustGroupMaxMinValue(Sparkline removedSparkline)
        {
            bool flag = this.setting.MaxAxisType == SparklineAxisMinMax.Group;
            bool flag2 = this.setting.MinAxisType == SparklineAxisMinMax.Group;
            bool flag3 = false;
            if (flag || flag2)
            {
                double minValue = double.MinValue;
                double maxValue = double.MaxValue;
                this.GetMaxMinValues(removedSparkline, out minValue, out maxValue);
                flag3 = (flag && (minValue == this.setting.GroupMaxValue)) || (flag2 && (maxValue == this.setting.GroupMinValue));
            }
            return flag3;
        }

        internal void OnGroupChanged()
        {
            SheetSparklineGroupDateProvider dateAxisData = this.DateAxisData as SheetSparklineGroupDateProvider;
            if (dateAxisData != null)
            {
                dateAxisData.ClearCache();
            }
            if (this.innerList != null)
            {
                foreach (Sparkline sparkline in this.innerList)
                {
                    if (sparkline != null)
                    {
                        sparkline.OnSparklineChanged();
                    }
                }
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the group.
        /// </summary>
        /// <param name="item">The object to remove from the group.</param>
        /// <returns>
        /// true if <paramref name="item" /> was successfully removed from the group; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the group.
        /// </returns>
        public bool Remove(Sparkline item)
        {
            bool flag = this.InnerList.Remove(item);
            if (flag)
            {
                item.Group = this.Clone();
                this.AdjustGroupMaxMinValue();
                this.OnGroupChanged();
            }
            return flag;
        }

        internal void ResumeAfterDeserialization()
        {
            if ((this.SparklineGroupManager != null) && (this.SparklineGroupManager.CalcEvaluator != null))
            {
                this.DateAxisReference = this.SparklineGroupManager.CalcEvaluator.Formula2Expression(this.cachedFormula, 0, 0) as CalcReferenceExpression;
                using (List<Sparkline>.Enumerator enumerator = this.InnerList.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.ResumeAfterDeserialization();
                    }
                }
            }
            this.AdjustGroupMaxMinValue();
        }

        void setting_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == "MinAxisType") || (e.PropertyName == "MaxAxisType"))
            {
                this.AdjustGroupMaxMinValue();
            }
            this.OnGroupChanged();
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.Init();
            Serializer.InitReader(reader);
            while (reader.Read())
            {
                XmlReader reader3;
                XmlReader reader4;
                XmlReader reader5;
                XmlReader reader6;
                string str;
                if ((reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element))) && ((str = reader.Name) != null))
                {
                    if (str != "SparklineType")
                    {
                        if (str == "DataOrientation")
                        {
                            goto Label_00AC;
                        }
                        if (str == "Formula")
                        {
                            goto Label_00D9;
                        }
                        if (str == "SparklineSetting")
                        {
                            goto Label_0106;
                        }
                        if (str == "SparklineList")
                        {
                            goto Label_0134;
                        }
                    }
                    else
                    {
                        XmlReader reader2 = Serializer.ExtractNode(reader);
                        this.SparklineType = (Dt.Cells.Data.SparklineType) Serializer.DeserializeObj(typeof(Dt.Cells.Data.SparklineType), reader2);
                        reader2.Close();
                    }
                }
                continue;
            Label_00AC:
                reader3 = Serializer.ExtractNode(reader);
                this.DateAxisOrientation = (DataOrientation) Serializer.DeserializeObj(typeof(DataOrientation), reader3);
                reader3.Close();
                continue;
            Label_00D9:
                reader4 = Serializer.ExtractNode(reader);
                this.cachedFormula = (string) ((string) Serializer.DeserializeObj(typeof(string), reader4));
                reader4.Close();
                continue;
            Label_0106:
                reader5 = Serializer.ExtractNode(reader);
                SparklineSetting setting = (SparklineSetting) Serializer.DeserializeObj(typeof(SparklineSetting), reader5);
                this.Setting = setting;
                reader5.Close();
                continue;
            Label_0134:
                reader6 = Serializer.ExtractNode(reader);
                List<Sparkline> list = new List<Sparkline>();
                Serializer.DeserializeList((IList) list, reader6);
                foreach (Sparkline sparkline in list)
                {
                    this.Add(sparkline);
                }
                reader6.Close();
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            Serializer.InitWriter(writer);
            if (this.SparklineType != Dt.Cells.Data.SparklineType.Line)
            {
                Serializer.SerializeObj(this.SparklineType, "SparklineType", writer);
            }
            if (this.DateAxisOrientation != DataOrientation.Vertical)
            {
                Serializer.SerializeObj(this.DateAxisOrientation, "DataOrientation", writer);
            }
            if (this.DateAxisReference != null)
            {
                string dateAxisFormula = this.GetDateAxisFormula(null);
                if (!string.IsNullOrEmpty(dateAxisFormula))
                {
                    Serializer.SerializeObj(dateAxisFormula, "Formula", writer);
                }
            }
            Serializer.SerializeObj(this.Setting, "SparklineSetting", writer);
            if ((this.innerList != null) && (this.innerList.Count > 0))
            {
                writer.WriteStartElement("SparklineList");
                Serializer.SerializeList((IList) this.innerList, writer);
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the group.
        /// </summary>
        /// <returns>The number of elements contained in the group.</returns>
        public int Count
        {
            get { return  this.InnerList.Count; }
        }

        internal ISparklineData DateAxisData
        {
            get
            {
                if (this.dateAxisData == null)
                {
                    this.dateAxisData = new SheetSparklineGroupDateProvider(this);
                    this.dateAxisData.DataChanged += new EventHandler(this.dateAxisData_DataChanged);
                }
                return this.dateAxisData;
            }
            set
            {
                if (this.dateAxisData != value)
                {
                    if (this.dateAxisData != null)
                    {
                        this.dateAxisData.DataChanged -= new EventHandler(this.dateAxisData_DataChanged);
                    }
                    this.dateAxisData = value;
                    if (this.dateAxisData != null)
                    {
                        this.dateAxisData.DataChanged += new EventHandler(this.dateAxisData_DataChanged);
                        this.isDateAxisDataSet = true;
                    }
                    else
                    {
                        this.isDateAxisDataSet = false;
                    }
                }
            }
        }

        internal DataOrientation DateAxisOrientation
        {
            get { return  this.axisOrientation; }
            set
            {
                if (this.axisOrientation != value)
                {
                    this.axisOrientation = value;
                    this.OnGroupChanged();
                }
            }
        }

        internal CalcExpression DateAxisReference
        {
            get { return  this.dateAxisReference; }
            set
            {
                if (this.dateAxisReference != value)
                {
                    this.dateAxisReference = value;
                    this.OnGroupChanged();
                }
            }
        }

        internal bool DisplayDateAxis
        {
            get { return  (this.DateAxisReference != null); }
        }

        List<Sparkline> InnerList
        {
            get
            {
                if (this.innerList == null)
                {
                    this.innerList = new List<Sparkline>();
                }
                return this.innerList;
            }
        }

        /// <summary>
        /// Gets the <see cref="T:Dt.Cells.Data.Sparkline" /> object at the specified index.
        /// </summary>
        public Sparkline this[int index]
        {
            get { return  this.InnerList[index]; }
        }

        /// <summary>
        /// Gets or sets the setting.
        /// </summary>
        /// <value>
        /// The setting.
        /// </value>
        public SparklineSetting Setting
        {
            get
            {
                if (this.setting == null)
                {
                    this.setting = new SparklineSetting();
                    this.setting.PropertyChanged += new PropertyChangedEventHandler(this.setting_PropertyChanged);
                }
                return this.setting;
            }
            set
            {
                if (value != this.setting)
                {
                    if (this.setting != null)
                    {
                        this.setting.PropertyChanged -= new PropertyChangedEventHandler(this.setting_PropertyChanged);
                    }
                    this.setting = value;
                    if (this.setting != null)
                    {
                        this.setting.PropertyChanged += new PropertyChangedEventHandler(this.setting_PropertyChanged);
                    }
                }
            }
        }

        internal WorksheetSparklineGroupManager SparklineGroupManager
        {
            get { return  this._manager; }
            set { this._manager = value; }
        }

        /// <summary>
        /// Gets or sets the sparkline type.
        /// </summary>
        public Dt.Cells.Data.SparklineType SparklineType
        {
            get { return  this.sparklineType; }
            set
            {
                if (this.sparklineType != value)
                {
                    this.sparklineType = value;
                    this.OnGroupChanged();
                }
            }
        }
    }
}

