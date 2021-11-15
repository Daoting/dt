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
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml;
using System.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using Windows.UI.Text;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    internal class FloatingObjectStyleInfo : INotifyPropertyChanged, ICloneable, IXmlSerializable
    {
        double _cornerRadius;
        bool _cornerRadiusSet;
        Brush _fill;
        bool _fillSet;
        string _fillThemeColor;
        bool _fillThemeColorSet;
        Windows.UI.Xaml.Media.FontFamily _fontFamily;
        bool _fontFamilySet;
        double _fontSize;
        bool _fontSizeSet;
        Windows.UI.Text.FontStretch _fontStretch;
        bool _fontStretchSet;
        Windows.UI.Text.FontStyle _fontStyle;
        bool _fontStyleSet;
        string _fontTheme;
        bool _fontThemeSet;
        Windows.UI.Text.FontWeight _fontWeight;
        bool _fontWeightSet;
        Brush _foreground;
        bool _foregroundSet;
        string _foregroundThemeColor;
        bool _foregroundThemeColorSet;
        IFormatter _formatter;
        bool _formatterSet;
        ArrowSettings _lineBeginArrowSettings;
        bool _lineBeginArrowSettingsSet;
        PenLineCap _lineCapType;
        bool _lineCapTypeSet;
        ArrowSettings _lineEndArrowSettings;
        bool _lineEndArrowSettingsSet;
        PenLineJoin _lineJoinType;
        bool _lineJoinTypeSet;
        Brush _stroke;
        Dt.Cells.Data.StrokeDashType _strokeDashType;
        bool _strokeDashTypeSet;
        bool _strokeSet;
        string _strokeThemeColor;
        bool _strokeThemeColorSet;
        double _strokeThickness;
        bool _strokeThicknessSet;
        WorkingState _suspendState;
        IThemeSupport _themeContext;
        internal static string FONTTHEME_BODY = "Body";
        internal static string FONTTHEME_HEADING = "Headings";
        static PropertyInfo[] infos = null;

        public event PropertyChangedEventHandler PropertyChanged;

        public FloatingObjectStyleInfo()
        {
            this._suspendState = new WorkingState();
            this._lineJoinType = PenLineJoin.Round;
            this.Init();
        }

        public FloatingObjectStyleInfo(FloatingObjectStyleInfo format)
        {
            this._suspendState = new WorkingState();
            this._lineJoinType = PenLineJoin.Round;
            this.CopyFromInternal(format, true);
        }

        public object Clone()
        {
            return new FloatingObjectStyleInfo(this);
        }

        internal void CopyFromInternal(object o, bool clone)
        {
            if ((o is FloatingObjectStyleInfo) && !object.ReferenceEquals(o, this))
            {
                FloatingObjectStyleInfo info = (FloatingObjectStyleInfo)o;
                this._fontTheme = info._fontTheme;
                this._fontThemeSet = info._fontThemeSet;
                this._fontFamily = info._fontFamily;
                this._fontFamilySet = info._fontFamilySet;
                this._fontSize = info._fontSize;
                this._fontSizeSet = info._fontSizeSet;
                this._fontStretch = info._fontStretch;
                this._fontStretchSet = info._fontStretchSet;
                this._fontStyle = info._fontStyle;
                this._fontStyleSet = info._fontStyleSet;
                this._fontWeight = info._fontWeight;
                this._fontWeightSet = info._fontWeightSet;
                if (info._fillSet)
                {
                    ICloneable cloneable = info._fill as ICloneable;
                    if (cloneable != null)
                    {
                        this._fill = cloneable.Clone() as Brush;
                    }
                    else
                    {
                        this._fill = info._fill;
                    }
                    this._fillSet = true;
                }
                else
                {
                    this._fill = null;
                    this._fillSet = false;
                }
                if (info._foregroundSet)
                {
                    ICloneable cloneable2 = info._foreground as ICloneable;
                    if (cloneable2 != null)
                    {
                        this._foreground = cloneable2.Clone() as Brush;
                    }
                    else
                    {
                        this._foreground = info._foreground;
                    }
                    this._foregroundSet = true;
                }
                else
                {
                    this._foreground = null;
                    this._foregroundSet = false;
                }
                this._fillThemeColor = info._fillThemeColor;
                this._fillThemeColorSet = info._fillThemeColorSet;
                this._foregroundThemeColor = info._foregroundThemeColor;
                this._foregroundThemeColorSet = info._foregroundThemeColorSet;
                if (info._strokeSet)
                {
                    ICloneable cloneable3 = info._stroke as ICloneable;
                    if (cloneable3 != null)
                    {
                        this._stroke = cloneable3.Clone() as Brush;
                    }
                    else
                    {
                        this._stroke = info._stroke;
                    }
                    this._strokeSet = true;
                }
                else
                {
                    this._stroke = null;
                    this._strokeSet = false;
                }
                this._strokeDashType = info._strokeDashType;
                this._strokeDashTypeSet = info._strokeDashTypeSet;
                this._strokeThemeColor = info._strokeThemeColor;
                this._strokeThemeColorSet = info._strokeThemeColorSet;
                this._strokeThickness = info._strokeThickness;
                this._strokeThicknessSet = info._strokeThicknessSet;
                this._cornerRadius = info._cornerRadius;
                this._cornerRadiusSet = info._cornerRadiusSet;
                this._themeContext = info._themeContext;
                if (info._formatterSet)
                {
                    ICloneable cloneable4 = info._formatter as ICloneable;
                    if (cloneable4 != null)
                    {
                        this._formatter = cloneable4.Clone() as IFormatter;
                    }
                    else
                    {
                        this._formatter = info._formatter;
                    }
                    this._formatterSet = true;
                }
                else
                {
                    this._formatter = null;
                    this._formatterSet = false;
                }
                this._lineCapType = info._lineCapType;
                this._lineCapTypeSet = info._lineCapTypeSet;
                this._lineJoinType = info.LineJoinType;
                this._lineJoinTypeSet = info._lineJoinTypeSet;
                this._lineBeginArrowSettings = info._lineBeginArrowSettings;
                this._lineBeginArrowSettingsSet = info._lineBeginArrowSettingsSet;
                this._lineEndArrowSettings = info._lineEndArrowSettings;
                this._lineEndArrowSettingsSet = info._lineEndArrowSettingsSet;
            }
        }

        internal static string GetFontWeightString(Windows.UI.Text.FontWeight fontWeight)
        {
            if (infos == null)
            {
                // hdt
                infos = typeof(FontWeights).GetRuntimeProperties().ToArray<PropertyInfo>();
            }
            for (int i = 0; i < infos.Length; i++)
            {
                object obj2 = infos[i].GetValue(null);
                if ((obj2 != null) && (((Windows.UI.Text.FontWeight)obj2).Weight == fontWeight.Weight))
                {
                    return infos[i].Name;
                }
            }
            return "Normal";
        }

        void Init()
        {
            this._fontTheme = null;
            this._fontFamily = null;
            this._fontSize = -1.0;
            this._fontStretch = Windows.UI.Text.FontStretch.Normal;
            this._fontStyle = Windows.UI.Text.FontStyle.Normal;
            this._fontWeight = FontWeights.Normal;
            this._fill = null;
            this._fillThemeColor = null;
            this._foreground = null;
            this._foregroundThemeColor = null;
            this._strokeThickness = 1.0;
            this._stroke = null;
            this._strokeDashType = Dt.Cells.Data.StrokeDashType.None;
            this._strokeThemeColor = null;
            this._cornerRadius = -1.0;
            this._themeContext = null;
            this._cornerRadiusSet = false;
            this._fillSet = false;
            this._fillThemeColorSet = false;
            this._fontFamilySet = false;
            this._fontSizeSet = false;
            this._fontStretchSet = false;
            this._fontStyleSet = false;
            this._fontThemeSet = false;
            this._fontWeightSet = false;
            this._foregroundSet = false;
            this._foregroundThemeColorSet = false;
            this._strokeDashTypeSet = false;
            this._strokeSet = false;
            this._strokeThemeColorSet = false;
            this._strokeThicknessSet = false;
            this._formatter = null;
            this._formatterSet = false;
            this._lineCapType = PenLineCap.Flat;
            this._lineCapTypeSet = false;
            this._lineJoinType = PenLineJoin.Round;
            this._lineJoinTypeSet = false;
            this._lineBeginArrowSettings = null;
            this._lineBeginArrowSettingsSet = false;
            this._lineEndArrowSettings = null;
            this._lineEndArrowSettingsSet = false;
        }

        void LineBeginArrowSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged("LineBeginArrowSettings");
        }

        void LineEndArrowSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged("LineEndArrowSettings");
        }

        void RaisePropertyChanged(string property)
        {
            if (!this._suspendState.IsWorking && (this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public void Reset()
        {
            this.Init();
            this.RaisePropertyChanged("FloatingObjectStyleInfo");
        }

        public void ResetCornerRadius()
        {
            this._cornerRadius = -1.0;
            this._cornerRadiusSet = false;
            this.RaisePropertyChanged("CornerRadius");
        }

        public void ResetFill()
        {
            this._fill = null;
            this._fillSet = false;
            this.RaisePropertyChanged("Fill");
        }

        public void ResetFillThemeColor()
        {
            this._fillThemeColor = null;
            this._fillThemeColorSet = false;
            this.RaisePropertyChanged("FillThemeColor");
        }

        public void ResetFontFamily()
        {
            this._fontFamily = null;
            this._fontFamilySet = false;
            this.RaisePropertyChanged("FontFamily");
        }

        public void ResetFontSize()
        {
            this._fontSize = -1.0;
            this._fontSizeSet = false;
            this.RaisePropertyChanged("FontSize");
        }

        public void ResetFontStretch()
        {
            this._fontStretch = Windows.UI.Text.FontStretch.Normal;
            this._fontStretchSet = false;
            this.RaisePropertyChanged("FontStretch");
        }

        public void ResetFontStyle()
        {
            this._fontStyle = Windows.UI.Text.FontStyle.Normal;
            this._fontStyleSet = false;
            this.RaisePropertyChanged("FontStyle");
        }

        public void ResetFontTheme()
        {
            this._fontTheme = null;
            this._fontThemeSet = false;
            this.RaisePropertyChanged("FontTheme");
        }

        public void ResetFontWeight()
        {
            this._fontWeight = FontWeights.Normal;
            this._fontWeightSet = false;
            this.RaisePropertyChanged("FontWeight");
        }

        public void ResetForeground()
        {
            this._foreground = null;
            this._foregroundSet = false;
            this.RaisePropertyChanged("Foreground");
        }

        public void ResetForegroundThemeColor()
        {
            this._foregroundThemeColor = null;
            this._foregroundThemeColorSet = false;
            this.RaisePropertyChanged("ForegroundThemeColor");
        }

        public void ResetFormatter()
        {
            this._formatter = null;
            this._formatterSet = false;
            this.RaisePropertyChanged("Formatter");
        }

        public void ResetLineBeginArrowSettings()
        {
            this._lineBeginArrowSettings = null;
            this._lineBeginArrowSettingsSet = false;
            this.RaisePropertyChanged("LineBeginArrowSettings");
        }

        public void ResetLineCapType()
        {
            this._lineCapType = PenLineCap.Flat;
            this._lineCapTypeSet = false;
            this.RaisePropertyChanged("LineCapType");
        }

        public void ResetLineEndArrowSettings()
        {
            this._lineEndArrowSettings = null;
            this._lineEndArrowSettingsSet = false;
            this.RaisePropertyChanged("LineEndArrowSettings");
        }

        public void ResetLineJoinType()
        {
            this._lineJoinType = PenLineJoin.Round;
            this._lineJoinTypeSet = false;
            this.RaisePropertyChanged("LineJoinType");
        }

        public void ResetStroke()
        {
            this._stroke = null;
            this._strokeSet = false;
            this.RaisePropertyChanged("Stroke");
        }

        public void ResetStrokeDashType()
        {
            this._strokeDashType = Dt.Cells.Data.StrokeDashType.None;
            this._strokeDashTypeSet = false;
            this.RaisePropertyChanged("StrokeDashes");
        }

        public void ResetStrokeThemeColor()
        {
            this._strokeThemeColor = null;
            this._strokeThemeColorSet = false;
            this.RaisePropertyChanged("StrokeThemeColor");
        }

        public void ResetStrokeThickness()
        {
            this._strokeThickness = 1.0;
            this._strokeThicknessSet = false;
            this.RaisePropertyChanged("StrokeThickness");
        }

        internal void ResumeEvents()
        {
            this._suspendState.Release();
        }

        internal void SuspendEvents()
        {
            this._suspendState.AddRef();
        }

        /// <summary>
        /// This method is reserved and should not be used. When implementing the <see cref="T:System.Xml.Serialization.IXmlSerializable" /> interface, you should return a null reference (Nothing in Visual Basic) from this method.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml" /> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml" /> method.
        /// </returns>
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized.</param>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            Serializer.InitReader(reader);
            this.Reset();
            while (reader.Read())
            {
                if (reader.NodeType == ((XmlNodeType)((int)XmlNodeType.Element)))
                {
                    switch (reader.Name)
                    {
                        case "FontTheme":
                            this._fontTheme = (string)(Serializer.DeserializeObj(typeof(string), reader) as string);
                            this._fontThemeSet = true;
                            break;

                        case "FontFamily":
                            if (!(Serializer.ReadAttribute("value", reader) == Serializer.Format(null)))
                            {
                                goto Label_0239;
                            }
                            this._fontFamily = null;
                            goto Label_0279;

                        case "FontSize":
                            this._fontSize = (float)Serializer.DeserializeObj(typeof(float), reader);
                            this._fontSizeSet = true;
                            break;

                        case "FontStretch":
                            {
                                Windows.UI.Text.FontStretch? stretch = null;
                                string str = (string)(Serializer.DeserializeObj(typeof(string), reader) as string);
                                object obj2 = Serializer.DeserializeEnum(this.FontStretch.GetType(), str);
                                if (obj2 != null)
                                {
                                    stretch = new Windows.UI.Text.FontStretch?((Windows.UI.Text.FontStretch)obj2);
                                }
                                if (stretch.HasValue)
                                {
                                    this._fontStretch = stretch.Value;
                                    this._fontStretchSet = true;
                                }
                                break;
                            }
                        case "FontStyle":
                            {
                                Windows.UI.Text.FontStyle? style = null;
                                string str = (string)(Serializer.DeserializeObj(typeof(string), reader) as string);
                                object obj2 = Serializer.DeserializeEnum(this.FontStyle.GetType(), str);
                                if (obj2 != null)
                                {
                                    style = new Windows.UI.Text.FontStyle?((Windows.UI.Text.FontStyle)obj2);
                                }
                                if (style.HasValue)
                                {
                                    this._fontStyle = style.Value;
                                    this._fontStyleSet = true;
                                }
                                break;
                            }
                        case "FontWeight":
                            {
                                string result = (string)(Serializer.DeserializeObj(typeof(string), reader) as string);
                                Windows.UI.Text.FontWeight? weight = Serializer.FindStaticDefinationStruct<Windows.UI.Text.FontWeight>(typeof(FontWeights), result);
                                if (weight.HasValue)
                                {
                                    this._fontWeight = weight.Value;
                                    this._fontWeightSet = true;
                                }
                                break;
                            }
                        case "Fill":
                            goto Label_03E3;

                        case "Foreground":
                            goto Label_0409;

                        case "FillTheme":
                            this._fillThemeColor = (string)(Serializer.DeserializeObj(typeof(string), reader) as string);
                            this._fillThemeColorSet = true;
                            break;

                        case "ForegroundTheme":
                            this._foregroundThemeColor = (string)(Serializer.DeserializeObj(typeof(string), reader) as string);
                            this._foregroundThemeColorSet = true;
                            break;

                        case "Formatter":
                            if (!(Serializer.ReadAttribute("value", reader) == Serializer.Format(null)))
                            {
                                goto Label_04B4;
                            }
                            this._formatter = null;
                            goto Label_04E2;

                        case "Stroke":
                            goto Label_04EE;

                        case "StrokeTheme":
                            this._strokeThemeColor = (string)(Serializer.DeserializeObj(typeof(string), reader) as string);
                            this._strokeThemeColorSet = true;
                            break;

                        case "StrokeThickness":
                            this._strokeThickness = (double)((double)Serializer.DeserializeObj(typeof(double), reader));
                            this._strokeThicknessSet = true;
                            break;

                        case "CornerRadius":
                            this._cornerRadius = (double)((double)Serializer.DeserializeObj(typeof(double), reader));
                            this._cornerRadiusSet = true;
                            break;

                        case "StrokeDashes":
                            this._strokeDashType = (Dt.Cells.Data.StrokeDashType)Serializer.DeserializeObj(typeof(Dt.Cells.Data.StrokeDashType), reader);
                            this._strokeDashTypeSet = true;
                            break;

                        case "LineCapType":
                            this._lineCapType = (PenLineCap)Serializer.DeserializeObj(typeof(PenLineCap), reader);
                            this._lineCapTypeSet = true;
                            break;

                        case "LineJoinType":
                            this._lineJoinType = (PenLineJoin)Serializer.DeserializeObj(typeof(PenLineJoin), reader);
                            this._lineJoinTypeSet = true;
                            break;

                        case "LineBeginArrowSettings":
                            this._lineBeginArrowSettings = (ArrowSettings)Serializer.DeserializeObj(typeof(ArrowSettings), reader);
                            this._lineBeginArrowSettingsSet = true;
                            break;

                        case "LineEndArrowSettings":
                            this._lineEndArrowSettings = (ArrowSettings)Serializer.DeserializeObj(typeof(ArrowSettings), reader);
                            this._lineEndArrowSettingsSet = true;
                            break;
                    }
                }
                continue;
            Label_0239:
                _fontFamily = new FontFamily(Serializer.DeserializeObj(typeof(string), reader) as string);
            Label_0279:
                this._fontFamilySet = true;
                continue;
            Label_03E3:
                _fill = Serializer.DeserializeObj(typeof(Brush), reader) as Brush;
                this._fillSet = true;
                continue;
            Label_0409:
                _foreground = Serializer.DeserializeObj(typeof(Brush), reader) as Brush;
                this._foregroundSet = true;
                continue;
            Label_04B4:
                string str3 = (string)(Serializer.DeserializeObj(typeof(string), reader) as string);
                if (str3 != null)
                {
                    this._formatter = new GeneralFormatter(str3);
                }
            Label_04E2:
                this._formatterSet = true;
                continue;
            Label_04EE:
                _stroke = Serializer.DeserializeObj(typeof(Brush), reader) as Brush;
                this._strokeSet = true;
            }
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized.</param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            Serializer.InitWriter(writer);
            if (this._fontThemeSet)
            {
                Serializer.SerializeObj(this._fontTheme, "FontTheme", writer);
            }
            if (this._fontFamilySet)
            {
                if (this._fontFamily == null)
                {
                    writer.WriteStartElement("FontFamily");
                    Serializer.WriteAttr("value", Serializer.Format(null), writer);
                    writer.WriteEndElement();
                }
                else
                {
                    Serializer.SerializeObj(this._fontFamily.Source, "FontFamily", writer);
                }
            }
            if (this._fontSizeSet)
            {
                Serializer.SerializeObj((double)this._fontSize, "FontSize", writer);
            }
            if (this._fontStretchSet)
            {
                Serializer.SerializeObj(this._fontStretch.ToString(), "FontStretch", writer);
            }
            if (this._fontStyleSet)
            {
                Serializer.SerializeObj(this._fontStyle.ToString(), "FontStyle", writer);
            }
            if (this._fontWeightSet)
            {
                Serializer.SerializeObj(StyleInfo.GetFontWeightString(this._fontWeight), "FontWeight", writer);
            }
            if (this._fillSet)
            {
                Serializer.SerializeObj(this._fill, "Fill", writer);
            }
            if (this._foregroundSet)
            {
                Serializer.SerializeObj(this._foreground, "Foreground", writer);
            }
            if (this._fillThemeColorSet)
            {
                Serializer.SerializeObj(this._fillThemeColor, "FillTheme", writer);
            }
            if (this._foregroundThemeColorSet)
            {
                Serializer.SerializeObj(this._foregroundThemeColor, "ForegroundTheme", writer);
            }
            if (this._strokeSet)
            {
                Serializer.SerializeObj(this._stroke, "Stroke", writer);
            }
            if (this._strokeThemeColorSet)
            {
                Serializer.SerializeObj(this._strokeThemeColor, "StrokeTheme", writer);
            }
            if (this._strokeThicknessSet)
            {
                Serializer.SerializeObj((double)this._strokeThickness, "StrokeThickness", writer);
            }
            if (this._cornerRadiusSet)
            {
                Serializer.SerializeObj((double)this._cornerRadius, "CornerRadius", writer);
            }
            if (this._strokeDashTypeSet && (this._strokeDashType != Dt.Cells.Data.StrokeDashType.None))
            {
                Serializer.SerializeObj(this._strokeDashType, "StrokeDashes", writer);
            }
            if (this._formatterSet)
            {
                if (this._formatter == null)
                {
                    writer.WriteStartElement("Formatter");
                    Serializer.WriteAttr("value", Serializer.Format(null), writer);
                    writer.WriteEndElement();
                }
                else
                {
                    Serializer.SerializeObj(this._formatter.FormatString, "Formatter", writer);
                }
            }
            if (this._lineCapTypeSet)
            {
                Serializer.SerializeObj(this._lineCapType, "LineCapType", writer);
            }
            if (this._lineJoinTypeSet)
            {
                Serializer.SerializeObj(this._lineJoinType, "LineJoinType", writer);
            }
            if (this._lineBeginArrowSettingsSet && (this._lineBeginArrowSettings != null))
            {
                Serializer.SerializeObj(this._lineBeginArrowSettings, "LineBeginArrowSettings", writer);
            }
            if (this._lineEndArrowSettingsSet && (this._lineEndArrowSettings != null))
            {
                Serializer.SerializeObj(this._lineEndArrowSettings, "LineEndArrowSettings", writer);
            }
        }

        public double CornerRadius
        {
            get { return this._cornerRadius; }
            set
            {
                this._cornerRadiusSet = true;
                if (this._cornerRadius != value)
                {
                    this._cornerRadius = value;
                    this.RaisePropertyChanged("CornerRadius");
                }
            }
        }

        public Brush Fill
        {
            get { return this._fill; }
            set
            {
                this._fillThemeColor = null;
                this._fillThemeColorSet = false;
                this._fillSet = true;
                if (value != this._fill)
                {
                    this._fill = value;
                    this.RaisePropertyChanged("Fill");
                }
            }
        }

        internal ExcelDrawingColorSettings FillDrawingColorSettings { get; set; }

        public string FillThemeColor
        {
            get { return this._fillThemeColor; }
            set
            {
                this._fill = null;
                this._fillSet = false;
                this._fillThemeColorSet = true;
                if (this._fillThemeColor != value)
                {
                    this._fillThemeColor = value;
                    this.RaisePropertyChanged("FillThemeColor");
                }
            }
        }

        public Windows.UI.Xaml.Media.FontFamily FontFamily
        {
            get { return this._fontFamily; }
            set
            {
                this._fontTheme = null;
                this._fontThemeSet = false;
                this._fontFamilySet = true;
                if (this._fontFamily != value)
                {
                    this._fontFamily = value;
                    this.RaisePropertyChanged("FontFamily");
                }
            }
        }

        public double FontSize
        {
            get { return this._fontSize; }
            set
            {
                this._fontSizeSet = true;
                if (this._fontSize != value)
                {
                    this._fontSize = value;
                    this.RaisePropertyChanged("FontSize");
                }
            }
        }

        public Windows.UI.Text.FontStretch FontStretch
        {
            get { return this._fontStretch; }
            set
            {
                this._fontStretchSet = true;
                if (this._fontStretch != value)
                {
                    this._fontStretch = value;
                    this.RaisePropertyChanged("FontStretch");
                }
            }
        }

        public Windows.UI.Text.FontStyle FontStyle
        {
            get { return this._fontStyle; }
            set
            {
                this._fontStyleSet = true;
                if (this._fontStyle != value)
                {
                    this._fontStyle = value;
                    this.RaisePropertyChanged("FontStyle");
                }
            }
        }

        public string FontTheme
        {
            get { return this._fontTheme; }
            set
            {
                this._fontFamily = null;
                this._fontFamilySet = false;
                this._fontThemeSet = true;
                if (this._fontTheme != value)
                {
                    this._fontTheme = value;
                    this.RaisePropertyChanged("FontTheme");
                }
            }
        }

        public Windows.UI.Text.FontWeight FontWeight
        {
            get { return this._fontWeight; }
            set
            {
                this._fontWeightSet = true;
                if (this._fontWeight.Weight != value.Weight)
                {
                    this._fontWeight = value;
                    this.RaisePropertyChanged("FontWeight");
                }
            }
        }

        public Brush Foreground
        {
            get { return this._foreground; }
            set
            {
                this._foregroundThemeColor = null;
                this._foregroundThemeColorSet = false;
                this._foregroundSet = true;
                if (value != this.Foreground)
                {
                    this._foreground = value;
                    this.RaisePropertyChanged("Foreground");
                }
            }
        }

        public string ForegroundThemeColor
        {
            get { return this._foregroundThemeColor; }
            set
            {
                this._foreground = null;
                this._foregroundSet = false;
                this._foregroundThemeColorSet = true;
                if (this._foregroundThemeColor != value)
                {
                    this._foregroundThemeColor = value;
                    this.RaisePropertyChanged("ForegroundThemeColor");
                }
            }
        }

        public IFormatter Formatter
        {
            get { return this._formatter; }
            set
            {
                this._formatterSet = true;
                if (!object.Equals(this._formatter, value))
                {
                    this._formatter = value;
                    this.RaisePropertyChanged("Formatter");
                }
            }
        }

        public bool IsCornerRadiusSet
        {
            get { return this._cornerRadiusSet; }
        }

        public bool IsEmpty
        {
            get { return ((((((!this._cornerRadiusSet && !this._fillSet) && (!this._fillThemeColorSet && !this._fontFamilySet)) && ((!this._fontSizeSet && !this._fontStretchSet) && (!this._fontStyleSet && !this._fontThemeSet))) && (((!this._fontWeightSet && !this._foregroundSet) && (!this._foregroundThemeColorSet && !this._strokeDashTypeSet)) && ((!this._strokeSet && !this._strokeThemeColorSet) && (!this._strokeThicknessSet && !this._formatterSet)))) && ((!this._lineCapTypeSet && !this._lineJoinTypeSet) && !this._lineBeginArrowSettingsSet)) && !this._lineEndArrowSettingsSet); }
        }

        public bool IsFillSet
        {
            get { return this._fillSet; }
        }

        public bool IsFillThemeColorSet
        {
            get { return this._fillThemeColorSet; }
        }

        public bool IsFontFamilySet
        {
            get { return this._fontFamilySet; }
        }

        public bool IsFontSizeSet
        {
            get { return this._fontSizeSet; }
        }

        public bool IsFontStretchSet
        {
            get { return this._fontStretchSet; }
        }

        public bool IsFontStyleSet
        {
            get { return this._fontStyleSet; }
        }

        public bool IsFontThemeSet
        {
            get { return this._fontThemeSet; }
        }

        public bool IsFontWeightSet
        {
            get { return this._fontWeightSet; }
        }

        public bool IsForegroundSet
        {
            get { return this._foregroundSet; }
        }

        public bool IsForegroundThemeColorSet
        {
            get { return this._foregroundThemeColorSet; }
        }

        public bool IsFormatterSet
        {
            get { return this._formatterSet; }
        }

        /// <summary>
        /// Gets a value that indicates whether this instance is a line begin arrow setting.
        /// </summary>
        /// <value>
        /// <c>true</c> if the line begin arrow setting is set; otherwise, <c>false</c>.
        /// </value>
        public bool IsLineBeginArrowSettingsSet
        {
            get { return this._lineBeginArrowSettingsSet; }
        }

        /// <summary>
        /// Gets a value that indicates whether the line cap type is set for this instance.
        /// </summary>
        /// <value>
        /// <c>true</c> if the line cap type is set; otherwise, <c>false</c>.
        /// </value>
        public bool IsLineCapTypeSet
        {
            get { return this._lineCapTypeSet; }
        }

        /// <summary>
        /// Gets a value that indicates whether the line end arrow setting is set.
        /// </summary>
        /// <value>
        /// <c>true</c> if the line end arrow setting is set; otherwise, <c>false</c>.
        /// </value>
        public bool IsLineEndArrowSettingsSet
        {
            get { return this._lineEndArrowSettingsSet; }
        }

        /// <summary>
        /// Gets a value that indicates whether this instance is a type of line join.
        /// </summary>
        /// <value>
        /// <c>true</c> if the line join type is set; otherwise, <c>false</c>.
        /// </value>
        public bool IsLineJoinTypeSet
        {
            get { return this._lineJoinTypeSet; }
        }

        public bool IsStrokeDashTypeSet
        {
            get { return this._strokeDashTypeSet; }
        }

        public bool IsStrokeSet
        {
            get { return this._strokeSet; }
        }

        public bool IsStrokeThemeColorSet
        {
            get { return this._strokeThemeColorSet; }
        }

        public bool IsStrokeThicknessSet
        {
            get { return this._strokeThicknessSet; }
        }

        /// <summary>
        /// Gets or sets the line begin arrow settings.
        /// </summary>
        /// <value>
        /// The line begin arrow settings.
        /// </value>
        public ArrowSettings LineBeginArrowSettings
        {
            get { return this._lineBeginArrowSettings; }
            set
            {
                this._lineBeginArrowSettingsSet = true;
                if (value != this.LineBeginArrowSettings)
                {
                    if (this._lineBeginArrowSettings != null)
                    {
                        this._lineBeginArrowSettings.PropertyChanged -= new PropertyChangedEventHandler(this.LineBeginArrowSettings_PropertyChanged);
                    }
                    this._lineBeginArrowSettings = value;
                    if (this._lineBeginArrowSettings != null)
                    {
                        this._lineBeginArrowSettings.PropertyChanged += new PropertyChangedEventHandler(this.LineBeginArrowSettings_PropertyChanged);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of the line cap.
        /// </summary>
        /// <value>
        /// The type of the line cap.
        /// </value>
        public PenLineCap LineCapType
        {
            get { return this._lineCapType; }
            set
            {
                this._lineCapTypeSet = true;
                if (value != this.LineCapType)
                {
                    this._lineCapType = value;
                    this.RaisePropertyChanged("LineCapType");
                }
            }
        }

        /// <summary>
        /// Gets or sets the line end arrow settings.
        /// </summary>
        /// <value>
        /// The line end arrow settings.
        /// </value>
        public ArrowSettings LineEndArrowSettings
        {
            get { return this._lineEndArrowSettings; }
            set
            {
                this._lineEndArrowSettingsSet = true;
                if (value != this.LineEndArrowSettings)
                {
                    if (this._lineEndArrowSettings != null)
                    {
                        this._lineEndArrowSettings.PropertyChanged -= new PropertyChangedEventHandler(this.LineEndArrowSettings_PropertyChanged);
                    }
                    this._lineEndArrowSettings = value;
                    if (this._lineEndArrowSettings != null)
                    {
                        this._lineEndArrowSettings.PropertyChanged += new PropertyChangedEventHandler(this.LineEndArrowSettings_PropertyChanged);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of the line join.
        /// </summary>
        /// <value>
        /// The type of the line join.
        /// </value>
        public PenLineJoin LineJoinType
        {
            get { return this._lineJoinType; }
            set
            {
                this._lineJoinTypeSet = true;
                if (value != this.LineJoinType)
                {
                    this._lineJoinType = value;
                    this.RaisePropertyChanged("LineJoinType");
                }
            }
        }

        public Brush Stroke
        {
            get { return this._stroke; }
            set
            {
                this._strokeThemeColor = null;
                this._strokeThemeColorSet = false;
                this._strokeSet = true;
                if (this._stroke != value)
                {
                    this._stroke = value;
                    this.RaisePropertyChanged("Stroke");
                }
            }
        }

        public Dt.Cells.Data.StrokeDashType StrokeDashType
        {
            get { return this._strokeDashType; }
            set
            {
                this._strokeDashTypeSet = true;
                if (this._strokeDashType != value)
                {
                    this._strokeDashType = value;
                    this.RaisePropertyChanged("StrokeDashes");
                }
            }
        }

        internal ExcelDrawingColorSettings StrokeDrawingColorSettings { get; set; }

        public string StrokeThemeColor
        {
            get { return this._strokeThemeColor; }
            set
            {
                this._stroke = null;
                this._strokeSet = false;
                this._strokeThemeColorSet = true;
                if (this._strokeThemeColor != value)
                {
                    this._strokeThemeColor = value;
                    this.RaisePropertyChanged("StrokeThemeColor");
                }
            }
        }

        public double StrokeThickness
        {
            get { return this._strokeThickness; }
            set
            {
                this._strokeThicknessSet = true;
                if (this._strokeThickness != value)
                {
                    this._strokeThickness = value;
                    this.RaisePropertyChanged("StrokeThickness");
                }
            }
        }
    }
}

