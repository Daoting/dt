#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine.Expressions;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Windows.UI.Text;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TableStyleInfo : ICloneable, IXmlSerializable
    {
        Brush background;
        bool backgroundSet;
        string backgroundThemeColor;
        bool backgroundThemeColorSet;
        BorderLine bottomBorder;
        bool bottomBorderSet;
        FontStretch fontStretch;
        bool fontStretchSet;
        Windows.UI.Text.FontStyle fontStyle;
        bool fontStyleSet;
        FontWeight fontWeight;
        bool fontWeightSet;
        Brush foreground;
        bool foregroundSet;
        string foregroundThemeColor;
        bool foregroundThemeColorSet;
        BorderLine horizontalBorder;
        bool horizontalBorderSet;
        BorderLine leftBorder;
        bool leftBorderSet;
        BorderLine rightBorder;
        bool rightBorderSet;
        BorderLine topBorder;
        bool topBorderSet;
        BorderLine verticalBorder;
        bool verticalBorderSet;

        public TableStyleInfo()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue((string)null)]
        public Brush Background
        {
            get { return  this.background; }
            set
            {
                if (!object.Equals(this.background, value))
                {
                    this.background = value;
                    this.OnChanged(EventArgs.Empty);
                    this.RaisePropertyChanged("Background");
                }
                this.backgroundThemeColor = null;
                this.backgroundThemeColorSet = false;
                this.backgroundSet = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue((string)null)]
        public string BackgroundThemeColor
        {
            get { return  this.backgroundThemeColor; }
            set
            {
                if (this.backgroundThemeColor != value)
                {
                    this.backgroundThemeColor = value;
                    this.OnChanged(EventArgs.Empty);
                    this.RaisePropertyChanged("BackgroundThemeColor");
                }
                this.background = null;
                this.backgroundSet = false;
                this.backgroundThemeColorSet = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue((string)null)]
        public BorderLine BorderBottom
        {
            get { return  this.bottomBorder; }
            set
            {
                if ((value != null) && value.IsBuiltIn)
                {
                    throw new NotSupportedException(ResourceStrings.SetBuiltInBorderError);
                }
                if (!object.Equals(this.bottomBorder, value))
                {
                    this.bottomBorder = value;
                    this.OnChanged(EventArgs.Empty);
                    this.RaisePropertyChanged("BorderBottom");
                }
                this.bottomBorderSet = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue((string)null)]
        public BorderLine BorderHorizontal
        {
            get { return  this.horizontalBorder; }
            set
            {
                if ((value != null) && value.IsBuiltIn)
                {
                    throw new NotSupportedException(ResourceStrings.SetBuiltInBorderError);
                }
                if (!object.Equals(this.horizontalBorder, value))
                {
                    this.horizontalBorder = value;
                    this.OnChanged(EventArgs.Empty);
                    this.RaisePropertyChanged("BorderHorizontal");
                }
                this.horizontalBorderSet = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue((string)null)]
        public BorderLine BorderLeft
        {
            get { return  this.leftBorder; }
            set
            {
                if ((value != null) && value.IsBuiltIn)
                {
                    throw new NotSupportedException(ResourceStrings.SetBuiltInBorderError);
                }
                if (!object.Equals(this.leftBorder, value))
                {
                    this.leftBorder = value;
                    this.OnChanged(EventArgs.Empty);
                    this.RaisePropertyChanged("BorderLeft");
                }
                this.leftBorderSet = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue((string)null)]
        public BorderLine BorderRight
        {
            get { return  this.rightBorder; }
            set
            {
                if ((value != null) && value.IsBuiltIn)
                {
                    throw new NotSupportedException(ResourceStrings.SetBuiltInBorderError);
                }
                if (!object.Equals(this.rightBorder, value))
                {
                    this.rightBorder = value;
                    this.OnChanged(EventArgs.Empty);
                    this.RaisePropertyChanged("BorderRight");
                }
                this.rightBorderSet = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue((string)null)]
        public BorderLine BorderTop
        {
            get { return  this.topBorder; }
            set
            {
                if ((value != null) && value.IsBuiltIn)
                {
                    throw new NotSupportedException(ResourceStrings.SetBuiltInBorderError);
                }
                if (!object.Equals(this.topBorder, value))
                {
                    this.topBorder = value;
                    this.OnChanged(EventArgs.Empty);
                    this.RaisePropertyChanged("BorderTop");
                }
                this.topBorderSet = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue((string)null)]
        public BorderLine BorderVertical
        {
            get { return  this.verticalBorder; }
            set
            {
                if ((value != null) && value.IsBuiltIn)
                {
                    throw new NotSupportedException(ResourceStrings.SetBuiltInBorderError);
                }
                if (!object.Equals(this.verticalBorder, value))
                {
                    this.verticalBorder = value;
                    this.OnChanged(EventArgs.Empty);
                    this.RaisePropertyChanged("BorderVertical");
                }
                this.verticalBorderSet = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue((string)null)]
        public Brush Foreground
        {
            get { return  this.foreground; }
            set
            {
                if (!object.Equals(this.foreground, value))
                {
                    this.foreground = value;
                    this.OnChanged(EventArgs.Empty);
                    this.RaisePropertyChanged("Foreground");
                }
                this.foregroundThemeColor = null;
                this.foregroundThemeColorSet = false;
                this.foregroundSet = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue((string)null)]
        public string ForegroundThemeColor
        {
            get { return  this.foregroundThemeColor; }
            set
            {
                if (this.foregroundThemeColor != value)
                {
                    this.foregroundThemeColor = value;
                    this.OnChanged(EventArgs.Empty);
                    this.RaisePropertyChanged("ForegroundThemeColor");
                }
                this.foreground = null;
                this.foregroundSet = false;
                this.foregroundThemeColorSet = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(true)]
        public bool IsEmpty
        {
            get { return  (((((!this.fontStretchSet && !this.fontStyleSet) && (!this.fontWeightSet && !this.leftBorderSet)) && ((!this.topBorderSet && !this.rightBorderSet) && (!this.bottomBorderSet && !this.horizontalBorderSet))) && ((!this.verticalBorderSet && !this.backgroundSet) && (!this.foregroundSet && !this.backgroundThemeColorSet))) && !this.foregroundThemeColorSet); }
        }


        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(FontStretch.Normal)]
        public FontStretch FontStretch
        {
            get { return  this.fontStretch; }
            set
            {
                if (this.fontStretch != value)
                {
                    this.fontStretch = value;
                    this.OnChanged(EventArgs.Empty);
                    this.RaisePropertyChanged("FontStretch");
                }
                this.fontStretchSet = true;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(Windows.UI.Text.FontStyle.Normal)]
        public Windows.UI.Text.FontStyle FontStyle
        {
            get { return  this.fontStyle; }
            set
            {
                if (this.fontStyle != value)
                {
                    this.fontStyle = value;
                    this.OnChanged(EventArgs.Empty);
                    this.RaisePropertyChanged("FontStyle");
                }
                this.fontStyleSet = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public FontWeight FontWeight
        {
            get { return  this.fontWeight; }
            set
            {
                if (!this.fontWeight.Equals(value))
                {
                    this.fontWeight = value;
                    this.OnChanged(EventArgs.Empty);
                    this.RaisePropertyChanged("FontWeight");
                }
                this.fontWeightSet = true;
            }
        }

        public object Clone()
        {
            return new TableStyleInfo
            {
                background = this.background,
                backgroundSet = this.backgroundSet,
                backgroundThemeColor = this.backgroundThemeColor,
                backgroundThemeColorSet = this.backgroundThemeColorSet,
                foreground = this.foreground,
                foregroundSet = this.foregroundSet,
                foregroundThemeColor = this.foregroundThemeColor,
                foregroundThemeColorSet = this.foregroundThemeColorSet,
                leftBorder = (this.leftBorder == null) ? null : (this.leftBorder.Clone() as BorderLine),
                leftBorderSet = this.leftBorderSet,
                topBorder = (this.topBorder == null) ? null : (this.topBorder.Clone() as BorderLine),
                topBorderSet = this.topBorderSet,
                rightBorder = (this.rightBorder == null) ? null : (this.rightBorder.Clone() as BorderLine),
                rightBorderSet = this.rightBorderSet,
                bottomBorder = (this.bottomBorder == null) ? null : (this.bottomBorder.Clone() as BorderLine),
                bottomBorderSet = this.bottomBorderSet,
                fontStretch = this.fontStretch,
                fontStretchSet = this.fontStretchSet,
                fontStyle = this.fontStyle,
                fontStyleSet = this.fontStyleSet,
                fontWeight = this.fontWeight,
                fontWeightSet = this.fontWeightSet,
                horizontalBorder = (this.horizontalBorder == null) ? null : (this.horizontalBorder.Clone() as BorderLine),
                horizontalBorderSet = this.horizontalBorderSet,
                verticalBorder = (this.verticalBorder == null) ? null : (this.verticalBorder.Clone() as BorderLine),
                verticalBorderSet = this.verticalBorderSet
            };
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            TableStyleInfo info = obj as TableStyleInfo;
            if (info == null)
            {
                return false;
            }
            return (((((object.Equals(this.Background, info.Background) && object.Equals(this.BackgroundThemeColor, info.BackgroundThemeColor)) && (object.Equals(this.BorderBottom, info.BorderBottom) && object.Equals(this.BorderHorizontal, info.BorderHorizontal))) && ((object.Equals(this.BorderLeft, info.BorderLeft) && object.Equals(this.BorderRight, info.BorderRight)) && (object.Equals(this.BorderTop, info.BorderTop) && object.Equals(this.BorderVertical, info.BorderVertical)))) && ((object.Equals(this.FontStretch, info.FontStretch) && object.Equals(this.FontStyle, info.FontStyle)) && (object.Equals(this.FontWeight, info.FontWeight) && object.Equals(this.Foreground, info.Foreground)))) && object.Equals(this.ForegroundThemeColor, info.ForegroundThemeColor));
        }

        public void Init()
        {
            this.fontStretch = FontStretch.Normal;
            this.fontStyle = Windows.UI.Text.FontStyle.Normal;
            this.fontWeight = FontWeights.Normal;
            this.leftBorder = null;
            this.topBorder = null;
            this.rightBorder = null;
            this.bottomBorder = null;
            this.horizontalBorder = null;
            this.verticalBorder = null;
            this.background = null;
            this.foreground = null;
            this.backgroundThemeColor = null;
            this.foregroundThemeColor = null;
            this.fontStretchSet = false;
            this.fontStyleSet = false;
            this.fontWeightSet = false;
            this.leftBorderSet = false;
            this.topBorderSet = false;
            this.rightBorderSet = false;
            this.bottomBorderSet = false;
            this.horizontalBorderSet = false;
            this.verticalBorderSet = false;
            this.backgroundSet = false;
            this.foregroundSet = false;
            this.backgroundThemeColorSet = false;
            this.foregroundThemeColorSet = false;
        }

        public bool IsBackgroundSet()
        {
            return this.backgroundSet;
        }

        public bool IsBackgroundThemeColorSet()
        {
            return this.backgroundThemeColorSet;
        }

        public bool IsBorderBottomSet()
        {
            return this.bottomBorderSet;
        }

        public bool IsBorderHorizontalSet()
        {
            return this.horizontalBorderSet;
        }

        public bool IsBorderLeftSet()
        {
            return this.leftBorderSet;
        }

        public bool IsBorderRightSet()
        {
            return this.rightBorderSet;
        }

        public bool IsBorderTopSet()
        {
            return this.topBorderSet;
        }

        public bool IsBorderVerticalSet()
        {
            return this.verticalBorderSet;
        }

        public bool IsFontStretchSet()
        {
            return this.fontStretchSet;
        }

        public bool IsFontStyleSet()
        {
            return this.fontStyleSet;
        }

        public bool IsFontWeightSet()
        {
            return this.fontWeightSet;
        }

        public bool IsForegroundSet()
        {
            return this.foregroundSet;
        }

        public bool IsForegroundThemeColorSet()
        {
            return this.foregroundThemeColorSet;
        }

        void OnChanged(EventArgs e)
        {
        }

        void RaisePropertyChanged(string propertyName)
        {
        }

        internal void Render(StyleInfo dest, bool firstRow, bool firstColumn, bool lastRow, bool lastColumn)
        {
            if (this.fontStretchSet)
            {
                dest.FontStretch = this.fontStretch;
            }
            if (this.fontStyleSet)
            {
                dest.FontStyle = this.fontStyle;
            }
            if (this.fontWeightSet)
            {
                dest.FontWeight = this.fontWeight;
            }
            if (firstColumn && this.leftBorderSet)
            {
                dest.BorderLeft = this.leftBorder;
            }
            if (firstRow && this.topBorderSet)
            {
                dest.BorderTop = this.topBorder;
            }
            if (lastColumn && this.rightBorderSet)
            {
                dest.BorderRight = this.rightBorder;
            }
            if (lastRow && this.bottomBorderSet)
            {
                dest.BorderBottom = this.bottomBorder;
            }
            if (!lastRow && this.horizontalBorderSet)
            {
                dest.BorderBottom = this.horizontalBorder;
            }
            if (!lastColumn && this.verticalBorderSet)
            {
                dest.BorderRight = this.verticalBorder;
            }
            if (this.backgroundSet)
            {
                dest.Background = this.background;
            }
            if (this.foregroundSet)
            {
                dest.Foreground = this.foreground;
            }
            if (this.backgroundThemeColorSet)
            {
                dest.BackgroundThemeColor = this.backgroundThemeColor;
            }
            if (this.foregroundThemeColorSet)
            {
                dest.ForegroundThemeColor = this.foregroundThemeColor;
            }
        }

        public void Reset()
        {
            this.Init();
            this.OnChanged(EventArgs.Empty);
        }

        public void ResetBackground()
        {
            this.background = null;
            this.backgroundSet = false;
            this.OnChanged(EventArgs.Empty);
        }

        public void ResetBackgroundThemeColor()
        {
            this.backgroundThemeColor = null;
            this.backgroundThemeColorSet = false;
            this.OnChanged(EventArgs.Empty);
        }

        public void ResetBorderBottom()
        {
            this.bottomBorder = null;
            this.bottomBorderSet = false;
            this.OnChanged(EventArgs.Empty);
        }

        public void ResetBorderHorizontal()
        {
            this.horizontalBorder = null;
            this.horizontalBorderSet = false;
            this.OnChanged(EventArgs.Empty);
        }

        public void ResetBorderRight()
        {
            this.rightBorder = null;
            this.rightBorderSet = false;
            this.OnChanged(EventArgs.Empty);
        }

        public void ResetBorderTop()
        {
            this.topBorder = null;
            this.topBorderSet = false;
            this.OnChanged(EventArgs.Empty);
        }

        public void ResetBorderVertical()
        {
            this.verticalBorder = null;
            this.verticalBorderSet = false;
            this.OnChanged(EventArgs.Empty);
        }

        public void ResetFontStretch()
        {
            this.fontStretch = FontStretch.Normal;
            this.fontStretchSet = false;
            this.OnChanged(EventArgs.Empty);
        }

        public void ResetFontStyle()
        {
            this.fontStyle = Windows.UI.Text.FontStyle.Normal;
            this.fontStyleSet = false;
            this.OnChanged(EventArgs.Empty);
        }

        public void ResetFontWeight()
        {
            this.fontWeight = FontWeights.Normal;
            this.fontWeightSet = false;
            this.OnChanged(EventArgs.Empty);
        }

        public void ResetForeground()
        {
            this.foreground = null;
            this.foregroundSet = false;
            this.OnChanged(EventArgs.Empty);
        }

        public void ResetForegroundThemeColor()
        {
            this.foregroundThemeColor = null;
            this.foregroundThemeColorSet = false;
            this.OnChanged(EventArgs.Empty);
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            Serializer.InitReader(reader);
            while (reader.Read())
            {
                if (reader.NodeType == ((XmlNodeType)((int)XmlNodeType.Element)))
                {
                    switch (reader.Name)
                    {
                        case "FontStretch":
                            {
                                FontStretch? stretch = null;
                                string str = (string)(Serializer.DeserializeObj(typeof(string), reader) as string);
                                object obj2 = Serializer.DeserializeEnum(this.FontStretch.GetType(), str);
                                if (obj2 != null)
                                {
                                    stretch = new FontStretch?((FontStretch)obj2);
                                }
                                if (stretch.HasValue)
                                {
                                    this.fontStretch = stretch.Value;
                                    this.fontStretchSet = true;
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
                                    this.fontStyle = style.Value;
                                    this.fontStyleSet = true;
                                }
                                break;
                            }
                        case "FontWeight":
                            goto Label_0214;

                        case "LeftBorder":
                            this.leftBorder = Serializer.DeserializeObj(typeof(BorderLine), reader) as BorderLine;
                            this.leftBorderSet = true;
                            break;

                        case "TopBorder":
                            this.topBorder = Serializer.DeserializeObj(typeof(BorderLine), reader) as BorderLine;
                            this.topBorderSet = true;
                            break;

                        case "RightBorder":
                            this.rightBorder = Serializer.DeserializeObj(typeof(BorderLine), reader) as BorderLine;
                            this.rightBorderSet = true;
                            break;

                        case "BottomBorder":
                            this.bottomBorder = Serializer.DeserializeObj(typeof(BorderLine), reader) as BorderLine;
                            this.bottomBorderSet = true;
                            break;

                        case "HorizontalBorder":
                            this.horizontalBorder = Serializer.DeserializeObj(typeof(BorderLine), reader) as BorderLine;
                            this.horizontalBorderSet = true;
                            break;

                        case "VerticalBorder":
                            this.verticalBorder = Serializer.DeserializeObj(typeof(BorderLine), reader) as BorderLine;
                            this.verticalBorderSet = true;
                            break;

                        case "Background":
                            goto Label_033E;

                        case "Foreground":
                            goto Label_035E;

                        case "BackgroundTheme":
                            this.backgroundThemeColor = (string)(Serializer.DeserializeObj(typeof(string), reader) as string);
                            this.backgroundThemeColorSet = true;
                            break;

                        case "ForegroundTheme":
                            this.foregroundThemeColor = (string)(Serializer.DeserializeObj(typeof(string), reader) as string);
                            this.foregroundThemeColorSet = true;
                            break;
                    }
                }
                continue;
            Label_0214:
                string result = (string)(Serializer.DeserializeObj(typeof(string), reader) as string);
                FontWeight? weight = Serializer.FindStaticDefinationStruct<FontWeight>(typeof(FontWeights), result);
                if (weight.HasValue)
                {
                    this.fontWeight = weight.Value;
                    this.fontWeightSet = true;
                }
                continue;
            Label_033E:
                background = Serializer.DeserializeObj(typeof(Brush), reader) as Brush;
                this.backgroundSet = true;
                continue;
            Label_035E:
                foreground = Serializer.DeserializeObj(typeof(Brush), reader) as Brush;
                this.foregroundSet = true;
            }
            this.OnChanged(EventArgs.Empty);
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            Serializer.InitWriter(writer);
            if (this.fontStretchSet)
            {
                Serializer.SerializeObj(this.fontStretch.ToString(), "FontStretch", writer);
            }
            if (this.fontStyleSet)
            {
                Serializer.SerializeObj(this.fontStyle.ToString(), "FontStyle", writer);
            }
            if (this.fontWeightSet)
            {
                Serializer.SerializeObj(StyleInfo.GetFontWeightString(this.fontWeight), "FontWeight", writer);
            }
            if (this.leftBorderSet && (this.leftBorder != null))
            {
                Serializer.SerializeObj(this.leftBorder, "LeftBorder", writer);
            }
            if (this.topBorderSet && (this.topBorder != null))
            {
                Serializer.SerializeObj(this.topBorder, "TopBorder", writer);
            }
            if (this.rightBorderSet && (this.rightBorder != null))
            {
                Serializer.SerializeObj(this.rightBorder, "RightBorder", writer);
            }
            if (this.bottomBorderSet && (this.bottomBorder != null))
            {
                Serializer.SerializeObj(this.bottomBorder, "BottomBorder", writer);
            }
            if (this.horizontalBorderSet && (this.horizontalBorder != null))
            {
                Serializer.SerializeObj(this.horizontalBorder, "HorizontalBorder", writer);
            }
            if (this.verticalBorderSet && (this.verticalBorder != null))
            {
                Serializer.SerializeObj(this.verticalBorder, "VerticalBorder", writer);
            }
            if (this.backgroundSet && (this.background != null))
            {
                Serializer.SerializeObj(this.background, "Background", writer);
            }
            if (this.foregroundSet && (this.foreground != null))
            {
                Serializer.SerializeObj(this.foreground, "Foreground", writer);
            }
            if (this.backgroundThemeColorSet && (this.backgroundThemeColor != null))
            {
                Serializer.SerializeObj(this.backgroundThemeColor, "BackgroundTheme", writer);
            }
            if (this.foregroundThemeColorSet && (this.foregroundThemeColor != null))
            {
                Serializer.SerializeObj(this.foregroundThemeColor, "ForegroundTheme", writer);
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}