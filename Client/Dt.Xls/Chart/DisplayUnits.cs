#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls.OOXml;
using Dt.Xls.Utils;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
#endregion

namespace Dt.Xls.Chart
{
    /// <summary>
    /// Specifies the scaling value of the display units for the value axis.
    /// </summary>
    public class DisplayUnits
    {
        private BuiltInDisplayUnitValue _builtInUnit;
        private long[] _builtInUnits = new long[] { 100L, 0x3e8L, 0x2710L, 0x186a0L, 0xf4240L, 0x989680L, 0x5f5e100L, 0x3b9aca00L, 0xe8d4a51000L };
        private double _customUnit;
        private bool _isCustomDisplayUnit;

        internal void ReadXml(XElement node, MemoryFolder mFolder, XFile xFile)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "custUnit")
                {
                    this.CustomDisplayUnit = element.GetAttributeValueOrDefaultOfDoubleType("val", 0.0);
                }
                else if (element.Name.LocalName == "builtInUnit")
                {
                    BuiltInDisplayUnitValue hundreds = BuiltInDisplayUnitValue.Hundreds;
                    Enum.TryParse<BuiltInDisplayUnitValue>(element.GetAttributeValueOrDefaultOfStringType("val", "hundreds"), true, out hundreds);
                    this.BuiltInDisplayUnit = hundreds;
                }
                else if (element.Name.LocalName == "dispUnitsLbl")
                {
                    foreach (XElement element2 in element.Elements())
                    {
                        if (element2.Name.LocalName == "layout")
                        {
                            Dt.Xls.Chart.Layout layout = new Dt.Xls.Chart.Layout();
                            layout.ReadXml(element2, mFolder, xFile);
                            this.Layout = layout;
                        }
                        else if (element2.Name.LocalName == "tx")
                        {
                            foreach (XElement element3 in element.Elements())
                            {
                                if (element3.Name.LocalName == "rich")
                                {
                                    Dt.Xls.Chart.RichText text = new Dt.Xls.Chart.RichText();
                                    text.ReadXml(element3, mFolder, xFile);
                                    this.RichText = text;
                                }
                                else if (element3.Name.LocalName == "strRef")
                                {
                                    this.TextFormula = element3.GetChildElementValue("f");
                                }
                            }
                        }
                    }
                }
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            using (writer.WriteElement("dispUnits", null, "c"))
            {
                if (this.IsCustomDisplayUnit && (this.CustomDisplayUnit > 0.0))
                {
                    writer.WriteLeafElementWithAttribute("custUnit", null, "c", "val", ((double) this.CustomDisplayUnit).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                }
                else if (this.BuiltInDisplayUnit != BuiltInDisplayUnitValue.None)
                {
                    writer.WriteLeafElementWithAttribute("builtInUnit", null, "c", "val", this.BuiltInDisplayUnit.ToString().ToCamelCase());
                }
                if (((this.Layout != null) || !string.IsNullOrWhiteSpace(this.TextFormula)) || (this.RichText != null))
                {
                    using (writer.WriteElement("dispUnitsLbl", null, "c"))
                    {
                        if (this.Layout != null)
                        {
                            this.Layout.WriteXml(writer, mFolder, chartFile);
                        }
                        if (this.RichText != null)
                        {
                            using (writer.WriteElement("tx", null, "c"))
                            {
                                this.RichText.WriteXml(writer, mFolder, chartFile);
                                return;
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(this.TextFormula))
                        {
                            writer.WriteStartElement("c", "tx", null);
                            writer.WriteStartElement("c", "strRef", null);
                            writer.WriteElementString("c", "f", null, this.TextFormula);
                            writer.WriteEndElement();
                            writer.WriteEndElement();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the built in display unit.
        /// </summary>
        /// <value>
        /// The built in display unit.
        /// </value>
        public BuiltInDisplayUnitValue BuiltInDisplayUnit
        {
            get
            {
                if (this._isCustomDisplayUnit)
                {
                    return BuiltInDisplayUnitValue.None;
                }
                return this._builtInUnit;
            }
            set
            {
                this._builtInUnit = value;
                this._isCustomDisplayUnit = false;
            }
        }

        /// <summary>
        /// Specifies a custom value for the display units.
        /// </summary>
        public double CustomDisplayUnit
        {
            get
            {
                if (this._isCustomDisplayUnit)
                {
                    return this._customUnit;
                }
                return double.NaN;
            }
            set
            {
                if (value <= 0.0)
                {
                    throw new ArgumentException("CustomDisplayUnit");
                }
                this._isCustomDisplayUnit = true;
                foreach (long num in this._builtInUnits)
                {
                    if ((value - num).IsZero())
                    {
                        this.BuiltInDisplayUnit = (BuiltInDisplayUnitValue) num;
                        break;
                    }
                }
                if (this.IsCustomDisplayUnit)
                {
                    this._customUnit = value;
                }
            }
        }

        /// <summary>
        /// Specifies use custom display unit or builtIn display unit.
        /// </summary>
        public bool IsCustomDisplayUnit
        {
            get { return  this._isCustomDisplayUnit; }
        }

        /// <summary>
        /// Specifies the layout
        /// </summary>
        public Dt.Xls.Chart.Layout Layout { get; set; }

        /// <summary>
        /// Specifies the text of the Display units in the rich text format
        /// </summary>
        public Dt.Xls.Chart.RichText RichText { get; set; }

        /// <summary>
        /// Specifies the display text.
        /// </summary>
        public string TextFormula { get; set; }
    }
}

