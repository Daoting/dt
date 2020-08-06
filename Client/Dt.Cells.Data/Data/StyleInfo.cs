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
using System.Linq;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class StyleInfo : INotifyPropertyChanged, IXmlSerializable, IsEmptySupport
    {
        internal static string FONTTHEME_BODY;
        internal static string FONTTHEME_HEADING;
        internal BorderLine BorderHorizontal;
        internal BorderLine BorderVertical;
        Brush background;
        bool backgroundSet;
        string backgroundThemeColor;
        bool backgroundThemeColorSet;
        BorderLine bottomBorder;
        bool bottomBorderSet;
        bool canFocus;
        bool canFocusSet;
        DataValidator dataValidator;
        bool dataValidatorSet;
        FontFamily fontFamily;
        bool fontFamilySet;
        double fontSize;
        bool fontSizeSet;
        FontStretch fontStretch;
        bool fontStretchSet;
        Windows.UI.Text.FontStyle fontStyle;
        bool fontStyleSet;
        string fontTheme;
        bool fontThemeSet;
        FontWeight fontWeight;
        bool fontWeightSet;
        Brush foreground;
        bool foregroundSet;
        string foregroundThemeColor;
        bool foregroundThemeColorSet;
        IFormatter formatter;
        bool formatterSet;
        CellHorizontalAlignment horizontalAlignment;
        bool horizontalAlignmentSet;
        static PropertyInfo[] infos;
        BorderLine leftBorder;
        bool leftBorderSet;
        bool locked;
        bool lockedSet;
        string name;
        bool nameSet;
        string parent;
        bool parentSet;
        BorderLine rightBorder;
        bool rightBorderSet;
        bool shrinkToFit;
        bool shrinkToFitSet;
        internal bool suspendEvents;
        bool tabStop;
        bool tabStopSet;
        int textIndent;
        bool textIndentSet;
        BorderLine topBorder;
        bool topBorderSet;
        CellVerticalAlignment verticalAlignment;
        bool verticalAlignmentSet;
        bool wordWrap;
        bool wordWrapSet;
        bool strikethrough;
        bool isStrikethroughSet;
        bool underline;
        bool isUnderlineSet;

        static StyleInfo()
        {
            FONTTHEME_BODY = "Body";
            FONTTHEME_HEADING = "Headings";
            infos = null;
        }

        public StyleInfo()
            : this(null, null, null)
        {
        }

        public StyleInfo(StyleInfo info)
        {
            this.name = string.Empty;
            this.canFocus = true;
            this.locked = true;
            this.tabStop = true;
            this.CopyFromInternal(info, true, false);
        }

        public StyleInfo(string name)
            : this(name, null, null)
        {
        }

        public StyleInfo(string name, string parentName)
            : this(name, parentName, null)
        {
        }

        public StyleInfo(string name, string parentName, StyleInfo style)
        {
            this.name = string.Empty;
            this.canFocus = true;
            this.locked = true;
            this.tabStop = true;
            if (style == null)
            {
                this.Init();
            }
            else
            {
                this.CopyFromInternal(style, false, true);
            }
            this.name = name;
            this.parent = parentName;
            if ((this.name != null) && (this.name != string.Empty))
            {
                this.nameSet = true;
            }
            if ((this.parent != null) && (this.parent != string.Empty))
            {
                this.parentSet = true;
            }
        }

        /// <summary>
        /// Occurs when the named style has changed. 
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue((string)null)]
        public virtual Brush Background
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
        public virtual string BackgroundThemeColor
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
        public virtual BorderLine BorderBottom
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
        public virtual BorderLine BorderLeft
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
        public virtual BorderLine BorderRight
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
        public virtual BorderLine BorderTop
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


        public virtual DataValidator DataValidator
        {
            get { return  this.dataValidator; }
            set
            {
                if (!object.Equals(this.dataValidator, value))
                {
                    this.dataValidator = value;
                    this.OnChanged(EventArgs.Empty);
                    this.RaisePropertyChanged("DataValidator");
                }
                this.dataValidatorSet = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(true)]
        public virtual bool Focusable
        {
            get { return  this.canFocus; }
            set
            {
                if (!object.Equals((bool)this.canFocus, (bool)value))
                {
                    this.canFocus = value;
                    this.OnChanged(EventArgs.Empty);
                    this.RaisePropertyChanged("Focusable");
                }
                this.canFocusSet = true;
            }
        }

        public FontFamily FontFamily
        {
            get { return  this.fontFamily; }
            set
            {
                if (this.fontFamily != value)
                {
                    this.fontFamily = value;
                    this.OnChanged(EventArgs.Empty);
                    this.RaisePropertyChanged("FontFamily");
                }
                this.fontTheme = null;
                this.fontThemeSet = false;
                this.fontFamilySet = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue((double)-1.0)]
        public double FontSize
        {
            get { return  this.fontSize; }
            set
            {
                if (this.fontSize != value)
                {
                    this.fontSize = value;
                    this.OnChanged(EventArgs.Empty);
                    this.RaisePropertyChanged("FontSize");
                }
                this.fontSizeSet = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue((string)null)]
        public virtual string FontTheme
        {
            get { return  this.fontTheme; }
            set
            {
                if (this.fontTheme != value)
                {
                    if (!string.Equals(value, FONTTHEME_BODY) && !string.Equals(value, FONTTHEME_HEADING))
                    {
                        throw new Exception(string.Concat((string[])new string[] { "'", FONTTHEME_BODY, "'", ResourceStrings.StyleInfoAnd, "'", FONTTHEME_HEADING, "'", ResourceStrings.StyleInfoexpected }));
                    }
                    this.fontTheme = value;
                    this.OnChanged(EventArgs.Empty);
                    this.RaisePropertyChanged("FontTheme");
                }
                this.fontFamily = null;
                this.fontFamilySet = false;
                this.fontThemeSet = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue((string)null)]
        public virtual Brush Foreground
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
        public virtual string ForegroundThemeColor
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
        [DefaultValue((string)null)]
        public virtual IFormatter Formatter
        {
            get { return  this.formatter; }
            set
            {
                if (!object.Equals(this.formatter, value))
                {
                    this.formatter = value;
                    this.OnChanged(EventArgs.Empty);
                    this.RaisePropertyChanged("Formatter");
                }
                this.formatterSet = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(3)]
        public CellHorizontalAlignment HorizontalAlignment
        {
            get { return  this.horizontalAlignment; }
            set
            {
                if (this.horizontalAlignment != value)
                {
                    this.horizontalAlignment = value;
                    this.OnChanged(EventArgs.Empty);
                    this.RaisePropertyChanged("HorizontalAlignment");
                }
                this.horizontalAlignmentSet = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(true)]
        public virtual bool IsEmpty
        {
            get { return  ((((((!this.nameSet && !this.parentSet) && (!this.canFocusSet && !this.lockedSet)) && ((!this.tabStopSet && !this.fontThemeSet) && (!this.fontFamilySet && !this.fontSizeSet))) && (((!this.fontStretchSet && !this.fontStyleSet) && (!this.fontWeightSet && !this.horizontalAlignmentSet)) && ((!this.verticalAlignmentSet && !this.textIndentSet) && (!this.wordWrapSet && !this.shrinkToFitSet)))) && ((((!this.leftBorderSet && !this.topBorderSet) && (!this.rightBorderSet && !this.bottomBorderSet)) && ((!this.backgroundSet && !this.foregroundSet) && (!this.backgroundThemeColorSet && !this.foregroundThemeColorSet))) && !this.formatterSet)) && !this.dataValidatorSet); }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(true)]
        public virtual bool Locked
        {
            get { return  this.locked; }
            set
            {
                if (!object.Equals((bool)this.locked, (bool)value))
                {
                    this.locked = value;
                    this.OnChanged(EventArgs.Empty);
                    this.RaisePropertyChanged("Locked");
                }
                this.lockedSet = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue("")]
        public virtual string Name
        {
            get
            {
                if (this.name == null)
                {
                    return string.Empty;
                }
                return this.name;
            }
            set
            {
                if (!object.Equals(this.name, value))
                {
                    this.name = value;
                    this.OnChanged(EventArgs.Empty);
                    this.RaisePropertyChanged("Name");
                }
                this.nameSet = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue((string)null)]
        public virtual string Parent
        {
            get
            {
                if (this.parent == null)
                {
                    return string.Empty;
                }
                return this.parent;
            }
            set
            {
                if (!object.Equals(this.parent, value))
                {
                    this.parent = value;
                    this.OnChanged(EventArgs.Empty);
                    this.RaisePropertyChanged("Parent");
                }
                this.parentSet = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(false)]
        public bool ShrinkToFit
        {
            get { return  this.shrinkToFit; }
            set
            {
                if (this.shrinkToFit != value)
                {
                    this.shrinkToFit = value;
                    this.OnChanged(EventArgs.Empty);
                    this.RaisePropertyChanged("ShrinkToFit");
                }
                this.shrinkToFitSet = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(false)]
        public virtual bool Strikethrough
        {
            get { return  this.strikethrough; }
            set
            {
                if (this.strikethrough != value)
                {
                    this.strikethrough = value;
                    this.OnChanged(EventArgs.Empty);
                    this.RaisePropertyChanged("Strikethrough");
                }
                this.isStrikethroughSet = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(false)]
        public virtual bool Underline
        {
            get { return  this.underline; }
            set
            {
                if (this.underline != value)
                {
                    this.underline = value;
                    this.OnChanged(EventArgs.Empty);
                    this.RaisePropertyChanged("Underline");
                }
                this.isUnderlineSet = true;
            }
        }
 

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(true)]
        public virtual bool TabStop
        {
            get { return  this.tabStop; }
            set
            {
                if (!object.Equals((bool)this.tabStop, (bool)value))
                {
                    this.tabStop = value;
                    this.OnChanged(EventArgs.Empty);
                    this.RaisePropertyChanged("TabStop");
                }
                this.tabStopSet = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(0)]
        public int TextIndent
        {
            get { return  this.textIndent; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("TextIndent", ResourceStrings.StyleInfoTextIndentMustBePositiveValue);
                }
                if (this.textIndent != value)
                {
                    this.textIndent = value;
                    this.OnChanged(EventArgs.Empty);
                    this.RaisePropertyChanged("TextIndent");
                }
                this.textIndentSet = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(1)]
        public CellVerticalAlignment VerticalAlignment
        {
            get { return  this.verticalAlignment; }
            set
            {
                if (this.verticalAlignment != value)
                {
                    this.verticalAlignment = value;
                    this.OnChanged(EventArgs.Empty);
                    this.RaisePropertyChanged("VerticalAlignment");
                }
                this.verticalAlignmentSet = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(false)]
        public bool WordWrap
        {
            get { return  this.wordWrap; }
            set
            {
                if (this.wordWrap != value)
                {
                    this.wordWrap = value;
                    this.OnChanged(EventArgs.Empty);
                    this.RaisePropertyChanged("WordWrap");
                }
                this.wordWrapSet = true;
            }
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

        public virtual object Clone()
        {
            return new StyleInfo(this);
        }

        public virtual void Compose(object source)
        {
            this.Compose(source, false);
        }

        /// <summary>
        /// Composes style settings using the current style settings and those of a specified object with an option to overwrite the current style settings. 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="merge"></param>
        public virtual void Compose(object source, bool merge)
        {
            StyleInfo info = source as StyleInfo;
            if (info != null)
            {
                if (((merge && !this.parentSet) && info.parentSet) || (!merge && info.parentSet))
                {
                    this.parent = info.parent;
                    this.parentSet = true;
                }
                if (((merge && !this.canFocusSet) && info.canFocusSet) || (!merge && info.canFocusSet))
                {
                    this.canFocus = info.Focusable;
                    this.canFocusSet = true;
                }
                if (((merge && !this.lockedSet) && info.lockedSet) || (!merge && info.lockedSet))
                {
                    this.locked = info.Locked;
                    this.lockedSet = true;
                }
                if (((merge && !this.tabStopSet) && info.tabStopSet) || (!merge && info.tabStopSet))
                {
                    this.tabStop = info.TabStop;
                    this.tabStopSet = true;
                }
                if (((merge && !this.fontThemeSet) && (!this.fontFamilySet && info.fontThemeSet)) || (!merge && info.fontThemeSet))
                {
                    this.fontTheme = info.fontTheme;
                    this.fontThemeSet = true;
                    this.fontFamily = null;
                    this.fontFamilySet = false;
                }
                if (((merge && !this.fontFamilySet) && (!this.fontThemeSet && info.fontFamilySet)) || (!merge && info.fontFamilySet))
                {
                    this.fontFamily = info.fontFamily;
                    this.fontFamilySet = true;
                    this.fontTheme = null;
                    this.fontThemeSet = false;
                }
                if (((merge && !this.fontSizeSet) && info.fontSizeSet) || (!merge && info.fontSizeSet))
                {
                    this.fontSize = info.fontSize;
                    this.fontSizeSet = true;
                }
                if (((merge && !this.fontStretchSet) && info.fontStretchSet) || (!merge && info.fontStretchSet))
                {
                    this.fontStretch = info.fontStretch;
                    this.fontStretchSet = true;
                }
                if (((merge && !this.fontStyleSet) && info.fontStyleSet) || (!merge && info.fontStyleSet))
                {
                    this.fontStyle = info.fontStyle;
                    this.fontStyleSet = true;
                }
                if (((merge && !this.fontWeightSet) && info.fontWeightSet) || (!merge && info.fontWeightSet))
                {
                    this.fontWeight = info.fontWeight;
                    this.fontWeightSet = true;
                }
                if (((merge && !this.horizontalAlignmentSet) && info.horizontalAlignmentSet) || (!merge && info.horizontalAlignmentSet))
                {
                    this.horizontalAlignment = info.horizontalAlignment;
                    this.horizontalAlignmentSet = true;
                }
                if (((merge && !this.verticalAlignmentSet) && info.verticalAlignmentSet) || (!merge && info.verticalAlignmentSet))
                {
                    this.verticalAlignment = info.verticalAlignment;
                    this.verticalAlignmentSet = true;
                }
                if (((merge && !this.textIndentSet) && info.textIndentSet) || (!merge && info.textIndentSet))
                {
                    this.textIndent = info.textIndent;
                    this.textIndentSet = true;
                }
                if (((merge && !this.wordWrapSet) && info.wordWrapSet) || (!merge && info.wordWrapSet))
                {
                    this.wordWrap = info.wordWrap;
                    this.wordWrapSet = true;
                }
                if (((merge && !this.shrinkToFitSet) && info.shrinkToFitSet) || (!merge && info.shrinkToFitSet))
                {
                    this.shrinkToFit = info.shrinkToFit;
                    this.shrinkToFitSet = true;
                }
                if (((merge && !this.leftBorderSet) && info.leftBorderSet) || (!merge && info.leftBorderSet))
                {
                    BorderLine borderLeft = info.BorderLeft;
                    this.leftBorder = (borderLeft == null) ? null : (borderLeft.Clone() as BorderLine);
                    this.leftBorderSet = true;
                }
                if (((merge && !this.topBorderSet) && info.topBorderSet) || (!merge && info.topBorderSet))
                {
                    BorderLine borderTop = info.BorderTop;
                    this.topBorder = (borderTop == null) ? null : (borderTop.Clone() as BorderLine);
                    this.topBorderSet = true;
                }
                if (((merge && !this.rightBorderSet) && info.rightBorderSet) || (!merge && info.rightBorderSet))
                {
                    BorderLine borderRight = info.BorderRight;
                    this.rightBorder = (borderRight == null) ? null : (borderRight.Clone() as BorderLine);
                    this.rightBorderSet = true;
                }
                if (((merge && !this.bottomBorderSet) && info.bottomBorderSet) || (!merge && info.bottomBorderSet))
                {
                    BorderLine borderBottom = info.BorderBottom;
                    this.bottomBorder = (borderBottom == null) ? null : (borderBottom.Clone() as BorderLine);
                    this.bottomBorderSet = true;
                }
                if (((merge && !this.backgroundSet) && (!this.backgroundThemeColorSet && info.backgroundSet)) || (!merge && info.backgroundSet))
                {
                    this.background = info.background;
                    this.backgroundSet = true;
                    this.backgroundThemeColor = null;
                    this.backgroundThemeColorSet = false;
                }
                if (((merge && !this.foregroundSet) && (!this.foregroundThemeColorSet && info.foregroundSet)) || (!merge && info.foregroundSet))
                {
                    this.foreground = info.foreground;
                    this.foregroundSet = true;
                    this.foregroundThemeColor = null;
                    this.foregroundThemeColorSet = false;
                }
                if (((merge && !this.backgroundThemeColorSet) && (!this.backgroundSet && info.backgroundThemeColorSet)) || (!merge && info.backgroundThemeColorSet))
                {
                    this.backgroundThemeColor = info.backgroundThemeColor;
                    this.backgroundThemeColorSet = true;
                    this.background = null;
                    this.backgroundSet = false;
                }
                if (((merge && !this.foregroundThemeColorSet) && (!this.foregroundSet && info.foregroundThemeColorSet)) || (!merge && info.foregroundThemeColorSet))
                {
                    this.foregroundThemeColor = info.foregroundThemeColor;
                    this.foregroundThemeColorSet = true;
                    this.foreground = null;
                    this.foregroundSet = false;
                }
                if (((merge && !this.formatterSet) && info.formatterSet) || (!merge && info.formatterSet))
                {
                    this.formatter = info.formatter;
                    this.formatterSet = true;
                }
                if (((merge && !this.dataValidatorSet) && info.dataValidatorSet) || (!merge && info.dataValidatorSet))
                {
                    this.dataValidator = info.DataValidator;
                    this.dataValidatorSet = true;
                }
                ///hdt 唐忠宝增加
                if (((merge && !this.isUnderlineSet) && info.isUnderlineSet) || (!merge && info.isUnderlineSet))
                {
                    this.underline = info.Underline;
                    this.isUnderlineSet = true;
                }
                if (((merge && !this.isStrikethroughSet) && info.isStrikethroughSet) || (!merge && info.isStrikethroughSet))
                {
                    this.strikethrough = info.Strikethrough;
                    this.isStrikethroughSet = true;
                }
            }
            if (!this.suspendEvents)
            {
                this.OnChanged(EventArgs.Empty);
            }
        }

        public virtual void CopyFrom(object o)
        {
            this.CopyFromInternal(o, true, true);
            this.OnChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Copies the specified style settings of the specified object to the current object. 
        /// </summary>
        /// <param name="o"></param>
        /// <param name="copyName"></param>
        /// <param name="clone"></param>
        internal void CopyFromInternal(object o, bool copyName, bool clone)
        {
            if ((o is StyleInfo) && !object.ReferenceEquals(o, this))
            {
                StyleInfo info = (StyleInfo)o;
                if (copyName)
                {
                    this.name = info.name;
                    this.nameSet = info.nameSet;
                }
                this.parent = info.parent;
                this.parentSet = info.parentSet;
                this.canFocus = info.canFocus;
                this.canFocusSet = info.canFocusSet;
                this.locked = info.locked;
                this.lockedSet = info.lockedSet;
                this.tabStop = info.tabStop;
                this.tabStopSet = info.tabStopSet;
                this.fontTheme = info.fontTheme;
                this.fontThemeSet = info.fontThemeSet;
                this.fontFamily = info.fontFamily;
                this.fontFamilySet = info.fontFamilySet;
                this.fontSize = info.fontSize;
                this.fontSizeSet = info.fontSizeSet;
                this.fontStretch = info.fontStretch;
                this.fontStretchSet = info.fontStretchSet;
                this.fontStyle = info.fontStyle;
                this.fontStyleSet = info.fontStyleSet;
                this.fontWeight = info.fontWeight;
                this.fontWeightSet = info.fontWeightSet;
                this.horizontalAlignment = info.horizontalAlignment;
                this.horizontalAlignmentSet = info.horizontalAlignmentSet;
                this.verticalAlignment = info.verticalAlignment;
                this.verticalAlignmentSet = info.verticalAlignmentSet;
                this.textIndent = info.textIndent;
                this.textIndentSet = info.textIndentSet;
                this.wordWrap = info.wordWrap;
                this.wordWrapSet = info.wordWrapSet;
                this.shrinkToFit = info.shrinkToFit;
                this.shrinkToFitSet = info.shrinkToFitSet;
                this.leftBorder = (clone && (info.leftBorder != null)) ? (info.leftBorder.Clone() as BorderLine) : info.leftBorder;
                this.leftBorderSet = info.leftBorderSet;
                this.topBorder = (clone && (info.topBorder != null)) ? (info.topBorder.Clone() as BorderLine) : info.topBorder;
                this.topBorderSet = info.topBorderSet;
                this.rightBorder = (clone && (info.rightBorder != null)) ? (info.rightBorder.Clone() as BorderLine) : info.rightBorder;
                this.rightBorderSet = info.rightBorderSet;
                this.bottomBorder = (clone && (info.bottomBorder != null)) ? (info.bottomBorder.Clone() as BorderLine) : info.bottomBorder;
                this.bottomBorderSet = info.bottomBorderSet;
                if (info.backgroundSet)
                {
                    ICloneable background = info.background as ICloneable;
                    if (background != null)
                    {
                        this.background = background.Clone() as Brush;
                    }
                    else
                    {
                        this.background = info.background;
                    }
                    this.backgroundSet = true;
                }
                else
                {
                    this.background = null;
                    this.backgroundSet = false;
                }
                if (info.foregroundSet)
                {
                    ICloneable foreground = info.foreground as ICloneable;
                    if (foreground != null)
                    {
                        this.foreground = foreground.Clone() as Brush;
                    }
                    else
                    {
                        this.foreground = info.foreground;
                    }
                    this.foregroundSet = true;
                }
                else
                {
                    this.foreground = null;
                    this.foregroundSet = false;
                }
                this.backgroundThemeColor = info.backgroundThemeColor;
                this.backgroundThemeColorSet = info.backgroundThemeColorSet;
                this.foregroundThemeColor = info.foregroundThemeColor;
                this.foregroundThemeColorSet = info.foregroundThemeColorSet;
                if (info.formatterSet)
                {
                    ICloneable formatter = info.formatter as ICloneable;
                    if (formatter != null)
                    {
                        this.formatter = formatter.Clone() as IFormatter;
                    }
                    else
                    {
                        this.formatter = info.formatter;
                    }
                    this.formatterSet = true;
                }
                else
                {
                    this.formatter = null;
                    this.formatterSet = false;
                }
                if (info.dataValidatorSet)
                {
                    ICloneable dataValidator = info.dataValidator;
                    if (dataValidator != null)
                    {
                        this.dataValidator = dataValidator.Clone() as DataValidator;
                    }
                    else
                    {
                        this.dataValidator = info.dataValidator;
                    }
                    this.dataValidatorSet = true;
                }
                else
                {
                    this.dataValidator = null;
                    this.dataValidatorSet = false;
                }
                // hdt 唐忠宝增加。
                this.underline = info.underline;
                this.isUnderlineSet = info.isUnderlineSet;
                this.strikethrough = info.strikethrough;
                this.isStrikethroughSet = info.isStrikethroughSet;
            }
        }

        public override bool Equals(object o)
        {
            if (!(o is StyleInfo))
            {
                return false;
            }
            StyleInfo info = (StyleInfo)o;
            if ((((((this.nameSet != info.nameSet) || (this.parentSet != info.parentSet)) || ((this.canFocusSet != info.canFocusSet) || (this.lockedSet != info.lockedSet))) || (((this.tabStopSet != info.tabStopSet) || (this.fontThemeSet != info.fontThemeSet)) || ((this.fontFamilySet != info.fontFamilySet) || (this.fontSizeSet != info.fontSizeSet)))) || ((((this.fontStretchSet != info.fontStretchSet) || (this.fontStyleSet != info.fontStyleSet)) || ((this.fontWeightSet != info.fontWeightSet) || (this.horizontalAlignmentSet != info.horizontalAlignmentSet))) || (((this.verticalAlignmentSet != info.verticalAlignmentSet) || (this.textIndentSet != info.textIndentSet)) || ((this.wordWrapSet != info.wordWrapSet) || (this.shrinkToFitSet != info.shrinkToFitSet))))) || ((((this.leftBorderSet != info.leftBorderSet) || (this.topBorderSet != info.topBorderSet)) || ((this.rightBorderSet != info.rightBorderSet) || (this.bottomBorderSet != info.bottomBorderSet))) || ((((this.backgroundSet != info.backgroundSet) || (this.foregroundSet != info.foregroundSet)) || ((this.backgroundThemeColorSet != info.backgroundThemeColorSet) || (this.foregroundThemeColorSet != info.foregroundThemeColorSet))) || ((this.formatterSet != info.formatterSet) || (this.dataValidatorSet != info.dataValidatorSet)))))
            {
                return false;
            }
            return ((((((!this.nameSet || object.Equals(this.name, info.name)) && (!this.parentSet || object.Equals(this.parent, info.parent))) && ((!this.canFocusSet || object.Equals((bool)this.canFocus, (bool)info.canFocus)) && (!this.lockedSet || object.Equals((bool)this.locked, (bool)info.locked)))) && (((!this.tabStopSet || object.Equals((bool)this.tabStop, (bool)info.tabStop)) && (!this.fontThemeSet || object.Equals(this.fontTheme, info.fontTheme))) && ((!this.fontFamilySet || object.Equals(this.fontFamily, info.fontFamily)) && (!this.fontSizeSet || object.Equals((double)this.fontSize, (double)info.fontSize))))) && ((((!this.fontStretchSet || object.Equals(this.fontStretch, info.fontStretch)) && (!this.fontStyleSet || object.Equals(this.fontStyle, info.fontStyle))) && ((!this.fontWeightSet || object.Equals(this.fontWeight, info.fontWeight)) && (!this.horizontalAlignmentSet || object.Equals(this.horizontalAlignment, info.horizontalAlignment)))) && (((!this.verticalAlignmentSet || object.Equals(this.verticalAlignment, info.verticalAlignment)) && (!this.textIndentSet || object.Equals((int)this.textIndent, (int)info.textIndent))) && ((!this.wordWrapSet || object.Equals((bool)this.wordWrap, (bool)info.wordWrap)) && (!this.shrinkToFitSet || object.Equals((bool)this.shrinkToFit, (bool)info.shrinkToFit)))))) && ((((!this.leftBorderSet || object.Equals(this.leftBorder, info.leftBorder)) && (!this.topBorderSet || object.Equals(this.topBorder, info.topBorder))) && ((!this.rightBorderSet || object.Equals(this.rightBorder, info.rightBorder)) && (!this.bottomBorderSet || object.Equals(this.bottomBorder, info.bottomBorder)))) && ((((!this.backgroundSet || object.Equals(this.background, info.background)) && (!this.foregroundSet || object.Equals(this.foreground, info.foreground))) && ((!this.backgroundThemeColorSet || object.Equals(this.backgroundThemeColor, info.backgroundThemeColor)) && (!this.foregroundThemeColorSet || object.Equals(this.foregroundThemeColor, info.foregroundThemeColor)))) && ((!this.formatterSet || object.Equals(this.formatter, info.formatter)) && (!this.dataValidatorSet || object.Equals(this.dataValidator, info.dataValidator))))));
        }


        internal static string GetFontWeightString(FontWeight fontWeight)
        {
            if (infos == null)
            {
                infos = typeof(FontWeights).GetRuntimeProperties().ToArray();
            }
            for (int i = 0; i < infos.Length; i++)
            {
                object obj2 = infos[i].GetValue(null);
                if ((obj2 != null) && (((FontWeight)obj2).Weight == fontWeight.Weight))
                {
                    return infos[i].Name;
                }
            }
            return "Normal";
        }

        /// <summary>
        /// hdt 唐忠宝增加 Underline 和 Strikethrough
        /// </summary>
        public void Init()
        {
            this.name = string.Empty;
            this.parent = null;
            this.canFocus = true;
            this.locked = true;
            this.tabStop = true;
            this.fontTheme = null;
            this.fontFamily = null;
            this.fontSize = -1.0;
            this.fontStretch = (FontStretch)5;
            this.fontStyle = 0;
            this.fontWeight = FontWeights.Normal;
            this.horizontalAlignment = CellHorizontalAlignment.General;
            this.verticalAlignment = CellVerticalAlignment.Center;
            this.textIndent = 0;
            this.wordWrap = false;
            this.shrinkToFit = false;
            this.leftBorder = null;
            this.topBorder = null;
            this.rightBorder = null;
            this.bottomBorder = null;
            this.background = null;
            this.foreground = null;
            this.backgroundThemeColor = null;
            this.foregroundThemeColor = null;
            this.formatter = null;
            this.dataValidator = null;
            this.underline = false;
            this.strikethrough = false;
            this.nameSet = false;
            this.parentSet = false;
            this.canFocusSet = false;
            this.lockedSet = false;
            this.tabStopSet = false;
            this.fontThemeSet = false;
            this.fontFamilySet = false;
            this.fontSizeSet = false;
            this.fontStretchSet = false;
            this.fontStyleSet = false;
            this.fontWeightSet = false;
            this.horizontalAlignmentSet = false;
            this.verticalAlignmentSet = false;
            this.textIndentSet = false;
            this.wordWrapSet = false;
            this.shrinkToFitSet = false;
            this.leftBorderSet = false;
            this.topBorderSet = false;
            this.rightBorderSet = false;
            this.bottomBorderSet = false;
            this.backgroundSet = false;
            this.foregroundSet = false;
            this.backgroundThemeColorSet = false;
            this.foregroundThemeColorSet = false;
            this.formatterSet = false;
            this.dataValidatorSet = false;
            this.isUnderlineSet = false;
            this.isStrikethroughSet = false;
        }

        public virtual bool IsBackgroundSet()
        {
            return this.backgroundSet;
        }

        public virtual bool IsBackgroundThemeColorSet()
        {
            return this.backgroundThemeColorSet;
        }

        public virtual bool IsBorderBottomSet()
        {
            return this.bottomBorderSet;
        }

        public virtual bool IsBorderLeftSet()
        {
            return this.leftBorderSet;
        }

        public virtual bool IsBorderRightSet()
        {
            return this.rightBorderSet;
        }

        public virtual bool IsBorderTopSet()
        {
            return this.topBorderSet;
        }

        public virtual bool IsDataValidatorSet()
        {
            return this.dataValidatorSet;
        }

        public virtual bool IsFocusableSet()
        {
            return this.canFocusSet;
        }

        public virtual bool IsFontFamilySet()
        {
            return this.fontFamilySet;
        }

        public virtual bool IsFontSizeSet()
        {
            return this.fontSizeSet;
        }

        public virtual bool IsFontStretchSet()
        {
            return this.fontStretchSet;
        }

        public virtual bool IsFontStyleSet()
        {
            return this.fontStyleSet;
        }

        public virtual bool IsFontThemeSet()
        {
            return this.fontThemeSet;
        }

        public virtual bool IsFontWeightSet()
        {
            return this.fontWeightSet;
        }

        public virtual bool IsForegroundSet()
        {
            return this.foregroundSet;
        }

        public virtual bool IsForegroundThemeColorSet()
        {
            return this.foregroundThemeColorSet;
        }

        public virtual bool IsFormatterSet()
        {
            return this.formatterSet;
        }

        public virtual bool IsHorizontalAlignmentSet()
        {
            return this.horizontalAlignmentSet;
        }

        public virtual bool IsLockedSet()
        {
            return this.lockedSet;
        }

        public virtual bool IsNameSet()
        {
            return this.nameSet;
        }

        public virtual bool IsParentSet()
        {
            return this.parentSet;
        }

        public virtual bool IsShrinkToFitSet()
        {
            return this.shrinkToFitSet;
        }

        public virtual bool IsStrikethroughSet()
        {
            return this.isStrikethroughSet;
        } 

        public virtual bool IsTabStopSet()
        {
            return this.tabStopSet;
        }

        public virtual bool IsTextIndentSet()
        {
            return this.textIndentSet;
        }

        public virtual bool IsUnderlineSet()
        {
            return this.isUnderlineSet;
        }

        public virtual bool IsVerticalAlignmentSet()
        {
            return this.verticalAlignmentSet;
        }

        public virtual bool IsWordWrapSet()
        {
            return this.wordWrapSet;
        }        

        public virtual void Merge(StyleInfo source)
        {
            this.Compose(source, true);
        }

        public virtual void Merge(StyleInfo source, bool force)
        {
            this.Compose(source, !force);
        }

        protected virtual void OnChanged(EventArgs e)
        {
            if (!this.suspendEvents)
            {
                StyleInfoCollection.IncreaseStyleInfoVersion();
                if (this.Changed != null)
                {
                    this.Changed(this, e);
                }
            }
        }

        void RaisePropertyChanged(string propertyName)
        {
            if (!this.suspendEvents && (this.PropertyChanged != null))
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public virtual void Reset()
        {
            this.Init();
            if (!this.suspendEvents)
            {
                this.OnChanged(EventArgs.Empty);
                this.RaisePropertyChanged("[StyleInfo]");
            }
        }

        public virtual void ResetBackground()
        {
            this.background = null;
            this.backgroundSet = false;
            this.OnChanged(EventArgs.Empty);
            this.RaisePropertyChanged("Background");
        }

        public virtual void ResetBackgroundThemeColor()
        {
            this.backgroundThemeColor = null;
            this.backgroundThemeColorSet = false;
            this.OnChanged(EventArgs.Empty);
            this.RaisePropertyChanged("BackgroundThemeColor");
        }

        public virtual void ResetBorderBottom()
        {
            this.bottomBorder = null;
            this.bottomBorderSet = false;
            this.OnChanged(EventArgs.Empty);
            this.RaisePropertyChanged("BorderBottom");
        }

        public virtual void ResetBorderLeft()
        {
            this.leftBorder = null;
            this.leftBorderSet = false;
            this.OnChanged(EventArgs.Empty);
            this.RaisePropertyChanged("BorderLeft");
        }

        public virtual void ResetBorderRight()
        {
            this.rightBorder = null;
            this.rightBorderSet = false;
            this.OnChanged(EventArgs.Empty);
            this.RaisePropertyChanged("BorderRight");
        }

        public virtual void ResetBorderTop()
        {
            this.topBorder = null;
            this.topBorderSet = false;
            this.OnChanged(EventArgs.Empty);
            this.RaisePropertyChanged("BorderTop");
        }

        public virtual void ResetDataValidator()
        {
            this.dataValidator = null;
            this.dataValidatorSet = false;
            this.OnChanged(EventArgs.Empty);
            this.RaisePropertyChanged("DataValidator");
        }

        public virtual void ResetFocusable()
        {
            this.canFocus = true;
            this.canFocusSet = false;
            this.OnChanged(EventArgs.Empty);
            this.RaisePropertyChanged("Focusable");
        }

        public virtual void ResetFontFamily()
        {
            this.fontFamily = null;
            this.fontFamilySet = false;
            this.OnChanged(EventArgs.Empty);
            this.RaisePropertyChanged("FontFamily");
        }

        public virtual void ResetFontSize()
        {
            this.fontSize = -1.0;
            this.fontSizeSet = false;
            this.OnChanged(EventArgs.Empty);
            this.RaisePropertyChanged("FontSize");
        }

        public virtual void ResetFontStretch()
        {
            this.fontStretch = FontStretch.Normal;
            this.fontStretchSet = false;
            this.OnChanged(EventArgs.Empty);
            this.RaisePropertyChanged("FontStretch");
        }

        public virtual void ResetFontStyle()
        {
            this.fontStyle = Windows.UI.Text.FontStyle.Normal;
            this.fontStyleSet = false;
            this.OnChanged(EventArgs.Empty);
            this.RaisePropertyChanged("FontStyle");
        }

        public virtual void ResetFontTheme()
        {
            this.fontTheme = null;
            this.fontThemeSet = false;
            this.OnChanged(EventArgs.Empty);
            this.RaisePropertyChanged("FontTheme");
        }

        public virtual void ResetFontWeight()
        {
            this.fontWeight = FontWeights.Normal;
            this.fontWeightSet = false;
            this.OnChanged(EventArgs.Empty);
            this.RaisePropertyChanged("FontWeight");
        }

        public virtual void ResetForeground()
        {
            this.foreground = null;
            this.foregroundSet = false;
            this.OnChanged(EventArgs.Empty);
            this.RaisePropertyChanged("Foreground");
        }

        public virtual void ResetForegroundThemeColor()
        {
            this.foregroundThemeColor = null;
            this.foregroundThemeColorSet = false;
            this.OnChanged(EventArgs.Empty);
            this.RaisePropertyChanged("ForegroundThemeColor");
        }

        public virtual void ResetFormatter()
        {
            this.formatter = null;
            this.formatterSet = false;
            this.OnChanged(EventArgs.Empty);
            this.RaisePropertyChanged("Formatter");
        }

        public virtual void ResetHorizontalAlignment()
        {
            this.horizontalAlignment = CellHorizontalAlignment.General;
            this.horizontalAlignmentSet = false;
            this.OnChanged(EventArgs.Empty);
            this.RaisePropertyChanged("HorizontalAlignment");
        }

        public virtual void ResetLocked()
        {
            this.locked = true;
            this.lockedSet = false;
            this.OnChanged(EventArgs.Empty);
            this.RaisePropertyChanged("Locked");
        }

        public virtual void ResetName()
        {
            this.name = string.Empty;
            this.nameSet = false;
            this.OnChanged(EventArgs.Empty);
            this.RaisePropertyChanged("Name");
        }

        public virtual void ResetParent()
        {
            this.parent = null;
            this.parentSet = false;
            this.OnChanged(EventArgs.Empty);
            this.RaisePropertyChanged("Parent");
        }

        public virtual void ResetShrinkToFit()
        {
            this.shrinkToFit = false;
            this.shrinkToFitSet = false;
            this.OnChanged(EventArgs.Empty);
            this.RaisePropertyChanged("ShrinkToFit");
        }

        public virtual void ResetStrikethrough()
        {
            this.strikethrough = false;
            this.isStrikethroughSet = false;
            this.OnChanged(EventArgs.Empty);
            this.RaisePropertyChanged("Strikethrough");
        }
        
        public virtual void ResetTabStop()
        {
            this.tabStop = true;
            this.tabStopSet = false;
            this.OnChanged(EventArgs.Empty);
            this.RaisePropertyChanged("TabStop");
        }

        public virtual void ResetTextIndent()
        {
            this.textIndent = 0;
            this.textIndentSet = false;
            this.OnChanged(EventArgs.Empty);
            this.RaisePropertyChanged("TextIndent");
        }

        public virtual void ResetUnderline()
        {
            this.underline = false;
            this.isUnderlineSet = false;
            this.OnChanged(EventArgs.Empty);
            this.RaisePropertyChanged("Underline");
        }

        public virtual void ResetVerticalAlignment()
        {
            this.verticalAlignment = CellVerticalAlignment.Center;
            this.verticalAlignmentSet = false;
            this.OnChanged(EventArgs.Empty);
            this.RaisePropertyChanged("VerticalAlignment");
        }

        public virtual void ResetWordWrap()
        {
            this.wordWrap = false;
            this.wordWrapSet = false;
            this.OnChanged(EventArgs.Empty);
            this.RaisePropertyChanged("WordWrap");
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            Serializer.InitReader(reader);
            try
            {
                this.suspendEvents = true;
                this.Reset();
                goto Label_090A;
            }
            finally
            {
                this.suspendEvents = false;
            }
        Label_0043:
            if (reader.NodeType == ((XmlNodeType)((int)XmlNodeType.Element)))
            {
                switch (reader.Name)
                {
                    case "Name":
                        this.name = (string)((string)Serializer.DeserializeObj(typeof(string), reader));
                        this.nameSet = true;
                        break;

                    case "Parent":
                        this.parent = (string)((string)Serializer.DeserializeObj(typeof(string), reader));
                        this.parentSet = true;
                        break;

                    case "Focusable":
                        this.canFocus = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                        this.canFocusSet = true;
                        break;

                    case "Locked":
                        this.locked = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                        this.lockedSet = true;
                        break;

                    case "TabStop":
                        this.tabStop = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                        this.tabStopSet = true;
                        break;

                    case "FontTheme":
                        this.fontTheme = (string)(Serializer.DeserializeObj(typeof(string), reader) as string);
                        this.fontThemeSet = true;
                        break;

                    case "FontFamily":
                        if (!(Serializer.ReadAttribute("value", reader) == Serializer.Format(null)))
                        {
                            string family = (string)(Serializer.DeserializeObj(typeof(string), reader) as string);
                            this.fontFamily = new FontFamily(family);
                        }
                        else
                        {
                            this.fontFamily = null;
                        }
                        this.fontFamilySet = true;
                        break;

                    case "FontSize":
                        this.fontSize = (float)Serializer.DeserializeObj(typeof(float), reader);
                        this.fontSizeSet = true;
                        break;

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
                        {
                            string result = (string)(Serializer.DeserializeObj(typeof(string), reader) as string);
                            FontWeight? weight = Serializer.FindStaticDefinationStruct<FontWeight>(typeof(FontWeights), result);
                            if (weight.HasValue)
                            {
                                this.fontWeight = weight.Value;
                                this.fontWeightSet = true;
                            }
                            break;
                        }
                    case "HorizontalAlignment":
                        this.horizontalAlignment = (CellHorizontalAlignment)Serializer.DeserializeObj(typeof(CellHorizontalAlignment), reader);
                        this.horizontalAlignmentSet = true;
                        break;

                    case "VerticalAlignment":
                        this.verticalAlignment = (CellVerticalAlignment)Serializer.DeserializeObj(typeof(CellVerticalAlignment), reader);
                        this.verticalAlignmentSet = true;
                        break;

                    case "TextIndent":
                        this.textIndent = (int)((int)Serializer.DeserializeObj(typeof(int), reader));
                        this.textIndentSet = true;
                        break;

                    case "WordWrap":
                        this.wordWrap = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                        this.wordWrapSet = true;
                        break;

                    case "ShrinkToFit":
                        this.shrinkToFit = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                        this.shrinkToFitSet = true;
                        break;

                    case "LeftBorder":
                        if (!(Serializer.ReadAttribute("value", reader) == Serializer.Format(null)))
                        {
                            this.leftBorder = Serializer.DeserializeObj(typeof(BorderLine), reader) as BorderLine;
                        }
                        else
                        {
                            this.leftBorder = null;
                        }
                        this.leftBorderSet = true;
                        break;

                    case "TopBorder":
                        if (!(Serializer.ReadAttribute("value", reader) == Serializer.Format(null)))
                        {
                            this.topBorder = Serializer.DeserializeObj(typeof(BorderLine), reader) as BorderLine;
                        }
                        else
                        {
                            this.topBorder = null;
                        }
                        this.topBorderSet = true;
                        break;

                    case "RightBorder":
                        if (!(Serializer.ReadAttribute("value", reader) == Serializer.Format(null)))
                        {
                            this.rightBorder = Serializer.DeserializeObj(typeof(BorderLine), reader) as BorderLine;
                        }
                        else
                        {
                            this.rightBorder = null;
                        }
                        this.rightBorderSet = true;
                        break;

                    case "BottomBorder":
                        if (!(Serializer.ReadAttribute("value", reader) == Serializer.Format(null)))
                        {
                            this.bottomBorder = Serializer.DeserializeObj(typeof(BorderLine), reader) as BorderLine;
                        }
                        else
                        {
                            this.bottomBorder = null;
                        }
                        this.bottomBorderSet = true;
                        break;

                    case "Background":
                        background = Serializer.DeserializeObj(typeof(Brush), reader) as Brush;
                        this.backgroundSet = true;
                        break;

                    case "Foreground":
                        foreground = Serializer.DeserializeObj(typeof(Brush), reader) as Brush;
                        this.foregroundSet = true;
                        break;

                    case "BackgroundTheme":
                        this.backgroundThemeColor = (string)(Serializer.DeserializeObj(typeof(string), reader) as string);
                        this.backgroundThemeColorSet = true;
                        break;

                    case "ForegroundTheme":
                        this.foregroundThemeColor = (string)(Serializer.DeserializeObj(typeof(string), reader) as string);
                        this.foregroundThemeColorSet = true;
                        break;

                    case "Formatter":
                        if (!(Serializer.ReadAttribute("value", reader) == Serializer.Format(null)))
                        {
                            string format = (string)(Serializer.DeserializeObj(typeof(string), reader) as string);
                            if (format != null)
                            {
                                string str8 = "[Auto]";
                                bool flag = format.StartsWith(str8);
                                if (flag)
                                {
                                    format = format.Remove(0, str8.Length);
                                }
                                this.formatter = new GeneralFormatter(format);
                                if (flag)
                                {
                                    this.formatter = new AutoFormatter(this.formatter as GeneralFormatter);
                                }
                            }
                        }
                        else
                        {
                            this.formatter = null;
                        }
                        this.formatterSet = true;
                        break;

                    case "DataValidator":
                        if (!(Serializer.ReadAttribute("value", reader) == Serializer.Format(null)))
                        {
                            this.dataValidator = Serializer.DeserializeObj(null, reader) as DataValidator;
                        }
                        else
                        {
                            this.dataValidator = null;
                        }
                        this.dataValidatorSet = true;
                        break;
                    case "Underline":
                        this.underline = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                        this.isUnderlineSet = true;
                        break;

                    case "Strikethrough":
                        this.strikethrough = (bool)((bool)Serializer.DeserializeObj(typeof(bool), reader));
                        this.isStrikethroughSet = true;
                        break;
                }
            }
        Label_090A:
            if (reader.Read())
            {
                goto Label_0043;
            }
            this.OnChanged(EventArgs.Empty);
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            Serializer.InitWriter(writer);
            if (this.nameSet)
            {
                Serializer.SerializeObj(this.name, "Name", writer);
            }
            if (this.parentSet)
            {
                Serializer.SerializeObj(this.parent, "Parent", writer);
            }
            if (this.canFocusSet)
            {
                Serializer.SerializeObj((bool)this.canFocus, "Focusable", writer);
            }
            if (this.lockedSet)
            {
                Serializer.SerializeObj((bool)this.locked, "Locked", writer);
            }
            if (this.tabStopSet)
            {
                Serializer.SerializeObj((bool)this.tabStop, "TabStop", writer);
            }
            if (this.fontThemeSet)
            {
                Serializer.SerializeObj(this.fontTheme, "FontTheme", writer);
            }
            if (this.fontFamilySet)
            {
                if (this.fontFamily == null)
                {
                    writer.WriteStartElement("FontFamily");
                    Serializer.WriteAttr("value", Serializer.Format(null), writer);
                    writer.WriteEndElement();
                }
                else
                {
                    Serializer.SerializeObj(this.fontFamily.Source, "FontFamily", writer);
                }
            }
            if (this.fontSizeSet)
            {
                Serializer.SerializeObj((double)this.fontSize, "FontSize", writer);
            }
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
                Serializer.SerializeObj(GetFontWeightString(this.fontWeight), "FontWeight", writer);
            }
            if (this.horizontalAlignmentSet)
            {
                Serializer.SerializeObj(this.horizontalAlignment, "HorizontalAlignment", writer);
            }
            if (this.verticalAlignmentSet)
            {
                Serializer.SerializeObj(this.verticalAlignment, "VerticalAlignment", writer);
            }
            if (this.textIndentSet)
            {
                Serializer.SerializeObj((int)this.textIndent, "TextIndent", writer);
            }
            if (this.wordWrapSet)
            {
                Serializer.SerializeObj((bool)this.wordWrap, "WordWrap", writer);
            }
            if (this.shrinkToFitSet)
            {
                Serializer.SerializeObj((bool)this.shrinkToFit, "ShrinkToFit", writer);
            }
            if (this.leftBorderSet)
            {
                if (this.leftBorder == null)
                {
                    writer.WriteStartElement("LeftBorder");
                    Serializer.WriteAttr("value", Serializer.Format(null), writer);
                    writer.WriteEndElement();
                }
                else
                {
                    Serializer.SerializeObj(this.leftBorder, "LeftBorder", writer);
                }
            }
            if (this.topBorderSet)
            {
                if (this.topBorder == null)
                {
                    writer.WriteStartElement("TopBorder");
                    Serializer.WriteAttr("value", Serializer.Format(null), writer);
                    writer.WriteEndElement();
                }
                else
                {
                    Serializer.SerializeObj(this.topBorder, "TopBorder", writer);
                }
            }
            if (this.rightBorderSet)
            {
                if (this.rightBorder == null)
                {
                    writer.WriteStartElement("RightBorder");
                    Serializer.WriteAttr("value", Serializer.Format(null), writer);
                    writer.WriteEndElement();
                }
                else
                {
                    Serializer.SerializeObj(this.rightBorder, "RightBorder", writer);
                }
            }
            if (this.bottomBorderSet)
            {
                if (this.bottomBorder == null)
                {
                    writer.WriteStartElement("BottomBorder");
                    Serializer.WriteAttr("value", Serializer.Format(null), writer);
                    writer.WriteEndElement();
                }
                else
                {
                    Serializer.SerializeObj(this.bottomBorder, "BottomBorder", writer);
                }
            }
            if (this.backgroundSet)
            {
                Serializer.SerializeObj(this.background, "Background", writer);
            }
            if (this.foregroundSet)
            {
                Serializer.SerializeObj(this.foreground, "Foreground", writer);
            }
            if (this.backgroundThemeColorSet)
            {
                Serializer.SerializeObj(this.backgroundThemeColor, "BackgroundTheme", writer);
            }
            if (this.foregroundThemeColorSet)
            {
                Serializer.SerializeObj(this.foregroundThemeColor, "ForegroundTheme", writer);
            }
            if (this.formatterSet)
            {
                if (this.formatter == null)
                {
                    writer.WriteStartElement("Formatter");
                    Serializer.WriteAttr("value", Serializer.Format(null), writer);
                    writer.WriteEndElement();
                }
                else
                {
                    string formatString = this.formatter.FormatString;
                    if (this.formatter is AutoFormatter)
                    {
                        formatString = formatString.Insert(0, "[Auto]");
                    }
                    Serializer.SerializeObj(formatString, "Formatter", writer);
                }
            }
            if (this.dataValidatorSet)
            {
                if (this.dataValidator == null)
                {
                    writer.WriteStartElement("DataValidator");
                    Serializer.WriteAttr("value", Serializer.Format(null), writer);
                    writer.WriteEndElement();
                }
                else
                {
                    Serializer.WriteStartObj("DataValidator", writer);
                    Serializer.WriteTypeAttr(this.dataValidator, writer);
                    Serializer.SerializeObj(this.dataValidator, null, writer);
                    Serializer.WriteEndObj(writer);
                }
            }
            if (this.isUnderlineSet)
            {
                Serializer.SerializeObj((bool)this.underline, "Underline", writer);
            }
            if (this.isStrikethroughSet)
            {
                Serializer.SerializeObj((bool)this.strikethrough, "Strikethrough", writer);
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}