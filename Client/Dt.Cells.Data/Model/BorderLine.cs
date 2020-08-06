#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Windows.UI;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents settings for the border line.
    /// </summary>
    public sealed class BorderLine : IThemeContextSupport, ICloneable, IXmlSerializable
    {
        bool _builtIn;
        Windows.UI.Color _color;
        IThemeSupport _context;
        static BorderLine _emptyBorder;
        static BorderLine _noBorder;
        BorderLineStyle _style;
        BorderLineData _styleData;
        string _themeColor;

        /// <summary>
        /// Creates an empty border line.
        /// </summary>
        public BorderLine()
            : this(Colors.Black, BorderLineStyle.None)
        {
        }

        /// <summary>
        /// Creates a border line with a specified theme color. 
        /// </summary>
        /// <param name="themeColor">The theme color name.</param>
        public BorderLine(string themeColor)
            : this(themeColor, BorderLineStyle.Thin)
        {
        }

        /// <summary>
        /// Creates a border line with a specified color.
        /// </summary>
        /// <param name="color">The border color.</param>
        public BorderLine(Windows.UI.Color color)
            : this(color, BorderLineStyle.Thin)
        {
        }

        /// <summary>
        /// Creates a border line with a specified theme color and style type.
        /// </summary>
        /// <param name="themeColor">The theme color name.</param>
        /// <param name="styleType">The border style type.</param>
        public BorderLine(string themeColor, BorderLineStyle styleType)
        {
            this._context = null;
            this._themeColor = themeColor;
            this._style = styleType;
            this._builtIn = false;
        }

        /// <summary>
        /// Caution, only use for built-in borders creating.
        /// </summary>
        internal BorderLine(Windows.UI.Color color, BorderLineData style)
        {
            this._context = null;
            this._style = BorderLineStyle.None;
            this._color = color;
            this._styleData = style;
            this._builtIn = true;
        }

        /// <summary>
        /// Creates a border line with a specified color and style.
        /// </summary>
        /// <param name="color">The border color.</param>
        /// <param name="styleType">The border style type.</param>
        public BorderLine(Windows.UI.Color color, BorderLineStyle styleType)
        {
            this._context = null;
            this._themeColor = null;
            this._color = color;
            this._style = styleType;
            this._builtIn = false;
        }

        /// <summary>
        /// Clones the same border line as the current border line.
        /// </summary>
        /// <returns>The cloned border line.</returns>
        public object Clone()
        {
            return new BorderLine { _color = this._color, _themeColor = this._themeColor, _style = this._style };
        }

        /// <summary>
        /// Compares whether the border line is equal to the specified border line.
        /// </summary>
        /// <param name="obj">The compared border line.</param>
        /// <returns>
        /// <c>true</c> if the border line is less than the second line; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            BorderLine line = obj as BorderLine;
            if (line == null)
            {
                return false;
            }
            if (this.Style != line.Style)
            {
                return false;
            }
            if (this._color != line._color)
            {
                return false;
            }
            if (this._themeColor != line._themeColor)
            {
                return false;
            }
            return true;
        }

        static int GetBrightness(Windows.UI.Color color)
        {
            return ((((color.R * 0x12b) + (color.G * 0x24b)) + (color.B * 0x72)) / 0x3e8);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Gets the theme context.
        /// </summary>
        /// <returns>
        /// Returns the theme context
        /// </returns>
        IThemeSupport IThemeContextSupport.GetContext()
        {
            return this._context;
        }

        /// <summary>
        /// Sets the theme context.
        /// </summary>
        /// <param name="context">The theme context object.</param>
        void IThemeContextSupport.SetContext(IThemeSupport context)
        {
            this._context = context;
        }

        void InitStyle()
        {
            if (this._styleData == null)
            {
                switch (this._style)
                {
                    case BorderLineStyle.None:
                        this._styleData = BorderLineStyleDatas.Empty;
                        return;

                    case BorderLineStyle.Thin:
                        this._styleData = BorderLineStyleDatas.Thin;
                        return;

                    case BorderLineStyle.Medium:
                        this._styleData = BorderLineStyleDatas.Medium;
                        return;

                    case BorderLineStyle.Dashed:
                        this._styleData = BorderLineStyleDatas.Dashed;
                        return;

                    case BorderLineStyle.Dotted:
                        this._styleData = BorderLineStyleDatas.Dotted;
                        return;

                    case BorderLineStyle.Thick:
                        this._styleData = BorderLineStyleDatas.Thick;
                        return;

                    case BorderLineStyle.Double:
                        this._styleData = BorderLineStyleDatas.Double;
                        return;

                    case BorderLineStyle.Hair:
                        this._styleData = BorderLineStyleDatas.Hair;
                        return;

                    case BorderLineStyle.MediumDashed:
                        this._styleData = BorderLineStyleDatas.MediumDashed;
                        return;

                    case BorderLineStyle.DashDot:
                        this._styleData = BorderLineStyleDatas.DashDot;
                        return;

                    case BorderLineStyle.MediumDashDot:
                        this._styleData = BorderLineStyleDatas.MediumDashDot;
                        return;

                    case BorderLineStyle.DashDotDot:
                        this._styleData = BorderLineStyleDatas.DashDotDot;
                        return;

                    case BorderLineStyle.MediumDashDotDot:
                        this._styleData = BorderLineStyleDatas.MediumDashDotDot;
                        return;

                    case BorderLineStyle.SlantedDashDot:
                        this._styleData = BorderLineStyleDatas.SlantedDashDot;
                        return;
                }
                this._styleData = BorderLineStyleDatas.Empty;
            }
        }

        /// <summary>
        /// Gets the maximum line of two lines.
        /// </summary>
        /// <param name="line1">The first line.</param>
        /// <param name="line2">The second line.</param>
        /// <returns>The maximum of two lines.</returns>
        public static BorderLine Max(BorderLine line1, BorderLine line2)
        {
            if (line1 <= line2)
            {
                return line2;
            }
            return line1;
        }

        /// <summary>
        /// Compares whether the first border line has a greater weight value or brightness than the second border line.
        /// </summary>
        /// <param name="line1">The first border line.</param>
        /// <param name="line2">The second border line.</param>
        /// <returns>
        /// <c>true</c> if the first border line is greater than the second border line; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator >(BorderLine line1, BorderLine line2)
        {
            if ((line1 == null) && (line2 == null))
            {
                return true;
            }
            if (line1 == null)
            {
                return false;
            }
            if (line2 == null)
            {
                return true;
            }
            if (line1.StyleData.Weight == line2.StyleData.Weight)
            {
                return (GetBrightness(line1.Color) < GetBrightness(line2.Color));
            }
            return (line1.StyleData.Weight > line2.StyleData.Weight);
        }

        /// <summary>
        /// Compares whether the first border line is less than the second border line.
        /// </summary>
        /// <param name="line1">The first border line.</param>
        /// <param name="line2">The second border line.</param>
        /// <returns>
        /// <c>true</c> if the first line is less than the second line; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator <(BorderLine line1, BorderLine line2)
        {
            return (line2 > line1);
        }

        /// <summary>
        /// 添加，hdt
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <returns></returns>
        public static bool operator >=(BorderLine line1, BorderLine line2)
        {
            if ((line1 == null) && (line2 == null))
            {
                return true;
            }
            if (line1 == null)
            {
                return false;
            }
            if (line2 == null)
            {
                return true;
            }
            if (line1.StyleData.Weight == line2.StyleData.Weight)
            {
                return (GetBrightness(line1.Color) <= GetBrightness(line2.Color));
            }
            return (line1.StyleData.Weight >= line2.StyleData.Weight);
        }

        /// <summary>
        /// 添加，hdt
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <returns></returns>
        public static bool operator <=(BorderLine line1, BorderLine line2)
        {
            if ((line1 == null) && (line2 == null))
            {
                return true;
            }
            if (line1 == null)
            {
                return true;
            }
            if (line2 == null)
            {
                return false;
            }
            if (line1.StyleData.Weight == line2.StyleData.Weight)
            {
                return (GetBrightness(line1.Color) >= GetBrightness(line2.Color));
            }
            return (line1.StyleData.Weight <= line2.StyleData.Weight);
        }

        /// <summary>
        /// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method. Instead, if a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute" /> to the class.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" /> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)" /> method.
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
            this._context = null;
            this._color = Colors.Transparent;
            this._themeColor = null;
            this._style = BorderLineStyle.None;
            this._styleData = null;
            while (reader.Read())
            {
                string str;
                if ((reader.NodeType == XmlNodeType.Element) && ((str = reader.Name) != null))
                {
                    if (str != "Type")
                    {
                        if (str == "Color")
                        {
                            goto Label_008E;
                        }
                        if (str == "Theme")
                        {
                            goto Label_00AB;
                        }
                    }
                    else
                    {
                        this._style = (BorderLineStyle)Serializer.DeserializeObj(typeof(BorderLineStyle), reader);
                    }
                }
                continue;
            Label_008E:
                this._color = (Windows.UI.Color)Serializer.DeserializeObj(typeof(Windows.UI.Color), reader);
                continue;
            Label_00AB:
                this._themeColor = (string)(Serializer.DeserializeObj(typeof(string), reader) as string);
            }
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized.</param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            Serializer.InitWriter(writer);
            Serializer.SerializeObj(this._style, "Type", writer);
            if (this._themeColor == null)
            {
                Serializer.SerializeObj(this._color, "Color", writer);
            }
            else
            {
                Serializer.SerializeObj(this._themeColor, "Theme", writer);
            }
        }

        /// <summary>
        /// Gets the border color.
        /// </summary>
        public Windows.UI.Color Color
        {
            get
            {
                if (this._styleData == BorderLineStyleDatas.Empty)
                {
                    return Colors.Transparent;
                }
                if (this._context == null)
                {
                    return this._color;
                }
                if (this._themeColor == null)
                {
                    return this._color;
                }
                return this._context.GetThemeColor(this._themeColor);
            }
        }

        /// <summary>
        /// Gets a border line with a transparent color.
        /// </summary>
        public static BorderLine Empty
        {
            get
            {
                if (_emptyBorder == null)
                {
                    _emptyBorder = new BorderLine(Colors.Transparent, BorderLineStyleDatas.Empty);
                }
                return _emptyBorder;
            }
        }

        internal bool IsBuiltIn
        {
            get { return  this._builtIn; }
        }

        /// <summary>
        /// Indicates whether the border is a built-in gridline.
        /// </summary>
        /// <value>
        /// <c>true</c> if the border is a type of grid line; otherwise, <c>false</c>.
        /// </value>
        public bool IsGridLine
        {
            get { return  (this._styleData == BorderLineStyleDatas.GridLine); }
        }

        /// <summary>
        /// Gets a border line with a transparent color where the border is not drawn.
        /// </summary>
        public static BorderLine NoBorder
        {
            get
            {
                if (_noBorder == null)
                {
                    _noBorder = new BorderLine(Colors.Transparent, BorderLineStyleDatas.NoBorder);
                }
                return _noBorder;
            }
        }

        /// <summary>
        /// Gets the border line style.
        /// </summary>
        public BorderLineStyle Style
        {
            get { return  this._style; }
        }

        /// <summary>
        /// Gets the style data of the border line.
        /// </summary>
        public BorderLineData StyleData
        {
            get
            {
                this.InitStyle();
                return this._styleData;
            }
        }

        /// <summary>
        /// Gets the theme color name of the border.
        /// </summary>
        /// <value>
        /// The color of the theme.
        /// </value>
        public string ThemeColor
        {
            get { return  this._themeColor; }
        }
    }
}

